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
            this.TransitiveUsername = cust.TransitiveUsername;

            if (!string.IsNullOrEmpty(cust.AgentAssociationType1) || !string.IsNullOrEmpty(cust.AgentAssociationType2))
            {
                this.AssociatedTypes = $"<span class='association-detect-detail-agent' data-fromusername={cust.FromUsername}>Agent </span>";               

                var agentInfos = new List<GetAssociationDetectDetailAgentViewModel>();
                var agentInfo1 = new GetAssociationDetectDetailAgentViewModel
                {
                    UserName = cust.FromUsername,
                    AgentName = cust.FromAgentUsername,
                    TransitiveUsername = cust.TransitiveUsername != null ? cust.TransitiveUsername : "-",
                    AgentAssociationTypes = cust.AgentAssociationType1
                };
                
                var agentInfo2 = new GetAssociationDetectDetailAgentViewModel
                {
                    UserName = cust.ToUsername,
                    AgentName = cust.ToAgentUsername,                    
                    TransitiveUsername = cust.TransitiveUsername != null ? cust.TransitiveUsername : "-",
                    AgentAssociationTypes = cust.AgentAssociationType1
                };

                if (!string.IsNullOrEmpty(cust.AgentAssociationType2))
                {
                    agentInfo1.AgentAssociationTypes += " / " + cust.AgentAssociationType2;
                    agentInfo2.AgentAssociationTypes += " / " + cust.AgentAssociationType2;
                }

                agentInfos.Add(agentInfo1);
                agentInfos.Add(agentInfo2);

                this.AgentInfoJson = JsonConvert.SerializeObject(agentInfos);
            }
            else
            {
                this.AssociatedTypes = cust.AssociationType1;

                if (!string.IsNullOrEmpty(cust.AssociationType2))
                {
                    this.AssociatedTypes += " / " + cust.AssociationType2;
                }
            }
        }
    
        public string UserName { get; set; }

        public int? CustID { get; set; }

        public long? CTSCustID { get; set; }

        public string TransitiveUsername { get; set; }

        public string AssociatedTypes { get; set; }

        public string AgentInfoJson { get; set; }
    }
}