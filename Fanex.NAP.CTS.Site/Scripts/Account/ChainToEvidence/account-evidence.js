function account_evidence() {
    let isHostPermission;

    let domElements = {
        tblChainToEvidenceFlagged: $('#tblChainToEvidenceFlagged'),
        btnChainToEvidenceFlagged: $('#btnChainToEvidenceFlagged'),
        divPlaceEvidence: $('#divPlaceEvidence'),
        lblPlaceEvidenceAccount: $('#lblPlaceEvidenceAccount'),
        lblPlaceEvidenceSubscriber: $('#lblPlaceEvidenceSubscriber'),
        ddlPlaceEvidenceListEvidence: $('#ddlPlaceEvidenceListEvidence'),
        btnPlaceEvidenceCancel: $('#btnPlaceEvidenceCancel'),
        btnPlaceEvidenceSubmit: $('#btnPlaceEvidenceSubmit'),
        divPlaceEvidenceDescription: $('#divPlaceEvidenceDescription'),
        txtPlaceEvidenceComment: $('#txtPlaceEvidenceComment'),
        tblChainToEvidenceAffectedRetract: $('#tblChainToEvidenceAffected'),
        cbbChainToEvidenceListEvidence: $('#cbbChainToEvidenceListEvidence'),
        btnQuickComment: $('.button-quick-comment'),
        spanClass: $('#spanClass'),
        hdCustSubId: $("#hdCustSubId")
    };

    function InitialFlaggedGrid(ctsCustId, username, custId, subscriberId) {
        domElements.tblChainToEvidenceFlagged.DataTable({
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
                    data: obj => '<span class="icon-delete' + (!isHostPermission ? ' disabled' : '') + '" title="Remove Evidence"></span>',
                    createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() =>
                                common.showFormConfirmation(`<div class="popup-content">Do you want remove flagged evidence <br/> <strong style="color: red;">${row.EvidenceDisplay}</strong> from <strong style="color: red;">${username}</strong>?</div>`, 'Confirmation', function () {
                                    RemoveFlaggedEvidence(row.EvidenceID, ctsCustId, username, custId, subscriberId);
                                })
                            );
                        }
                    }
                }
            ],
            createdRow: function (row, obj) {
                $(row).attr('evidence-code', obj.EvidenceCode);
            }
        });
    }    

    function InitialAffectedRetractGrid() {
        domElements.tblChainToEvidenceAffectedRetract.DataTable({
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
                            html += '<td class="text-center">';
                            html += '<span class="icon-delete-restore delete' + (!isHostPermission ? ' icon-delete disabled"' : '') +'" title="Restore Evidence" evidence-id = "' + data[i].EvidenceID + '" evidence-display = "' + data[i].EvidenceDisplay + '"></span>';
                            html += '</td></tr>';
                        } else {
                            html = '<tr class="affected" evidence-code="' + data[i].EvidenceCode + '">';
                            html += '<td class="affected-group" colspan="5">';
                            html += '<span class="icon icon-chevron-up""></span>';
                            html += '<span>' + group + '</span>';
                            html += '</td>';
                            html += '<td class="text-center">';
                            html += '<span class="icon-delete delete' + (!isHostPermission ? ' icon-delete disabled"' : '') +'" title="Remove Evidence" evidence-id = "' + data[i].EvidenceID + '" evidence-display = "' + data[i].EvidenceDisplay + '"></span>';
                            html += '</td></tr>';
                        }
                        $(rows).eq(i).before(html);
                        last = group;
                    }
                });
            }
        });
    }   

    function RenderUsername(obj) {
        if (obj.UserName !== null) {
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
                domElements.tblChainToEvidenceFlagged.find('tbody').addClass('loading');
                domElements.tblChainToEvidenceAffectedRetract.find('tbody').addClass('loading');
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
                domElements.tblChainToEvidenceFlagged.find('tbody').removeClass('loading');
                domElements.tblChainToEvidenceAffectedRetract.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);                
            }
        });
    }

    function GenerateFlaggedEvidences(data) {
        domElements.tblChainToEvidenceFlagged.DataTable().clear().rows.add(data).draw();
        domElements.tblChainToEvidenceFlagged.DataTable().columns.adjust();
    }

    function RemoveFlaggedEvidence(evidenceId, ctsCustId, username, custId, subscriberId) {
        var url = common.modifyRelativeUrl('/Account/RemoveFlaggedEvidence');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                common.showLoading();
            },
            data: {
                CustId: custId,
                CTSCustId: ctsCustId,
                SubscriberId: subscriberId,
                EvidenceId: evidenceId
            },
            success: function (response) {
                common.showSuccess();
                var customerCategory = new customer_category();
                customerCategory.ReloadTab(ctsCustId);
                GetCustomerClassification(custId);
            },
            complete: function () {
                common.closeLoading();
                GetAccountEvidence(ctsCustId, username);
            },
            error: function (errMsg) {
                console.log(errMsg);                
            }
        });
    }

    function GenerateAffectedRetractEvidences(data, ctsCustId, username) {
        domElements.tblChainToEvidenceAffectedRetract.DataTable().clear().rows.add(data).draw();
        domElements.tblChainToEvidenceAffectedRetract.find('tr').each(function () {
            if ($(this).attr('cust-retract') === "true")
                $(this).addClass('hide');
        });

        domElements.tblChainToEvidenceAffectedRetract.DataTable().columns.adjust();

        if (isHostPermission) {
            domElements.tblChainToEvidenceAffectedRetract.find('span.delete').click(function () {
                var evidenceId = $(this).attr('evidence-id');
                var evidenceDisplay = $(this).attr('evidence-display');

                $(this).hasClass('icon-delete-restore')
                    ? RestoreAffectedEvidence(ctsCustId, username, evidenceId, evidenceDisplay)
                    : RetractAffectedEvidence(ctsCustId, username, evidenceId, evidenceDisplay);
            });
        }
        
        domElements.tblChainToEvidenceAffectedRetract.find('tr.affected').each(function () {
            AffectedRetractToogleGroup(this);
        });

        domElements.tblChainToEvidenceAffectedRetract.find('tr td.affected-group').on('click', function () {
            AffectedRetractToogleGroup($(this).closest('tr'));
        });        
    }

    function AffectedRetractToogleGroup(target) {
        var evidenceCode = $(target).attr('evidence-code');

        $('tr[evidence-code="' + evidenceCode + '"]', domElements.tblChainToEvidenceAffectedRetract).each(function () {
            if (!$(this).hasClass('affected')) {
                if ($(this).hasClass('hide')) {
                    $(this).removeClass('hide');
                    $('span.icon', $(target)).removeClass('icon-chevron-down').addClass('icon-chevron-up');
                } else {
                    $(this).addClass('hide');
                    $('span.icon', $(target)).removeClass('icon-chevron-up').addClass('icon-chevron-down');
                }
            }
        });
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
                }
            });
        });
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
                }
            });
        });
    }

    function GenerateEvidenceList(data) {
        $('option', '#cbbChainToEvidenceListEvidence').filter(function () {
            return $(this).val() !== '';
        }).remove();

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
            width: 217,
            placeholder: '',
            label: ''
        });
    }

    function RefreshFlaggedEvidence (evidenceCode, checked) {
        domElements.tblChainToEvidenceFlagged.find('tr').each(function () {
            $(this).attr('evidence-code') === evidenceCode
                ? $(this).removeClass('hide')
                : $(this).addClass('hide');
        });
    }

    function RefreshAllFlaggedEvidence(checked) {
        domElements.tblChainToEvidenceFlagged.find('tr').each(function () {
            checked
                ? $(this).removeClass('hide')
                : $(this).addClass('hide');            
        });
    }    

    function RefreshAllAffectedRetractEvidence(checked) {
        domElements.tblChainToEvidenceAffectedRetract.find('tr').each(function () {
            checked && $(this).attr('cust-retract') !== "true"
                ? $(this).removeClass('hide')
                : $(this).addClass('hide');
        });
    }
   
    function RefreshAffectedRetractEvidence(evidenceCode, checked) {
        domElements.tblChainToEvidenceAffectedRetract.find('tr').each(function () {
            if (!$(this).attr('cust-retract') === true) {
                $(this).attr('evidence-code') === evidenceCode
                    ? $(this).removeClass('hide')
                    : $(this).addClass('hide');               
            }
        });
    }     

    function GetEvidences() {
        var flaggedEvidenceCodes = GetEvidenceInFlaggedEvidenceGrid();

        $.ajax(common.modifyRelativeUrl('/Account/GetEvidenceList'), {
            type: 'GET',
            beforeSend: function () {
                domElements.divPlaceEvidenceDescription.html('');
            },
            success: function (response) {
                if (response != null) {
                    $('option', domElements.ddlPlaceEvidenceListEvidence).filter(function () {
                        return $(this).val() !== '';
                    }).remove();

                    response.map(function (x) {
                        if ($.inArray(x.EvidenceCode, flaggedEvidenceCodes) == -1) {
                            domElements.ddlPlaceEvidenceListEvidence.append(`<option value='${x.EvidenceID}' code='${x.EvidenceCode}' desc='${x.EvidenceDesc}'>${x.EvidenceCode} ${x.EvidenceName}</option>`);
                        }                        
                    });

                    $('option:first-child', domElements.ddlPlaceEvidenceListEvidence).prop("selected", true);
                    var desc = $('#ddlPlaceEvidenceListEvidence :selected').attr('desc');
                    if (desc === undefined)
                        desc = '';
                    domElements.divPlaceEvidenceDescription.html(desc);
                    domElements.ddlPlaceEvidenceListEvidence.multiselect("refresh");
                }
            },
            complete: function () {                                          
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function InitialPlaceEvidenceForm(ctsCustId, username, subscriberId, subscriber, custId) {        
        domElements.btnChainToEvidenceFlagged.on('click', function () {
            domElements.ddlPlaceEvidenceListEvidence.multiselect({
                multiple: false,
                selectedList: 1,
                menuWidth: 340,
                menuHeight: 300
            }).multiselectfilter({
                width: 217,
                placeholder: '',
                label: ''
            });

            GetEvidences();

            domElements.ddlPlaceEvidenceListEvidence.unbind('change').bind('change', function (e) {
                var desc = $('#ddlPlaceEvidenceListEvidence :selected').attr('desc');
                if (desc === undefined)
                    desc = '';
                domElements.divPlaceEvidenceDescription.html(desc);
            });      

            ResetPlaceEvidence(username, subscriber);

            domElements.divPlaceEvidence.dialog({
                modal: true,
                width: '900px',
                title: 'ADD EVIDENCE',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        });

        domElements.btnPlaceEvidenceCancel.on('click', function () {
            domElements.divPlaceEvidence.dialog('close');
        });

        domElements.btnPlaceEvidenceSubmit.on('click', function () {
            AddPlaceEvidence(ctsCustId, username, subscriberId, custId);
        });
    }

    function ResetPlaceEvidence(username, subscriber) {
        domElements.lblPlaceEvidenceAccount.text(username);
        domElements.lblPlaceEvidenceSubscriber.text(subscriber);
        var desc = $('option:first-child', domElements.ddlPlaceEvidenceListEvidence).attr('desc');
        if (desc === undefined)
            desc = '';
        domElements.divPlaceEvidenceDescription.text(desc);
        domElements.txtPlaceEvidenceComment.val('');
    }

    function GetEvidenceInFlaggedEvidenceGrid() {
        var evidenceCodes = [];

        domElements.tblChainToEvidenceFlagged.find('tr').each(function () {
            if ($(this).attr('evidence-code') !== undefined) 
                evidenceCodes.push($(this).attr('evidence-code'));
        });

        return evidenceCodes;
    }

    function AddPlaceEvidence(ctsCustId, username, subscriberId, custId) {
        var url = common.modifyRelativeUrl('/Account/PlaceEvidence');
        var params = {};
        params.CTSCustId = ctsCustId;
        params.CustId = custId;
        params.SubscriberId = subscriberId;
        params.EvidenceId = domElements.ddlPlaceEvidenceListEvidence.find('option:selected').val();
        params.EvidenceCode = domElements.ddlPlaceEvidenceListEvidence.find('option:selected').attr("code");
        params.Comment = domElements.txtPlaceEvidenceComment.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.btnPlaceEvidenceSubmit.prop('disabled', true);
                common.showLoading();
            },
            success: function (response) {
                common.showSuccess('Add the evidence successful!');
                domElements.divPlaceEvidence.dialog('close');
                var customerCategory = new customer_category();
                customerCategory.ReloadTab(ctsCustId);
                GetCustomerClassification(custId);
            },
            complete: function () {
                GetAccountEvidence(ctsCustId, username);
                common.closeLoading();
                domElements.btnPlaceEvidenceSubmit.prop('disabled', false);
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add the evidence fail!');
                common.closeLoading();
            }
        });
    }

    function AddQuickComment(text) {
        let maxLength = domElements.txtPlaceEvidenceComment.attr('maxlength');
        let currentText = domElements.txtPlaceEvidenceComment.val();        
        currentText += " " + text;

        if (currentText.length <= maxLength) 
            domElements.txtPlaceEvidenceComment.val(currentText);
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission || domElements.hdCustSubId.val() !== "0")
            domElements.btnChainToEvidenceFlagged.prop('disabled', true);
    }

    function GetCustomerClassification(custId) {
        setTimeout(function () {
            $.ajax(common.modifyRelativeUrl('Account/GetLastestCustomerClassification'), {
                type: 'POST',
                data: {
                    custId: custId
                },
                success: function (response) {
                    domElements.spanClass.html(response.customerClassification + "  ");
                },
                complete: function () {
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }, 10000);
    }

    this.ReloadAccountEvidence = function (ctsCustId, username) {
        GetAccountEvidence(ctsCustId, username);
    };

    this.Initial = function (ctsCustId, username, subscriberId, subscriber, custId, hostPermission) {
        domElements.cbbChainToEvidenceListEvidence.multiselect({
            menuWidth: 330,
            menuHeight: 330,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        DisableButton(hostPermission);

        InitialFlaggedGrid(ctsCustId, username, custId, subscriberId);

        InitialAffectedRetractGrid();

        GetAccountEvidence(ctsCustId, username);

        InitialPlaceEvidenceForm(ctsCustId, username, subscriberId, subscriber, custId);

        domElements.btnQuickComment.on('click', function () {
            AddQuickComment($(this).text());
        });
    };
}