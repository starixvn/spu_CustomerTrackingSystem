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
        subHome: $('#subHome'),
        subAway: $('#subAway'),
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

    let isFilterOption = false;
    let MaxCustGroupCal = 5000;

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
                { targets: [16], visible: false }
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
                        if (obj.IsNormalLinkedPA || obj.IsRobot) {
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
                {
                    data: obj => {
                        if (obj.ADetectGroupID == -2)
                            return `<span class="has-icon icon-loading-group"></span>`;              
                        else return obj.ADetectGroup;
                    }
                },
                { data: obj => obj.MDetectGroup },
                {
                    data: obj => () => {
                        let reasonList = obj.ReasonList;
                        if (reasonList != '')
                            reasonList = reasonList.replace(",", ", ");
                        return reasonList;
                    }
                },
                {
                    data: obj => () => {
                        if (obj.PICVerified) return "<span class='icon-Check'></span>";
                        return "-";                       
                    }
                },
                { data: obj => obj.TransIdToString }
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

    function GetMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorDetail'), {
            type: 'POST',
            data:
            {
                EventDateToString: function () { return domElements.eventDataToString; },
                MatchID: function () { return domElements.matchID; },
                BetTypeID: function () { return domElements.betTypeID; },
                BetID: function () { return domElements.betIDToString; },
                Sites: function () { return domElements.cbbSites.val(); },              
                Reason: function () { return domElements.reason; },
                LiveIndicator: function () { return domElements.liveIndicator; }                       
            },
            beforeSend: function () {
                domElements.tblMMHomeTeam.find('tbody').addClass('loading');
                domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            },
            success: function (response) {
                RefreshFilter(domElements.cbbMatchScore, response.data.ScoreList);
                RefreshFilter(domElements.cbbReason, response.data.ReasonList);              

                domElements.tblMMHomeTeam.DataTable().clear().rows.add(response.data.HomeMatchItemList).draw();
                domElements.tblMMAwayTeam.DataTable().clear().rows.add(response.data.AwayMatchItemList).draw();

                domElements.spanReasonInfo.text(response.data.ReasonInfos);
                
                if (response.data.TotalCust <= MaxCustGroupCal) {
                    $.post(common.modifyRelativeUrl('MatchMonitor/DetectGroupAssDefault'), { ctsCustIDList: response.data.CTSCustIDList },
                        function (xResponse) {                            
                            RenderAssociationGroup(domElements.tblMMHomeTeam, xResponse.ItemList);
                            RenderAssociationGroup(domElements.tblMMAwayTeam, xResponse.ItemList);
                            RefreshGroupFilter(xResponse.GroupList, false);
                        });                   
                } else {
                    RenderNoneAssociationGroup(domElements.tblMMHomeTeam);
                    RenderNoneAssociationGroup(domElements.tblMMAwayTeam);
                    RefreshGroupFilter([], true);
                }
            },
            complete: function (response) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');   
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');                   
            },
            error: function (error) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                console.log(error);
            }
        });
    }

    function RefreshFilter(element, data) {
        element.multiselect().empty();
       
        for (var i = 0; i < data.length; i++) {
            element.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
        }

        element.multiselect("refresh");
    }

    function RefreshGroupFilter(data, isExceedCust) {
        domElements.cbbGroup.multiselect().empty();

        if (isExceedCust)
            domElements.cbbGroup.append(`<option selected value='-1'>-</option>`);
        else if (data.length == 0)
            domElements.cbbGroup.append(`<option selected value='-1'>None</option>`);
        
        for (var i = 0; i < data.length; i++) {
            domElements.cbbGroup.append(`<option selected value='${data[i].GroupID}'>${data[i].GroupName}</option>`);
        }

        domElements.cbbGroup.multiselect("refresh");
    }

    function RenderAssociationGroup(elements, associations) {
        elements.DataTable().rows().every(function () {
            let rowData = this.data();

            let group = GetAssociationGroup(rowData.CustID, associations);
            if (group != undefined) {
                rowData.ADetectGroup = group.GroupName;
                rowData.ADetectGroupID = group.GroupID;
            } else {
                rowData.ADetectGroup = 'None';
                rowData.ADetectGroupID = -1;
            }

            this.invalidate();
        });
        
        elements.DataTable().draw();        
    }

    function RenderNoneAssociationGroup(elements) {
        elements.DataTable().rows().every(function () {
            let rowData = this.data();

            rowData.ADetectGroup = '-';
            rowData.ADetectGroupID = -1;

            this.invalidate();
        });

        elements.DataTable().draw();
    }

    function GetAssociationGroup(custID, associations) {
        for (var i = 0; i < associations.length; i++) {
            if (associations[i].CustID == custID) {
                return associations[i];
            }
        }
        return undefined;
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

    function FilterGrid() {
        $.fn.dataTable.ext.search.push(
            function (settings, searchData, index, rowData, counter) {
                let isEqualScore = true;
                let isEqualReason = true;
                let isEqualGroupID = true;
                
                if (!isFilterOption) return true;

                let arrScores = domElements.cbbMatchScore.val();
                let arrReasons = domElements.cbbReason.val();
                let arrGroupIDs = domElements.cbbGroup.val();                
                
                if (arrScores.find(e => e == rowData['Score']) == undefined) { isEqualScore = false; }

                let reason = arrReasons.find(e => rowData['ReasonList'].includes(e));
                if (reason == false || reason == undefined) { isEqualReason = false; }
                
                if (arrGroupIDs.find(e => e == rowData['ADetectGroupID']) == undefined) { isEqualGroupID = false; }

                if (isEqualScore && isEqualReason && isEqualGroupID)               
                    return true;               
                else
                    return false;

                return true;
            }
        );
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

        InitialDetailGird(domElements.tblMMHomeTeam);

        InitialDetailGird(domElements.tblMMAwayTeam);
      
        domElements.btnRefresh.on('click', function () {
            isFilterOption = false;
            GetMatchMonitorDetail();
        })

        domElements.btnGenerateDetail.on('click', function () { isFilterOption = true; DrawGrid(); })

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

        FilterGrid();
    }

    $(document).ready(function () {
        Initial();        

        GetMatchMonitorDetail();               

        BindScrollHeaderFixed();
    });
}

$(MatchMonitorDetail);