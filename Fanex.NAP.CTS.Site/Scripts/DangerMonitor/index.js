function DangerousSetting() {
    let dateEmptyString = "------,----";
    let dataTable;
    let sportBetType = [];
    let bettypeList = {};
    let dangerListSelected = {
        Danger1: null,
        Danger2: null,
        Danger3: null,
        Danger4: null,
        Danger5: null
    }

    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        userId: $("#hdCTSCustId").val(),
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbIsVerified: $('#cbbIsVerified'),
        cbbBetType: $('#cbbBetType'),
        cbbDgr: $('#cbbDgr'),
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
        rbtnEventDate: $('#rbtnEventDate'),
        iconClear: $('#iconClear')
    };

    let timeoutRenderTicketDetail = undefined;
    let xhr;

    function FetchSports() {
        if (sportBetType) {
            domElements.cbbSport.empty();

            let sportList = {};
            sportBetType.map(x => sportList[x.SportType] = ({ Name: x.SportName, IsSelected: x.SportType == 1 }));

            Object.keys(sportList).forEach(index => {
                domElements.cbbSport.append(`<option ${sportList[index].IsSelected ? 'selected' : ''} value="${index}">${sportList[index].Name}</option>`);
            });

            domElements.cbbSport.multiselect("refresh");
        }

        FetchBetTypes();
    }

    function FetchBetTypes() {
        if (sportBetType) {
            domElements.cbbBetType.empty();

            let sportSelectedList = domElements.cbbSport.val();
            bettypeList = {};

            sportBetType.filter(x => sportSelectedList.includes(x.SportType.toString()))
                .map(x =>
                    bettypeList[`${x.BetTypeID}_${x.BetID}`] =
                    ({
                        "BetTypeName": x.BetTypeName,
                        "BetType": x.BetTypeID,
                        "BetChoiceType": x.BetChoiceType,
                        "BetChoiceHome": x.BetChoiceHome,
                        "BetChoiceAway": x.BetChoiceAway,
                        "BetID": x.BetID
                    }));

            Object.keys(bettypeList).forEach(value => {
                domElements.cbbBetType.append(`<option selected value="${value}">${bettypeList[value].BetTypeName}</option>`);
            })

            domElements.cbbBetType.multiselect("refresh");
        }
    }

    function GetSportBetTypeList() {
        $.ajax(common.modifyRelativeUrl('DangerMonitor/GetSportBettype'), {
            async: false,
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response?.data) {
                    sportBetType = [...response.data];

                    sportBetType.sort(function (a, b) {
                        return a.PriorityOrder - b.PriorityOrder;
                    })
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

    function GetDgrList() {
        let data = {
            Danger1: {
                DangerItems: []
            },
            Danger2: {
                DangerItems: []
            },
            Danger3: {
                DangerItems: []
            },
            Danger4: {
                DangerItems: []
            },
            Danger5: {
                DangerItems: []
            }
        }

        for (let i = 1; i <= 50; i++) {
            data.Danger3.DangerItems.push({ 'Danger': i, 'DangerName': 'Other Sports ' + i });
            data.Danger2.DangerItems.push({ 'Danger': i, 'DangerName': 'ABI ' + i });
            data.Danger1.DangerItems.push({ 'Danger': i, 'DangerName': 'Original ' + i });
            data.Danger4.DangerItems.push({ 'Danger': i, 'DangerName': 'Basketball ' + i });
            data.Danger5.DangerItems.push({ 'Danger': i, 'DangerName': 'E-Sports ' + i });
        }

        let dangerSelector = domElements.cbbDgr.empty();

        if (data.Danger3.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(3, "Other Sports", data.Danger3.DangerItems));

        if (data.Danger2.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(2, "ABI", data.Danger2.DangerItems));

        if (data.Danger1.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(1, "Original", data.Danger1.DangerItems));

        if (data.Danger4.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(4, "Basketball", data.Danger4.DangerItems));

        if (data.Danger5.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(5, "E-Sports", data.Danger5.DangerItems));

        domElements.cbbDgr.multiselect('refresh');
    }

    function BuildOptgroup(optgroupValue, optgroupName, items) {
        let optgroup = `<optgroup label="${optgroupName}" DangerType=${optgroupValue}>`;
        items.map(function (x) {
            optgroup += `<option selected value=${optgroupValue}-${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
        })

        optgroup += "</optgroup>";

        return optgroup;
    }

    function GetDangerMonitor(lastMatchID = null, lastEventDate = null, lastMatchSection = null, isSkipTracking = true) {
        timeoutRenderTicketDetail = setTimeout(() => {
            let data = {
                FromDate: domElements.dpkFromDate.val(),
                ToDate: !domElements.dpkToDate.val().includes(dateEmptyString) ? domElements.dpkToDate.val() : null,
                DangerList: JSON.stringify(GetDangerList()),
                Sports: domElements.cbbSport.val().join(),
                Event: domElements.txtEvent.val().trim().toUpperCase(),
                Market: domElements.cbbMarket.val() == "" ? "0,1" : domElements.cbbMarket.val(),
                MatchSection: domElements.cbbMatchSection.val().join(),
                BetTypes: JSON.stringify(domElements.cbbBetType.val().map(x => ({
                    "BetType": bettypeList[x].BetType,
                    "BetChoiceType": bettypeList[x].BetChoiceType,
                    "BetChoiceHome": bettypeList[x].BetChoiceType == 2 ? bettypeList[x].BetChoiceHome : undefined,
                    "BetChoiceAway": bettypeList[x].BetChoiceType == 2 ? bettypeList[x].BetChoiceAway : undefined,
                    "BetID": bettypeList[x].BetID
                }))),
                isSkipTracking: isSkipTracking,
                LastMatchID: lastMatchID,
                LastMatchSection: lastMatchSection ?? domElements.cbbMatchSection.val()[0],
                LastEventDate: lastEventDate
            };

            xhr = $.ajax(common.modifyRelativeUrl('DangerMonitor/GetDangerMonitor'), {
                type: 'POST',
                tryCount: 0,
                retryLimit: 3,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response?.data) {
                        domElements.tblMonitor.DataTable().rows.add(response?.data?.Items).draw(false);
                    }

                    if (response?.data?.LastMatchSection) {
                        GetDangerMonitor(response?.data?.LastMatchID, response?.data?.LastEventDate, response?.data?.LastMatchSection, false);
                        if (response?.data?.Items.length > 0) {
                            domElements.tblMonitor.find('tbody').removeClass('loading hide');
                        }
                    }
                    else {
                        domElements.tblMonitor.find('tbody').removeClass('loading hide');
                        $('.loading').addClass('hide');
                    }
                },
                error: function (xhr, textStatus, error) {
                    domElements.tblMonitor.find('tbody').removeClass('loading');
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
        }, 10);
    }

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
                minDate: '-7D'
            })
            .datepicker("setDate", moment().toDate());

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
                minDate: '-7D'
            })
            .datepicker("setDate", new Date());

        domElements.cbbDgr.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: `Select Dgr`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });

        domElements.cbbSport.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # select",
            noneSelectedText: `Select Sports`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });

        domElements.cbbBetType.multiselect({
            menuWidth: 340,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        }).multiselectfilter({
            width: 260,
            placeholder: '',
            label: ''
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
        });

        domElements.dpkToDate.on('change', function () {
            domElements.iconClear.removeClass('hide');
        });

        domElements.iconClear.on('click', function () {
            domElements.dpkToDate.val(dateEmptyString);

            domElements.iconClear.addClass('hide');
        });

        domElements.cbbSport.on('change', function () {
            FetchBetTypes();
        });

        domElements.iconClear.click();
        GetSportBetTypeList();
        GetDgrList();

        FetchSports();

        domElements.btnGenerate.on('click', function () {
            if (ValidateRangeDate()) {
                dangerListSelected.Danger1 = GetDangerListById(1);
                dangerListSelected.Danger2 = GetDangerListById(2);
                dangerListSelected.Danger3 = GetDangerListById(3);
                dangerListSelected.Danger4 = GetDangerListById(4);
                dangerListSelected.Danger5 = GetDangerListById(5);

                if (!dataTable) {
                    dataTable = GenerateGrid();
                    $('.wrapper-info-table').append("<div class=\"loading hide\"></div>");
                }

                if (timeoutRenderTicketDetail || xhr) {
                    clearTimeout(timeoutRenderTicketDetail);
                    xhr.onreadystatechange = null;
                    xhr.abort();
                }

                domElements.tblMonitor.DataTable().clear().draw();

                domElements.tblMonitor.find('tbody').addClass('loading');
                $('.loading').removeClass('hide');

                GetDangerMonitor();
            }
        });
    }

    function GenerateGrid() {
        return domElements.tblMonitor.DataTable({
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
                // info: "<div class=\"loading\"></div> Total Item: _TOTAL_ item(s)",
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [1000, 1500, 2000],
            pageLength: 1000,
            scrollX: false,
            scrollCollapse: true,
            // paging: false,
            scrollY: `${$(window).height() - 285}px`,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [6, 7, 8, 9], className: "text-right" },
                { targets: [0], width: "4%" },
                { targets: [1], width: "16%" },
                { targets: [2], width: "20%" },
                { targets: [3], width: "4%" },
                { targets: [4], width: "6%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "6%" },
                { targets: [7], width: "6%" },
                { targets: [8], width: "4%" },
                { targets: [9], width: "4%" },
                { targets: [10], visible: false }
            ],
            columns: [
                { data: obj => obj.SportName },
                { data: obj => obj.LeagueName },
                {
                    data: obj => () => {
                        let paramTemplate = "EventDateToString={0}&MatchID={1}&HomeID={2}&AwayID={3}&BetTypeID={4}&BettypeName={5}&MatchSection={6}&Market={7}&LeagueID={8}&BetID={9}&SportType={10}&KOTime={11}&Danger1={12}&Danger2={13}&Danger3={14}&Danger4={15}&Danger5={16}";
                        let para = paramTemplate.format(
                            obj.EventDateToString,
                            obj.MatchID,
                            obj.HomeID,
                            obj.AwayID,
                            obj.BettypeID,
                            obj.BettypeName,
                            obj.MatchSection,
                            obj.Market,
                            obj.LeagueID,
                            obj.BetID,
                            obj.SportType,
                            obj.KOTime,
                            dangerListSelected.Danger1.join(),
                            dangerListSelected.Danger2.join(),
                            dangerListSelected.Danger3.join(),
                            dangerListSelected.Danger4.join(),
                            dangerListSelected.Danger5.join());

                        return '<a class="click-view-details" href="DangerMonitor/Detail?' + para + '" target="_blank">' + obj.MatchName + '</a>';
                    }
                },
                { data: obj => obj.MarketName },
                { data: obj => obj.EventDateToString },
                { data: obj => obj.BettypeName },
                { data: obj => ConvertNumberToThousandsString(Math.round(obj.TotalAmtHome)) },
                { data: obj => obj.BetChoiceType == 2 ? ConvertNumberToThousandsString(Math.round(obj.TotalAmtAway)) : '-' },
                { data: obj => ConvertNumberToThousandsString(obj.TotalTicketHome) },
                { data: obj => obj.BetChoiceType == 2 ? ConvertNumberToThousandsString(obj.TotalTicketAway) : '-' },
                { data: obj => obj.MatchSectionName },
            ],
            drawCallback: function () {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(10, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group group group-site"><td colspan="13"> ' + data[i].MatchSectionName + '</td></tr>'
                        );

                        last = group;
                    }
                });
            },
            rowCallback: function (row, data) {
                if (data.BetChoiceType != 2) {
                    $('td:eq(7)', row).addClass("betchoice-1");
                    $('td:eq(9)', row).addClass("betchoice-1");
                }
            }
        });
    }

    function GetDangerList() {
        return domElements.cbbDgr.val().map((x) => {
            let key = x.split('-')[0];
            let val = Number(x.split('-')[1]);
            return { [`Danger${key}`]: val };
        });
    }

    function GetDangerListById(id) {
        return domElements.cbbDgr.val().filter(x => x.split('-')[0] == id)
            .map((x) => Number(x.split('-')[1]));
    }

    function ValidateRangeDate() {
        let isValid = true;
        let fromDateValue = domElements.dpkFromDate.val();
        let toDateValue = !domElements.dpkToDate.val().includes(dateEmptyString) ? domElements.dpkToDate.val() : null;

        if (toDateValue != null && moment(fromDateValue) > moment(toDateValue)) {
            isValid = false;
        }

        if (!isValid) {
            common.showError("Invalid date range");
        }

        return isValid;
    }

    $(document).ready(function () {
        Initial();
    });

    $(window).resize(function () {
        $('.dataTables_scrollBody').css('max-height', ($(window).height() - 340));
    });
}

$(DangerousSetting);

function ConvertNumberToThousandsString(x) {
    return x ? x.toLocaleString() : 0;
}