function MatchMonitorDetail() {

    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        betTypeID: $("#hdBetTypeID").val(),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbReasonGroup: $('#cbbReasonGroup'), 
        cbbLiveIndicator: $('#cbbLiveIndicator'),
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
                LiveIndicator: function () { return domElements.cbbLiveIndicator.val(); },
                ListScore: function () { return domElements.cbbMatchScore.val().join(); },
                ListReasonGroup: function () { return domElements.cbbReasonGroup.val().join(); }
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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" },
                { targets: [0], width: "6%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "6%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "6%" },
                { targets: [6], width: "6%" },
                { targets: [7], width: "6%" },
                { targets: [8], width: "10%" },
                { targets: [9], width: "6%" },
                { targets: [10], width: "15%" },
                { targets: [11], width: "15%" },

            ],
            columns: [
                { data: obj => obj.SubscriberType },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return html = `<a class='click-view-name ' href=${url}> ${obj.Username} </a>`;
                    }
                },
                { data: obj => obj.DangerToString },
                { data: obj => obj.Odds },
                { data: obj => obj.BetID },
                { data: obj => obj.Currency },
                { data: obj => obj.LiveIndicator == true ? "Live" : "-" },
                { data: obj => obj.Amount },
                { data: obj => obj.HDP > 0 ? "+" + obj.HDP : obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTime },
                { data: obj => obj.ReasonGroup }
            ],
            createdRow: function (row, data, index) {
                if (data.TicketStatus)
                    $(row.cells).addClass('void-ticket');
                else
                    $(row.cells).addClass('rejected-ticket');
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
                { targets: [0], width: "6%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "6%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "6%" },
                { targets: [6], width: "6%" },
                { targets: [7], width: "6%" },
                { targets: [8], width: "10%" },
                { targets: [9], width: "6%" },
                { targets: [10], width: "15%" },
                { targets: [11], width: "15%" },

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
                { data: obj => obj.HDP > 0 ? "+" + obj.HDP : obj.HDP },
                { data: obj => obj.Score },
                { data: obj => obj.TransTime },
                { data: obj => obj.ReasonGroup }
            ],
            createdRow: function (row, data, index) {
                if (data.TicketStatus)
                    $(row.cells).addClass('void-ticket');
                else
                    $(row.cells).addClass('rejected-ticket');
            }
        });
    }

    function Initial() {
        domElements.cbbMatchScore.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Match Score"
        });

        domElements.cbbReasonGroup.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Reason Group"

        });

        InitialFirstDetailGird();
        InitialSecondDetailGird();

        domElements.btnGenerateDetail.on('click', function () {
            GetMatchMonitorDetail();
        })
    }

    $(document).ready(function () {
        Initial();
    });
}

$(MatchMonitorDetail);