namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using System;
    using System.Collections.Generic;

    public class GetUpdateAssociatedGroupMonitorByIDViewModel
    {
        public GetUpdateAssociatedGroupMonitorByIDViewModel() { }

        public GetUpdateAssociatedGroupMonitorByIDViewModel(GetUpdateAssociatedGroupMonitorByIDModel data)
        {
            this.GroupID = data.GroupID;
            this.GroupName = data.GroupName;
            this.ABI = data.ABI;
            this.IsAllCredit = data.AllCredit;
            this.IsAllLicensee = data.AllLicensee;
            this.Sites = data.Sites;
            this.Remark = data.Remark;
           
            this.AssociatedTypeList = new List<AssociatedTypeViewModel>()
            {
                new AssociatedTypeViewModel
                {
                    TypeID = (int)AssociationType.BettingPattern,
                    TypeName = "Betting Pattern",
                    IsSelected = data.HasAI
                },
                new AssociatedTypeViewModel
                {
                    TypeID = (int)AssociationType.Device,
                    TypeName = "Device",
                    IsSelected = data.HasDevice
                },
                new AssociatedTypeViewModel
                {
                    TypeID = (int)AssociationType.IP,
                    TypeName = "IP",
                    IsSelected = data.HasIP
                },
                new AssociatedTypeViewModel
                {
                    TypeID = (int)AssociationType.Manual,
                    TypeName = "Manual",
                    IsSelected = data.HasManual
                }
            };           
        }

        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public int ABI { get; set; }

        public bool IsAllCredit { get; set; }

        public bool IsAllLicensee { get; set; }

        public string Sites { get; set; }       

        public List<AssociatedTypeViewModel> AssociatedTypeList { get; set; }

        public string Remark { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Modified { get; set; }

        public string ModifiedBy { get; set; }
    }
}
