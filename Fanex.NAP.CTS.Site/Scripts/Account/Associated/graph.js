function graph() {
    var $this = this; 
    var margin = { top: -5, right: -5, bottom: -5, left: -5 };
    var width = 2000 - margin.left - margin.right, height = 980 - margin.top - margin.bottom;
    var collapse_level = 1;
    var root;    

    var urls = {
        GetShortestAssociationGraph: common.modifyRelativeUrl('/Account/GetShortestAssociationGraph'),
        GetDirectAssociationGraph: common.modifyRelativeUrl('/Account/GetDirectAssociationGraph')
    };

    $this.OpenModal = function () {
        $("#container-graph").html('');

        $('#form-graph').dialog({
            modal: true,
            width: '1500px',
            title: 'Graph visualization',
            dialogClass: "modal-graph-render",
            resizable: true,
            draggable: true,
            close: function () { $(this).dialog('close'); }
        });
    };
      
    $this.GenerateShortestPath = function (custIdFrom, custIdTo) {
        collapse_level = 0;
        var paramsFilter = {};
        paramsFilter.custIdFrom = custIdFrom;
        paramsFilter.custIdTo = custIdTo;

        $.ajax(urls.GetShortestAssociationGraph, {
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
    };

    $this.GenerateFullPath = function (params) {
        $.ajax(urls.GetDirectAssociationGraph, {
            type: 'POST',
            data: params,
            success: function (response) {
                generateGraph(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }; 

    function generateGraph(data) {
        root = data;
        var force = d3.layout.force()
            .size([width + margin.left + margin.right, height + margin.top + margin.bottom])
            .on("tick", tick);
     
        d3.behavior.drag()
            .origin(function (d) { return d; })
            .on("dragstart", () => {
                d3.event.sourceEvent.stopPropagation();

                d3.select(this).classed("dragging", true);
                force.start();
            })
            .on("drag", () => {
                d3.select(this).attr("cx", d.x = d3.event.x).attr("cy", d.y = d3.event.y);
            })
            .on("dragend", () => {
                d3.select(this).classed("dragging", false);
            });

        var svg = d3.select("#container-graph").append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .attr("transform", "translate(" + margin.left + "," + margin.right + ")");

        var link = svg.selectAll(".link");
        var node = svg.selectAll(".node");        

        parseLevel(root, 0);
        update();        

        // Initialize the display to show a few nodes.
        if (collapse_level == 1) {
            if (root.children) { root.children.forEach(toggleAll); }
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
            var nodes = flatten(root);
                
            updateLevelX(nodes);            
        }       

        function updateLevelX(nodes) {
            links = d3.layout.tree().links(nodes);
            // Restart the force layout.
            force.nodes(nodes)
                .links(links)
                .charge(-600)
                .linkDistance(100)
                .friction(0.9)
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
                .text(function (d) { return d.DisplayName; });

            node.select("circle")
                .style("fill", color);
        }     
        
        function color(d) {
            return d._children ? "#3182bd" // collapsed package
                    : d.children ? "#8dc0f0" // expanded package
                    : "#3182bd"; // leaf node
                    //: "#fd8d3c"; // leaf node
        }

        function tick() {
            svg.selectAll('.link')
                .attr("x1", function (d) { return d.source.x; })
                .attr("y1", function (d) { return d.source.y; })
                .attr("x2", function (d) { return d.target.x; })
                .attr("y2", function (d) { return d.target.y; });

            node.attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; });

            svg.selectAll('text.aEnd')
                .attr('x', function (d) { return xposition(d.source, d.target); })
                .attr('y', function (d) { return yposition(d.source, d.target); });
        }        

        // Toggle children on click.
        function click(d) {
            if (d3.event.defaultPrevented) { return; } // ignore drag
            if (collapse_level == 1) {
                if (d.children) {
                    d._children = d.children;
                    d.children = null;
                    update();
                } else {
                    let params = {};
                    params.CTSCustId = d.CTSCustId;
                    params.displayName = d.Username;
                    params.AssociationStatus = d.AssociationStatus;
                    params.AssociationType = d.AssociationType;
                    params.AccountStatusIds = d.AccountStatusIds;
                    params.CategoryIds = d.CategoryIds;
                    params.SiteIds = d.SiteIds;
                    params.start = d.Start;
                    params.length = d.Length;                    

                    $.ajax(urls.GetDirectAssociationGraph, {
                        type: 'POST',
                        data: params,
                        success: function (response) {
                            d.children = response.children;
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
        }
      
        // Returns a list of all nodes under the root.
        function flatten(root) {
            var nodes = [];

            function recurse(node) {
                if (node.children) {
                    node.children.forEach(recurse);                    
                }
                if (!node.id) {
                    node.id = node.ItemCode;
                }                
                nodes.push(node);
            }
            recurse(root);

            return nodes;
        }          

        function xposition(source, target) {
            var angle = Math.atan2(target.y - source.y, target.x - source.x);
            return 100 * Math.cos(angle) + source.x;
        }

        function yposition(source, target) {
            var angle = Math.atan2(target.y - source.y, target.x - source.x);
            return 100 * Math.sin(angle) + source.y;
        }

        function addLink(linkEnter) {
            linkEnter.append('svg:line')
                .attr("class", 'link')
                .attr("x1", function (d) { return d.source.x; })
                .attr("y1", function (d) { return d.source.y; })
                .attr("x2", function (d) { return d.target.x; })
                .attr("y2", function (d) { return d.target.y; });
        }
    }
}