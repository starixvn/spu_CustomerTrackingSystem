function ProblemMatchMonitorDetail() {
    let dataTable;  

    let domElements = {
        hdMatchId: $("#hdMatchId"),
        hdFraudCaseId: $("#hdFraudCaseId"),
        cbbSite: $("#cbbSite"),
        cbbBetType: $("#cbbBetType"),
        ticketsTable: $("#ticketsTable"),
        btnGenerate: $('#btnGenerate')
    };

    function GenerateGrid() {
        return domElements.ticketsTable.DataTable({
            serverSide: true,
            ajax: {
                url: common.modifyRelativeUrl('ProblemMatchMonitor/GetTickets'),
                data: function () {
                    var paramsFilter = {};
                    paramsFilter.matchId = domElements.hdMatchId.val();
                    paramsFilter.fraudCaseId = domElements.hdFraudCaseId.val();

                    return paramsFilter;
                },
                type: "POST",
                beforeSend: function () {
                    domElements.ticketsTable.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    return response;
                },
                complete: function () {
                    domElements.ticketsTable.find('tbody').removeClass('loading');

                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.ticketsTable.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
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
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 11, 12], className: "text-center" },
                { targets: [8,9,10], className: "text-right" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "5%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "5%" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "5%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "5%" },
                { targets: [12], width: "5%" }
            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => obj.BetDate },
                { data: obj => obj.Username },
                { data: obj => obj.CurrencyName },
                { data: obj => obj.SiteName },
                { data: obj => obj.BetId },
                { data: obj => obj.Choice },
                { data: obj => obj.LiveBetScore },
                { data: obj => obj.Handicap },
                { data: obj => obj.Stake },
                { data: obj => obj.Odds },
                { data: obj => obj.Status },
                { data: obj => obj.IsVoid }
            ]
        });
    }

    function LoadGrid() {
        $(this).prop('disabled', true);
        if (dataTable) {
            dataTable.ajax.reload(null, true);
        } else {
            dataTable = GenerateGrid();
        }
    }

    function Initial() {

        domElements.cbbSite.multiselect({
            menuWidth: 330,
            menuHeight: 330,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 250,
            placeholder: ''
            });

        domElements.cbbBetType.multiselect({
            menuWidth: 200,
            menuHeight: 200,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 150,
            placeholder: ''
        });

        LoadGrid();

        domElements.btnGenerate.on('click', () => LoadGrid());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemMatchMonitorDetail);