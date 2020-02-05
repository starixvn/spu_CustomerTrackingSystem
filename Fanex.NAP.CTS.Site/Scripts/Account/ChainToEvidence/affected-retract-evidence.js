function affected_retract_evidence() {
    var tblChainToEvidenceAffectedRetract = $('#tblChainToEvidenceAffected');

    function InitialAffectedRetractGrid(ctsCustId) {
        tblChainToEvidenceAffectedRetract.DataTable({
            language: {
                zeroRecords: "There's no data"
            },
            info: false,
            ordering: false,
            paging: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },                
                { targets: [0], width: "15%", className: "text-left", visible: false },
                { targets: [1], width: "15%", className: "text-left" },
                { targets: [2], width: "15%", className: "text-center" },
                { targets: [3], width: "20%", className: "text-center" },
                { targets: [4], width: "20%", className: "text-center" },
                { targets: [5], width: "10%", className: "text-center" },
                { targets: [6], width: "10%", className: "text-center" }                
            ],
            columns: [
                { data: obj => obj.EvidenceDisplay},
                { data: obj => obj.UserName },
                { data: obj => obj.LoginID },
                { data: obj => obj.AssociationType },
                { data: obj => moment(obj.AffectedTime).format("MMM DD, YYYY HH:mm:ss") },
                { data: obj => moment(obj.LastLoginTime).format("MMM DD, YYYY HH:mm:ss") }
            ],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {                        
                        var html = '';
                        if (!data[i].IsRetract) {
                            html = '<tr style="background-color: #e3e9f6">';
                            html += '<td colspan="5">' + group + '</td>';
                            html += '<td class="text-center"><span class="icon-delete" style="cursor: pointer;"></span></td>';
                            html += '</tr>';
                                
                        } else {
                            html = '<tr style="background-color: #e3e9f6">';
                            html += '<td colspan="5">' + group + '</td>';
                            html += '<td class="text-center"><span class="icon-delete icon-retract" style="cursor: pointer;"></span></td>';
                            html += '</tr>';
                        }
                        $(rows).eq(i).before(html);                       
                        last = group;
                    }
                });
            }
        })
    }     

    function GetAffectedRetractEvidences(ctsCustId) {
        var url = common.modifyRelativeUrl('/Account/GetAffectedRetractEvidences');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceAffectedRetract.find('tbody').addClass('loading');
            },
            data: {
                ctsCustId: 501755,
                evidenceCodes: '6.8'
            },
            success: function (response) {
                RenderAffectedRetractEvidences(response);
            },
            complete: function () {
                tblChainToEvidenceAffectedRetract.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function RenderAffectedRetractEvidences(data) {
        tblChainToEvidenceAffectedRetract.DataTable().clear();
        if (data.length > 0) {
            tblChainToEvidenceAffectedRetract.DataTable().rows.add(data).draw();
        }
        tblChainToEvidenceAffectedRetract.DataTable().columns.adjust();

        tblChainToEvidenceAffectedRetract.find('span.icon-delete').click(function () {
            console.log("deleted");
        })
    }

    this.Initial = function (ctsCustId) {
        InitialAffectedRetractGrid(ctsCustId);
        GetAffectedRetractEvidences(ctsCustId);
    }
}