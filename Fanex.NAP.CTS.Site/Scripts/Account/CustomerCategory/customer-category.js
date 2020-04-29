function customer_category() {
    var btnMarkCategory = $('#btnmarkcategory');
    var divMarkCategory = $('#divMarkCategory');
    var tblcustomercategory = $('#customer-category-table');

    function BindingMarkCategoryForAddNew(ctsCustId, subscriberId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            AddCustomerClassification(ctsCustId, subscriberId);
        });

        $("#ddlcategory").select2();

        $("#ddlsports").multiselect({
            selectedList: 3,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        });
    }

    function BindingMarkCategoryForUpdate(ctsCustId, subscriberId, categoryId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            UpdateCustomerClassification(ctsCustId, subscriberId, categoryId);
        });

        $("#ddlsports").multiselect({
            selectedList: 3,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        });
    }

    function AddCustomerClassification(ctsCustId, subscriberId) {
        if ($('#ddlsports').val() == '') {

            common.showError('Sport is required.');
            return;
        }

        var url = common.modifyRelativeUrl('/Account/AddCustomerClassification');
        var params = {};
        params.ctsCustId = ctsCustId;
        params.subscriberId = subscriberId;
        params.categoryId = $('#ddlcategory').val();
        params.sportGroupIds = $('#ddlsports').val().join(',');
        params.remark = $('#txtcategorycomment').val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode == 0) {
                    common.showSuccess('Add the category successful!', 2000);
                    divMarkCategory.dialog('close');
                }
                else {
                    common.showError('Add the category fail!');
                }
            },
            complete: function () {
                GetCustomerClassification(ctsCustId);
            }
        });
    }

    function RemoveCustomerClassification(ctsCustId, categoryId) {
        var url = common.modifyRelativeUrl('/Account/RemoveCustomerClassification');
        var params = {};
        params.ctsCustId = ctsCustId;
        params.categoryId = categoryId;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode == 0) {
                    common.showSuccess('Remove the category successful!', 2000);
                    GetCustomerClassification(ctsCustId);
                }
                else {
                    common.showError('Remove the category fail!');
                }
            },
            complete: function () {
            }
        });
    }

    function UpdateCustomerClassification(ctsCustId, subscriberId, categoryId) {
        var url = common.modifyRelativeUrl('/Account/UpdateCustomerClassification');
        var params = {};
        params.ctsCustId = ctsCustId;
        params.categoryId = categoryId;
        params.subscriberId = subscriberId;
        params.sportGroupIds = $('#ddlsports').val().join(',');;
        params.remark = $('#txtcategorycomment').val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode == 0) {
                    common.showSuccess('Update the category successful!', 2000);
                    divMarkCategory.dialog('close');

                }
                else {
                    common.showError('Update the category fail!');
                }
            },
            complete: function () {
                GetCustomerClassification(ctsCustId);
            }
        });
    }

    function InitialMarkCategory(ctsCustId, subscriberId) {
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
                BindingMarkCategoryForAddNew(ctsCustId, subscriberId);
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

    function EditCategory(ctsCustId, subscriberId, categoryName, categoryId, sportGroupIdList, comment) {
        $.ajax(common.modifyRelativeUrl('Account/EditMarkCatergory'), {
            type: 'POST',
            data: {
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
                BindingMarkCategoryForUpdate(ctsCustId, subscriberId, categoryId);
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

    function RenderGrid(ctsCustId, username, subscriberId) {
        tblcustomercategory.DataTable({
            language: {
                zeroRecords: "There's no data"
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
                    ctsCustId: ctsCustId
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
                    data: obj => '<span class="icon-edit" title="Edit Category"></span><span class="icon-delete" title="Remove Category"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() => {
                            common.showConfirmation("Do you want to remove category <b>" + row.CategoryName + "</b> for <b>" + username + "</b> ?", function () {
                                RemoveCustomerClassification(ctsCustId, row.CategoryId);
                            });

                        });

                        $('.icon-edit', cell).click(() => {
                            EditCategory(ctsCustId, subscriberId, row.CategoryName, row.CategoryId, row.SportGroupIdList, row.Remark);
                        });
                    }
                }                
            ],
            rowGroup: {
                dataSrc: 'ParentCategoryName'
            }
        });
    }

    function GetCustomerClassification(ctsCustId) {
        var url = common.modifyRelativeUrl('/Account/GetCustomerClassification');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblcustomercategory.find('tbody').addClass('loading');
            },
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                tblcustomercategory.DataTable().clear();
                tblcustomercategory.DataTable().rows.add(response).draw();
                //tblcustomercategory.DataTable().columns.adjust();
            },
            complete: function () {
                tblcustomercategory.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    this.Initial = function (ctsCustId, subscriberId, username) {
        btnMarkCategory.click(function () { InitialMarkCategory(ctsCustId, subscriberId); });

        RenderGrid(ctsCustId, username, subscriberId);
       // GetCustomerClassification(ctsCustId);
    }
}