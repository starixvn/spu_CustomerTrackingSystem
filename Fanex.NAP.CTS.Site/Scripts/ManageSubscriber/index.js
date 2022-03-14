function ManageSubscriber() {
    let dataTable;
    let isAddSub = 1;
    let detailedSiteCounter = 1;
    let backupEditInfo = null;
    let isSkipTracking = false;

    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        userId: $("#hdCTSCustId").val(),
        btnAdd: $('#btnAdd'),
        btnSave: $('#btnSave'),
        btnCancel: $('#btnCancel'),
        manageSubscriberTable: $('#manageSubscriberTable'),
        detailedSiteTable: $('#detailedSiteTable'),
        divDetailedSiteTable: $('#divDetailedSiteTable'),
        detailedSiteGroup: $('#detailedSiteGroup'),
        btnIconDelete: $('.sub-icon.-delete'),
        divAddSubscriber: $('#divAddSubscriber'),
        siteValidation: $('#siteValidation'),
        siteIDValidation: $('#siteIDValidation'),
        prefixValidation: $('#prefixValidation'),
        subNameValidation: $('#subNameValidation'),
        statisticIcon: $('#toggleAddSub1'),
        addSiteButton: $('#addSiteButton')
    };

    //Region: Main Page 
    function RenderGrid() {

        return domElements.manageSubscriberTable.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('ManageSubscriber/GetSubscribers'),
                data: function () {
                    let params = {};
                    params.isSkipTracking = isSkipTracking;
                    return params;
                },
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
            paging: false,
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
                    data: obj => HyperlinkSiteName(obj), createdCell: (cell, _, row) => {
                        $('.click-popup', cell).click(() => {                            
                                OpenSiteDetailTable(row);
                            });
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
                                    RemoveSubscriber(row.SubscriberID);
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

    function LoadGrid(isSkip) {
        $(this).prop('disabled', true);

        isSkipTracking = isSkip;

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = RenderGrid();
    } 

    function RenderEditUI(IsActive) {
        return IsActive === 1 ? html = '<span class="sub-icon delete test" id="sub-icon " title="Edit Subscriber"><i class="icon-edit ' + (domElements.isHostPermission === "False" ? ' disabled' : '') + '"></i></span>' : "";
    }

    function RenderRemoveUI(IsActive, TerminatedDateString) {
        return IsActive === 1 ?
            '<span class="sub-icon delete test" id="sub-icon " title="Remove Subscriber"><i class="icon-delete ' + (domElements.isHostPermission === "False" ? ' disabled' : '') + '"></i></span>'
            : '<span>' + TerminatedDateString + '</span>';
    }
    
    function HyperlinkSiteName(obj) {
        return (obj.SiteCount > 1) ? "<a class='click-popup'>" + obj.SiteName + "</a>" : obj.SiteName;
    }

    function OpenSiteDetailTable(row) {            
        LoadDetailedSiteGrid(row);
    }

    function RemoveSubscriber(SubscriberID) {
        var url = common.modifyRelativeUrl('ManageSubscriber/RemoveSubscriber');
        var params = {};
        params.subscriberID = SubscriberID;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess('Remove subscriber successful!', 1000);
                    LoadGrid(true);
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

    function IsDisableAddButton() {
        if (domElements.isHostPermission === "False") {
            domElements.btnAdd.prop('disabled', true);
            domElements.btnAdd.addClass('disabled');
        }
        else {
            domElements.btnAdd.prop('disabled', false);
            domElements.btnAdd.removeClass('disabled');
        }
    }
    
    function LoadDetailedSiteGrid(row) {
        $.ajax(common.modifyRelativeUrl('ManageSubscriber/GetDetailedSite'), {
            type: 'GET',
            data: { subscriberID: row.SubscriberID },
            beforeSend: function () {
                domElements.divDetailedSiteTable.dialog({
                    modal: true,
                    width: '900px',
                    title: row.SubscriberNameValidation,
                    resizable: false,
                    draggable: true,
                    close: function () {
                        domElements.detailedSiteTable.DataTable().clear().draw(true);
                        $(this).dialog('close');
                    }
                });

                domElements.detailedSiteTable.find('tbody').addClass('loading');
            },
            success: function (response) {
               domElements.detailedSiteTable.DataTable().clear().rows.add(response).draw(true);             
            },
            complete: function () {
               domElements.detailedSiteTable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
               domElements.detailedSiteTable.find('tbody').removeClass('loading');
               console.log(errMsg);
            }
        });
    }

    //Region: Pop-up Implementation (Edit - Insert - Delete Functions)
    function AddSite(siteName) {
        CollapseSite();
        detailedSiteCounter++;
        domElements.addSiteButton.before(CreateAddedSiteHTML());

        $('#toggleAddSub' + detailedSiteCounter).on('click', function () {
            $(this).closest('.border-form-toggle').find('.group-toggle-btn').toggleClass("hide");
            $(this).toggleClass("icon-open");
        });

        $('#btnDeletedSite' + detailedSiteCounter).on('click', function () {
            if (siteName !== undefined) {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure to delete Site " + siteName + "?</div></div >", () => {
                    $(this).parents()[2].remove();
                    detailedSiteCounter--;
                }, () => {
                    return;
                });
            }
            else {
                $(this).parents()[2].remove();
                detailedSiteCounter--;
            }
        });
    }

    function CreateAddedSiteHTML()
    {
        var html =
            '<div class="border-form-toggle" id="addedDetailedSite' + detailedSiteCounter +'">' 
            + '<div class="form-control mbform">'
            + '<div class="ischeckInput">'
            + '<div class="group-input">'
            + '<div class="label"><span class="toggle-add-sub icon-open" id="toggleAddSub' + detailedSiteCounter + '"><i class="icon icon-chevron-right"></i></span> Site</div>'
            + '<input id="txtSite' + detailedSiteCounter +'" type="text" class="input-field full-width" maxlength="50">'
            + '</div>'
            + '<button class="deleted-input" id="btnDeletedSite' + detailedSiteCounter + '">'
            + '<i class="icon-delete"></i>'
            + '</button>'
            + '</div>'
            + '<p class="validate-form" id="siteValidation' + detailedSiteCounter +'" hidden>Please fill out this field</p>'
            + '</div>'
            + '<div id="statistic-detail' + detailedSiteCounter +'" class="group-toggle-btn">'
            + ' <div class="form-control mbform">'
            + '<div class="ischeckInput">'
            + '<div class="group-input">'
            + '<div class="label">Site ID</div>'
            + '<input id="txtSiteID' + detailedSiteCounter +'" type="number" class="input-field full-width" maxlength="50">'
            + '</div>'
            + '</div>'
            + '<p class="validate-form" id="siteIDValidation' + detailedSiteCounter +'" hidden>Please fill out this field</p>'
            + '</div>'
            + '<div class="form-control mbform">'
            + '<div class="ischeckInput">'
            + '<div class="group-input">'
            + '<div class="label">Role Mapping</div>'
            + '<select class="control full-width" id="txtRoleMapping' + detailedSiteCounter +'">'
            + '<option value="0" selected="">0</option>'
            + '<option value="1">1</option>'
            + '<option value="2">2</option>'
            + '</select>'
            + '</div>'
            + '</div>'
            + '</div>'
            + '</div>'
            + '</div>';

        return html;
    }

    function DisplayEditSubscriberForm(rowInfo) {
        isAddSub = 0;
        GetSubscriberInfoToEdit(rowInfo);
    }

    function GetSubscriberInfoToEdit(rowInfo) {
        backupEditInfo = {
            SubscriberID: rowInfo.SubscriberID,
            SubscriberName: rowInfo.SubscriberNameValidation,
            IsTest: rowInfo.IsTest === 1 ? true : false,
            SubscriberType: rowInfo.SubscriberGroupName,
            IBCUserNamePrefix: rowInfo.SubscriberPrefixValidation,
            SiteListJson: null,
            Message: null
        };

        $('#txtSubscriberId').val(rowInfo.SubscriberID);
        $('#txtSubscriberName').val(rowInfo.SubscriberNameValidation);
        $('#txtSubscriberType').val(rowInfo.SubscriberGroupName === "Credit" ? 0 : 1);
        $('#txtIBCUserNamePrefix').val(rowInfo.SubscriberPrefixValidation);
        $('#overwriteCheckbox').prop('checked', rowInfo.IsTest === 1 ? true : false);  

        if (rowInfo.SiteCount > 1) {
            GetSites(rowInfo.SubscriberID, rowInfo.SiteCount);
        }
        else {
            backupEditInfo.SiteListJson = {
                SiteID: rowInfo.SiteName,
                SiteName: rowInfo.SiteID,
                RoleMapping: rowInfo.RoleMapping
            };

            $('#txtSite1').val(rowInfo.SiteName);
            $('#txtSiteID1').val(rowInfo.SiteID);
            $('#txtRoleMapping1').val(rowInfo.RoleMapping);  

            detailedSiteCounter = rowInfo.SiteCount;

            domElements.divAddSubscriber.dialog({
                modal: true,
                width: '500px',
                title: 'EDIT SUBSCRIBER',
                resizable: true,
                draggable: true,
                close: function () {            
                    RefreshInputData();
                    detailedSiteCounter = 1;
                    $(this).dialog('close');
                }
            });
        }

    }

    function GetSites(subscriberID, siteCount) {
        $.ajax(common.modifyRelativeUrl('ManageSubscriber/GetDetailedSite'), {
            type: 'GET',
            data: { subscriberID: subscriberID },
            success: function (response) {
                LoadMoreDetailedSite(response, siteCount);
            },
            complete: function () {
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadMoreDetailedSite(response, siteCount) {
        backupEditInfo.SiteListJson = response;

        for (let i = 1; i <= siteCount; i++)
        {
            if (i > 1) AddSite(response[i - 1].SiteName);
            $('#txtSite' + i).val(response[i-1].SiteName);
            $('#txtSiteID' + i).val(response[i-1].SiteID);
            $('#txtRoleMapping' + i).val(response[i-1].RoleMapping);  
        }

        $('#toggleAddSub' + siteCount).removeClass("icon-open");
        $('#statistic-detail' + siteCount).addClass("hide");

        domElements.divAddSubscriber.dialog({
            modal: true,
            width: '500px',
            title: 'EDIT SUBSCRIBER',
            resizable: true,
            draggable: true,
            close: function () {
                RefreshInputData();
                $(this).dialog('close');
            }
        });
    }

    function RefreshInputData() {
        $('#txtSubscriberName').val("");
        $('#txtSubscriberType').val("");
        $('#txtIBCUserNamePrefix').val(""); 
        $('#txtSubscriberId').val(0);
        $('#overwriteCheckbox').prop('checked', false);

        for (let i = 1; i <= detailedSiteCounter; i++) {
            if (i === 1) {
                $('#txtSite' + i).val("");
                $('#txtSiteID' + i).val("");
                $('#txtRoleMapping' + i).val(0);
            }
            else {
                $('#btnDeletedSite' + i).parents()[2].remove();
            }
        }

        domElements.prefixValidation.hide();
        domElements.subNameValidation.hide();
        for (let i = 1; i <= detailedSiteCounter; i++) {
            $('#siteValidation' + i).hide();
            $('#siteIDValidation' + i).hide();
        }

        detailedSiteCounter = 1;
        //backupEditInfo
    }

    function CollapseSite()
    {
        for (let i = 1; i <= detailedSiteCounter; i++) {
            $('#toggleAddSub' + i).removeClass("icon-open");
            $('#statistic-detail' + i ).addClass("hide");
        }
    }

    function InsertSubscriber() {
        let subscriberInsertModel = GetInputUser();

        $.ajax(common.modifyRelativeUrl('ManageSubscriber/UpdateSubscriber'), {
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
                        LoadGrid(true);
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

        $.ajax(common.modifyRelativeUrl('ManageSubscriber/UpdateSubscriber'), {
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
                        LoadGrid(true);
                    }, 1000);
                } else {
                    common.showError("Edit subscriber fail! - " + response.ErrorMsg);
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

    function GetInputUser() {
        backupEditInfo;

        let rawSiteList = [];

        for (let i = 1; i <= detailedSiteCounter; i++) {
            let obj = {
                SiteID: $('#txtSiteID' + i).val().trim(),
                SiteName: $('#txtSite' + i).val().trim(),
                RoleMapping: $('#txtRoleMapping' + i).val()
            };
            rawSiteList.push(obj);
        }

        let subscriberInsertModel = {
            SubscriberID: $('#txtSubscriberId').val(),
            SubscriberName: $('#txtSubscriberName').val().trim(),
            IsTest: $('#overwriteCheckbox').prop('checked') ? "1" : "0",
            SubscriberType: $('#txtSubscriberType').val(),
            IBCUserNamePrefix: $('#txtIBCUserNamePrefix').val().trim(),
            SiteListJson: JSON.stringify(rawSiteList)                     
        };

        return subscriberInsertModel;
    }

    function IsInputValidation() {
        let invalidElement = 0;

        if (($('#txtIBCUserNamePrefix').val() !== undefined && $('#txtIBCUserNamePrefix').val().trim() !== "") || detailedSiteCounter > 1)
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

        for (let i = 1; i <= detailedSiteCounter; i++) {
            if ($('#txtSite' + i).val() !== undefined && $('#txtSite' + i).val().trim() !== "")
                $('#siteValidation' + i).hide();
            else {
                $('#siteValidation' + i).show();
                invalidElement++;
            }

            if ($('#txtSiteID' + i).val() !== undefined && $('#txtSiteID' + i).val().trim() !== "")
                $('#siteIDValidation' + i).hide();
            else {
                $('#siteIDValidation' + i).show();
                invalidElement++;
            }
        }

        return (invalidElement === 0) ? true : false;
    }

    function DoInsertOrEdit() {
        if (!IsInputValidation()) {
            return;
        }

        isAddSub === 1 ? InsertSubscriber() : EditSubscriber();
    }

    function InitDetailSiteTable() {
        domElements.detailedSiteTable.DataTable({
            serverSide: false,           
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "simple",
            language: {
                lengthMenu: "",
                paginate: {
                },
                info: "",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            paging: false,
            scrollX: false,
            scrollCollapse: false,            
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2], className: "text-center" },
                { targets: [0] },
                { targets: [1] },
                { targets: [2] }
            ],
            columns: [
                { data: obj => obj.RoleMapping },
                { data: obj => obj.SiteID },
                { data: obj => obj.SiteName }
            ]
        });
    }

    //Region: Initialization
    function Initial() {
        LoadGrid(false);

        InitDetailSiteTable();

        IsDisableAddButton();

        domElements.btnAdd.on('click', function () {
            RefreshInputData();
            isAddSub = 1;

            domElements.divAddSubscriber.dialog({
                modal: true,
                width: '430px',
                title: 'ADD SUBSCRIBER',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                    RefreshInputData();
                }
            });
        });

        domElements.statisticIcon.on('click', function () {
            $(this).closest('.border-form-toggle').find('.group-toggle-btn').toggleClass("hide");
            $(this).toggleClass("icon-open");
        });
  
        domElements.btnCancel.on('click', () => {
            domElements.divAddSubscriber.dialog('close');
        });

        domElements.btnSave.on('click', () => DoInsertOrEdit());

        domElements.addSiteButton.on('click', () => AddSite());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ManageSubscriber);