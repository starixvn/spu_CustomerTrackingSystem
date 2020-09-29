function transaction_history() {
    var btnSearch = $('#transaction-history-btn-search');
    var dateRange = $('#history_trans_date_range');
    var transactionTable = $('#transaction-history-table');
    var formIPInformation = new form_ip_information();
    var _ctsCustId = 0;
    var _subscriberId = 0;
    var dataTable;

    function RenderGrid(isViewIP) {
        var url = common.modifyRelativeUrl('Account/GetTransactionHistory');

        var table = transactionTable.DataTable({ 
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: true,
            ajax: {
                url: url,
                data: {
                    ctsCustId: function () {
                        return _ctsCustId;
                    },
                    subscriberId: function () {
                        return _subscriberId;
                    },
                    dateRange: function () {
                        return dateRange.val();
                    }
                },
                type: "POST",
                beforeSend: function () {
                    transactionTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    transactionTable.find('tbody').removeClass('loading');
                }
            },
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
            pagingType: "full_numbers",
            ordering: false,
            searching: false,
            lengthMenu: [50, 100, 200, 500],            
            pageLength: 100,
            scrollX: false,
            scrollCollapse: false,           
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -1], className: "text-center" },
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "15%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" }
            ],
            columns: [
                { data: obj => obj.TransTime },
                { data: obj => obj.DeviceId },
                { data: obj => RenderIPInformation(obj) },
                { data: obj => obj.Flagged },
                { data: obj => obj.Action },
                { data: obj => obj.ActionResult },
                { data: obj => obj.OSName },
                { data: obj => obj.BrowserName },
                { data: obj => GetHostName(obj.Url) }
            ]
        });       

        transactionTable.on('click', '.ip-link', ShowFormIPInformation);

        if (!isViewIP)
        {
            table.columns([2]).visible(false);
        }

        return table;
    }

    function GetHostName(url) {
        if (url != null) {
            var m = url.match(/^http:\/\/[^/]+/);
            return m ? m[0] : null;
        }

        return null;
               // common.showError(errMsg);
    }

    function RenderIPInformation(obj) {
        //return "<span class='link ip-link' id=" +  obj.IpId + ">" + obj.Ip + "</span>";
        return obj.Ip;
    }

    function ShowFormIPInformation() {
        var valueIP = $(this).text();
        var targetIPAddress = "#" + $(this).attr("id");

        formIPInformation.OpenModal(valueIP, targetIPAddress);
    }

    this.Initial = function (ctsCustId, subscriberId, isViewIP) {
        _ctsCustId = ctsCustId;
        _subscriberId = subscriberId;
        var dataTable = RenderGrid(isViewIP); 

        btnSearch.on('click', function () {          

            if (dataTable) {
                dataTable.ajax.reload(null, true);
            }
        });


        
    }
}