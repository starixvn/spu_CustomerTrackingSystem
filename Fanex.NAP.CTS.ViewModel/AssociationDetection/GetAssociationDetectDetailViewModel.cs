namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class GetAssociationDetectDetailViewModel
    {
        public GetAssociationDetectDetailViewModel() { }
      
        public GetAssociationDetectDetailViewModel(IEnumerable<GetAssociationDetectDetailModel> custs)
        {
            var cust = custs.FirstOrDefault();
            var assAgents = custs.FirstOrDefault(x=> !string.IsNullOrEmpty(x.AgentAssociationType));

            if (cust != null)
            {
                this.UserName = cust.ToUsername;
                this.CustID = cust.ToCustID;
                this.CTSCustID = cust.ToCTSCustID;
                this.AssociatedTypes = cust.AssociationType;
            }

            if (assAgents != null)
            {
                var assNoAgents = custs.Where(x => string.IsNullOrEmpty(x.AgentAssociationType)).Select(x => x);

                this.AssociatedTypes = $"<span class='association-detect-detail-agent' data-fromusername={assAgents.FromUsername}>Agent </span>"
                    + string.Join(", ", assNoAgents.Select(x => x.AssociationType));
                 
                var agentInfo = new List<GetAssociationDetectDetailAgentViewModel>
                {
                    new GetAssociationDetectDetailAgentViewModel
                    {
                        UserName = assAgents.FromUsername,
                        Agent = assAgents.FromAgentUsername,
                        AssociatedType = assAgents.AgentAssociationType
                    },
                    new GetAssociationDetectDetailAgentViewModel
                    {
                        UserName = assAgents.ToUsername,
                        Agent = assAgents.ToAgentUsername,
                        AssociatedType = assAgents.AgentAssociationType
                    }
                };

                this.AgentInfoJson = JsonConvert.SerializeObject(agentInfo);
            }
        }

        public string UserName { get; set; }

        public int CustID { get; set; }

        public long CTSCustID { get; set; }

        public string AssociatedTypes { get; set; }

        public string AgentInfoJson { get; set; }
    }
}