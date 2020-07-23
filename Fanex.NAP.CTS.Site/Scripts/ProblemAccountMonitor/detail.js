function ProblemAccountMonitorDetail() {
    let dataTable;  
    
    let domElements = {
        hdMatchId: $("#hdMatchId"),
        hdMatchStatus: $("#hdMatchStatus"),
        hdFraudCaseId: $("#hdFraudCaseId"),
        hdSiteIds: $('#hdSiteIds'),
        hdIsHostPermission: $('#hdIsHostPermission'),
        cbbSite: $("#cbbSite"),
        cbbBetType: $("#cbbBetType"),
        cbbCurrency: $("#cbbCurrency"),
        cbbTicketStatus: $('#cbbTicketStatus'),
        cbbVoid: $('#cbbVoid'),
        cbbGroupId: $('#cbbGroupId'),
        cbbAssociatedGroup: $('#cbbAssociatedGroup'),
        lblMatchName: $('#lblMatchName'),
        lblEvidence: $('#lblEvidence'),
        lblEventDate: $('#lblEventDate'),
        lblScore: $('#lblScore'),
        ticketsTable: $("#ticketsTable"),
        divStakeRangeContainer: $('#divStakeRangeContainer'),  
        tblDataTablesWrapper: $(".dataTables_wrapper"),
        inputStakeMin: $('#inputStakeMin'),
        inputStakeMax: $('#inputStakeMax'),
        spanTotalSelections: $('#spanTotalSelections'),
        btnGenerate: $('#btnGenerate'),
        btnArrowStake: $('.arrowStake'),
        btnCopyToClipboard: $('#btnCopyToClipboard'),
        formCreateVoidRequest: $('#formCreateVoidRequest'),
        btnCreateVoidRequest: $('#btnCreateVoidRequest'),
        divNotificationWarning: $('#divNotificationWarning'),
        btnNotificationWarningClose: $('#btnNotificationWarningClose'),
        hdUserId: $('#hdUserId'),
        hdOnUserId: $('#hdOnUserId'),
        hdTime: $('#hdTime'),
        hdIsTest: $('#hdIsTest'),
        hdSubIssueTypeId: $('#hdSubIssueTypeId'),
        hdActionId: $('#hdActionId'),
        hdTransids: $('#hdTransids')
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
                    paramsFilter.siteIds = domElements.hdSiteIds.val();
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

                    RefreshFilter(json.data.SiteList
                        , json.data.BetTypeList
                        , json.data.CurrencyList
                        , json.data.TicketStatusList
                        , json.data.GroupList
                        , json.data.AssociatedGroupList);

                    domElements.spanTotalSelections.text(json.data.TotalHighlightStatusWhite);

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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13, 14, 15], className: "text-center" },
                { targets: [10, 11, 12], className: "text-right" },
                { targets: [0], width: "3%" },
                { targets: [1], width: "5%" },
                { targets: [2], width: "5%" },
                { targets: [3], width: "8%" },
                { targets: [4], width: "8%" },
                { targets: [5], width: "5%" },
                { targets: [6], width: "5%" },
                { targets: [7], width: "8%" },
                { targets: [8], width: "8%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "5%" },
                { targets: [12], width: "5%" },
                { targets: [13], width: "5%" },
                { targets: [14], width: "5%" },
                { targets: [15], width: "5%" }
            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },               
                { data: obj => () => { return "<span class=group data-group-id=" + obj.GroupId + ">" + obj.GroupName + "</span>"; } },
                {
                    data: obj => () => {
                        name = obj.AssociatedGroupName === "" ? "-" : obj.AssociatedGroupName;
                        return "<span class=associatedGroup data-associated-group=" + name + ">" + name + "</span>";
                    }
                },
                { data: obj => obj.BetDate },
                { data: obj => obj.Username },
                { data: obj => () => { return "<span class=currency data-currency-id=" + obj.CurrencyId + ">" + obj.CurrencyName + "</span>"; } },
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
                        let ticketStatusName = capitalize.charAt(0).toUpperCase() + capitalize.slice(1);
                        let html = "<span class=ticketStatus data-ticket-status-id=" + obj.TicketStatusId + ">" + ticketStatusName + "</span>";
                        return html;
                    }
                },
                {
                    data: obj => () => {
                        let spanRequestId = '<span> RID: ' + obj.VoidRequestId + '</span>';

                        // TicketStatusId = 0 => running, 102 => void
                        if (obj.TicketStatusId === 102 || obj.TicketStatusId !== 0)
                            return '';

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
                $(row).attr('criteria-hide', '');

                // match is running
                if (domElements.hdMatchStatus.val() === "running") {
                    if (data.TicketStatusId !== 0) {
                        $(row).attr('criteria-hide', ',data-ticket-status-id');
                        $(row).addClass("hide");
                    }
                }
            }
        });
    }    

    function RefreshFilter(siteList, betTypeList, currencyList, ticketStatusList, groupList, associatedGroupList) {
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

        $('option', domElements.cbbTicketStatus).filter(function () {
            return $(this).val() !== '';
        }).remove();

        ticketStatusList.map(function (x) {
            let capitalize = x.TicketStatusName;
            if (typeof capitalize !== 'string') return '';
            let ticketStatusName = capitalize.charAt(0).toUpperCase() + capitalize.slice(1);
            let isSelected = false;

            if (domElements.hdMatchStatus.val() !== "running")
                isSelected = true;
            else if (ticketStatusName === "Running")
                isSelected = true;

            domElements.cbbTicketStatus.append("<option " + (isSelected ? "selected" : "") + " value=" + x.TicketStatusId + ">" + ticketStatusName + "</option>");
        });

        domElements.cbbTicketStatus.multiselect("refresh");


        $('option', domElements.cbbGroupId).filter(function () {
            return $(this).val() !== '';
        }).remove();

        groupList.map(function (x) {
            domElements.cbbGroupId.append(`<option selected value='${x.GroupId}'>${x.GroupName}</option>`);
        });

        domElements.cbbGroupId.multiselect("refresh");

        $('option', domElements.cbbAssociatedGroup).filter(function () {
            return $(this).val() !== '';
        }).remove();

        associatedGroupList.map(function (x) {
            domElements.cbbAssociatedGroup.append(`<option selected value='${x}'>${x}</option>`);
        });

        domElements.cbbAssociatedGroup.multiselect("refresh");
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
        let url = common.modifyRelativeUrl('/ProblemAccountMonitor/GetMatchInfo');
        let params = {};
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
        let minText = domElements.inputStakeMin.val();
        let maxText = domElements.inputStakeMax.val();

        if (minText === "")
            minText = "0";
        if (maxText === "")
            maxText = "0";

        try {            
            let minValue = parseFloat(minText);
            let maxValue = parseFloat(maxText);

            if (minValue > maxValue) {                
                common.showError("Stake Range Min must be less than Max");                
                return;
            }
            domElements.ticketsTable.find('tbody').addClass('loading');
            domElements.ticketsTable.find('tr').each(function () {
                let tr = $(this);
                let span = tr.find('span.stake');
                if (span.length > 0) {
                    let stakeValue = parseFloat(span.attr('data-stake'));
                    let defaultValue = minValue === 0 && maxValue === 0;
                    let isEqual = stakeValue >= minValue && stakeValue <= maxValue || defaultValue;

                    let criteriaHideList = GetCriteriaHideList(tr, "stake-range", isEqual);
                    tr.attr("criteria-hide", criteriaHideList.join());

                    criteriaHideList.length === 1
                        ? tr.removeClass('hide')
                        : tr.addClass('hide');
                }
            });
            domElements.ticketsTable.find('tbody').removeClass('loading');
        } catch (e) {
            domElements.ticketsTable.find('tbody').removeClass('loading');
            console.log(e);
        }
    }

    function RefreshGridByFilter(criteria, attrName, value, checked) {
        try {
            domElements.ticketsTable.find('tbody').addClass('loading');
            domElements.ticketsTable.find('tr').each(function () {
                let tr = $(this);
                let span = tr.find(criteria);
                if (span.attr(attrName) === value) {
                    let criteriaHideList = GetCriteriaHideList(tr, attrName, checked);
                    tr.attr("criteria-hide", criteriaHideList.join());

                    criteriaHideList.length === 1
                        ? tr.removeClass('hide')
                        : tr.addClass('hide');
                }
            });
            domElements.ticketsTable.find('tbody').removeClass('loading');
        } catch (e) {
            domElements.ticketsTable.find('tbody').removeClass('loading');
            console.log(e);
        }        
    }

    function RefreshAllGridByFilter(criteria, attrName, checked) {
        try {
            domElements.ticketsTable.find('tbody').addClass('loading');
            domElements.ticketsTable.find('tr').each(function () {
                let tr = $(this);
                let span = tr.find(criteria);
                if (span.length > 0) {

                    let criteriaHideList = GetCriteriaHideList(tr, attrName, checked);
                    tr.attr("criteria-hide", criteriaHideList.join());

                    criteriaHideList.length === 1
                        ? tr.removeClass('hide')
                        : tr.addClass('hide');
                }
            });
            domElements.ticketsTable.find('tbody').removeClass('loading');
        } catch (e) {
            domElements.ticketsTable.find('tbody').removeClass('loading');
            console.log(e);
        }
    }

    function GetCriteriaHideList(tr, attrName, checked) {
        let criteriaHide = tr.attr("criteria-hide");
        let criteriaHideList = criteriaHide.split(',');

        checked
            ? criteriaHideList = $.grep(criteriaHideList, function (ele) {
                return ele !== attrName;
            })
            : criteriaHideList.push(attrName);

        return criteriaHideList;
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

    function GetTickedChecked() {
        let arrBetIds = [];

        domElements.ticketsTable.find('tr').each(function () {
            let input = $(this).find("td input");
            let checked = $(this).find("td input:checked");
            let disabled = input.attr('disabled');

            if (checked.length > 0 && !disabled)
                arrBetIds.push(input.attr('data-bet-id'));
        });

        return arrBetIds;
    }

    function AppendBetIdsToForm(arrBetIds) {
        domElements.hdTransids.empty();

        arrBetIds.map(function (betId) {
            let input = $(document.createElement("input"))
                .attr({ "name": "transIds[]", "value": betId });

            domElements.hdTransids.append(input);
        });
    }

    function CreateVoidRequestToTVS() {
        let arrBetIds = GetTickedChecked();
        AppendBetIdsToForm(arrBetIds);

        let url = common.modifyRelativeUrl('/ProblemAccountMonitor/GetGetInfoVoidRequestToTVS');
        $.ajax(url, {
            type: 'GET',
            async: false,
            success: function (response) {
                domElements.hdUserId.val(response.EnCryptedUserId);
                domElements.hdOnUserId.val(response.EnCryptedOnUserId);
                domElements.hdTime.val(response.EnCryptedTime);
                domElements.hdIsTest.val(response.IsTest);
                domElements.hdSubIssueTypeId.val(response.SubIssueTypeId);
                domElements.hdActionId.val(response.ActionId);
                domElements.formCreateVoidRequest.attr('action', response.RedirectUrl);               
            },
            complete: function () {
                domElements.formCreateVoidRequest.submit();
            },
            error: function (errorMessage) {
                console.log(errorMessage);
            }
        });
    }

    function ValidateVoidRequest() {
        let checkeds = domElements.ticketsTable.find('tr td input:checked');
        let totalSelections = checkeds.length;
        let isPass = parseInt(totalSelections) <= 2000;

        if (!isPass) {
            domElements.btnNotificationWarningClose.on('click', () => {
                domElements.divNotificationWarning.dialog('close');
            });

            domElements.divNotificationWarning.dialog({
                modal: true,
                width: '600px',
                title: 'Notification',
                resizable: false,
                draggable: false,
                close: function () { $(this).dialog('close'); }
            });
        }

        return isPass;
    }

    function UpDownStakeRangeInput(e) {
        let newValue = 0;
        let $button = $(e.target);
        let oldValue = $button.parent().find("input").val();
        if (oldValue === "")
            oldValue = 0;        

        if ($button.attr('data-type') === "up")
            newValue = parseFloat(oldValue) + 1;
        else if (oldValue > 0)
            newValue = parseFloat(oldValue) - 1;

        $button.parent().find("input").val(newValue);
    }

    function DisableButtons() {
        if (!isHostPermission) {                      
            domElements.btnCopyToClipboard.prop('disabled', true);
            domElements.btnCreateVoidRequest.prop('disabled', true);
            return;
        }
        
        domElements.btnCopyToClipboard.on('click', () => CopyToClipboardChecked());
        domElements.btnCreateVoidRequest.on('click', () => {
            if (ValidateVoidRequest())
                CreateVoidRequestToTVS();
        });
    } 

    function InitMultiselectFilter() {
        domElements.cbbSite.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sites",
            click: function (event, ui) { RefreshGridByFilter("td span.site", "data-site-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.site", "data-site-id", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.site", "data-site-id", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.cbbBetType.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type",
            click: function (event, ui) { RefreshGridByFilter("td span.bettype", "data-bettype-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.bettype", "data-bettype-id", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.bettype", "data-bettype-id", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.cbbCurrency.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Currency",
            click: function (event, ui) { RefreshGridByFilter("td span.currency", "data-currency-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.currency", "data-currency-id", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.currency", "data-currency-id", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.cbbTicketStatus.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Status",
            click: function (event, ui) { RefreshGridByFilter("td span.ticketStatus", "data-ticket-status-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.ticketStatus", "data-ticket-status-id", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.ticketStatus", "data-ticket-status-id", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
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

        domElements.cbbGroupId.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Selected Group",
            click: function (event, ui) { RefreshGridByFilter("td span.group", "data-group-id", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.group", "data-group-id", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.group", "data-group-id", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.cbbAssociatedGroup.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Selected Ass Group",
            click: function (event, ui) { RefreshGridByFilter("td span.associatedGroup", "data-associated-group", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.associatedGroup", "data-associated-group", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.associatedGroup", "data-associated-group", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });
    }

    function Initial() {
        InitMultiselectFilter();
        LoadMatchInfo();
        LoadGrid();
        DisableButtons();        
        
        domElements.inputStakeMin.on('keypress', (e) => {
            if ((e.keyCode ? e.keyCode : e.which) === 13) {
                RefreshGridByFilterStakeRange();
            }
        });
        domElements.inputStakeMax.on('keypress', (e) => {
            if ((e.keyCode ? e.keyCode : e.which) === 13) {
                RefreshGridByFilterStakeRange();
            }
        });
        domElements.inputStakeMin.on('blur', () => RefreshGridByFilterStakeRange());
        domElements.inputStakeMax.on('blur', () => RefreshGridByFilterStakeRange());
        domElements.btnArrowStake.on('click', (e) => UpDownStakeRangeInput(e));
        domElements.btnGenerate.on('click', () => LoadGrid());

         // handle freeze header
        $(window).scroll(function (event) {
            var scroll = $(window).scrollTop();
            if (scroll >= 96) {
                let height_filter_detail = $(".form-filter-detail").height() + 22; //208
                let height_info = $(".dataTables_info").height() + 25; //40
                let height_header = $("#ticketsTable_wrapper #ticketsTable thead tr:first-child th").height() + 13;//70

                $("#ticketsTable_wrapper .wrapper-info-table").css({ "top": height_filter_detail + 56 })
                $("#ticketsTable_wrapper #ticketsTable thead tr:first-child th").css({ "top": height_filter_detail + height_info + 55 })
                $("#ticketsTable_wrapper #ticketsTable thead tr:last-child th").css({ "top": height_filter_detail + height_info + height_header + 55 })
            }
            //console.log(scroll);
        });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemAccountMonitorDetail);