function affected_retract_evidence() {
    var tblChainToEvidenceAffectedRetract = $('#tblChainToEvidenceAffected');    

    function InitialAffectedRetractGrid() {
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
            createdRow: function (row, obj) {
                $(row).attr('evidence-code', obj.EvidenceCode);
                if (obj.IsRetract) {
                    $(row).attr('cust-retract', true);
                }
            },
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        var html = '';
                        if (data[i].IsRetract) {
                            html = '<tr class="retract" evidence-code="' + data[i].EvidenceCode + '" style="background-color: #70809e">';
                            html += '<td colspan="5" style="color: #fff !important; text-decoration: line-through;">' + group + '</td>';
                            html += '<td class="text-center"><span class="icon-delete-restore delete" evidence-id = "' + data[i].EvidenceID + '"></span></td>';
                            html += '</tr>';
                        } else {
                            html = '<tr class="affected" evidencecode="' + data[i].EvidenceCode + '" style="background-color: #70809e">';
                            html += '<td colspan="5" style="color: #fff !important">' + group + '</td>';
                            html += '<td class="text-center"><span class="icon-delete delete" evidence-id = "' + data[i].EvidenceID + '"></span></td>';
                            html += '</tr>';  
                        }
                        $(rows).eq(i).before(html);
                        last = group;
                    }
                });
            }
        })
    }     

    function GetAffectedRetractEvidences(ctsCustId, username, evidenceCodes) {
        var url = common.modifyRelativeUrl('/Account/GetAffectedRetractEvidences');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceAffectedRetract.find('tbody').addClass('loading');
            },
            data: {
                ctsCustId: ctsCustId,
                evidenceCodes: evidenceCodes
            },
            success: function (response) {
                RenderAffectedRetractEvidences(response, ctsCustId, username);
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

    function RenderAffectedRetractEvidences(data, ctsCustId, username) {        
        tblChainToEvidenceAffectedRetract.DataTable().clear().rows.add(data).draw();
        tblChainToEvidenceAffectedRetract.find('tr').each(function () {
            if ($(this).attr('cust-retract') == "true") {
                $(this).addClass('hide');
            }
        })

        tblChainToEvidenceAffectedRetract.DataTable().columns.adjust();

        tblChainToEvidenceAffectedRetract.find('span.delete').click(function () {
            var evidenceId = $(this).attr('evidence-id');
            if ($(this).hasClass('icon-delete-restore')) {
                RestoreAffectedEvidence(ctsCustId, username, evidenceId);
            } else {
                RetractAffectedEvidence(ctsCustId, username, evidenceId);
            }
        })
    }

    function RestoreAffectedEvidence(ctsCustId, username, evidenceId) {
        common.showConfirmation(`Do you want restore affected evidence to ${username}?`, function () {
            var url = common.modifyRelativeUrl('/Account/RestoreAffectedEvidence');

            $.ajax(url, {
                type: 'POST',
                beforeSend: function () {
                    common.showLoading();
                },
                data: {
                    ctsCustId: ctsCustId,
                    evidenceId: evidenceId
                },
                success: function (response) {
                    common.showSuccess();
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    common.closeLoading();
                    common.showError(errMsg);
                }
            })
        })
    }

    function RetractAffectedEvidence(ctsCustId, username, evidenceId) {
        common.showConfirmation(`Do you want retract affected evidence to ${username}?`, function () {
            var url = common.modifyRelativeUrl('/Account/RetractAffectedEvidence');

            $.ajax(url, {
                type: 'POST',
                beforeSend: function () {
                    common.showLoading();
                },
                data: {
                    ctsCustId: ctsCustId,
                    evidenceId: evidenceId
                },
                success: function (response) {
                    common.showSuccess();
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    common.closeLoading();
                    common.showError(errMsg);
                }
            })
        })        
    }

    this.Initial = function (ctsCustId, username, evidenceCodes) {
        InitialAffectedRetractGrid(ctsCustId);
        GetAffectedRetractEvidences(ctsCustId, username, evidenceCodes);
    }

    this.Refresh = function (evidenceCode, checked) {
        tblChainToEvidenceAffectedRetract.find('tr').each(function () {
            if ($(this).attr('evidence-code') == evidenceCode) {
                if (checked) {
                    $(this).removeClass('hide');
                } else {
                    $(this).addClass('hide');
                }                
            }
        })                
    }

    this.RefreshAll = function (checked) {
        tblChainToEvidenceAffectedRetract.find('tr').each(function () {            
            if (checked && $(this).attr('cust-retract') != "true") {
                $(this).removeClass('hide');
            } else {
                $(this).addClass('hide');
            }
        })
    }
}