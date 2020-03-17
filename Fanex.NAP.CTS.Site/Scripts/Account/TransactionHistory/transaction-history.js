function transaction_history() {
    var btnSearch = $('#transaction-history-btn-search');
    var dateRange = $('#history_trans_date_range');
    var transactionTable = $('#transaction-history-table');    
    var formIPInformation = new form_ip_information();
    var _ctsCustId = 0;
    var _subscriberId = 0;

    function GenerateGrid() {
        transactionTable.DataTable({           
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },            
            ordering: false,
            lengthMenu: [10, 20, 50, 100, 200],
            pageLength: 20,
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
                {
                    targets: [5],
                    width: "15%"
                },
                {
                    targets: [6],
                    width: "15%"
                }
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
    }        

    function GetHostName(url) {
        if (url != null) {
            var m = url.match(/^http:\/\/[^/]+/);
            return m ? m[0] : null;
        }

        return null;
    }    

    function RenderIPInformation(obj) {
        //return "<span class='link ip-link' id=" +  obj.IpId + ">" + obj.Ip + "</span>";
        return obj.Ip;
    }

    function GenerateReport() {

        var url = common.modifyRelativeUrl('/Account/GetTransactionHistory');

        var params = {};
        params.ctsCustId = _ctsCustId;
        params.subscriberId = _subscriberId;
        params.dateRange = dateRange.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            beforeSend: function () {
                transactionTable.find('tbody').addClass('loading');
            },
            success: function (response) {
                BindDataToGrid(response);
            },
            complete: function () {
                transactionTable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    function BindDataToGrid(data) {
        transactionTable.DataTable().clear().rows.add(data).draw();
        transactionTable.DataTable().columns.adjust();
    }

    function ShowFormIPInformation() {
        var valueIP = $(this).text();
        var targetIPAddress = "#" + $(this).attr("id");

        formIPInformation.OpenModal(valueIP, targetIPAddress);
    }

    this.Initial = function (ctsCustId, subscriberId) {
        _ctsCustId = ctsCustId;
        _subscriberId = subscriberId;
        btnSearch.click(GenerateReport);

        GenerateGrid();
    }
}