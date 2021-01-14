function customer_category() {
    let btnMarkCategory = $('#btnmarkcategory');
    let btnRemoveCategory = $('#btnremovecategory');
    let divMarkCategory = $('#divMarkCategory');
    let divRemoveCategory = $('#divRemoveCategory');
    let tblcustomercategory = $('#customer-category-table');
    let isHostPermission;
    let inputPinCategory = $('#inputPinCategory');
    let spanClass =  $('#spanClass');

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
    
    function RenderGrid(custId) {
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
                    custId: custId
                },
                type: 'POST',
                dataFilter: function (response) {
                    if (response !== undefined) {
                        let json = jQuery.parseJSON(response);

                        inputPinCategory.prop('checked', json.isPinCategory);
                        inputPinCategory.prop('disabled', json.isVVip);
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

    function ReloadGrid(custId)
    {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerClassification'), {
            type: 'POST',
            data: { custId: custId },
            dataFilter: function (response) {
                if (response !== undefined) {
                    let json = jQuery.parseJSON(response);                  

                    inputPinCategory.prop('checked', json.isPinCategory);
                    inputPinCategory.prop('disabled', json.isVVip);
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

    function RenderCategorySummary(data) {
        let html = "";
        let defaultNormalCategoryList = GetNormalCategoryList();
        let indexColorProfile = 1;

        for (let i = 0; i < defaultNormalCategoryList.length; i++) {
            data.map(function (x) {
                let isEquals = x.CategoryId === defaultNormalCategoryList[i].id;
                if (isEquals) {
                    let classIconColor = defaultNormalCategoryList[i].cssClass;
                    
                    html = BuildCategoryToHtmlDiv(html, classIconColor, x.CategoryName);

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

    function BuildCategoryToHtmlDiv(html, classIconColor, categoryName) {
        html += "<div class='info-status'>";
        html += "<div class='status-left'>";
        html += "<span class='" + classIconColor + "'></span>";
        html += "<span class='text-status text-left'>" + categoryName + "</span>";
        html += "</div>";
        html += "</div>";

        return html;
    }    

    function GetNormalCategoryList() {
        var list = [];

        list.push({ 'id': 204, 'name': 'High Risk Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'id': 203, 'name': 'Smart Punter', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'id': 206, 'name': 'Probation', 'cssClass': 'circle bg-yellow', 'indexColor': 2 });
        list.push({ 'id': 202, 'name': 'Normal Account', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'id': 213, 'name': 'VVIP', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'id': 205, 'name': 'VIP', 'cssClass': 'circle bg-green', 'indexColor': 1 });
        list.push({ 'id': 201, 'name': 'New Member', 'cssClass': 'circle bg-green', 'indexColor': 1 });

        return list;
    }   

    function PinCategory(custId, ctsCustId, subscriberId) {
        let html = "<div style='text-align:center'>Are you sure about this action? </div>";

        let isPinCategory = inputPinCategory.is(":checked");

        if (isPinCategory) {
            html += "By enabling pin option, this account will not be classified automatically by our service in future";
        } else {
            html += "By unpin option, this account will be classified automatically by our service in future";
        }

        common.showFormConfirmation(html, 'Confirmation', function () {
            var url = common.modifyRelativeUrl('/Account/PinCategoryCustomerClassification');
            var params = {};     
            params.custId = custId;
            params.ctsCustId = ctsCustId;
            params.subscriberId = subscriberId;
            params.isPinCategory = isPinCategory;

            $.ajax(url, {
                type: 'POST',
                data: params,
                success: function (response) {
                    if (response.errorCode === 0) {
                        let message = isPinCategory
                            ? "Pinned category successfully!"
                            : "Unpinned category successfully!";

                        common.showSuccess(message);
                    }
                    else {
                        common.showError('Failed! ' + response.errorMessage);
                        inputPinCategory.prop('checked', !isPinCategory);
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
                    inputPinCategory.prop('checked', !isPinCategory);
                }
            });
        });
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission) {
            inputPinCategory.prop('disabled', true);
            btnMarkCategory.prop('disabled', true);
            btnRemoveCategory.prop('disabled', true);
        }
    }

    this.Initial = function (custId, ctsCustId, subscriberId, username, hostPermission) {
        DisableButton(hostPermission);

        if (isHostPermission) {
            inputPinCategory.click(function () { PinCategory(custId, ctsCustId, subscriberId); });

            btnMarkCategory.click(function () { DisplayMarkCategory(custId, ctsCustId, subscriberId); });

            btnRemoveCategory.click(function () { DisplayRemoveCategory(custId, ctsCustId, subscriberId); });
        }

        RenderGrid(custId);
    };

    this.ReloadTab = function (custId) {
        ReloadGrid(custId);
    };
}