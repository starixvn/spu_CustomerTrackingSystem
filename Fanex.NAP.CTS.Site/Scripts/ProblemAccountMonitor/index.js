function ProblemAccountMonitor() {
    let dataTable;
    let timeoutHolder = null;
    let isInitLeague = true;

    let domElements = {
        dpkDate: $('#dpkDate'),
        cbbSport: $('#cbbSport'),
        cbbLeague: $('#cbbLeague'),
        cbbSite: $('#cbbSite'),
        cbbMatchStatus: $('#cbbMatchStatus'),
        btnGenerate: $('#btnGenerate'),
        refreshInterval: $('#refreshInterval'),
        btnRefresh: $('#btnRefresh'),
        refreshCountDown: $('#refreshCountDown'),
        problemAccountMonitorTable: $('#problemAccountMonitorTable'),
        iconCalendarText: $('.icon-calendar-text'),
        formViewDetail: $('#formViewDetail'),
        hdMatchId: $('#matchId'),
        hdFraudCaseId: $('#fraudCaseId'),
        hdSiteIds: $('#siteIds'),
        tableLength: null,
        tablePaginate: null
    };

    function GetSports() {
        $.ajax(common.modifyRelativeUrl('ProblemAccountMonitor/GetSports'), {
            type: 'GET',
            success: function (response) {
                GenerateSportDropdown(response);
            },
            complete: function () {
                GetLeagues();
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GenerateSportDropdown(data) {
        let sportSelector = domElements.cbbSport.empty();              

        data.map(function (x) {
            let option = "<option selected value='" + x.SportType + "'>" + x.SportName + "</option>";
            sportSelector.append(option);
        });

        domElements.cbbSport.multiselect("refresh");
    }

    function GetLeagues() {
        if (domElements.cbbSport.val().length === 0)
            return GenerateEmptyLeagueDropdown();

        var paramsFilter = {};
        paramsFilter.EventDate = domElements.dpkDate.val();
        paramsFilter.SportIds = domElements.cbbSport.val().join();

        $.ajax(common.modifyRelativeUrl('ProblemAccountMonitor/GetLeagues'), {
            type: 'GET',
            data: paramsFilter,
            success: function (response) {
                GenerateLeagueDropdown(response);
            },
            complete: function () {
                domElements.cbbLeague.select2();

                if (isInitLeague) {
                    LoadGrid();
                    isInitLeague = false;
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GenerateEmptyLeagueDropdown() {
        let leagueSelector = domElements.cbbLeague.empty();
        leagueSelector.append("<option value=''>No available league</option>");
        domElements.btnGenerate.prop("disabled", true);
        domElements.refreshInterval.val("--").change();
    }

    function GenerateLeagueDropdown(data) {
        if (data.length === 0) {
            return GenerateEmptyLeagueDropdown();
        } else {
            let leagueSelector = domElements.cbbLeague.empty();
            var allLeague = data.map(l => l.LeagueId).join();

            leagueSelector.append("<option value='" + allLeague + "'>All League</option>");

            for (var i = 0; i < data.length; i++) {
                leagueSelector.append("<option value='" + data[i].LeagueId + "'>" + data[i].LeagueName + "</option>");
            }

            domElements.btnGenerate.prop("disabled", false);
        }
    }

    function GetAllSubscriberGroups() {
        $.ajax(common.modifyRelativeUrl('ProblemAccountMonitor/GetAllSubscriberGroups'), {
            type: 'GET',
            success: function (response) {
                GenerateSubscriberGroup(response);
            },
            complete: function () {
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GenerateSubscriberGroup(data) {
        let siteSelector = domElements.cbbSite.empty();

        data.map(function (x) {
            let optgroup = "<optgroup label='" + x.SubscriberGroupName + "'>";

            if (x.SiteList.length > 0)
                x.SiteList.map(function (y) {
                    optgroup += "<option selected value='" + y.SiteID + "'>" + y.SiteName + "</option>";
                });

            optgroup += "</optgroup>";

            siteSelector.append(optgroup);
        });

        domElements.cbbSite.multiselect("refresh");
    }

    function GenerateGrid() {
        return domElements.problemAccountMonitorTable.DataTable({
            serverSide: false,            
            ajax: {
                url: common.modifyRelativeUrl('ProblemAccountMonitor/GetProblemAccountMonitor'),
                data: function () {
                    var paramsFilter = {};
                    paramsFilter.eventDate = domElements.dpkDate.val();
                    paramsFilter.leagueIds = domElements.cbbLeague.val();
                    paramsFilter.matchStatus = domElements.cbbMatchStatus.val();                   
                    paramsFilter.siteIds = domElements.cbbSite.val().join();
                    return paramsFilter;
                },
                type: "POST",
                beforeSend: function () {
                    domElements.problemAccountMonitorTable.find('tbody').addClass('loading');
                    domElements.btnGenerate.prop('disabled', true);
                },
                dataFilter: function (response) {
                    var json = jQuery.parseJSON(response);
                    domElements.tableLength = $('#problemAccountMonitorTable_length');
                    domElements.tablePaginate = $('#problemAccountMonitorTable_paginate');

                    if (json.recordsTotal === 0) {
                        domElements.tableLength.hide();
                        domElements.tablePaginate.hide();
                    } else {
                        domElements.tableLength.show();
                        domElements.tablePaginate.show();
                    }                       
                    return response;
                },
                complete: function () {
                    domElements.problemAccountMonitorTable.find('tbody').removeClass('loading');
                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.problemAccountMonitorTable.find('tbody').removeClass('loading');
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
                info: "Displaying _START_ to _END_ of _TOTAL_ item (s)",
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
                { targets: [0, 2, 3, 4, 5], className: "text-center" },
                { targets: [1], className: "text-left" },
                { targets: [0], width: "3%" },
                { targets: [1], width: "57%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" }
            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => RenderMatchName(obj) },
                { data: obj => obj.MatchScore },
                { data: obj => obj.Status },
                {
                    data: obj => RenderTickedFraud(obj.GroupBettingCount)
                    , createdCell: (cell, _, row) => {
                        $(".divViewDetail", cell).click(() => {
                            ViewDetailTickets(row.MatchId, 13);
                        });
                    }
                },
                {
                    data: obj => RenderTickedFraud(obj.HedgingCount)
                    , createdCell: (cell, _, row) => {
                        $(".divViewDetail", cell).click(() => {
                            ViewDetailTickets(row.MatchId, 14);
                        });
                    }
                }
            ]
        });
    }

    function RenderMatchName(obj) {        
        let html = "<div class='event-match-name'>" + obj.HomeName + " vs " + obj.AwayName + "</div>";
        html += "<div class='event-league-name'>" + obj.LeagueName + " - ";
        html += "<span class='event-sport-name'>" + obj.SportName + "</span>";
        html += "</div>";
        html += "<div class='event-match-id'> Match ID: " + obj.MatchId + "</div>";

        return html;
    }

    function RenderTickedFraud(tickedFraudCount) {
        if (tickedFraudCount === 0) return "<span class='icon-approval'></span>";
        else {           
            let html = "<div class='warning divViewDetail' title='Click to view details'>";
            html += "<span class='icon-alert-circle'></span >";
            html += "<span class='number'>" + GenerateViolation(tickedFraudCount) + "</span>";
            html += "</div>";

            return html;
        }
    }

    function ViewDetailTickets(matchId, fraudCaseId) { 
        let url = common.modifyRelativeUrl('ProblemAccountMonitor/Detail/' + matchId);

        let siteIds = domElements.cbbSite.val().join();
        domElements.hdSiteIds.val(siteIds);
        domElements.hdMatchId.val(matchId);
        domElements.hdFraudCaseId.val(fraudCaseId);
        domElements.formViewDetail.attr('action', url);
        domElements.formViewDetail.submit();
    }

    function GenerateViolation(violation = 1) {
        if (violation < 1000) return violation;

        if (parseInt(violation / 1e6) !== 0) return parseInt(violation / 1e6) + "M";
        else return parseInt(violation / 1e3) + "K";
    }

    function RefreshChange($target) {
        var selectedVal = parseInt($target.val());

        clearTimeout(timeoutHolder);

        if (isNaN(selectedVal)) domElements.refreshCountDown.text("-");

        else StartCountingDown(domElements.refreshCountDown, selectedVal, selectedVal);
    }

    function StartCountingDown($target, originalVal, val) {
        $target.text(val);

        if (val !== 0) timeoutHolder = setTimeout(() => StartCountingDown($target, originalVal, val - 1), 1000);
        else {
            domElements.btnGenerate.click();

            StartCountingDown($target, originalVal, originalVal);
        }
    }

    function LoadGrid() {
        $(this).prop('disabled', true);

        dataTable 
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
    }

    function Initial() {
        domElements.dpkDate
            .datepicker({
                dateFormat: "M dd, yy",
                minDate: balanceUpDay,
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(() => GetLeagues());

        domElements.cbbSport.multiselect({
            header: ['checkAll', 'uncheckAll'],
            menuWidth: 170,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sport"
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
            });
        
        domElements.cbbSite.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Site",        
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
            });

        GetSports();
        GetAllSubscriberGroups();

        domElements.refreshInterval.val(60).change(e => RefreshChange($(e.target))).change();
        domElements.iconCalendarText.on('click', () => domElements.dpkDate.focus());
        domElements.btnRefresh.on('click', () => LoadGrid());
        domElements.btnGenerate.on('click', () => LoadGrid());
        domElements.cbbSport.on('change', () => GetLeagues());
    }

    $(document).ready(function () {
        Initial();        
    });
}

$(ProblemAccountMonitor);