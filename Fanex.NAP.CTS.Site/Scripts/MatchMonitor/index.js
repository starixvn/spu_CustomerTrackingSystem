function MatchMonitor() {
    let dataTable;
    let timeoutHolder = null;
    let isSkipTracking = false;
    let sportBetType = [];
    let bettypeList = {};

    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        userId: $("#hdCTSCustId").val(),
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbIsVerified: $('#cbbIsVerified'),
        cbbBetType: $('#cbbBetType'),
        cbbSport: $('#cbbSport'),
        groupDropDown: $('.group-drop-down'),
        refreshList: $('#refreshList'),
        btnRefresh: $('#btnRefresh'),
        tblMonitor: $('#tblMonitor'),
        btnGenerate: $('#btnGenerate'),
        eventTarget: $('#selectedRefresh')[0],
        txtEvent: $('#txtEvent'),
        cbbMarket: $('#cbbMarket'),
        cbbMatchSection: $('#cbbMatchSection'),
        cbbReason: $('#cbbReason'),
        rbtnScanDate: $('#rbtnScanDate'),
        rbtnEventDate: $('#rbtnEventDate')
    };

    function FetchSports() {
        if (sportBetType) {
            let data = {
                MainSport: {
                    Items: {}
                },
                VirtualSport: {
                    Items: {}
                },
                OtherSport: {
                    Items: {}
                }
            }

            sportBetType.filter(x => x.SportGroup == 1).map(x => data.MainSport.Items[`${x.SportType}_${x.LeagueGroupID}`] = ({ Name: x.SportNameDisplay, IsSelected: x.SportType == 1 && x.LeagueGroupID == null, LeagueGroupID: x.LeagueGroupID, SportType: x.SportType }))

            sportBetType.filter(x => x.SportGroup == 2).map(x => data.VirtualSport.Items[`${x.SportType}_${x.LeagueGroupID}`] = ({ Name: x.SportNameDisplay, IsSelected: x.SportType == 1 && x.LeagueGroupID == null, LeagueGroupID: x.LeagueGroupID, SportType: x.SportType }))

            sportBetType.filter(x => x.SportGroup == 3).map(x => data.OtherSport.Items[`${x.SportType}_${x.LeagueGroupID}`] = ({ Name: x.SportNameDisplay, IsSelected: x.SportType == 1 && x.LeagueGroupID == null, LeagueGroupID: x.LeagueGroupID, SportType: x.SportType }))

            let sportGroupSelector = domElements.cbbSport.empty();

            if (data.MainSport.Items && Object.keys(data.MainSport.Items).length !== 0)
                sportGroupSelector.append(BuildOptgroupSport(1, "Main Sport", data.MainSport.Items));

            if (data.VirtualSport.Items && Object.keys(data.VirtualSport.Items).length !== 0)
                sportGroupSelector.append(BuildOptgroupSport(2, "Virtual Sport", data.VirtualSport.Items));

            // if (data.OtherSport.Items && Object.keys(data.OtherSport.Items).length !== 0)
            //     sportGroupSelector.append(BuildOptgroupSport(3, "Other Sport", data.OtherSport.Items));

            domElements.cbbSport.multiselect("refresh");
        }

        FetchBetTypes();
    }

    function BuildOptgroupSport(optgroupValue, optgroupName, sportList) {
        let optgroup = `<optgroup label="${optgroupName}" SportType=${optgroupValue}>`;
        Object.keys(sportList).forEach(index => {
            optgroup += `<option ${sportList[index].IsSelected ? 'selected' : ''} value="${index}">${sportList[index].Name}</option>`;
        })

        optgroup += "</optgroup>";

        return optgroup;
    }

    function FetchBetTypes() {
        if (sportBetType) {
            domElements.cbbBetType.empty();

            let sportSelectedList = domElements.cbbSport.val();
            bettypeList = {};

            sportBetType.filter(x => sportSelectedList.includes(`${x.SportType}_${x.LeagueGroupID}`)).sort(function (a, b) {
                return a.BettypeOrder - b.BettypeOrder;
            }).map(x =>
                    bettypeList[`${x.BetTypeID}_${x.BetID}`] =
                    ({
                        "BetTypeName": x.BetTypeName,
                        "BettypeID": x.BetTypeID,
                        "BetID": x.BetID,
                        "BetIDPattern": x.SportType == 9 ? null : x.BetIDPattern, // Hard code for Badminton then BetID = 0
                        "NameDetailsX": x.NameDetailsX
                    }));

            Object.keys(bettypeList).forEach(value => {
                domElements.cbbBetType.append(`<option selected value="${value}">${bettypeList[value].BetTypeName}</option>`);
            })

            domElements.cbbBetType.multiselect("refresh");
        }
    }

    function GetSportBetTypeList() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetSportBettype'), {
            async: false,
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response?.data) {
                    sportBetType = [...response.data];

                    sportBetType.sort(function (a, b) {
                        return a.SportOrder - b.SportOrder;
                    })
                }
            },
            error: function (xhr, textStatus, error) {
                if (textStatus == 'timeout') {
                    this.tryCount++;
                    if (this.tryCount <= this.retryLimit) {
                        //try again
                        $.ajax(this);
                        return;
                    }
                }

                console.log(error);
            }
        });
    }

    function GetMatchMonitor(isSkip) {
        if (!IsSkipGetMatch() && ValidateRangeDate()) {       
            isSkipTracking = isSkip;
            dataTable ? dataTable.ajax.reload(null, true) : dataTable = GenerateGrid();
        }
    }

    function IsSkipGetMatch() {
        return domElements.tblMonitor.find('tbody').hasClass('loading');
    }

    function ValidateRangeDate() {
        let isValid = true;
        let toDateValue = domElements.dpkToDate.datepicker("getDate");
        let toDateValueAllow = domElements.dpkFromDate.datepicker("getDate");

        if (toDateValueAllow > toDateValue)
            isValid = false;

        toDateValueAllow.setDate(toDateValueAllow.getDate() + 1);

        if (toDateValue > toDateValueAllow)
            isValid = false;

        if (!isValid) {
            common.showError("Invalid date range, please select not more than 2 days");
        }

        return isValid;
    }

    function GenerateFilter() {
        var data = {
            DateType: domElements.rbtnScanDate.is(":checked") ? 1 : 2,
            FromDate: domElements.dpkFromDate.val(),
            ToDate: domElements.dpkToDate.val(),
            Sports: domElements.cbbSport.val().map(x => ({
                "LeagueGroupID": Number.parseInt(x.split('_')[1]),
                "SportTypeID": Number.parseInt(x.split('_')[0])
            })),
            Market: domElements.cbbMarket.val(),
            MatchSection: domElements.cbbMatchSection.val().join(),
            IsVerified: domElements.cbbIsVerified.val(),
            BetTypes: domElements.cbbBetType.val().map(x => MapperBettypes(bettypeList[x])),
            Reasons: domElements.cbbReason.val().join(),
            Event: domElements.txtEvent.val().trim().toUpperCase(),
            isSkipTracking: isSkipTracking
        };

        return data;
    }

    function MapperBettypes(item) {
        var result = {};

        // For Bettype HDP, O/U, 1H HDP, 1H O/U, ... then BetID = 0
        //// Current hardcode for Badminton MapX then BetID = 0
        if (item.BetIDPattern == null && item.NameDetailsX == '') {
            result.BetID = 0;
        }

        // For SportType BasketBall then BetID = 801, 802, 803, 804
        if (item.BetIDPattern != '' && item.NameDetailsX != '') {
            result.BetID = item.BetID;
        }

        // Exception case for Bettype MapX, MapXY then BetID = null

        result.BettypeID = item.BettypeID;

        return result;
    }

    function GenerateGrid() {
        return domElements.tblMonitor.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('MatchMonitor/GetMatchMonitor'),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: function () { return JSON.stringify(GenerateFilter()) },
                type: "POST",
                beforeSend: function () {
                    domElements.tblMonitor.find('tbody').addClass('loading');
                },
                complete: function () {
                    domElements.tblMonitor.find('tbody').removeClass('loading');
                },
                error: function (error) {
                    domElements.tblMonitor.find('tbody').removeClass('loading');
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
            lengthMenu: [1000, 1500, 2000],
            pageLength: 1000,
            scrollX: false,
            scrollCollapse: true,
            scrollY: `${$(window).height() - 340}px`,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 11, 12], className: "text-center" },
                { targets: [7, 8, 9, 10], className: "text-right" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "18%" },
                { targets: [2], width: "18%" },
                { targets: [3], width: "4%" },
                { targets: [4], width: "6%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "6%" },
                { targets: [7], width: "6%" },
                { targets: [8], width: "6%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "4%" },
                { targets: [12], width: "9%" },
                { targets: [13], visible: false }
            ],
            columns: [
                { data: obj => obj.SportName },
                { data: obj => obj.LeagueName },
                {
                    data: obj => () => {
                        let paramTemplate = "EventDateToString={0}&MatchID={1}&HomeID={2}&AwayID={3}&BetTypeID={4}&BettypeName={5}&MatchSection={6}&Market={7}&Reason={8}&ReasonName={9}&LeagueID={10}&BetID={11}&SportType={12}&KOTime={13}&IsSupportShowAllTickets={14}&MX={15}&MY={16}&TicketType={17}";
                        let para = paramTemplate.format(obj.EventDateToString, obj.MatchID, obj.HomeID, obj.AwayID, obj.BettypeID, obj.BettypeName, obj.MatchSection, obj.Market, obj.Reason, obj.ReasonName, obj.LeagueID, obj.BetID, obj.SportType, obj.KOTime, obj.IsSupportShowAllTickets, obj.MapX, obj.MapYY, obj.TicketType);

                        return '<a class="click-view-details" href="MatchMonitor/Detail?' + para + '" target="_blank">' + obj.MatchName + '</a>';
                    }
                },
                { data: obj => obj.MarketName },
                { data: obj => obj.EventDateToString },
                { data: obj => obj.BettypeName },
                {
                    data: obj => () => {
                        if (obj.ReasonName != '')
                            return obj.ReasonName.replaceAll(",", ", ");
                        return obj.ReasonName;
                    }
                },
                { data: obj => ConvertNumberToThousandsString(Math.round(obj.HomeTotalAmount)) },
                { data: obj => ConvertNumberToThousandsString(Math.round(obj.AwayTotalAmount)) },
                { data: obj => ConvertNumberToThousandsString(obj.HomeBetCount) },
                { data: obj => ConvertNumberToThousandsString(obj.AwayBetCount) },
                { data: obj => RenderVerifiedUI(obj.IsVerified) },
                {
                    data: obj => () => {
                        if (obj.VerifiedBy != null && obj.VerifiedBy != '') {
                            return obj.VerifiedBy.replaceAll(",", ", ")
                        }
                        return obj.VerifiedBy;
                    }
                },
                { data: obj => obj.MatchSectionName }
            ],
            drawCallback: function () {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(13, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group group group-site"><td colspan="13"> ' + data[i].MatchSectionName + '</td></tr>'
                        );

                        last = group;
                    }
                });
            }
        });
    }

    function RenderVerifiedUI(isVerified) {
        return '<label class="container-checkbox" id="containerCheckbox"><input id="isVerifiedCheckbox" type="checkbox" disabled' + (isVerified ? ' checked' : '') + '><span class="checkmark"></span></label>';
    }

    function RefreshChange($target) {
        domElements.eventTarget = $target
        var selectedVal = parseInt($target.getAttribute("value"));

        clearTimeout(timeoutHolder);
        StartCountingDown(domElements.btnRefresh, selectedVal, selectedVal);
    }

    function StartCountingDown($target, originalVal, val) {
        if (isNaN(originalVal) || originalVal == -1) {
            domElements.btnRefresh.text("None");
        }
        else {
            domElements.btnRefresh.text("Refresh(" + val + ")");

            if (val !== 0) {
                timeoutHolder = setTimeout(() => StartCountingDown($target, originalVal, val - 1), 1000);
            }
            else {
                domElements.btnRefresh.click();
                StartCountingDown($target, originalVal, originalVal);
            }
        }
    }

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date());

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date());

        // domElements.cbbSport.multiselect({
        //     menuWidth: 260,
        //     menuHeight: 260,
        //     selectedText: "# of # selected",
        //     noneSelectedText: "Select Sport"
        // });
        domElements.cbbSport.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # select",
            noneSelectedText: `Select Sports`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });

        domElements.cbbBetType.multiselect({
            menuWidth: 340,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        }).multiselectfilter({
            width: 260,
            placeholder: '',
            label: ''
        });

        domElements.cbbMatchSection.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Section"
        });

        domElements.cbbReason.multiselect({
            menuWidth: 260,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Select Reason",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        });

        GetSportBetTypeList();

        FetchSports();

        domElements.groupDropDown.hover(function () {
            domElements.refreshList.toggleClass('block-box');
        })

        domElements.btnGenerate.on('click', function () { GetMatchMonitor(false); });
        domElements.btnRefresh.on('click', function () {
            GetMatchMonitor(true);
            RefreshChange(domElements.eventTarget);
        });

        domElements.refreshList.find("li").on('click', function () { RefreshChange(event.target); })

        RefreshChange(domElements.eventTarget);

        domElements.cbbSport.on('change', function () { FetchBetTypes(); });
    }

    $(document).ready(function () {
        Initial();
    });

    $(window).resize(function () {
        $('.dataTables_scrollBody').css('max-height', ($(window).height() - 340));
    });
}

function ConvertNumberToThousandsString(x) {
    return x ? x.toLocaleString() : 0;
}

$(MatchMonitor);