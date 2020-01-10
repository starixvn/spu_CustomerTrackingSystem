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

        public ShortestPathOfAccount GenerateSampleShortestPathOfAccount()
        {
            string[] lblCustomer = { "CTSCustomer" };
            string[] lblDevice = { "Device" };
            var shortestPath = new ShortestPathOfAccount();
            shortestPath.Path = new ShortestPath();
            shortestPath.Path.Start = new Node
            {
                Id = "9977105",
                Labels = lblCustomer,
                Properties = new GraphCustomer
                {
                    CustId = 23081993,
                    Username = null,
                    Username2 = null,
                    RoleId = 0
                }
            };
            shortestPath.Path.End = new Node
            {
                Id = "9977108",
                Labels = lblCustomer,
                Properties = new GraphCustomer
                {
                    CustId = 23081995,
                    Username = null,
                    Username2 = null,
                    RoleId = 0
                }
            };

            var nodes = new List<Node>();            
            nodes.Add(new Node
            {
                Id = "9977105",
                Labels = lblCustomer,
                Properties = new GraphCustomer
                {
                    CustId = 23081993,
                    Username = "Member001"
                }
            });
            nodes.Add(new Node
            {
                Id = "9977106",
                Labels = lblDevice,
                Properties = new GraphCustomer
                {
                    CustId = 100,
                    Username = "Device"                   
                }
            });
            nodes.Add(new Node
            {
                Id = "9977126",
                Labels = lblCustomer,
                Properties = new GraphCustomer
                {
                    CustId = 23081994,
                    Username = "Member002"                    
                }
            });
            nodes.Add(new Node
            {
                Id = "9977107",
                Labels = lblDevice,
                Properties = new GraphCustomer
                {
                    CustId = 101,
                    Username = "Device"                  
                }
            });
            nodes.Add(new Node
            {
                Id = "9977108",
                Labels = lblCustomer,
                Properties = new GraphCustomer
                {
                    CustId = 23081995,
                    Username = "Member003"                 
                }
            });

            shortestPath.Path.Nodes = nodes.ToArray();
            var relationShips = new List<Relationship>();
            relationShips.Add(new Relationship
            {
                StartNodeId = 9977105,
                EndNodeId = 9977106,
                Type = "USED_DEVICE"
            });
            relationShips.Add(new Relationship
            {
                StartNodeId = 9977126,
                EndNodeId = 9977106,
                Type = "USED_DEVICE"
            });
            relationShips.Add(new Relationship
            {
                StartNodeId = 9977126,
                EndNodeId = 9977107,
                Type = "USED_IP"
            });
            relationShips.Add(new Relationship
            {
                StartNodeId = 9977108,
                EndNodeId = 9977107,
                Type = "USED_IP"
            });
            shortestPath.Path.Relationships = relationShips.ToArray();

            return shortestPath;
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
                CustId = node.Properties.CustId,
                ItemCode = node.Properties.CustId.ToString(),
                Username = node.Properties.Username,
                DisplayName = node.Properties.Username,
                AssociationTypeName = associationType
            };
        
    }
}