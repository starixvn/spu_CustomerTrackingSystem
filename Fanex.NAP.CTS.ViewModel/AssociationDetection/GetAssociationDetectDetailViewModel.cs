namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssociationDetectDetailViewModel
    {
        public GetAssociationDetectDetailViewModel() { }

        public GetAssociationDetectDetailViewModel(GetAssociationDetectDetailModel cust)
        {
            this.UserName = cust.ToUsername;
            this.CustID = cust.ToCustID;
            this.CTSCustID = cust.ToCTSCustID;
            
            if (cust.AssociationType != null && cust.AssociationType.ToLower() == "agent")
            {
                this.AssociatedTypes = $"<span class='association-detect-detail-agent' data-fromusername={cust.FromUsername}>Agent </span>";

                var agentInfo = new List<GetAssociationDetectDetailAgentViewModel>
                {
                    new GetAssociationDetectDetailAgentViewModel
                    {
                        UserName = cust.FromUsername,
                        Agent = cust.FromAgentUsername,
                        AssociatedType = cust.AgentAssociationType
                    },
                    new GetAssociationDetectDetailAgentViewModel
                    {
                        UserName = cust.ToUsername,
                        Agent = cust.ToAgentUsername,
                        AssociatedType = cust.AgentAssociationType
                    }
                };

                this.AgentInfoJson = JsonConvert.SerializeObject(agentInfo);
            }
            else
            {
                this.AssociatedTypes = cust.AssociationType;
            }
        }
    
        public string UserName { get; set; }

        public int? CustID { get; set; }

        public long? CTSCustID { get; set; }

        public string AssociatedTypes { get; set; }

        public string AgentInfoJson { get; set; }
    }
}