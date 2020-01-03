function graph() {
    var $this = this;
    var width = 1850;
    var height = 800;
    var collapse_level = 1;
    var root;

    var urls = {
        GetShortestAssociationGraph: common.modifyRelativeUrl('/Account/GetShortestAssociationGraph'),
        GetDirectAssociationGraph: common.modifyRelativeUrl('/Account/GetDirectAssociationGraph')
    };

    $this.OpenModal = function () {
        $('#form-graph').dialog({
            modal: true,
            width: '1500px',
            title: 'Graph visualization',
            dialogClass: "modal-graph-render",
            resizable: true,
            draggable: true,
            close: function () { $(this).dialog('close'); }
        })
    }

    $this.GenerateShortestPath = function (custIdFrom, userNameFrom, custIdTo) {
        collapse_level = 0;
        //var sampleData = new sample_data_graph();
        //var data = sampleData.GenereateSampleShortest();
        var paramsFilter = {};
        paramsFilter.custIdFrom = custIdFrom;
        paramsFilter.userNameFrom = "Master001";
        paramsFilter.custIdTo = custIdTo;

        $.ajax(urls.GetShortestAssociationGraph, {
            type: 'POST',
            data: paramsFilter,
            success: function (response) {
                generateGraph(data);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    $this.GenerateFullPath = function (custId, userName) {
        //var sampleData = new sample_data_graph();
        //var data = sampleData.GenereateSampleFullPath();
        var paramsFilter = {};
        paramsFilter.custId = custId;
        paramsFilter.userName = userName;

        $.ajax(urls.GetDirectAssociationGraph, {
            type: 'POST',
            data: paramsFilter,
            success: function (response) {
                generateGraph(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    function generateGraph(data) {
        root = data;
        var force = d3.layout.force()
            .size([width, height])
            .on("tick", tick);

        var svg = d3.select("#container-graph").append("svg")
            .attr("width", width)
            .attr("height", height);

        var link = svg.selectAll(".link");
        var node = svg.selectAll(".node");        

        parseLevel(root, 0);
        update();        

        // Initialize the display to show a few nodes.
        if (collapse_level == 1) {
            root.children.forEach(toggleAll);            
        }  
        update(root);

        function parseLevel(node, level) {
            node.level = level;
            if (typeof node.children !== 'undefined' && node.children != null) {
                node.children.forEach(function (children) {
                    parseLevel(children, level + 1);
                });
            }
        }

        function toggleAll(d) {
            if (d.children) {
                d.children.forEach(toggleAll);
                if (d.level < collapse_level) {
                    return;
                }
                toggle(d);
            }
        }

        function toggle(d) {
            if (d.children) {
                d._children = d.children;
                d.children = null;
            } else {
                d.children = d._children;
                d._children = null;
            }
        }

        function update() {
            var nodes = flatten(root),
                links = d3.layout.tree().links(nodes);

            // Restart the force layout.
            force.nodes(nodes)
                .links(links)
                .charge(-1000)
                .linkDistance(180)
                .friction(0.8)
                .start();

            // Update links.
            link = link.data(links, function (d) { return d.target.id; });

            link.exit().remove();

            var linkEnter = link
                .enter()
                .insert("g")
                .attr('class', 'link');

            addLink(linkEnter);

            // Update nodes.
            node = node.data(nodes, function (d) { return d.id; });
            node.exit().remove();

            var nodeEnter = node.enter().append("g")
                .attr("class", "node")
                .on("click", click)
                .call(force.drag);

            nodeEnter.append("circle")
                .attr("r", function (d) {
                    return 26;
                });

            nodeEnter.append("text")
                .attr("dy", ".35em")
                .text(function (d) { return d.Username; });

            node.select("circle")
                .style("fill", color);
        }

        function color(d) {
            return d._children ? "#3182bd" // collapsed package
                : d.children ? "#c6dbef" // expanded package
                    : "#fd8d3c"; // leaf node
        }

        function tick() {
            svg.selectAll('.link')
                .attr("x1", function (d) { return d.source.x; })
                .attr("y1", function (d) { return d.source.y; })
                .attr("x2", function (d) { return d.target.x; })
                .attr("y2", function (d) { return d.target.y; });

            node.attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; });

            svg.selectAll('text.aEnd')
                .attr('x', function (d) { return xpos(d.source, d.target); })
                .attr('y', function (d) { return ypos(d.source, d.target); });
        }        

        // Toggle children on click.
        function click(d) {
            if (d3.event.defaultPrevented){ return; } // ignore drag
            if (d.children) {
                d._children = d.children;
                d.children = null;
            } else {
                var paramsFilter = {};
                paramsFilter.custId = d.CustId;
                paramsFilter.userName = d.Username;
                $.ajax(urls.GetDirectAssociationGraph, {
                    type: 'POST',
                    data: paramsFilter,
                    success: function (response) {
                        d.children = response;
                        d._children = null;
                        update();
                    },
                    error: function (errMsg) {
                        d.children = d._children;
                        d._children = null;
                        console.log(errMsg);
                        common.showError(errMsg);
                    }
                });
            }            
        }

        // Returns a list of all nodes under the root.
        function flatten(root) {
            var nodes = [];
            var index = 0;

            function recurse(node) {
                if (node.children) {
                    //node.children.forEach(recurse);
                    var childs = node.children;
                    for (var i = 0; i < childs.length; i++) {
                        recurse(childs[i]);
                    }
                }
                if (!node.id) {
                    node.id = ++index;
                }
                nodes.push(node);
            }

            recurse(root);

            return nodes;
        }        

        function xpos(source, target) {
            var angle = Math.atan2(target.y - source.y, target.x - source.x);
            return 120 * Math.cos(angle) + source.x;
        };

        function ypos(source, target) {
            var angle = Math.atan2(target.y - source.y, target.x - source.x);
            return 120 * Math.sin(angle) + source.y;
        };

        function addLink(linkEnter) {
            linkEnter.append('svg:line')
                .attr("class", 'link')
                .attr("x1", function (d) { return d.source.x; })
                .attr("y1", function (d) { return d.source.y; })
                .attr("x2", function (d) { return d.target.x; })
                .attr("y2", function (d) { return d.target.y; });

            linkEnter.append('svg:text')
                .attr('class', 'aEnd')
                .attr('x', function (d) { return d.source.x; })
                .attr('y', function (d) { return d.source.y; })
                .attr('text-anchor', 'middle')
                .text(function (d) {
                    return d.target.AssociationType;
                });
        };
    }
}