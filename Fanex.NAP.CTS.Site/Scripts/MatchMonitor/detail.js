function MatchMonitorDetail() {
    let domElements = {
        detailpage: $('#detailpage'),
        groupTable: $('#groupTable'),
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        betTypeID: $("#hdBetTypeID").val(),
        isHedging: $("#hdIsHedging").val(),
        liveIndicator: $("#hdLiveIndicator").val(),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbGroup: $('#cbbGroup'),
        cbbSites: $('#cbbSites'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        btnVerify: $('#btnVerify'),
        firsttblMonitor: $('#first-tblMonitor'),
        secondtblMonitor: $('#second-tblMonitor')
    };

    function GetMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorDetail'), {
            type: 'POST',
            data:
            {
                EventDateToString: function () { return domElements.eventDataToString; },
                MatchID: function () { return domElements.matchID; },
                BetTypeID: function () { return domElements.betTypeID; },
                Sites: function () { return domElements.cbbSites.val(); },
                IsHedging: function () { return domElements.isHedging; },
                LiveIndicator: function () { return domElements.liveIndicator; },
                ListScore: function () { return domElements.cbbMatchScore.val().join(); },
                ListGroup: function () { return "[" + domElements.cbbGroup.val().join() + "]"; }
            },
            beforeSend: function () {
                domElements.firsttblMonitor.find('tbody').addClass('loading');
                domElements.secondtblMonitor.find('tbody').addClass('loading');
            },
            success: function (response) {
                domElements.firsttblMonitor.DataTable().clear().rows.add(response.homeTableData).draw();
                domElements.secondtblMonitor.DataTable().clear().rows.add(response.awayTableData).draw();
            },
            complete: function (response) {
                domElements.firsttblMonitor.find('tbody').removeClass('loading');
                domElements.secondtblMonitor.find('tbody').removeClass('loading');
                resizeDataTable();
            },
            error: function (error) {
                domElements.firsttblMonitor.find('tbody').removeClass('loading');
                domElements.secondtblMonitor.find('tbody').removeClass('loading');
                console.log(error);
            }
        });
    }

    function InitialFirstDetailGird() {
        domElements.firsttblMonitor.DataTable({
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
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            autoWidth: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13], className: "text-center" },
                { targets: [0] },
                { targets: [1] },
                { targets: [2] },
                { targets: [3] },
                { targets: [4] },
                { targets: [5] },
                { targets: [6] },
                { targets: [7] },
                { targets: [8] },
                { targets: [9] },
                { targets: [10] },
                { targets: [11] },
                { targets: [12] },
                { targets: [13], visible: false }
            ],
            columns: [
                { data: obj => obj.SubscriberType },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='click-view-name ' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => RenderBotIcon(obj.RobotType, obj.IsRobot) },
                { data: obj => obj.DangerToString },
                { data: obj => obj.Odds },
                { data: obj => obj.BetID },
                { data: obj => obj.Currency },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTimeToString },
                { data: obj => obj.Group },
                { data: obj => obj.PICVerified },
                { data: obj => obj.TransId }
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

    function InitialSecondDetailGird() {
        return domElements.secondtblMonitor.DataTable({
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
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            autoWidth: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13], className: "text-center" },
                { targets: [0] },
                { targets: [1] },
                { targets: [2] },
                { targets: [3] },
                { targets: [4] },
                { targets: [5] },
                { targets: [6] },
                { targets: [7] },
                { targets: [8] },
                { targets: [9] },
                { targets: [10] },
                { targets: [11] },
                { targets: [12] },
                { targets: [13], visible: false }
            ],
            columns: [
                { data: obj => obj.SubscriberType },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='click-view-name' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => RenderBotIcon(obj.RobotType, obj.IsRobot) },
                { data: obj => obj.DangerToString },
                { data: obj => obj.Odds },
                { data: obj => obj.BetID },
                { data: obj => obj.Currency },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTimeToString },
                { data: obj => obj.Group },
                { data: obj => obj.PICVerified },
                { data: obj => obj.TransId }
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
                TranIds: GetTranIds().join()
            },
            beforeSend: function () {
                domElements.firsttblMonitor.addClass('loading-value');
                domElements.secondtblMonitor.addClass('loading-value');
            },
            success: function () {
                close();
            },
            complete: function () {
                domElements.firsttblMonitor.removeClass("loading-value");
                domElements.secondtblMonitor.removeClass("loading-value");
            },
            error: function (errMsg) {
                console.log(errMsg);
                domElements.firsttblMonitor.removeClass("loading-value");
                domElements.secondtblMonitor.removeClass("loading-value");
            }
        });
    }

    function GetTranIds() {
        var firstTableInfo = domElements.firsttblMonitor.DataTable().data().toArray();
        var secondTableInfo = domElements.secondtblMonitor.DataTable().data().toArray();
        var tranIds = new Array();

        firstTableInfo.forEach(element =>
        {
            if (element.PICVerified == "-")
            {
                tranIds.push(element.TransId);
            }
        });

        secondTableInfo.forEach(element =>
        {
            if (element.PICVerified == "-")
            {
                tranIds.push(element.TransId);
            }
        });

        return tranIds;
    }

    function resizeDataTable() {
        var widthTables = domElements.firsttblMonitor[0].offsetWidth + domElements.secondtblMonitor[0].offsetWidth + 41;

        if (domElements.groupTable[0].offsetWidth - widthTables > 0 && widthTables < $(window).width()) {
            domElements.detailpage.removeClass('page-content-monitor');
            domElements.firsttblMonitor.removeClass('table-maxcontent');
            domElements.secondtblMonitor.removeClass('table-maxcontent');
        }
        else{
            domElements.detailpage.addClass('page-content-monitor');
            domElements.firsttblMonitor.addClass('table-maxcontent');
            domElements.secondtblMonitor.addClass('table-maxcontent');
        }
    }

    function Initial() {
        domElements.cbbMatchScore.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Match Score "
        });

        domElements.cbbGroup.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Group "
        });

        InitialFirstDetailGird();
        InitialSecondDetailGird();

        domElements.btnGenerateDetail.on('click', function () {
            GetMatchMonitorDetail();
        })

        domElements.btnVerify.on('click', function () {
            common.showConfirmation('<div class="popup - content">Are you sure about this?<br>Once confirmed, all tickets\' review status will be updated to "Verified".</div>', function () {
                VerifyTransMatchMonitorDetail();
            });
        })
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