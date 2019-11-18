namespace Fanex.NAP.CTS.Test.Fake
{
    using Fanex.NAP.Core.Web.Session;

    public static class FakeSession
    {
        public static CurrentSession CurrentSession
        {
            get
            {
                return new CurrentSession
                {
                    UserId = 8,
                    Username = "sa"
                };
            }
        }
    }
}