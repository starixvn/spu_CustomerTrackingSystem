function account_evidence() {
    var tblChainToEvidenceFlagged = $('#tblChainToEvidenceFlagged');
    var btnChainToEvidenceFlagged = $('#btnChainToEvidenceFlagged');
    var divPlaceEvidence = $('#divPlaceEvidence');
    var lblPlaceEvidenceAccount = $('#lblPlaceEvidenceAccount');
    var lblPlaceEvidenceSubscriber = $('#lblPlaceEvidenceSubscriber');
    var ddlPlaceEvidenceListEvidence = $('#ddlPlaceEvidenceListEvidence');
    var btnPlaceEvidenceCancel = $('#btnPlaceEvidenceCancel');
    var btnPlaceEvidenceSubmit = $('#btnPlaceEvidenceSubmit');
    var divPlaceEvidenceDescription = $('#divPlaceEvidenceDescription');
    var txtPlaceEvidenceComment = $('#txtPlaceEvidenceComment');    
    var tblChainToEvidenceAffectedRetract = $('#tblChainToEvidenceAffected');   

    function InitialFlaggedGrid(ctsCustId, username) {
        tblChainToEvidenceFlagged.DataTable({
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
                { targets: [0], width: "30%", className: "text-left" },
                { targets: [1], width: "20%", className: "text-center" },
                { targets: [2], width: "20%", className: "text-left" },
                { targets: [3], width: "20%", className: "text-center" },
                { targets: [4], width: "20%", className: "text-center" }
            ],
            columns: [
                { data: obj => obj.EvidenceDisplay },
                { data: obj => moment(obj.FlaggedDate).format("MMM DD, YYYY HH:mm:ss") },
                { data: obj => obj.Comment },
                { data: obj => obj.Creator },
                {
                    data: obj => '<span class="icon-delete"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() =>
                            common.showConfirmation(`Do you want remove flagged evidence ${row.EvidenceDisplay} to ${username}?`, function () {
                                RemoveFlaggedEvidence(row.EvidenceID, ctsCustId, username)
                            })
                        );
                    }
                }
            ],
            createdRow: function (row, obj) {
                $(row).attr('evidence-code', obj.EvidenceCode);
            },
        })
    }    

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
                { targets: [0], width: "15%", className: "text-left", visible: false },
                { targets: [1], width: "15%", className: "text-left", defaultContent: "-" },
                { targets: [2], width: "15%", className: "text-center", defaultContent: "-" },
                { targets: [3], width: "20%", className: "text-center", defaultContent: "-" },
                { targets: [4], width: "20%", className: "text-center", defaultContent: "-" },
                { targets: [5], width: "10%", className: "text-center", defaultContent: "-" },
                { targets: [6], width: "10%", className: "text-center", defaultContent: "" }
            ],
            columns: [
                { data: obj => obj.EvidenceDisplay },
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
                            html = '<tr class="retract" evidence-code="' + data[i].EvidenceCode + '"style="background-color: #70809e">';
                            html += '<td colspan="5" style="color: #fff !important; text-decoration: line-through; position: relative;">';
                            html += '<span class="evidence-retract">' + group + '</span>';
                            html += '<div class="tooltip">';
                            html += '<div>Create Date: ' + moment(data[i].AffectedTime).format("MMM DD, YYYY HH:mm:ss") + '</div>';
                            html += '<div>Create By: ' + data[i].Creator + '</div>';
                            html += '<div>Comment:  ' + data[i].Comment + '</div></td>';
                            html += '<td class="text-center"><span class="icon-delete-restore delete" evidence-id = "' + data[i].EvidenceID + '"></span></td>';
                            html += '</tr>';
                        } else {
                            html = '<tr class="affected" evidence-code="' + data[i].EvidenceCode + '" style="background-color: #70809e">';
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

    function GetAccountEvidence(ctsCustId, username) {
        var url = common.modifyRelativeUrl('/Account/GetAccountEvidence');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceFlagged.find('tbody').addClass('loading');
                tblChainToEvidenceAffectedRetract.find('tbody').addClass('loading');
            },
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {                
                GenerateFlaggedEvidences(response.FlaggedEvidenceList);

                GenerateAffectedRetractEvidences(response.AffectedRetractEvidenceList, ctsCustId, username);

                GenerateEvidenceList(response.EvidenceList);
            },
            complete: function () {
                tblChainToEvidenceFlagged.find('tbody').removeClass('loading');
                tblChainToEvidenceAffectedRetract.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function GenerateFlaggedEvidences(data) {
        tblChainToEvidenceFlagged.DataTable().clear().rows.add(data).draw();
        tblChainToEvidenceFlagged.DataTable().columns.adjust();
    }

    function RemoveFlaggedEvidence(evidenceId, ctsCustId, username) {
        var url = common.modifyRelativeUrl('/Account/RemoveFlaggedEvidence');

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
                GetAccountEvidence(ctsCustId, username);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function GenerateAffectedRetractEvidences(data, ctsCustId, username) {
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
                    GetAccountEvidence(ctsCustId, username);
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

        var html = '<div class="filter"><div class="dropdown">';        
        html += '<div class="control-label">Comment</div>';
        html += '<div><textarea rows="4" style="width:400px" id="txtRemoveEvidenceComment" maxlength="500"></textarea></div>';
        html += '</div></div>';        

        common.showConfirmation(html, function () {
            var url = common.modifyRelativeUrl('/Account/RetractAffectedEvidence');
            var params = {};
            params.ctsCustId = ctsCustId;
            params.evidenceId = evidenceId;
            params.comment = $('#txtRemoveEvidenceComment').val();

            $.ajax(url, {
                type: 'POST',
                beforeSend: function () {
                    common.showLoading();
                },
                data: params,
                success: function (response) {
                    common.showSuccess();
                },
                complete: function () {
                    common.closeLoading();
                    GetAccountEvidence(ctsCustId, username);
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    common.closeLoading();
                    common.showError(errMsg);
                }
            })
        })
    }

    function GenerateEvidenceList(data) {
        $('option', '#cbbChainToEvidenceListEvidence').filter(function () {          
            return $(this).val() != '';
        }).remove()

        $('#cbbChainToEvidenceListEvidence').multiselect("refresh");

        for (var i = 0; i < data.length; i++) {
            $('#cbbChainToEvidenceListEvidence').append(`<option selected value='${data[i].EvidenceCode}'>${data[i].EvidenceDisplay}</option>`);
        }      

        $('#cbbChainToEvidenceListEvidence').multiselect("refresh").multiselect({
            menuWidth: 330,
            menuHeight: 330,
            selectedText: "# of # selected",
            noneSelectedText: "Select options",
            click: function (event, ui) {
                RefreshFlaggedEvidence(ui.value, ui.checked);
                RefreshAffectedRetractEvidence(ui.value, ui.checked);
            },
            checkAll: function () {
                RefreshAllFlaggedEvidence(true);
                RefreshAllAffectedRetractEvidence(true);
            },
            uncheckAll: function () {
                RefreshAllFlaggedEvidence(false);
                RefreshAllAffectedRetractEvidence(false);
            }
        }).multiselectfilter({
            width: 250,
            placeholder: ''
        });
    }

    function RefreshFlaggedEvidence (evidenceCode, checked) {
        tblChainToEvidenceFlagged.find('tr').each(function () {
            if ($(this).attr('evidence-code') == evidenceCode) {
                if (checked) {
                    $(this).removeClass('hide');
                } else {
                    $(this).addClass('hide');
                }
            }
        })
    }

    function RefreshAllFlaggedEvidence(checked) {
        tblChainToEvidenceFlagged.find('tr').each(function () {
            if (checked) {
                $(this).removeClass('hide');
            } else {
                $(this).addClass('hide');
            }
        })
    }    

    function RefreshAllAffectedRetractEvidence(checked) {
        tblChainToEvidenceAffectedRetract.find('tr').each(function () {
            if (checked && $(this).attr('cust-retract') != "true") {
                $(this).removeClass('hide');
            } else {
                $(this).addClass('hide');
            }
        })
    }
   
    function RefreshAffectedRetractEvidence(evidenceCode, checked) {
        tblChainToEvidenceAffectedRetract.find('tr').each(function () {            
            if (!$(this).attr('cust-retract') == true) {
                if ($(this).attr('evidence-code') == evidenceCode) {
                    if (checked) {
                        $(this).removeClass('hide');
                    } else {
                        $(this).addClass('hide');
                    }
                }
            }            
        })
    }     

    function InitialPlaceEvidenceForm(ctsCustId, username, subscriberId, subscriber) {        
        btnChainToEvidenceFlagged.on('click', function () {
            ddlPlaceEvidenceListEvidence.multiselect({
                multiple: false,
                selectedList: 1,
                menuWidth: 340,
                menuHeight: 300,              
            }).multiselectfilter({
                width: 250,
                placeholder: ''
                });

            var flaggedEvidenceCodes = GetEvidenceInFlaggedEvidenceGrid();

            $('option', ddlPlaceEvidenceListEvidence).filter(function () {
                return $.inArray($(this).attr('code'), flaggedEvidenceCodes) !== -1
            }).remove();
                        
            $('option:first-child', ddlPlaceEvidenceListEvidence).prop("selected", true);
            ddlPlaceEvidenceListEvidence.multiselect('refresh');

            ddlPlaceEvidenceListEvidence.bind('change', function (e) {
                var desc = $('#ddlPlaceEvidenceListEvidence :selected').attr('desc');
                if (desc === undefined)
                    desc = '';
                divPlaceEvidenceDescription.html(desc);
            })

            ResetPlaceEvidence(username, subscriber);

            divPlaceEvidence.dialog({
                modal: true,
                width: '900px',
                title: 'ADD EVIDENCE',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        })

        btnPlaceEvidenceCancel.on('click', function () {
            divPlaceEvidence.dialog('close');
        })

        btnPlaceEvidenceSubmit.on('click', function (e) {
            AddPlaceEvidence(ctsCustId, username, subscriberId);
        })
    }

    function ResetPlaceEvidence(username, subscriber) {
        lblPlaceEvidenceAccount.text(username);
        lblPlaceEvidenceSubscriber.text(subscriber);
        var desc = $('option:first-child', ddlPlaceEvidenceListEvidence).attr('desc');
        if (desc === undefined)
            desc = '';
        divPlaceEvidenceDescription.text(desc);
        txtPlaceEvidenceComment.val('');
    }

    function GetEvidenceInFlaggedEvidenceGrid() {
        var evidenceCodes = [];

        tblChainToEvidenceFlagged.find('tr').each(function () {
            if ($(this).attr('evidence-code') != undefined) {
                evidenceCodes.push($(this).attr('evidence-code'));
            }
        })

        return evidenceCodes;
    }

    function AddPlaceEvidence(ctsCustId, username, subscriberId) {
        var url = common.modifyRelativeUrl('/Account/PlaceEvidence');
        var params = {};
        params.ctsCustId = ctsCustId;
        params.subscriberId = subscriberId;
        params.evidenceId = ddlPlaceEvidenceListEvidence.find('option:selected').val();
        params.comment = txtPlaceEvidenceComment.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                common.showSuccess('Add the evidence successful!');
                divPlaceEvidence.dialog('close');
            },
            complete: function () {
                GetAccountEvidence(ctsCustId, username);
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add the evidence fail!');
            }
        })
    }

    this.Initial = function (ctsCustId, username, subscriberId, subscriber) {

        $('#cbbChainToEvidenceListEvidence').multiselect({
            menuWidth: 330,
            menuHeight: 330,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 250,
            placeholder: ''
            });

        InitialFlaggedGrid(ctsCustId, username, subscriberId, subscriber);        

        InitialAffectedRetractGrid();

        GetAccountEvidence(ctsCustId, username);

        InitialPlaceEvidenceForm(ctsCustId, username, subscriberId, subscriber);
    }
}