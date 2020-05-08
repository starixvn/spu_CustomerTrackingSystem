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
                { targets: [4], width: "20%", className: "text-center remove-action" }
            ],
            columns: [
                { data: obj => obj.EvidenceDisplay },
                { data: obj => obj.FlaggedDate },
                { data: obj => obj.Comment },
                { data: obj => obj.Creator },
                {
                    data: obj => '<span class="icon-delete" title="Remove Evidence"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() =>
                            common.showFormConfirmation(`<div class="popup-content">Do you want remove flagged evidence <br/> <strong style="color: red;">${row.EvidenceDisplay}</strong> from <strong style="color: red;">${username}</strong>?</div>`, 'Confirmation', function () {
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
                { data: obj => RenderUsername(obj) },
                { data: obj => obj.LoginID },
                { data: obj => obj.AssociationType },
                { data: obj => obj.AffectedTime },
                { data: obj => obj.LastLoginTime }
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
                            html = '<tr class="retract" evidence-code="' + data[i].EvidenceCode + '">';
                            html += '<td colspan="5" class="has-tooltip">';
                            html += '<span class="evidence-retract">' + group + '</span>';
                            html += '<div class="tooltip">';
                            html += '<div>Removed Date: <strong>' + data[i].AffectedTime + '</strong></div>';
                            html += '<div>Removed By: <strong>' + data[i].Creator + '</strong></div>';
                            html += '<div>Comment:  <strong>' + data[i].Comment + '</strong></div></td>';
                            html += '<td class="text-center"><span class="icon-delete-restore delete" title="Restore Evidence" evidence-id = "' + data[i].EvidenceID + '" evidence-display = "' + data[i].EvidenceDisplay + '"></span></td>';
                            html += '</tr>';
                        } else {
                            html = '<tr class="affected" evidence-code="' + data[i].EvidenceCode + '">';
                            html += '<td class="affected-group" colspan="5">';
                            html += '<span class="icon icon-chevron-up""></span>';
                            html += '<span>' + group + '</span>';
                            html += '</td>';
                            html += '<td class="text-center"><span class="icon-delete delete" title="Remove Evidence" evidence-id = "' + data[i].EvidenceID + '" evidence-display = "' + data[i].EvidenceDisplay + '"></span></td>';
                            html += '</tr>';
                        }
                        $(rows).eq(i).before(html);
                        last = group;
                    }
                });
            }
        })
    }   

    function RenderUsername(obj) {
        if (obj.UserName != null) {
            var url = common.modifyRelativeUrl('/Account/Index/' + obj.CTSCustID);

            return '<a class="info-bng" href="' + url + '">' + obj.UserName + '</a>';
        }

        return null;        
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
               // common.showError(errMsg);
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
                CTSCustId: ctsCustId,
                EvidenceId: evidenceId
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
               // common.showError(errMsg);
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
            var evidenceDisplay = $(this).attr('evidence-display');

            if ($(this).hasClass('icon-delete-restore')) {
                RestoreAffectedEvidence(ctsCustId, username, evidenceId, evidenceDisplay);
            } else {
                RetractAffectedEvidence(ctsCustId, username, evidenceId, evidenceDisplay);
            }
        })

        tblChainToEvidenceAffectedRetract.find('tr.affected').each(function () {
            AffectedRetractToogleGroup(this);
        })

        tblChainToEvidenceAffectedRetract.find('tr td.affected-group').on('click', function () {
            AffectedRetractToogleGroup($(this).closest('tr'));
        });        
    }

    function AffectedRetractToogleGroup(target) {
        var evidenceCode = $(target).attr('evidence-code');

        $('tr[evidence-code="' + evidenceCode + '"]', tblChainToEvidenceAffectedRetract).each(function () {
            if (!$(this).hasClass('affected')) {               
                if ($(this).hasClass('hide')) {
                    $(this).removeClass('hide');                    
                    $('span.icon', $(target)).removeClass('icon-chevron-down').addClass('icon-chevron-up');
                } else {
                    $(this).addClass('hide');
                    $('span.icon', $(target)).removeClass('icon-chevron-up').addClass('icon-chevron-down');
                }
            }
        })
    }

    function RestoreAffectedEvidence(ctsCustId, username, evidenceId, evidenceDisplay) {   
        var html = '<div>';
        html += '<p>Evidence: <strong>' + evidenceDisplay + '</strong></p>';
        html += '<p>From: <strong>All associations</strong></p>';
        html += '<p>To: <strong>' + username + '</strong></p>';
        html += '</div>';

        var title = `Recover Affected Evidence`;

        common.showFormConfirmation(html, title, function () {
            var url = common.modifyRelativeUrl('/Account/RestoreAffectedEvidence');

            $.ajax(url, {
                type: 'POST',
                beforeSend: function () {
                    common.showLoading();
                },
                data: {
                    CTSCustId: ctsCustId,
                    EvidenceId: evidenceId
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
                  //  common.showError(errMsg);
                }
            })
        })
    }

    function RetractAffectedEvidence(ctsCustId, username, evidenceId, evidenceDisplay) {

        var html = '<div class="filter"><div class="dropdown">';         
        html += '<p>Evidence: <strong>' + evidenceDisplay + '</strong></p>';
        html += '<p>From: <strong>All associations</strong></p>';
        html += '<p>To: <strong>' + username + '</strong></p>';        
        html += '<div class="control-label" style="margin-top: 16px;">Comment</div>';
        html += '<div><textarea rows="4" style="width:100%" id="txtRemoveEvidenceComment" maxlength="500"></textarea></div>';
        html += '</div></div>';

        var title = `Remove Affected Evidence`;

        common.showFormConfirmation(html, title, function () {
            var url = common.modifyRelativeUrl('/Account/RetractAffectedEvidence');
            var params = {};
            params.CTSCustId = ctsCustId;
            params.EvidenceId = evidenceId;
            params.Comment = $('#txtRemoveEvidenceComment').val();

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
                   // common.showError(errMsg);
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

    function InitialPlaceEvidenceForm(ctsCustId, username, subscriberId, subscriber, custId) {        
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
            AddPlaceEvidence(ctsCustId, username, subscriberId, custId);
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

    function AddPlaceEvidence(ctsCustId, username, subscriberId, custId) {
        var url = common.modifyRelativeUrl('/Account/PlaceEvidence');
        var params = {};
        params.CTSCustId = ctsCustId;
        params.CustId = custId;
        params.SubscriberId = subscriberId;
        params.EvidenceId = ddlPlaceEvidenceListEvidence.find('option:selected').val();
        params.EvidenceCode = ddlPlaceEvidenceListEvidence.find('option:selected').attr("code");
        params.Comment = txtPlaceEvidenceComment.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            beforeSend: function () {
                btnPlaceEvidenceSubmit.prop('disabled', true);
                common.showLoading();
            },
            success: function (response) {
                common.showSuccess('Add the evidence successful!');
                divPlaceEvidence.dialog('close');
            },
            complete: function () {
                GetAccountEvidence(ctsCustId, username);
                common.closeLoading();
                btnPlaceEvidenceSubmit.prop('disabled', false);
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add the evidence fail!');
                common.closeLoading();
            }
        })
    }

    this.ReloadAccountEvidence = function (ctsCustId, username) {
        GetAccountEvidence(ctsCustId, username);
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