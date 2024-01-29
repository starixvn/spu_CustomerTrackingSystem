namespace Fanex.NAP.CTS.Model.API
{
    using System.Collections.Generic;
    using System.Linq;

    public class DetectAssociationByTypeViewModel
    {
        public IEnumerable<DetectAssociationByTypeModel> AsssociationByDevice { get; set; }

        public IEnumerable<DetectAssociationByTypeModel> AsssociationIP { get; set; }

        public IEnumerable<DetectAssociationByTypeModel> AsssociationAI { get; set; }

        public IEnumerable<DetectAssociationByTypeModel> GetAssociation()
        {
            var assList = new List<DetectAssociationByTypeModel>();

            if (AsssociationByDevice.Any())
            {
                assList.AddRange(AsssociationByDevice);
            }

            if (AsssociationIP.Any())
            {
                assList.AddRange(AsssociationIP);
            }

            if (AsssociationAI.Any())
            {
                assList.AddRange(AsssociationAI);
            }

            return assList;
        }
    }
}