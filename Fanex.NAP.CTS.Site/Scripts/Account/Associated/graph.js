function graph() {   
    this.OpenModal = function () {
        $('#form-graph-render').dialog({
            modal: true,
            width: '1500px',
            title: 'Graph visualization',
            dialogClass: "modal-graph-render",
            resizable: true,
            draggable: true,
            close: function () { $(this).dialog('close'); }
        })
    }

    this.Options = function () {
        popoto.rest.CYPHER_URL = "https://db-kh9ct9ai1mqn6hz2itry.graphenedb.com:24780/db/data/transaction/commit";
        popoto.rest.AUTHORIZATION = "Basic cG9wb3RvOmIuVlJZQVF2blZjV2tyLlRaYnpmTks5aHp6SHlTdXk=";
        popoto.query.RESULTS_PAGE_SIZE = 100;
        popoto.logger.LEVEL = popoto.logger.LogLevels.ERROR;
    }

    this.Provider = function () {
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
    }

    this.Start = function () {
        popoto.start("Customer");
    }

    this.GenerateShortestNodes = function () {
        var sampleData = generateSampleData();
        var nodes = convertNodes(sampleData.nodes);
        var edges = convertEdges(sampleData.edges);
        var container = document.getElementById('popoto-graph');
        var data = {
            nodes: nodes,
            edges: edges
        };
        var options = getOptions();

        // initialize the network
        var network = new vis.Network(container, data, options);
    }

    this.GenerateFullNodes = function () {
        var sampleData = generateSampleDataFullPath();
        var nodes = convertNodes(sampleData.nodes);
        var edges = convertEdges(sampleData.edges);
        var container = document.getElementById('popoto-graph');
        var data = {
            nodes: nodes,
            edges: edges
        };
        var options = getOptions();

        // initialize the network
        var network = new vis.Network(container, data, options);
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

    function getOptions() {
        return {
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
    }

    function generateSampleData() {
        var sampleData = new sample_data_graph();
        var data = sampleData.genereateSampleShortest();

        return data;
    }

    function generateSampleDataFullPath() {
        var sampleData = new sample_data_graph();
        var data = sampleData.genereateSampleFullPath();

        return data;
    }
}