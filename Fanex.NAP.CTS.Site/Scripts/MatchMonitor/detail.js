function MatchMonitorDetail() {
    let domElements = {
        detailpage: $('#detailpage'),
        groupTable: $('#groupTable'),
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),       
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        betTypeID: $("#hdBetTypeID").val(),
        betIDToString: $("#hdBetIDToString").val(),
        reason: $("#hdreason").val(),
        liveIndicator: $("#hdLiveIndicator").val(),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbReason: $('#cbbReason'),
        cbbGroup: $('#cbbGroup'),
        cbbSites: $('#cbbSites'),
        cbbBetChoice: $('#cbbBetChoice'),
        tblMMHomeTeam: $('#tblMMHomeTeam'),
        tblMMAwayTeam: $('#tblMMAwayTeam'),
        spanReason: $('#spanReason'),
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
        btnRefresh: $('#btnRefresh'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        btnShowAll: $('#btnShowAll'),
        btnAssDetection: $('#btnAssDetection'),
        btnExportExcel: $('#btnExportExcel'),
        btnVerifyAndClose: $('#btnVerifyAndClose')
    };

    let filterOptions = {
        isShowAll: true,
        isAssDetect: false,
        isFilterScore: false,
        isFilterReason: false,
        isFilterGroup: false
    }

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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 15], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "25%" },
                { targets: [2], width: "5%" },
                { targets: [3], width: "5%" },
                { targets: [4], width: "5%" },
                { targets: [5], width: "5%" },
                { targets: [6], width: "5%" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "5%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "7%", className: "text-center col-transtime" },
                { targets: [12], width: "5%" },
                { targets: [13], width: "5%", className: "text-center col-reason" },
                { targets: [14], width: "5%" },
                { targets: [15], visible: false }
            ],
            columns: [
                {
                    data: obj => () => {
                        return '<label class="container-checkbox"><input class="chk-selected" type="checkbox"><span class="checkmark"></span></label>'; 
                    }
                },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='click-view-name ' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => obj.CustomerClassToString },
                { data: obj => RenderBotIcon(obj.RobotType, obj.IsRobot) },
                { data: obj => obj.DangerToString },               
                { data: obj => obj.Odds },
                { data: obj => obj.TransIdToString },
                { data: obj => obj.Currency },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTimeToString },
                {
                    data: obj => {
                        if (obj.GroupID == -1)
                            return `<span class="has-icon icon-loading-group"></span>`;                        
                        else return obj.Group;
                    }
                },
                { data: obj => obj.Reason },
                {
                    data: obj => () => {
                        if (obj.PICVerified) return "<span class='icon-Check'></span>";
                        return "";
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
                BetChoice: function () { return domElements.cbbBetChoice.val(); },
                Reason: function () { return domElements.reason; },
                LiveIndicator: function () { return domElements.liveIndicator; },
                ListScore: function () { return domElements.cbbMatchScore.val().join(); },
                ListGroup: function () { return "[" + domElements.cbbGroup.val().join() + "]"; }
            },
            beforeSend: function () {
                domElements.tblMMHomeTeam.find('tbody').addClass('loading');
                domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            },
            success: function (response) {
                domElements.tblMMHomeTeam.DataTable().clear().rows.add(response.data.HomeMatchItemList).draw();
                domElements.tblMMAwayTeam.DataTable().clear().rows.add(response.data.AwayMatchItemList).draw();

                RefreshFilter(domElements.cbbMatchScore, response.data.ScoreList);
                RefreshFilter(domElements.cbbReason, response.data.ReasonList);

                if (response.data.CTSCustIDList != '') {
                    $.post(common.modifyRelativeUrl('MatchMonitor/DetectGroupAssociation'), { ctsCustIDList: response.data.CTSCustIDList },
                        function (xResponse) {            
                            RenderAssociationGroup(domElements.tblMMHomeTeam, xResponse.ItemList);
                            RenderAssociationGroup(domElements.tblMMAwayTeam, xResponse.ItemList);

                            RefreshGroupFilter(xResponse.GroupList);                            
                        });
                }
            },
            complete: function (response) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');   
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                
                resizeDataTable();
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

    function RefreshGroupFilter(data) {
        domElements.cbbGroup.multiselect().empty();

        if (data.length == 0) {
            domElements.cbbGroup.append(`<option selected value='-1'>None</option>`);
        }
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
                rowData.Group = group.GroupName;
                rowData.GroupID = group.GroupID;
            } else {
                rowData.Group = '-';
                rowData.GroupID = 0;
            }

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
        let html = "";

        switch (robotType) {
            case -1:
                html += "<span class='icon-base-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            case 0:
                html += "<span class='icon-ob-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            case 1:
                html += "<span class='icon-good-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            case 2:
                html += "<span class='icon-bad-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            default:
                break;
        }

        if (isRobot) {
            html += "<span class='icon-big-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
        }

        return html === "" ? "-" : html;
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
                TranIds: GetTranIds().join(),
                Reason: domElements.reason
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
        var firstTableInfo = domElements.tblMMHomeTeam.DataTable().data().toArray();
        var tranIds = new Array();

        firstTableInfo.forEach(element =>
        {
            if (element.PICVerified == "-")
            {
                tranIds.push(element.TransIdToString);
            }
        });

        return tranIds;
    }

    function DetectAssociation() {
        filterOptions.isAssDetect = true;

        domElements.divPopupAssDetect.dialog('close');

        DrawGrid();

        filterOptions.isAssDetect = false;
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
                let arrScores = domElements.cbbMatchScore.val();
                let arrReasons = domElements.cbbReason.val();
                let arrGroupIDs = domElements.cbbGroup.val();

                if (arrGroupIDs.length == 0) return true;
                
                if (arrScores.find(e => e == rowData['Score']) == undefined) { isEqualScore = false; }

                if (arrReasons.find(e => e == rowData['Reason']) == undefined) { isEqualReason = false; }
                
                if (arrGroupIDs.find(e => e == rowData['GroupID']) == undefined) { isEqualGroupID = false; }

                if (isEqualScore && isEqualReason && isEqualGroupID) return true;               
                else return false;

                return true;
            }
        );
    }

    function resizeDataTable() {
        var widthTables = domElements.tblMMHomeTeam[0].offsetWidth + 37;

        //if (domElements.groupTable[0].offsetWidth - widthTables > 0 && widthTables < $(window).width()) {
        if (widthTables < $(window).width()) {
            domElements.detailpage.removeClass('page-content-monitor');
        }
        else{
            domElements.detailpage.addClass('page-content-monitor');
        }
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

        domElements.cbbMatchScore.change(function () { filterOptions.isFilterScore = true; })

        domElements.cbbReason.change(function () { filterOptions.isFilterReason = true; })

        domElements.cbbGroup.change(function () { filterOptions.isFilterGroup = true; })

        domElements.btnRefresh.on('click', function () { GetMatchMonitorDetail(); })

        domElements.btnGenerateDetail.on('click', function () { DrawGrid(); })

        domElements.tickCheckAll.on('click', function () {
            ToggleTickCheckAll(true);           
        });

        domElements.tickUncheckAll.on('click', function () {
            ToggleTickCheckAll(false);           
        });

        domElements.btnShowAll.on('click', function () {
            filterOptions.isShowAll = true;
            DrawGrid();
            filterOptions.isShowAll = false;
        })

        domElements.btnShowPopupAssDetection.on('click', function () {
            domElements.divPopupAssDetect.dialog({
                modal: true,
                width: '700px',
                title: 'ASSOCIATION DETECTION',
                titleIsHtml: false,
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            })
        })

        domElements.btnAssDetection.on('click', function () {
            DetectAssociation();
        })

        domElements.btnExportExcel.on('click', function () {

        })

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

        $(window).resize(function () {
            resizeDataTable();
        })
    });
}

$(MatchMonitorDetail);