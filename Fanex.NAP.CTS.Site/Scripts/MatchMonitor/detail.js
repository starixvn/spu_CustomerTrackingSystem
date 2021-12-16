function MatchMonitorDetail() {

    let domElements = {
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
                { targets: [13] },
            ],
            columns: [
                { data: obj => obj.SubscriberType },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='click-view-name ' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => RenderBotIcon(obj.IsRobotUser, obj.RobotBetting) }, //TODO
                { data: obj => obj.DangerToString },
                { data: obj => obj.Odds },
                { data: obj => obj.BetID },
                { data: obj => obj.Currency },
                { data: obj => obj.LiveIndicator ? "Live" : "None-Live" },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTime },
                { data: obj => obj.Group },
                { data: obj => obj.PICVerified }
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
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" },
                { targets: [0], width: "1%" },
                { targets: [1], width: "6%" },
                { targets: [2], width: "5%" },
                { targets: [3], width: "2%" },
                { targets: [4], width: "2%" },
                { targets: [5], width: "1%" },
                { targets: [6], width: "2%" },
                { targets: [7], width: "1%" },
                { targets: [8], width: "1%" },
                { targets: [9], width: "1%" },
                { targets: [10], width: "6%" },
                { targets: [11], width: "6%" },

            ],
            columns: [
                { data: obj => obj.SubscriberType },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return html = `<a class='click-view-name' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => obj.DangerToString },
                { data: obj => obj.Odds },
                { data: obj => obj.BetID },
                { data: obj => obj.Currency },
                { data: obj => obj.LiveIndicator == true ? "Live" : "None-Live" },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTime },
                { data: obj => obj.Group }
            ],
            createdRow: function (row, data, index) {
                if (data.TicketStatus == 102)
                    $(row.cells).addClass('void-ticket');
                else if (data.TicketStatus == 101)
                    $(row.cells).addClass('rejected-ticket');
            }
        });
    }

    //TODO
    function RenderBotIcon(IsRobotUser, robotBetting) {

        let html = "";

        switch (robotBetting) {
            case 1:
                html += "<span class='icon-base-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            case 2:
                html += "<span class='icon-good-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            case 3:
                html += "<span class='icon-ob-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            case 4:
                html += "<span class='icon-bad-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
                break;
            default:
                break;
        }

        if (IsRobotUser) {
            html += "<span class='icon-big-robot'><span class='path1'></span><span class='path2'></span><span class='path3'></span><span class='path4'></span><span class='path5'></span><span class='path6'></span></span>";
        }

        return html === "" ? "-" : html;
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
    }

    $(document).ready(function () {
        Initial();
        GetMatchMonitorDetail();
    });
}

$(MatchMonitorDetail);