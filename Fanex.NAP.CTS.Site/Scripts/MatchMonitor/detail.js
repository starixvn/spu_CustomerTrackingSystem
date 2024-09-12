const MatchMonitorDetailViewType = {
    TicketAll: 3,
    TicketFraud: 2
}

const SiteViewType = {
    SiteAll: "A",
    SiteCredit: "C",
    SiteLicensee: "L"
}

const IndexFilterSelected = {
    Score: 1,
    Site: 2,
    CC: 3,
    Danger: 4,
    Currency: 5,
    Reason: 6,
    AGr: 7
}

function ConvertNumberToThousandsString(x) {
    return x ? x.toLocaleString() : 0;
}

function MatchMonitorDetail() {

    //#region Variables
    let domElements = {
        detailpage: $('#detailpage'),
        divContent: $('#divContent'),
        divPageLoading: $('#divPageLoading'),
        groupControl: $('#groupControl'),
        divFilterMonitor: $('#divFilterMonitor'),
        divFilterLoading: $('#divFilterLoading'),
        breadcrumbButton: $('#breadcrumbButton'),
        breadcrumbTotalTickets: $(".breadcrumb-totaltickets"),
        hdbreadcrumTotalTickets: $("#hdbreadcrumTotalTickets"),
        groupTable: $('#groupTable'),
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        eventDataToString: $("#hdEventDataToString").val(),
        matchID: $("#hdMatchID").val(),
        matchName: $("#hdMatchName").val(),
        betTypeID: $("#hdBetTypeID").val(),
        betTypeName: $("#hdBetTypeName").val(),
        betIDToString: $("#hdBetIDToString").val(),
        sportType: $('#hdSportType').val(),
        sportName: $('#hdSportName').val(),
        reason: $("#hdreason").val(),
        liveIndicator: $("#hdLiveIndicator").val(),
        matchSection: $("#hdMatchSection").val(),
        isSupportShowAllTickets: $("#hdIsSupportShowAllTickets").val(),
        defaultReasonNames: $('#hdReasonNames').val(),
        mapX: $('#hdMapX').val(),
        mapYY: $('#hdMapYY').val(),
        ticketType: $("#hdTicketType").val(),
        cbbMarket: $('#cbbMarket'),
        cbbBetType: $('#cbbBetType'),
        cbbMapX: $('#cbbMapX'),
        cbbMapYY: $('#cbbMapYY'),
        divMapX: $('#divMapX'),
        divMapYY: $('#divMapYY'),
        cbbTypeOfTickets: $('#cbbTypeOfTickets'),
        cbbMatchScore: $('#cbbMatchScore'),
        cbbSites: $('#cbbSites'),
        cbbCC: $('#cbbCC'),
        cbbCCHidden: $('#cbbCCHidden'),
        cbbReasonGroupHidden: $('#cbbReasonGroupHidden'),
        cbbDanger: $('#cbbDanger'),
        cbbCurrency: $('#cbbCurrency'),
        cbbReason: $('#cbbReason'),
        cbbAGroup: $('#cbbAGroup'),
        cbbMGroup: $('#cbbMGroup'),
        cbbHDPBetTeam: $('#cbbHDPBetTeam'),
        inputCustAmountRM: $('#inputCustAmountRM'),
        iconClearUsnAmt: $('#iconClearUsnAmt'),
        cbbTicketsStatus: $('#cbbTicketsStatus'),
        cbbCashoutType: $('#cbbCashoutType'),
        tblMMHomeTeam: $('#tblMMHomeTeam'),
        tblMMAwayTeam: $('#tblMMAwayTeam'),
        tblMMSG: $('#tblMMSG'),
        divHomeMonitor: $('#first-monitor-datatable'),
        divAwayMonitor: $('#second-monitor-datatable'),
        divSGMonitor: $('#monitor-datatable'),
        spanSportBetTypeInfo: $('#spanSportBetTypeInfo'),
        spanMarketInfo: $('#spanMarketInfo'),
        spanReasonInfo: $('#spanReasonInfo'),
        inputHomeSelectAll: $('#inputHomeSelectAll'),
        inputAwaySelectAll: $('#inputAwaySelectAll'),
        inputSGSelectAll: $('#inputSGSelectAll'),
        iconHToggleBetID: $('#iconHToggleBetID'),
        iconAToggleBetID: $('#iconAToggleBetID'),
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
        hdSelectedCustIDs: $('#hdSelectedCustIDs'),
        hdSelectedUsernames: $('#hdSelectedUsernames'),
        btnImportNewTicket: $('#btnImportNewTicket'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        btnAssDetection: $('#btnAssDetection'),
        btnExportExcel: $('#btnExportExcel'),
        btnVoidTicket: $('#btnVoidTicket'),
        btnMarkPA: $('#btnMarkPA'),
        btnVerifyAndClose: $('#btnVerifyAndClose'),
        btnReasonFeedback: $('#btnReasonFeedback'),
        inputTranstimeFrom: $('#inputTranstimeFrom'),
        inputTranstimeTo: $('#inputTranstimeTo'),
        iconTranstimeSubmit: $('#iconTranstimeSubmit'),
        iconTranstimeClear: $('#iconTranstimeClear'),
        boderTranstimeInput: $('.info-input'),
        iconHTranstimeAutofill: $('#iconHTranstimeAutofill'),
        iconATranstimeAutofill: $('#iconATranstimeAutofill'),
        iconSGTranstimeAutofill: $('#iconSGTranstimeAutofill'),
        tggHoldKey: $('#tggHoldKey'),
        ttHoldKey: $('#ttHoldKey')
    };

    let transtime = {
        fromDate: undefined,
        toDate: undefined
    }

    let recordsFilteredGlobal = 0;
    let recordsTotalGlobal = 0;
    let maxSequenceId = 1;

    let cacheFilter = {};
    let cacheFilterScore = undefined;
    let cacheFilterForImportNew = {};
    let cacheFilterDependOn = {
        DropdownListFilterDependOn: [],
        MGrList: []
    };
    let cacheFilterDefault = {
        ReasonList: [],
        AGrList: []
    };
    let cacheMGroup = {
        allMGroup: {},
        selectMgroup: {},
        isActive: false
    };
    let cacheDefaultReason = null;

    let posClickGlobal = 0;
    let xhr;
    let currentType = MatchMonitorDetailViewType.TicketFraud;
    let associationDetection = "AssociationDetection";
    let isImportNewTicket = false;
    let isSupportShowAllTickets = false;
    let isDisplaySingleGrid = false;
    let timeoutRenderTicketDetail = undefined;

    const regexValidAmount = /^[0-9]{1,15}/;
    const regexInvalidAmount = /[^0-9]{1,15}/g;
    const regexValidTranstime = /^(\d{2}\/\d{2}\/\d{2})(\s)(\d{2}\:\d{2}\:\d{2})$/;
    //#endregion Variables

    //#region Initial Grid
    function InitialDetailGird(element) {
        element.DataTable({
            serverSide: false,
            // dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            dom: '',
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
            paging: true,
            bInfo: false,
            lengthMenu: [-1, 1000, 1500, 2000],
            pageLength: 1000,
            scrollX: false,
            autoWidth: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 5, 6, 8, 10, 13, 14, 15, 16, 17], className: "text-center" },
                {
                    targets: [0], width: "3%", render: function (data, type, full, meta) {
                        return '<label><input type="checkbox" class="col-checkbox" name="chkbx" /></label>';
                    } // Checkbox
                },
                { targets: [1], width: "17%", className: "col-username" }, // UserName
                { targets: [2], width: "4%", className: "col-customerclass" }, // CC
                { targets: [3], width: "4%", className: "col-bot" }, // Bot
                { targets: [4], width: "10%", className: "text-center col-dgr" }, // Dgr
                { targets: [5], width: "5%", className: "col-odds" }, // Odds
                { targets: [6], width: "5%", className: "col-myodds" }, // MY.Odds
                { targets: [7], width: "5%", className: "text-left col-betid" }, // BetID
                { targets: [8], width: "5%", className: "col-currency"}, // Cur
                { targets: [9], width: "5%", className: "text-right col-amt" }, // Amt
                { targets: [10], width: "4%", className: "col-hdp" }, // HDP
                { targets: [11], width: "5%", className: "text-center col-score" }, // Score
                { targets: [12], width: "13%", className: "text-center col-transtime" }, // Transtime
                { targets: [13], width: "3%" }, // B.f.
                { targets: [14], width: "5%" }, // A.Gr.
                { targets: [15], width: "5%", className: "text-center" }, // M.Gr.
                { targets: [16], width: "7%", className: "text-center col-reason" }, // Reason
                { targets: [17], width: "3%" }, // Icon Verify
                { targets: [18], visible: false },
                { targets: [19], visible: false }
            ],
            columns: [
                {
                    data: obj => () => {
                        return `<label><input type="checkbox" name="chkbx" class="col-checkbox" /></label>`;
                    }
                },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        let classColRed = '';
                        if (obj.IsPA || obj.IsNormalLinkedPA || obj.IsRobot) {
                            classColRed = 'col-red';
                        }
                        return `<a class='click-view-name ${classColRed}' href=${url}>${obj.Username}</a>`;
                    }
                },
                { data: obj => obj.CustomerClassToString },
                { data: obj => RenderBotIcon(obj.RobotType, obj.IsRobot) },
                { data: obj => obj.DangerToString },
                { data: obj => RenderOdds(obj) },
                { data: obj => RenderMalayOdds(obj) },
                {
                    data: obj => RenderBetInfo(obj),
                },
                { data: obj => RenderCurrency(obj) },
                {
                    data: obj => RenderAmount(obj), createdCell: (cell, _, row) => {
                        if (row.Stake >= 1000) {
                            $(cell).addClass('col-red')
                        }
                    }
                },
                { data: obj => RenderHDPChoice(obj) },
                { data: obj => RenderScore(obj) },
                { data: obj => obj.TransTimeToString },
                { data: obj => obj.BetFrom },
                { data: obj => obj.ADetectGroup },
                {
                    data: obj => () => {
                        const assDetectionItem = cacheMGroup.allMGroup[obj.BetID];
                        if (assDetectionItem) {
                            obj.MDetectGroupID = assDetectionItem.GroupID;
                            obj.MDetectGroup = assDetectionItem.GroupName;
                        }

                        return obj.MDetectGroup;
                    }
                },
                {
                    data: obj => () => {
                        let reasonList = obj.ReasonList;
                        if (reasonList && reasonList != '')
                            return reasonList.replace(",", ", ");
                        return "-";
                    }
                },
                {
                    data: obj => () => {
                        if (obj.PICVerified) return "<span class='icon-Check'></span>";
                        return "-";
                    }
                },
                { data: obj => obj.TransIdToString },
                { data: obj => obj.ADetectGroupID }
            ],
            createdRow: function (row, data, index) {
                if (data.TicketStatus == 102)
                    $(row.cells).addClass('void-ticket');
                else if (data.TicketStatus == 101)
                    $(row.cells).addClass('rejected-ticket');

                if (data.SubscriberType === "L") {
                    $(row.cells[1]).addClass('username-hightlight');
                }
               
                if (data.ViewMoreInfo) {
                    if (data.IsParlay) {                           
                        let myArr = [17, 16, 15, 14, 13, 12, 4, 3, 2, 1, 0];
                        myArr.forEach(function (value, _) {
                            $(row.cells[value]).remove();
                        });

                        if (data.ParlayTicket != undefined && data.ParlayTicket.IsMainTicket) {
                            let myArr = [0, 1, 2, 3, 4, 5, 6];
                            myArr.forEach(function (value, _) {
                                $(row.cells[value]).addClass('bg-parlay-main');
                            })
                        }
                                              
                    } else {    
                        let myArr = [0, 1, 2, 3, 4, 12, 13, 14, 15, 16, 17];
                        myArr.forEach(function (value, _) {
                            $(row.cells[value]).attr('rowspan', data.Combinition + 1);
                        });

                    }                  
                } 
            }            
        });

        OnclickBetInfo(element);
    }

    function OnclickBetInfo(element) {
        $('tbody', element).on('click', 'td.col-betid', function () {
          
            let rowIndex = element.DataTable().row(this).index(); 

            let row = element.DataTable().row(this);

            let data = row.data();  
            
            if (data.ViewMoreInfo) {
                data.ViewMoreInfo = false;
                CollapseParlayTicket(rowIndex);
            } else {
                data.ViewMoreInfo = true;
                ExpandParlayTicket(data.ParlayTickets, rowIndex); 
            }
        })
    }

    function CollapseParlayTicket(rowIndex) {
        let table = domElements.tblMMHomeTeam.DataTable();
             
        let rowWithRowspan = $(table.row(rowIndex).node());
       
        let rowspanCell = rowWithRowspan.find('td[rowspan]');        

        if (rowspanCell.length) {
            let currentRowspan = parseInt(rowspanCell.attr('rowspan'), 10);
                      
            while (currentRowspan > 1) {
                rowspanCell.attr('rowspan', currentRowspan - 1);

                // Find and remove the next row (the one that the rowspan affects)
                let nextRow = rowWithRowspan.next('tr');

                table.row(nextRow).remove().draw(false);

                currentRowspan = currentRowspan - 1;
            }
        }
    }

    function ExpandParlayTicket(parlayTickets, rowIndex) {       
        if (parlayTickets.length > 0) {
            let table = domElements.tblMMHomeTeam.DataTable();
            let data = table.rows().data().toArray();            

            parlayTickets.forEach(function (value, _) {
                let newRowData = { Combinition: value.Combinition, ParlayTicket: value, ViewMoreInfo: true, IsParlay: value.IsParlay };

                // Insert new row data at the clicked row's index + 1
                data.splice(rowIndex + 1, 0, newRowData);
            })

            // Clear the table and re-add all rows with the new row included
            table.clear().rows.add(data).draw();
        }
    }

    //#endregion Initial Grid

    //#region Setup Filter & Grid
    function SetupMultiselect(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`
        });
    }

    function SetupMultiselectFilter(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });
    }

    function SetupMultiselectGroup(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        });
    }

    function SetupFilterGroupReason(element, data) {
        element.multiselect().empty();
        let arrReason = [];

        if (cacheDefaultReason != null) {
            arrReason = cacheDefaultReason.split(',');
        }

        if (data) {
            for (var i = 0; i < data.length; i++) {
                let selected = true;
                if (arrReason.length > 0) {
                    if (arrReason.indexOf(data[i]) == -1) {
                        selected = false;
                    }
                }
              
                let optgroup = `<optgroup label='Detect By v.v...'>`;
                if (selected) {
                    optgroup += `<option selected value='${data[i]}'>${data[i]}</option>`;
                } else {
                    optgroup += `<option value='${data[i]}'>${data[i]}</option>`;
                }
                optgroup += `</optgroup>`;

                element.append(optgroup);
            }

            element.parent().find('button').removeClass('border-error');
        }

        element.multiselect("refresh");
    }

    function SetupFilter(element, data) {
        element.multiselect().empty();

        if (data) {
            for (var i = 0; i < data.length; i++) {
                element.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
            }

            element.parent().find('button').removeClass('border-error');
        }

        element.multiselect("refresh");
    }

    function SetupFilterReason(element, data) {
        element.multiselect().empty();
        let arrReason = [];

        if (cacheDefaultReason != null) {
            arrReason = cacheDefaultReason.split(',');
        }

        if (data) {
            for (var i = 0; i < data.length; i++) {
                let selected = true;
                if (arrReason.length > 0) {
                    if (arrReason.indexOf(data[i]) == -1) {
                        selected = false;
                    }
                }
                if (selected) {
                    element.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
                } else {
                    element.append(`<option value='${data[i]}'>${data[i]}</option>`);
                }
            }

            element.parent().find('button').removeClass('border-error');
        }

        element.multiselect("refresh");
    }

    function SetupFilterScore(element, data) {
        if (cacheFilterScore != undefined) {
            data = [...new Set([...data, ...cacheFilterScore])];

            SetupFilter(element, data);

            setTimeout(RefreshFilterSelected(element, cacheFilterScore), 10);

        } else {
            let cacheSelected = cacheFilterForImportNew[currentType];

            if (cacheSelected != undefined && cacheSelected.cbbMatchScore != undefined && cacheSelected.cbbMatchScore.length > 0) {
                data = [...new Set([...data, ...cacheSelected.cbbMatchScore])];
            }

            SetupFilter(element, data);

            if (cacheSelected != undefined && cacheSelected.cbbMatchScore != undefined && cacheSelected.cbbMatchScore.length > 0) {
                setTimeout(RefreshFilterSelected(element, cacheSelected.cbbMatchScore), 10);
            }
        }
    }

    function SetupFilterFromDictinary(element, data) {
        element.multiselect().empty();

        if (data) {
            data.forEach(item => {
                element.append(`<option selected value='${item.Key}'>${item.Value}</option>`);
            });

            element.parent().find('button').removeClass('border-error');
        }

        element.multiselect("refresh");
    }

    function SetupAGroupFilter(data) {
        domElements.cbbAGroup.multiselect().empty();

        if (data) {
            for (var i = 0; i < data.length; i++) {
                domElements.cbbAGroup.append(`<option selected value='${data[i].GroupID}'>${data[i].GroupName}</option>`);
            }

            domElements.cbbAGroup.parent().find('button').removeClass('border-error');
        }

        domElements.cbbAGroup.multiselect("refresh");
    }

    function SetupMGroupFilter(data) {
        domElements.cbbMGroup.multiselect().empty();
        let totalTicket = domElements.divPopupAssDetect.attr('data-total-ticket');
        let totalTicketDetect = domElements.divPopupAssDetect.attr('data-total-ticket-detect');

        if (data) {
            let isExistedBlank = false;
            if (parseInt(totalTicketDetect) < parseInt(totalTicket)) {
                domElements.cbbMGroup.append(`<option value='0'>-</option>`);
                cacheFilterDependOn.MGrList = ReUnshiftBlankMGr(cacheFilterDependOn.MGrList);
                isExistedBlank = true;
            }

            data.forEach(element => {
                if (element.MDetectGroupID == 0) {
                    if (!isExistedBlank) {
                        domElements.cbbMGroup.append(`<option value='0'>-</option>`);
                        cacheFilterDependOn.MGrList = ReUnshiftBlankMGr(cacheFilterDependOn.MGrList);
                    }
                } else {
                    domElements.cbbMGroup.append(`<option selected value='${element.MDetectGroupID}'>${element.MDetectGroup}</option>`);
                }
            });

            domElements.cbbMGroup.parent().find('button').removeClass('border-error');
        }

        domElements.cbbMGroup.multiselect("refresh");
    }

    function SetupDefaultMGroupFilter(data) {
        domElements.cbbMGroup.multiselect().empty();

        if (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i] == "-") {
                    domElements.cbbMGroup.append(`<option selected value='0'>${data[i]}</option>`);
                } else {
                    domElements.cbbMGroup.append(`<option selected value='${data[i]}'>${data[i]}</option>`);
                }
            }

            domElements.cbbMGroup.parent().find('button').removeClass('border-error');
        }

        domElements.cbbMGroup.multiselect("refresh");
    }

    function SetupDanger(data) {
        let cacheSelectedDanger = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbDanger) {
            cacheSelectedDanger = cacheSelected.cbbDanger;
        }

        if (data) {
            let dangerSelector = domElements.cbbDanger.empty();

            if (data.NoneDanger.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(0, "None", data.NoneDanger.DangerItems, cacheSelectedDanger));

            if (data.Danger1.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(1, "Original", data.Danger1.DangerItems, cacheSelectedDanger));

            if (data.Danger2.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(2, "ABI", data.Danger2.DangerItems, cacheSelectedDanger));

            if (data.Danger3.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(3, "Other Sports", data.Danger3.DangerItems, cacheSelectedDanger));

            if (data.Danger4.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(4, "Basketball", data.Danger4.DangerItems, cacheSelectedDanger));

            if (data.Danger5.DangerItems.length > 0) dangerSelector.append(BuildOptgroup(5, "E-Sports", data.Danger5.DangerItems, cacheSelectedDanger));

            domElements.cbbDanger.parent().find('button').removeClass('border-error');

            domElements.cbbDanger.multiselect('refresh');
        }
    }

    function BuildOptgroup(optgroupValue, optgroupName, items, cacheSelectedDanger) {
        let optgroup = `<optgroup label="${optgroupName}" DangerType=${optgroupValue}>`;
        if (cacheSelectedDanger.length > 0) {
            items.map(function (x) {
                if (cacheSelectedDanger.indexOf(optgroupValue + '-' + x.Danger) != -1) {
                    optgroup += `<option selected value=${optgroupValue}-${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
                } else {
                    optgroup += `<option value=${optgroupValue}-${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
                }
            })
        } else {
            items.map(function (x) {
                optgroup += `<option selected value=${optgroupValue}-${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
            })
        }

        optgroup += "</optgroup>";

        return optgroup;
    }

    function SetupCC(data) {
        let cacheSelectedCC = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbCC) {
            cacheSelectedCC = cacheSelected.cbbCC;
        }

        if (data) {
            let ccSelector = domElements.cbbCC.empty();
            let ccOthers = [];

            domElements.cbbCCHidden.find('optgroup').each(function () {
                let optGroup = $(this);
                let groupName = optGroup.attr("label");
                let ccItems = [];
                optGroup.find('option:selected').each(function (idx, opItem) {
                    ccItems.push(parseInt($(opItem).val()));
                });

                let ccMatched = ccItems.filter(item => data.includes(item));

                data = data.filter(item => !ccMatched.includes(item));

                if (groupName.toLocaleLowerCase() == 'others') {
                    if (data.length > 0) {
                        ccOthers = [...new Set([...ccOthers, ...data])];
                    }
                    if (ccMatched.length > 0) {
                        ccOthers = [...new Set([...ccOthers, ...ccMatched])];
                    }
                } else {
                    if (ccMatched.length > 0) {
                        ccSelector.append(BuildCCGroup(ccMatched, groupName, cacheSelectedCC));
                    }
                }
            });

            if (ccOthers.length > 0) {
                ccSelector.append(BuildCCGroup(ccOthers, "Others", cacheSelectedCC));
            }

            domElements.cbbCC.parent().find('button').removeClass('border-error');

            domElements.cbbCC.multiselect('refresh');
        }
    }

    function BuildCCGroup(ccItems, groupName, cacheSelectedCC) {
        let optgroup = `<optgroup label="${groupName}">`;

        if (cacheSelectedCC.length > 0) {
            ccItems.map(function (item) {
                let itemName = item == 0 ? '-' : item;
                if (cacheSelectedCC.indexOf(item.toString()) != -1) {
                    optgroup += `<option selected value=${item}>${itemName}</option>`;
                } else {
                    optgroup += `<option value=${item}>${itemName}</option>`;
                }
            });
        } else {
            ccItems.map(function (item) {
                let itemName = item == 0 ? '-' : item;
                optgroup += `<option selected value=${item}>${itemName}</option>`;
            });
        }

        optgroup += "</optgroup>";

        return optgroup;
    }

    function SetupSubscriberTypeFilter(data) {
        if (data && data.length > 0) {
            domElements.cbbSites.empty();
            data.forEach(item => {
                let subName = "All";
                if (item == "C") {
                    subName = "Credit";
                } else if (item == "L") {
                    subName = "Licensee";
                }
                domElements.cbbSites.append(`<option value='${item}'>${subName}</option>`);
            });

            if (data.length > 1) {
                let cacheSelected = cacheFilterForImportNew[currentType];
                if (cacheSelected != undefined && cacheSelected.cbbSites) {
                    domElements.cbbSites.val(cacheSelected.cbbSites);
                }
            }
        }
    }

    function SetupHDPBetTeam(hdpBetTeamItems) {
        let cacheSelectedHDPBetTeam = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbHDPBetTeam) {
            cacheSelectedHDPBetTeam = cacheSelected.cbbHDPBetTeam;
        }

        domElements.cbbHDPBetTeam.multiselect().empty();

        if (hdpBetTeamItems.length > 0) {
            if (cacheSelectedHDPBetTeam.length > 0) {
                hdpBetTeamItems.forEach(item => {
                    if ($.inArray(item.toString(), cacheSelectedHDPBetTeam) != -1) {
                        domElements.cbbHDPBetTeam.append(`<option selected value='${item}'>${item}</option>`);
                    } else {
                        domElements.cbbHDPBetTeam.append(`<option value='${item}'>${item}</option>`);
                    }
                });
            } else {
                hdpBetTeamItems.forEach(item => {
                    domElements.cbbHDPBetTeam.append(`<option selected value='${item}'>${item}</option>`);
                });
            }
        }

        domElements.cbbHDPBetTeam.multiselect("refresh");
    }

    function SetupTicketStatusFilter(data) {
        let cacheTicketStatus = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbTicketsStatus) {
            cacheTicketStatus = cacheSelected.cbbTicketsStatus;
        }

        domElements.cbbTicketsStatus.multiselect().empty();

        if (data) {
            if (cacheTicketStatus.length > 0) {
                data.forEach(item => {
                    if (cacheTicketStatus.indexOf(item.TicketStatus.toString()) != -1) {
                        domElements.cbbTicketsStatus.append(`<option selected value='${item.TicketStatus}'>${item.TicketStatusName}</option>`);
                    } else {
                        domElements.cbbTicketsStatus.append(`<option value='${item.TicketStatus}'>${item.TicketStatusName}</option>`);
                    }
                });
            } else {
                data.forEach(item => {
                    if (item.TicketStatus == 101) {// untick "Reject"
                        domElements.cbbTicketsStatus.append(`<option value='${item.TicketStatus}'>${item.TicketStatusName}</option>`);
                    } else {
                        domElements.cbbTicketsStatus.append(`<option selected value='${item.TicketStatus}'>${item.TicketStatusName}</option>`);
                    }
                });
            }

            domElements.cbbTicketsStatus.parent().find('button').removeClass('border-error');
        }

        domElements.cbbTicketsStatus.multiselect("refresh");
    }

    function SetupCashoutTypeFilter(data) {
        let cacheCashoutType = undefined;
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbCashoutType) {
            cacheCashoutType = cacheSelected.cbbCashoutType;
        }

        domElements.cbbCashoutType.empty();

        if (data && data.length > 0) {
            data.forEach(item => {
                domElements.cbbCashoutType.append(`<option value='${item.CashoutStatus}'>${item.CashoutName}</option>`);
            });

            domElements.cbbCashoutType.val(cacheCashoutType ?? `${data[0].CashoutStatus}`);
        }
    }

    function SetupDoubleDataTable() {
        $("#tbrMMHomeTeam").html(`
                                <table id="tblMMHomeTeam" class="table table-striped table-monitor-detail">
                                <thead>
                                    <tr>
                                        <th><input name="select_all" value="1" id="inputHomeSelectAll" type="checkbox" class="col-checkbox" /></th>
                                        <th>Username</th>
                                        <th>CC</th>
                                        <th>Bot</th>
                                        <th>Dgr</th>
                                        <th>Odds</th>
                                        <th>MY.Odds</th>
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">BetID</span>
                                                <span id="iconHToggleBetID" class="dropdown-button icon-collapse-expand">
                                                    <span></span>
                                                    <span></span>
                                                    <span></span>
                                                </div>
                                            </div>
                                        </th>
                                        <th>Cur</th>
                                        <th>Amt</th>
                                        <th>HDP</th>
                                        <th>Score</th>
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">Transtime</span>
                                                <span class="icon-transtime-autofill icon-icon-copy" id="iconHTranstimeAutofill"></span>
                                            </div>
                                        </th>
                                        <th>B.f.</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 5000 Usernames">
                                            M.Gr.<sup style="vertical-align: super; font-size: smaller;">i</sup>
                                        </th>
                                        <th>Reason</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>`);
        $("#tbrMMAwayTeam").html(`
                                <table id="tblMMAwayTeam" class="table table-striped table-monitor-detail">
                                <thead>
                                    <tr>
                                        <th><input name="select_all" value="1" id="inputAwaySelectAll" type="checkbox" class="col-checkbox" /></th>
                                        <th>Username</th>
                                        <th>CC</th>
                                        <th>Bot</th>
                                        <th>Dgr</th>
                                        <th>Odds</th>
                                        <th>MY.Odds</th>
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">BetID</span>
                                                <span id="iconAToggleBetID" class="dropdown-button icon-collapse-expand">
                                                    <span></span>
                                                    <span></span>
                                                    <span></span>
                                                </span>
                                            </div>
                                        </th>
                                        <th>Cur</th>
                                        <th>Amt</th>
                                        <th>HDP</th>
                                        <th>Score</th>
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">Transtime</span>
                                                <span class="icon-transtime-autofill icon-icon-copy" id="iconATranstimeAutofill"></span>
                                            </div>
                                        </th>
                                        <th>B.f.</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 5000 Usernames">
                                            M.Gr.<sup style="vertical-align: super; font-size: smaller;">i</sup>
                                        </th>
                                        <th>Reason</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>`);

        domElements.tblMMHomeTeam = $("#tblMMHomeTeam");
        domElements.tblMMAwayTeam = $("#tblMMAwayTeam");

        InitialDetailGird(domElements.tblMMHomeTeam);
        InitialDetailGird(domElements.tblMMAwayTeam);

        domElements.inputHomeSelectAll = $('#inputHomeSelectAll');
        domElements.inputAwaySelectAll = $('#inputAwaySelectAll');

        domElements.inputHomeSelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputHomeSelectAll, domElements.tblMMHomeTeam.DataTable()) });
        domElements.inputAwaySelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputAwaySelectAll, domElements.tblMMAwayTeam.DataTable()) });

        domElements.iconHToggleBetID = $('#iconHToggleBetID');
        domElements.iconAToggleBetID = $('#iconAToggleBetID');

        domElements.iconHToggleBetID.on('click', function () {
            if (domElements.iconHToggleBetID.hasClass('expand')) {
                domElements.iconHToggleBetID.removeClass('expand');
                ToggleBetID(domElements.tblMMHomeTeam, false);
            } else {
                domElements.iconHToggleBetID.addClass('expand');
                ToggleBetID(domElements.tblMMHomeTeam, true);
            }
        });
        domElements.iconAToggleBetID.on('click', function () {
            if (domElements.iconAToggleBetID.hasClass('expand')) {
                domElements.iconAToggleBetID.removeClass('expand');
                ToggleBetID(domElements.tblMMAwayTeam, false);
            } else {
                domElements.iconAToggleBetID.addClass('expand');
                ToggleBetID(domElements.tblMMAwayTeam, true);
            }
        });

        domElements.iconHTranstimeAutofill = $('#iconHTranstimeAutofill');
        domElements.iconATranstimeAutofill = $('#iconATranstimeAutofill');

        domElements.iconHTranstimeAutofill.on('click', function () {
            AutoFillTranstimeFromDataGrid();
        });

        domElements.iconATranstimeAutofill.on('click', function () {
            AutoFillTranstimeFromDataGrid();
        });
    }

    function SetupSingleTable() {
        $('#tbrMMSG').html(`
                                <table id="tblMMSG" class="table table-striped table-monitor-detail">
                                <thead>
                                    <tr>
                                        <th><input name="select_all" value="1" id="inputSGSelectAll" type="checkbox" class="col-checkbox" /></th>
                                        <th>Username</th>
                                        <th>CC</th>
                                        <th>Bot</th>
                                        <th>Dgr</th>
                                        <th>Odds</th>
                                        <th>MY.Odds</th>
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">BetID</span>                                                                                               
                                            </div>
                                        </th>
                                        <th>Cur</th>
                                        <th>Amt</th>
                                        <th>Choice</th>
                                        <th>Score</th>
                                        <th>
                                            <div class="type-outer">
                                                <span class="col-title">Transtime</span>
                                                <span class="icon-transtime-autofill icon-icon-copy" id="iconSGTranstimeAutofill"></span>
                                            </div>
                                        </th>
                                        <th>B.f.</th>
                                        <th title="Auto-detected Association Group">
                                            A.Gr.
                                        </th>
                                        <th title="Manual-detected Association Group: max. 5000 Usernames">
                                            M.Gr.<sup style="vertical-align: super; font-size: smaller;">i</sup>
                                        </th>
                                        <th>Reason</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>`);

        domElements.tblMMSG = $("#tblMMSG");

        InitialDetailGird(domElements.tblMMSG);

        domElements.inputSGSelectAll = $('#inputSGSelectAll');
        domElements.inputSGSelectAll.on('click', function () { ToggleGridSelectAll(domElements.inputSGSelectAll, domElements.tblMMSG.DataTable()) });

        domElements.iconSGTranstimeAutofill = $('#iconSGTranstimeAutofill');

        domElements.iconSGTranstimeAutofill.on('click', function () {
            AutoFillTranstimeFromDataGrid();
        });
    }

    //#endregion Setup Filter & Grid

    //#region Get Filter Params Selected
    function GetInputAmount() {
        let amount = domElements.inputCustAmountRM.val();
        if (regexValidAmount.test(amount) && Number(amount)) {
            if (amount < 0) {
                amount = 0;
            }
            if (amount > 922337203685477) {
                amount = 922337203685477;
            }
            return amount;
        }

        return undefined;
    }

    function GetSelectDataFilter(dom_ccb, isGetAllValue = false) {
        let allVal = '';

        if (dom_ccb[0].length == 0) {
            return allVal;
        }

        for (let i = 0; i < dom_ccb[0].length; i++) {
            allVal += dom_ccb[0][i].value;
        }
        if (isGetAllValue || dom_ccb.val().join('') != allVal) {
            return dom_ccb.val();
        }

        return undefined;
    }

    function GetSelectDataFilterTicketStatus() {
        let allVal = '';

        let ticketStatus = domElements.cbbTicketsStatus[0];
        let ticketStatusLength = domElements.cbbTicketsStatus[0].length;
        let ticketStatusSelected = domElements.cbbTicketsStatus.val();

        if (ticketStatusLength == 0) {
            return allVal;
        }

        for (let i = 0; i < ticketStatusLength; i++) {
            if (ticketStatus[i].value != "101") { // ticket Reject
                allVal += ticketStatus[i].value;
            }
        }

        let tRejectIdx = $.inArray("101", ticketStatusSelected);

        if (tRejectIdx !== -1) {
            // Remove the element using splice
            ticketStatusSelected.splice(tRejectIdx, 1);
        }

        if (ticketStatusSelected.join('') != allVal) {
            return ticketStatusSelected;
        }

        return undefined;
    }

    function GetHDPSelected(dom_ccb, isGetAllValue = false) {
        return isDisplaySingleGrid ? undefined : GetSelectDataFilter(dom_ccb, isGetAllValue);
    }

    function GetBetTeamSelected(dom_ccb, isGetAllValue = false) {
        return isDisplaySingleGrid ? GetSelectDataFilter(dom_ccb, isGetAllValue) : undefined;
    }

    function GetBetTypeSelected() {
        let betTypeSelected = $("option:selected", domElements.cbbBetType);

        if (betTypeSelected.length > 0) {
            return betTypeSelected.attr('data-bettypeid');
        }

        return domElements.betTypeID;
    }

    function GetAttributeBetTypeSelected(attribute) {
        return $("option:selected", domElements.cbbBetType).attr(attribute);
    }

    function GetBetIDSelected() {
        let betidpattern = GetAttributeBetTypeSelected('data-betidpattern');

        if (domElements.sportType != 2 && betidpattern.includes('X')) {
            let betIDString = "80" + domElements.cbbMapX.val();

            if (betidpattern.includes('YY')) {
                betIDString += ("0" + domElements.cbbMapYY.val()).slice(-2);
            }
            return betIDString;

        } else if (domElements.sportType == 2) {
            return $("option:selected", domElements.cbbBetType).attr('data-betid');
        } else {
            return 0; //default
        }
    }

    function IsFilterSuspicious(params) {
        if (params.AGroupList.length == cacheFilterDefault.AGrList.length
            && params.ReasonList.length == cacheFilterDefault.ReasonList.length
            && params.MGroupList.length == cacheFilterDependOn.MGrList.length) {
            return false;
        }
        return true;
    }

    function ValidateTicketStatus() {
        if (isDisplaySingleGrid) {
            domElements.tblMMSG.DataTable().clear().draw(false);
            domElements.divHomeMonitor.addClass('hide');
            domElements.divAwayMonitor.addClass('hide');
            domElements.divSGMonitor.removeClass('hide');
        } else {
            domElements.tblMMHomeTeam.DataTable().clear().draw(false);
            domElements.tblMMAwayTeam.DataTable().clear().draw(false);
            domElements.divHomeMonitor.removeClass('hide');
            domElements.divAwayMonitor.removeClass('hide');
            domElements.divSGMonitor.addClass('hide');
        }

        let opt = $("option", domElements.cbbTicketsStatus).length;
        if (opt > 0) {
            return true;
        }

        RefreshBreadcrumbTotalTickets(0, 0);

        return false;
    }

    function CheckSelected(element) {
        let opt = $("option", element).length;
        let optSelect = $("option:selected", element).length;

        if (opt > 0 && optSelect > 0) {
            return true;
        }
        return false;
    }

    function ValidateCbb(element) {
        if (!CheckSelected(element)) {
            element.parent().find('button').addClass('border-error');
            return false;
        } else {
            element.parent().find('button').removeClass('border-error');
            return true;
        }
    }

    function ValidationFilter() {
        let result = true;

        if (!ValidateCbb(domElements.cbbMatchScore)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbDanger)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbCurrency)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbReason)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbAGroup)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbMGroup)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbHDPBetTeam)) {
            result = false;
        }
        if (!ValidateCbb(domElements.cbbTicketsStatus)) {
            result = false;
        }
        if (domElements.boderTranstimeInput.hasClass('info-input-error')) {
            result = false;
        }
        if (domElements.iconTranstimeSubmit.hasClass('transtime-icon-forcus')) {
            result = false;
        }

        return result;
    }

    function GetDangerSelectDataFilter(dangerType) {
        let dangerValues = [];
        domElements.cbbDanger.find('optgroup').each(function () {
            let optGroup = $(this);
            let currentDangerType = optGroup.attr("DangerType");

            if (currentDangerType == dangerType) {
                optGroup.find('option:selected').each(function (idx, opItem) {
                    dangerValues.push($(opItem).attr("DangerValue"));
                })
            }
        });

        return dangerValues;
    }

    //#endregion Get Filter Params Selected

    //#region Render Filter

    function RenderFilter(data) {
        recordsTotalGlobal = data.RecordsTotal;
        isDisplaySingleGrid = data.IsDisplaySingleGrid;

        SetupFilterFromDictinary(domElements.cbbCurrency, data.CurrencyList);

        if (data.IsDisplaySingleGrid) {
            SetupHDPBetTeam(data.BetTeamList);
        } else {
            SetupHDPBetTeam(data.HDPList);
        }

        SetupFilterScore(domElements.cbbMatchScore, data.ScoreList);

        cacheFilterDefault.AGrList = data.GroupList;
        cacheFilterDefault.ReasonList = data.ReasonList;   

        SetupFilterGroupReason(domElements.cbbReason, data.ReasonList);

        SetupAGroupFilter(data.GroupList);

        if (transtime.fromDate == undefined && transtime.toDate == undefined) {
            RefreshReasonInfo(data.ReasonList);
        }

        SetupDefaultMGroupFilter(data.MGroupList);

        if (data.DropdownListFilter) {
            SetupCC(data.DropdownListFilter.CCList);
            SetupDanger(data.DropdownListFilter.DangerList);
            SetupTicketStatusFilter(data.DropdownListFilter.TicketStatusList);
            SetupCashoutTypeFilter(data.DropdownListFilter.CashoutStatusList);

            SetupSubscriberTypeFilter(data.DropdownListFilter.SubscriberTypeList);
        }

        if (data.DropdownListFilterDependOn) {
            cacheFilterDependOn.DropdownListFilterDependOn = data.DropdownListFilterDependOn;

            FilterDependOnSelected(IndexFilterSelected.Score);
        }

        cacheMGroup.allMGroup = {};
        cacheMGroup.selectMgroup = {};
        cacheFilterForImportNew[currentType] = undefined;
    }

    //#endregion Render

    //#region Cache Filter

    function CheckChangeFilterByAssociationDetection() {
        let mGrList = cacheFilterDependOn.MGrList.map(obj => ({ ...obj }));

        if (mGrList.length > 0) {
            // check blank in M.Gr
            let blankItems = $.grep(mGrList, function (item) {
                return item.MDetectGroupID == 0;
            });

            if (blankItems.length != 0) {
                RefreshMGr(mGrList);
                $("option", domElements.cbbMGroup).prop("selected", true);
                domElements.cbbMGroup.multiselect("refresh");
                return;
            }

            let selectedFilterCache = cacheFilter[associationDetection];

            if (selectedFilterCache != undefined) {
                let selectedFilter = GetForCacheByAssociationDetection();

                let equal = common.CompareObjects(selectedFilterCache, selectedFilter);

                if (equal) {
                    let newMGrList = $.grep(mGrList, function (item) {
                        return item.MDetectGroupID != 0;
                    });
                    RefreshMGr(newMGrList);
                } else {
                    mGrList.unshift({ "MDetectGroupID": 0, "MDetectGroup": "-", "Checked": true });
                    RefreshMGr(mGrList);
                }
            }
        }
    }

    function CacheSelectDataFilter() {
        if (currentType == MatchMonitorDetailViewType.TicketFraud) {
            cacheFilterScore = GetSelectDataFilter(domElements.cbbMatchScore);
        }
        else {
            cacheFilterScore = undefined;
        }
    }

    function CacheSelectFilterForImportNew() {
        var selectedFilter = cacheFilterForImportNew[currentType];
        if (selectedFilter == undefined) {
            selectedFilter = {};
        }

        selectedFilter.cbbMatchScore = GetSelectDataFilter(domElements.cbbMatchScore);
        selectedFilter.cbbSites = domElements.cbbSites.val();
        selectedFilter.cbbCC = GetSelectDataFilter(domElements.cbbCC);
        selectedFilter.cbbDanger = GetSelectDataFilter(domElements.cbbDanger);
        selectedFilter.cbbCurrency = GetSelectDataFilter(domElements.cbbCurrency);
        selectedFilter.cbbReason = GetSelectDataFilter(domElements.cbbReason);
        selectedFilter.cbbAGroup = GetSelectDataFilter(domElements.cbbAGroup);
        selectedFilter.cbbHDPBetTeam = GetSelectDataFilter(domElements.cbbHDPBetTeam);
        selectedFilter.cbbTicketsStatus = GetSelectDataFilterTicketStatus();
        selectedFilter.cbbCashoutType = domElements.cbbCashoutType.val();

        cacheFilterForImportNew[currentType] = selectedFilter;
    }

    function RefreshMGr(items) {
        domElements.cbbMGroup.multiselect().empty();

        for (var i = 0; i < items.length; i++) {
            if (items[i].MDetectGroupID == 0) {
                domElements.cbbMGroup.append(`<option value='${items[i].MDetectGroupID}'>${items[i].MDetectGroup}</option>`);
            }
            else {
                domElements.cbbMGroup.append(`<option selected value='${items[i].MDetectGroupID}'>${items[i].MDetectGroup}</option>`);
            }
        }

        domElements.cbbMGroup.multiselect("refresh");
    }

    function ReUnshiftBlankMGr(items) {
        let newMGrList = $.grep(items, function (item) {
            return item.MDetectGroupID != 0;
        });

        newMGrList.unshift({ "MDetectGroupID": 0, "MDetectGroup": "-", "Checked": false });

        return newMGrList;
    }

    function GetForCacheByAssociationDetection() {
        let selectedFilter = {};
        selectedFilter.cbbMatchScore = GetSelectDataFilter(domElements.cbbMatchScore, true);
        selectedFilter.cbbDanger = GetSelectDataFilter(domElements.cbbDanger, true);
        selectedFilter.cbbSites = domElements.cbbSites.val();
        selectedFilter.cbbCurrency = GetSelectDataFilter(domElements.cbbCurrency, true);
        selectedFilter.cbbReason = GetSelectDataFilter(domElements.cbbReason, true);
        selectedFilter.cbbAGroup = GetSelectDataFilter(domElements.cbbAGroup, true);

        return selectedFilter;
    }

    function SetCacheByAssociationDetection() {
        let selectedFilter = GetForCacheByAssociationDetection();

        if (selectedFilter != undefined) {
            cacheFilter[associationDetection] = selectedFilter;
        }
    }

    //#endregion Cache Filter

    //#region Fetch Match Monitor Detail

    function FetchMatchMonitorDetail(typeOfTickets = MatchMonitorDetailViewType.TicketFraud, lastScannedSequenceID = maxSequenceId) {
        recordsFilteredGlobal = 0;

        RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsFilteredGlobal);

        SetupDoubleDataTable();

        SetupSingleTable();

        setTimeout(() => {
            RefreshLengthPage(500, true);
        }, 10);

        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        if (lastScannedSequenceID || typeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
            domElements.tblMMHomeTeam.find('tbody').addClass('loading');
            domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            domElements.tblMMSG.find('tbody').addClass('loading');

            if (timeoutRenderTicketDetail) {
                clearTimeout(timeoutRenderTicketDetail);
            }

            domElements.breadcrumbTotalTickets.html('');

            if (isDisplaySingleGrid) {
                domElements.divHomeMonitor.addClass('hide');
                domElements.divAwayMonitor.addClass('hide');
                domElements.divSGMonitor.removeClass('hide');
            } else {
                domElements.divHomeMonitor.removeClass('hide');
                domElements.divAwayMonitor.removeClass('hide');
                domElements.divSGMonitor.addClass('hide');
            }

            timeoutRenderTicketDetail = setTimeout(() => {
                let params = {
                    EventDateToString: domElements.eventDataToString,
                    MatchID: domElements.matchID,
                    BetTypeID: GetBetTypeSelected(),
                    BetID: GetBetIDSelected(),
                    BetChoiceType: GetAttributeBetTypeSelected('data-betchoicetype'),
                    BetChoiceHome: GetAttributeBetTypeSelected('data-betchoicehome'),
                    BetChoiceAway: GetAttributeBetTypeSelected('data-betchoiceaway'),
                    LiveIndicator: domElements.cbbMarket.val(),
                    LastScannedSequenceID: lastScannedSequenceID,
                    IsSupportShowAllTickets: isSupportShowAllTickets,
                    IsFilter: true,
                    TypeOfTickets: typeOfTickets,
                    SportType: domElements.sportType,
                    ScoreList: domElements.sportType == 2 ? undefined : GetSelectDataFilter(domElements.cbbMatchScore, true),
                    Sites: domElements.cbbSites.val(),
                    CustomerClassList: GetSelectDataFilter(domElements.cbbCC, true),
                    IsDangerNone: GetDangerSelectDataFilter(0).length > 0,
                    Danger1List: GetDangerSelectDataFilter(1),
                    Danger2List: GetDangerSelectDataFilter(2),
                    Danger3List: GetDangerSelectDataFilter(3),
                    Danger4List: GetDangerSelectDataFilter(4),
                    Danger5List: GetDangerSelectDataFilter(5),
                    CurrencyIdList: GetSelectDataFilter(domElements.cbbCurrency, true),
                    ReasonList: GetSelectDataFilter(domElements.cbbReason, true),
                    Reason: domElements.reason,
                    AGroupList: GetSelectDataFilter(domElements.cbbAGroup, true),
                    MGroupList: GetSelectDataFilter(domElements.cbbMGroup, true),
                    HDPList: GetHDPSelected(domElements.cbbHDPBetTeam, true),
                    BetTeamList: GetBetTeamSelected(domElements.cbbHDPBetTeam, true),
                    CustAmountRM: GetInputAmount(),
                    TicketStatusList: GetSelectDataFilter(domElements.cbbTicketsStatus, true),
                    IsCashout: domElements.cbbCashoutType.val(),
                    MGrTranIDListFiltered: GetTranIDsByFilterMGr(),
                    Sport: domElements.sportType,
                    TranstimeFrom: transtime.fromDate,
                    TranstimeTo: transtime.toDate,
                    TicketType: domElements.ticketType
                };

                params.IsFilterSuspicious = IsFilterSuspicious(params);

                GetMatchMonitorTotalTicket(params, lastScannedSequenceID);

                if (recordsTotalGlobal > 0) {
                    GetMatchMonitorDetail(params, lastScannedSequenceID, typeOfTickets == MatchMonitorDetailViewType.TicketAll ? posClickGlobal : 0);
                }
                else {
                    domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                    domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                    domElements.tblMMSG.find('tbody').removeClass('loading');
                }
            }, 10)
        }
    }

    function GetMatchMonitorTotalTicket(data = null, lastScannedSequenceID = maxSequenceId) {
        if (lastScannedSequenceID || data.TypeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
            data.LastScannedSequenceID = lastScannedSequenceID;
            currentType = data.TypeOfTickets;

            $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorTotalTicket'), {
                async: false,
                type: 'POST',
                tryCount: 0,
                retryLimit: 3,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response?.recordsTotal >= 0) {
                        recordsTotalGlobal = response?.recordsTotal;
                        if ($('option', domElements.cbbMGroup).length > 1) {
                            domElements.breadcrumbTotalTickets.html(`Total Tickets: -/-`);
                        } else {
                            RefreshBreadcrumbTotalTickets(0, recordsTotalGlobal);

                            domElements.divPopupAssDetect.attr('data-total-ticket', recordsTotalGlobal);
                        }
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
    }

    function GetMatchMonitorDetail(data = null, lastScannedSequenceID = maxSequenceId, posClick = 0) {
        if (lastScannedSequenceID || data.TypeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
            data.LastScannedSequenceID = lastScannedSequenceID;
            currentType = data.TypeOfTickets;

            xhr = $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorDetail'), {
                type: 'POST',
                tryCount: 0,
                retryLimit: 3,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data),
                success: function (response) {
                    if (response?.data) {
                        if (data.TypeOfTickets == MatchMonitorDetailViewType.TicketAll && posClickGlobal != posClick) {
                            return;
                        }

                        RenderMatchMonitorDetail(response, data.TypeOfTickets);

                        if (data.TypeOfTickets == MatchMonitorDetailViewType.TicketAll && response?.data?.LastSequenceID) {
                            GetMatchMonitorDetail(data, response?.data?.LastSequenceID, posClick);
                        }

                        if (response?.data?.LastSequenceID == null) {
                            RefreshLengthPage(-1, false);

                            let hdTotalTicketTxt = domElements.hdbreadcrumTotalTickets.text();
                            if (hdTotalTicketTxt != "0") {
                                RefreshBreadcrumbTotalTickets(hdTotalTicketTxt, hdTotalTicketTxt);
                                domElements.hdbreadcrumTotalTickets.text("0");
                            }
                        }
                    }
                },
                complete: function (response) {
                    domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                    domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                    domElements.tblMMSG.find('tbody').removeClass('loading');
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

                    RefreshLengthPage(-1, false);
                    console.log(error);
                }
            });
        }
    }

    function FetchFilterMatchMonitorDetail() {
        recordsTotalGlobal = 0;

        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        GetFilterMatchMonitorDetail();
    }

    function GetFilterMatchMonitorDetail() {
        var paramFilter = {
            EventDateToString: domElements.eventDataToString,
            MatchID: domElements.matchID,
            BetTypeID: GetBetTypeSelected(),
            BetID: GetBetIDSelected(),
            BetChoiceType: GetAttributeBetTypeSelected('data-betchoicetype'),
            BetChoiceHome: GetAttributeBetTypeSelected('data-betchoicehome'),
            BetChoiceAway: GetAttributeBetTypeSelected('data-betchoiceaway'),
            Sites: domElements.cbbSites.val(),
            Reason: domElements.reason,
            LiveIndicator: domElements.cbbMarket.val(),
            TypeOfTickets: currentType,
            IsImportNewTicket: isImportNewTicket,
            IsSupportShowAllTickets: isSupportShowAllTickets,
            Sport: domElements.sportType,
            TranstimeFrom: transtime.fromDate,
            TranstimeTo: transtime.toDate,
            TicketType: domElements.ticketType
        };

        $.ajax(common.modifyRelativeUrl('MatchMonitor/GetMatchMonitorFilter'), {
            async: false,
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(paramFilter),
            success: function (response) {
                const data = response?.data;
                if (data) {
                    RenderFilter(data);

                    if (data.MaxSequenceId > 0 && currentType == MatchMonitorDetailViewType.TicketAll) {
                        maxSequenceId = data.MaxSequenceId;
                    }
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
            },
            timeout: 5000
        });

        isImportNewTicket = false;
    }

    //#endregion Fetch Match Monitor Detail

    //#region Render Grid
    function RenderMatchMonitorDetail(response, typeOfTickets) {
        let data = response?.data;

        if (data) {
            if (typeOfTickets == MatchMonitorDetailViewType.TicketFraud) {
                recordsTotalGlobal = recordsFilteredGlobal = response.recordsFiltered;
            }
            if (typeOfTickets == MatchMonitorDetailViewType.TicketAll) {
                recordsFilteredGlobal += response?.recordsFiltered ?? 0;
            }

            if (isDisplaySingleGrid) {
                RenderSingleGrid(data, recordsFilteredGlobal, recordsTotalGlobal);
            } else {
                RenderDoubleGrid(data, recordsFilteredGlobal, recordsTotalGlobal);
            }
        }
    }

    function RenderDoubleGrid(data, recordsFilteredGlobal, recordsTotalGlobal) {
        if (data?.HomeMatchItemList) {
            domElements.tblMMHomeTeam.DataTable().rows.add(data.HomeMatchItemList).draw(false);
        }

        if (data?.AwayMatchItemList) {
            domElements.tblMMAwayTeam.DataTable().rows.add(data.AwayMatchItemList).draw(false);
        }

        RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsTotalGlobal);
    }

    function RenderSingleGrid(data, recordsFilteredGlobal, recordsTotalGlobal) {
        if (data?.ItemList) {
            domElements.tblMMSG.DataTable().rows.add(data.ItemList).draw(false);
        }

        RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsTotalGlobal);
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

    function RenderOdds(obj) {  
        if (obj.IsParlay && obj.ParlayTicket != undefined) {
            return obj.ParlayTicket.Odds;
        }

        return obj.Odds;
    }

    function RenderMalayOdds(obj) {
        if (obj.IsParlay && obj.ParlayTicket != undefined) {
            return obj.ParlayTicket.MalayOdds;
        }

        return obj.MalayOdds;
    }

    function RenderBetInfo(obj) {
        if (domElements.ticketType == 1) {
            if (!isDisplaySingleGrid && obj.TransIdToString.length > 6) {
                let halfBetID = '...' + obj.TransIdToString.substring(obj.TransIdToString.length - 6);

                return `<span class='span-betid' data-bet-id=${obj.TransIdToString}>${halfBetID}</span>`;
            } else {
                return `<span class='span-betid' data-bet-id=${obj.TransIdToString}>${obj.TransIdToString}</span>`;
            }
        } else {
            return RenderParlayBetInfo(obj);
        }        
    }

    function RenderParlayBetInfo(obj) {
        let html = '';

        if (obj.ViewMoreInfo && obj.ParlayTicket != undefined) {           
            let ticket = obj.ParlayTicket;           
            
            if (ticket.IsMainTicket) {
                html = `<div>
                            <ul><li class='parlay-li'>${ticket.HomeName} vs ${ticket.AwayName}</li></ul>
                        </div>`;
            } else {
                html = `<div>                    
                            <ul><li class='parlay-li bold'>${ticket.BetTypeName} - ${ticket.BetTeam}</li></ul>
                            <ul><li class='parlay-li bold'>${ticket.HomeName} vs ${ticket.AwayName}</li></ul>
                            <ul><li class='parlay-li'>${ticket.SportName} ${ticket.LeagueName}</li></ul>
                            <ul><li class='parlay-li'>${ticket.GlobalShowTime}</li></ul>                  
                        </div>`;
            }

            return `<span class='span-betid' data-bet-id=${obj.TransIdToString}>${html} </span>`;
        } else if (obj.ViewMoreInfo) {
            let description = obj.Combinition == 2 ? "Double (1 Bet)" : "Treble(1 Bet)";
            
            return `<div style='white-space: nowrap'>
                        <span class='span-betid' data-bet-id=${obj.TransIdToString}>${obj.TransIdToString} </span>
                        <span class="icon-chevron-down"></span>
                        <div class='more-info'>       
                            <ul><li class='parlay-li'>Mix Parlay</li></ul>
                            <ul><li class='parlay-li'>${description}</li></ul>
                        </div>
                    </div>`;
        } else {
            return `<div style='white-space: nowrap'>
                        <span class='span-betid' data-bet-id=${obj.TransIdToString}>${obj.TransIdToString} </span>
                        <span class="icon-chevron-right"></span>
                    </div>`;
        }
    }

    function RenderCurrency(obj) {
        if (obj.IsParlay && obj.ParlayTicket != undefined) {               
            return obj.ParlayTicket.CurrencyName;
        }

        return obj.CurrencyName;
    }

    function RenderAmount(obj) {
        if (obj.IsParlay && obj.ParlayTicket != undefined) {
            return ConvertNumberToThousandsString(obj.ParlayTicket.Amount);
        }
        return ConvertNumberToThousandsString(obj.Amount);
    }

    function RenderHDPChoice(obj) {
        if (obj.IsParlay && obj.ParlayTicket != undefined) {
            return obj.ParlayTicket.Choice;
        }
        return isDisplaySingleGrid ? obj.BetTeam : obj.HDP;        
    }

    function RenderScore(obj) {
        if (obj.IsParlay && obj.ParlayTicket != undefined) {
            return obj.ParlayTicket.Score;
        }

        return obj.Score;
    }

    //#endregion Render Grid

    //#region Refresh
    function RefreshLengthPage(length, isDraw = true) {
        domElements.tblMMHomeTeam.DataTable().page.len(length).draw(isDraw);
        domElements.tblMMAwayTeam.DataTable().page.len(length).draw(isDraw);
        domElements.tblMMSG.DataTable().page.len(length).draw(isDraw);
    }

    function RefreshFilterSelected(element, data) {
        if (element && data) {
            element.val(data);
            element.multiselect("refresh");
        }
    }

    function RefreshReasonInfo(reasonList) {
        let data = reasonList;

        if (data?.length > 0) {
            let filteredReasons = data.filter(item => !item.includes('-'));
            domElements.spanReasonInfo.text(filteredReasons.sort().join()); return;
        }
        domElements.spanReasonInfo.text('');
    }

    function RefreshBreadcrumbTotalTickets(recordsFilteredGlobal, recordsTotalGlobal) {
        domElements.breadcrumbTotalTickets.html(`Total Tickets: ${ConvertNumberToThousandsString(recordsFilteredGlobal)}/${ConvertNumberToThousandsString(recordsTotalGlobal)}`);
    }

    //#endregion Refresh

    //#region Verify Trans
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

    function VerifyTransMatchMonitorDetail() {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/VerifyTransMMDetail'), {
            type: 'POST',
            data:
            {
                MatchId: domElements.matchID,
                LiveIndicator: domElements.cbbMarket.val(),
                BettypeID: GetBetTypeSelected(),
                BetID: GetBetIDSelected(),
                TranIds: GetTranIds().join()
            },
            beforeSend: function () {
                domElements.tblMMHomeTeam.addClass('loading-value');
                domElements.tblMMAwayTeam.addClass('loading-value');
            },
            success: function () {
                window.close();
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

    //#endregion Verify Trans

    //#region Detect Association
    function DetectAssociation() {
        posClickGlobal++;
        if (xhr) {
            xhr.onreadystatechange = null;
            xhr.abort();
        }

        cacheFilterDependOn.MGrList = [];
        cacheMGroup.allMGroup = {};
        cacheMGroup.selectMgroup = {};

        SetCacheByAssociationDetection();

        let params = {
            RawCustIDs: domElements.hdSelectedCustIDs.val(),
            RawUserNames: domElements.hdSelectedUsernames.val(),
            IsOptBP: domElements.inputOptBP.is(":checked"),
            IsOptDevice: domElements.inputOptDevice.is(":checked"),
            IsOptIP: domElements.inputOptIP.is(":checked"),
            IsOptAgent: domElements.inputOptAgent.is(":checked"),
            IsOpt3MatchesLast7Days: domElements.inputOpt3MatchesLast7Days.is(":checked"),
            IsOptIPLast3Days: domElements.inputOptIPLast3Days.is(":checked")
        }

        xhr = $.ajax(common.modifyRelativeUrl('MatchMonitor/DetectGroupAssWithOptions'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.tblMMHomeTeam.find('tbody').addClass('loading');
                domElements.tblMMAwayTeam.find('tbody').addClass('loading');
            },
            success: function (response) {
                let totalItem = 0;
                let itemList = response?.data?.ItemList;
                if (response?.data?.ItemList) {
                    cacheMGroup.isActive = true;

                    cacheFilterDependOn.MGrList = response.mGrList.MGrItems;

                    SetupMGroupFilter(response.mGrList.MGrItems);

                    if (isDisplaySingleGrid) {
                        BindChangeGroupAssWithOptions(domElements.tblMMSG.DataTable(), itemList);
                        let itemSG = domElements.tblMMSG.DataTable().rows().data().filter(function (x) { return cacheMGroup.selectMgroup[x.BetID]; });

                        totalItem = itemSG.length;
                    } else {
                        BindChangeGroupAssWithOptions(domElements.tblMMHomeTeam.DataTable(), itemList);
                        BindChangeGroupAssWithOptions(domElements.tblMMAwayTeam.DataTable(), itemList);

                        let itemMMHomeTeam = domElements.tblMMHomeTeam.DataTable().rows().data().filter(function (x) { return cacheMGroup.selectMgroup[x.BetID]; });
                        let itemMMAwayTeam = domElements.tblMMAwayTeam.DataTable().rows().data().filter(function (x) { return cacheMGroup.selectMgroup[x.BetID]; });

                        totalItem = itemMMHomeTeam.length + itemMMAwayTeam.length;
                    }
                }

                RefreshBreadcrumbTotalTickets(totalItem, totalItem);
            },
            complete: function (response) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');

                domElements.inputHomeSelectAll.prop('checked', false);
                domElements.inputAwaySelectAll.prop('checked', false);
                domElements.inputSGSelectAll.prop('checked', false);
            },
            error: function (error) {
                domElements.tblMMHomeTeam.find('tbody').removeClass('loading');
                domElements.tblMMAwayTeam.find('tbody').removeClass('loading');
                console.log(error);
            }
        });

        domElements.divPopupAssDetect.dialog('close');
    }

    function ShowPopupAssDetection() {
        let ctsCustIDs = [];
        let custIDs = [];
        let userNames = [];

        if (isDisplaySingleGrid) {
            domElements.tblMMSG.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    custIDs.push(data.CustID);
                    userNames.push(data.Username);
                }
            });
        } else {
            domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    custIDs.push(data.CustID);
                    userNames.push(data.Username);
                }
            });
            domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    custIDs.push(data.CustID);
                    userNames.push(data.Username);
                }
            });
        }

        domElements.divPopupAssDetect.attr('data-total-ticket-detect', custIDs.length);

        ctsCustIDs = ctsCustIDs.filter(OnlyUnique);
        custIDs = custIDs.filter(OnlyUnique);
        userNames = userNames.filter(OnlyUnique);

        if (IsValidAssDetection(custIDs.length)) {
            domElements.hdSelectedCustIDs.val(custIDs.join());
            domElements.hdSelectedUsernames.val(userNames.join());

            domElements.divPopupAssDetect.dialog({
                modal: true,
                width: '800px',
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

    function IsValidAssDetection(custSelectedLength) {
        if (custSelectedLength <= 1) {
            common.showError("Please select at least 2 Usernames to proceed!");
            return false;
        }

        if (custSelectedLength > 5000) {
            common.showError("Exceeded max 5000 usernames. Please try again with a smaller range.");
            return false;
        }

        return true;
    }

    function OnlyUnique(value, index, self) {
        return self.indexOf(value) == index;
    }

    function BindChangeGroupAssWithOptions(table, itemList) {
        table.rows().every(function () {
            let rowData = this.data();
            if ($(this.node()).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                let newGroupData = itemList.find(function (x) { return x.CustID == rowData.CustID; });
                if (newGroupData != undefined && newGroupData.GroupID > 0) {
                    rowData.MDetectGroupID = newGroupData.GroupID;
                    rowData.MDetectGroup = newGroupData.GroupName;

                    cacheMGroup.allMGroup[rowData.BetID] = newGroupData;
                    cacheMGroup.selectMgroup[rowData.BetID] = true;
                } else $(this.node()).css('display', 'none');
            } else $(this.node()).css('display', 'none');

            this.invalidate();
        })
    }

    function GetTranIDsByFilterMGr() {
        let arrMGrTranIDs = [];
        Object.keys(cacheMGroup.allMGroup).forEach((betID) => {
            arrMGrTranIDs.push({
                BetID: betID,
                MGrName: cacheMGroup.allMGroup[betID].GroupName
            });
        })

        return JSON.stringify(arrMGrTranIDs);
    }

    //#endregion Detect Association

    //#region Export To Excel
    function ExportToExcel() {
        if (IsValidExportExcel()) {
            let mmDetails = {
                isViewParlay: domElements.ticketType == 2,
                isDisplaySingleGrid: isDisplaySingleGrid,
                matchInfo: GetDataMatchInfo(),
                homeMItems: isDisplaySingleGrid ? null : GetDataSelectedInGrid(domElements.tblMMHomeTeam.DataTable()),
                awayMItems: isDisplaySingleGrid ? null : GetDataSelectedInGrid(domElements.tblMMAwayTeam.DataTable()),
                mItems: isDisplaySingleGrid ? GetDataSelectedInGrid(domElements.tblMMSG.DataTable()) : null
            }
            let url = common.modifyRelativeUrl('MatchMonitor/ExportToExcelMMDetail');

            $('body').append('<form action="' + url + '" method="post" target="output_frame" id="postToIframe"></form>');
            $('#postToIframe').append('<input type="hidden" asp-for="isDisplaySingleGridf" name=isDisplaySingleGrid value="' + mmDetails.isDisplaySingleGrid + '" />');
            $('#postToIframe').append('<input type="hidden" asp-for="isViewParlay" name=isViewParlay value="' + mmDetails.isViewParlay + '" />');
            $('#postToIframe').append('<input type="hidden" asp-for="matchf" name=match value="' + mmDetails.matchInfo + '" />');
            $('#postToIframe').append('<input type="hidden" asp-for="homef" name=homes value="[' + mmDetails.homeMItems + ']" />');
            $('#postToIframe').append('<input type="hidden" asp-for="awayf" name=aways value="[' + mmDetails.awayMItems + ']" />');
            $('#postToIframe').append('<input type="hidden" asp-for="itemf" name=items value="[' + mmDetails.mItems + ']" />');
            $('body').append('<iframe  name="output_frame" width="0" height="0" class="display-none"></iframe>');
            $('#postToIframe').submit().remove();

            common.showSuccess("Export excel file is in process");
        }
    }

    function IsValidExportExcel() {
        let homeCheckedL = 0,
            awayCheckedL = 0,
            itemCheckL = 0;

        if (isDisplaySingleGrid) {
            domElements.tblMMSG.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked'))
                    itemCheckL = itemCheckL + 1;
            })
        } else {
            domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked'))
                    homeCheckedL = homeCheckedL + 1;
            })
            domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked'))
                    awayCheckedL = awayCheckedL + 1;
            })
        }

        if (homeCheckedL + awayCheckedL + itemCheckL < 1) {
            common.showError("Please select at least 1 record to proceed!")
            return false;
        }
        if (homeCheckedL + awayCheckedL + itemCheckL > 40000) {
            common.showError("Export fail due to size limitation, please contact Administrator for further support.")
            return false;
        }

        return true;
    }

    function GetDataMatchInfo() {
        let datas = '';
        let betTypeName = $('option:selected', domElements.cbbBetType).text();

        datas += `{&quot;MatchName&quot;:&quot;${domElements.matchName}&quot;`;
        datas += `,&quot;EventDateStr&quot;:&quot;${domElements.eventDataToString}&quot;`;
        datas += `,&quot;BetTypeName&quot;:&quot;${betTypeName}&quot;}`;
        return datas;
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
                    datas += `,&quot;MalayOdds&quot;:&quot;${data.MalayOdds ?? "-"}&quot;`;
                    datas += `,&quot;BetID&quot;:&quot;${data.BetID}&quot;`;
                    datas += `,&quot;BetTeam&quot;:&quot;${data.BetTeam}&quot;`;
                    datas += `,&quot;Currency&quot;:&quot;${data.CurrencyName}&quot;`;
                    datas += `,&quot;Amount&quot;:&quot;${data.Amount}&quot;`;
                    datas += `,&quot;HDP&quot;:&quot;${data.HDP}&quot;`;
                    datas += `,&quot;Choice&quot;:&quot;${data.Choice}&quot;`;
                    datas += `,&quot;Score&quot;:&quot;${data.Score}&quot;`;
                    datas += `,&quot;TransTimeToString&quot;:&quot;${data.TransTimeToString}&quot;`;
                    datas += `,&quot;BetFrom&quot;:&quot;${data.BetFrom}&quot;`;
                    datas += `,&quot;ADetectGroup&quot;:&quot;${data.ADetectGroup}&quot;`;
                    datas += `,&quot;MDetectGroup&quot;:&quot;${data.MDetectGroup}&quot;`;
                    datas += `,&quot;ReasonList&quot;:&quot;${data.ReasonList}&quot;`;
                    datas += `,&quot;PICVerified&quot;:&quot;${data.PICVerified}&quot;`;

                    datas += `,&quot;ParlayTickets&quot;:[${GetExportParlayTicket(data.ParlayTickets)}]`;

                    datas += `},`;
                }
            }
        });

        return datas != '' ? datas.slice(0, -1) : datas;
    }

    function GetExportParlayTicket(parlayTickets) {
        let datas = '';
        $.each(parlayTickets, function (index, value) {
            datas += `{&quot;HomeName&quot;:&quot;${value.HomeName}&quot;`;
            datas += `,&quot;AwayName&quot;:&quot;${value.AwayName}&quot;`;
            datas += `,&quot;BetTypeName&quot;:&quot;${value.BetTypeName}&quot;`;
            datas += `,&quot;BetTeam&quot;:&quot;${value.BetTeam}&quot;`;
            datas += `,&quot;LeagueName&quot;:&quot;${value.LeagueName}&quot;`;
            datas += `,&quot;SportName&quot;:&quot;${value.SportName}&quot;`;
            datas += `,&quot;GlobalShowTime&quot;:&quot;${value.GlobalShowTime}&quot;`;
            datas += `,&quot;IsMainTicket&quot;:&quot;${value.IsMainTicket}&quot;`;
            datas += `,&quot;Odds&quot;:&quot;${value.Odds}&quot;`;
            datas += `,&quot;MalayOdds&quot;:&quot;${value.MalayOdds}&quot;`;
            datas += `,&quot;CurrencyName&quot;:&quot;${value.CurrencyName}&quot;`;
            datas += `,&quot;Amount&quot;:&quot;${value.Amount}&quot;`;
            datas += `,&quot;Choice&quot;:&quot;${value.Choice}&quot;`;
            datas += `,&quot;Score&quot;:&quot;${value.Score}&quot;`;
            datas += `},`;
        })
            
                
        return datas != '' ? datas.slice(0, -1) : datas;
    }

    //#endregion Export To Excel  

    //#region Feedback
    function GetRangeTransTime() {
        let rangeTransTime = [];
        let currentMin = '';
        let currentMax = '';

        if (isDisplaySingleGrid) {
            domElements.tblMMSG.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    if (currentMin == '') {
                        currentMin = data;
                        currentMax = data;
                    } else {
                        if (currentMin.TransTime > data.TransTime) currentMin = data;
                        if (currentMax.TransTime < data.TransTime) currentMax = data;
                    }
                }
            })
        } else {
            domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    if (currentMin == '') {
                        currentMin = data;
                        currentMax = data;
                    } else {
                        if (currentMin.TransTime > data.TransTime) currentMin = data;
                        if (currentMax.TransTime < data.TransTime) currentMax = data;
                    }
                }
            });

            domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    if (currentMin == '') {
                        currentMin = data;
                        currentMax = data;
                    } else {
                        if (currentMin.TransTime > data.TransTime) currentMin = data;
                        if (currentMax.TransTime < data.TransTime) currentMax = data;
                    }
                }
            })
        }

        rangeTransTime.push(currentMin.TransTimeToString);
        rangeTransTime.push(currentMax.TransTimeToString);

        return rangeTransTime;
    }

    //#endregion Feedback

    //#region Event OnChange

    function ChangeCbbTypeOfTickets() {
        RemoveAllFilterError();

        let betTypeSelected = $("option:selected", domElements.cbbBetType);

        if (betTypeSelected.attr('data-is-support-suspicious') == 'false') {
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketAll);
            domElements.cbbTypeOfTickets.prop('disabled', true);
        } else if (!isSupportShowAllTickets) {
            $(".readOnly", domElements.cbbBetType).prop("disabled", true);
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud);
            domElements.cbbTypeOfTickets.prop('disabled', true);
        } else {
            domElements.cbbTypeOfTickets.val(MatchMonitorDetailViewType.TicketFraud);
            domElements.cbbTypeOfTickets.prop('disabled', false);
        }
    }

    function ResetDefaulCbbSite() {
        domElements.cbbSites.prop('disabled', false);
        domElements.cbbSites.empty();
        domElements.cbbSites.append(`<option selected value='A'>All</option>`);
        domElements.cbbSites.append(`<option selected value='C'>Credit</option>`);
        domElements.cbbSites.append(`<option selected value='L'>Licensee</option>`);

        domElements.cbbSites.val(SiteViewType.SiteAll);
    }

    function BetTypeOnChange() {
        AddFilterLoading();

        cacheFilterScore = undefined;

        ResetDefaulCbbSite();

        ChangeCbbTypeOfTickets();

        let txtMarket = $("option:selected", domElements.cbbMarket).text();
        domElements.spanMarketInfo.text(`${txtMarket}`);

        let txtBetType = $("option:selected", domElements.cbbBetType).text();
        domElements.spanSportBetTypeInfo.text(`${domElements.sportName} - ${txtBetType} -`);

        currentType = Number(domElements.cbbTypeOfTickets.val());

        cacheFilterDependOn.MGrList = [];

        isImportNewTicket = true;

        FetchFilterMatchMonitorDetail();

        domElements.btnVerifyAndClose.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

        domElements.btnReasonFeedback.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

        RemoveFilterLoading();
    }

    function TypeOfTicketsOnChange() {
        AddFilterLoading();

        domElements.cbbSites.prop('disabled', false);

        ResetDefaulCbbSite();

        currentType = Number(domElements.cbbTypeOfTickets.val());

        cacheFilterDependOn.MGrList = [];

        FetchFilterMatchMonitorDetail();

        domElements.btnVerifyAndClose.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

        domElements.btnReasonFeedback.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

        RemoveFilterLoading();
    }

    function ScoreOnChange() {
        $('option', domElements.cbbSites).each(function (_, siteItem) {
            if ($(siteItem).val() == 'A') {
                domElements.cbbSites.val('A');
            }
        });

        $('option', domElements.cbbCC).prop("selected", true);
        domElements.cbbCC.multiselect('refresh');

        $('option', domElements.cbbDanger).prop("selected", true);
        domElements.cbbDanger.multiselect('refresh');

        $("option", domElements.cbbHDPBetTeam).prop("selected", true);
        domElements.cbbHDPBetTeam.multiselect('refresh');

        cacheFilterDependOn.MGrList = [];
        cacheMGroup.allMGroup = {};
        cacheMGroup.selectMgroup = {};

        domElements.cbbMGroup.multiselect().empty();
        domElements.cbbMGroup.append(`<option selected value='0'>-</option>`);
        domElements.cbbMGroup.multiselect("refresh");

        domElements.inputCustAmountRM.val('');
        domElements.iconClearUsnAmt.addClass("hide");

        $("option", domElements.cbbTicketsStatus).each(function (_, itemStatus) {
            if ($(itemStatus).val() == 101) { // Reject
                $(itemStatus).prop("selected", false);
            } else {
                $(itemStatus).prop("selected", true);
            }
        });
        domElements.cbbTicketsStatus.multiselect('refresh');

        var cashoutOption = $('option', domElements.cbbCashoutType)[0]?.value;
        if (cashoutOption) {
            domElements.cbbCashoutType.val(cashoutOption);
        }

        FilterDependOnSelected(IndexFilterSelected.Score);
    }

    function FilterDependOnSelected(indexFilterSelected) {
        let scoresSelected = domElements.cbbMatchScore.val();
        let siteSelected = domElements.cbbSites.val();
        let ccSelected = domElements.cbbCC.val();
        let isDangerNone = GetDangerSelectDataFilter(0).length > 0;
        let danger1Selected = GetDangerSelectDataFilter(1);
        let danger2Selected = GetDangerSelectDataFilter(2);
        let danger3Selected = GetDangerSelectDataFilter(3);
        let danger4Selected = GetDangerSelectDataFilter(4);
        let danger5Selected = GetDangerSelectDataFilter(5);

        let currencySelected = domElements.cbbCurrency.val();
        let reasonSelected = domElements.cbbReason.val();

        let isNoDangerSelect = !isDangerNone && danger1Selected.length == 0 && danger2Selected.length == 0 &&
            danger3Selected.length == 0 && danger4Selected.length == 0 && danger5Selected.length == 0;

        if (cacheFilterDependOn.DropdownListFilterDependOn) {
            let dependOnFilter = cacheFilterDependOn.DropdownListFilterDependOn.filter(function (item) {
                if (scoresSelected.length == 0 || isNoDangerSelect) {
                    return false;
                }
                if (indexFilterSelected == IndexFilterSelected.Reason) {
                    if (!reasonSelected.find(x => item.ReasonItem.indexOf(x) > -1)) {
                        return false;
                    }
                    if (!currencySelected.find(x => x == item.CurrencyId)) {
                        return false;
                    }
                }

                if (indexFilterSelected == IndexFilterSelected.Currency) {
                    if (!currencySelected.find(x => x == item.CurrencyId)) {
                        return false;
                    }
                }

                let countDangerType = 0;
                if (isDangerNone && item.DangerToString == '-') {
                    countDangerType += 1;
                }
                if (danger1Selected.length > 0 && danger1Selected.find(x => x == item.Danger1)) {
                    countDangerType += 1;
                }
                if (danger2Selected.length > 0 && danger2Selected.find(x => x == item.Danger2)) {
                    countDangerType += 1;
                }
                if (danger3Selected.length > 0 && danger3Selected.find(x => x == item.Danger3)) {
                    countDangerType += 1;
                }
                if (danger4Selected.length > 0 && danger4Selected.find(x => x == item.Danger4)) {
                    countDangerType += 1;
                }
                if (danger5Selected.length > 0 && danger5Selected.find(x => x == item.Danger5)) {
                    countDangerType += 1;
                }
                if (countDangerType == 0) {
                    return false;
                }

                if (!ccSelected.find(x => x == item.CC)) {
                    return false;
                }

                if (siteSelected != SiteViewType.SiteAll && siteSelected != item.SubscriberType) {
                    return false;
                }

                if (!scoresSelected.find(x => x == item.Score)) {
                    return false;
                }

                return true;
            });

            BindEffectFilterDependOn(indexFilterSelected, dependOnFilter);

            CheckChangeFilterByAssociationDetection();
        }
    }

    function BindEffectFilterDependOn(indexFilterSelected, dependOnFilter) {
        let aGrItems = [];
        let reasonItems = [];
        let currencyItems = [];

        switch (indexFilterSelected) {
            case IndexFilterSelected.Reason:
                aGrItems = DistinctByAGr(dependOnFilter);
                RefreshAGr(aGrItems);
                break;
            case IndexFilterSelected.Currency:
                reasonItems = DistinctByReason(dependOnFilter);
                BuildReasonGroup(reasonItems);

                RefreshReason(reasonItems);
                
                aGrItems = DistinctByAGr(dependOnFilter);
                RefreshAGr(aGrItems);
                break;
            default:
                currencyItems = DistinctByCurrency(dependOnFilter);
                let isRefreshDependCurrency = RefreshCurrency(currencyItems);

                if (!isRefreshDependCurrency) {
                    reasonItems = DistinctByReason(dependOnFilter);
                    BuildReasonGroup(reasonItems);

                    RefreshReason(reasonItems);                    

                    aGrItems = DistinctByAGr(dependOnFilter);
                    RefreshAGr(aGrItems);
                }

                break;
        }
    }

    function DistinctByCurrency(dependOnFilter) {
        let distinct = new Set();
        let newObj = dependOnFilter.filter(obj => {
            if (!distinct.has(obj.CurrencyId)) {
                distinct.add(obj.CurrencyId);
                return true;
            }
            return false;
        }).map(({ CurrencyId, CurrencyName }) => ({ CurrencyId, CurrencyName }));

        return newObj.sort();
    }

    function DistinctByReason(dependOnFilter) {
        let distinct = new Set();
        let reasonItems = [];
        dependOnFilter.filter(obj => {
            if (!distinct.has(obj.ReasonItem)) {
                distinct.add(obj.ReasonItem);
                return true;
            }
            return false;
        }).map(function (reason) {
            if (reason.ReasonItem) {
                reason.ReasonItem.split(',').forEach(x => {
                    if (reasonItems.findIndex((obj) => obj == x) == -1)
                        reasonItems.push(x);
                })
            }
        });

        return reasonItems.sort();
    }

    function DistinctByAGr(dependOnFilter) {
        let distinct = new Set();
        let aGrItems = [];
        let isBlank = false;

        dependOnFilter.filter(obj => {
            if (!distinct.has(obj.ADetectGroupID)) {
                distinct.add(obj.ADetectGroupID);
                return true;
            }
            return false;
        }).map(function (aGr) {
            if (aGrItems.findIndex((obj) => obj == aGr.ADetectGroup) == -1) {
                if (aGr.ADetectGroup == "-") {
                    isBlank = true;
                } else {
                    aGrItems.push(parseInt(aGr.ADetectGroup));
                }
            }
        });

        let aGrItemsInt = aGrItems.map(str => parseInt(str));

        aGrItemsInt.sort((a, b) => a - b);

        if (isBlank) aGrItemsInt.unshift("-");

        return aGrItemsInt;
    }

    function RefreshCurrency(currencyItems) {
        let cacheSelectedCurrency = [];
        let isRefreshDependCurrency = false;
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbCurrency) {
            cacheSelectedCurrency = cacheSelected.cbbCurrency;
        }
        domElements.cbbCurrency.multiselect().empty();
        if (currencyItems.length > 0) {
            if (cacheSelectedCurrency.length > 0) {
                currencyItems.forEach(item => {
                    if ($.inArray(item.CurrencyId.toString(), cacheSelectedCurrency) != -1) {
                        domElements.cbbCurrency.append(`<option selected value='${item.CurrencyId}'>${item.CurrencyName}</option>`);
                    } else {
                        domElements.cbbCurrency.append(`<option value='${item.CurrencyId}'>${item.CurrencyName}</option>`);
                        isRefreshDependCurrency = true;
                    }
                });
            } else {
                currencyItems.forEach(item => {
                    domElements.cbbCurrency.append(`<option selected value='${item.CurrencyId}'>${item.CurrencyName}</option>`);
                });
            }
        }
        domElements.cbbCurrency.multiselect("refresh");
        if (isRefreshDependCurrency) {
            FilterDependOnSelected(IndexFilterSelected.Currency);
        }
        return isRefreshDependCurrency;
    }

    function RefreshReasonOld(reasonItems) {
        let cacheSelectedReason = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbReason) {
            cacheSelectedReason = cacheSelected.cbbReason;
        }

        domElements.cbbReason.multiselect().empty();
        let arrReason = [];
        if (cacheDefaultReason != null) {
            arrReason = cacheDefaultReason.split(',');
        }
        if (reasonItems.length > 0) {
            if (cacheSelectedReason.length > 0) {
                reasonItems.forEach(item => {
                    if ($.inArray(item.toString(), cacheSelectedReason) != -1) {
                        domElements.cbbReason.append(`<option selected value='${item}'>${item}</option>`);
                    } else {
                        domElements.cbbReason.append(`<option value='${item}'>${item}</option>`);
                    }
                });
            } else if (arrReason.length > 0) {
                let arrReason = cacheDefaultReason.split(',');

                reasonItems.forEach(item => {
                    if ($.inArray(item.toString(), arrReason) != -1) {
                        domElements.cbbReason.append(`<option selected value='${item}'>${item}</option>`);
                    } else {
                        domElements.cbbReason.append(`<option value='${item}'>${item}</option>`);
                    }
                });

                cacheDefaultReason = null;
            } else {
                reasonItems.forEach(item => {
                    domElements.cbbReason.append(`<option selected value='${item}'>${item}</option>`);
                });
            }
        }
        domElements.cbbReason.multiselect("refresh");
    }

    function RefreshReason(reasonItems) {
        let cacheSelectedReason = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbReason) {
            cacheSelectedReason = cacheSelected.cbbReason;
        }

        //domElements.cbbReason.multiselect().empty();
        let arrReason = [];
        if (cacheDefaultReason != null) {
            arrReason = cacheDefaultReason.split(',');
        }
        if (reasonItems.length > 0) {
            if (cacheSelectedReason.length > 0) {
                reasonItems.forEach(item => {
                    if ($.inArray(item.toString(), cacheSelectedReason) != -1) {                       
                        domElements.cbbReason.find(`option[value='${item}']`).prop("selected", "selected");
                    } else {                      
                        domElements.cbbReason.find(`option[value='${item}']`).prop("selected", "");
                    }
                });
            } else if (arrReason.length > 0) {
                let arrReason = cacheDefaultReason.split(',');

                reasonItems.forEach(item => {
                    if ($.inArray(item.toString(), arrReason) != -1) {                      
                        domElements.cbbReason.find(`option[value='${item}']`).prop("selected", "selected");
                    } else {                      
                        domElements.cbbReason.find(`option[value='${item}']`).prop("selected", "");
                    }
                });

                cacheDefaultReason = null;
            } else {
                reasonItems.forEach(item => {                 
                    domElements.cbbReason.find(`option[value='${item}']`).prop("selected", "selected");
                });
            }
        }
        domElements.cbbReason.multiselect("refresh");
    }

    function BuildReasonGroup(reasonItems) {
        domElements.cbbReason.multiselect().empty();      

        domElements.cbbReasonGroupHidden.find('optgroup').each(function () {
            let optGroup = $(this);
            let groupName = optGroup.attr('label');
            let items = [];
            optGroup.find('option:selected').each(function (idx, opItem) {
                items.push($(opItem).val());
            })

            let reasonMatched = items.filter(i => reasonItems.includes(i));

            if (reasonMatched.length > 0) {
                let optgroup = `<optgroup label='${groupName}'>`;
                reasonMatched.forEach(reason => {                   
                    optgroup += `<option value='${reason}'>${reason}</option>`;
                });

                optgroup += `</optgroup>`;
                domElements.cbbReason.append(optgroup);
            }
        });

        domElements.cbbReason.multiselect("refresh");       
    }

    function RefreshAGr(aGrItems) {
        let cacheSelectedAGr = [];
        let cacheSelected = cacheFilterForImportNew[currentType];
        if (cacheSelected != undefined && cacheSelected.cbbAGroup) {
            cacheSelectedAGr = cacheSelected.cbbAGroup;
        }
        domElements.cbbAGroup.multiselect().empty();
        if (aGrItems.length > 0) {
            if (cacheSelectedAGr.length > 0) {
                aGrItems.forEach(item => {
                    if ($.inArray(item.toString(), cacheSelectedAGr) != -1) {
                        domElements.cbbAGroup.append(`<option selected value='${item}'>${item}</option>`);
                    } else {
                        domElements.cbbAGroup.append(`<option  value='${item}'>${item}</option>`);
                    }
                });
            } else {
                aGrItems.forEach(item => {
                    domElements.cbbAGroup.append(`<option selected value='${item}'>${item}</option>`);
                });
            }
        }
        domElements.cbbAGroup.multiselect("refresh");
    }

    function AddFilterLoading() {
        domElements.divFilterMonitor.addClass('loading-background');
        domElements.divFilterLoading.css('display', 'block');
    }

    function RemoveFilterLoading() {
        domElements.divFilterMonitor.removeClass('loading-background');
        domElements.divFilterLoading.css('display', 'none');
    }

    function RemoveAllFilterError() {
        domElements.cbbMatchScore.parent().find('button').removeClass('border-error');
        domElements.cbbDanger.parent().find('button').removeClass('border-error');
        domElements.cbbCurrency.parent().find('button').removeClass('border-error');
        domElements.cbbReason.parent().find('button').removeClass('border-error');
        domElements.cbbAGroup.parent().find('button').removeClass('border-error');
        domElements.cbbMGroup.parent().find('button').removeClass('border-error');
        domElements.cbbHDPBetTeam.parent().find('button').removeClass('border-error');
        domElements.cbbTicketsStatus.parent().find('button').removeClass('border-error');
    }

    function ResetDefault(isResetAmountRM) {
        cacheMGroup.isActive = false;
        cacheFilterDependOn.MGrList = [];
        cacheMGroup.allMGroup = {};
        cacheMGroup.selectMgroup = {};
        if (isResetAmountRM) {
            domElements.inputCustAmountRM.val('');
            domElements.iconClearUsnAmt.addClass("hide");
        }
    }

    function OnchangeFilter() {
        ResetDefault(true);
        BetTypeOnChange();
    }

    function ResetTranstime() {
        domElements.inputTranstimeFrom.val('');
        domElements.inputTranstimeTo.val('');
        transtime.fromDate = undefined;
        transtime.toDate = undefined;
    }

    //#endregion Event OnChange

    //#region Event Toggle
    function ToggleTickCheckAll(isCheckAll) {
        domElements.inputOptBP.prop("checked", isCheckAll);
        domElements.inputOptDevice.prop("checked", isCheckAll);
        domElements.inputOptIP.prop("checked", isCheckAll);
        domElements.inputOptAgent.prop("checked", isCheckAll);
        domElements.inputOpt3MatchesLast7Days.prop("checked", isCheckAll);
        domElements.inputOptIPLast3Days.prop("checked", isCheckAll);
    }

    function ToggleGridSelectAll(input, table) {
        var nodes = table.rows({ 'search': 'applied' }).nodes();

        $('input[type="checkbox"][name="chkbx"]:not(:hidden)', nodes).prop('checked', input.is(":checked"));
    }

    function ToggleBetID(tblElement, isExpand) {
        let nextRowIdx = 0;
        let arrDataExpand = [];
       
        tblElement.DataTable().rows().every(function (rowIdx, tableLoop, row) {
            let data = this.data(); 

            if (data.IsParlay == undefined) {                
                data.ViewMoreInfo = isExpand;
               
                if (isExpand) {
                    let index = rowIdx == 0 ? rowIdx : (nextRowIdx + rowIdx);

                    arrDataExpand.push({ ParlayTickets: data.ParlayTickets, rowIdx: index });

                    nextRowIdx = nextRowIdx + (data.IsParlay ? 0 : data.Combinition);
                } else {
                    CollapseParlayTicket(rowIdx);
                }
            }
        });
        if (arrDataExpand.length > 0) {
            $.each(arrDataExpand, function (index, value) {
                ExpandParlayTicket(value.ParlayTickets, value.rowIdx);
            });
        }
       
    }

    function ToggleMapXYY(isBetTypeChange) {
        let betidpattern = GetAttributeBetTypeSelected('data-betidpattern');

        if (domElements.sportType != 2 && betidpattern.includes('X')) {
            $("option[value='0']", domElements.cbbMapX).remove();
            domElements.cbbMapX.prop('disabled', false);
            if (isBetTypeChange) {
                domElements.cbbMapX.val('1').trigger('change');
            }
        } else {
            domElements.cbbMapX.prepend("<option value='0' selected='selected'>-</option>");
            domElements.cbbMapX.prop('disabled', true);
        }

        if (domElements.sportType != 2 && betidpattern != undefined && betidpattern.includes('YY')) {
            $("option[value='0']", domElements.cbbMapYY).remove();
            domElements.cbbMapYY.prop('disabled', false);
            if (isBetTypeChange) {
                domElements.cbbMapYY.val('1').trigger('change');
            }
        } else {
            domElements.cbbMapYY.prepend("<option value='0' selected='selected'>-</option>");
            domElements.cbbMapYY.prop('disabled', true);
        }
    }

    //#endregion Event Toggle

    //#region TranstimeFilter
    function AutoFillTranstimeFromDataGrid() {
        var rangeTransTime = GetRangeTransTime();
        if (rangeTransTime[0] && rangeTransTime[1]) {
            domElements.inputTranstimeFrom.val(rangeTransTime[0]);
            domElements.inputTranstimeTo.val(rangeTransTime[1]);

            domElements.iconTranstimeSubmit.addClass('transtime-icon-forcus');
        }
        else {
            common.showError("Please select at least 1 record to proceed!")
        }
    }

    function SetupTranstimeDateRange() {
        if (domElements.inputTranstimeFrom.val() || domElements.inputTranstimeTo.val()) {
            var transtimeFrom = GetDateFromInputTranstime(domElements.inputTranstimeFrom.val().trim());
            var transtimeTo = GetDateFromInputTranstime(domElements.inputTranstimeTo.val().trim());

            if (transtimeFrom.isException || transtimeTo.isException) {
                common.showConfirmation('Convert to the correct Transtime format (dd/mm/yy hh:mm:ss) ?',
                    function () {
                        transtimeFrom = transtimeFrom.isException ? ConvertDateFromInputTranstime(domElements.inputTranstimeFrom.val().trim()) : transtimeFrom;
                        transtimeTo = transtimeTo.isException ? ConvertDateFromInputTranstime(domElements.inputTranstimeTo.val().trim()) : transtimeTo;
                        SetDataFromInputTranstime(transtimeFrom, transtimeTo);
                    });
            }

            SetDataFromInputTranstime(transtimeFrom, transtimeTo);
        }

        if (domElements.inputTranstimeFrom.val().length === 0 &&
            domElements.inputTranstimeTo.val().length === 0) {
            transtime.fromDate = undefined;
            transtime.toDate = undefined;

            FetchFilterMMDetailByTranstime();
        }
    }

    function SetDataFromInputTranstime(transtimeFrom, transtimeTo) {
        if (transtimeFrom.date <= transtimeTo.date) {
            transtime.fromDate = transtimeFrom.date;
            transtime.toDate = transtimeTo.date;
            domElements.inputTranstimeFrom.val(transtimeFrom.dateToString);
            domElements.inputTranstimeTo.val(transtimeTo.dateToString);

            FetchFilterMMDetailByTranstime();
        }
        else {
            domElements.boderTranstimeInput.addClass('info-input-error');
        }
    }

    function FetchFilterMMDetailByTranstime() {
        ResetDefault(false);
        ResetDefaulCbbSite();

        domElements.boderTranstimeInput.removeClass('info-input-error');

        isImportNewTicket = true;

        FetchFilterMatchMonitorDetail();

        domElements.btnVerifyAndClose.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

        domElements.btnReasonFeedback.prop('disabled', currentType == MatchMonitorDetailViewType.TicketAll);

        RemoveFilterLoading();
    }

    function GetDateFromInputTranstime(dateStr) {
        var result = {
            date: undefined,
            dateToString: undefined,
            isException: undefined
        }

        if (dateStr)
            try {
                if (regexValidTranstime.test(dateStr)) {
                    const [dateValues, timeValues] = dateStr.split(' ');
                    const [day, month, year] = dateValues.split('/');
                    const [hours, minutes, seconds] = timeValues.split(':');

                    result.date = new Date(
                        Date.UTC(
                            +year + 2000,
                            +month - 1,
                            +day,
                            +hours,
                            +minutes,
                            +seconds,
                        )
                    );

                    result.dateToString = dateStr;
                }
                else {
                    throw new Error();
                }
            } catch {
                result.isException = true;
            }

        return result;

    }

    function ConvertDateFromInputTranstime(dateStr) {
        var result = {
            date: undefined,
            dateToString: undefined,
            isException: undefined
        }

        try {
            const date = new Date(dateStr);
            result.date = new Date(Date.UTC(
                date.getFullYear(),
                date.getMonth(),
                date.getDate(),
                date.getHours(),
                date.getMinutes(),
                date.getSeconds()
            ));
            result.dateToString = `${date.getDate().toString().padStart(2, '0')}/` +
                `${(+date.getMonth() + 1).toString().padStart(2, '0')}/` +
                `${date.getFullYear().toString().slice(2, 4)} ` +
                `${date.getHours().toString().padStart(2, '0')}:` +
                `${date.getMinutes().toString().padStart(2, '0')}:` +
                `${date.getSeconds().toString().padStart(2, '0')}`;
        } catch { }

        return result;
    }

    //#endregion TranstimeFilter

    function Resize() {
        domElements.groupTable.width($(window).width() - 38);

        let detailpageHeight = domElements.detailpage.outerHeight();
        let breadcrumbButtonHeight = domElements.breadcrumbButton.outerHeight();

        let breadcrumbButtonTop = domElements.detailpage.offset().top + domElements.detailpage.outerHeight();
        let groupTableTop = breadcrumbButtonTop + domElements.breadcrumbButton.outerHeight();

        domElements.breadcrumbButton.css({ top: breadcrumbButtonTop });
        domElements.groupTable.css('top', groupTableTop);

        domElements.groupTable.height($(window).height() - detailpageHeight - breadcrumbButtonHeight - 77);
    }

    //#region Hold Key
    function UpdateHoldKeyStatus(matchId, matchSection, sportType, isHold) {
        common.showConfirmation('<div class="popup - content"> Are you sure to turn ' + (isHold ? 'on' : 'off')+ ' the Hold Key?</div>', function () { 
            $.ajax(common.modifyRelativeUrl('MatchMonitor/UpdateHoldKeyStatus'), {
                type: 'POST',
                data:
                {
                    MatchId: matchId,
                    MatchSection: matchSection,
                    SportType: sportType,
                    IsHold: isHold,
                    IsCurrentStatus: domElements.tggHoldKey.is('.toggle-on'),
                    Username: domElements.ttHoldKey.text()
                },
                beforeSend: function () {
                    domElements.divPageLoading.addClass('loading-icon');
                },
                success: function (response) {
                    if (response.data.HoldKeyStatus.IsHold) {
                        domElements.tggHoldKey.addClass('toggle-on');
                        domElements.ttHoldKey.addClass('tooltiptext');
                        domElements.ttHoldKey.text(response.data.HoldKeyStatus.UserName);
                    }
                    else {
                        domElements.tggHoldKey.removeClass('toggle-on');
                        domElements.ttHoldKey.removeClass('tooltiptext');
                        domElements.ttHoldKey.text("");
                    }

                    if (!response.data.IsSuccess) {
                        common.showError((isHold ? "Turn on" : "Turn off") + " Hold Key failed.");
                        console.log(response.data.Message);
                    }
                    else {
                        common.showSuccess((isHold ? "Turn on" : "Turn off") + " Hold Key successfully.");
                    }
                },
                complete: function () {
                    domElements.divPageLoading.removeClass('loading-icon');
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    domElements.divPageLoading.removeClass('loading-icon');
                }
            });
        });
    }
    //#endregion Hold Key

    //#region Ticket Voiding
    function RedirectToTicketVoiding() {
        let tranIds = [];

        if (isDisplaySingleGrid) {
            domElements.tblMMSG.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    tranIds.push(data.BetID);
                }
            });
        } else {
            domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    tranIds.push(data.BetID);
                }
            });
            domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    tranIds.push(data.BetID);
                }
            });
        }

        if (tranIds.length > 2000) {
            common.showError("Exceeded max 2000 Tickets.Please try again with a smaller range.");
        } else if (tranIds.length == 0) {
            common.showError("Please select at least 1 record to proceed.");
        } else {

            $.ajax(common.modifyRelativeUrl('MatchMonitor/RedirectToTicketVoiding'), {
                type: 'POST',
                data: {
                    tranIds: tranIds.join()
                },
                beforeSend: function () {
                    domElements.divPageLoading.addClass('loading-icon');
                },
                success: function (response) {
                    if (response.isValid) {
                        OpenWindowWithPost(response.data.TVSRedirectURL, response.data);
                    }
                },
                complete: function () {
                    domElements.divPageLoading.removeClass('loading-icon');
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    domElements.divPageLoading.removeClass('loading-icon');
                }
            });
        }
    }

    function RedirectToMarkPA() {
        let userNames = [];

        if (isDisplaySingleGrid) {
            domElements.tblMMSG.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    userNames.push(data.Username);
                }
            });
        } else {
            domElements.tblMMHomeTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    userNames.push(data.Username);
                }
            });
            domElements.tblMMAwayTeam.DataTable().rows(function (idx, data, node) {
                if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                    userNames.push(data.Username);
                }
            });
        }

        userNames = userNames.filter(OnlyUnique);
        
        if (IsValidMarkPA(userNames.length))
        {
            const data = {
                "usernames" : userNames.join()
            };

            $.ajax(common.modifyRelativeUrl('MatchMonitor/MarkPA'), {
                type: 'POST',
                data: data,
                beforeSend: function () {
                    domElements.divPageLoading.addClass('loading-icon');
                },
                success: function (response) {
                    if (response.isValid) {
                        OpenWindowWithPost(common.modifyRelativeUrl(response.data.MarkPARedirectURL), data);
                    }
                },
                complete: function () {
                    domElements.divPageLoading.removeClass('loading-icon');
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    domElements.divPageLoading.removeClass('loading-icon');
                }
            });
        }
    }

    function IsValidMarkPA(custSelectedLength) {
        if (custSelectedLength <= 0) {
            common.showError("Please select at least 1 Username to proceed!");
            return false;
        }

        if (custSelectedLength > 1001) {
            common.showError("Exceeded max 1001 usernames. Please try again with a smaller range.");
            return false;
        }

        return true;
    }

    function OpenWindowWithPost(url, data) {
        var form = document.createElement("form");
        form.target = "_blank";
        form.method = "POST";
        form.action = url;
        form.style.display = "none";

        for (var key in data) {
            var input = document.createElement("input");
            input.type = "hidden";
            input.name = key;
            input.value = data[key];
            form.appendChild(input);
        }

        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    }

    //#endregion Ticket Voiding

    function Initial() {
        SetupMultiselectFilter(domElements.cbbCC, "CC");
        SetupMultiselectFilter(domElements.cbbDanger, "Danger");
        SetupMultiselectFilter(domElements.cbbCurrency, "Currency");
        SetupMultiselect(domElements.cbbHDPBetTeam, "HDP");
        SetupMultiselect(domElements.cbbMatchScore, "Score");
        SetupMultiselect(domElements.cbbAGroup, "A.Gr.");
        SetupMultiselect(domElements.cbbMGroup, "M.Gr.");
        SetupMultiselectGroup(domElements.cbbReason, "Reason");
        SetupMultiselect(domElements.cbbTicketsStatus, "Status");
        domElements.cbbBetType.select2();
        domElements.cbbMapX.select2();
        domElements.cbbMapYY.select2();
        domElements.inputCustAmountRM.val('');
        domElements.iconClearUsnAmt.addClass("hide");
        domElements.inputTranstimeFrom.val('');
        domElements.inputTranstimeTo.val('');

        if (domElements.mapX != "0" && domElements.sportType != 2) {
            domElements.cbbMapX.val(domElements.mapX).trigger('change');
            domElements.cbbBetType.val(`${domElements.betTypeID}_0`).trigger('change');
        } else {
            domElements.cbbBetType.val(`${domElements.betTypeID}_${domElements.betIDToString}`).trigger('change');
        }
        if (domElements.mapYY != "0") {
            domElements.cbbMapYY.val(domElements.mapYY).trigger('change');
        }

        ToggleMapXYY(false);

        domElements.cbbMarket.val(domElements.liveIndicator);

        domElements.cbbMarket.change(function () {
            ResetTranstime();
            OnchangeFilter();
        });
        domElements.cbbBetType.change(function () {
            ResetTranstime();
            ToggleMapXYY(true);

            OnchangeFilter();
        });
        domElements.cbbMapX.change(function () {
            OnchangeFilter();
        });
        domElements.cbbMapYY.change(function () {
            OnchangeFilter();
        });
        domElements.cbbMatchScore.change(function () { ScoreOnChange(); });
        domElements.cbbSites.change(function () { FilterDependOnSelected(IndexFilterSelected.Site); });
        domElements.cbbCC.change(function () { FilterDependOnSelected(IndexFilterSelected.CC); });
        domElements.cbbDanger.change(function () { FilterDependOnSelected(IndexFilterSelected.Danger); });
        domElements.cbbCurrency.change(function () { FilterDependOnSelected(IndexFilterSelected.Currency); });
        domElements.cbbReason.change(function () { FilterDependOnSelected(IndexFilterSelected.Reason); });
        domElements.cbbAGroup.change(function () { CheckChangeFilterByAssociationDetection(); });

        cacheDefaultReason = domElements.defaultReasonNames.toString();

        isSupportShowAllTickets = JSON.parse(domElements?.isSupportShowAllTickets?.toLowerCase());

        domElements.cbbTypeOfTickets.prop('disabled', !isSupportShowAllTickets);

        if (isSupportShowAllTickets) {
            domElements.cbbTypeOfTickets.change(function () {
                ResetDefault(true);

                CacheSelectDataFilter();

                TypeOfTicketsOnChange();
            });
        }

        SetupDoubleDataTable();

        SetupSingleTable();

        domElements.inputCustAmountRM.on('input', function () {
            let validAmount = domElements.inputCustAmountRM.val().replace(regexInvalidAmount, "");

            domElements.inputCustAmountRM.val(validAmount);

            if (validAmount >= 0) {
                domElements.iconClearUsnAmt.removeClass("hide");
            } else {
                domElements.iconClearUsnAmt.addClass("hide");
            }
        });

        domElements.btnImportNewTicket.on('click', function () {
            isImportNewTicket = true;
            maxSequenceId = 1;
            cacheFilter = {};
            cacheFilterScore = undefined;
            cacheMGroup.isActive = false;
            cacheMGroup.allMGroup = {};
            cacheMGroup.selectMgroup = {};

            CacheSelectFilterForImportNew();
            cacheFilterDependOn.MGrList = [];

            ResetDefaulCbbSite();

            TypeOfTicketsOnChange();

            FetchMatchMonitorDetail(currentType);
        });

        domElements.iconClearUsnAmt.on('click', function () {
            domElements.inputCustAmountRM.val('');
            domElements.iconClearUsnAmt.addClass("hide");
        })

        domElements.btnGenerateDetail.on('click', function () {
            if (!ValidateTicketStatus()) {
                return;
            }
            //if (ValidationFilter()) { // comment code for test
                FetchMatchMonitorDetail(currentType);
            //}
        })

        domElements.tickCheckAll.on('click', function () { ToggleTickCheckAll(true); });

        domElements.tickUncheckAll.on('click', function () { ToggleTickCheckAll(false); });

        domElements.btnShowPopupAssDetection.on('click', function () { ShowPopupAssDetection(); })

        domElements.btnAssDetection.on('click', function () { DetectAssociation(); })

        domElements.btnExportExcel.on('click', function () { ExportToExcel(); })

        domElements.btnVerifyAndClose.on('click', function () {
            common.showConfirmation2('<div class="popup - content">Are you sure about this?<br>Once confirmed, all tickets\' review status will be updated to "Verified".</div>', function () {
                VerifyTransMatchMonitorDetail();
            });
        });

        domElements.inputTranstimeFrom.on('input', function () {
            domElements.iconTranstimeSubmit.addClass('transtime-icon-forcus');
        });

        domElements.inputTranstimeTo.on('input', function () {
            domElements.iconTranstimeSubmit.addClass('transtime-icon-forcus');
        });

        domElements.iconTranstimeClear.on('click', function () {
            domElements.iconTranstimeSubmit.removeClass('transtime-icon-forcus');

            domElements.inputTranstimeFrom.val('');
            domElements.inputTranstimeTo.val('');

            SetupTranstimeDateRange();
        });

        domElements.iconTranstimeSubmit.on('click', function () {
            domElements.iconTranstimeSubmit.removeClass('transtime-icon-forcus');

            SetupTranstimeDateRange();
        });

        domElements.inputHomeSelectAll.prop('checked', false);
        domElements.inputAwaySelectAll.prop('checked', false);

        ChangeCbbTypeOfTickets();

        Resize();

        var reasonFeedback = new feedback();

        $('#btnReasonFeedback').on('click', function () {

            let betTypeID = GetBetTypeSelected();
            let betID = GetBetIDSelected();
            let reasonList = cacheFilterDefault.ReasonList;
            let reasonCheckedList = GetSelectDataFilter(domElements.cbbReason, true);
            let rangeTransTime = GetRangeTransTime();

            reasonFeedback.Initial(domElements.matchID, domElements.cbbMarket.val(), betTypeID, betID, domElements.sportType, reasonList, reasonCheckedList, rangeTransTime);
        });

        domElements.tggHoldKey.on('click', function () {
            UpdateHoldKeyStatus(domElements.matchID, domElements.matchSection, domElements.sportType, domElements.tggHoldKey.is('.toggle-on') == true ? false : true);
        });

        domElements.btnVoidTicket.on('click', function () {
            RedirectToTicketVoiding();
        });

        domElements.btnMarkPA.on('click', function () {
            RedirectToMarkPA();
        });
    }

    $(window).resize(function () {
        Resize();
    });

    $(document).ready(function () {
        Initial();

        domElements.btnImportNewTicket.click();
    });
}

$(MatchMonitorDetail);