function customer_category()
{
    var btnMarkCategory = $('#btnmarkcategory');
    var divMarkCategory = $('#divMarkCategory');
    var tblcustomercategory = $('#customer-category-table');

    function BindingMarkCategory(ctsCustId, subscriberId) {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            AddCustomerClassification(ctsCustId, subscriberId);
        });

        $("#ddlcategory").multiselect({
            header: "Select an option",
            noneSelectedText: "Select an Option",
            selectedList: 1
        });

        $("#ddlsports").multiselect({
            selectedList: 5,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        });
    }

    function AddCustomerClassification(ctsCustId, subscriberId)
    {        
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
                BindingMarkCategory(ctsCustId, subscriberId);
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

    function RenderGrid() {
        tblcustomercategory.DataTable({
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
                { targets: [0], width: "15%", className: "text-left" },
                { targets: [1], width: "15%", className: "text-left" },
                { targets: [2], width: "15%", className: "text-left" },
                { targets: [3], width: "30%", className: "text-center" },
                { targets: [4], width: "15%", className: "text-center" },
                { targets: [5], width: "10%", className: "text-center" }
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
                            
                        });
                    }
                }
            ]
        })
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
                tblcustomercategory.DataTable().columns.adjust();
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

    this.Initial = function (ctsCustId, subscriberId) {
        btnMarkCategory.click(function () { InitialMarkCategory(ctsCustId, subscriberId); });

        RenderGrid();
        GetCustomerClassification(ctsCustId);
    }
}