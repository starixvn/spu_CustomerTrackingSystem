namespace Fanex.NAP.CTS.Model
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TrackingFunctionUsageFilterAttribute : ActionFilterAttribute
    {
        public TrackingCtsFunctionId FunctionId { get; set; }

        public ICurrentSession CurrentSession { get; set; }

        public SystemId SystemId { get; set; }

        public string IsSkipTrackingFuncAttr { get; set; }

        public string AttKey { get; set; }

        public string[] AttValues { get; set; }

        private readonly IMonitorCtsFunctionService _monitorService;

        public TrackingFunctionUsageFilterAttribute(SystemId systemId, TrackingCtsFunctionId functionId, string IsSkipTrackingFuncAttr = "false")
        {
            this._monitorService = new MonitorCtsFunctionService();
            this.CurrentSession = new CurrentSession();
            this.FunctionId = functionId;
            this.SystemId = systemId;
            this.IsSkipTrackingFuncAttr = IsSkipTrackingFuncAttr;
        }
      
        public TrackingFunctionUsageFilterAttribute(SystemId systemId,        
           string attKey,
           params string[] attValues)
        {
            this._monitorService = new MonitorCtsFunctionService();
            this.CurrentSession = new CurrentSession();         
            this.SystemId = systemId;
            if (!string.IsNullOrEmpty(attKey))
            {
                this.AttKey = attKey;
            }            
            this.AttValues = attValues;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (IsSkipTrackingFuncAttr != null && filterContext.ActionParameters.ContainsKey(IsSkipTrackingFuncAttr))
                {
                    var isSkipTracking = filterContext.ActionParameters[IsSkipTrackingFuncAttr] as bool? ?? true;

                    if (isSkipTracking)
                    {
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(this.AttKey) && this.AttValues != null && this.AttValues.Any())
                {
                    InsertStatsAccessmentByOnlyKey(filterContext);
                }
                else if (this.AttValues != null && this.AttValues.Any())
                {
                    InsertStatsAccessmentByOptBoolean(filterContext);
                }
                else
                {
                    InsertStatsAccessment(this.FunctionId);
                }

                base.OnActionExecuting(filterContext);
            }
            catch (Exception ex)
            {
                ex.LogError($"Tracking function usage - Insert Stats Accessment fail - {ex.Message}");
            }
        }

        private void InsertStatsAccessmentByOnlyKey(ActionExecutingContext filterContext)
        {           
            var key = filterContext.Controller.ValueProvider.GetValue(this.AttKey).AttemptedValue;

            var attValues = this.AttValues.Select(s => s.Split('=')).ToDictionary(d => d[0], d => d[1]);

            foreach (var keyValue in attValues)
            {
                if (keyValue.Value == key)
                {
                    Enum.TryParse(keyValue.Key, out TrackingCtsFunctionId newFunctionId);
                    this.FunctionId = newFunctionId;

                    break;
                }
            }

            if (this.FunctionId != default(TrackingCtsFunctionId))
            {
                InsertStatsAccessment(this.FunctionId);
            }
        }

        private void InsertStatsAccessmentByOptBoolean(ActionExecutingContext filterContext)
        {
            var functionIDs = new List<TrackingCtsFunctionId>();                

            var attValues = this.AttValues.Select(s => s.Split(':')).ToDictionary(d => d[0], d => d[1]);

            foreach (var key in attValues)
            {
                var existedKey = filterContext.Controller.ValueProvider.GetValue(key.Key).AttemptedValue;

                if (existedKey != null)
                {                    
                    bool.TryParse(existedKey, out bool newExistedKey);

                    if (newExistedKey)
                    {
                        var arrFunction = key.Value.Split('=');

                        Enum.TryParse(arrFunction[0], out TrackingCtsFunctionId newFunctionId);

                        functionIDs.Add(newFunctionId);
                    }                    
                }
            }

            foreach (var functionID in functionIDs)
            {
                InsertStatsAccessment(functionID);
            }
        }

        private void InsertStatsAccessment(TrackingCtsFunctionId functionID)
        {
            _monitorService.StatsAccessmentInsert(SystemId, functionID, CurrentSession.UserId);
        }
    }
}