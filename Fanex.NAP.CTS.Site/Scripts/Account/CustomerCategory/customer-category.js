function customer_category()
{
    var btnMarkCategory = $('#btnmarkcategory');
    var divMarkCategory = $('#divMarkCategory');
    var tblcustomercategory = $('#customer-category-table');

    function BindingMarkCategory() {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            //AddPlaceEvidence(ctsCustId, username, subscriberId);
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

    function InitialMarkCategory(ctsCustId) {
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
                BindingMarkCategory();
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
                    data: obj => '<span class="icon-edit" title="Remove Category"></span><span class="icon-delete" title="Remove Category"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() => {
                            
                        });
                    }
                }
            ]
        })
    }

    function GetException(ctsCustId) {
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

    this.Initial = function (ctsCustid, username) {
        btnMarkCategory.click(function () { InitialMarkCategory(ctsCustid); });

        RenderGrid();
        GetException(ctsCustid);
    }
}