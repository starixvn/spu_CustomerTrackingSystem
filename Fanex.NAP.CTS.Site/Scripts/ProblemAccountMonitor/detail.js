function ProblemAccountMonitorDetail() {
    let dataTable;  
    
    let domElements = {
        hdMatchId: $("#hdMatchId"),
        hdFraudCaseId: $("#hdFraudCaseId"),
        hdIsHostPermission: $('#hdIsHostPermission'),
        cbbSite: $("#cbbSite"),
        cbbBetType: $("#cbbBetType"),
        cbbCurrency: $("#cbbCurrency"),
        cbbVoid: $('#cbbVoid'),
        lblMatchName: $('#lblMatchName'),
        lblEvidence: $('#lblEvidence'),
        lblEventDate: $('#lblEventDate'),
        lblScore: $('#lblScore'),
        ticketsTable: $("#ticketsTable"),
        btnShowStakeRange: $('#btnShowStakeRange'),
        lblStakeRange: $('#lblStakeRange'),
        divStakeRangeContainer: $('#divStakeRangeContainer'),
        formInputStakeRange: $('#formInputStakeRange'),
        tblDataTablesWrapper: $(".dataTables_wrapper"),
        inputStakeMin: $('#inputStakeMin'),
        inputStakeMax: $('#inputStakeMax'),
        spanTotalSelections: $('#spanTotalSelections'),
        btnApply: $('#btnApply'),
        btnGenerate: $('#btnGenerate'),
        btnCopyToClipboard: $('#btnCopyToClipboard')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

    function GenerateGrid() {
        return domElements.ticketsTable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('ProblemAccountMonitor/GetTickets'),
                data: function () {
                    var paramsFilter = {};
                    paramsFilter.matchId = domElements.hdMatchId.val();
                    paramsFilter.fraudCaseId = domElements.hdFraudCaseId.val();
                    paramsFilter.stakeFrom = domElements.inputStakeMin.val();
                    paramsFilter.stakeTo = domElements.inputStakeMax.val();
                    return paramsFilter;
                },
                type: "POST",
                beforeSend: function () {
                    domElements.ticketsTable.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    var json = jQuery.parseJSON(response);

                    RefreshFilter(json.data.SiteList, json.data.BetTypeList, json.data.CurrencyList);
                    domElements.spanTotalSelections.text(json.data.TotalHighlightStatusGreen);

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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 13], className: "text-center" },
                { targets: [8, 9, 10], className: "text-right" },
                { targets: [0], width: "3%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "5%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "7%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "5%" },
                { targets: [12], width: "5%" },
                { targets: [13], width: "5%" }
            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => obj.BetDate },
                { data: obj => obj.Username },
                { data: obj => () => { return "<span class=currency data-currency-id=" + obj.CurrencyId + ">" + obj.CurrencyName + "</span>" } },
                { data: obj => () => { return "<span class=site data-site-id=" + obj.SiteId + ">" + obj.SiteName + "</span>"; } },
                { data: obj => obj.BettypeName },
                { data: obj => () => { return "<span class=bettype data-bettype-id=" + obj.BettypeId + ">" + obj.BetId + "</span>"; } },
                { data: obj => obj.Choice },
                { data: obj => obj.LiveBetScore },
                { data: obj => RenderHandicap(obj.HtmlChoice) },
                { data: obj => () => { return "<span class=stake data-stake= " + obj.Stake + ">" + obj.Stake + "</span>"; } },
                { data: obj => obj.Odds },
                {
                    data: obj => () => {
                        let capitalize = obj.TicketStatus;
                        if (typeof capitalize !== 'string') return '';
                        return capitalize.charAt(0).toUpperCase() + capitalize.slice(1);
                    }
                },
                {
                    data: obj => () => {
                        let spanRequestId = '<span> RID: ' + obj.VoidRequestId + '</span>';

                        if (obj.TicketStatusId === 102) return '';

                        return obj.VoidRequestId === ''
                            ? RenderVoidTicket(obj.BetId, obj.HighlightStatus)
                            : spanRequestId;
                    }, createdCell: (cell, _, row) => {
                        $(".inputSelect", cell).click((e) => {                            
                            let isChecked = $(e.target).is(":checked");

                            RefreshTotalSelections(isChecked);
                        });
                    }
                }
            ],
            createdRow: function (row, data, index) {
                let cssClassTR = 'white';
                if (data.HighlightStatus === 1) cssClassTR = 'red';
                else if (data.HighlightStatus === 2) cssClassTR = 'green';
                
                $(row).addClass(cssClassTR);
            }
        });
    }    

    function RefreshFilter(siteList, betTypeList, currencyList) {
        $('option', domElements.cbbSite).filter(function () {
            return $(this).val() !== '';
        }).remove();        
       
        siteList.map(function (x) {
            domElements.cbbSite.append(`<option selected value='${x.SiteId}'>${x.SiteName}</option>`);      
        });

        domElements.cbbSite.multiselect("refresh");

        $('option', domElements.cbbBetType).filter(function () {
            return $(this).val() !== '';
        }).remove();
        
        betTypeList.map(function (x) {
            domElements.cbbBetType.append(`<option selected value='${x.BetTypeId}'>${x.BetTypeName}</option>`);
        });

        domElements.cbbBetType.multiselect("refresh");

        $('option', domElements.cbbCurrency).filter(function () {
            return $(this).val() !== '';
        }).remove();

        currencyList.map(function (x) {
            domElements.cbbCurrency.append(`<option selected value='${x.CurrencyId}'>${x.CurrencyName}</option>`);
        });

        domElements.cbbCurrency.multiselect("refresh");
    }

    function RefreshTotalSelections(checked) {
        let totalSelections = domElements.spanTotalSelections.text();
        let newTotalSelections = parseInt(totalSelections);

        if (checked)
            newTotalSelections = newTotalSelections + 1;
        else
            newTotalSelections = newTotalSelections - 1;

        domElements.spanTotalSelections.text(newTotalSelections);
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

    function RenderVoidTicket(betId, highlightStatus) {
        let checked = highlightStatus === 0;
        let disabled = !isHostPermission;

        let html = "<label class='checkbox'>";
        html += "<input type='checkbox'" + (checked ? ' checked' : '') + (disabled ? ' disabled' : '') + " data-bet-id=" + betId + " class='inputSelect' />";
        html += "<span class='" + (disabled ? 'disabled' : '') + "'></span>";
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

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
    }

    function RefreshVoidTicket(value, checked) {
        domElements.ticketsTable.find('tr').each(function () {
            if ($(this).hasClass(value)) {
                var input = $(this).find("td input");
                if (input.length > 0) {
                    input.prop('checked', checked);
                    RefreshTotalSelections(checked);
                }
            }
        });
    }

    function RefreshAllVoidTicket(checked) {        
        let newTotalSelections = 0;

        domElements.ticketsTable.find('tr').each(function () {
            var input = $(this).find("td input");
            if (input.length > 0) {
                input.prop('checked', checked);
                if (checked)
                    newTotalSelections = newTotalSelections + 1;
            }
        });

        domElements.spanTotalSelections.text(newTotalSelections);
    }

    function RefreshGridByFilterStakeRange() {
        domElements.formInputStakeRange.toggle();

        let minValue = parseFloat(domElements.inputStakeMin.val());
        let maxValue = parseFloat(domElements.inputStakeMax.val());
        let html = minValue + " - " + maxValue;
        domElements.lblStakeRange.text(html);        

        domElements.ticketsTable.find('tr').each(function () {
            let tr = $(this);
            let span = tr.find('span.stake');
            if (span.length > 0) {
                let stakeValue = parseFloat(span.attr('data-stake'));
                let isEqual = stakeValue >= minValue && stakeValue <= maxValue;

                isEqual
                    ? tr.removeClass('hide')
                    : tr.addClass('hide');
            }            
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

    function CopyToClipboardChecked() {
        let arrBetIds = [];

        domElements.ticketsTable.find('tr').each(function () {
            let input = $(this).find("td input");
            let checked = $(this).find("td input:checked");
            let disabled = input.attr('disabled');

            if (checked.length > 0 && !disabled) 
                arrBetIds.push(input.attr('data-bet-id'));
        });

        CopyToClipboard(arrBetIds.join(','));
    }

    function CopyToClipboard(text) {
        let target = document.createElement("textarea");
        target.style.position = "absolute";
        target.style.left = "-9999px";
        target.style.top = "0";
        target.id = "_hiddenCopyText_";
        target.textContent = text;

        document.body.appendChild(target);       
    
        target.focus();
        target.setSelectionRange(0, target.value.length);
        
        try {
            return document.execCommand("copy");
        } catch (e) {
            console.log("CopyToClipboard exception:");
            console.log(e);
            return false;
        }
    }

    function DisableButtons() {
        if (!isHostPermission) {                      
            domElements.btnCopyToClipboard.prop('disabled', true);
            return;
        }
        
        domElements.btnCopyToClipboard.on('click', () => CopyToClipboardChecked());
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

        domElements.cbbCurrency.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Currency",
            click: function (event, ui) { RefreshGridByFilter("td span.currency", "data-currency-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.currency", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.currency", false); }
        }).multiselectfilter({
            width: 130,
            placeholder: ''
            });

        domElements.cbbVoid.multiselect({
            menuWidth: 155,
            menuHeight: 155,
            selectedText: "# of # color(s) selected",
            noneSelectedText: "0 colors selected",
            click: function (event, ui) { RefreshVoidTicket(ui.value, ui.checked); },
            checkAll: function () { RefreshAllVoidTicket(true); },
            uncheckAll: function () { RefreshAllVoidTicket(false); }
        });

        domElements.btnShowStakeRange.on('click', function () {
            domElements.formInputStakeRange.toggle();
            domElements.inputStakeMin.focus();
            $(".dataTables_wrapper").css("position", "sticky");
        });

        $(document).mouseup(function (e) {
            if (domElements.formInputStakeRange.css('display') === 'none') return;

            let container = domElements.divStakeRangeContainer;
            if (!container.is(e.target) && container.has(e.target).length === 0) {
                domElements.formInputStakeRange.toggle();                
            }
        });

        LoadMatchInfo();
        LoadGrid();
        DisableButtons();        

        domElements.btnApply.on('click', () => RefreshGridByFilterStakeRange());
        domElements.btnGenerate.on('click', () => LoadGrid());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemAccountMonitorDetail);