function ProblemMatchMonitorDetail() {
    let dataTable;  

    let domElements = {
        hdMatchId: $("#hdMatchId"),
        hdFraudCaseId: $("#hdFraudCaseId"),
        cbbSite: $("#cbbSite"),
        cbbBetType: $("#cbbBetType"),
        cbbVoid: $('#cbbVoid'),
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
                    var json = jQuery.parseJSON(response);

                    RefreshFilter(json.data.SiteList, json.data.BetTypeList);

                    var newJson = {};
                    newJson.data = json.data.TicketList;
                    newJson.recordsTotal = json.recordsTotal;
                    newJson.recordsFiltered = json.recordsFiltered;

                    return JSON.stringify(newJson);
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
                { data: obj => RenderHandicap(obj.HtmlChoice) },
                { data: obj => obj.Stake },
                { data: obj => obj.Odds },
                { data: obj => obj.TicketStatus },
                { data: obj => RenderVoidTicket(obj.BetId, obj.IsVoid) }
            ]
        });
    }

    function RefreshFilter(siteList, betTypeList) {
        $('option', domElements.cbbSite).filter(function () {
            return $(this).val() !== '';
        }).remove();        

        for (let i = 0; i < siteList.length; i++) {
            domElements.cbbSite.append(`<option selected value='${siteList[i].SiteId}'>${siteList[i].SiteName}</option>`);        
        }

        domElements.cbbSite.multiselect("refresh");

        $('option', domElements.cbbBetType).filter(function () {
            return $(this).val() !== '';
        }).remove();

        for (let i = 0; i < betTypeList.length; i++) {
            domElements.cbbBetType.append(`<option selected value='${betTypeList[i].BetTypeId}'>${betTypeList[i].BetTypeName}</option>`);
        }

        domElements.cbbBetType.multiselect("refresh");
    }

    function RenderHandicap(htmlChoice) {
        var handicap = '-';

        if (htmlChoice !== undefined) {
            var spanHandicap = $(htmlChoice).find("span.handicap");
            if (spanHandicap !== undefined) {
                console.log(spanHandicap);
                return spanHandicap.text();
            }
        }

        return handicap;
    }

    function RenderVoidTicket(betId, isVoid) {
        let html = "<label class='checkbox'>";
        html += "<input type='checkbox' class='' " + (isVoid ? 'checked' : '') + " data-bet-id=" + betId + " />";
        html += "<span></span>";
        html += "</label>";

        return html;
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
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sites"
        }).multiselectfilter({
            width: 250,
            placeholder: ''
            });

        domElements.cbbBetType.multiselect({
            menuWidth: 180,
            menuHeight: 180,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        }).multiselectfilter({
            width: 130,
            placeholder: ''
            });

        domElements.cbbVoid.multiselect({
            menuWidth: 180,
            menuHeight: 180,
            selectedText: "# of # selected",
            noneSelectedText: "Select Void"
        }).multiselectfilter({
            width: 130,
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