function ProblemMatchMonitor() {
    let dataTable;
    let timeoutHolder = null;

    let domElements = {
        dpkDate: $('#dpkDate'),
        cbbSport: $('#cbbSport'),
        cbbLeague: $('#cbbLeague'),
        cbbMatchStatus: $('#cbbMatchStatus'),
        btnGenerate: $('#btnGenerate'),
        refreshInterval: $('#refreshInterval'),
        btnRefresh: $('#btnRefresh'),
        refreshCountDown: $('#refreshCountDown'),
        problemMatchMonitorTable: $('#problemMatchMonitorTable'),
        iconCalendarText: $('.icon-calendar-text')
    };

    function GetSports() {
        $.ajax(common.modifyRelativeUrl('ProblemMatchMonitor/GetSports'), {
            type: 'GET',
            success: function (response) {
                GenerateSportDropdown(response);

                GetLeagues();
            },
            complete: function () {
                domElements.cbbSport.select2({
                    allowClear: true
                });
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GenerateSportDropdown(data) {
        let sportSelector = domElements.cbbSport.empty();

        for (var i = 0; i < data.length; i++) {
            sportSelector.append("<option value='" + data[i].SportType + "'>" + data[i].SportName + "</option>");
        }
    }

    function GetLeagues() {
        var paramsFilter = {};
        paramsFilter.EventDate = domElements.dpkDate.val();
        paramsFilter.SportId = domElements.cbbSport.val();

        $.ajax(common.modifyRelativeUrl('ProblemMatchMonitor/GetLeagues'), {
            type: 'GET',
            data: paramsFilter,
            success: function (response) {
                GenerateLeagueDropdown(response);
            },
            complete: function () {
                domElements.cbbLeague.select2({
                    allowClear: true
                });
            },
            error: function (errMsg) {
            }
        });
    }

    function GenerateLeagueDropdown(data) {
        let leagueSelector = domElements.cbbLeague.empty();

        if (data.length !== 0) {
            var allLeague = data.map(l => l.LeagueId).join();

            leagueSelector.append("<option value='" + allLeague + "'>All League</option>");

            for (var i = 0; i < data.length; i++) {
                leagueSelector.append("<option value='" + data[i].LeagueId + "'>" + data[i].LeagueName + "</option>");
            }

            domElements.btnGenerate.prop("disabled", false);
        }
        else {
            leagueSelector.append("<option value=''>No available league</option>");

            domElements.btnGenerate.prop("disabled", true);
            domElements.refreshInterval.val("--").change();
        }
    }

    function GenerateGrid() {
        return domElements.problemMatchMonitorTable.DataTable({
            serverSide: true,
            ajax: {
                url: common.modifyRelativeUrl('ProblemMatchMonitor/GetProblemMatchMonitor'),
                data: function () {
                    var paramsFilter = {};
                    paramsFilter.eventDate = domElements.dpkDate.val();
                    paramsFilter.leagueIds = domElements.cbbLeague.val();
                    paramsFilter.matchStatus = domElements.cbbMatchStatus.val();
                    return paramsFilter;
                },
                type: "POST",
                beforeSend: function () {
                    domElements.problemMatchMonitorTable.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    return response;
                },
                complete: function () {
                    domElements.problemMatchMonitorTable.find('tbody').removeClass('loading');

                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.problemMatchMonitorTable.find('tbody').removeClass('loading');
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
                { targets: [0, 2, 3, 4, 5], className: "text-center" },
                { targets: [1], className: "text-left" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "55%" },
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
                { data: obj => RenderTickedFraud(obj.MatchId, 13, obj.GroupBettingCount) },
                { data: obj => RenderTickedFraud(obj.MatchId, 14, obj.HedgingCount) }
            ]
        });
    }

    function RenderMatchName(obj) {        
        let html = "<div class='event-match-name'>" + obj.HomeName + " vs " + obj.AwayName + "</div>";
        html += "<div class='event-league-name'>" + obj.LeagueName + "</div>";
        html += "<div class='event-match-id'> Match ID: " + obj.MatchCode + "</div>";

        return html;
    }

    function RenderTickedFraud(matchId, fraudCaseId, tickedFraudCount) {
        if (tickedFraudCount === 0) return "<span class='icon-approval'></span>";
        else {
            var link = "href=ProblemMatchMonitor/Detail?matchId=" + matchId + "&fraudCaseId=" + fraudCaseId + " target='_blank'";
            html = "<a class='warning' " + link + "><span class='icon-alert-circle' ></span>";
            html += "<span class='number'>" + GenerateViolation(tickedFraudCount) + "</span>";
            html += "</a>";

            return html;
        }
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
        if (dataTable) {
            dataTable.ajax.reload(null, true);
        } else {
            dataTable = GenerateGrid();
        }
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
            .change(GetLeagues);
       
        GetSports();

        domElements.refreshInterval.val(60).change(e => RefreshChange($(e.target))).change();
        domElements.iconCalendarText.on('click', () => domElements.dpkDate.focus());
        domElements.btnRefresh.on('click', () => LoadGrid());
        domElements.btnGenerate.on('click', () => LoadGrid());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemMatchMonitor);