function ManageSubscriber() {
    var isPreviousStatisticOpen = false;
    let dataTable;
    let isAddSub = 1;
    let subID = -1;
    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        userId: $("#hdCTSCustId").val(),
        btnAdd: $('#btnAdd'),
        btnSave: $('#btnSave'),
        btnCancel: $('#btnCancel'),
        manageSubscriberTable: $('#manageSubscriberTable'),
        btnIconDelete: $('.sub-icon.-delete'),
        divAddSubscriber: $('#divAddSubscriber'),
        siteValidation: $('#siteValidation'),
        siteIDValidation: $('#siteIDValidation'),
        prefixValidation: $('#prefixValidation'),
        subNameValidation: $('#subNameValidation'),
        childTable: $('#childTable'),
        statisticIcon: $('.toggle-add-sub'),

    };

    function InsertSubscriber() {
        let subscriberInsertModel = GetInputUser();

        $.ajax(common.modifyRelativeUrl('ManageSubscriber/InsertSubscriber'), {
            type: 'POST',
            data: subscriberInsertModel,
            beforeSend: function () {
                domElements.divAddSubscriber.addClass("loading-block");
            },
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess("Add subscriber successful");
                    RefreshInputData();
                    domElements.divAddSubscriber.dialog('close');
                    setTimeout(() => {
                        LoadGrid();
                    }, 1000);
                } else {
                    common.showError("Add subscriber fail! - " + response.ErrorMsg);
                }
            },
            complete: function () {
                domElements.divAddSubscriber.removeClass("loading-block");
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function EditSubscriber() {
        let subscriberInsertModel = GetInputUser();

        $.ajax(common.modifyRelativeUrl('ManageSubscriber/EditSubscriber'), {
            type: 'POST',
            data: subscriberInsertModel,
            beforeSend: function () {
                domElements.divAddSubscriber.addClass("loading-block");
            },
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess("Edit subscriber successful");
                    RefreshInputData();
                    domElements.divAddSubscriber.dialog('close');
                    setTimeout(() => {
                        LoadGrid();
                    }, 1000);
                } else {
                    common.showError("Edit subscriber fail! - " + response.ErrorMsg);
                }
            },
            complete: function () {
                domElements.divAddSubscriber.removeClass("loading-block");
                subID = -1;
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetInputUser()
    {
        let subscriberInsertModel = {
            SiteID: $('#txtSiteID').val().trim(),
            Site: $('#txtSite').val().trim(),
            IBCUserNamePrefix: $('#txtIBCUserNamePrefix').val().trim(),
            SubscriberType: $('#txtSubscriberGroup').val(),
            SubscriberID: subID,
            SubscriberName: $('#txtSubscriberName').val().trim(),
            RoleMapping: $('#txtRoleMapping').val(),
            IsTest: $('#overwriteCheckbox').prop('checked') ? "1" : "0"
        };

        return subscriberInsertModel;
    }

    function IsInputValidation() {
        let invalidElement = 0;

        if ($('#txtSite').val() !== undefined && $('#txtSite').val().trim() !== "")
            domElements.siteValidation.hide();
        else {
            domElements.siteValidation.show();
            invalidElement++;
        }

        if ($('#txtSiteID').val() !== undefined && $('#txtSiteID').val().trim() !== "")
            domElements.siteIDValidation.hide();
        else {
            domElements.siteIDValidation.show();
            invalidElement++;
        }

        if ($('#txtIBCUserNamePrefix').val() !== undefined && $('#txtIBCUserNamePrefix').val().trim() !== "")
            domElements.prefixValidation.hide();
        else {
            domElements.prefixValidation.show();
            invalidElement++;
        }

        if ($('#txtSubscriberName').val() !== undefined && $('#txtSubscriberName').val().trim() !== "")
            domElements.subNameValidation.hide();
        else {
            domElements.subNameValidation.show();
            invalidElement++;
        }

        return (invalidElement === 0) ? true : false;
    }

    function CheckBusinessValidationToInsertOrEdit() {
        let params = {};
        params.prefixSubscriberName = $('#txtIBCUserNamePrefix').val().trim();

        $.ajax(common.modifyRelativeUrl('ManageSubscriber/VerifySubscriberToInsert'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.divAddSubscriber.addClass("loading-block");
            },
            success: function (response) {
                if (response.IsSubscriberPrefixValidation === 1) {
                    common.showConfirmation("<div class='popup-content'><div class='popup-title'>There is a subscriber with this IBCUserName_Prefix. Please check before submit. </div>", function () {
                        isAddSub === 1 ? InsertSubscriber() : EditSubscriber();
                    }, () => {
                        $('td #sub-icon').removeClass('none-pointer');
                        domElements.divAddSubscriber.removeClass("loading-block");
                    });
                } else {
                    isAddSub === 1? InsertSubscriber() : EditSubscriber();
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function DoInsertOrEdit() {
        if (!IsInputValidation()) {
            return;
        }
        CheckBusinessValidationToInsertOrEdit();
    }

    function RenderGrid() {

        return domElements.manageSubscriberTable.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('ManageSubscriber/GetSubscribers'),
                data: {},
                type: "POST",
                beforeSend: function () {
                    domElements.manageSubscriberTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    domElements.manageSubscriberTable.find('tbody').removeClass('loading');
                },
                error: function (error) {
                    domElements.manageSubscriberTable.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "simple",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "",
                    last: "",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 1000,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], className: "text-center" },
                { targets: [0], width: "2%" },
                { targets: [1], width: "9%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "9%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "9%" },
                { targets: [6], width: "9%"},
                { targets: [7], width: "9%"},
                { targets: [8], width: "9%" },
                { targets: [9], width: "9%" },
                { targets: [10], width: "10%" }
            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => obj.SubscriberID },
                { data: obj => obj.SubscriberNameValidation },
                { data: obj => obj.SubscriberPrefixValidation },
                { data: obj => obj.SubscriberGroupName },
                { data: obj => obj.RoleMapping },
                { data: obj => obj.SiteID },
                {
                    data: obj => LoadPopup(obj), createdCell: (cell, _, row) => {
                        if (domElements.isHostPermission === "True") {
                            $('.click-popup', cell).click(() => {
                                childTable(row);
                            });
                        }
                    }
                },
                { data: obj => obj.IsActive },
                { data: obj => obj.CreatedDateString },
                {
                    data: obj => RenderEditUI(obj.IsActive), createdCell: (cell, _, row) => {
                        if (domElements.isHostPermission === "True") {
                            $('.icon-edit', cell).click(() => {
                                DisplayEditSubscriberForm(row);
                            });
                        }
                    }
                },
                {
                    data: obj => () => RenderRemoveUI(obj.IsActive, obj.TerminatedDateString), createdCell: (cell, _, row) => {
                        if (domElements.isHostPermission === "True") {
                            $('.icon-delete', cell).click(() => {
                                $('td #sub-icon').addClass('none-pointer');
                                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, this account will not automatically update by our service in the future </div>", function () {
                                    RemoveSubscriber(row.SubscriberID, row.SiteID);
                                }, () => {
                                    $('td #sub-icon').removeClass('none-pointer');
                                });
                            });
                        }
                    }
                }
            ],
            createdRow: function (row, data, index) {             
                if (data.IsActive === 0) 
                    $(row.cells).addClass('deleted');  //grey color 
            }
        });
    }


    function LoadPopup(obj) {
        let html = "<a class='click-popup'>" + obj.SiteName + "</a>";
        return html;
    }

    function childTable() {
        domElements.childTable.dialog({
            modal: true,
            width: '900px',
            title: 'SanJi',
            resizable: true,
            draggable: true,
            close: function () {
                $(this).dialog('close');
            }
        });
    }
    

    function DisplayEditSubscriberForm(rowInfo)
    {
        isAddSub = 0;
        DisplaySubscriberInfoToEdit(rowInfo);
        IsInputValidation();

        domElements.divAddSubscriber.dialog({
            modal: true,
            width: '500px',
            title: 'EDIT SUBSCRIBER',
            resizable: true,
            draggable: true,
            close: function () {
                $(this).dialog('close');
            }
        });
    }

    function RenderEditUI(IsActive) {
        return IsActive === 1 ? html = '<span class="sub-icon delete test" id="sub-icon " title="Edit Subscriber"><i class="icon-edit ' + (domElements.isHostPermission === "False" ? ' disabled' : '') + '"></i></span>' : "";
    }

    function RenderRemoveUI(IsActive, TerminatedDateString) {
        return IsActive === 1 ?
            '<span class="sub-icon delete test" id="sub-icon " title="Remove Subscriber"><i class="icon-delete ' + (domElements.isHostPermission === "False" ? ' disabled' : '') + '"></i></span>'
            : '<span>' + TerminatedDateString + '</span>';
    }

    function RemoveSubscriber(SubscriberID, SiteID) {
        var url = common.modifyRelativeUrl('ManageSubscriber/RemoveSubscriber');
        var params = {};
        params.subscriberID = SubscriberID;
        params.siteID = SiteID;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess('Remove subscriber successful!', 1000);
                    LoadGrid();
                }
                else {
                    common.showError('Remove subscriber fail! - ' + response.ErrorMsg);
                }
            },
            complete: function () {
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function RefreshInputData()
    {
        $('#txtSite').val("");     
        $('#txtSiteID').val("");      
        $('#txtIBCUserNamePrefix').val("");      
        $('#txtSubscriberName').val("");
        $('#overwriteCheckbox').prop('checked', false);
        domElements.siteValidation.hide();
        domElements.siteIDValidation.hide();
        domElements.prefixValidation.hide();
        domElements.subNameValidation.hide();
    }

    function DisplaySubscriberInfoToEdit(rowInfo) {
        $('#txtSite').val(rowInfo.SiteName);
        $('#txtSiteID').val(rowInfo.SiteID);
        $('#txtIBCUserNamePrefix').val(rowInfo.SubscriberPrefixValidation);
        $('#txtSubscriberGroup').val(rowInfo.SubscriberGroupName === "Credit" ? 0 : 1);
        $('#txtSubscriberName').val(rowInfo.SubscriberNameValidation);
        $('#txtRoleMapping').val(rowInfo.RoleMapping);    
        $('#overwriteCheckbox').prop('checked', rowInfo.IsTest === 1 ? true : false);
        subID = rowInfo.SubscriberID;
    }

    function IsDisableAddButton() {
        if (domElements.isHostPermission === "False") {
            domElements.btnAdd.prop('disabled', true);
        }
        else 
            domElements.btnAdd.prop('disabled', false);
    }

    function LoadGrid() {
        $(this).prop('disabled', true);
        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = RenderGrid();
    }

    function Initial() {
        LoadGrid();

        IsDisableAddButton();

        domElements.btnAdd.on('click', function () {
            RefreshInputData();
            isAddSub = 1;
            domElements.divAddSubscriber.dialog({
                modal: true,
                width: '500px',
                title: 'ADD SUBSCRIBER',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });
        });







        domElements.statisticIcon.on('click', function () {
            $(this).closest('.group-border').find('.group-toggle-btn').toggleClass("hide");
            $(this).toggleClass("icon-open");
        });

        domElements.btnCancel.on('click', () => {
            domElements.divAddSubscriber.dialog('close');
        });

        domElements.btnSave.on('click', () => DoInsertOrEdit());

        $('#txtSite').on('input', () => ($('#txtSite').val() !== undefined && $('#txtSite').val().trim() !== "") ? domElements.siteValidation.hide() : domElements.siteValidation.show());
        $('#txtSiteID').on('input', () => ($('#txtSiteID').val() !== undefined && $('#txtSiteID').val().trim() !== "") ? domElements.siteIDValidation.hide() : domElements.siteIDValidation.show());
        $('#txtIBCUserNamePrefix').on('input', () => ($('#txtIBCUserNamePrefix').val() !== undefined && $('#txtIBCUserNamePrefix').val().trim() !== "") ? domElements.prefixValidation.hide() : domElements.prefixValidation.show());
        $('#txtSubscriberName').on('input', () => ($('#txtSubscriberName').val() !== undefined && $('#txtSubscriberName').val().trim() !== "") ? domElements.subNameValidation.hide() : domElements.subNameValidation.show());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ManageSubscriber);