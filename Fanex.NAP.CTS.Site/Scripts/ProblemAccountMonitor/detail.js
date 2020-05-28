function ProblemAccountMonitorDetail() {
    let dataTable;  
    
    let domElements = {
        hdMatchId: $("#hdMatchId"),
        hdFraudCaseId: $("#hdFraudCaseId"),
        hdIsHostPermission: $('#hdIsHostPermission'),
        cbbSite: $("#cbbSite"),
        cbbBetType: $("#cbbBetType"),
        cbbVoid: $('#cbbVoid'),
        lblMatchName: $('#lblMatchName'),
        lblEvidence: $('#lblEvidence'),
        lblEventDate: $('#lblEventDate'),
        lblScore: $('#lblScore'),
        ticketsTable: $("#ticketsTable"),
        btnGenerate: $('#btnGenerate'),
        btnVoidWinningRequest: $('#btnVoidWinningRequest'),
        btnVoidAllRequest: $('#btnVoidAllRequest'),
        btnVoidManualRequest: $('#btnVoidManualRequest')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

    function GenerateGrid() {
        return domElements.ticketsTable.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('ProblemAccountMonitor/GetTickets'),
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
                info: "Total: _TOTAL_  Bet Tickets",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },            
            searching: false,
            ordering: true,           
            paging: false,
            scrollX: false,
            scrollCollapse: false,           
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 11, 12], className: "text-center" },
                { targets: [8, 9, 10], className: "text-right" },
                { targets: [0], width: "3%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "5%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "5%" },
                { targets: [7], width: "7%" },
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
                { data: obj => () => { return "<span class=site data-site-id=" + obj.SiteId + ">" + obj.SiteName + "</span>"; } },
                { data: obj => () => { return "<span class=bettype data-bettype-id=" + obj.BettypeId + ">" + obj.BetId + "</span>"; } },
                { data: obj => obj.Choice },
                { data: obj => obj.LiveBetScore },
                { data: obj => RenderHandicap(obj.HtmlChoice) },
                { data: obj => obj.Stake },
                { data: obj => obj.Odds },
                { data: obj => obj.TicketStatus },
                { data: obj => RenderVoidTicket(obj.BetId, obj.IsVoid, obj.HighlightStatus) }
            ],
            createdRow: function (row, data, index) {
                let cssClassTR = 'white';
                if (data.HighlightStatus === 1) cssClassTR = 'red';
                else if (data.HighlightStatus === 2) cssClassTR = 'green';
                
                $(row).addClass(cssClassTR);
            }
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
                return spanHandicap.text();
            }
        }

        return handicap;
    }

    function RenderVoidTicket(betId, isVoid, highlightStatus) {
        let checked = highlightStatus === 0;
        let disabled = !isHostPermission || isVoid;

        let html = "<label class='checkbox'>";
        html += "<input type='checkbox'" + (checked ? ' checked' : '') + (disabled ? ' disabled' : '') + " data-bet-id=" + betId + " />";
        html += "<span></span>";
        html += "</label>";

        return html;
    }

    function LoadMatchInfo() {
        var url = common.modifyRelativeUrl('/ProblemAccountMonitor/GetMatchInfo');
        var params = {};
        params.matchId = domElements.hdMatchId.val();
        params.fraudCaseId = domElements.hdFraudCaseId.val();

        $.ajax(url, {
            type: 'GET',
            data: params,
            beforeSend: function () {
            },
            success: function (response) {
                domElements.lblMatchName.text(response.MatchName);
                domElements.lblEvidence.text(response.Evidence);
                domElements.lblEventDate.text(response.EventDate);
                domElements.lblScore.text(response.Score);
            },
            complete: function () {
            },
            error: function (errorMessage) {
                console.log(errorMessage);
            }
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

    function RefreshVoidTicket(value, checked) {
        domElements.ticketsTable.find('tr').each(function () {
            if ($(this).hasClass(value)) {
                var input = $(this).find("td input");
                if (input !== undefined)
                    input.prop('checked', checked);
            }
        });
    }

    function RefreshAllVoidTicket(checked) {
        domElements.ticketsTable.find('tr').each(function () {
            var input = $(this).find("td input");
            if (input !== undefined)
                input.prop('checked', checked);
        });
    }

    function RefreshGridByFilter(criteria, attrName, value, checked) {
        domElements.ticketsTable.find('tr').each(function () {
            let tr = $(this);
            let span = tr.find(criteria);            
            if (span.attr(attrName) === value) {
                checked
                    ? tr.removeClass('hide')
                    : tr.addClass('hide');
            }
        });
    }

    function RefreshAllGridByFilter(criteria, checked) {
        domElements.ticketsTable.find('tr').each(function () {
            let tr = $(this);
            let span = tr.find(criteria);
            if (span.length > 0) {
                checked
                    ? tr.removeClass('hide')
                    : tr.addClass('hide');
            }
        });
    }

    function RequestVoidTicket(actionId) {

    }

    function DisableButtons() {
        if (!isHostPermission) {            
            domElements.btnVoidWinningRequest.prop('disabled', true);
            domElements.btnVoidAllRequest.prop('disabled', true);
            domElements.btnVoidManualRequest.prop('disabled', true);

            return;
        }

        domElements.btnVoidWinningRequest.on('click', () => RequestVoidTicket(1));
        domElements.btnVoidAllRequest.on('click', () => RequestVoidTicket(4));
        domElements.btnVoidManualRequest.on('click', () => RequestVoidTicket(5));
    }     

    function Initial() {

        domElements.cbbSite.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sites",
            click: function (event, ui) { RefreshGridByFilter("td span.site", "data-site-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.site", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.site", false); }
        }).multiselectfilter({
            width: 250,
            placeholder: ''
            });

        domElements.cbbBetType.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type",
            click: function (event, ui) { RefreshGridByFilter("td span.bettype", "data-bettype-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.bettype", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.bettype", false); }
        }).multiselectfilter({
            width: 130,
            placeholder: ''
            });

        domElements.cbbVoid.multiselect({
            menuWidth: 180,
            menuHeight: 180,
            selectedText: "# of # color(s) selected",
            noneSelectedText: "0 colors selected",
            click: function (event, ui) { RefreshVoidTicket(ui.value, ui.checked); },
            checkAll: function () { RefreshAllVoidTicket(true); },
            uncheckAll: function () { RefreshAllVoidTicket(false); }
        });

        LoadMatchInfo();
        LoadGrid();
        DisableButtons();

        domElements.btnGenerate.on('click', () => LoadGrid());       
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemAccountMonitorDetail);