namespace Fanex.NAP.CTS.Core.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Resource;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Routing;

    public class CriteriaService : ICriteriaService
    {
        private readonly IDynamicRepository _repository;

        public CriteriaService()
        {
            _repository = new DynamicRepository();
        }

        public T Get<T>(ICriteria criteria)
        {
            return _repository.Get<T>(criteria);
        }

        public int Execute(INonQueryCommand command)
        {
            return _repository.Execute(command);
        }

        public IEnumerable<T> Fetch<T>(ICriteria criteria)
        {
            return _repository.Fetch<T>(criteria);
        }

        public (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet) Fetch<TFirst, TSecond>(ICriteria criteria)
        {
            return _repository.FetchMultiple<TFirst, TSecond>(criteria);
        }

        public (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet, IEnumerable<TThird> thirdResultSet) Fetch<TFirst, TSecond, TThird>(ICriteria criteria)
        {
            return _repository.FetchMultiple<TFirst, TSecond, TThird>(criteria);
        }

        public DBResult UpdateData(CommandBase criteria, string prefix)
        {
            _repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = criteria.ErrCode,
                ErrorMsg = ErrorLookup.GetValue(prefix + "_" + criteria.ErrCode),
                OutParam = criteria.GetOutParam()
            };
        }

        #region LogFanexAPI
        public (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet, IEnumerable<TThird> thirdResultSet) FetchWithLogFanexAPI<TFirst, TSecond, TThird>(ICriteria criteria, string serviceName)
        {
            var startTime = DateTime.Now;
            try
            {
                (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet, IEnumerable<TThird> thirdResultSet) =
                Fetch<TFirst, TSecond, TThird>(criteria);
                var endTime = (DateTime.Now - startTime).TotalMilliseconds;
                RunTaskLogDurationFanexAPI(HttpContext.Current.Request.RequestContext.RouteData, GetRequestId(), true, serviceName, (long)endTime);

                return (firstResultSet, secondResultSet, thirdResultSet);
            }
            catch (Exception ex)
            {
                var endTime = (DateTime.Now - startTime).TotalMilliseconds;
                RunTaskLogDurationFanexAPI(HttpContext.Current.Request.RequestContext.RouteData, GetRequestId(), false, serviceName, (long)endTime);

                throw ex;
            }
        }

        public (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet) FetchWithLogFanexAPI<TFirst, TSecond>(ICriteria criteria, string serviceName)
        {
            var startTime = DateTime.Now;
            try
            {
                (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet) = Fetch<TFirst, TSecond>(criteria);
                var endTime = (DateTime.Now - startTime).TotalMilliseconds;
                RunTaskLogDurationFanexAPI(HttpContext.Current.Request.RequestContext.RouteData, GetRequestId(), true, serviceName, (long)endTime);

                return (firstResultSet, secondResultSet);
            }
            catch (Exception ex)
            {
                var endTime = (DateTime.Now - startTime).TotalMilliseconds;
                RunTaskLogDurationFanexAPI(HttpContext.Current.Request.RequestContext.RouteData, GetRequestId(), false, serviceName, (long)endTime);

                throw ex;
            }
        }

        public IEnumerable<T> FetchWithLogFanexAPI<T>(ICriteria criteria, string serviceName)
        {
            var startTime = DateTime.Now;
            try
            {
                var result = Fetch<T>(criteria);
                var endTime = (DateTime.Now - startTime).TotalMilliseconds;
                RunTaskLogDurationFanexAPI(HttpContext.Current.Request.RequestContext.RouteData, GetRequestId(), true, serviceName, (long)endTime);

                return result;
            }
            catch (Exception ex)
            {
                var endTime = (DateTime.Now - startTime).TotalMilliseconds;
                RunTaskLogDurationFanexAPI(HttpContext.Current.Request.RequestContext.RouteData, GetRequestId(), false, serviceName, (long)endTime);

                throw ex;
            }
        }

        private string GetCallerControllerAndAction(RouteData routeData)
        {
            var controller = routeData.Values["controller"]?.ToString();
            var action = routeData.Values["action"]?.ToString();

            return string.Format($"{controller}.{action}");
        }

        private void RunTaskLogDurationFanexAPI(RouteData routeData, string requestId, bool isSuccess, string serviceName, long endTime)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    var functionName = GetCallerControllerAndAction(routeData);

                    var parameters = new
                    {
                        functionName = functionName,
                        serviceName = serviceName,
                        durationFanexApi = endTime,
                        uid = requestId,
                        isSuccess = isSuccess
                    };

                    var result = ApiHelper.Post<bool>(ConfigSettings.LogDurationFanexAPIUrl, parameters);

                    var response = result.Result;
                    if (!response)
                    {
                        throw new Exception(JsonConvert.SerializeObject(parameters));
                    }
                }
                catch (Exception ex)
                {
                    LogExtension.LogError("Log Duration FanexAPI failed: ", ex);
                }
            });
        }

        private const string Key = "RequestUniqueId";

        private static string GetRequestId()
        {
            if (HttpContext.Current.Items[Key] == null)
            {
                HttpContext.Current.Items[Key] = Guid.NewGuid().ToString();
            }
            return HttpContext.Current.Items[Key].ToString();
        }
        #endregion
    }
}
