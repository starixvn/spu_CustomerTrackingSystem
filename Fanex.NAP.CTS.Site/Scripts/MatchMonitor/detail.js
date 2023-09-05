const MatchMonitorDetailViewType = {
    TicketAll: 3,
    TicketFraud: 2
}

const SiteViewType = {
    SiteAll: "A",
    SiteCredit: "C",
    SiteLicensee: "L"
}

const IndexFilterSelected = {
    Score: 1,
    Site: 2,
    Danger: 3,
    Currency: 4,
    Reason: 5,
    AGr: 6
}

function ConvertNumberToThousandsString(x) {
    return x ? x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") : 0;
}

function MatchMonitorDetail() {

    //#region Variables
    let domElements = {
        detailpage: $('#detailpage'),
        divContent: $('#divContent'),
        groupControl: $('#groupControl'),
        divFilterMonitor: $('#divFilterMonitor'),
        divFilterLoading: $('#divFilterLoading'),
        breadcrumbButton: $('#breadcrumbButton'),
        breadcrumbTotalTickets: $(".breadcrumb-totaltickets"),
        groupTable: $('#groupTable'),
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        matchName: $("#hdMatchName").val(),
        betTypeID: $("#hdBetTypeID").val(),
        betTypeName: $("#hdBetTypeName").val(),
        betIDToString: $("#hdBetIDToString").val(),
        sportType: $('#hdSportType').val(),
        sportName: $('#hdSportName').val(),
        reason: $("#hdreason").val(),
        liveIndicator: $("#hdLiveIndicator").val(),
        isSupportShowAllTickets: $("#hdIsSupportShowAllTickets").val(),
        cbbMarket: $('#cbbMarket'),
        cbbBetType: $('#cbbBetType'),
        cbbTypeOfTickets: $('#cbbTypeOfTickets'),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbSites: $('#cbbSites'),
        cbbDanger: $('#cbbDanger'),
        cbbCurrency: $('#cbbCurrency'),
        cbbReason: $('#cbbReason'),
        cbbAGroup: $('#cbbAGroup'),
        cbbMGroup: $('#cbbMGroup'),
        cbbHDPBetTeam: $('#cbbHDPBetTeam'),
        inputAmountFrom: $('#inputAmountFrom'),
        inputAmountTo: $('#inputAmountTo'),
        cbbTicketsStatus: $('#cbbTicketsStatus'),
        tblMMHomeTeam: $('#tblMMHomeTeam'),
        tblMMAwayTeam: $('#tblMMAwayTeam'),
        divHomeMonitor: $('#first-monitor-datatable'),
        divAwayMonitor: $('#second-monitor-datatable'),
        spanSportBetTypeInfo: $('#spanSportBetTypeInfo'),
        spanMarketInfo: $('#spanMarketInfo'),
        spanReasonInfo: $('#spanReasonInfo'),
        inputHomeSelectAll: $('#inputHomeSelectAll'),
        inputAwaySelectAll: $('#inputAwaySelectAll'),
        iconHToggleBetID: $('#iconHToggleBetID'),
        iconAToggleBetID: $('#iconAToggleBetID'),
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
        hdSelectedCustIDs: $('#hdSelectedCustIDs'),
        hdSelectedUsernames: $('#hdSelectedUsernames'),
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
    let cacheFilterDependOn = {
        DropdownFilterList: [],
        MGrList: []
    };
    let cacheMGroup = {
        itemMMHomeTeam: [],
        itemMMAwayTeam: [],
        allMGroup: {},
        selectMgroup: {},
        isActive: false
    };

    let posClickGlobal = 0;
    let xhr;
    let currentType = MatchMonitorDetailViewType.TicketFraud;
    let associationDetection = "AssociationDetection";
    let isRefresh = false;
    let isSupportShowAllTickets = false;
    let isSingleGrid = false;
    let timeoutRenderTicketDetail = undefined;

    const regexValidAmount = /^[0-9]{1,15}/;
    const regexInvalidAmount = /[^0-9]{1,15}/g;
    //#endregion Variables

    //#region Initial Grid
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
                { targets: [0, 1, 2, 3, 5, 7, 9, 10, 12, 13, 14, 15, 16], className: "text-center" },
                {
                    targets: [0], width: "3%", render: function (data, type, full, meta) {
                        return '<input type="checkbox" class="col-checkbox" name="chkbx" value="' + $('<div/>').text(data).html() + '">';
                    }
                },
                { targets: [1], width: "17%" },
                { targets: [2], width: "4%" },
                { targets: [3], width: "4%" },
                { targets: [4], width: "10%", className: "text-center col-dgr" },
                { targets: [5], width: "5%" },
                { targets: [6], width: "5%", className: "text-center col-betid" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "5%", className: "text-right col-amt" },
                { targets: [9], width: "4%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "13%", className: "text-center col-transtime" },
                { targets: [12], width: "3%" },
                { targets: [13], width: "5%" },
                { targets: [14], width: "5%", className: "text-center" },
                { targets: [15], width: "7%", className: "text-center col-reason" },
                { targets: [16], width: "3%" },
                { targets: [17], visible: false },
                { targets: [18], visible: false }
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
                {
                    data: obj => {
                        if (obj.TransIdToString.length > 6) {
                            let halfBetID = '...' + obj.TransIdToString.substring(obj.TransIdToString.length - 6);

                            return `<span class='span-betid' data-bet-id=${obj.TransIdToString}>${halfBetID}</span>`;
                        } else {
                            return `<span class='span-betid' data-bet-id=${obj.TransIdToString}>${obj.TransIdToString}</span>`;
                        }
                    }
                },
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
                { data: obj => obj.BetFrom },
                { data: obj => obj.ADetectGroup },
                {
                    data: obj => () => {
                        const assDetectionItem = cacheMGroup.allMGroup[obj.BetID];
                        if (assDetectionItem) {
                            obj.MDetectGroupID = assDetectionItem.GroupID;
                            obj.MDetectGroup = assDetectionItem.GroupName;
                        }

                        return obj.MDetectGroup;
                    }
                },
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

                if (data.SubscriberType === "L") {
                    $(row.cells[1]).addClass('username-hightlight');
                }
            }
        });
    }

    //#endregion Initial Grid

    //#region Setup Filter & Grid
    function SetupMultiselect(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`
        });
    }

    function SetupMultiselectFilter(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });
    }

    function SetupFilter(element, data) {
        element.multiselect().empty();

        if (data) {
            for (var i = 0; i < data.length; i++) {
                element.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
            }

            element.parent().find('button').removeClass('border-error');
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
            data.forEach(item => {
                element.append(`<option selected value='${item.Key}'>${item.Value}</option>`);
            });

            element.parent().find('button').removeClass('border-error');
        }

        element.multiselect("refresh");
    }

    function SetupAGroupFilter(data) {
        domElements.cbbAGroup.multiselect().empty();

        if (data) {           
            for (var i = 0; i < data.length; i++) {
                domElements.cbbAGroup.append(`<option selected value='${data[i].GroupID}'>${data[i].GroupName}</option>`);
            }

            domElements.cbbAGroup.parent().find('button').removeClass('border-error');
        }

        domElements.cbbAGroup.multiselect("refresh");
    }

    function SetupMGroupFilter(data) {
        domElements.cbbMGroup.multiselect().empty();
        let totalTicketDetect = domElements.divPopupAssDetect.attr('data-total-ticket-detect');
      
        if (data) {
            let isExistedBlank = false;
            if (totalTicketDetect < recordsTotalGlobal) {
                domElements.cbbMGroup.append(`<option value='0'>-</option>`);
                cacheFilterDependOn.MGrList = ReUnshiftBlankMGr(cacheFilterDependOn.MGrList);              
                isExistedBlank = true;
            }

            data.forEach(element => {
                if (element.MDetectGroupID == 0) {
                    if (!isExistedBlank) {
                        domElements.cbbMGroup.append(`<option value='0'>-</option>`);                       
                        cacheFilterDependOn.MGrList = ReUnshiftBlankMGr(cacheFilterDependOn.MGrList);
                    }                    
                } else {
                    domElements.cbbMGroup.append(`<option selected value='${element.MDetectGroupID}'>${ element.MDetectGroup }</option>`);
                }                
            });

            domElements.cbbMGroup.parent().find('button').removeClass('border-error');
        }

        domElements.cbbMGroup.multiselect("refresh");
    }

    function SetupDefaultMGroupFilter(data) {
        domElements.cbbMGroup.multiselect().empty();

        if (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i] == "-") {
                    domElements.cbbMGroup.append(`<option selected value='0'>${data[i]}</option>`);
                } else {
                    domElements.cbbMGroup.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
                }                
            }            

            domElements.cbbMGroup.parent().find('button').removeClass('border-error');
        }

        domElements.cbbMGroup.multiselect("refresh");
    }

    function SetupDanger(data) {
        if (data) {
            let siteSelector = domElements.cbbDanger.empty();

            if (data.NoneDanger.DangerItems.length > 0) siteSelector.append(BuildOptgroup(0, "None", data.NoneDanger.DangerItems));

            if (data.Danger1.DangerItems.length > 0) siteSelector.append(BuildOptgroup(1, "Original", data.Danger1.DangerItems));

            if (data.Danger2.DangerItems.length > 0) siteSelector.append(BuildOptgroup(2, "ABI", data.Danger2.DangerItems));

            if (data.Danger3.DangerItems.length > 0) siteSelector.append(BuildOptgroup(3, "Other Sports", data.Danger3.DangerItems));

            if (data.Danger4.DangerItems.length > 0) siteSelector.append(BuildOptgroup(4, "Basketball", data.Danger4.DangerItems));

            if (data.Danger5.DangerItems.length > 0) siteSelector.append(BuildOptgroup(5, "E-Sports", data.Danger5.DangerItems));

            domElements.cbbDanger.parent().find('button').removeClass('border-error');

            domElements.cbbDanger.multiselect('refresh');
        }
    }

    function BuildOptgroup(optgroupValue, optgroupName, items) {
        let optgroup = `<optgroup label="${optgroupName}" DangerType=${optgroupValue}>`;
        items.map(function (x) {
            optgroup += `<option selected value=${optgroupValue}-${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
        })
        optgroup += "</optgroup>";

        return optgroup;
    }

    function SetupSubscriberTypeFilter(data) {
        domElements.cbbSites.empty();
        if (data) {
            data.forEach(item => {
                let subName = "All";
                if (item == "C") {
                    subName = "Credit";
                } else if (item == "L") {
                    subName = "Licensee";
                }
                domElements.cbbSites.append(`<option value='${item}'>${subName}</option>`);
            });
        }
    }

    function SetupTicketStatusFilter(data) {
        domElements.cbbTicketsStatus.multiselect().empty();

        if (data) {
            data.forEach(item => {
                if (item.TicketStatus == 101) { // untick "Reject"
                    domElements.cbbTicketsStatus.append(`<option value='${item.TicketStatus}'>${item.TicketStatusName}</option>`);
                } else {
                    domElements.cbbTicketsStatus.append(`<option selected value='${item.TicketStatus}'>${item.TicketStatusName}</option>`);
                }
            });

            domElements.cbbTicketsStatus.parent().find('button').removeClass('border-error');
        }

        domElements.cbbTicketsStatus.multiselect("refresh");
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
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">BetID</span>
                                                <span id="iconHToggleBetID" class="dropdown-button icon-collapse-expand">
                                                    <span></span>
                                                    <span></span>
                                                    <span></span>
                                                </div>
                                            </div>
                                        </th>
                                        <th>Cur</th>
                                        <th>Amt</th>
                                        <th>HDP</th>
                                        <th>Score</th>
                                        <th>Transtime</th>
                                        <th>B.f.</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 5000 Usernames">
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
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">BetID</span>
                                                <span id="iconAToggleBetID" class="dropdown-button icon-collapse-expand">
                                                    <span></span>
                                                    <span></span>
                                                    <span></span>
                                                </span>
                                            </div>
                                        </th>
                                        <th>Cur</th>
                                        <th>Amt</th>
                                        <th>HDP</th>
                                        <th>Score</th>
                                        <th>Transtime</th>
                                        <th>B.f.</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 5000 Usernames">
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

        domElements.iconHToggleBetID = $('#iconHToggleBetID');
        domElements.iconAToggleBetID = $('#iconAToggleBetID');

        domElements.iconHToggleBetID.on('click', function () {
            if (domElements.iconHToggleBetID.hasClass('expand')) {
                domElements.iconHToggleBetID.removeClass('expand');
                ToggleBetID(domElements.tblMMHomeTeam, false);
            } else {
                domElements.iconHToggleBetID.addClass('expand');
                ToggleBetID(domElements.tblMMHomeTeam, true);
            }
        });
        domElements.iconAToggleBetID.on('click', function () {
            if (domElements.iconAToggleBetID.hasClass('expand')) {
                domElements.iconAToggleBetID.removeClass('expand');
                ToggleBetID(domElements.tblMMAwayTeam, false);
            } else {
                domElements.iconAToggleBetID.addClass('expand');
                ToggleBetID(domElements.tblMMAwayTeam, true);
            }
        });
    }

    //#endregion Setup Filter & Grid

    //#region Get Filter Params Selected
    function GetInputAmount() {
        if (regexValidAmount.test(domElements.inputAmountFrom.val()) && regexValidAmount.test(domElements.inputAmountTo.val())) {
            if (Number(domElements.inputAmountFrom.val()) <= Number(domElements.inputAmountTo.val())) {
                return [domElements.inputAmountFrom.val(), domElements.inputAmountTo.val()];
            }
        }

        return undefined;
    }

    function GetSelectDataFilter(dom_ccb, isGetAllValue = false) {
        let allVal = '';

        if (dom_ccb[0].length == 0) {
            return allVal;
        }

        for (let i = 0; i < dom_ccb[0].length; i++) {
            allVal += dom_ccb[0][i].value;
        }
        if (isGetAllValue || dom_ccb.val().join('') != allVal) {
            return dom_ccb.val();
        }

        return undefined;
    }

    function GetHDPSelected(dom_ccb, isGetAllValue = false) {
        return isSingleGrid ? undefined : GetSelectDataFilter(dom_ccb, isGetAllValue);
    }

    function GetBetTeamSelected(dom_ccb, isGetAllValue = false) {
        return isSingleGrid ? GetSelectDataFilter(dom_ccb, isGetAllValue) : undefined;
    }

    function GetBetTypeSelected() {
        let betTypeSelected = $("option:selected", domElements.cbbBetType);

        if (betTypeSelected.length > 0) {
            return betTypeSelected.attr('data-bettypeid')
        }

        return domElements.betTypeID;
    }

    function GetBetIDSelected() {
        if (domElements.sportType == 2) {
            let betTypeSelected = $("option:selected", domElements.cbbBetType);

            if (betTypeSelected.length > 0) {
                return betTypeSelected.attr('data-betid');
            }
        }
        return domElements.betIDToString;
    }

    function ValidateTicketStatus() {
        let opt = $("option", domElements.cbbTicketsStatus).length;
        if (opt > 0) {
            return true;
        }
        domElements.tblMMHomeTeam.DataTable().clear().draw(false);
        domElements.tblMMAwayTeam.DataTable().clear().draw(false);

        return false;
    }   

    function CheckSelected(element) {
        let opt = $("option", element).length;
        let optSelect = $("option:selected", element).length;

        if (opt > 0 && optSelect > 0) {
            return true;
        }
        return false;
    }      

    function ValidateCbb(element) {
        if (!CheckSelected(element)) {
            element.parent().find('button').addClass('border-error');
            return false;
        } else {
            element.parent().find('button').removeClass('border-error');
            return true;
        }
    }

    function ValidationFilter() {
        let result = true;

        if (!ValidateCbb(domElements.cbbMatchScore)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbDanger)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbCurrency)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbReason)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbAGroup)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbMGroup)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbHDPBetTeam)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbTicketsStatus)) {
            result = false;
        }

        if (!((domElements.inputAmountFrom.val() == '' && domElements.inputAmountTo.val() == '') || GetInputAmount())) {
            common.showError("Please input a valid range (Ex: 500 - 1000).");
            result = false;
        }

        return result;
    }    
    
    function GetDangerSelectDataFilter(dangerType) {
        let dangerValues = [];
        domElements.cbbDanger.find('optgroup').each(function () {
            let optGroup = $(this);
            let currentDangerType = optGroup.attr("DangerType");

            if (currentDangerType == dangerType) {
                optGroup.find('option:selected').each(function (idx, opItem) {
                    dangerValues.push($(opItem).attr("DangerValue"));
                })
            }
        });

        return dangerValues;
    }

    //#endregion Get Filter Params Selected

    //#region Render Filter
   
    function RenderFilter(data, isRefreshcbbSite) {
        recordsTotalGlobal = data.RecordsTotal;
        isSingleGrid = data.IsSingleGrid;     

        SetupFilterFromDictinary(domElements.cbbCurrency, data.CurrencyList);

        if (data.IsSingleGrid) {
            SetupFilter(domElements.cbbHDPBetTeam, data.BetTeamList);           
        } else {
            SetupFilter(domElements.cbbHDPBetTeam, data.HDPList);
        }

        SetupFilterScore(domElements.cbbMatchScore, data.ScoreList);

        SetupFilter(domElements.cbbReason, data.ReasonList);

        SetupAGroupFilter(data.GroupList);

        RefreshReasonInfo(data.ReasonList);

        SetupDefaultMGroupFilter(data.MGroupList);

        if (data.DropdownList) {
            SetupDanger(data.DropdownList.DangerList);
            SetupTicketStatusFilter(data.DropdownList.TicketStatusList);

            if (data.RecordsTotal > 0 && isRefreshcbbSite) {
                SetupSubscriberTypeFilter(data.DropdownList.SubscriberTypeList);
            }                                         
        }

        if (data.DropdownFilterList) {
            cacheFilterDependOn.DropdownFilterList = data.DropdownFilterList;

            FilterDependOnSelected(IndexFilterSelected.Score);
        }

        cacheMGroup.allMGroup = {};
        cacheMGroup.selectMgroup = {};       
    }

    //#endregion Render

    //#region Cache Filter

    function CacheSelectDataFilter() {
        if (currentType == MatchMonitorDetailViewType.TicketFraud && IsFilterOptionServerSide()) {
            var selectedFilter = cacheFilter[currentType];
            if (selectedFilter == undefined) {
                selectedFilter = {};
            }

            selectedFilter.cbbCurrency = GetSelectDataFilter(domElements.cbbCurrency);
            selectedFilter.cbbHDPBetTeam = GetSelectDataFilter(domElements.cbbHDPBetTeam);

            cacheFilterScore = GetSelectDataFilter(domElements.cbbMatchScore);

            selectedFilter.cbbReason = GetSelectDataFilter(domElements.cbbReason);
            selectedFilter.cbbAGroup = GetSelectDataFilter(domElements.cbbAGroup);
            selectedFilter.inputAmount = GetInputAmount();

            cacheFilter[currentType] = selectedFilter;
        }
        else {
            cacheFilter = {};
        }
    }   

    function CheckChangeFilterByAssociationDetection() { 
        let mGrList = cacheFilterDependOn.MGrList.map(obj => ({ ...obj }));

        if (mGrList.length > 0) {
            // check blank in M.Gr
            let blankItems = $.grep(mGrList, function (item) {
                return item.MDetectGroupID == 0;
            });

            if (blankItems.length != 0) {
                RefreshMGr(mGrList);
                return;
            }

            let selectedFilterCache = cacheFilter[associationDetection];

            if (selectedFilterCache != undefined) {
                let selectedFilter = GetForCacheByAssociationDetection();

                let equal = common.CompareObjects(selectedFilterCache, selectedFilter);

                if (equal) {
                    let newMGrList = $.grep(mGrList, function (item) {
                        return item.MDetectGroupID != 0;
                    });
                    RefreshMGr(newMGrList);
                } else {
                    mGrList.unshift({ "MDetectGroupID": 0, "MDetectGroup": "-", "Checked": true });
                    RefreshMGr(mGrList);
                }
            }
        }        
    }

    function RefreshMGr(items) {
        domElements.cbbMGroup.multiselect().empty();

        for (var i = 0; i < items.length; i++) {
            domElements.cbbMGroup.append(`<option selected value='${items[i].MDetectGroupID}'>${items[i].MDetectGroup}</option>`);                    
        }

        domElements.cbbMGroup.multiselect("refresh");
    }

    function ReUnshiftBlankMGr(items) {
        let newMGrList = $.grep(items, function (item) {
            return item.MDetectGroupID != 0;
        });

        newMGrList.unshift({ "MDetectGroupID": 0, "MDetectGroup": "-", "Checked": false });

        return newMGrList;
    }

    function GetForCacheByAssociationDetection() {
        let selectedFilter = {};
        selectedFilter.cbbMatchScore = GetSelectDataFilter(domElements.cbbMatchScore, true);
        selectedFilter.cbbDanger = GetSelectDataFilter(domElements.cbbDanger, true);
        selectedFilter.cbbSites = domElements.cbbSites.val();
        selectedFilter.cbbCurrency = GetSelectDataFilter(domElements.cbbCurrency, true);
        selectedFilter.cbbReason = GetSelectDataFilter(domElements.cbbReason, true);
        selectedFilter.cbbAGroup = GetSelectDataFilter(domElements.cbbAGroup, true);

        return selectedFilter;
    }

    function SetCacheByAssociationDetection() {
        let selectedFilter = GetForCacheByAssociationDetection();

        if (selectedFilter != undefined) {
            cacheFilter[associationDetection] = selectedFilter;
        }
    }   

    function IsFilterOptionServerSide() {
        if (GetSelectDataFilter(domElements.cbbCurrency))
            return true;

        if (GetSelectDataFilter(domElements.cbbHDPBetTeam))
            return true;

        if (GetSelectDataFilter(domElements.cbbMatchScore))
            return true;

        if (GetSelectDataFilter(domElements.cbbReason))
            return true;

        if (GetSelectDataFilter(domElements.cbbAGroup))
            return true;

        if (GetInputAmount() != undefined) {
            return true;
        }

        return false;
    }
    //#endregion Cache Filter

    //#region Fetch Match Monitor Detail

    function FetchMatchMonitorDetail(typeOfTickets = MatchMonitorDetailViewType.TicketFraud, lastScannedSequenceID = maxSequenceId) {
        recordsFilteredGlobal = 0;

        RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsTotalGlobal);

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

            domElements.breadcrumbTotalTickets.html('');

            timeoutRenderTicketDetail = setTimeout(() => {
                let data = {
                    EventDateToString: domElements.eventDataToString,
                    MatchID: domElements.matchID,
                    BetTypeID: GetBetTypeSelected(),
                    BetID: GetBetIDSelected(),
                    LiveIndicator: domElements.cbbMarket.val(),
                    LastScannedSequenceID: lastScannedSequenceID,
                    IsSupportShowAllTickets: isSupportShowAllTickets,
                    IsFilter: true,
                    TypeOfTickets: typeOfTickets,
                    ScoreList: domElements.sportType == 1 ? GetSelectDataFilter(domElements.cbbMatchScore, true) : undefined,
                    Sites: domElements.cbbSites.val(),
                    IsDangerNone: GetDangerSelectDataFilter(0).length > 0,
                    Danger1List: GetDangerSelectDataFilter(1),
                    Danger2List: GetDangerSelectDataFilter(2),
                    Danger3List: GetDangerSelectDataFilter(3),
                    Danger4List: GetDangerSelectDataFilter(4),
                    Danger5List: GetDangerSelectDataFilter(5),
                    CurrencyIdList: GetSelectDataFilter(domElements.cbbCurrency, true),
                    ReasonList: GetSelectDataFilter(domElements.cbbReason, true),
                    Reason: domElements.reason,
                    AGroupList: GetSelectDataFilter(domElements.cbbAGroup, true),
                    MGroupList: GetSelectDataFilter(domElements.cbbMGroup, true),
                    HDPList: GetHDPSelected(domElements.cbbHDPBetTeam, true),
                    BetTeamList: GetBetTeamSelected(domElements.cbbHDPBetTeam, true),
                    AmountList: GetInputAmount()?.map(str => {
                        return Number(str);
                    }) ?? undefined,
                    TicketStatusList: GetSelectDataFilter(domElements.cbbTicketsStatus, true)
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
                        RefreshBreadcrumbTotalTickets(0, recordsTotalGlobal);
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

    function FetchFilterMatchMonitorDetail(isRefreshcbbSite) {
        recordsTotalGlobal = 0;

        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        GetFilterMatchMonitorDetail(isRefreshcbbSite);
    }

    function GetFilterMatchMonitorDetail(isRefreshcbbSite) {
        var paramFilter = {
            EventDateToString: domElements.eventDataToString,
            MatchID: domElements.matchID,
            BetTypeID: GetBetTypeSelected(),
            BetID: GetBetIDSelected(),
            Sites: domElements.cbbSites.val(),
            Reason: domElements.reason,
            LiveIndicator: domElements.cbbMarket.val(),
            TypeOfTickets: currentType,
            IsRefresh: isRefresh,
            IsSupportShowAllTickets: isSupportShowAllTickets
        };

        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorFilter'), {
            async: false,
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            data: paramFilter,
            success: function (response) {
                const data = response?.data;
                if (data) {
                    RenderFilter(data, isRefreshcbbSite);

                    if (data.MaxSequenceId > 0 && currentType == MatchMonitorDetailViewType.TicketAll) {
                        maxSequenceId = data.MaxSequenceId;
                    }
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

    //#endregion Fetch Match Monitor Detail

    //#region Render Grid
    function RenderMatchMonitorDetail(response, typeOfTickets = MatchMonitorDetailViewType.TicketFraud) {
        let data = response?.data;

        if (data) {
            if (typeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
                recordsTotalGlobal = recordsFilteredGlobal = response.recordsFiltered;
            }
            if (typeOfTickets == MatchMonitorDetailViewType.TicketAll) {
                recordsFilteredGlobal += response?.recordsFiltered ?? 0;
                // recordsTotalGlobal = recordsFilteredGlobal;
                //if (recordsFilteredGlobal > recordsTotalGlobal) {
                //    recordsTotalGlobal = recordsFilteredGlobal;
                //}
            }

            if ($('option', domElements.cbbMGroup).length > 1) {
                FilterUIGridByMGr(data?.HomeMatchItemList, data?.AwayMatchItemList);
            }
            else {
                if (data?.HomeMatchItemList) {
                    domElements.tblMMHomeTeam.DataTable().rows.add(data.HomeMatchItemList).draw(false);
                }

                if (data?.AwayMatchItemList) {
                    domElements.tblMMAwayTeam.DataTable().rows.add(data.AwayMatchItemList).draw(false);
                }

                RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsTotalGlobal);        
            }
        }
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

    //#endregion Render Grid

    //#region Refresh
    function RefreshLengthPage(length, isDraw = true) {
        domElements.tblMMHomeTeam.DataTable().page.len(length).draw(isDraw);
        domElements.tblMMAwayTeam.DataTable().page.len(length).draw(isDraw);
    }

    function RefreshFilterSelected(element, data) {
        if (element && data) {
            element.val(data);
            element.multiselect("refresh");
        }
    }

    function RefreshReasonInfo(reasonList) {
        let data = reasonList;

        if (data?.length > 0) {
            let filteredReasons = data.filter(item => !item.includes('-'));
            domElements.spanReasonInfo.text(filteredReasons.sort().join()); return;
        }
        domElements.spanReasonInfo.text('');
    }

    function RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsTotalGlobal) {
        domElements.breadcrumbTotalTickets.html(`Total Tickets: ${ConvertNumberToThousandsString(recordsFilteredGlobal)}/${ConvertNumberToThousandsString(recordsTotalGlobal)}`);
    }

    //#endregion Refresh

    //#region Verify Trans
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

    function VerifyTransMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/VerifyTransMMDetail'), {
            type: 'POST',
            data:
            {
                MatchId: domElements.matchID,
                LiveIndicator: domElements.cbbMarket.val(),
                BettypeID: GetBetTypeSelected(),
                BetID: GetBetIDSelected(),
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

    //#endregion Verify Trans

    //#region Detect Association
    function DetectAssociation() {
        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        cacheMGroup.selectMgroup = {};
        cacheFilterDependOn.MGrList = [];
        cacheMGroup.allMGroup = {};
        cacheMGroup.selectMgroup = {};
        cacheMGroup.itemMMAwayTeam = [];
        cacheMGroup.itemMMHomeTeam = [];

        SetCacheByAssociationDetection();

        let params = {           
            RawCustIDs: domElements.hdSelectedCustIDs.val(),
            RawUserNames: domElements.hdSelectedUsernames.val(),
            IsOptBP: domElements.inputOptBP.is(":checked"),
            IsOptDevice: domElements.inputOptDevice.is(":checked"),
            IsOptIP: domElements.inputOptIP.is(":checked"),
            IsOptAgent: domElements.inputOptAgent.is(":checked"),
            IsOpt3MatchesLast7Days: domElements.inputOpt3MatchesLast7Days.is(":checked"),
            IsOptIPLast3Days: domElements.inputOptIPLast3Days.is(":checked")           
        }

        xhr = $.ajax(common.modifyRelativeUrl('MatchMonitor/DetectGroupAssWithOptions'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.tblMMHomeTeam.find('tbody').addClass('loading');
                domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            },
            success: function (response) {
                let itemList = response?.data?.ItemList;
                if (response?.data?.ItemList) {
                    cacheMGroup.isActive = true;

                    cacheFilterDependOn.MGrList = response.mGrList.MGrItems;

                    SetupMGroupFilter(response.mGrList.MGrItems);
                    
                    BindChangeGroupAssWithOptions(domElements.tblMMHomeTeam.DataTable(), itemList);
                    BindChangeGroupAssWithOptions(domElements.tblMMAwayTeam.DataTable(), itemList);

                    cacheMGroup.itemMMHomeTeam = domElements.tblMMHomeTeam.DataTable().rows().data().filter(function (x) { return cacheMGroup.selectMgroup[x.BetID]; });
                    cacheMGroup.itemMMAwayTeam = domElements.tblMMAwayTeam.DataTable().rows().data().filter(function (x) { return cacheMGroup.selectMgroup[x.BetID]; });

                    let totalItem = cacheMGroup.itemMMHomeTeam.length + cacheMGroup.itemMMAwayTeam.length;

                    RefreshBreadcrumbTotalTickets(totalItem, recordsTotalGlobal);
                }
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

    function ShowPopupAssDetection() {
        let ctsCustIDs = [];
        let custIDs = [];
        let userNames = [];
        
        domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {              
                custIDs.push(data.CustID);
                userNames.push(data.Username);
            }
        });
        domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
            if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {             
                custIDs.push(data.CustID);
                userNames.push(data.Username);
            }
        });

        domElements.divPopupAssDetect.attr('data-total-ticket-detect', custIDs.length);

        ctsCustIDs = ctsCustIDs.filter(OnlyUnique);
        custIDs = custIDs.filter(OnlyUnique);
        userNames = userNames.filter(OnlyUnique);

        if (IsValidAssDetection(custIDs.length)) {                                   
            domElements.hdSelectedCustIDs.val(custIDs.join());
            domElements.hdSelectedUsernames.val(userNames.join());

            domElements.divPopupAssDetect.dialog({
                modal: true,
                width: '800px',
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

    function IsValidAssDetection(custSelectedLength) {
        if (custSelectedLength <= 1) {
            common.showError("Please select at least 2 Usernames to proceed!");
            return false;
        }

        if (custSelectedLength > 5000) {
            common.showError("Exceeded max 5000 usernames. Please try again with a smaller range.");
            return false;
        }

        return true;
    }

    function OnlyUnique(value, index, self) {
        return self.indexOf(value) == index;
    }

    function FilterAssociationDetection(item) {
        if (!(domElements.cbbTypeOfTickets.val() == MatchMonitorDetailViewType.TicketAll || domElements.cbbTypeOfTickets.val() == item.TypeOfTicket)) {
            return false;
        }

        if (!domElements.cbbMatchScore.val().find(x => x == "-" || x == item.Score)) {
            return false;
        }

        if (!(domElements.cbbSites.val() == SiteViewType.SiteAll || domElements.cbbSites.val() == item.SubscriberType)) {
            return false;
        }

        // Danger

        if (!domElements.cbbCurrency.val().find(x => x == item.CurrencyId)) {
            return false;
        }

        if (!domElements.cbbReason.val().find(x => x == "-" || item.ReasonList.indexOf(x) > -1)) {
            return false;
        }

        if (!domElements.cbbAGroup.val().find(x => x == item.ADetectGroupID)) {
            return false;
        }

        if (!domElements.cbbMGroup.val().find(x => x == item.MDetectGroupID)) {
            return false;
        }

        if (!domElements.cbbHDPBetTeam.val().find(x => x == Math.abs(item.HDP))) {
            return false;
        }

        let amountList = GetInputAmount()?.map(str => {
            return Number(str);
        }) ?? undefined;

        if (amountList && !(item.Stake >= amountList[0] && item.Stake <= amountList[1])) {
            return false;
        }

        if (!domElements.cbbTicketsStatus.val().find(x => x == item.TicketStatus)) {
            return false;
        }

        return true;
    }

    function BindChangeGroupAssWithOptions(table, itemList) {
        table.rows().every(function () {
            let rowData = this.data();
            if ($(this.node()).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                let newGroupData = itemList.find(function (x) { return x.CustID == rowData.CustID; });
                if (newGroupData != undefined && newGroupData.GroupID > 0) {
                    rowData.MDetectGroupID = newGroupData.GroupID;
                    rowData.MDetectGroup = newGroupData.GroupName;

                    cacheMGroup.allMGroup[rowData.BetID] = newGroupData;
                    cacheMGroup.selectMgroup[rowData.BetID] = true;
                } else $(this.node()).css('display', 'none');
            } else $(this.node()).css('display', 'none');

            this.invalidate();
        })
    }

    function FilterUIGridByMGr(dataHome, dataAway) {  
        let totalNewItem = 0;
       
        let arrMGr = domElements.cbbMGroup.val();
        let isExistedBlank = arrMGr.indexOf("0") != -1;

        let itemMMHomeTeamFilter = dataHome.filter(function (item) {
            let assDetectionItem = cacheMGroup.allMGroup[item.BetID];
            if (assDetectionItem) {
                return arrMGr.indexOf(assDetectionItem.GroupName) != -1;       
            } else {
                if (isExistedBlank) {
                    return item.MDetectGroup == "-";
                } else {
                    return false;
                }               
            }
        });

        let itemMMAwayTeamFilter = dataAway.filter(function (item) {
            let assDetectionItem = cacheMGroup.allMGroup[item.BetID];
            if (assDetectionItem) {
                return arrMGr.indexOf(assDetectionItem.GroupName) != -1;
            } else {
                if (isExistedBlank) {
                    return item.MDetectGroup == "-";
                } else {
                    return false;
                }
            }
        });

        //domElements.tblMMHomeTeam.DataTable().clear().rows.add(itemMMHomeTeamFilter).draw(false);
        //domElements.tblMMAwayTeam.DataTable().clear().rows.add(itemMMAwayTeamFilter).draw(false);
        let totalOldItem = domElements.tblMMHomeTeam.DataTable().data().length + domElements.tblMMAwayTeam.DataTable().data().length;

        domElements.tblMMHomeTeam.DataTable().rows.add(itemMMHomeTeamFilter).draw(false);
        domElements.tblMMAwayTeam.DataTable().rows.add(itemMMAwayTeamFilter).draw(false);

        totalNewItem = totalOldItem + itemMMHomeTeamFilter.length + itemMMAwayTeamFilter.length;

        RefreshBreadcrumbTotalTickets(totalNewItem, recordsTotalGlobal);
    }

    function FilterUIGrid() {
        let totalItem = 0;

        let itemMMHomeTeamFilter = cacheMGroup.itemMMHomeTeam.filter(FilterAssociationDetection);
        let itemMMAwayTeamFilter = cacheMGroup.itemMMAwayTeam.filter(FilterAssociationDetection);

        domElements.tblMMHomeTeam.DataTable().clear().rows.add(itemMMHomeTeamFilter).draw();
        domElements.tblMMAwayTeam.DataTable().clear().rows.add(itemMMAwayTeamFilter).draw();
        totalItem = itemMMHomeTeamFilter.length + itemMMAwayTeamFilter.length;

        RefreshBreadcrumbTotalTickets(totalItem, totalItem);
    }

    function ShowAllDataGrid() {
        if (!ValidateTicketStatus()) {
            return;
        }
        if (ValidationFilter()) {
            CacheSelectDataFilter();
            domElements.btnVerifyAndClose.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

            if (!cacheMGroup.isActive) {
                FetchMatchMonitorDetail(currentType);
            }
            else {
                domElements.inputHomeSelectAll.prop('checked', false);
                domElements.inputAwaySelectAll.prop('checked', false);
            }

            //FilterUIGridByMGr();
        }
    }

    //#endregion Detect Association

    //#region Export To Excel
    function ExportToExcel() {
        if (IsValidExportExcel()) {
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
        }
    }

    function IsValidExportExcel() {
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

        if (homeCheckedL + awayCheckedL < 2) {
            common.showError("Please select at least 2 records to proceed!")
            return false;
        }
        if (homeCheckedL + awayCheckedL > 40000) {
            common.showError("Export fail due to size limitation, please contact Administrator for further support.")
            return false;
        }

        return true;
    }

    function GetDataMatchInfo() {
        let datas = '';
        datas += `{&quot;MatchName&quot;:&quot;${domElements.matchName}&quot;`;
        datas += `,&quot;EventDateStr&quot;:&quot;${domElements.eventDataToString}&quot;`;
        datas += `,&quot;BetTypeName&quot;:&quot;${domElements.betTypeName}&quot;}`;
        return datas;
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
                    datas += `,&quot;BetFrom&quot;:&quot;${data.BetFrom}&quot;`;
                    datas += `,&quot;ADetectGroup&quot;:&quot;${data.ADetectGroup}&quot;`;
                    datas += `,&quot;MDetectGroup&quot;:&quot;${data.MDetectGroup}&quot;`;
                    datas += `,&quot;ReasonList&quot;:&quot;${data.ReasonList}&quot;`;
                    datas += `,&quot;PICVerified&quot;:&quot;${data.PICVerified}&quot;},`;
                }
            }
        });

        return datas != '' ? datas.slice(0, -1) : datas;
    }

    //#endregion Export To Excel

    //#region Event OnChange

    function ChangeCbbTypeOfTickets() {
        RemoveAllFilterError();

        let betTypeSelected = $("option:selected", domElements.cbbBetType);

        if (betTypeSelected.attr('data-is-support-suspicious') == 'false') {
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketAll);
            domElements.cbbTypeOfTickets.prop('disabled', true);
        } else if (!isSupportShowAllTickets) {
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud);
            domElements.cbbTypeOfTickets.prop('disabled', true);
        } else {
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud);
            domElements.cbbTypeOfTickets.prop('disabled', false);
        }
    }

    function RestoreDefaulCbbSite() {
        domElements.cbbSites.prop('disabled', false);
        domElements.cbbSites.empty();
        domElements.cbbSites.append(`<option selected value='A'>All</option>`);
        domElements.cbbSites.append(`<option selected value='C'>Credit</option>`);
        domElements.cbbSites.append(`<option selected value='L'>Licensee</option>`);

        domElements.cbbSites.val(SiteViewType.SiteAll);
    }

    function MarketOnChange() {
        cacheFilterScore = undefined;

        RemoveAllFilterError();

        AddFilterLoading();

        RestoreDefaulCbbSite();

        let txtMarket = $("option:selected", domElements.cbbMarket).text();

        domElements.spanMarketInfo.text(txtMarket);

        ChangeCbbTypeOfTickets();

        currentType = Number(domElements.cbbTypeOfTickets.val());

        isRefresh = true;

        FetchFilterMatchMonitorDetail(true);

        RemoveFilterLoading();
    }

    function BetTypeOnChange() {
        cacheFilterScore = undefined;

        RemoveAllFilterError();

        AddFilterLoading();

        RestoreDefaulCbbSite();

        ChangeCbbTypeOfTickets();

        let txtBetType = $("option:selected", domElements.cbbBetType).text();

        domElements.spanSportBetTypeInfo.text(`${domElements.sportName} - ${txtBetType} -`);

        currentType = Number(domElements.cbbTypeOfTickets.val());

        isRefresh = true;

        FetchFilterMatchMonitorDetail(true);

        RemoveFilterLoading();
    }

    function TypeOfTicketsOnChange(isClear, isRefreshcbbSite) {
        AddFilterLoading();

        if (isClear || cacheFilterScore?.length <= 0) {
            cacheFilterScore = undefined;
        }

        domElements.cbbSites.prop('disabled', false);

        RestoreDefaulCbbSite();

        currentType = Number(domElements.cbbTypeOfTickets.val());

        cacheFilterDependOn.MGrList = [];

        FetchFilterMatchMonitorDetail(isRefreshcbbSite);        

        RemoveFilterLoading();
    }   

    function FilterDependOnSelected(indexFilterSelected) {        
        let scoresSelected = domElements.cbbMatchScore.val();
        let siteSelected = domElements.cbbSites.val();
        let isDangerNone = GetDangerSelectDataFilter(0).length > 0;
        let danger1Selected = GetDangerSelectDataFilter(1);
        let danger2Selected = GetDangerSelectDataFilter(2);
        let danger3Selected = GetDangerSelectDataFilter(3);
        let danger4Selected = GetDangerSelectDataFilter(4);
        let danger5Selected = GetDangerSelectDataFilter(5);    
        
        let currencySelected = domElements.cbbCurrency.val();
        let reasonSelected = domElements.cbbReason.val();

        let isNoDangerSelect = !isDangerNone && danger1Selected.length == 0 && danger2Selected.length == 0 &&
            danger3Selected.length == 0 && danger4Selected.length == 0 && danger5Selected.length == 0;
       
        if (cacheFilterDependOn.DropdownFilterList) {
            let dependOnFilter = cacheFilterDependOn.DropdownFilterList.filter(function (item) {
                if (scoresSelected.length == 0 || isNoDangerSelect) {
                    return false;
                }
                if (indexFilterSelected == IndexFilterSelected.Reason) {
                    if (!reasonSelected.find(x => x == "-" || item.ReasonItem.indexOf(x) > -1)) {
                        return false;
                    }

                    if (!currencySelected.find(x => x == item.CurrencyId)) {
                        return false;
                    }                    
                }

                if (indexFilterSelected == IndexFilterSelected.Currency) {
                    if (!currencySelected.find(x => x == item.CurrencyId)) {
                        return false;
                    }
                }
           
                let countDangerType = 0;
                if (isDangerNone && item.DangerToString == '-') {
                    countDangerType += 1;
                }
                if (danger1Selected.length > 0 && danger1Selected.find(x => x == item.Danger1)) {
                    countDangerType += 1;
                }
                if (danger2Selected.length > 0 && danger2Selected.find(x => x == item.Danger2)) {
                    countDangerType += 1;
                }
                if (danger3Selected.length > 0 && danger3Selected.find(x => x == item.Danger4)) {
                    countDangerType += 1;
                }
                if (danger4Selected.length > 0 && danger4Selected.find(x => x == item.Danger4)) {
                    countDangerType += 1;
                }
                if (danger5Selected.length > 0 && danger5Selected.find(x => x == item.Danger5)) {
                    countDangerType += 1;
                }
                if (countDangerType == 0) {
                    return false;
                }

                if (siteSelected != SiteViewType.SiteAll && siteSelected != item.SubscriberType) {
                    return false;
                }

                if (!scoresSelected.find(x => x == item.Score)) {
                    return false;
                }

                return true;
            });

            BindEffectFilterDependOn(indexFilterSelected, dependOnFilter);

            CheckChangeFilterByAssociationDetection();
        }
    }   

    function BindEffectFilterDependOn(indexFilterSelected, dependOnFilter) {
        let aGrItems = [];
        let reasonItems = [];
        let currencyItems = [];

        switch (indexFilterSelected) {
            case IndexFilterSelected.Reason:
                aGrItems = DistinctByAGr(dependOnFilter);
                RefreshAGr(aGrItems);
                break;
            case IndexFilterSelected.Currency:
                reasonItems = DistinctByReason(dependOnFilter);
                RefreshReason(reasonItems);

                aGrItems = DistinctByAGr(dependOnFilter);
                RefreshAGr(aGrItems);
                break;
            default:
                currencyItems = DistinctByCurrency(dependOnFilter);
                RefreshCurrency(currencyItems);

                reasonItems = DistinctByReason(dependOnFilter);
                RefreshReason(reasonItems);

                aGrItems = DistinctByAGr(dependOnFilter);
                RefreshAGr(aGrItems);
                break;
        }        
    }

    function DistinctByCurrency(dependOnFilter) {       
        let distinct = new Set();
        let newObj = dependOnFilter.filter(obj => {
            if (!distinct.has(obj.CurrencyId)) {
                distinct.add(obj.CurrencyId);
                return true;
            }
            return false;
        }).map(({ CurrencyId, CurrencyName }) => ({ CurrencyId, CurrencyName }));

        return newObj.sort();
    }

    function DistinctByReason(dependOnFilter) {
        let distinct = new Set();
        let reasonItems = [];
        dependOnFilter.filter(obj => {
            if (!distinct.has(obj.ReasonItem)) {
                distinct.add(obj.ReasonItem);
                return true;
            }
            return false;   
        }).map(function (reason) {
            if (reason.ReasonItem) {
                reason.ReasonItem.split(',').forEach(x => {
                    if (reasonItems.findIndex((obj) => obj == x) == -1)
                        reasonItems.push(x);
                })
            }
        });

        return reasonItems.sort();
    }

    function DistinctByAGr(dependOnFilter) {
        let distinct = new Set();
        let aGrItems = [];
        let isBlank = false;

        dependOnFilter.filter(obj => {
            if (!distinct.has(obj.ADetectGroupID)) {
                distinct.add(obj.ADetectGroupID);
                return true;
            }
            return false;
        }).map(function (aGr) {
            if (aGrItems.findIndex((obj) => obj == aGr.ADetectGroup) == -1) {
                if (aGr.ADetectGroup == "-") {
                    isBlank = true;
                } else {
                    aGrItems.push(parseInt(aGr.ADetectGroup));
                }     
            }                
        });     

        let aGrItemsInt = aGrItems.map(str => parseInt(str));

        aGrItemsInt.sort((a, b) => a - b);      

        if (isBlank) aGrItemsInt.unshift("-");
   
        return aGrItemsInt;
    }

    function RefreshCurrency(currencyItems) {
        domElements.cbbCurrency.multiselect().empty();
        if (currencyItems.length > 0) {
            currencyItems.forEach(item => {
                domElements.cbbCurrency.append(`<option selected value='${item.CurrencyId}'>${item.CurrencyName}</option>`);
            });
        }
        domElements.cbbCurrency.multiselect("refresh");
    }    

    function RefreshReason(reasonItems) {
        domElements.cbbReason.multiselect().empty();
        if (reasonItems.length > 0) {
            reasonItems.forEach(item => {
                domElements.cbbReason.append(`<option selected value='${item}'>${item}</option>`);
            });
        }
        domElements.cbbReason.multiselect("refresh");
    }

    function RefreshAGr(aGrItems) {        
        domElements.cbbAGroup.multiselect().empty();
        if (aGrItems.length > 0) {
            aGrItems.forEach(item => {
                domElements.cbbAGroup.append(`<option selected value='${item}'>${item}</option>`);
            });
        }
        domElements.cbbAGroup.multiselect("refresh");
    }

    function AddFilterLoading() {
        domElements.divFilterMonitor.addClass('loading-background');
        domElements.divFilterLoading.css('display', 'block');
    }

    function RemoveFilterLoading() {
        domElements.divFilterMonitor.removeClass('loading-background');
        domElements.divFilterLoading.css('display', 'none');
    }

    function RemoveAllFilterError() {
        domElements.cbbMatchScore.parent().find('button').removeClass('border-error');
        domElements.cbbDanger.parent().find('button').removeClass('border-error');
        domElements.cbbCurrency.parent().find('button').removeClass('border-error');
        domElements.cbbReason.parent().find('button').removeClass('border-error');
        domElements.cbbAGroup.parent().find('button').removeClass('border-error');
        domElements.cbbMGroup.parent().find('button').removeClass('border-error');
        domElements.cbbHDPBetTeam.parent().find('button').removeClass('border-error');
        domElements.cbbTicketsStatus.parent().find('button').removeClass('border-error');
    }

    //#endregion Event OnChange

    //#region Event Toggle
    function ToggleTickCheckAll(isCheckAll) {
        domElements.inputOptBP.prop("checked", isCheckAll);
        domElements.inputOptDevice.prop("checked", isCheckAll);
        domElements.inputOptIP.prop("checked", isCheckAll);
        domElements.inputOptAgent.prop("checked", isCheckAll);
        domElements.inputOpt3MatchesLast7Days.prop("checked", isCheckAll);
        domElements.inputOptIPLast3Days.prop("checked", isCheckAll);
    }

    function ToggleGridSelectAll(input, table) {
        var nodes = table.rows({ 'search': 'applied' }).nodes();
       
        $('input[type="checkbox"][name="chkbx"]:not(:hidden)', nodes).prop('checked', input.is(":checked"));
    }

    function ToggleBetID(tblElement, isExpand) {
        $('tr', tblElement).each(function (idx, tr) {
            let spanBetID = $(tr).find('span.span-betid').first();

            if (spanBetID != undefined) {
                let halfBetID = '...';
                let fullBetID = $(spanBetID[0]).attr('data-bet-id');

                if (fullBetID != undefined && fullBetID.length > 5) {
                    if (isExpand) {
                        spanBetID.text(fullBetID);
                    } else {
                        halfBetID = halfBetID + fullBetID.substring(fullBetID.length - 6);

                        spanBetID.text(halfBetID);
                    }
                }
            }
        });
    }

    //#endregion Event Toggle

    function Resize() {
        domElements.groupTable.width($(window).width() - 38);

        let detailpageHeight = domElements.detailpage.outerHeight();
        let breadcrumbButtonHeight = domElements.breadcrumbButton.outerHeight();

        let breadcrumbButtonTop = domElements.detailpage.offset().top + domElements.detailpage.outerHeight();
        let groupTableTop = breadcrumbButtonTop + domElements.breadcrumbButton.outerHeight();

        domElements.breadcrumbButton.css({ top: breadcrumbButtonTop });
        domElements.groupTable.css('top', groupTableTop);

        domElements.groupTable.height($(window).height() - detailpageHeight - breadcrumbButtonHeight - 77);
    }

    function Initial() {
        SetupMultiselectFilter(domElements.cbbDanger, "Danger");
        SetupMultiselectFilter(domElements.cbbCurrency, "Currency");
        SetupMultiselect(domElements.cbbHDPBetTeam, "HDP");
        SetupMultiselect(domElements.cbbMatchScore, "Score");
        SetupMultiselect(domElements.cbbAGroup, "A.Gr.");
        SetupMultiselect(domElements.cbbMGroup, "M.Gr.");
        SetupMultiselect(domElements.cbbReason, "Reason");
        SetupMultiselect(domElements.cbbTicketsStatus, "Status");

        domElements.cbbMarket.val(domElements.liveIndicator);
        domElements.cbbBetType.val(`${domElements.betTypeID}_${domElements.betIDToString}`);

        domElements.cbbMarket.change(function () { MarketOnChange(); });
        domElements.cbbBetType.change(function () { BetTypeOnChange(); });
        domElements.cbbMatchScore.change(function () { FilterDependOnSelected(IndexFilterSelected.Score); });
        domElements.cbbSites.change(function () { FilterDependOnSelected(IndexFilterSelected.Site); })
        domElements.cbbDanger.change(function () { FilterDependOnSelected(IndexFilterSelected.Danger); });
        domElements.cbbCurrency.change(function () { FilterDependOnSelected(IndexFilterSelected.Currency); });
        domElements.cbbReason.change(function () { FilterDependOnSelected(IndexFilterSelected.Reason); });
        domElements.cbbAGroup.change(function () {
            CheckChangeFilterByAssociationDetection();
        });

        try {
            isSupportShowAllTickets = JSON.parse(domElements?.isSupportShowAllTickets?.toLowerCase());
        }
        catch { }

        domElements.cbbTypeOfTickets.prop('disabled', !isSupportShowAllTickets);      

        if (isSupportShowAllTickets) {
            domElements.cbbTypeOfTickets.change(function () {
                cacheMGroup.isActive = false;
                cacheMGroup.selectMgroup = {};
                CacheSelectDataFilter();
                TypeOfTicketsOnChange(currentType == MatchMonitorDetailViewType.TicketAll, true);
            });
        }

        SetupDataTable();

        domElements.inputAmountFrom.on('input', function () {
            domElements.inputAmountFrom.val(domElements.inputAmountFrom.val().replace(regexInvalidAmount, ""));
        });

        domElements.inputAmountTo.on('input', function () {
            domElements.inputAmountTo.val(domElements.inputAmountTo.val().replace(regexInvalidAmount, ""));
        });

        domElements.btnRefresh.on('click', function () {
            isRefresh = true;
            domElements.inputAmountFrom.val('');
            domElements.inputAmountTo.val('');
            maxSequenceId = 1;
            cacheFilter = {};
            cacheMGroup.allMGroup = {};
            cacheMGroup.selectMgroup = {};
            cacheMGroup.isActive = false;
            cacheFilterDependOn.MGrList = [];

            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud);
            RestoreDefaulCbbSite();    

            TypeOfTicketsOnChange(true, true);
            FetchMatchMonitorDetail();
        })

        domElements.btnGenerateDetail.on('click', function () {
            if (!ValidateTicketStatus()) {
                return;
            }
            if (ValidationFilter()) {              
                domElements.btnVerifyAndClose.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

                FetchMatchMonitorDetail(currentType);
                //if (!cacheMGroup.isActive) {
                //    FetchMatchMonitorDetail(currentType);                   
                //}
                //else {
                //    domElements.inputHomeSelectAll.prop('checked', false);
                //    domElements.inputAwaySelectAll.prop('checked', false);
                //    //FilterUIGrid();
                //    FilterUIGridByMGr();
                //}
            }
        })

        domElements.tickCheckAll.on('click', function () { ToggleTickCheckAll(true); });

        domElements.tickUncheckAll.on('click', function () { ToggleTickCheckAll(false); });

        //domElements.btnShowAll.on('click', function () {
        //    if (cacheMGroup.isActive) {
        //        cacheMGroup.isActive = false;
        //        domElements.inputHomeSelectAll.prop('checked', false);
        //        domElements.inputAwaySelectAll.prop('checked', false);

        //        //domElements.btnGenerateDetail.click();
        //        ShowAllDataGrid();

        //        cacheMGroup.selectMgroup = {};
        //    }
        //})

        domElements.btnShowPopupAssDetection.on('click', function () { ShowPopupAssDetection(); })

        domElements.btnAssDetection.on('click', function () { DetectAssociation(); })

        domElements.btnExportExcel.on('click', function () { ExportToExcel(); })

        domElements.btnVerifyAndClose.on('click', function () {
            common.showConfirmation2('<div class="popup - content">Are you sure about this?<br>Once confirmed, all tickets\' review status will be updated to "Verified".</div>', function () {
                VerifyTransMatchMonitorDetail();
            });
        })

        domElements.inputHomeSelectAll.prop('checked', false);
        domElements.inputAwaySelectAll.prop('checked', false);

        Resize();
    }

    $(window).resize(function () {
        Resize();
    });

    $(document).ready(function () {
        Initial();

        domElements.btnRefresh.click();
    });
}

$(MatchMonitorDetail);