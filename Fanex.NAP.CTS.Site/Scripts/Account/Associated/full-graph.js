function full_graph() {
    popoto.rest.CYPHER_URL = "https://db-kh9ct9ai1mqn6hz2itry.graphenedb.com:24780/db/data/transaction/commit";
    popoto.rest.AUTHORIZATION = "Basic cG9wb3RvOmIuVlJZQVF2blZjV2tyLlRaYnpmTks5aHp6SHlTdXk=";

    popoto.provider.node.Provider = {
        Customer: {
            returnAttributes: ["CustId", "Username", "Username2"],
            constraintAttribute: "CustId",
            displayAttribute: "Username",
            getDisplayType: function () {
                return popoto.provider.node.DisplayTypes.IMAGE;
            },
            getImagePath: function () {
                return "../images/graphrender/account.svg";
            }
        },
        Device: {
            returnAttributes: ["DeviceId"],
            constraintAttribute: "DeviceId",
            autoExpandRelations: true,
            isAutoLoadValue: true,
            getDisplayType: function () {
                return popoto.provider.node.DisplayTypes.IMAGE;
            },
            getImagePath: function () {
                return "../images/graphrender/device_pc.svg";
            }
        },
        Group: {
            returnAttributes: ["GroupId", "CustomerQuantity"],
            constraintAttribute: "GroupId",
            getDisplayType: function () {
                return popoto.provider.node.DisplayTypes.IMAGE;
            },
            getImagePath: function () {
                return "../images/graphrender/group.svg";
            }
        }
    };

    popoto.query.RESULTS_PAGE_SIZE = 100;
    popoto.query.USE_RELATION_DIRECTION = true;
    popoto.logger.LEVEL = popoto.logger.LogLevels.ERROR;

    popoto.tools.TOGGLE_FIT_TEXT = true;
    popoto.tools.SAVE_GRAPH = true;
    popoto.tools.TOGGLE_VIEW_RELATION = true;

    popoto.result.onGraphResultReceived(
        function (graphResultObject) {
            var nodes = convertNodes(graphResultObject.nodes);
            var edges = convertEdges(graphResultObject.edges);
            var container = document.getElementById('vis');
            // provide the data in the vis format
            var data = {
                nodes: nodes,
                edges: edges
            };
            var options = {
                nodes: {
                    shape: 'circle',
                    borderWidth: 2,
                    size: 20,
                    font: {
                        size: 10,
                        color: '#fff'
                    }
                },
                edges: {
                    length: 200,
                    font: {
                        align: 'top',
                        color: '#fff',
                        strokeWidth: 0,
                        size: 10
                    },
                    arrows: 'to'
                },
                groups: {
                    "Customer": {
                        color: { background: '#68BDF6', border: '#5CA8DB' }
                    },
                    "Device": {
                        color: { background: '#FF756E', border: '#E06760' }
                    }
                }
            };
            // initialize the network
            var network = new vis.Network(container, data, options);
        }
    );

    function convertNodes(nodes) {
        var convertedNodes = [];
        nodes.forEach(function (node) {
            var nodeLabel = node.labels[0];
            convertedNodes.push({
                id: node.id,
                label: (nodeLabel === "Customer" ? node.properties["Username"] : node.properties[0]),
                group: nodeLabel
            })
        });

        return convertedNodes;
    }

    function convertEdges(edges) {
        var convertedEdges = [];
        edges.forEach(function (edge) {
            convertedEdges.push({
                from: edge.startNode,
                to: edge.endNode,
                label: edge.type
            })
        });

        return convertedEdges;
    }

    var saves = {
        "save-1": {
            label: "Customer",
            value: {
                Username: "12BETEURO013194",
                CustId: 2993612
            }
        }
    };

    popoto.start(saves["save-1"]);
}