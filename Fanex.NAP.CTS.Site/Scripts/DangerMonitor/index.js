function DangerousSetting() {
    let dataTable;
    let timeoutHolder = null;
    let isSkipTracking = false;
    let sportBetType = [];
    let bettypeList = {};

    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        userId: $("#hdCTSCustId").val(),
        checkboxFrom: $('#checkboxFrom'),
        checkboxTo: $('#checkboxTo'),
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
        rbtnEventDate: $('#rbtnEventDate')
    };

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
                    bettypeList[`${x.BetTypeId}_${x.BetID}`] =
                    ({
                        "BetTypeName": x.BetTypeName,
                        "BetType": x.BetTypeId,
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

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
                minDate: '-7D'
            })
            .datepicker("setDate", moment().add(-7, "days").toDate());

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
                minDate: '-7D'
            })
            .datepicker("setDate", new Date());

        domElements.checkboxFrom.on('change', function () {
            domElements.dpkFromDate.prop('disabled', !domElements.checkboxFrom.is(':checked'))

            if (!domElements.checkboxFrom.is(':checked')) {
                domElements.dpkFromDate.datepicker("setDate", moment().add(-7, "days").toDate());
            }
        })

        domElements.checkboxTo.on('change', function () {
            domElements.dpkToDate.prop('disabled', !domElements.checkboxTo.is(':checked'))

            if (domElements.checkboxTo.is(':checked')) {
                domElements.dpkToDate.datepicker("setDate", new Date());
            }
            else {
                domElements.dpkToDate.val('--- --,----');
            }
        })

        domElements.checkboxFrom.change();
        domElements.checkboxTo.change();

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
            selectedText: "# of # selected",
            noneSelectedText: "Select Sport"
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

        domElements.cbbSport.on('change', function () {
            FetchBetTypes();
        })

        GetSportBetTypeList();
        GetDgrList();

        FetchSports();

        domElements.btnGenerate.on('click', function () {
            let dangers = domElements.cbbDgr.val();

            let params = {
                FromDate: domElements.dpkFromDate.val(),
                ToDate: domElements.dpkToDate.val(),
                Danger1: GetDangerList(dangers, 1),
                Danger2: GetDangerList(dangers, 2),
                Danger3: GetDangerList(dangers, 3),
                Danger4: GetDangerList(dangers, 4),
                Danger5: GetDangerList(dangers, 5),
                Sports: domElements.cbbSport.val().join(),
                Event: domElements.txtEvent.val().trim().toUpperCase(),
                Market: domElements.cbbMarket.val(),
                MatchSection: domElements.cbbMatchSection.val().join(),
                BetTypes: JSON.stringify(domElements.cbbBetType.val().map(x => ({
                    "BetType": bettypeList[x].BetType,
                    "BetChoiceType": bettypeList[x].BetChoiceType,
                    "BetChoiceHome": bettypeList[x].BetChoiceType == 2 ? bettypeList[x].BetChoiceHome : undefined,
                    "BetChoiceAway": bettypeList[x].BetChoiceType == 2 ? bettypeList[x].BetChoiceAway : undefined,
                    "BetID": bettypeList[x].BetID
                }))),
            };

            console.log(params);
        });
    }

    function GetDangerList(list, groupNumber) {
        return list.filter(x => x.split('-')[0] == groupNumber).map(x => Number(x.split('-')[1]));
    }

    $(document).ready(function () {
        Initial();
    });

    $(window).resize(function () {
        $('.dataTables_scrollBody').css('max-height', ($(window).height() - 340));
    });
}

$(DangerousSetting);