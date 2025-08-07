namespace Fanex.NAP.CTS.Core.Helpers.Api
{
    using MailDelivery.Client;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class ApiHelper
    {
        [Obsolete("Too many generic and parameter")]
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param, int environment, int mailingListId)
        {
            try
            {
                using (var web = new HttpClient())
                {
                    var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        return default(TResult);
                    }

                    return await response.Content.ReadAsAsync<TResult>().ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                var newMail = new MailDelivery.Core.Models.Mail
                {
                    Subject = string.Format("[({0}) SPU FS] ", environment == 1 ? "LOCAL" : environment == 2 ? "UAT" : "PRO") + " Error Calling FPS Middle Service's API",
                    Title = "SPU Fraud Sensor",
                    Body = "Error when calling API from FPS Middle Service \n API url: " + url + " \n Params: " + param + " \n Error: " + exception,
                    MailingListId = mailingListId
                };

                MailClientHandler.SendFunctionMail(newMail);

                return default(TResult);
            }
        }

        public static async Task<TResult> Post<TResult>(string url, object param, double timeOutbySeconds = 120)
        {
            using (var web = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(param);

                var buffer = Encoding.UTF8.GetBytes(content);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                web.Timeout = TimeSpan.FromSeconds(timeOutbySeconds);

                var response = await web.PostAsync(url, byteContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public static async Task<TResult> PostWithOutMemoryParseJson<TResult>(string url, object param, double timeOutbySeconds = 120)
        {
            using (var web = new HttpClient())
            {
                var content = CreateStreamContent(param);

                web.Timeout = TimeSpan.FromSeconds(timeOutbySeconds);

                var response = await web.PostAsync(url, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

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

        private static void WriteEscapedJsonStringFromStringBuilder(StreamWriter writer, StringBuilder sb)
        {
            writer.Write('"');

            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                switch (c)
                {
                    case '"': writer.Write("\\\""); break;
                    case '\\': writer.Write("\\\\"); break;
                    case '\b': writer.Write("\\b"); break;
                    case '\f': writer.Write("\\f"); break;
                    case '\n': writer.Write("\\n"); break;
                    case '\r': writer.Write("\\r"); break;
                    case '\t': writer.Write("\\t"); break;
                    default:
                        if (char.IsControl(c))
                        {
                            writer.Write($"\\u{((int)c):x4}");
                        }
                        else
                        {
                            writer.Write(c);
                        }
                        break;
                }
            }

            writer.Write('"');
        }

        private static void WriteEscapedJsonString(StreamWriter writer, string value)
        {
            writer.Write('"');
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': writer.Write("\\\""); break;
                    case '\\': writer.Write("\\\\"); break;
                    case '\b': writer.Write("\\b"); break;
                    case '\f': writer.Write("\\f"); break;
                    case '\n': writer.Write("\\n"); break;
                    case '\r': writer.Write("\\r"); break;
                    case '\t': writer.Write("\\t"); break;
                    default:
                        if (char.IsControl(c))
                        {
                            writer.Write($"\\u{((int)c):x4}");
                        }
                        else
                        {
                            writer.Write(c);
                        }
                        break;
                }
            }
            writer.Write('"');
        }

        public static async Task<TResult> PostWithProxy<TResult>(string url, object param)
        {
            var proxy = new WebProxy
            {
                Address = new Uri($"http://10.3.252.100:8801")
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };

            using (var web = new HttpClient(handler: httpClientHandler, disposeHandler: true))
            {
                var content = JsonConvert.SerializeObject(param);

                var buffer = Encoding.UTF8.GetBytes(content);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await web.PostAsync(url, byteContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }

        public static async Task Post(string url, object param)
        {
            using (var web = new HttpClient())
            {
                await web.PostAsJsonAsync(url, param).ConfigureAwait(false);
            }
        }

        public static async Task<TResult> Get<TResult>(string url)
        {
            using (var web = new HttpClient())
            {
                var response = await web.GetAsync(url).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TResult>(result);
            }
        }
    }
}