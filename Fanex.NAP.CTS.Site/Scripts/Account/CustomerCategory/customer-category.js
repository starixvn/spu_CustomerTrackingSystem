function customer_category() {
    let btnMarkCategory = $('#btnmarkcategory');
    let divMarkCategory = $('#divMarkCategory');
    let tblcustomercategory = $('#customer-category-table');
    let isHostPermission;
    let inputSwitchCategorySummary = $('#account-switch');
    let divToogleSwitch = $('#divToogleSwitch');
    let chooseExternalCategory = "ChooseExternalCategory";

    function BindingMarkCategoryForAddNew(custId, ctsCustId, subscriberId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            AddCustomerClassification(custId, ctsCustId, subscriberId);
        });

        $("#ddlcategory").select2();

        $("#ddlsports").multiselect({
            menuWidth: 340,
            selectedList: 3,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        });
    }

    function BindingMarkCategoryForUpdate(custId, ctsCustId, subscriberId, categoryId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            UpdateCustomerClassification(custId, ctsCustId, subscriberId, categoryId);
        });

        $("#ddlsports").multiselect({
            selectedList: 3,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        });
    }

    function AddCustomerClassification(custId, ctsCustId, subscriberId) {
        if ($('#ddlsports').val() === '') {

            common.showError('Sport is required.');
            return;
        }

        var url = common.modifyRelativeUrl('/Account/AddCustomerClassification');
        var params = {};
        params.custId = custId;
        params.ctsCustId = ctsCustId;
        params.subscriberId = subscriberId;
        params.categoryId = $('#ddlcategory').val();
        params.sportGroupIds = $('#ddlsports').val().join(',');
        params.remark = $('#txtcategorycomment').val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Add the category successful!', 2000);
                    divMarkCategory.dialog('close');

                    tblcustomercategory.DataTable().ajax.reload();
                }
                else {
                    common.showError(response.errorMessage);
                }
            },
            complete: function () {

            }
        });
    }

    function RemoveCustomerClassification(custId, ctsCustId, categoryId) {
        var url = common.modifyRelativeUrl('/Account/RemoveCustomerClassification');
        var params = {};
        params.custId = custId;
        params.ctsCustId = ctsCustId;
        params.categoryId = categoryId;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Remove the category successful!', 2000);

                    tblcustomercategory.DataTable().ajax.reload();
                }
                else {
                    common.showError('Remove the category fail!');
                }
            },
            complete: function () {
            }
        });
    }

    function UpdateCustomerClassification(custId, ctsCustId, subscriberId, categoryId) {
        var url = common.modifyRelativeUrl('/Account/UpdateCustomerClassification');
        var params = {};
        params.custId = custId;
        params.ctsCustId = ctsCustId;
        params.categoryId = categoryId;
        params.subscriberId = subscriberId;
        params.sportGroupIds = $('#ddlsports').val().join(',');;
        params.remark = $('#txtcategorycomment').val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Update the category successful!', 2000);
                    divMarkCategory.dialog('close');

                    tblcustomercategory.DataTable().ajax.reload();
                }
                else {
                    common.showError(response.errorMessage);
                }
            },
            complete: function () {

            }
        });
    }

    function InitialMarkCategory(custId, ctsCustId, subscriberId) {
        $.ajax(common.modifyRelativeUrl('Account/MarkCatergory'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                divMarkCategory.html(response);
                BindingMarkCategoryForAddNew(custId, ctsCustId, subscriberId);
            },
            complete: function () {
                divMarkCategory.dialog({
                    modal: true,
                    width: '600px',
                    title: 'MARK CUSTOMER CATEGORY',
                    resizable: true,
                    draggable: true,
                    close: function () { $(this).dialog('close'); }
                });

                common.closeLoading();
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function EditCategory(custId, ctsCustId, subscriberId, categoryName, categoryId, sportGroupIdList, comment) {
        $.ajax(common.modifyRelativeUrl('Account/EditMarkCatergory'), {
            type: 'POST',
            data: {
                custId: custId,
                ctsCustId: ctsCustId,
                subscriberId: subscriberId,
                categoryName: categoryName,
                categoryId: categoryId,
                sportGroupIdList: sportGroupIdList,
                comment: comment
            },
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                divMarkCategory.html(response);
                BindingMarkCategoryForUpdate(custId, ctsCustId, subscriberId, categoryId);
            },
            complete: function () {
                divMarkCategory.dialog({
                    modal: true,
                    width: '600px',
                    title: 'UPDATE CUSTOMER CATEGORY',
                    resizable: true,
                    draggable: true,
                    close: function () { $(this).dialog('close'); }
                });

                common.closeLoading();
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function RenderGrid(custId, ctsCustId, username, subscriberId) {
        tblcustomercategory.DataTable({
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
            paging: true,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            ajax: {
                url: common.modifyRelativeUrl('/Account/GetCustomerClassification'),
                data: {
                    custId: custId
                },
                type: 'POST',
                dataFilter: function (response) {
                    if (response !== undefined) {
                        let json = jQuery.parseJSON(response);
                        let isExternalChecked = true;
                        let isCache = GetStorageToggleCategory(chooseExternalCategory);

                        if (isCache !== null) {
                            isExternalChecked = isCache === "true";
                        }

                        SwitchCategorySummary(json.internalCategoryList, json.externalCategoryList, isExternalChecked);

                        inputSwitchCategorySummary.click(function () {                            
                            SwitchCategorySummary(json.internalCategoryList, json.externalCategoryList, inputSwitchCategorySummary.is(':checked'));
                        });
                    }
                    return response;
                },
                beforeSend: function () {
                    tblcustomercategory.find('tbody').addClass('loading');
                },
                complete: function () {
                    tblcustomercategory.find('tbody').removeClass('loading');
                }
            },
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], className: "text-left", defaultContent: "-" },
                { targets: [1], className: "text-left", defaultContent: "-" },
                { targets: [2], className: "text-left", defaultContent: "-" },
                { targets: [3], className: "text-center", defaultContent: "-" },
                { targets: [4], className: "text-center", defaultContent: "-" },
                { targets: [5], className: "text-center", defaultContent: "-"},
                { targets: [6], className: "text-center", defaultContent: "-" }
            ],
            columns: [
                { data: obj => obj.CategoryName },
                { data: obj => obj.SportGroupNameList },
                { data: obj => obj.Remark },
                { data: obj => obj.LastModifiedBy },
                { data: obj => obj.LastModifiedDate },
                { data: obj => obj.NextScanDatePeriod },
                {
                    data: obj => () => {
                        var html = '<span class="icon-edit' + (!isHostPermission ? ' disabled"' : '') + '" title="Edit Category"></span>';
                        html += '<span class="icon-delete' + (!isHostPermission ? ' disabled"' : '') + '" title="Remove Category"></span>';

                        return obj.IsEnabledDelete ? html : '';
                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                common.showConfirmation("Do you want to remove category <b>" + row.CategoryName + "</b> for <b>" + username + "</b> ?", function () {
                                    RemoveCustomerClassification(custId, ctsCustId, row.CategoryId);
                                });
                            });

                            $('.icon-edit', cell).click(() => {
                                EditCategory(custId, ctsCustId, subscriberId, row.CategoryName, row.CategoryId, row.SportGroupIdList, row.Remark);
                            });
                        }
                    }
                }
            ],
            rowGroup: {
                dataSrc: 'ParentCategoryName'
            }
        });
    }

    function RenderInternalProblemAccount(data) {
        var isExistProblemAccount = false;
        let html = "";

        data.map(function (x) {
            if (x.IsProblemAccount) {
                let classIconColor = "circle bg-red";
                let sportGroupNameList = x.SportGroupNameList ? x.SportGroupNameList : "None";

                html = BuildInternalCategoryToHtmlDiv(html, classIconColor, x.CategoryName, sportGroupNameList);
               
                isExistProblemAccount = true;
            }
        });

        if (isExistProblemAccount) {
            $('#divHeaderUsername')
                .removeClass('bg-yellow')
                .removeClass('bg-green')
                .addClass('bg-red');
            $('#divHeaderInfo')
                .removeClass('border-yellow')
                .removeClass('border-green')
                .addClass('border-red');

            $('#divCustomerCategorySummary').html(html);
            $('#spanContentInfoLoading').css('visibility', 'hidden');
            $('#divHeaderInfo').css('visibility', 'visible');
        }

        return isExistProblemAccount;
    }

    function RenderExternalProblemAccount(data) {
        var isExistProblemAccount = false;
        let html = "";

        data.map(function (x) {
            if (x.IsProblemAccount) {
                let classIconColor = "circle bg-red";
                
                html = BuildExternalCategoryToHtmlDiv(html, classIconColor, x.CategoryName);

                isExistProblemAccount = true;
            }
        });

        if (isExistProblemAccount) {
            $('#divHeaderUsername')
                .removeClass('bg-yellow')
                .removeClass('bg-green')
                .addClass('bg-red');
            $('#divHeaderInfo')
                .removeClass('border-yellow')
                .removeClass('border-green')
                .addClass('border-red');

            $('#divCustomerCategorySummary').html(html);
            $('#spanContentInfoLoading').css('visibility', 'hidden');
            $('#divHeaderInfo').css('visibility', 'visible');
        }

        return isExistProblemAccount;
    }

    function RenderInternalCategorySummary(data) {
        let html = "";
        let defaultNormalCategoryList = GetInternalNormalCategoryList();
        let indexColorProfile = 1;
       
        for (let i = 0; i < defaultNormalCategoryList.length; i++) {
            data.map(function (x) {
                let isEquals = x.CategoryName === defaultNormalCategoryList[i].name;
                if (isEquals) {
                    let classIconColor = defaultNormalCategoryList[i].cssClass;
                    let sportGroupNameList = x.SportGroupNameList ? x.SportGroupNameList : "None";

                    html = BuildInternalCategoryToHtmlDiv(html, classIconColor, x.CategoryName, sportGroupNameList);

                    indexColorProfile = indexColorProfile < defaultNormalCategoryList[i].indexColor
                        ? defaultNormalCategoryList[i].indexColor
                        : indexColorProfile;
                }
            });
        }        

        switch (indexColorProfile) {
            case 1:
                $('#divHeaderUsername')
                    .removeClass('bg-red')
                    .removeClass('bg-yellow')
                    .addClass('bg-green');
                $('#divHeaderInfo')
                    .removeClass('border-red')
                    .removeClass('border-yellow')
                    .addClass('border-green');
                break;
            case 2:
                $('#divHeaderUsername')
                    .removeClass('bg-red')
                    .removeClass('bg-green')
                    .addClass('bg-yellow');
                $('#divHeaderInfo')
                    .removeClass('border-red')
                    .removeClass('border-green')
                    .addClass('border-yellow');
                break;
            default:
                break;
        }       

        $('#divCustomerCategorySummary').html(html);
        $('#spanContentInfoLoading').css('visibility', 'hidden');
        $('#divHeaderInfo').css('visibility', 'visible');
    }

    function RenderExternalCategorySummary(data) {
        let html = "";
        let defaultNormalCategoryList = GetExternalNormalCategoryList();
        let indexColorProfile = 1;

        for (let i = 0; i < defaultNormalCategoryList.length; i++) {
            data.map(function (x) {
                let isEquals = x.CategoryName.includes(defaultNormalCategoryList[i].name);
                if (isEquals) {
                    let classIconColor = defaultNormalCategoryList[i].cssClass;
                    
                    html = BuildExternalCategoryToHtmlDiv(html, classIconColor, x.CategoryName);

                    indexColorProfile = indexColorProfile < defaultNormalCategoryList[i].indexColor
                        ? defaultNormalCategoryList[i].indexColor
                        : indexColorProfile;
                }
            });
        }

        switch (indexColorProfile) {
            case 1:
                $('#divHeaderUsername')
                    .removeClass('bg-red')
                    .removeClass('bg-yellow')
                    .addClass('bg-green');
                $('#divHeaderInfo')
                    .removeClass('border-red')
                    .removeClass('border-yellow')
                    .addClass('border-green');
                break;
            case 2:
                $('#divHeaderUsername')
                    .removeClass('bg-red')
                    .removeClass('bg-green')
                    .addClass('bg-yellow');
                $('#divHeaderInfo')
                    .removeClass('border-red')
                    .removeClass('border-green')
                    .addClass('border-yellow');
                break;
            default:
                break;
        }

        $('#divCustomerCategorySummary').html(html);
        $('#spanContentInfoLoading').css('visibility', 'hidden');
        $('#divHeaderInfo').css('visibility', 'visible');
    }

    function BuildInternalCategoryToHtmlDiv(html, classIconColor, categoryName, sportGroupNameList){
        html += "<div class='info-status'>";
        html += "<div class='status-left'>";
        html += "<span class='" + classIconColor + "'></span>";
        html += "<span class='text-status text-left'>" + categoryName + "</span>";
        html += "</div>";
        html += "<div class='status-right'>";
        html += "<span class='text-status text-right'>" + sportGroupNameList + "</span>";
        html += "</div>";
        html += "</div>";

        return html;
    }

    function BuildExternalCategoryToHtmlDiv(html, classIconColor, categoryName) {
        html += "<div class='info-status'>";
        html += "<div class='status-left'>";
        html += "<span class='" + classIconColor + "'></span>";
        html += "<span class='text-status text-left'>" + categoryName + "</span>";
        html += "</div>";
        html += "</div>";

        return html;
    }

    function GetInternalNormalCategoryList() {
        var list = [];

        list.push({ 'name': 'High Risk Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'name': 'Smart Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2});
        list.push({ 'name': 'Probation', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'name': 'Limited Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 }); 
        list.push({ 'name': 'Lucky Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'name': 'VVIP', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'name': 'VIP', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'name': 'Normal Member', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'name': 'New Member', 'cssClass': 'circle bg-green', 'indexColor': 1 });
           
        return list;
    }

    function GetExternalNormalCategoryList() {
        var list = [];

        list.push({ 'name': 'High Risk Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'name': 'Smart Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'name': 'Probation', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'name': 'Normal Account', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'name': 'VIP', 'cssClass': 'circle bg-green', 'indexColor': 1 });

        return list;
    }

    function SwitchCategorySummary(dataInternal, dataExternal, isExternalChecked) {
        SetStorageToggleCategory(chooseExternalCategory, isExternalChecked);

        let isProblemAccount = isExternalChecked
            ? RenderExternalProblemAccount(dataExternal)
            : RenderInternalProblemAccount(dataInternal);

        if (!isProblemAccount)
        {
            isExternalChecked
                ? RenderExternalCategorySummary(dataExternal)
                : RenderInternalCategorySummary(dataInternal);
        }

        if (isExternalChecked) {
            divToogleSwitch.attr('data-checked', '-');
            divToogleSwitch.attr('data-unchecked', 'GENERAL');
        } else {
            divToogleSwitch.attr('data-checked', 'DETAILED');
            divToogleSwitch.attr('data-unchecked', '-');
        }
    }

    function GetStorageToggleCategory(key) {
        return typeof (Storage) !== "undefined"
            ? localStorage.getItem(key)
            : null;
    }

    function SetStorageToggleCategory(key, value) {
        if (typeof (Storage) !== "undefined")
            localStorage.setItem(key, value);
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission)
            btnMarkCategory.prop('disabled', true);
    }

    this.Initial = function (custId, ctsCustId, subscriberId, username, hostPermission) {
        DisableButton(hostPermission);

        let isCache = GetStorageToggleCategory(chooseExternalCategory);
        if (isCache === null || isCache === "true") 
            inputSwitchCategorySummary.attr('checked', true);

        btnMarkCategory.click(function () { InitialMarkCategory(custId, ctsCustId, subscriberId); });        

        RenderGrid(custId, ctsCustId, username, subscriberId);
    };
}