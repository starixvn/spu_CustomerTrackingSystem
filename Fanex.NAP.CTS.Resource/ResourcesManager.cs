namespace Fanex.NAP.CTS.Resource
{
    using System.Resources;

    public static class ResourcesManager
    {
        private static readonly ResourceManager resource = new ResourceManager(typeof(Resources));

        public static string GetValue(string key)
        {
            return resource.GetString(key);
        }
    }
}