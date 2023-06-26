const MatchMonitorDetailViewType = {
    TicketAll: 3,
    TicketFraud: 2
}

const SiteViewType = {
    SiteAll: "A",
    SiteCredit: "C",
    SiteLicensee: "L"
}

function ConvertNumberToThousandsString(x) {
    return x ? x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") : 0;
}

function MatchMonitorDetail() {
    let domElements = {
        detailpage: $('#detailpage'),
        groupControl: $('#groupControl'),
        breadcrumbButton: $('#breadcrumbButton'),
        groupTable: $('#groupTable'),
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        matchName: $("#hdMatchName").val(),
        betTypeID: $("#hdBetTypeID").val(),
        betTypeName: $("#hdBetTypeName").val(),
        betIDToString: $("#hdBetIDToString").val(),
        reason: $("#hdreason").val(),
        liveIndicator: $("#hdLiveIndicator").val(),
        isSupportShowAllTickets: $("#hdIsSupportShowAllTickets").val(),
        cbbTypeOfTickets: $('#cbbTypeOfTickets'),
        cbbCurrency: $('#cbbCurrency'),
        cbbHDP: $('#cbbHDP'),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbReason: $('#cbbReason'),
        cbbGroup: $('#cbbGroup'),
        cbbSites: $('#cbbSites'),
        inputAmountFrom: $('#inputAmountFrom'),
        inputAmountTo: $('#inputAmountTo'),
        tblMMHomeTeam: $('#tblMMHomeTeam'),
        tblMMAwayTeam: $('#tblMMAwayTeam'),
        divHomeMonitor: $('#first-monitor-datatable'),
        divAwayMonitor: $('#second-monitor-datatable'),
        tblHomeHeaderFixed: $('#tblHomeHeaderFixed'),
        tblAwayHeaderFixed: $('#tblAwayHeaderFixed'),
        spanReasonInfo: $('#spanReasonInfo'),
        inputHomeSelectAll: $('#inputHomeSelectAll'),
        inputAwaySelectAll: $('#inputAwaySelectAll'),
        divPopupAssDetect: $('#divPopupAssDetect'),
        btnShowPopupAssDetection: $('#btnShowPopupAssDetection'),
        tickCheckAll: $('#tickCheckAll'),
        tickUncheckAll: $('#tickUncheckAll'),
        inputOptBP: $('#inputOptBP'),
        inputOptDevice: $('#inputOptDevice'),
        inputOptIP: $('#inputOptIP'),
        inputOptAgent: $('#inputOptAgent'),
        inputOptIPLast3Days: $('#inputOptIPLast3Days'),
        inputOpt3MatchesLast7Days: $('#inputOpt3MatchesLast7Days'),
        hdSelectedCTSCustIDs: $('#hdSelectedCTSCustIDs'),
        hdSelectedCustIDs: $('#hdSelectedCustIDs'),
        btnRefresh: $('#btnRefresh'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        btnShowAll: $('#btnShowAll'),
        btnAssDetection: $('#btnAssDetection'),
        btnExportExcel: $('#btnExportExcel'),
        btnVerifyAndClose: $('#btnVerifyAndClose')
    };

    let recordsFilteredGlobal = 0;
    let recordsTotalGlobal = 0;
    let maxSequenceId = 1;

    let cacheFilter = {};
    let cacheFilterScore = undefined;

    let cacheReasonGroupIDDic = {};

    let posClickGlobal = 0;
    let xhr;
    let currentType = MatchMonitorDetailViewType.TicketFraud;
    let isRefresh = false;
    let isSupportShowAllTickets = false;
    let timeoutRenderTicketDetail = undefined;

    const regexValidAmount = /^[0-9]{1,8}/;
    const regexInvalidAmount = /[^0-9]{1,8}/g;

    function InitialDetailGird(element) {
        element.DataTable({
            serverSide: false,
            // dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            dom: '',
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
            paging: true,
            bInfo: false,
            lengthMenu: [-1, 1000, 1500, 2000],
            pageLength: 1000,
            scrollX: false,
            autoWidth: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 5, 7, 9, 10, 12, 14, 15], className: "text-center" },
                {
                    targets: [0], width: "3%", render: function (data, type, full, meta) {
                        return '<input type="checkbox" class="col-checkbox" name="chkbx" value="' + $('<div/>').text(data).html() + '">';
                    }
                },
                { targets: [1], width: "17%" },
                { targets: [2], width: "4%" },
                { targets: [3], width: "4%" },
                { targets: [4], width: "11%", className: "text-center col-dgr" },
                { targets: [5], width: "5%" },
                { targets: [6], width: "5%", className: "text-center col-betid" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "5%", className: "text-right" },
                { targets: [9], width: "4%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "15%", className: "text-center col-transtime" },
                { targets: [12], width: "5%" },
                { targets: [13], width: "5%", className: "text-center" },
                { targets: [14], width: "7%", className: "text-center col-reason" },
                { targets: [15], width: "3%" },
                { targets: [16], visible: false },
                { targets: [17], visible: false }
            ],
            columns: [
                {
                    data: obj => () => {
                        return `<input type="checkbox" name="chkbx" class="col-checkbox" />`;
                    }
                },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        let classColRed = '';
                        if (obj.IsPA || obj.IsNormalLinkedPA || obj.IsRobot) {
                            classColRed = 'col-red';
                        }
                        return `<a class='click-view-name ${classColRed}' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => obj.CustomerClassToString },
                { data: obj => RenderBotIcon(obj.RobotType, obj.IsRobot) },
                { data: obj => obj.DangerToString },
                { data: obj => obj.Odds },
                { data: obj => obj.TransIdToString },
                { data: obj => obj.CurrencyName },
                {
                    data: obj => ConvertNumberToThousandsString(obj.Amount), createdCell: (cell, _, row) => {
                        if (row.Stake >= 1000) {
                            $(cell).addClass('col-red')
                        }
                    }
                },
                { data: obj => obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTimeToString },
                { data: obj => obj.ADetectGroup },
                { data: obj => obj.MDetectGroup },
                {
                    data: obj => () => {
                        let reasonList = obj.ReasonList;
                        if (reasonList && reasonList != '')
                            return reasonList.replace(",", ", ");
                        return "-";
                    }
                },
                {
                    data: obj => () => {
                        if (obj.PICVerified) return "<span class='icon-Check'></span>";
                        return "-";
                    }
                },
                { data: obj => obj.TransIdToString },
                { data: obj => obj.ADetectGroupID }
            ],
            createdRow: function (row, data, index) {
                if (data.TicketStatus == 102)
                    $(row.cells).addClass('void-ticket');
                else if (data.TicketStatus == 101)
                    $(row.cells).addClass('rejected-ticket');
                else if (data.SubscriberType === "L") {
                    $(row.cells[1]).addClass('username-hightlight');
                }
            }
        });
    }

    function FetchMatchMonitorDetail(typeOfTickets = MatchMonitorDetailViewType.TicketFraud, lastScannedSequenceID = maxSequenceId) {
        recordsFilteredGlobal = 0;

        SetupDataTable();
        setTimeout(() => {
            RefreshLengthPage(500, true);
        }, 10);

        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        if (lastScannedSequenceID || typeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
            domElements.tblMMHomeTeam.find('tbody').addClass('loading');
            domElements.tblMMAwayTeam.find('tbody').addClass('loading');

            if (timeoutRenderTicketDetail) {
                clearTimeout(timeoutRenderTicketDetail);
            }

            $(".breadcrumb-totaltickets").html('');

            timeoutRenderTicketDetail = setTimeout(() => {
                let data = {
                    EventDateToString: domElements.eventDataToString,
                    MatchID: domElements.matchID,
                    BetTypeID: domElements.betTypeID,
                    BetID: domElements.betIDToString,
                    Sites: domElements.cbbSites.val(),
                    Reason: domElements.reason,
                    LiveIndicator: domElements.liveIndicator,
                    LastScannedSequenceID: lastScannedSequenceID,
                    TypeOfTickets: typeOfTickets,
                    IsFilter: IsFilterOptionServerSide(),
                    CurrencyIdList: GetSelectDataFilter(domElements.cbbCurrency),
                    HDPList: GetSelectDataFilter(domElements.cbbHDP),
                    ScoreList: GetSelectDataFilter(domElements.cbbMatchScore),
                    ReasonList: GetSelectDataFilter(domElements.cbbReason),
                    AGroupList: GetSelectDataFilter(domElements.cbbGroup),
                    AmountList: GetInputAmount()?.map(str => {
                        return Number(str);
                    }) ?? undefined,
                };

                GetMatchMonitorTotalTicket(data, lastScannedSequenceID);
                if (recordsTotalGlobal > 0) {
                    GetMatchMonitorDetail(data, lastScannedSequenceID, typeOfTickets == MatchMonitorDetailViewType.TicketAll ? posClickGlobal : 0);
                }
                else {
                    domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                    domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                }
            }, 10)
        }
    }

    function GetMatchMonitorTotalTicket(data = null, lastScannedSequenceID = maxSequenceId) {
        if (lastScannedSequenceID || data.TypeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
            data.LastScannedSequenceID = lastScannedSequenceID;
            currentType = data.TypeOfTickets;

            $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorTotalTicket'), {
                async: false,
                type: 'POST',
                tryCount: 0,
                retryLimit: 3,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response?.recordsTotal >= 0) {
                        recordsTotalGlobal = response?.recordsTotal;
                        $(".breadcrumb-totaltickets").html(`Total Tickets: 0/${ConvertNumberToThousandsString(recordsTotalGlobal)}`);
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
    }

    function GetMatchMonitorDetail(data = null, lastScannedSequenceID = maxSequenceId, posClick = 0) {
        if (lastScannedSequenceID || data.TypeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
            data.LastScannedSequenceID = lastScannedSequenceID;
            currentType = data.TypeOfTickets;

            xhr = $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorDetail'), {
                type: 'POST',
                tryCount: 0,
                retryLimit: 3,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response?.data) {
                        if (data.TypeOfTickets == MatchMonitorDetailViewType.TicketAll && posClickGlobal != posClick) {
                            return;
                        }

                        RenderMatchMonitorDetail(response, data.TypeOfTickets);

                        if (data.TypeOfTickets == MatchMonitorDetailViewType.TicketAll && response?.data?.LastSequenceID) {
                            GetMatchMonitorDetail(data, response?.data?.LastSequenceID, posClick);
                        }

                        if (response?.data?.LastSequenceID == null) {
                            RefreshLengthPage(-1, false);
                        }
                    }
                },
                complete: function (response) {
                    domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                    domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
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

                    RefreshLengthPage(-1, false);
                    console.log(error);
                }
            });
        }
    }

    function RefreshLengthPage(length, isDraw = true) {
        domElements.tblMMHomeTeam.DataTable().page.len(length).draw(isDraw);
        domElements.tblMMAwayTeam.DataTable().page.len(length).draw(isDraw);
    }

    function FetchFilterMatchMonitorDetail() {
        recordsTotalGlobal = 0;

        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        GetFilterMatchMonitorDetail();
    }

    function GetFilterMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorFilter'), {
            async: false,
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            data:
            {
                EventDateToString: function () { return domElements.eventDataToString; },
                MatchID: function () { return domElements.matchID; },
                BetTypeID: function () { return domElements.betTypeID; },
                BetID: function () { return domElements.betIDToString; },
                Sites: function () { return domElements.cbbSites.val(); },
                Reason: function () { return domElements.reason; },
                LiveIndicator: function () { return domElements.liveIndicator; },
                TypeOfTickets: currentType,
                IsRefresh: isRefresh,
                IsSupportShowAllTickets: isSupportShowAllTickets
            },
            success: function (response) {
                const data = response?.data;
                if (data) {
                    RenderFilter(data);
                    maxSequenceId = data.MaxSequenceId;
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
            },
            timeout: 5000
        });

        isRefresh = false;
    }

    function RenderMatchMonitorDetail(response, typeOfTickets = MatchMonitorDetailViewType.TicketFraud) {
        const data = response?.data;
        if (data) {
            if (typeOfTickets == MatchMonitorDetailViewType.TicketFraud ||
                (typeOfTickets == MatchMonitorDetailViewType.TicketAll && data.IsFilter == true)) {
                recordsTotalGlobal = recordsFilteredGlobal = response.recordsFiltered;
            }
            if (typeOfTickets == MatchMonitorDetailViewType.TicketAll && data.IsFilter == false) {
                recordsFilteredGlobal += response?.recordsFiltered ?? 0;
                if (recordsFilteredGlobal > recordsTotalGlobal) {
                    recordsTotalGlobal = recordsFilteredGlobal;
                }
            }

            if (data?.HomeMatchItemList) {
                domElements.tblMMHomeTeam.DataTable().rows.add(data.HomeMatchItemList).draw(false);
            }

            if (data?.AwayMatchItemList) {
                domElements.tblMMAwayTeam.DataTable().rows.add(data.AwayMatchItemList).draw(false);
            }

            $(".breadcrumb-totaltickets").html(`Total Tickets: ${ConvertNumberToThousandsString(recordsFilteredGlobal)}/${ConvertNumberToThousandsString(recordsTotalGlobal)}`);
        }
    }

    function RenderFilter(data) {
        recordsTotalGlobal = data.RecordsTotal;
        cacheReasonGroupIDDic = data.ReasonGroupIDDic;
        SetupFilterFromDictinary(domElements.cbbCurrency, data.CurrencyList);
        SetupFilter(domElements.cbbHDP, data.HDPList);
        SetupFilterScore(domElements.cbbMatchScore, data.ScoreList);
        SetupFilter(domElements.cbbReason, data.ReasonList);
        domElements.spanReasonInfo.text(data.ReasonInfos);
        SetupGroupFilter(data.GroupList);

        if (cacheFilter[MatchMonitorDetailViewType.TicketFraud]) {
            LoadSelectDataFilter();
        }
    }

    function IsFilterOptionServerSide() {
        if (GetSelectDataFilter(domElements.cbbCurrency))
            return true;

        if (GetSelectDataFilter(domElements.cbbHDP))
            return true;

        if (GetSelectDataFilter(domElements.cbbMatchScore))
            return true;

        if (GetSelectDataFilter(domElements.cbbReason))
            return true;

        if (GetSelectDataFilter(domElements.cbbGroup))
            return true;

        if (GetInputAmount() != undefined) {
            return true;
        }

        return false;
    }

    function LoadSelectDataFilter() {
        if (currentType == MatchMonitorDetailViewType.TicketAll) {
            var selectedFilter = cacheFilter[MatchMonitorDetailViewType.TicketFraud];
            if (selectedFilter) {
                RefreshFilterSelected(domElements.cbbCurrency, selectedFilter.cbbCurrency);
                RefreshFilterSelected(domElements.cbbHDP, selectedFilter.cbbHDP);
                RefreshFilterSelected(domElements.cbbReason, selectedFilter.cbbReason);
                RefreshFilterSelected(domElements.cbbGroup, selectedFilter.cbbGroup);
                if (selectedFilter.inputAmount) {
                    domElements.inputAmountFrom.val(selectedFilter.inputAmount[0]);
                    domElements.inputAmountTo.val(selectedFilter.inputAmount[1]);
                }
            }
        }
    }

    function CacheSelectDataFilter() {
        if (currentType == MatchMonitorDetailViewType.TicketFraud && IsFilterOptionServerSide()) {
            var selectedFilter = cacheFilter[currentType];
            if (selectedFilter == undefined) {
                selectedFilter = {};
            }

            selectedFilter.cbbCurrency = GetSelectDataFilter(domElements.cbbCurrency);
            selectedFilter.cbbHDP = GetSelectDataFilter(domElements.cbbHDP);
            cacheFilterScore = GetSelectDataFilter(domElements.cbbMatchScore);
            selectedFilter.cbbReason = GetSelectDataFilter(domElements.cbbReason);
            selectedFilter.cbbGroup = GetSelectDataFilter(domElements.cbbGroup);
            selectedFilter.inputAmount = GetInputAmount();

            cacheFilter[currentType] = selectedFilter;
        }
        else {
            ClearCache();
        }
    }

    function GetInputAmount() {
        if (regexValidAmount.test(domElements.inputAmountFrom.val()) && regexValidAmount.test(domElements.inputAmountTo.val())) {
            if (Number(domElements.inputAmountFrom.val()) <= Number(domElements.inputAmountTo.val())) {
                return [domElements.inputAmountFrom.val(), domElements.inputAmountTo.val()];
            }
        }

        return undefined;
    }

    function GetSelectDataFilter(dom_ccb) {
        let allVal = '';
        for (let i = 0; i < dom_ccb[0].length; i++) {
            allVal += dom_ccb[0][i].value;
        }
        if (dom_ccb.val().join('') != allVal) {
            return dom_ccb.val();
        }

        return undefined;
    }

    function ClearCache() {
        cacheFilter = {};
    }

    function RefreshFilterSelected(element, data) {
        if (element && data) {
            element.val(data);
            element.multiselect("refresh");
        }
    }

    function SetupFilter(element, data) {
        element.multiselect().empty();

        if (data) {
            for (var i = 0; i < data.length; i++) {
                element.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
            }
        }

        element.multiselect("refresh");
    }

    function SetupFilterScore(element, data) {
        if (cacheFilterScore) {
            data = [...new Set([...data, ...cacheFilterScore])];
        }

        SetupFilter(element, data);

        if (cacheFilterScore) {
            setTimeout(RefreshFilterSelected(element, cacheFilterScore), 10);
        }
    }

    function SetupFilterFromDictinary(element, data) {
        element.multiselect().empty();

        if (data) {
            Object.keys(data).forEach(key => {
                element.append(`<option selected value='${data[key]}'>${key}</option>`);
            });
        }

        element.multiselect("refresh");
    }

    function SetupGroupFilter(data) {
        domElements.cbbGroup.multiselect().empty();

        if (data) {
            if (data.length == 0)
                domElements.cbbGroup.append(`<option selected value='0'>-</option>`);

            for (var i = 0; i < data.length; i++) {
                domElements.cbbGroup.append(`<option selected value='${data[i].GroupID}'>${data[i].GroupName}</option>`);
            }
        }

        domElements.cbbGroup.multiselect("refresh");
    }

    function RenderBotIcon(robotType, isRobot) {
        let html = '-';
        let spanHtml = '<span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span><span class="path6"></span><span class="path7"></span>';

        if (isRobot) {
            if (robotType == 1)
                html = `<span class="icon-big-Robot icon-big-good-robot">${spanHtml}</span>`;
            else if (robotType == 2)
                html = `<span class="icon-big-Robot icon-big-bad-robot">${spanHtml}</span>`;
        }

        return html;
    }

    function VerifyTransMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/VerifyTransMMDetail'), {
            type: 'POST',
            data:
            {
                MatchId: domElements.matchID,
                LiveIndicator: domElements.liveIndicator == "True",
                BettypeID: domElements.betTypeID,
                BetID: domElements.betIDToString,
                TranIds: GetTranIds().join()
            },
            beforeSend: function () {
                domElements.tblMMHomeTeam.addClass('loading-value');
                domElements.tblMMAwayTeam.addClass('loading-value');
            },
            success: function () {
                window.close();
            },
            complete: function () {
                domElements.tblMMHomeTeam.removeClass("loading-value");
                domElements.tblMMAwayTeam.removeClass("loading-value");
            },
            error: function (errMsg) {
                console.log(errMsg);
                domElements.tblMMHomeTeam.removeClass("loading-value");
                domElements.tblMMAwayTeam.removeClass("loading-value");
            }
        });
    }

    function GetTranIds() {
        let tranIds = [];
        domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
            if (data.PICVerified == '-' || data.PICVerified == '' || data.PICVerified == null) {
                tranIds.push(data.TransIdToString);
            }
        });
        domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
            if (data.PICVerified == '' || data.PICVerified == '' || data.PICVerified == null) {
                tranIds.push(data.TransIdToString);
            }
        });

        return tranIds;
    }

    function OnlyUnique(value, index, self) {
        return self.indexOf(value) === index;
    }

    function ShowPopupAssDetection() {
        let ctsCustIDs = [];
        let custIDs = [];
        domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                ctsCustIDs.push(data.CTSCustID);
                custIDs.push(data.CustID);
            }
        });
        domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                ctsCustIDs.push(data.CTSCustID);
                custIDs.push(data.CustID);
            }
        });

        ctsCustIDs = ctsCustIDs.filter(OnlyUnique);
        custIDs = custIDs.filter(OnlyUnique);

        let custSelectedLength = custIDs.length;
        if (custSelectedLength <= 1) {
            common.showError("Please select at least 2 Usernames to proceed!");
        }
        else if (custSelectedLength > 1 && custSelectedLength <= 1001) {
            domElements.hdSelectedCTSCustIDs.val(ctsCustIDs.join());
            domElements.hdSelectedCustIDs.val(custIDs.join());
            domElements.divPopupAssDetect.dialog({
                modal: true,
                width: '750px',
                title: 'ASSOCIATION DETECTION',
                titleIsHtml: false,
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            })
        }
    }

    function DetectAssociation() {
        let params = {
            CTSCustIDs: domElements.hdSelectedCTSCustIDs.val(),
            CustIDs: domElements.hdSelectedCustIDs.val(),
            IsOptBP: domElements.inputOptBP.is(":checked"),
            IsOptDevice: domElements.inputOptDevice.is(":checked"),
            IsOptIP: domElements.inputOptIP.is(":checked"),
            IsOptAgent: domElements.inputOptAgent.is(":checked"),
            IsOpt3MatchesLast7Days: domElements.inputOpt3MatchesLast7Days.is(":checked"),
            IsOptIPLast3Days: domElements.inputOptIPLast3Days.is(":checked")
        }

        $.ajax(common.modifyRelativeUrl('MatchMonitor/DetectGroupAssWithOptions'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.tblMMHomeTeam.find('tbody').addClass('loading');
                domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            },
            success: function (response) {
                BindChangeGroupAssWithOptions(domElements.tblMMHomeTeam.DataTable(), response.data.ItemList);
                BindChangeGroupAssWithOptions(domElements.tblMMAwayTeam.DataTable(), response.data.ItemList);
            },
            complete: function (response) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');

                domElements.inputHomeSelectAll.prop('checked', false);
                domElements.inputAwaySelectAll.prop('checked', false);
            },
            error: function (error) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                console.log(error);
            }
        });

        domElements.divPopupAssDetect.dialog('close');
    }

    function BindChangeGroupAssWithOptions(table, itemList) {
        table.rows().every(function () {
            let rowData = this.data();
            if ($(this.node()).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                let newGroupData = itemList.find(function (x) { return x.CustID == rowData.CustID; });
                if (newGroupData != undefined && newGroupData.GroupID > 0) {
                    rowData.MDetectGroupID = newGroupData.GroupID;
                    rowData.MDetectGroup = newGroupData.GroupName;
                } else $(this.node()).css('display', 'none');
            } else $(this.node()).css('display', 'none');

            this.invalidate();
        })
    }

    function ShowAllData(table) {
        table.rows().every(function () {
            $(this.node()).css('display', '');

            this.invalidate();
        })
    }

    function ToggleTickCheckAll(isCheckAll) {
        domElements.inputOptBP.prop("checked", isCheckAll);
        domElements.inputOptDevice.prop("checked", isCheckAll);
        domElements.inputOptIP.prop("checked", isCheckAll);
        domElements.inputOptAgent.prop("checked", isCheckAll);
        domElements.inputOpt3MatchesLast7Days.prop("checked", isCheckAll);
        domElements.inputOptIPLast3Days.prop("checked", isCheckAll);
    }

    function ReasonOnChange() {
        let filterGroupIDs = [];
        var reasonKeys = Object.keys(cacheReasonGroupIDDic);

        $("option:selected", domElements.cbbReason).each(function () {
            let groups = FindByReason($(this).text(), reasonKeys)

            $(groups).each(function (index, element) {
                filterGroupIDs.push(element);
            })
        });

        filterGroupIDs = filterGroupIDs.filter(OnlyUnique);
        filterGroupIDs = filterGroupIDs.sort(function (a, b) {
            return a - b;
        })

        if (filterGroupIDs.indexOf(0) !== -1) {
            filterGroupIDs = $.grep(filterGroupIDs, function (value) {
                return value != 0;
            });
            filterGroupIDs.unshift('-');
        }

        SetupFilter(domElements.cbbGroup, filterGroupIDs);
    }

    function TypeOfTicketsOnChange(isClear) {
        if (isClear) {
            domElements.cbbSites.val(SiteViewType.SiteAll).change();
            cacheFilterScore = undefined;
            domElements.inputAmountFrom.val('');
            domElements.inputAmountTo.val('');
        }

        currentType = Number(domElements.cbbTypeOfTickets.val());
        domElements.btnVerifyAndClose.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);
        FetchFilterMatchMonitorDetail();
    }

    function FindByReason(reason, reasonKeys) {
        let groupIDs = [];
        $.grep(reasonKeys, function (key) {
            if (reason == key) {
                let groups = cacheReasonGroupIDDic[key];
                $(groups).each(function (index, element) {
                    groupIDs.push(element);
                })
            }
            return true;
        });

        return groupIDs;
    }

    function ToggleGridSelectAll(input, table) {
        var nodes = table.rows({ 'search': 'applied' }).nodes();

        $('input[type="checkbox"][name="chkbx"]', nodes).prop('checked', input.is(":checked"));
    }

    function ExportToExcel() {
        const errorExportString = ReturnStringErrorExportExcel();
        if (errorExportString == '') {
            let mmDetails = {
                matchInfo: GetDataMatchInfo(),
                homeMItems: GetDataSelectedInGrid(domElements.tblMMHomeTeam.DataTable()),
                awayMItems: GetDataSelectedInGrid(domElements.tblMMAwayTeam.DataTable())
            }
            let url = common.modifyRelativeUrl('MatchMonitor/ExportToExcelMMDetail');

            $('body').append('<form action="' + url + '" method="post" target="output_frame" id="postToIframe"></form>');
            $('#postToIframe').append('<input type="hidden" asp-for="matchf" name=match value="' + mmDetails.matchInfo + '" />');
            $('#postToIframe').append('<input type="hidden" asp-for="homef" name=homes value="[' + mmDetails.homeMItems + ']" />');
            $('#postToIframe').append('<input type="hidden" asp-for="awayf" name=aways value="[' + mmDetails.awayMItems + ']" />');
            $('body').append('<iframe  name="output_frame" width="0" height="0" class="display-none"></iframe>');
            $('#postToIframe').submit().remove();

            common.showSuccess("Export excel file is in process");
        } else {
            common.showError(errorExportString);
        }
    }

    function ReturnStringErrorExportExcel() {
        let homeCheckedL = 0,
            awayCheckedL = 0;
        domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked'))
                homeCheckedL = homeCheckedL + 1;
        })
        domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked'))
                awayCheckedL = awayCheckedL + 1;
        })

        if (homeCheckedL + awayCheckedL < 2)
            return "Please select at least 2 records to proceed!";
        if (homeCheckedL + awayCheckedL > 40000)
            return "Export fail due to size limitation, please contact Administrator for further support.";

        return '';
    }

    function GetDataSelectedInGrid(table) {
        let datas = '';
        table.rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                if (!$(node).is(':hidden')) {
                    datas += `{&quot;SubscriberType&quot;:&quot;${data.SubscriberType}&quot;`;
                    datas += `,&quot;CTSCustID&quot;:&quot;${data.CTSCustID}&quot;`;
                    datas += `,&quot;CustID&quot;:&quot;${data.CustID}&quot;`;
                    datas += `,&quot;Username&quot;:&quot;${data.Username}&quot;`;
                    datas += `,&quot;CustomerClass&quot;:&quot;${data.CustomerClass}&quot;`;
                    datas += `,&quot;IsRobot&quot;:&quot;${data.IsRobot}&quot;`;
                    datas += `,&quot;DangerToString&quot;:&quot;${data.DangerToString}&quot;`;
                    datas += `,&quot;Odds&quot;:&quot;${data.Odds}&quot;`;
                    datas += `,&quot;BetID&quot;:&quot;${data.BetID}&quot;`;
                    datas += `,&quot;BetTeam&quot;:&quot;${data.BetTeam}&quot;`;
                    datas += `,&quot;Currency&quot;:&quot;${data.CurrencyName}&quot;`;
                    datas += `,&quot;Amount&quot;:&quot;${data.Amount}&quot;`;
                    datas += `,&quot;HDP&quot;:&quot;${data.HDP}&quot;`;
                    datas += `,&quot;Score&quot;:&quot;${data.Score}&quot;`;
                    datas += `,&quot;TransTimeToString&quot;:&quot;${data.TransTimeToString}&quot;`;
                    datas += `,&quot;ADetectGroup&quot;:&quot;${data.ADetectGroup}&quot;`;
                    datas += `,&quot;MDetectGroup&quot;:&quot;${data.MDetectGroup}&quot;`;
                    datas += `,&quot;ReasonList&quot;:&quot;${data.ReasonList}&quot;`;
                    datas += `,&quot;PICVerified&quot;:&quot;${data.PICVerified}&quot;},`;
                }
            }
        });

        return datas != '' ? datas.slice(0, -1) : datas;
    }

    function GetDataMatchInfo() {
        let datas = '';
        datas += `{&quot;MatchName&quot;:&quot;${domElements.matchName}&quot;`;
        datas += `,&quot;EventDateStr&quot;:&quot;${domElements.eventDataToString}&quot;`;
        datas += `,&quot;BetTypeName&quot;:&quot;${domElements.betTypeName}&quot;}`;
        return datas;
    }

    //function BindScrollHeaderFixed() {
    //    let $homeHeader = $('thead', domElements.tblMMHomeTeam).clone();
    //    let $homeFixedHeader = domElements.tblHomeHeaderFixed.append($homeHeader);
    //    let $awayHeader = $('thead', domElements.tblMMAwayTeam).clone();
    //    let $awayFixedHeader = domElements.tblAwayHeaderFixed.append($awayHeader);

    //    $awayFixedHeader.hide();
    //    $homeFixedHeader.hide();

    //    domElements.groupTable.bind("scroll", function () {            
    //        let offset = $(this).scrollTop();
    //        var heightHome = domElements.tblMMHomeTeam.innerHeight();
    //        var heightAway = domElements.tblMMAwayTeam.innerHeight();
    //        if (offset >= heightAway && $awayFixedHeader.is(":hidden")) {                
    //            $awayFixedHeader.show();              
    //        } else if (offset <= heightAway && !$awayFixedHeader.is(":hidden")) {
    //            $awayFixedHeader.hide();
    //        }

    //        if (offset >= heightHome && $homeFixedHeader.is(":hidden")) {
    //            $homeFixedHeader.show();               
    //        } else if (offset <= heightHome && !$homeFixedHeader.is(":hidden")) {
    //            $homeFixedHeader.hide();
    //        }          
    //    })
    //}

    function SetupMultiselect(ccb, selectName, menuWidth = 260, menuHeight = 260) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # selected",
            noneSelectedText: `Select ${selectName} `
        });
    }

    function SetupDataTable() {
        $("#tbrMMHomeTeam").html(`
                                <table id="tblMMHomeTeam" class="table table-striped table-monitor-detail">
                                <thead>
                                    <tr>
                                        <th><input name="select_all" value="1" id="inputHomeSelectAll" type="checkbox" class="col-checkbox" /></th>
                                        <th>Username</th>
                                        <th>CC</th>
                                        <th>Bot</th>
                                        <th>Dgr</th>
                                        <th>Odds</th>
                                        <th>BetID</th>
                                        <th>Cur</th>
                                        <th>Amount</th>
                                        <th>HDP</th>
                                        <th>Score</th>
                                        <th>Transtime</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 1001 Usernames">
                                            M.Gr.<sup style="vertical-align: super; font-size: smaller;">i</sup>
                                        </th>
                                        <th>Reason</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>`);
        $("#tbrMMAwayTeam").html(`
                                <table id="tblMMAwayTeam" class="table table-striped table-monitor-detail">
                                <thead>
                                    <tr>
                                        <th><input name="select_all" value="1" id="inputAwaySelectAll" type="checkbox" class="col-checkbox" /></th>
                                        <th>Username</th>
                                        <th>CC</th>
                                        <th>Bot</th>
                                        <th>Dgr</th>
                                        <th>Odds</th>
                                        <th>BetID</th>
                                        <th>Cur</th>
                                        <th>Amount</th>
                                        <th>HDP</th>
                                        <th>Score</th>
                                        <th>Transtime</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 1001 Usernames">
                                            M.Gr.<sup style="vertical-align: super; font-size: smaller;">i</sup>
                                        </th>
                                        <th>Reason</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>`);

        domElements.tblMMHomeTeam = $("#tblMMHomeTeam");
        domElements.tblMMAwayTeam = $("#tblMMAwayTeam");
        InitialDetailGird(domElements.tblMMHomeTeam);
        InitialDetailGird(domElements.tblMMAwayTeam);

        domElements.inputHomeSelectAll = $('#inputHomeSelectAll');
        domElements.inputAwaySelectAll = $('#inputAwaySelectAll');
        domElements.inputHomeSelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputHomeSelectAll, domElements.tblMMHomeTeam.DataTable()) });
        domElements.inputAwaySelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputAwaySelectAll, domElements.tblMMAwayTeam.DataTable()) });

        domElements.tblHomeHeaderFixed = $('#tblHomeHeaderFixed');
        domElements.tblAwayHeaderFixed = $('#tblAwayHeaderFixed');
    }

    function ValidationFilter() {
        let result = true;

        if (GetSelectDataFilter(domElements.cbbCurrency) == '') {
            result = false;
        }

        if (GetSelectDataFilter(domElements.cbbHDP) == '') {
            result = false;
        }

        if (GetSelectDataFilter(domElements.cbbMatchScore) == '') {
            result = false;
        }

        if (GetSelectDataFilter(domElements.cbbReason) == '') {
            result = false;
        }
        if (GetSelectDataFilter(domElements.cbbGroup) == '') {
            result = false;
        }

        if (!((domElements.inputAmountFrom.val() == '' && domElements.inputAmountTo.val() == '') || GetInputAmount())) {
            common.showError("Please input a valid range (Ex: 500 - 1000)");
            result = false;
        }

        return result;
    }

    function Resize() {
        domElements.groupTable.width($(window).width() - 25);
        domElements.groupTable.height($(window).height() - 250);

        let detailpageHeight = domElements.detailpage.height();

        if (detailpageHeight > 347) {
            domElements.breadcrumbButton.css('top', '40rem');
            domElements.groupTable.css('top', '50rem');
            domElements.groupTable.height($(window).height() - 420);
        } else if (detailpageHeight > 267) {
            domElements.breadcrumbButton.css('top', '32rem');
            domElements.groupTable.css('top', '39rem');
            domElements.groupTable.height($(window).height() - 420);
        } else if (detailpageHeight > 246) {
            domElements.breadcrumbButton.css('top', '30rem');
            domElements.groupTable.css('top', '37rem');
            domElements.groupTable.height($(window).height() - 380);
        } else if (detailpageHeight > 205) {
            domElements.breadcrumbButton.css('top', '26rem');
            domElements.groupTable.css('top', '30.5rem');
            domElements.groupTable.height($(window).height() - 340);
        } else if (detailpageHeight > 188) {
            domElements.breadcrumbButton.css('top', '24rem');
            domElements.groupTable.css('top', '28.5rem');
            domElements.groupTable.height($(window).height() - 300);
        } else {
            domElements.breadcrumbButton.css('top', '17.8rem');
            domElements.groupTable.css('top', '22.5rem');
        }
    }

    function Initial() {
        SetupMultiselect(domElements.cbbCurrency, "Currency");
        SetupMultiselect(domElements.cbbHDP, "HDP");
        SetupMultiselect(domElements.cbbMatchScore, "Score");
        SetupMultiselect(domElements.cbbGroup, "Group");
        SetupMultiselect(domElements.cbbReason, "Reason");

        domElements.cbbReason.change(function () { ReasonOnChange(); });

        try {
            isSupportShowAllTickets = JSON.parse(domElements?.isSupportShowAllTickets?.toLowerCase());
        }
        catch { }

        domElements.cbbTypeOfTickets.prop('disabled', !isSupportShowAllTickets);
        if (isSupportShowAllTickets) {
            domElements.cbbTypeOfTickets.change(function () {
                CacheSelectDataFilter();
                TypeOfTicketsOnChange(currentType == MatchMonitorDetailViewType.TicketAll);
            });
        }

        SetupDataTable();

        domElements.cbbSites.change(function () {
            ClearCache();
            domElements.inputAmountFrom.val('');
            domElements.inputAmountTo.val('');
            cacheFilterScore = GetSelectDataFilter(domElements.cbbMatchScore);
            FetchFilterMatchMonitorDetail();
        });

        domElements.inputAmountFrom.on('input', function () {
            domElements.inputAmountFrom.val(domElements.inputAmountFrom.val().replace(regexInvalidAmount, ""));
        });

        domElements.inputAmountTo.on('input', function () {
            domElements.inputAmountTo.val(domElements.inputAmountTo.val().replace(regexInvalidAmount, ""));
        });

        domElements.btnRefresh.on('click', function () {
            isRefresh = true;
            ClearCache();
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud);
            CacheSelectDataFilter();
            TypeOfTicketsOnChange(true);

            domElements.cbbSites.val(SiteViewType.SiteAll).change();

            FetchMatchMonitorDetail();
        })

        domElements.btnGenerateDetail.on('click', function () {
            if (ValidationFilter()) {
                CacheSelectDataFilter();
                FetchMatchMonitorDetail(currentType);
            }
        })

        domElements.tickCheckAll.on('click', function () { ToggleTickCheckAll(true); });

        domElements.tickUncheckAll.on('click', function () { ToggleTickCheckAll(false); });

        domElements.btnShowAll.on('click', function () {
            ShowAllData(domElements.tblMMHomeTeam.DataTable());
            ShowAllData(domElements.tblMMAwayTeam.DataTable());

            domElements.inputHomeSelectAll.prop('checked', false);
            domElements.inputAwaySelectAll.prop('checked', false);
        })

        domElements.btnShowPopupAssDetection.on('click', function () { ShowPopupAssDetection(); })

        domElements.btnAssDetection.on('click', function () { DetectAssociation(); })

        domElements.btnExportExcel.on('click', function () { ExportToExcel(); })

        domElements.btnVerifyAndClose.on('click', function () {
            common.showConfirmation2('<div class="popup - content">Are you sure about this?<br>Once confirmed, all tickets\' review status will be updated to "Verified".</div>', function () {
                VerifyTransMatchMonitorDetail();
            });
        })

        domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud).change();
        domElements.cbbSites.val(SiteViewType.SiteAll).change();
        domElements.inputHomeSelectAll.prop('checked', false);
        domElements.inputAwaySelectAll.prop('checked', false);

        Resize();
    }

    $(window).resize(function () {
        Resize();
    });

    $(document).ready(function () {
        Initial();

        isRefresh = true;
        FetchFilterMatchMonitorDetail();
        FetchMatchMonitorDetail();

        //BindScrollHeaderFixed();
    });
}

$(MatchMonitorDetail);