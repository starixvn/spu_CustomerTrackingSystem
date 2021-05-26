function MatchMonitor() {
    let dataTable;
    let domElements = {
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        refreshBtn: $('.btn-refresh'),
        refreshList: $('.refresh-list'),
        tblMonitor: $('#tblMonitor'),
        btnGenerate: $('#btnGenerate'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        firsttblMonitor: $('#first-tblMonitor'),
        secondtblMonitor: $('#second-tblMonitor'),

    };
    function SetRestrictFromDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");

        domElements.dpkFromDate.datepicker("option", "maxDate", toDateValue);
    }

    function SetRestrictToDate() {
        let fromDateValue = domElements.dpkFromDate.datepicker("getDate");

        domElements.dpkToDate
            .datepicker("option", "minDate", fromDateValue)
            .datepicker("option", "maxDate", moment(fromDateValue).add(1, "year").toDate());
    }
    function GenerateGrid() {
        var data = [];
        for (var i = 0; i < 500; i++) {
            data.push([i, i, i, i, i, i, i]);
        }
        return domElements.tblMonitor.DataTable({
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
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
            ],
            data: data,
            columns: [
                {
                    data: obj => () => {
                        return html = '<a class="click-view-details">Click Me</a>';
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            alert(row, 'click view');
                        });
                    }
                },
                { data: obj => obj.Event_Date },
                { data: obj => obj.League },
                { data: obj => obj.Reason },
                { data: obj => obj.PIC },
                { data: obj => obj.Last_Verified },
                {
                    data: obj => () => {
                        return html = '<label class="container-checkbox" id="containerCheckbox"><input id="overwriteCheckbox" type="checkbox"><span class="checkmark"></span></label>'
                                        
                    }
                }
            ]
        });
    }

    function FirstDetailGird() {
        var data = [];
        for (var i = 0; i < 500; i++) {
            data.push([i, i, i, i, i, i, i , i,i,i,i,i]);
        }
        return domElements.firsttblMonitor.DataTable({
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
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6 , 7 , 8 , 9 , 10 , 11], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
                { targets: [7], width: "15%" },
                { targets: [8], width: "15%" },
                { targets: [9], width: "15%" },
                { targets: [10], width: "15%" },
                { targets: [11], width: "15%" },

            ],
            data: data,
            columns: [
                {
                    data: obj => () => {
                        return html = '<a class="click-view-details">Click Me</a>';
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            alert(row, 'click view');
                        });
                    }
                }
            ]
        });
    }
    function SecondDetailGird() {
        var data = [];
        for (var i = 0; i < 20; i++) {
            data.push([i, i, i, i, i, i, i, i, i, i, i, i]);
        }
        return domElements.secondtblMonitor.DataTable({
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
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
                { targets: [7], width: "15%" },
                { targets: [8], width: "15%" },
                { targets: [9], width: "15%" },
                { targets: [10], width: "15%" },
                { targets: [11], width: "15%" },

            ],
            data: data,
            columns: [
                {
                    data: obj => () => {
                        return html = '<a class="click-view-details">Click Me</a>';
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            alert(row, 'click view');
                        });
                    }
                }
            ]
        });
    }

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictToDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictFromDate);
        domElements.refreshBtn.on('click', function () {
            domElements.refreshList.toggleClass('block-box');
        })
        domElements.btnGenerate.on('click', function () { GenerateGrid(); });
        domElements.btnGenerateDetail.on('click', function () { FirstDetailGird(); SecondDetailGird(); })
    }

    $(document).ready(function () {
        Initial();
    });
}

$(MatchMonitor);