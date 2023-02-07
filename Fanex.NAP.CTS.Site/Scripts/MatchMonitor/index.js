function MatchMonitor() {
    let dataTable;
    let timeoutHolder = null;
    let isSkipTracking = false;

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

    function GetMatchMonitor(isSkip) {
        if (ValidateRangeDate()) {

            isSkipTracking = isSkip;
            dataTable ? dataTable.ajax.reload(null, true) : dataTable = GenerateGrid();
        }
    }

    function ValidateRangeDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");
        let toDateValueAllow = domElements.dpkFromDate.datepicker("getDate");
        toDateValueAllow.setDate(toDateValueAllow.getDate() + 6);

        if (toDateValue > toDateValueAllow) {
            common.showError("Invalid date range, please select not more than 7 days");
            return false;
        }

        return true;
    }

    function GenerateGrid() {       
        return domElements.tblMonitor.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('MatchMonitor/GetMatchMonitor'),
                data: {
                    DateType: function () { return domElements.rbtnScanDate.is(":checked") ? 1 : 2; },
                    FromDate: function () { return domElements.dpkFromDate.val(); },
                    ToDate: function () { return domElements.dpkToDate.val(); },
                    Sports: function () { return domElements.cbbSport.val().join(); },
                    Market: function () { return domElements.cbbMarket.val(); },
                    MatchSection: function () { return domElements.cbbMatchSection.val().join(); },
                    IsVerified: function () { return domElements.cbbIsVerified.val(); },
                    BetTypes: function () { return "[" + domElements.cbbBetType.val().join() + "]" },
                    Reasons: function () { return domElements.cbbReason.val().join(); },
                    Event: function () { return domElements.txtEvent.val().trim().toUpperCase(); },
                    isSkipTracking: function () { return isSkipTracking; }
                },
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
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8], className: "text-center" },
                { targets: [0], width: "7%" },
                { targets: [1], width: "19%" },
                { targets: [2], width: "26%" },
                { targets: [3], width: "6%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "7%" },
                { targets: [6], width: "11%" },
                { targets: [7], width: "4%" },
                { targets: [8], width: "11%" },
                { targets: [9], visible: false }
            ],
            columns: [
                { data: obj => obj.SportName },
                { data: obj => obj.LeagueName },
                {
                    data: obj => () => {
                        let paramTemplate = "EventDateToString={0}&MatchID={1}&HomeID={2}&AwayID={3}&BetTypeID={4}&BettypeName={5}&MatchSection={6}&Market={7}&Reason={8}&ReasonName={9}&LeagueID={10}&BetID={11}&SportType={12}&KOTime={13}";
                        let para = paramTemplate.format(obj.EventDateToString, obj.MatchID, obj.HomeID, obj.AwayID, obj.BettypeID, obj.BettypeName, obj.MatchSection, obj.Market, obj.Reason, obj.ReasonName, obj.LeagueID, obj.BetID, obj.SportType, obj.KOTime);
                        
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

                api.column(9, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group group group-site"><td colspan="9"> ' + data[i].MatchSectionName + '</td></tr>'
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

    function SetRestrictFromDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");

        domElements.dpkFromDate.datepicker("option", "maxDate", toDateValue);
    }

    function SetRestrictToDate() {
        let fromDateValue = domElements.dpkFromDate.datepicker("getDate");

        domElements.dpkToDate
            .datepicker("option", "minDate", fromDateValue)
            .datepicker("option", "maxDate", moment(fromDateValue).add(1, "year").toDate());
    }

    function RefreshChange($target) {
        domElements.eventTarget = $target
        var selectedVal = parseInt($target.getAttribute("value"));

        clearTimeout(timeoutHolder);
        StartCountingDown(domElements.btnRefresh, selectedVal, selectedVal);
    }

    function StartCountingDown($target, originalVal, val) {
        if (isNaN(originalVal)|| originalVal == -1) {
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

    function RefreshBetTypeFilter() {  
        domElements.cbbBetType.find('option').remove(); 
        
        let sportIDs = domElements.cbbSport.val().join()
        if (sportIDs != '') {
            $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMMFilterBetTypeBySportID'), {
                type: 'POST',
                data: {
                    sportIDs: domElements.cbbSport.val().join()
                },
                success: function (response) {
                    if (response != undefined) {
                        response.forEach(i => {
                            let value = RenderBetTypeHTML(i.BetTypeID, i.BetID);
                            domElements.cbbBetType.append(`<option value="${value}">${i.BetTypeName}</option>`);
                        })
                    }
                },
                complete: function () {
                    domElements.cbbBetType.find('option').prop('selected', true);
                    domElements.cbbBetType.multiselect("refresh");
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        } else {
            domElements.cbbBetType.multiselect("refresh");
        }                      
    }

    function RenderBetTypeHTML(betTypeID, betId) {
        return `{&quot;BettypeID&quot;:` + betTypeID + `, &quot;BetID&quot;:` + betId +`}`;
    }

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictToDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictFromDate);

        domElements.cbbSport.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sport"
        });

        domElements.cbbBetType.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        });

        domElements.cbbMatchSection.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Section"
        });

        domElements.cbbReason.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Reason"
        });        

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

        domElements.cbbSport.on('change', function () { RefreshBetTypeFilter(); });

        RefreshBetTypeFilter();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(MatchMonitor);