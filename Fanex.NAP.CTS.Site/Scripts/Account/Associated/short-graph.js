function short_graph() {
    this.Register = function () {
        popoto.rest.CYPHER_URL = "https://db-kh9ct9ai1mqn6hz2itry.graphenedb.com:24780/db/data/transaction/commit";
        popoto.rest.AUTHORIZATION = "Basic cG9wb3RvOmIuVlJZQVF2blZjV2tyLlRaYnpmTks5aHp6SHlTdXk=";                
    }

    this.OpenGraph = function () {
        // Define the list of label provider to customize the graph behavior:
        popoto.provider.node.Provider = {
            Customer: {
                returnAttributes: ["Username"],
                constraintAttribute: "Username"
            },
            Device: {
                returnAttributes: ["DeviceId"],
                constraintAttribute: "DeviceId"
            }
        };

        // Change the number of displayed results:
        popoto.query.RESULTS_PAGE_SIZE = 100;
        popoto.logger.LEVEL = popoto.logger.LogLevels.ERROR;

        // Add a listener on result received to update map.
        popoto.result.onGraphResultReceived(
            function (graphResultObject) {
                var sampleData = generateSampleData();
                var nodes = convertNodes(sampleData.nodes);
                var edges = convertEdges(sampleData.edges);

                //var nodes = convertNodes(graphResultObject.nodes);
                //var edges = convertEdges(graphResultObject.edges);
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
                            color: '#000000'
                        }
                    },
                    edges: {
                        length: 200,
                        font: {
                            align: 'top',
                            color: '#ffffff',
                            strokeWidth: 0,
                            size: 10
                        },
                        arrows: 'to'
                    },
                    groups: {
                        Customer: {
                            color: { background: '#FFFF00', border: '#5CA8DB' }
                        },
                        Device: {
                            color: { background: '#FF756E', border: '#E06760' }
                        }
                    }
                };

                // initialize the network
                var network = new vis.Network(container, data, options);
            }
        );

        // Start the generation using parameter as root label of the query.
        popoto.start({
            label: "Customer",
            rel: [
                {
                    label: "USED",
                    target: {
                        label: "Customer",
                        value: [
                            {
                                Username: "Agent007"
                            }
                        ]
                    }
                }
            ]
        });        
    }   

    function convertNodes(nodes) {
        var convertedNodes = [];

        nodes.forEach(function (node) {
            var nodeLabel = node.labels[0];
            convertedNodes.push({
                id: node.id,
                label: (nodeLabel === "Customer" ? node.properties["Username"] : node.properties["DeviceId"]),
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

    function generateSampleData() {
        var sampleData = new sample_data_graph();
        var data = sampleData.genereateSampleShortest();

        return data;
    }
}