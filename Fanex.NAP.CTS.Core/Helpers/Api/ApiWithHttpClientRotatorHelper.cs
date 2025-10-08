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
        #region Post
        public static async Task<TResult> Post<TResult>(string url, object param, double timeOutbySeconds = 120, CancellationToken cancellationToken = default)
        {
            var web = HttpClientRotator.GetClient();

            using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeOutbySeconds)))
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
            {
                var content = JsonConvert.SerializeObject(param);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response;
                try
                {
                    response = await web.PostAsync(url, byteContent, linkedCts.Token);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    throw new TimeoutException($"Request to {url} timed out after {timeOutbySeconds} seconds.");
                }
                catch (OperationCanceledException)
                {
                    throw new TaskCanceledException($"Request to {url} was canceled by caller.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API error ({response.StatusCode}): {error}");
                }

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }
        #endregion

        #region PostWithStreamWritter
        public static async Task<TResult> PostWithStreamWritter<TResult>(string url, object param, double timeOutbySeconds = 120)
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
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return content;
        }

        private static void WriteEscapedJsonStringFromStringBuilder(StreamWriter writer, StringBuilder sb, int bufferSize = 3 * 1024) // chunk 3MB
        {
            char[] buffer = new char[bufferSize];

            writer.Write('"');

            int totalLength = sb.Length;
            for (int offset = 0; offset < totalLength; offset += bufferSize)
            {
                int length = Math.Min(bufferSize, totalLength - offset);
                sb.CopyTo(offset, buffer, 0, length);

                var text = new string(buffer)
                             .Replace("\"", "\\\"")
                             .Replace("\\", "\\\\")
                             .Replace("\b", "\\b")
                             .Replace("\f", "\\f")
                             .Replace("\n", "\\n")
                             .Replace("\r", "\\r")
                             .Replace("\t", "\\t");

                writer.Write(text);
            }

            writer.Write('"');
        }

        private static void WriteEscapedJsonString(StreamWriter writer, string value)
        {
            writer.Write('"');
            var text = value
                             .Replace("\"", "\\\"")
                             .Replace("\\", "\\\\")
                             .Replace("\b", "\\b")
                             .Replace("\f", "\\f")
                             .Replace("\n", "\\n")
                             .Replace("\r", "\\r")
                             .Replace("\t", "\\t");

            writer.Write(text);
            writer.Write('"');
        }
        #endregion
    }
}
