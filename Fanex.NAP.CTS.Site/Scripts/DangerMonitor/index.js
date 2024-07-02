function DangerousSetting() {
    let dateEmptyString = "------,----";
    let dataTable;
    let sportBetType = [];
    let leagueGroup = [];
    let bettypeList = {};
    let dangerListSelected = {
        Danger1: null,
        Danger2: null,
        Danger3: null,
        Danger4: null,
        Danger5: null
    }

    let items = [];

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
        txtEventHeader: null,
        cbbMarket: $('#cbbMarket'),
        cbbMatchSection: $('#cbbMatchSection'),
        rbtnScanDate: $('#rbtnScanDate'),
        rbtnEventDate: $('#rbtnEventDate'),
        iconClear: $('#iconClear'),
        cbbLeagueGroup: $('#cbbLeagueGroup'),
        txtUsername: $('#txtUsername'),
        rbtnSelect: $('input[type=radio][name=radioSelect]'),
    };

    let timeoutRenderTicketDetail = undefined;
    let xhr;

    function FetchSports() {
        if (sportBetType) {
            let data = {
                MainSport: {
                    Items: {}
                },
                VirtualSport: {
                    Items: {}
                },
                OtherSport: {
                    Items: {}
                }
            }

            sportBetType.filter(x => x.SportGroup == 1).map(x => data.MainSport.Items[`${x.SportType}_${x.LeagueGroupID}`] = ({ Name: x.SportNameDisplay, IsSelected: x.SportType == 1 && x.LeagueGroupID == null, LeagueGroupID: x.LeagueGroupID, SportType: x.SportType }))

            sportBetType.filter(x => x.SportGroup == 2).map(x => data.VirtualSport.Items[`${x.SportType}_${x.LeagueGroupID}`] = ({ Name: x.SportNameDisplay, IsSelected: x.SportType == 1 && x.LeagueGroupID == null, LeagueGroupID: x.LeagueGroupID, SportType: x.SportType }))

            sportBetType.filter(x => x.SportGroup == 3).map(x => data.OtherSport.Items[`${x.SportType}_${x.LeagueGroupID}`] = ({ Name: x.SportNameDisplay, IsSelected: x.SportType == 1 && x.LeagueGroupID == null, LeagueGroupID: x.LeagueGroupID, SportType: x.SportType }))

            let sportGroupSelector = domElements.cbbSport.empty();

            if (data.MainSport.Items && Object.keys(data.MainSport.Items).length !== 0)
                sportGroupSelector.append(BuildOptgroupSport(1, "Main Sport", data.MainSport.Items));

            if (data.VirtualSport.Items && Object.keys(data.VirtualSport.Items).length !== 0)
                sportGroupSelector.append(BuildOptgroupSport(2, "Virtual Sport", data.VirtualSport.Items));

            if (data.OtherSport.Items && Object.keys(data.OtherSport.Items).length !== 0)
                sportGroupSelector.append(BuildOptgroupSport(3, "Other Sport", data.OtherSport.Items));

            domElements.cbbSport.val("1_null").change();
            // domElements.cbbSport.multiselect("refresh");
        }

        FetchLeagueGroups();
        FetchBetTypes();
    }

    function BuildOptgroupSport(optgroupValue, optgroupName, sportList) {
        let optgroup = `<optgroup label="${optgroupName}" SportType=${optgroupValue}>`;
        Object.keys(sportList).forEach(index => {
            optgroup += `<option ${sportList[index].IsSelected ? 'selected' : ''} value="${index}">${sportList[index].Name}</option>`;
        })

        optgroup += "</optgroup>";

        return optgroup;
    }

    function FetchLeagueGroups() {
        if (sportBetType) {
             domElements.cbbLeagueGroup.empty();

            let sportSelected = domElements.cbbSport.val().split('_')[0];
            var filterLeagueGroup = leagueGroup.filter(x => x.SportType == 0 || sportSelected == x.SportType);

            let firstItem = filterLeagueGroup.shift();

            filterLeagueGroup.forEach(item => {
                domElements.cbbLeagueGroup.append(`<option selected value="${item.GroupID}">${item.GroupName}</option>`);
            })

            // None option
            domElements.cbbLeagueGroup.append(`<option selected value="${firstItem.GroupID}">${firstItem.GroupName}</option>`);

            domElements.cbbLeagueGroup.multiselect("refresh");
        }
    }

    function FetchBetTypes() {
        if (sportBetType) {
            domElements.cbbBetType.empty();

            let sportSelectedList = domElements.cbbSport.val();
            bettypeList = {};

            sportBetType.filter(x => sportSelectedList.includes(`${x.SportType}_${x.LeagueGroupID}`))
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

    function GetLeagueGroupList() {
        $.ajax(common.modifyRelativeUrl('DangerMonitor/GetLeageGroup'), {
            async: false,
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response?.data) {
                    leagueGroup = [...response.data];

                    leagueGroup.sort(function (a, b) {
                        return a.GroupID - b.GroupID;
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

        dangerSelector.append('<option></option>');

        if (data.Danger3.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(3, "Other Sports", data.Danger3.DangerItems));

        if (data.Danger2.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(2, "ABI", data.Danger2.DangerItems));

        if (data.Danger1.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(1, "Original", data.Danger1.DangerItems));

        if (data.Danger4.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(4, "Basketball", data.Danger4.DangerItems));

        if (data.Danger5.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(5, "E-Sports", data.Danger5.DangerItems));
    }

    function BuildOptgroup(optgroupValue, optgroupName, items) {
        let optgroup = `<optgroup label="${optgroupName}" DangerType=${optgroupValue}>`;
        items.map(function (x) {
            optgroup += `<option value=${optgroupValue}_${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
        })

        optgroup += "</optgroup>";

        return optgroup;
    }

    function GetDangerMonitor(isSkipTracking = true) {
        timeoutRenderTicketDetail = setTimeout(() => {
            let data = {
                FromDate: domElements.dpkFromDate.val(),
                ToDate: !domElements.dpkToDate.val().includes(dateEmptyString) ? domElements.dpkToDate.val() : null,
                QueryDanger: domElements.cbbDgr.val(),
                SportList: domElements.cbbSport.val(),
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
                isSkipTracking: isSkipTracking
            };

            xhr = $.ajax(common.modifyRelativeUrl('DangerMonitor/GetDangerMonitor'), {
                type: 'POST',
                tryCount: 0,
                retryLimit: 3,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                beforeSend: function () {
                    // $('.loading').removeClass('hide');
                },
                success: function (response) {
                    if (response?.data?.Items) {
                        items = response?.data?.Items;

                        domElements.txtEventHeader ? SearchEventHeader() : domElements.tblMonitor.DataTable().rows.add(items).draw(false);
                    }

                    domElements.tblMonitor.find('tbody').removeClass('loading hide');
                    // $('.loading').addClass('hide');
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
            .datepicker("setDate", moment().add(-7, 'd').toDate());

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
                minDate: '-7D'
            })
            .datepicker("setDate", moment().toDate());

        domElements.cbbDgr.select2(
            {
                placeholder: "Select Dgr",
            });

        domElements.cbbSport.select2();
        // domElements.cbbSport.multiselect({
        //     menuWidth: 260,
        //     menuHeight: 260,
        //     selectedText: "# of # select",
        //     noneSelectedText: `Select Sports`,
        //     enableClickableOptGroups: true,
        //     includeSelectAllOption: true,
        //     groupsCollapsable: true,
        //     optGroupChecked: true
        // }).multiselectfilter({
        //     width: 150,
        //     placeholder: '',
        //     label: ''
        // });

        domElements.cbbBetType.multiselect({
            menuWidth: 250,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        }).multiselectfilter({
            width: 260,
            placeholder: '',
            label: ''
        });

        domElements.cbbMatchSection.multiselect({
            menuWidth: 250,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Section"
        });

        domElements.cbbLeagueGroup.multiselect({
            menuWidth: 250,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select League Group"
        }).multiselectfilter({
            width: 260,
            placeholder: '',
            label: ''
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
            FetchLeagueGroups();
            FetchBetTypes();
        });

        GetSportBetTypeList();
        GetLeagueGroupList();
        GetDgrList();

        FetchSports();

        domElements.rbtnSelect.on('change', function (e) {
            domElements.txtUsername.prop('disabled', e.currentTarget.value == 0);
            domElements.cbbDgr.prop('disabled', e.currentTarget.value == 1);

            // domElements.rbtnSelect.filter(":checked").val()
        })

        domElements.btnGenerate.on('click', function () {
            if (ValidateRangeDate()) {
                var dangerSelected = GetDangerListById();
                dangerListSelected.Danger1 = dangerSelected[1];
                dangerListSelected.Danger2 = dangerSelected[2];
                dangerListSelected.Danger3 = dangerSelected[3];
                dangerListSelected.Danger4 = dangerSelected[4];
                dangerListSelected.Danger5 = dangerSelected[5];

                if (!dataTable) {
                    dataTable = GenerateGrid();
                }

                if (timeoutRenderTicketDetail || xhr) {
                    clearTimeout(timeoutRenderTicketDetail);
                    xhr.onreadystatechange = null;
                    xhr.abort();
                }

                domElements.tblMonitor.DataTable().clear().draw();

                domElements.tblMonitor.find('tbody').addClass('loading');
                // $('.loading').removeClass('hide');

                GetDangerMonitor();
            }
        });
    }

    function GenerateGrid() {
        return domElements.tblMonitor.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip<"filter">>>t',
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
            // infoCallback: function (settings, start, end, max, total, pre) {
            //     if (!total) {
            //         return `<div class="display-total-ticket"></div> <div class="loading hide"></div>`
            //     }
            //     else {
            //         $('.display-total-ticket').html(`Displaying ${start} to ${end} of ${total} item(s)`);
            //     }
            // },
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
                        let paramTemplate = "EventDateToString={0}&MatchID={1}&HomeID={2}&AwayID={3}&BetTypeID={4}&BettypeName={5}&MatchSection={6}&Market={7}&LeagueID={8}&BetID={9}&SportType={10}&KOTime={11}";
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
                            obj.KOTime);

                        Object.keys(dangerListSelected).forEach(item => {
                            if (dangerListSelected[item]) {
                                para += `&${item}=${dangerListSelected[item]}`;
                            }
                        });

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
            },
            initComplete: function (settings, json) {
                if (!domElements.txtEventHeader) {
                    $('.filter').html(`<input type="text" placeholder="Enter League/Match" id="txtEventHeader">`);

                    domElements.txtEventHeader = document.querySelector('#txtEventHeader');

                    domElements.txtEventHeader.addEventListener('input', function () {
                        SearchEventHeader();
                    });
                }
            }
        });
    }

    function SearchEventHeader() {
        var value = domElements.txtEventHeader.value;

        var itemFilters = items.filter(x =>
            x.LeagueName.toUpperCase().includes(value.toUpperCase()) ||
            x.MatchName.toUpperCase().includes(value.toUpperCase()));

        domElements.tblMonitor.DataTable().clear().rows.add(itemFilters).draw(false);
    }

    function GetDangerListById() {
        var result = {};
        if (domElements.cbbDgr.val()) {
            var dangerString = domElements.cbbDgr.val().split('_');

            result[dangerString[0]] = Number(dangerString[1]);
        }


        return result;
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
        $('.dataTables_scrollBody').css('max-height', ($(window).height() - 285));
    });
}

$(DangerousSetting);

function ConvertNumberToThousandsString(x) {
    return x ? x.toLocaleString() : 0;
}