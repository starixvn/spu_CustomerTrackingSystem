namespace Fanex.NAP.CTS.Core.Helpers.Api
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ApiWithHttpClientRotatorHelper
    {
        private const string MEDIA_JSON_TYPE = "application/json";
        #region Post
        public static async Task<TResult> PostAsync<TResult>(string url, object param, double timeOutbySeconds = 120, CancellationToken? cancellationToken = null)
        {
            var client = HttpClientRotator.GetClient();

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
                        var response = await client.PostAsync(url, content, linkedCts.Token).ConfigureAwait(false);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new HttpRequestException($"API error ({(int)response.StatusCode} {response.ReasonPhrase}): {error}");
                        }

                        var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        try
                        {
                            return JsonConvert.DeserializeObject<TResult>(resultString);
                        }
                        catch (JsonException ex)
                        {
                            throw new InvalidOperationException($"Failed to deserialize response from {url}. Content: {resultString}", ex);
                        }
                    }, url, timeOutbySeconds, linkedCts.Token);
                }
            }
        }
        #endregion

        #region PostWithStreamWritter
        public static async Task<TResult> PostWithStreamWritterAsync<TResult>(string url, object param, double timeOutbySeconds = 120)
        {
            var web = HttpClientRotator.GetClient();

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutbySeconds)))
            {
                var content = CreateStreamContent(param);

                var response = await web.PostAsync(url, content, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                var result = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public static StreamContent CreateStreamContent(object model)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
            {
                writer.Write("{");

                var props = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                bool first = true;

                foreach (var prop in props)
                {
                    var value = prop.GetValue(model);
                    if (value == null)
                    {
                        continue;
                    }

                    if (!first)
                    {
                        writer.Write(",");
                    }
                    first = false;

                    writer.Write($"\"{prop.Name}\":");

                    switch (value)
                    {
                        case string str:
                            WriteEscapedJsonString(writer, str);
                            break;
                        case int i:
                            writer.Write(i.ToString());
                            break;
                        case StringBuilder sb:
                            WriteEscapedJsonStringFromStringBuilder(writer, sb);
                            break;
                        default:
                            writer.Write($"\"{value.ToString()}\""); // fallback
                            break;
                    }
                }

                writer.Write("}");
                writer.Flush();
            }

            stream.Position = 0;
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(MEDIA_JSON_TYPE);

            return content;
        }

        private static void WriteEscapedJsonStringFromStringBuilder(StreamWriter writer, StringBuilder sb, int bufferSize = 10 * 1024 * 1024) // chunk 10MB
        {
            char[] buffer = new char[bufferSize];

            writer.Write('"');

            int totalLength = sb.Length;
            for (int offset = 0; offset < totalLength; offset += bufferSize)
            {
                int length = Math.Min(bufferSize, totalLength - offset);
                sb.CopyTo(offset, buffer, 0, length);

                var text = new string(buffer, 0, length).EscapeJsonString();

                writer.Write(text);
            }

            writer.Write('"');
        }

        private static void WriteEscapedJsonString(StreamWriter writer, string value)
        {
            writer.Write('"');
            var text = value.EscapeJsonString();

            writer.Write(text);
            writer.Write('"');
        }

        private static string EscapeJsonString(this string val)
        {
            return val
                    .Replace("\"", "\\\"")
                    .Replace("\\", "\\\\")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
        }
        #endregion

        #region Get Server Sent Event
        public static async Task GetServerSentEventAsync(
            string url,
            Action<string> onMessage,
            int timeOutbySeconds = 120,
            CancellationToken? cancellationToken = null)
        {

            var client = HttpClientRotator.GetClient();

            using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutbySeconds)))
            using (var linkedCts = cancellationToken.HasValue ?
                        CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken.Value) :
                        timeoutCts)
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                await HandleExceptionWhileExecutingAsync(async () =>
                {
                    request.Headers.Accept.ParseAdd("application/x-ndjson");

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

                    while (!linkedCts.Token.IsCancellationRequested)
                    {
                        if (!stream.CanRead) break;

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
                        }
                        else if (line.StartsWith("{") || line.StartsWith("["))
                        {
                                onMessage?.Invoke(line.Trim());
                        }
                    }

                    return Task.CompletedTask;
                }, url, timeOutbySeconds, linkedCts.Token).ConfigureAwait(false);
            }
        }
        #endregion

        #region private method
        private static async Task<T> HandleExceptionWhileExecutingAsync<T>(Func<Task<T>> func, string url, double timeOutbySeconds, CancellationToken cancellationToken)
        {
            try
            {
                return await func();
            }
            catch (IOException ex)
            {
                if (IsCancellationRequestedToken(cancellationToken))
                {
                    throw new TaskCanceledException($"Request to {url} was canceled by caller.", ex);
                }

                throw ex; // Re-throw unknown cancellation
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

                throw ex; // Re-throw unknown cancellation
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool IsCancellationRequestedToken(CancellationToken? cancellationToken)
        {
            return cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested;
        }
        #endregion
    }
}
