function TransactionReport() {
    let dataTable;
    let domElements = {
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbSubscriber: $('#cbbSubscriber'),
        btnGenerate: $('#btnGenerate'),
        tblTransaction: $('#tblTransaction'),
        footerTransaction: $('#footerTransaction')
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
        jQuery.fn.dataTable.Api.register('sum()', function () {
            return this.flatten().reduce(function (a, b) {
                if (typeof a === 'string') {
                    a = a.replace(/[^\d.-]/g, '') * 1;
                }
                if (typeof b === 'string') {
                    b = b.replace(/[^\d.-]/g, '') * 1;
                }

                return a + b;
            }, 0);
        });

        let table = domElements.tblTransaction.DataTable({
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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "10%" }
            ],
            columns: [
                { data: obj => obj.Subscriber },
                { data: obj => obj.TotalTrans.toLocaleString() },
                { data: obj => obj.TotalValidTrans.toLocaleString() },
                { data: obj => obj.ValidTransNewDevice.toLocaleString() },
                { data: obj => obj.ValidTransOldDevice.toLocaleString() },
                { data: obj => obj.ValidTransRecoverDevice.toLocaleString() },
                { data: obj => obj.ValidTransNoDevice.toLocaleString() },
                { data: obj => obj.InvalidTrans.toLocaleString() },
                { data: obj => obj.Browserless.toLocaleString() }                
            ],
            footerCallback: function () {
                var api = this.api();
                var columns = [1, 2, 3, 4, 5, 6, 7, 8];       
                
                for (var i = 0; i < columns.length; i++) {

                    var number = api.column(columns[i], { filter: 'applied', page: 'current' }).data().sum();

                    $('tfoot th').eq(columns[i]).empty().append(number.toLocaleString());
                }

                if (api.data().length == 0)
                    domElements.footerTransaction.addClass('hide');
                else
                    domElements.footerTransaction.removeClass('hide');
            }
        });              

        return table;
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