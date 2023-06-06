const MatchMonitorDetailViewType = {
    All: 3,
    Fraud: 2
}

const SiteViewType = {
    All: "A",
    Credit: "C",
    Licensee: "L"
}

function MatchMonitorDetail() {
    let domElements = {
        detailpage: $('#detailpage'),
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

    let isFilterOptionServerSide = false;

    let recordsFilteredGlobal = 0;
    let recordsTotalGlobal = 0;
    let cacheSite = SiteViewType.All;
    let cacheTicketDetail;
    let cacheFilter;
    let cacheReasonGroupIDDic = {};

    let posClickGlobal = 0;
    let xhr;
    let currentType = MatchMonitorDetailViewType.Fraud;

    function InitialDetailGird(element) {
        element.DataTable({
            serverSide: false,
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
            searching: true,
            paging: false,
            bInfo: false,
            lengthMenu: [1000, 1500, 2000],
            pageLength: 1000,
            scrollX: false,
            scrollCollapse: false,
            autoWidth: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 5, 7, 8, 9, 10, 12, 14, 15], className: "text-center" },
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
                { targets: [8], width: "6%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "6%" },
                { targets: [11], width: "10%", className: "text-center col-transtime" },
                { targets: [12], width: "5%" },
                { targets: [13], width: "5%", className: "text-center" },
                { targets: [14], width: "8%", className: "text-center col-reason" },
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
                { data: obj => obj.Currency },
                {
                    data: obj => obj.Amount, createdCell: (cell, _, row) => {
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

    function FetchMatchMonitorDetail(typeOfTickets = MatchMonitorDetailViewType.Fraud, lastScannedSequenceID = 0) {
        recordsFilteredGlobal = 0;
        domElements.tblMMHomeTeam.DataTable().clear();
        domElements.tblMMAwayTeam.DataTable().clear();
        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }
        GetMatchMonitorDetail(typeOfTickets, lastScannedSequenceID, typeOfTickets == MatchMonitorDetailViewType.All ? posClickGlobal : 0);
    }

    function GetMatchMonitorDetail(typeOfTickets = MatchMonitorDetailViewType.Fraud, lastScannedSequenceID = 0, posClick = 0) {
        currentType = typeOfTickets;

        const data = {
            EventDateToString: function () { return domElements.eventDataToString; },
            MatchID: function () { return domElements.matchID; },
            BetTypeID: function () { return domElements.betTypeID; },
            BetID: function () { return domElements.betIDToString; },
            Sites: function () { return cacheSite; },
            Reason: function () { return domElements.reason; },
            LiveIndicator: function () { return domElements.liveIndicator; },
            LastScannedSequenceID: lastScannedSequenceID,
            TypeOfTickets: typeOfTickets,
            isFilter: isFilterOptionServerSide,
            CurrencyListString: JSON.stringify(domElements.cbbCurrency.val()),
            HDPListString: JSON.stringify(domElements.cbbHDP.val()),
            ScoreListString: JSON.stringify(domElements.cbbMatchScore.val()),
            ReasonListString: JSON.stringify(domElements.cbbReason.val()),
            ReasonGroupIDDicString: JSON.stringify(domElements.cbbGroup.val())
        };

        xhr = $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorDetail'), {
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            data: data,
            beforeSend: function () {
                domElements.tblMMHomeTeam.find('tbody').addClass('loading');
                domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (typeOfTickets == MatchMonitorDetailViewType.All && posClickGlobal != posClick) {
                    return;
                }

                if (typeOfTickets == MatchMonitorDetailViewType.Fraud) {
                    RenderFilter(response.data);
                    CacheTicketDetail(response);
                }

                RenderMatchMonitorDetail(response, typeOfTickets);

                if (typeOfTickets == MatchMonitorDetailViewType.All && response?.data?.LastSequenceID) {
                    GetMatchMonitorDetail(typeOfTickets, response?.data?.LastSequenceID, posClick);
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

                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                console.log(error);
            },
            timeout: 5000
        });
    }

    function FetchFilterMatchMonitorDetail() {
        recordsTotalGlobal = 0;
        GetFilterMatchMonitorDetail();
    }

    function GetFilterMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorFilter'), {
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            data:
            {
                EventDateToString: function () { return domElements.eventDataToString; },
                MatchID: function () { return domElements.matchID; },
                BetTypeID: function () { return domElements.betTypeID; },
                BetID: function () { return domElements.betIDToString; },
                Sites: function () { return cacheSite; },
                Reason: function () { return domElements.reason; },
                LiveIndicator: function () { return domElements.liveIndicator; },
                TypeOfTickets: MatchMonitorDetailViewType.All,
            },
            success: function (response) {
                const data = response?.data;
                if (data) {
                    recordsTotalGlobal = data.RecordsTotal;
                    RenderFilter(data, recordsTotalGlobal);
                    FetchMatchMonitorDetail(MatchMonitorDetailViewType.All);
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
    }

    function RenderMatchMonitorDetail(response, typeOfTickets = MatchMonitorDetailViewType.Fraud) {
        const data = response?.data;
        if (data) {
            if (typeOfTickets == MatchMonitorDetailViewType.Fraud ||
                (typeOfTickets == MatchMonitorDetailViewType.All && data.isFilter == true)) {
                recordsTotalGlobal = recordsFilteredGlobal = response.recordsFiltered;
            }
            if (typeOfTickets == MatchMonitorDetailViewType.All && data.isFilter == false) {
                recordsFilteredGlobal += response?.recordsFiltered ?? 0;
                if (recordsFilteredGlobal > recordsTotalGlobal) {
                    recordsTotalGlobal = recordsFilteredGlobal;
                }
            }

            domElements.tblMMHomeTeam.DataTable().rows.add(data.HomeMatchItemList).draw();
            domElements.tblMMAwayTeam.DataTable().rows.add(data.AwayMatchItemList).draw();

            $(".breadcrumb-totaltickets").html(`Total Tickets: ${recordsFilteredGlobal}/${recordsTotalGlobal}`);
        }
    }

    function RenderFilter(data, typeOfTickets = MatchMonitorDetailViewType.Fraud, isCache = true) {
        if (data.isFilter === false) {
            cacheReasonGroupIDDic = data.ReasonGroupIDDic;
            SetupFilter(domElements.cbbCurrency, data.CurrencyList);
            SetupFilter(domElements.cbbHDP, data.HDPList);
            SetupFilter(domElements.cbbMatchScore, data.ScoreList);
            SetupFilter(domElements.cbbReason, data.ReasonList);
            domElements.spanReasonInfo.text(data.ReasonInfos);
            SetupGroupFilter(data.GroupList);

            $(".breadcrumb-totaltickets").html(`Total Tickets: 0/${recordsTotalGlobal}`);
        }

        if (data.isFilter === false && typeOfTickets == MatchMonitorDetailViewType.Fraud && isCache) {
            if (cacheFilter == undefined) {
                cacheFilter = {};
            }

            cacheFilter.data = data;
        }
    }

    function RefreshFilter() {
        if (cacheFilter && currentType == MatchMonitorDetailViewType.Fraud) {
            if (cacheFilter.data != undefined) {
                RenderFilter(cacheFilter.data, MatchMonitorDetailViewType.Fraud, false);
            }

            isFilterOptionServerSide = cacheFilter.isFilterOption;
            if (isFilterOptionServerSide === true) {
                RefreshFilterSelected(domElements.cbbCurrency, cacheFilter.cbbCurrency);
                RefreshFilterSelected(domElements.cbbHDP, cacheFilter.cbbHDP);
                RefreshFilterSelected(domElements.cbbMatchScore, cacheFilter.cbbMatchScore);
                RefreshFilterSelected(domElements.cbbReason, cacheFilter.cbbReason);
                RefreshFilterSelected(domElements.cbbGroup, cacheFilter.cbbGroup);
            }
        }
    }

    function CacheTicketDetail(response) {
        if (cacheTicketDetail == undefined) {
            cacheTicketDetail = {};
        }

        cacheTicketDetail.response = response;
    }

    function CacheFilter() {
        if (cacheFilter == undefined) {
            cacheFilter = {};
        }

        cacheFilter.isFilterOption = isFilterOptionServerSide;
        if (isFilterOptionServerSide === true) {
            cacheFilter.cbbCurrency = domElements.cbbCurrency.val();
            cacheFilter.cbbHDP = domElements.cbbHDP.val();
            cacheFilter.cbbMatchScore = domElements.cbbMatchScore.val();
            cacheFilter.cbbReason = domElements.cbbReason.val();
            cacheFilter.cbbGroup = domElements.cbbGroup.val();
        }
    }

    function ClearCache() {
        cacheTicketDetail = undefined;
        cacheFilter = undefined;
        cacheSite = domElements.cbbSites.val();
        domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.Fraud);
    }

    function LoadCacheMatchMonitorDetail() {
        if (cacheTicketDetail) {
            domElements.tblMMHomeTeam.DataTable().clear();
            domElements.tblMMAwayTeam.DataTable().clear();
            posClickGlobal++;
            if (xhr) {
                xhr.onreadystatechange = null;
                xhr.abort();
            }
            RefreshFilter();
            RenderMatchMonitorDetail(cacheTicketDetail.response);
        }
    }

    function RefreshFilterSelected(element, data) {
        element.val(data);
        element.multiselect("refresh");
    }

    function SetupFilter(element, data) {
        element.multiselect().empty();

        for (var i = 0; i < data.length; i++) {
            element.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
        }

        element.multiselect("refresh");
    }

    function SetupGroupFilter(data) {
        domElements.cbbGroup.multiselect().empty();

        if (data.length == 0)
            domElements.cbbGroup.append(`<option selected value='0'>-</option>`);

        for (var i = 0; i < data.length; i++) {
            domElements.cbbGroup.append(`<option selected value='${data[i].GroupID}'>${data[i].GroupName}</option>`);
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
                close();
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

    function DrawGrid() {
        domElements.tblMMHomeTeam.DataTable().draw();
        domElements.tblMMAwayTeam.DataTable().draw();
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

    function TypeOfTicketsOnChange() {
        switch (Number(domElements.cbbTypeOfTickets.val())) {
            case MatchMonitorDetailViewType.All: {
                isFilterOptionServerSide = false;
                currentType = MatchMonitorDetailViewType.All;
                FetchFilterMatchMonitorDetail();
                break;
            }
            case MatchMonitorDetailViewType.Fraud: {
                currentType = MatchMonitorDetailViewType.Fraud;
                LoadCacheMatchMonitorDetail();
                break;
            }
        }
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
        if (CheckRuleExportExcel()) {
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
            common.showError("Please select at least 2 records to proceed!");
        }
    }

    function CheckRuleExportExcel() {
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

        if (homeCheckedL + awayCheckedL >= 2) return true;
        else return false;
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
                    datas += `,&quot;Currency&quot;:&quot;${data.Currency}&quot;`;
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

    function BindScrollHeaderFixed() {
        let $homeHeader = $('thead', domElements.tblMMHomeTeam).clone();
        let $homeFixedHeader = domElements.tblHomeHeaderFixed.append($homeHeader);
        let $awayHeader = $('thead', domElements.tblMMAwayTeam).clone();
        let $awayFixedHeader = domElements.tblAwayHeaderFixed.append($awayHeader);

        $awayFixedHeader.hide();
        $homeFixedHeader.hide();

        $(window).bind("scroll", function () {
            let offset = $(this).scrollTop();
            var heightHome = domElements.tblMMHomeTeam.innerHeight();
            var heightAway = domElements.tblMMAwayTeam.innerHeight();
            if (offset >= heightAway && $awayFixedHeader.is(":hidden")) {
                $awayFixedHeader.show();
            } else if (offset <= heightAway && !$awayFixedHeader.is(":hidden")) {
                $awayFixedHeader.hide();
            }

            if (offset >= heightHome && $homeFixedHeader.is(":hidden")) {
                $homeFixedHeader.show();
            } else if (offset <= heightHome && !$homeFixedHeader.is(":hidden")) {
                $homeFixedHeader.hide();
            }
        })
    }

    function Initial() {
        if (cacheTicketDetail) {
            console.log("aaa")
        }
        domElements.cbbCurrency.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Currency "
        });

        domElements.cbbHDP.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select HDP "
        });

        domElements.cbbMatchScore.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Score "
        });

        domElements.cbbGroup.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Group "
        });

        domElements.cbbReason.multiselect({
            menuWidth: 180,
            menuHeight: 180,
            selectedText: "# of # selected",
            noneSelectedText: "Select Reason "
        });

        domElements.cbbReason.change(function () { ReasonOnChange(); });

        var isSupportShowAllTickets = false;
        try {
            isSupportShowAllTickets = JSON.parse(domElements?.isSupportShowAllTickets?.toLowerCase());
        }
        catch { }

        domElements.cbbTypeOfTickets.prop('disabled', !isSupportShowAllTickets);
        if (isSupportShowAllTickets) {
            domElements.cbbTypeOfTickets.change(function () {
                TypeOfTicketsOnChange();
            });
        }

        InitialDetailGird(domElements.tblMMHomeTeam);

        InitialDetailGird(domElements.tblMMAwayTeam);

        domElements.btnRefresh.on('click', function () {
            isFilterOptionServerSide = false;
            ClearCache();
            FetchMatchMonitorDetail();
        })

        domElements.btnGenerateDetail.on('click', function () {
            isFilterOptionServerSide = true;
            if (currentType == MatchMonitorDetailViewType.Fraud) {
                CacheFilter();
            }

            FetchMatchMonitorDetail(currentType);
        })

        domElements.inputHomeSelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputHomeSelectAll, domElements.tblMMHomeTeam.DataTable()) });

        domElements.inputAwaySelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputAwaySelectAll, domElements.tblMMAwayTeam.DataTable()) })

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

        // FilterGrid();
    }

    $(document).ready(function () {
        Initial();

        domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.Fraud);
        FetchMatchMonitorDetail();

        BindScrollHeaderFixed();
    });
}

$(MatchMonitorDetail);