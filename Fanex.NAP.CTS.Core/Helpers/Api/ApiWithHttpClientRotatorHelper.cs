namespace Fanex.NAP.CTS.Core.Helpers.Api
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ApiWithHttpClientRotatorHelper
    {
        private const string MEDIA_JSON_TYPE = "application/json";
        private const string MEDIA_XNDJSON_TYPE = "application/x-ndjson";

        #region Post
        public static async Task<TResult> PostAsync<TResult>(
            string url,
            object param,
            double timeOutbySeconds = 120,
            CancellationToken? cancellationToken = null,
            int maxRetryCount = 3,
            int retryDelayMs = 2000)
        {
            using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutbySeconds)))
            using (var linkedCts = cancellationToken.HasValue ?
                        CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken.Value) :
                        timeoutCts)
            {
                var json = JsonConvert.SerializeObject(param);
                using (var content = new StringContent(json, Encoding.UTF8, MEDIA_JSON_TYPE))
                {
                    return await HandleExceptionWhileExecutingAsync(async () =>
                    {
                        var client = HttpClientRotator.GetClient();

                        var response = await client.PostAsync(url, content, linkedCts.Token).ConfigureAwait(false);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new HttpRequestException($"API error ({(int)response.StatusCode} {response.ReasonPhrase}): {error}");
                        }

                        try
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            using (var sr = new StreamReader(stream))
                            using (var reader = new JsonTextReader(sr))
                            {
                                var serializer = new JsonSerializer();
                                var result = serializer.Deserialize<TResult>(reader);

                                return result;
                            }
                        }
                        catch (JsonException ex)
                        {
                            throw new InvalidOperationException($"Failed to deserialize response from {url}.", ex);
                        }
                    },
                    url,
                    timeOutbySeconds,
                    linkedCts.Token,
                    maxRetryCount,
                    retryDelayMs).ConfigureAwait(false);
                }
            }
        }
        #endregion

        #region PostWithConttent
        public static async Task<TResult> PostWithContentAsync<TResult>(
            string url,
            Func<HttpContent> createContent,
            double timeOutbySeconds = 120,
            int maxRetryCount = 3,
            int retryDelayMs = 2000)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutbySeconds)))
            {

                return await HandleExceptionWhileExecutingAsync(async () =>
                {
                    var httpClient = HttpClientRotator.GetClient();

                    using (var httpContent = createContent()) // create per request, if not will did not data in next request
                    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        request.Content = httpContent;
                        request.Headers.TransferEncodingChunked = true;

                        var response = await httpClient.SendAsync(
                            request,
                            HttpCompletionOption.ResponseHeadersRead,
                            cts.Token);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new Exception(error);
                        }

                        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return JsonConvert.DeserializeObject<TResult>(result);
                    }
                },
                url,
                timeOutbySeconds,
                cts.Token,
                maxRetryCount,
                retryDelayMs).ConfigureAwait(false);
            }
        }
        #endregion

        #region Get Server Sent Event
        public static async Task GetServerSentEventAsync(
            string url,
            Action<string> onMessage,
            int timeOutbySeconds = 120,
            int idleTimeoutSeconds = 60,
            CancellationToken? cancellationToken = null,
            int maxRetryCount = 3,
            int retryDelayMs = 2000)
        {
            using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutbySeconds)))
            using (var linkedCts = cancellationToken.HasValue ?
                        CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken.Value) :
                        timeoutCts)
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                await HandleExceptionWhileExecutingAsync(async () =>
                {
                    var client = HttpClientRotator.GetClient();

                    request.Headers.Accept.ParseAdd(MEDIA_XNDJSON_TYPE);

                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, linkedCts.Token);
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    var reader = new StreamReader(stream);
                    linkedCts.Token.Register(() =>
                    {
                        try
                        {
                            stream.Close();
                            response.Dispose();
                        }
                        catch { }
                    });

                    string dataBuffer = "";
                    var lastMessageTime = DateTime.UtcNow;

                    while (!linkedCts.Token.IsCancellationRequested)
                    {
                        if (!stream.CanRead) break;

                        if ((DateTime.UtcNow - lastMessageTime).TotalSeconds > idleTimeoutSeconds)
                        {
                            throw new TimeoutException($"No SSE message received for {idleTimeoutSeconds} seconds. Triggering retry...");
                        }

                        var line = await reader.ReadLineAsync().ConfigureAwait(false);
                        if (line == null) break;

                        if (string.IsNullOrWhiteSpace(line))
                        {
                            if (!string.IsNullOrEmpty(dataBuffer))
                            {
                                var message = dataBuffer.Trim();
                                dataBuffer = string.Empty;
                            }
                            continue;
                        }

                        // SSE event: "data: ..."
                        if (line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                        {
                            dataBuffer += line.Substring(5).Trim() + "\n";
                            lastMessageTime = DateTime.UtcNow;
                        }
                        else if (line.StartsWith("{") || line.StartsWith("["))
                        {
                            onMessage?.Invoke(line.Trim());
                            lastMessageTime = DateTime.UtcNow;
                        }
                    }

                    return Task.CompletedTask;
                },
                url,
                timeOutbySeconds,
                linkedCts.Token,
                maxRetryCount,
                retryDelayMs).ConfigureAwait(false);
            }
        }
        #endregion

        #region private method
        private static async Task<T> HandleExceptionWhileExecutingAsync<T>(
            Func<Task<T>> func,
            string url,
            double timeOutbySeconds,
            CancellationToken cancellationToken,
            int maxRetryCount = 3,
            int retryDelayMs = 2000)
        {
            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return await func();
                }
                catch (HttpRequestException httpRequestException)
                {
                    string innerMsg = httpRequestException.InnerException?.Message ?? string.Empty;

                    bool isTransientConnectionIssue =
                        innerMsg.Contains("connection was closed unexpectedly") ||
                        innerMsg.Contains("expected to be kept alive was closed by the server") ||
                        httpRequestException.Message.Contains("503") ||
                        httpRequestException.Message.Contains("Service Unavailable");

                    if (isTransientConnectionIssue && retryAttempt < maxRetryCount)
                    {
                        retryAttempt++;

                        await Task.Delay(retryDelayMs, cancellationToken);
                        continue; // Retry the request
                    }

                    LogExtension.LogInfo($"Retry {url}: {retryAttempt}/{maxRetryCount} by Reason ExceptMsg: {httpRequestException?.Message ?? "null"} InnerMsg: {innerMsg}");

                    throw; // Re-throw if max retries reached or not a transient issue
                }
                catch (TimeoutException ex)
                {
                    var msg = ex?.Message ?? string.Empty;
                    if (msg.Contains("Triggering retry...") && retryAttempt < maxRetryCount)
                    {
                        retryAttempt++;

                        await Task.Delay(retryDelayMs, cancellationToken);
                        continue; // Retry the request
                    }

                    LogExtension.LogInfo($"Retry {url}: {retryAttempt}/{maxRetryCount} by Reason ExceptMsg: {ex?.Message ?? "null"}");

                    throw; // Re-throw if max retries reached or not a transient issue
                }
                catch (IOException ex)
                {
                    if (IsCancellationRequestedToken(cancellationToken))
                    {
                        throw new TaskCanceledException($"Request to {url} was canceled by caller.", ex);
                    }

                    throw;
                }
                catch (OperationCanceledException ex)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException($"Request to {url} timed out after {timeOutbySeconds} seconds.", ex);
                    }

                    if (IsCancellationRequestedToken(cancellationToken))
                    {
                        throw new TaskCanceledException($"Request to {url} was canceled by caller.", ex);
                    }

                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static bool IsCancellationRequestedToken(CancellationToken? cancellationToken)
        {
            return cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested;
        }
        #endregion
    }
}
