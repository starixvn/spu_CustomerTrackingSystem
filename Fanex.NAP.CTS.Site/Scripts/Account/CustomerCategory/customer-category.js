function customer_category() {
    let btnMarkCategory = $('#btnmarkcategory');
    let divMarkCategory = $('#divMarkCategory');
    let tblcustomercategory = $('#customer-category-table');
    let isHostPermission;

    function BindingMarkCategoryForAddNew(custId, ctsCustId, subscriberId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            AddCustomerClassification(custId, ctsCustId, subscriberId);
        });

        $("#ddlcategory").select2();

        $("#ddlsports").multiselect({
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
                },
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
                type: 'POST'
            },
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], className: "text-left", defaultContent: "-" },
                { targets: [1], className: "text-left", defaultContent: "-" },
                { targets: [2], className: "text-left", defaultContent: "-" },
                { targets: [3], className: "text-center", defaultContent: "-" },
                { targets: [4], className: "text-center", defaultContent: "-" },
                { targets: [5], className: "text-center", defaultContent: "-" }
            ],
            columns: [
                { data: obj => obj.CategoryName },
                { data: obj => obj.SportGroupNameList },
                { data: obj => obj.Remark },
                { data: obj => obj.LastModifiedBy },
                { data: obj => obj.LastModifiedDate },
                {
                    data: obj => () =>
                    {
                        var html = '<span class="icon-edit' + (!isHostPermission ? ' disabled"' : '') +'" title="Edit Category"></span>';
                        html += '<span class="icon-delete' + (!isHostPermission ? ' disabled"' : '') +'" title="Remove Category"></span>';

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

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission)
            btnMarkCategory.prop('disabled', true);
    }

    this.Initial = function (custId, ctsCustId, subscriberId, username, hostPermission) {
        DisableButton(hostPermission);

        btnMarkCategory.click(function () { InitialMarkCategory(custId, ctsCustId, subscriberId); });

        RenderGrid(custId, ctsCustId, username, subscriberId);
        // GetCustomerClassification(ctsCustId);
    };
}