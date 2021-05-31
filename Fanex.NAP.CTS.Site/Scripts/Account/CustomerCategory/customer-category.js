function customer_category() {
    let cbbCategorySetting = $('#cbbCategorySetting');
    let btnMarkCategory = $('#btnmarkcategory');
    let btnRemoveCategory = $('#btnremovecategory');
    let divMarkCategory = $('#divMarkCategory');
    let divRemoveCategory = $('#divRemoveCategory');
    let tblcustomercategory = $('#customer-category-table');
    let isHostPermission; 
    let spanClass = $('#spanClass');
    let cacheCategoryColors = [];
    let cacheData = {
        isVVip: false,
        isPinCategory: false,
        isExcludedTagAssPA: false,
        isProblemAccount: false
    };
        
    function GetNormalCategoryList() {
        var list = [];

        list.push({ 'id': 205, 'indexColor': 1, 'cssClass': 'circle bg-green', 'name': 'Good Member' });
        list.push({ 'id': 210, 'indexColor': 1, 'cssClass': 'circle bg-green', 'name': 'VVIP' });
        list.push({ 'id': 201, 'indexColor': 1, 'cssClass': 'circle bg-green', 'name': 'New Member' });
        list.push({ 'id': 202, 'indexColor': 1, 'cssClass': 'circle bg-green', 'name': 'Normal Account' });

        list.push({ 'id': 206, 'indexColor': 2, 'cssClass': 'circle bg-yellow', 'name': 'Probation' });
        list.push({ 'id': 203, 'indexColor': 2, 'cssClass': 'circle bg-yellow', 'name': 'Smart Punter' });
        list.push({ 'id': 204, 'indexColor': 2, 'cssClass': 'circle bg-yellow', 'name': 'High Risk Punter' });
        
        list.push({ 'id': 211, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'Good Member (111)' });
        list.push({ 'id': 212, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'Good Member (112)' });
        list.push({ 'id': 213, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'Good Member (113)' });
        list.push({ 'id': 221, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'Normal Account (121)' });
        list.push({ 'id': 222, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'Normal Account (122)' });
        list.push({ 'id': 223, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'Normal Account (123)' });
        list.push({ 'id': 231, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'New Member (31)' });
        list.push({ 'id': 232, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'New Member (32)' });
        list.push({ 'id': 233, 'indexColor': 3, 'cssClass': 'circle green-line', 'name': 'New Member (33)' });

        list.push({ 'id': 251, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'Probation (51)' });
        list.push({ 'id': 252, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'Probation (52)' });
        list.push({ 'id': 253, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'Probation (53)' });
        list.push({ 'id': 261, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'Smart Punter (61)' });
        list.push({ 'id': 262, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'Smart Punter (62)' });
        list.push({ 'id': 263, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'Smart Punter (63)' });
        list.push({ 'id': 271, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'High Risk Punter (71)' });
        list.push({ 'id': 272, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'High Risk Punter (72)' });
        list.push({ 'id': 273, 'indexColor': 4, 'cssClass': 'circle yellow-line', 'name': 'High Risk Punter (73)' });

        return list;
    }

    function BindingMarkCategoryForAddNew(custId, ctsCustId, subscriberId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            AddCustomerCategory(custId, ctsCustId, subscriberId);
        });

        $("#ddlcategory").select2();      
    }

    function GetCustomerCategory(custId) {
        setTimeout(function () {
            $.ajax(common.modifyRelativeUrl('Account/GetLastestCustomerClassification'), {
                type: 'POST',
                data: {
                    custId: custId
                },
                success: function (response) {
                    spanClass.html(response.customerClassification + "  ");
                },
                complete: function () {
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }, 10000);
    }

    function DisplayMarkCategory(custId, ctsCustId, subscriberId) {
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

    function AddCustomerCategory(custId, ctsCustId, subscriberId) {
        var url = common.modifyRelativeUrl('Account/AddCustomerCategory');
        var params = {};
        params.CustId = custId;
        params.CTSCustId = ctsCustId;
        params.SubscriberId = subscriberId;
        params.CategoryId = $('#ddlcategory').val();
        params.Remark = $('#txtcategorycomment').val();
        params.isVVip = true;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Add the category successful!', 2000);
                    divMarkCategory.dialog('close');

                    tblcustomercategory.DataTable().ajax.reload();
                    GetCustomerCategory(custId);
                }
                else {
                    common.showError(response.errorMessage);
                }
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (error) {
                console.log(error);
                common.closeLoading();
            }
        });
    }

    function DisplayRemoveCategory(custId, ctsCustId, subscriberId) {
        $.ajax(common.modifyRelativeUrl('Account/RemoveCatergory'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                divRemoveCategory.html(response);

                $('#btnCancelRemoveCategory').on('click', function () {
                    divRemoveCategory.dialog('close');
                });

                $('#btnSaveRemoveCategory').on('click', function (e) {
                    RemoveCategory(custId, ctsCustId);
                });

                $("#ddlRemoveCategory").select2(); 
            },
            complete: function () {
                divRemoveCategory.dialog({
                    modal: true,
                    width: '600px',
                    title: 'REMOVE CUSTOMER CATEGORY',
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

    function RemoveCategory(custId, ctsCustId) {
        var url = common.modifyRelativeUrl('Account/RemoveCustomerCategory');
        var params = {};
        params.custId = custId;
        params.ctsCustId = ctsCustId;
        params.categoryId = $('#ddlRemoveCategory').val();
        params.isVVip = true;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Remove the category successful!', 2000);
                    divRemoveCategory.dialog('close');

                    tblcustomercategory.DataTable().ajax.reload();
                    GetCustomerCategory(custId);
                }
                else {
                    common.showError(response.errorMessage);
                }
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (error) {
                console.log(error);
                common.closeLoading();
            }
        });
    }
    
    function RenderGrid(ctsCustId) {
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
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            ajax: {
                url: common.modifyRelativeUrl('Account/GetCustomerClassification'),
                data: {
                    ctsCustId: ctsCustId
                },
                type: 'POST',
                dataFilter: function (response) {
                    if (response !== undefined) {
                        let json = jQuery.parseJSON(response);
                     
                        cacheData.isProblemAccount = json.isProblemAccount;
                        cacheData.isPinCategory = json.isPinCategory;
                        cacheData.isExcludedTagAssPA = json.isExcludedTagAssPA;
                        cacheData.isVVip = json.isVVip;

                        ResetCategorySetting();                       
                        
                        btnMarkCategory.prop('disabled', json.isVVip);
                        btnRemoveCategory.prop('disabled', !json.isVVip);

                        let isProblemAccount = RenderProblemAccount(json.categoryList);

                        if (!isProblemAccount) {
                            RenderCategorySummary(json.categoryList);
                        }
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
                { targets: [0], className: "text-center", defaultContent: "-" },
                { targets: [1], className: "text-center", defaultContent: "-" },
                { targets: [2], className: "text-center", defaultContent: "-" },
                { targets: [3], className: "text-center", defaultContent: "-" },
                { targets: [4], className: "text-center", defaultContent: "-" }
            ],
            columns: [
                { data: obj => obj.CreatedTimeString },
                { data: obj => obj.CategoryName },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.Reason },
                { data: obj => obj.CreatedBy }                                
            ]
        });
    }

    function ReloadGrid(ctsCustId)
    {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerClassification'), {
            type: 'POST',
            data: { ctsCustId: ctsCustId },
            dataFilter: function (response) {
                if (response !== undefined) {
                    let json = jQuery.parseJSON(response);                  
                 
                    btnMarkCategory.prop('disabled', json.isVVip);
                    btnRemoveCategory.prop('disabled', !json.isVVip);

                    cacheData.isProblemAccount = json.isProblemAccount;
                    cacheData.isPinCategory = json.isPinCategory;
                    cacheData.isExcludedTagAssPA = json.isExcludedTagAssPA;
                    cacheData.isVVip = json.isVVip;

                    ResetCategorySetting();                                      

                    let isProblemAccount = RenderProblemAccount(json.categoryList);

                    if (!isProblemAccount) {
                        RenderCategorySummary(json.categoryList);
                    }
                }
                return response;
            },
            beforeSend: function () {
                tblcustomercategory.find('tbody').addClass('loading');
            },
            success: function (response) {
                tblcustomercategory.DataTable().clear().rows.add(response.data).draw(true);
            },
            complete: function () {
                tblcustomercategory.find('tbody').removeClass('loading');
            }
        });
    }    

    function RenderProblemAccount(data) {
        var isExistProblemAccount = false;
        let html = "";

        data.map(function (x) {
            if (x.IsProblemAccount) {
                let classIconColor = "circle bg-red";
                
                html = BuildCategoryToHtmlDiv(html, classIconColor, x.CategoryName);

                isExistProblemAccount = true;
            }
        });

        if (isExistProblemAccount) {
            $('#divHeaderUsername')
                .removeClass('bg-yellow').removeClass('bg-green')
                .removeClass('green-line').removeClass('yellow-line')
                .addClass('bg-red');
            $('#divHeaderInfo')
                .removeClass('border-yellow').removeClass('border-green')
                .addClass('border-red');

            $('#divCustomerCategorySummary').html(html);
            $('#spanContentInfoLoading').css('visibility', 'hidden');
            $('#divHeaderInfo').css('visibility', 'visible');
        }

        return isExistProblemAccount;
    }    

    function RenderCategorySummary(data) {
        let html = "";
        let indexColorProfile = 1;

        for (let i = 0; i < cacheCategoryColors.length; i++) {
            data.map(function (x) {
                let isEquals = x.CategoryId === cacheCategoryColors[i].id;
                if (isEquals) {
                    let classIconColor = cacheCategoryColors[i].cssClass;
                    
                    html = BuildCategoryToHtmlDiv(html, classIconColor, x.CategoryName);

                    indexColorProfile = indexColorProfile < cacheCategoryColors[i].indexColor
                        ? cacheCategoryColors[i].indexColor
                        : indexColorProfile;
                }
            });
        }

        switch (indexColorProfile) {
            case 1:
                $('#divHeaderUsername')
                    .removeClass('bg-red').removeClass('bg-yellow')
                    .removeClass('green-line').removeClass('yellow-line')
                    .addClass('bg-green');
                $('#divHeaderInfo')
                    .removeClass('border-red').removeClass('border-yellow')                      
                    .addClass('border-green');
                break;
            case 2:
                $('#divHeaderUsername')
                    .removeClass('bg-red').removeClass('bg-green')
                    .removeClass('green-line').removeClass('yellow-line')
                    .addClass('bg-yellow');
                $('#divHeaderInfo')
                    .removeClass('border-red').removeClass('border-green')
                    .addClass('border-yellow');
                break;
            case 3:
                $('#divHeaderUsername')
                    .removeClass('bg-red').removeClass('bg-yellow')
                    .removeClass('bg-green').removeClass('yellow-line')
                    .addClass('green-line')
                $('#divHeaderInfo')
                    .removeClass('border-red').removeClass('border-yellow')
                    .addClass('border-green');
                break;
            case 4:
                $('#divHeaderUsername')
                    .removeClass('bg-red').removeClass('bg-yellow')
                    .removeClass('bg-green').removeClass('green-line')
                    .addClass('yellow-line')
                $('#divHeaderInfo')
                    .removeClass('border-red').removeClass('border-yellow')
                    .addClass('border-yellow');
                break;
            default:
                break;
        }

        $('#divCustomerCategorySummary').html(html);
        $('#spanContentInfoLoading').css('visibility', 'hidden');
        $('#divHeaderInfo').css('visibility', 'visible');
    }

    function BuildCategoryToHtmlDiv(html, classIconColor, categoryName) {
        html += "<div class='info-status'>";
        html += "<div class='status-left'>";
        html += "<span class='" + classIconColor + "'></span>";
        html += "<span class='text-status text-left'>" + categoryName + "</span>";
        html += "</div>";
        html += "</div>";

        return html;
    }        

    function PinCategory(isChecked, ctsCustId, subscriberId) {
        let html = "<div style='text-align:center'>Are you sure about this action? </div>";  
        
        if (isChecked) {
            html += "By enabling pin option, this account will not be classified automatically by our service in future";
        } else {
            html += "By unpin option, this account will be classified automatically by our service in future";
        }

        common.showFormConfirmation(html, 'Confirmation',
            function () {
                var url = common.modifyRelativeUrl('/Account/PinCategoryCustomerClassification');
                var params = {};     
                params.ctsCustId = ctsCustId;
                params.subscriberId = subscriberId;
                params.isPinCategory = isChecked;

                $.ajax(url, {
                    type: 'POST',
                    data: params,
                    success: function (response) {
                        if (response.errorCode === 0) {
                            let message = isChecked
                                ? "Pinned category successfully!"
                                : "Unpinned category successfully!";

                            common.showSuccess(message);
                            tblcustomercategory.DataTable().ajax.reload();
                        }
                        else {
                            common.showError('Failed! ' + response.errorMessage);
                            ResetCategorySetting();
                        }
                    },
                    beforeSend: function () {
                        common.showLoading();
                    },
                    complete: function () {
                        common.closeLoading();
                    },
                    error: function (error) {
                        common.closeLoading();
                        console.log(error);
                        ResetCategorySetting();                    
                    }
                });
            },
            function () {
                ResetCategorySetting();
            }
        );
    }

    function ExcludedTagAssPA(isChecked, custId, ctsCustId, subscriberId, custSubId) {
        let html = "";
        let isPinCategory = false;
       
        $("option", cbbCategorySetting).each(function () {
            if ($(this).val() === "PinCategory") {
                isPinCategory = $(this).is(":checked");
            }
        });

        if (isChecked && isPinCategory) {
            html = "<div style='text-align:center'>You are not allow to turn on this setting because Pin Category setting is on.</div>";
            html += "<div>Please Turn Off Pin Category Setting before this action</div>";

            common.showError(html, 2000);
            ResetCategorySetting();
            
            return;

        } else if (isChecked) {
            html = "<div style='text-align:center'>Are you sure about this action? </div>";
            html += "<div>By selecting this option, this account will not be classified with PA</div>";
            html += "<div>Associated Category by our service in future</div>";
        } else {
            html = "<div style='text-align:center'>Are you sure about this action? </div>";
            html += "<div>By unselecting this option, this account can be classified with PA</div>";
            html += "<div>Associated Category by our service in future</div>";
        }

        common.showFormConfirmation(html, 'Confirmation',
            function () {
                var url = common.modifyRelativeUrl('/Account/ChangeExcludedTagAssPA');
                var params = {};
                params.custId = custId;
                params.ctsCustId = ctsCustId;
                params.subscriberId = subscriberId;
                params.isExcludedTagAssPA = isChecked;
                params.isProblemAccount = cacheData.isProblemAccount;
                params.custSubId = custSubId;

                $.ajax(url, {
                    type: 'POST',
                    data: params,
                    success: function (response) {
                        if (response.errorCode === 0) {
                            let message = isChecked
                                ? "Mark category successfully!"
                                : "Unmark category successfully!";

                            common.showSuccess(message);
                            tblcustomercategory.DataTable().ajax.reload();
                        }
                        else {
                            common.showError('Failed! ' + response.errorMessage);
                            ResetCategorySetting();
                        }
                    },
                    beforeSend: function () {
                        common.showLoading();
                    },
                    complete: function () {
                        common.closeLoading();
                    },
                    error: function (error) {
                        common.closeLoading();
                        console.log(error);
                        ResetCategorySetting();
                    }
                });
            },
            function () {
                ResetCategorySetting();
            }
        );
    }  

    function ResetCategorySetting() {
        let cbbCategorySettingSelector = cbbCategorySetting.empty();

        cbbCategorySettingSelector.append("<option " + (cacheData.isPinCategory ? "selected" : "") + (cacheData.isVVip ? " disabled" : "") + " value='PinCategory'>Pin Category</option>");

        cbbCategorySettingSelector.append("<option " + (cacheData.isExcludedTagAssPA ? "selected" : "") + " value='ExcludedTagAssPA'>Normal Category</option>");       

        cbbCategorySetting.multiselect("refresh");
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission) {
            cbbCategorySetting.prop('disabled', true);
            btnMarkCategory.prop('disabled', true);
            btnRemoveCategory.prop('disabled', true);
        }
    }

    this.Initial = function (custId, ctsCustId, subscriberId, username, hostPermission, custSubId) {
        DisableButton(hostPermission);

        cbbCategorySetting.multiselect({
            menuWidth: 160,
            menuHeight: 60,
            header: false,
            click: function (_, ui) {
                if (ui.value == "PinCategory")
                    PinCategory(ui.checked, ctsCustId, subscriberId);
                else
                    ExcludedTagAssPA(ui.checked, custId, ctsCustId, subscriberId, custSubId);
            },
            selectedText: "Category Setting",
            noneSelectedText: "Category Setting"
        });

        if (isHostPermission) {

            btnMarkCategory.click(function () { DisplayMarkCategory(custId, ctsCustId, subscriberId); });

            btnRemoveCategory.click(function () { DisplayRemoveCategory(custId, ctsCustId, subscriberId); });
        }

        cacheCategoryColors = GetNormalCategoryList();

        RenderGrid(ctsCustId);
    };

    this.ReloadTab = function (ctsCustId) {
        ReloadGrid(ctsCustId);
    };
}