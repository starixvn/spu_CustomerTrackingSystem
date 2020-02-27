namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;   
    using System.Collections.Generic;
    using System.Linq;

    public class GraphService : IGraphService
    {
        private const string Node_Label_CTSCustomer = "CTSCustomer";

        public GraphService() { }

        public GraphCustomerItemViewModel GetGraphCustomerItemByShortestPathOfAccount(ShortestPathOfAccount shortestPathOfAccount)
        {
            var graphCustomer = new GraphCustomerItemViewModel();            
            var path = shortestPathOfAccount.Path;
            if (path != null)
            {
                var nodeStartId = int.Parse(path.Start.Id);
                graphCustomer = GenerateCustomerItem(path.Relationships.ToList(), path.Nodes.ToList(), nodeStartId, RelationshipNodeType.Start, string.Empty);
            }

            return graphCustomer;
        }     

        public GraphCustomerItemViewModel GenerateCustomerItem(
            List<Relationship> relationshipList, 
            List<Node> nodeList, 
            int nodeId, 
            RelationshipNodeType relationshipNodeType,
            string associationType)
        {
            var customerItem = new GraphCustomerItemViewModel();
            var relationshipItem = new Relationship();           
            var nodeItem = GetNode(relationshipList, nodeList, nodeId, relationshipNodeType, out relationshipItem);
            if (nodeItem != null)
            {
                customerItem = GenerateCustomerItem(nodeItem, associationType);

                if (relationshipItem != null)
                {
                    var nextRelationshipNodeType = relationshipItem.StartNodeId.ToString() == nodeItem.Id
                            ? RelationshipNodeType.End
                            : RelationshipNodeType.Start;
                    var nextNodeId = nextRelationshipNodeType == RelationshipNodeType.Start
                            ? relationshipItem.StartNodeId
                            : relationshipItem.EndNodeId;
                    customerItem.children = new List<GraphCustomerItemViewModel>
                                {
                                    GenerateCustomerItem(relationshipList, nodeList, nextNodeId, nextRelationshipNodeType, relationshipItem.Type)
                                }.ToArray();
                }              
            }

            return customerItem;
        }

        public Node GetNode(List<Relationship> relationshipList, 
            List<Node> nodeList, 
            int nodeId, 
            RelationshipNodeType relationshipNodeType,
            out Relationship relationshipItem)
        {
            var nodeItem = new Node();
            relationshipItem = new Relationship();
            nodeItem = GetNode(nodeList, nodeId.ToString());
            if (nodeItem != null)
            {
                nodeList.Remove(nodeItem);
                relationshipItem = GetRelationship(relationshipList, nodeId, relationshipNodeType);
                if (relationshipItem != null)
                {
                    relationshipList.Remove(relationshipItem);
                    if (nodeItem.Labels[0] != Node_Label_CTSCustomer)
                    {
                        var nextRelationshipNodeType = relationshipNodeType == RelationshipNodeType.Start
                                ? RelationshipNodeType.End
                                : RelationshipNodeType.Start;
                        var nextNodeId = relationshipItem.StartNodeId;
                        nodeItem = GetNode(relationshipList, nodeList, nextNodeId, nextRelationshipNodeType, out relationshipItem);
                    }
                }
            }
            
            return nodeItem;
        }

        public Relationship GetRelationship(List<Relationship> relationshipList, int nodeId, RelationshipNodeType relationshipNodeType)
        {
            foreach (var item in relationshipList)
            {
                if (relationshipNodeType == RelationshipNodeType.Start && item.StartNodeId == nodeId)
                    return item;
                else if (relationshipNodeType == RelationshipNodeType.End && item.EndNodeId == nodeId)
                    return item;
            }

            return null;
        }        

        public Node GetNode(List<Node> nodeList, string nodeId)
        {
            foreach (var item in nodeList)
            {
                if (item.Id == nodeId)
                    return item;
            }

            return null;
        }

        public GraphCustomerItemViewModel GenerateCustomerItem(Node node, string associationType)
            => new GraphCustomerItemViewModel
            {
                CTSCustId = node.Properties.CustId,
                ItemCode = node.Properties.CustId.ToString(),
                Username = node.Properties.Username,
                DisplayName = !string.IsNullOrWhiteSpace(node.Properties.Username) ? node.Properties.Username : node.Properties.Username2,
                AssociationTypeName = associationType
            };
        
    }
}