function TransactionReport() {
    let dataTable;
    let domElements = {
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),       
        cbbSubscriber: $('#cbbSubscriber'),       
        btnGenerate: $('#btnGenerate'),
        tblTransaction: $('#tblTransaction')
    };

    function GetTransactionReport() {
        if (ValidateRangeDate()) {
            dataTable
                ? dataTable.ajax.reload(null, true)
                : dataTable = GenerateGrid();
        }
    }

    function ValidateRangeDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");
        let toDateValueAllow = domElements.dpkFromDate.datepicker("getDate");
        toDateValueAllow.setDate(toDateValueAllow.getDate() + 29);

        if (toDateValue > toDateValueAllow) {
            common.showError("Invalid date range, please select not more than 30 days");

            return false;
        }

        return true;
    }

    function GenerateGrid() {
        return domElements.tblTransaction.DataTable({
            serverSide: true,
            ajax: {
                url: common.modifyRelativeUrl('TransactionReport/GetTransactions'),
                data: {
                    FromDate: function () { return domElements.dpkFromDate.val(); },
                    ToDate: function () { return domElements.dpkToDate.val(); },                   
                    SubscriberIDs: function () {
                        return domElements.cbbSubscriber.val().join();
                    }                   
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblTransaction.find('tbody').addClass('loading');
                    domElements.btnGenerate.prop('disabled', true);
                },
                dataFilter: function (response) {
                    return response;
                },
                complete: function () {
                    domElements.tblTransaction.find('tbody').removeClass('loading');
                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.tblTransaction.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
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
                { targets: [0, 1, 2, 3, 4], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "20%" },
                { targets: [3], width: "20%" },
                { targets: [4], width: "20%" }
            ],
            columns: [
                { data: obj => obj.SubscriberName },
                { data: obj => obj.TotalTrans, render: $.fn.dataTable.render.number(',', '.', 0, '') },
                { data: obj => obj.ValidTrans, render: $.fn.dataTable.render.number(',', '.', 0, '') },
                { data: obj => obj.InvalidTrans, render: $.fn.dataTable.render.number(',', '.', 0, '') },
                { data: obj => obj.BrowserlessValidTrans, render: $.fn.dataTable.render.number(',', '.', 0, '') }
            ]
        });
    }

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

        domElements.cbbSubscriber.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Subscriber",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.btnGenerate.on('click', function () { GetTransactionReport(); });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(TransactionReport);