namespace Fanex.NAP.CTS.Test.Fake
{
    using Fanex.NAP.Core.Web.Session;

    public static class FakeSession
    {
        private static CurrentSession currentSession;

        public static CurrentSession CurrentSession
        {
            get
            {
                currentSession = currentSession ?? new CurrentSession
                {
                    UserId = 8,
                    Username = "sa"
                };

                return currentSession;
            }        
        }
    }
}