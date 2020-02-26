function account_by_account() {
    var btnSearch = $('#account-by-account-btn-search');
    var btnViewOnGraph = $('#account-by-account-btn-on-graph');
    var associatedByAccountTable = $('#account-by-account-table');

    function InitialGrid(ctsCustid) {
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
                { data: obj => RenderUsername(obj, ctsCustid) },
                { data: obj => obj.LoginId },
                { data: obj => obj.Status },
                { data: obj => obj.Subscriber },
                { data: obj => obj.AssociationTypeName },
                { data: obj => obj.SharedDevices == -1 ? "-" : obj.SharedDevices },
                { data: obj => obj.SharedIps == -1 ? "-" : obj.SharedIps },
                {
                    data: obj => obj.AssociationDate
                },
                {
                    data: obj => RenderGraphImage(), createdCell: (cell, _, row) => {
                        $('.icon-path', cell).click(() => HandleGraphShortestPath(row, ctsCustid));
                    }
                }
            ]
        });
    }

    function RenderTag(obj) {

        var html = "";

        if (obj.AffectedTag == 1) {
            html += "<span class='icon icon-Affected'  title='Affected'></span>";
        }

        if (obj.FlaggedTag != 0) {
            if (obj.FlaggedTag == 2) {
                html += "<span class='icon icon-Flagged' title='Flagged'></span>";
            }
            else {
                html += "<span class='icon icon-Flagged-With-Evidence' title='Flagged with Evidence 100'></span>";
            }
        }

        return html == "" ? "-" : html;
    }

    function RenderUsername(obj) {
        if (obj.Username != null) {
            var url = common.modifyRelativeUrl('/Account/Index/' + obj.CTSCustId);

            return '<a class="info-bng" href="' + url + '">' + obj.Username + '</a>';
        }

        return null;
    }

    function RenderGraphImage() {
        return '<span class="icon-path graph" title="Graph View"></span>';
    }

    function RenderGrid(custId, ctsCustId, statusIds, associationType, isDirect) {
        var url = common.modifyRelativeUrl('/Account/GetAssociatedByAccount');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                associatedByAccountTable.find('tbody').addClass('loading');
            },
            data: {
                custId: custId,
                ctsCustId: ctsCustId,
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

    function HandleGraphShortestPath(row, ctsCustid) {
        var formGraph = new graph();
        formGraph.GenerateShortestPath(ctsCustid, row.CTSCustId);
        formGraph.OpenModal();
    }

    this.Initial = function (custId, ctsCustid, username, statusIds, associationType, isDirect) {
        $("#account-by-account-cbb-status").multiselect();

        InitialGrid(ctsCustid);
        RenderGrid(custId, ctsCustid, statusIds, associationType, isDirect);

        btnSearch.on('click', function () {
            var isDirect = $("#radio03-01").is(':checked');
            var accountStatus = $('#account-by-account-cbb-status').val().join(',');
            var associationType = $("#account-by-account-cbb-association-type").val();
            var custId = $("#hdCustId").val();
            var ctsCustId = $("#hdCTSCustId").val();

            RenderGrid(custId, ctsCustId, accountStatus, associationType, isDirect);
        });

        btnViewOnGraph.on('click', function () {
            var formGraph = new graph();
            formGraph.GenerateFullPath(ctsCustid, username);
            formGraph.OpenModal();
        });
    }
}



