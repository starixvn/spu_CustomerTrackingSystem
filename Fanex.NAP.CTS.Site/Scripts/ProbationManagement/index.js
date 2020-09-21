function ProbationManagement() {
    let dataTableGeneral;
    let dataTableDetailed;

    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        tabs: $("#tabs-probation-detail"),        
        inputUserNameGeneral: $('#inputUserNameGeneral'),
        inputDayLeftGeneral: $('#inputDayLeftGeneral'),
        inputMinMarginGeneral: $('#inputMinMarginGeneral'),
        inputMaxMarginGeneral: $('#inputMaxMarginGeneral'),
        btnGenerateGeneral: $('#btnGenerateGeneral'),
        gridGeneral: $('#gridGeneral'),
        tblGeneral: $('#tblGeneral'),
        inputUserNameDetailed: $('#inputUserNameDetailed'),
        inputDayLeftDetailed: $('#inputDayLeftDetailed'),
        inputMinMarginDetailed: $('#inputMinMarginDetailed'),
        inputMaxMarginDetailed: $('#inputMaxMarginDetailed'),
        btnGenerateDetailed: $('#btnGenerateDetailed'),
        gridDetailed: $('#gridDetailed'),
        tblDetailed: $('#tblDetailed'),
        btnEdit: $('.sub-icon.icon-edit'),
        formGeneralCategories: $('#formGeneralCategories'),
        formDetailedCategories: $('#formDetailedCategories')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

    function LoadGridGeneral() {
        domElements.gridGeneral.prop('hidden', false);

        dataTableGeneral
            ? dataTableGeneral.ajax.reload(null, true)
            : dataTableGeneral = GenerateGridGeneral();
    }

    function LoadGridDetailed() {
        domElements.gridDetailed.prop('hidden', false);

        dataTableDetailed
            ? dataTableDetailed.ajax.reload(null, true)
            : dataTableDetailed = GenerateGridDetailed();
    }

    function GenerateGridGeneral() {
        return domElements.tblGeneral.DataTable({
            ajax: {
                url: common.modifyRelativeUrl('ProbationManagement/GetProbationCategory'),
                data: function () {
                    return GetParamFilters(1);
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblGeneral.find('tbody').addClass('loading');
                    domElements.btnGenerateGeneral.prop('disabled', true);
                },
                complete: function () {
                    domElements.tblGeneral.find('tbody').removeClass('loading');
                    domElements.btnGenerateGeneral.prop('disabled', false);
                },
                error: function (error) {
                    domElements.tblGeneral.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: true,
            searching: false,
            lengthMenu: [ 20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-"},
                { targets: [1], className: "text-left" },
                { targets: [0, 5, 9, 2, 3, 4], className: "text-center" },
                { targets: [6, 7, 8], className: "text-right" },
                { targets: [0], width: "3%" },
                { targets: [1], width: "12%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "9%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "9%" },
                { targets: [6], width: "9%" },
                { targets: [7], width: "9%" },
                { targets: [8], width: "9%" },
                { targets: [9], width: "4%" }

            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => obj.UserName },
                { data: obj => obj.SiteName },
                { data: obj => obj.SportName },
                { data: obj => obj.TargetCategoryName },
                { data: obj => obj.TaggedDate },
                { data: obj => obj.FirstMargin },
                { data: obj => obj.CurrentMargin },
                { data: obj => obj.DaysLeft },
                {
                    data: obj => () => {
                        let html = '<span class="sub-icon delete" id="sub-icon " title="Change Category"><i class="icon-edit"></i></span>';
                        return html;
                    }, createdCell: (cell, _, obj) => {
                        if (isHostPermission) {
                            $('.icon-edit', cell).click((event) => {
                                ShowFormChangeCategory(cell, obj.UserName, obj.CustId, obj.SportGroupId, 1);
                            });
                        }
                    }
                }
            ]
        });
    }  

    function GenerateGridDetailed() {
        return domElements.tblDetailed.DataTable({
            ajax: {
                url: common.modifyRelativeUrl('ProbationManagement/GetProbationCategory'),
                data: function () {
                    return GetParamFilters(2);
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblDetailed.find('tbody').addClass('loading');
                    domElements.btnGenerateDetailed.prop('disabled', true);
                },
                complete: function () {
                    domElements.tblDetailed.find('tbody').removeClass('loading');
                    domElements.btnGenerateDetailed.prop('disabled', false);
                },
                error: function (error) {
                    domElements.tblDetailed.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: true,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [1, 4], className: "text-left" },
                { targets: [0, 5, 9, 2, 3, 4 ], className: "text-center" },
                { targets: [6, 7, 8], className: "text-right" },
                { targets: [0], width: "4%" },
                { targets: [1], width: "9%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "9%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "9%" },
                { targets: [6], width: "9%" },
                { targets: [7], width: "9%" },
                { targets: [8], width: "9%" },
                { targets: [9], width: "4%" }

            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => obj.UserName },
                { data: obj => obj.SiteName },
                { data: obj => obj.SportName },
                { data: obj => obj.TargetCategoryName },
                { data: obj => obj.TaggedDate },
                { data: obj => obj.FirstMargin },
                { data: obj => obj.CurrentMargin },
                { data: obj => obj.DaysLeft },
                {
                    data: obj => () => {
                        let html = '<span class="sub-icon delete" title="Change Category"><i class="icon-edit"></i></span>';
                        return html;
                    }, createdCell: (cell, _, obj) => {
                        if (isHostPermission) {
                            $('.icon-edit', cell).click((event) => {
                                ShowFormChangeCategory(cell, obj.UserName, obj.CustId, obj.SportGroupId, 2);
                            });
                        }
                    }
                }
            ],
            createdRow: function (row, obj, _) {
                let cssClassTR = '';
                if (obj.DaysLeft > 60) {
                    cssClassTR = 'red';
                } else if (obj.CurrentMargin < 0) {
                    cssClassTR = 'green';
                }
                $(row).addClass(cssClassTR);
            }
        });
    }

    function GetParamFilters(categoryMode) {
        let paramsFilter = {};
        let daysLeft = -1, minMargin = 0, maxMargin = 0, username = '';

        if (categoryMode === 1) {
            daysLeft = domElements.inputDayLeftGeneral.val();
            minMargin = domElements.inputMinMarginGeneral.val();
            maxMargin = domElements.inputMaxMarginGeneral.val();
            username = domElements.inputUserNameGeneral.val();
        } else {
            daysLeft = domElements.inputDayLeftDetailed.val();
            minMargin = domElements.inputMinMarginDetailed.val();
            maxMargin = domElements.inputMaxMarginDetailed.val();
            username = domElements.inputUserNameDetailed.val();
        }

        paramsFilter.username = username;
        paramsFilter.daysLeft = daysLeft === '' ? -1 : daysLeft;
        paramsFilter.minMargin = minMargin === '' ? 0 : minMargin;
        paramsFilter.maxMargin = maxMargin === '' ? 0 : maxMargin;
        paramsFilter.categoryMode = categoryMode;

        return paramsFilter;
    }    

    function ShowFormChangeCategory(element, username, custId, sportGroupId, categoryMode) {
        let offset = $(element).offset();
        let top = offset.top + 30;
        let left = offset.left - 110;

        if (categoryMode === 1) {
            $('.ui-menu-item', domElements.formGeneralCategories).off('click').on('click', function () {
                let categoryId = $(this).attr('data-category-id');
                let categoryName = $(this).attr('data-category-name');

                ChangeCategory(username, custId, sportGroupId, categoryId, categoryName, categoryMode);
            });

            domElements.formGeneralCategories.css({
                'top': top,
                'left': left,
                'position': 'absolute'
            }).show();
        } else {
            $('.ui-menu-item', domElements.formDetailedCategories).off('click').on('click', function () {
                let categoryId = $(this).attr('data-category-id');
                let categoryName = $(this).attr('data-category-name');

                ChangeCategory(username, custId, sportGroupId, categoryId, categoryName, categoryMode);
            });

            domElements.formDetailedCategories.css({
                'top': top,
                'left': left,
                'position': 'absolute'
            }).show();
        }
    }

    function ChangeCategory(username, custId, sportGroupId, targetCategoryId, targetCategoryName, categoryMode) {
        let html = "<div style='text-align:center'>Are you sure about this action? </div>";
        html += "<div>For confirm this action, account <strong>" + username + "</strong> will change to <strong> " + targetCategoryName + "</strong> category</div>";

        common.showFormConfirmation(html, 'Confirmation', function () {
            let params = {};
            params.custId = custId;
            params.sportGroupId = sportGroupId;
            params.targetCategoryId = targetCategoryId;

            $.ajax(common.modifyRelativeUrl('ProbationManagement/ChangeCategory'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    common.showLoading();
                },
                success: function (response) {
                    if (response !== undefined) {
                        if (response.ErrorCode === 0) {
                            common.showSuccess('Changed category successfully!');

                            categoryMode
                                ? LoadGridGeneral()
                                : LoadGridDetailed();
                        } else {
                            common.showError('Failed! ' + response.ErrorMsg);
                        }
                    }
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    common.closeLoading();
                }
            });
        });
    }

    function Initial() {
        domElements.formGeneralCategories.hide();
        domElements.tabs.tabs();
        domElements.btnGenerateGeneral.on('click', function () { LoadGridGeneral(); });
        domElements.btnGenerateDetailed.on('click', function () { LoadGridDetailed(); });

        $(window).click(function (e) {
            let container = $('.icon-edit');
            if (!container.is(e.target) && container.has(e.target).length === 0) {
                domElements.formGeneralCategories.hide();
                domElements.formDetailedCategories.hide();
            }            
        });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProbationManagement);