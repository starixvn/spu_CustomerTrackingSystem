function account_by_account() {
    var btnSearch = $('#account-by-account-btn-search');
    var btnViewOnGraph = $('#account-by-account-btn-on-graph');
    var associatedByAccountTable = $('#account-by-account-table');    

    function InitialGrid() {
        associatedByAccountTable.DataTable({           
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"                
            },
            ordering: false,
            lengthMenu: [10, 20, 50, 100, 200],
            pageLength: 20,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -1], className: "text-center" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "10%" },
                { targets: [9], width: "5%" }
            ],
            columns: [
                { data: obj => RenderTag(obj) },
                { data: obj => RenderUsername(obj) },
                { data: obj => obj.LoginId },
                { data: obj => obj.Status },
                { data: obj => obj.Subscriber },
                { data: obj => obj.AssociationType },
                { data: obj => obj.SharedDevices },
                { data: obj => obj.SharedIps },
                { data: obj => moment(obj.AssociationDate).format("MMM DD, YYYY") },
                {
                    data: obj => RenderGraphImage(), createdCell: (cell, _, row) => {
                        $('.icon-path', cell).click(() => HandleGraphShortestPath(row));
                    }
                }
            ]
        });
    }

    function RenderTag(obj) {

        var html = "";

        if (obj.Tag == 1) {
            html = "<span class='icon icon-Flagged-With-Evidence'></span>";
        }
        else if (obj.Tag == 2) {
            html = "<span class='icon icon-Flagged'></span>";
        }
        else {
            html = "<span class='icon icon-Affected'></span>";
        }        

        return html;
    }

    function RenderUsername(obj) {
        if (obj.Username == null) {
            return obj.Username;
        }

        var url = common.modifyRelativeUrl('/Account/DetailAccount?CustId=' + obj.CustId);

        var html = '<a class="info-bng" href="' + url + '" target="_blank">' + obj.Username + '</a>';

        return html;
    }

    function RenderGraphImage() {
        return '<span class="icon-path graph" title="Graph View"></span>';
    }

    function RenderGrid(custId, statusIds, associationType, isDirect) {
        var url = common.modifyRelativeUrl('/Account/GetAssociationByAccount');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                associatedByAccountTable.find('tbody').addClass('loading');
            },
            data: {
                custId: custId,               
                statusIds: statusIds,
                associationType: associationType,
                isDirect: isDirect
            },
            success: function (response) {
                associatedByAccountTable.DataTable().clear().rows.add(response).draw();
                associatedByAccountTable.DataTable().columns.adjust();               
            },
            complete: function () {
                associatedByAccountTable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    function HandleGraphShortestPath(row) {
        var formGraph = new graph();
        formGraph.GenerateShortestPath(row.CTSCustId, row.CustId);
        formGraph.OpenModal();
    }

    //function BindDataToGrid(data) {
    //    loading.hide();
    //    associatedContent.show();

    //    associatedTable.DataTable().clear().rows.add(data).draw();
    //    associatedTable.DataTable().columns.adjust();        
    //}

    //function AnimateContent() {
    //    $('html, body').animate({
    //        scrollTop: $(associatedContent).offset().top
    //    }, 1500);
    //}

    //function InitSearchOnGrid() {
    //    associatedTable.DataTable().columns([0]).every(function () {
    //        var column = this;

    //        accountSearch.keyup(function () {
    //            if (column.search() !== this.value) {
    //                column.search(this.value).draw();
    //            }
    //        });
    //    });
    //}

    //function ValidateInputSearch(deviceAwayOne, deviceAwayMore) {
    //    if (!deviceAwayOne && !deviceAwayMore) {
    //        return false;
    //    }
    //    return true;
    //}

    //function OpenGraphRender() {
    //    $('#form-graph-render').dialog({
    //        modal: true,
    //        width: '1500px',
    //        title: 'Graph visualization',
    //        dialogClass: "modal-graph-render",
    //        resizable: true,
    //        draggable: true,
    //        close: function () { $(this).dialog('close'); }
    //    })
    //}

    //function RenderGraphImage() {
    //    return '<span class="icon-path graph" title="Graph View" onclick="OpenShortestPath(22971808,4677919)"></span>';
    //}

    //function OpenShortestPath(startCustId, endCustId) {
    //    $('#shortest-path-render').dialog({
    //        modal: true,
    //        width: '1500px',
    //        title: 'Shortest Path',
    //        dialogClass: "modal-graph-render",
    //        resizable: true,
    //        draggable: true,
    //        open: function () {
    //            $.ajax({
    //                type: "POST",
    //                data: { startCustId: startCustId, endCustId: endCustId },
    //                url: "GetShortestPath",
    //                beforeSend: function () {
    //                    alert('before send');
    //                },
    //                success: function () {

    //                }
    //            });
    //        },
    //        close: function () { $(this).dialog('close'); }
    //    })
    //}

    //GenerateGrid();

    //InitSearchOnGrid();

    //$("#account-by-account-cbb-status").multiselect({
    //    menuWidth: 225,
    //    menuHeight: 225,
    //    selectedText: "# of # selected",
    //    noneSelectedText: "Select options"
    //}).multiselect("checkAll");

    //usernameSpan.text(USERNAME);

    //loading.hide(); 

    //btnSearch.click(GenerateReport);

    //$('#account-by-account-btn-shortest-path').click(function () {
    //    OpenShortestPath(22971808, 4677919);
    //});

    //$('#account-by-account-btn-on-graph').click(OpenGraphRender);

    this.Initial = function (custId, username, statusIds, associationType, isDirect) {
        $("#account-by-account-cbb-status").multiselect();

        InitialGrid();
        RenderGrid(custId, statusIds, associationType, isDirect);

        btnSearch.on('click', function () {
            var isDirect = $("#radio03-01").is(':checked');
            var accountStatus = $('#account-by-account-cbb-status').val().join(',');
            var associationType = $("#account-by-account-cbb-association-type").val();
            var custId = $("#hdCustId").val();

            RenderGrid(custId, accountStatus, associationType, isDirect);
        });

        btnViewOnGraph.on('click', function () {
            var formGraph = new graph();
            formGraph.GenerateFullPath(custId, username);
            formGraph.OpenModal();
        });
    }
}



