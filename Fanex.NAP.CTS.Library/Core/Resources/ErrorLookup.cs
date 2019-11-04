namespace Fanex.NAP.CTS.Library
{
    using Fanex.NAP.CTS.Library.Core.Resources;
    using System.Resources;

    public static class ErrorLookup
    {
        private static readonly ResourceManager resource = new ResourceManager(typeof(Error));

        public static string GetValue(string key)
        {
            return resource.GetString(key);
        }
    }
}