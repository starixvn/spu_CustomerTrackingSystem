namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Criteria.CTSUser;
    using Fanex.NAP.CTS.Resource;
    using System;

    public class CTSUserService : ICTSUserService
    {
        public void InsertCTSUser(int userId, string userName)
        {
            var criteria = new InsertCTSUserCriteria
            {
                 UserId = userId,
                 UserName = userName
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    db.ExecuteNonQuery(criteria);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(ResourcesManager.GetValue(ConstsResources.INSERT_CTS_USER_ERROR), userId, userName);
                LogExtension.LogError(errorMessage, ex);
            }
        }
    }
}