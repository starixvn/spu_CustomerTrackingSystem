namespace Fanex.NAP.CTS.ViewModel.API.Http
{
    using System.Net;

    public class HttpResponseViewModel<T>
    {
        public HttpStatusCode statusCode { get; set; }

        public string errorMessage { get; set; }

        public T detail { get; set; }
    }
}
