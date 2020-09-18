function ProbationManagement() {
    let dataTableGeneral;
    let dataTableDetailed;

    let domElements = {
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
        formChangeCategory: $('#formChangeCategory')
    };

    function GenerateGridGeneral() {
        return domElements.tblGeneral.DataTable({
            ajax: {
                url: common.modifyRelativeUrl('ProbationManagement/GetProbationCategory'),
                data: function () {
                    return GetGeneralFilter();
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
                { data: obj => obj.TargetDate },
                { data: obj => obj.FirstMargin },
                { data: obj => obj.CurrentMargin },
                { data: obj => obj.DayLeft },
                {
                    data: obj => () => {
                        let html = '<span class="sub-icon delete" id="sub-icon " title="Change Category"><i class="icon-edit"></i></span>';
                        return html;
                    }, createdCell: (cell, _, row) => {
                        $('.icon-edit', cell).click((event) => {
                            ChangeCategory(cell, 1);
                        });
                    }
                }
            ]
        });
    }

    function GetGeneralFilter() {
        let paramsFilter = {};
        let dayLeft = domElements.inputDayLeftGeneral.val();
        let minMargin = domElements.inputMinMarginGeneral.val();
        let maxMargin = domElements.inputMaxMarginGeneral.val();

        paramsFilter.username = domElements.inputUserNameGeneral.val();
        paramsFilter.dayLeft = dayLeft === '' ? 0 : dayLeft;
        paramsFilter.minMargin = minMargin === '' ? 0 : minMargin;
        paramsFilter.maxMargin = maxMargin === '' ? 0 : maxMargin;

        return paramsFilter;
    }

    function GenerateGridDetailed() {
        return domElements.tblDetailed.DataTable({
            ajax: {
                url: common.modifyRelativeUrl('ProbationManagement/GetProbationCategory'),
                data: function () {
                    return GetDetailedFilter();
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
                { data: obj => obj.TargetDate },
                { data: obj => obj.FirstMargin },
                { data: obj => obj.CurrentMargin },
                { data: obj => obj.DayLeft },
                {
                    data: obj => () => {
                        let html = '<span class="sub-icon delete" title="Change Category"><i class="icon-edit"></i></span>';
                        return html;
                    }, createdCell: (cell, _, row) => {
                        $('.icon-edit', cell).click((event) => {
                            ChangeCategory(cell, 1);
                        });
                    }
                }
            ]
        });
    }

    function GetDetailedFilter() {
        let paramsFilter = {};
        let dayLeft = domElements.inputDayLeftDetailed.val();
        let minMargin = domElements.inputMinMarginDetailed.val();
        let maxMargin = domElements.inputMaxMarginDetailed.val();

        paramsFilter.username = domElements.inputUserNameDetailed.val();
        paramsFilter.dayLeft = dayLeft === '' ? 0 : dayLeft;
        paramsFilter.minMargin = minMargin === '' ? 0 : minMargin;
        paramsFilter.maxMargin = maxMargin === '' ? 0 : maxMargin;

        return paramsFilter;
    }

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

    function ChangeCategory(element, custId) {
        let offset = $(element).offset();

        var top = offset.top + 30;
        var left = offset.left - 110;
        domElements.formChangeCategory.css({
            'top': top,
            'left': left,
            'position': 'absolute'
        }).toggle();
    }

    function Initial() {
        domElements.formChangeCategory.hide();
        domElements.tabs.tabs();
        domElements.btnGenerateGeneral.on('click', function () { LoadGridGeneral(); });
        domElements.btnGenerateDetailed.on('click', function () { LoadGridDetailed(); });

        $(window).click(function (e) {
            let container = $('.icon-edit');
            if (!container.is(e.target) && container.has(e.target).length === 0) {
                domElements.formChangeCategory.hide();
            }            
        });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProbationManagement);