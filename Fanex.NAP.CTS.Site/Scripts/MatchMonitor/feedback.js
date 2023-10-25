function feedback() {

    let domElementFeedbacks = {
        divIncorrectReasonFeedback: $('#divIncorrectReasonFeedback'),
        hdMMUserCommentID: $('#hdMMUserCommentID'),
        btnReasonFeedback: $('#btnReasonFeedback'),
        grpInCorrectReason: $('#grpInCorrectReason'),
        grpRadioCorrectReason: $('#grpRadioCorrectReason'),
        txtComment: $('#txtComment'),
        btnCancel: $('#btnCancel'),
        btnSave: $('#btnSave'),
        btnSaveAndClose: $('#btnSaveAndClose'),
        tblFeedback: $('#tblFeedback')
    }

    let parameter = {
        MatchID: null,
        IsLive: null,
        BetTypeID: null,
        BetID: null,
        SportType: null
    }

    function LoadIncorrectReasonFeedback() {       
        domElementFeedbacks.divIncorrectReasonFeedback.dialog({
            modal: true,
            width: '1060px',
            title: 'MATCH MONITOR - INCORRECT REASON FEEDBACK',
            resizable: false,
            draggable: true,
            close: function () {
                ResetInputFeedback();
                ClearElement();
                $(this).dialog('close');           
            }
        });

        domElementFeedbacks.btnCancel.off('click').on('click', function () {            
            domElementFeedbacks.divIncorrectReasonFeedback.dialog('close');
        });

        domElementFeedbacks.btnSave.off('click').on('click', function () {                       
            SaveUserFeedback(false);           
        }); 

        domElementFeedbacks.btnSaveAndClose.off('click').on('click', function () {                     
            SaveUserFeedback(true);           
        }); 
      
        LoadGrid();

        ReloadGrid();
    }

    function BuildGroupIncorrectReason(reasonList) {       
        if (reasonList.length > 0) {
            $('.flex-item', domElementFeedbacks.grpInCorrectReason).each(function (idx, ele) {
                let reason = $(ele).attr('reason');
                if (reason != "-1") {
                    if (reasonList.indexOf(reason) == -1) {
                        $(ele).addClass('hidden');
                    } else {
                        $(ele).removeClass('hidden');
                    }
                }                
            });
        } else {
            $('.flex-item', domElementFeedbacks.grpInCorrectReason).each(function (idx, ele) {
                $(ele).addClass('hidden');
            });
        }
    }

    function SaveUserFeedback(isClosePopup) {
        let paras = parameter;
        
        paras.IncorrectReason = GetInputIncorrectReason();     
        paras.CorrectReasonID = $('input[name=CorrectReason]:checked', domElementFeedbacks.grpRadioCorrectReason).attr('correctreasonid');
        paras.UserComment = domElementFeedbacks.txtComment.val().trim();

        let url = 'MatchMonitor/InsertUserFeedback';

        if (domElementFeedbacks.btnSave.hasClass('edit')) {
            url = 'MatchMonitor/UpdateUserFeedback';
            paras.MMUserCommentID = domElementFeedbacks.hdMMUserCommentID.val();
        }

        $.ajax(common.modifyRelativeUrl(url), {
            type: 'POST',
            data: paras,
            beforeSend: function () {
                domElementFeedbacks.tblFeedback.find('tbody').addClass('loading');
            },
            success: function (response) {
                common.showSuccess("Thanks very much. We're grateful for your feedback.");

                domElementFeedbacks.btnSave.removeClass('edit');
                if (!isClosePopup) {
                    ReloadGrid();
                    ResetInputFeedback();
                }                               
            },
            complete: function () {
                domElementFeedbacks.tblFeedback.find('tbody').removeClass('loading');                
            },
            error: function (errMsg) {
                domElementFeedbacks.tblFeedback.find('tbody').removeClass('loading');
                console.log(errMsg);
            }
        })

        if (isClosePopup) {
            domElementFeedbacks.divIncorrectReasonFeedback.dialog('close');
        }
        
    }

    function GetInputIncorrectReason() {
        let incorrectReasons = [];        
        
        $('.flex-item', domElementFeedbacks.grpInCorrectReason).each(function (idx, ele) {

            $('input[name=incorrectreason]:checked', ele).each(function (chkIdx, chkEle) {
                let eleReason = $(chkEle).attr('incorrectreasonid');
                let eleInputFrom = $('input[name=from]', ele).val().trim();
                let eleInputTo = $('input[name=to]', ele).val().trim();

                incorrectReasons.push({
                    Reason: eleReason,
                    FromTransInfo: eleInputFrom,
                    ToTransInfo: eleInputTo
                });

            })
        });

        if (incorrectReasons.length > 0) {
            return JSON.stringify(incorrectReasons);
        }
        return -1;
    }

    function LoadUpdateUserFeedback(obj) {        
        domElementFeedbacks.btnSave.addClass('edit');
        
        if (obj.UserComment == "-") {
            domElementFeedbacks.txtComment.val("");
        } else {
            domElementFeedbacks.txtComment.val(obj.UserComment);
        }

        domElementFeedbacks.hdMMUserCommentID.val(obj.MMUserCommentID);
        

        $('input[name=CorrectReason]', domElementFeedbacks.grpRadioCorrectReason).each(function (idx, ele) {
            if ($(ele).val().toString() == obj.CorrectReason) {
                $(ele).prop('checked', true);               
            }
        });

        if (obj.IncorrectReason.length > 0) {
            let arrIncorrectReason = obj.IncorrectReasonRaw.split(',');
            $('.flex-item', domElementFeedbacks.grpInCorrectReason).each(function (idx, ele) {
                if (arrIncorrectReason.indexOf($(ele).attr('reasonName')) != -1) {
                    $('input[name=incorrectreason]', ele).prop('checked', true);                    
                    $('input[name=from]', ele).val(obj.FromTransInfo);
                    $('input[name=to]', ele).val(obj.ToTransInfo);
                }
            });
        }
    }

    function DeleteUserFeedback(mmUserCommentID) {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/DeleteUserFeedback'), {
            type: 'POST',
            data: {
                MMUserCommentID: mmUserCommentID
            },
            beforeSend: function () {
                domElementFeedbacks.tblFeedback.find('tbody').addClass('loading');
            },
            success: function (response) {
                common.showSuccess("Thanks very much. We're grateful for your feedback.");

                ReloadGrid();                
            },
            complete: function () {
                domElementFeedbacks.tblFeedback.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                domElementFeedbacks.tblFeedback.find('tbody').removeClass('loading');
                console.log(errMsg);
            }
        })
    }

    function LoadGrid() {
        $(this).prop('disabled', true);

        RenderGrid();
    }

    function ReloadGrid() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetUserFeedback'), {
            type: 'POST',
            data: parameter,
            beforeSend: function () {
                domElementFeedbacks.tblFeedback.find('tbody').addClass('loading');
            },
            success: function (response) {
                domElementFeedbacks.tblFeedback.DataTable().clear().rows.add(response.data).draw(true);
                domElementFeedbacks.tblFeedback.DataTable().columns.adjust();
            },
            complete: function () {
                domElementFeedbacks.tblFeedback.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                domElementFeedbacks.tblFeedback.find('tbody').removeClass('loading');               
                console.log(errMsg);
            }
        })
    }

    function RenderGrid() {
        domElementFeedbacks.tblFeedback.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                zeroRecords: "There's no data",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                }
            },
            info: false,
            ordering: false,
            paging: false,
            searching: false,
            scrollX: false,       
            scrollCollapse: true,
            scrollY: '20vh',
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], width: "5%", className: "text-center", defaultContent: "-" },
                { targets: [1], width: "12%", className: "text-center", defaultContent: "-" },
                { targets: [2], width: "12%", className: "text-center", defaultContent: "-" },
                { targets: [3], width: "30%", className: "text-center", defaultContent: "-" },
                { targets: [4], width: "12%", className: "text-center", defaultContent: "-" },
                { targets: [5], width: "12%", className: "text-center", defaultContent: "-" },
                { targets: [6], width: "12%", className: "text-center", defaultContent: "-" },
                { targets: [7], width: "5%", className: "text-center", defaultContent: "-" }
            ],
            columns: [
                { render: function (data, type, full, meta) { return meta.row + 1; } },
                { data: obj => obj.IncorrectReason },
                { data: obj => obj.CorrectReason },
                {
                    data: obj => {
                        if (obj.UserComment.length > 100) {
                            let comment = obj.UserComment.substring(0, obj.UserComment.length - 100) + ' ...';

                            return `<span>${comment}</span>`;
                        } else {
                            return `<span>${obj.UserComment}</span>`;
                        }
                    }
                },
                {
                    data: obj => {
                        if (obj.FromTransInfo != '' || obj.ToTransInfo != '') {
                            return obj.FromTransInfo + " -> " + obj.ToTransInfo
                        } else {
                            return '-';
                        }
                    }
                },
                { data: obj => obj.LastModifiedBy },
                { data: obj => obj.LastModifiedDate },
                {
                    data: obj => {
                        let htmlSPANEdit = '<span class="sub-icon - edit" id="sub-icon " title="Edit"><i class="icon-edit"></i></span>';
                        let htmlSPANRemove = '<span class="sub-icon - delete" id="sub-icon " title="Remove"><i class="icon-delete"></i></span>';

                        return htmlSPANEdit + htmlSPANRemove;
                    }, createdCell: (cell, _, row) => {
                        $('.icon-edit', cell).click(() => {
                            $('td #sub-icon').addClass('none-pointer');

                            LoadUpdateUserFeedback(row);
                        });

                        $('.icon-delete', cell).click(() => {
                            $('td #sub-icon').addClass('none-pointer');
                            let html = "<div class='popup - content'>Are you sure you want to remove this feedback? </div>";

                            common.showConfirmation(html, function () {
                                DeleteUserFeedback(row.MMUserCommentID);
                            }, () => {
                                $('td #sub-icon').removeClass('none-pointer');
                            });
                        });
                    }
                }
            ]
        })
    }

    function ResetInputFeedback() {
        domElementFeedbacks.txtComment.val('');

        $('input[id=COthers]', domElementFeedbacks.grpRadioCorrectReason).prop('checked', true);

        $('.flex-item', domElementFeedbacks.grpInCorrectReason).each(function (idx, ele) {
            $('input[name=incorrectreason]', ele).prop('checked', false);
            $('input[name=from]', ele).val('');
            $('input[name=to]', ele).val('');
        });
    }

    function ClearElement() {
        domElementFeedbacks.tblFeedback.DataTable().clear();
        domElementFeedbacks.tblFeedback.DataTable().destroy(false);
      
        parameter = {
            MatchID: null,
            IsLive: null,
            BetTypeID: null,
            BetID: null,
            SportType: null
        }
    }
   
    this.Initial = function (matchID, isLive, betTypeID, betID, sportType, reasonList) {

        parameter.MatchID = matchID;
        parameter.IsLive = isLive;
        parameter.BetTypeID = betTypeID;
        parameter.BetID = betID;
        parameter.SportType = sportType;
        parameter.ReasonList = reasonList;

        BuildGroupIncorrectReason(reasonList);

        LoadIncorrectReasonFeedback();
        
    }

}