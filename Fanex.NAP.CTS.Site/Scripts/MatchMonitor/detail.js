function MatchMonitorDetail() {
    let domElements = {
        detailpage: $('#detailpage'),
        groupTable: $('#groupTable'),
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        isDisableGroup: $("#hdIsDisableGroup").attr("value"),
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        betTypeID: $("#hdBetTypeID").val(),
        betIDToString: $("#hdBetIDToString").val(),
        reason: $("#hdreason").val(),
        liveIndicator: $("#hdLiveIndicator").val(),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbGroup: $('#cbbGroup'),
        cbbSites: $('#cbbSites'),
        cbbBetChoice: $('#cbbBetChoice'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        btnVerify: $('#btnVerify'),
        firsttblMonitor: $('#first-tblMonitor'),
    };

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
                domElements.firsttblMonitor.find('tbody').addClass('loading');
            },
            success: function (response) {
                domElements.firsttblMonitor.DataTable().clear().rows.add(response.data).draw();

                if (domElements.reason == 58 && response.data.length > 0) {
                    let params = {};
                    params.custIDList = response.custIDList;
                    params.categoryIDList = "58,78,98,118,138,158";

                    $.post(common.modifyRelativeUrl('MatchMonitor/GetAssociationIrrigation'), params, function (xResponse) {
                        RenderAssociationIrrigation(xResponse);
                    })
                }
            },
            complete: function (response) {
                domElements.firsttblMonitor.find('tbody').removeClass('loading');     
                
                resizeDataTable();
            },
            error: function (error) {
                domElements.firsttblMonitor.find('tbody').removeClass('loading');
                console.log(error);
            }
        });
    }

    function RenderAssociationIrrigation(associations) {
        domElements.firsttblMonitor.DataTable().rows().every(function () {
            let rowData = this.data();
            
            rowData.AssociationIrrigation = GetTotalAssociationIrrigation(rowData.CustID, associations);
           
            this.invalidate();
        });

        domElements.firsttblMonitor.DataTable().draw();
       
        //domElements.firsttblMonitor.find('span.has-icon').removeClass('icon-loading');
    }

    function GetTotalAssociationIrrigation(custID, associations) {
        for (var i = 0; i < associations.length; i++) {
            if (associations[i].CustID == custID) {
                return associations[i].TotalAssociation;
            }
        }
        return 0;
    }

    function InitialDetailGird() {
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
            lengthMenu: [1000, 1500, 2000],
            pageLength: 1000,
            scrollX: false,
            scrollCollapse: false,
            autoWidth: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15], className: "text-center" },
                { targets: [0] },
                { targets: [1] },
                { targets: [2], visible: domElements.reason == 58 ? true : false },
                { targets: [3] },
                { targets: [4] },
                { targets: [5], visible: domElements.reason == 58 ? true : false },
                { targets: [6] },
                { targets: [7] },
                { targets: [8] },
                { targets: [9] },
                { targets: [10] },
                { targets: [11] }, 
                { targets: [12] },
                { targets: [13] },
                { targets: [14], visible: domElements.isDisableGroup == "True" ? false : true },
                { targets: [15] },
                { targets: [16], visible: false }
            ],
            columns: [
                { data: obj => obj.SubscriberType },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='click-view-name ' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => obj.CustomerClassToString},
                { data: obj => RenderBotIcon(obj.RobotType, obj.IsRobot) },
                { data: obj => obj.DangerToString },
                {
                    data: obj => {
                        if (obj.AssociationIrrigation == -1) {
                            return `<span class="has-icon icon-loading">${obj.AssociationIrrigation}</span>`;                        
                        } else {
                            return obj.AssociationIrrigation;
                        }
                    } },
                { data: obj => obj.Odds },
                { data: obj => obj.TransIdToString },
                { data: obj => obj.Currency },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP },
                { data: obj => obj.BetChoice },
                { data: obj => obj.Score },
                { data: obj => obj.TransTimeToString },
                { data: obj => obj.Group },
                { data: obj => obj.PICVerified },
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
                domElements.firsttblMonitor.addClass('loading-value');
            },
            success: function () {
                close();
            },
            complete: function () {
                domElements.firsttblMonitor.removeClass("loading-value");
            },
            error: function (errMsg) {
                console.log(errMsg);
                domElements.firsttblMonitor.removeClass("loading-value");
            }
        });
    }

    function GetTranIds() {
        var firstTableInfo = domElements.firsttblMonitor.DataTable().data().toArray();
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

    function resizeDataTable() {
        var widthTables = domElements.firsttblMonitor[0].offsetWidth + 37;

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
            noneSelectedText: "Select Match Score "
        });

        domElements.cbbGroup.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Group "
        });

        InitialDetailGird();

        domElements.btnGenerateDetail.on('click', function () {
            GetMatchMonitorDetail();
        })

        domElements.btnVerify.on('click', function () {
            common.showConfirmation2('<div class="popup - content">Are you sure about this?<br>Once confirmed, all tickets\' review status will be updated to "Verified".</div>', function () {
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