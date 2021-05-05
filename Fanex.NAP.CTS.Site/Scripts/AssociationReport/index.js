function AssociationReport() {
    let dataTable;
    let domElements = {
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbAccountStatus: $('#cbbAccountStatus'),
        cbbSubscriber: $('#cbbSubscriber'),
        cbbAssociationStatus: $('#cbbAssociationStatus'),
        cbbEvidence: $('#cbbEvidence'),
        cbbAssociationType: $('#cbbAssociationType'),
        btnGenerate: $('#btnGenerate'),
        tblAssociation: $('#tblAssociation')
    };

    function GetAssociationReport() {
        if (ValidateRangeDate()) {
            dataTable
                ? dataTable.ajax.reload(null, true)
                : dataTable = GenerateGrid();
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
        return domElements.tblAssociation.DataTable({
            serverSide: true,
            ajax: {
                url: common.modifyRelativeUrl('AssociationReport/GetAssociations'),
                data: {
                    FromDate: function () { return domElements.dpkFromDate.val(); },
                    ToDate: function () { return domElements.dpkToDate.val(); },
                    AccountStatusIds: function () { return domElements.cbbAccountStatus.val(); },
                    SubscriberIDs: function () {
                        return domElements.cbbSubscriber.val().join();
                    },
                    AssociationStatus: function () {
                        let options = domElements.cbbAssociationStatus.find('option');
                        let optionSelecteds = domElements.cbbAssociationStatus.find('option:selected');

                        return options.length !== optionSelecteds.length
                            ? domElements.cbbAssociationStatus.val()
                            : 0;
                    },
                    EvidenceIDs: function () { return domElements.cbbEvidence.val().join(); },
                    AssociationType: function () { return domElements.cbbAssociationType.val(); }                    
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblAssociation.find('tbody').addClass('loading');
                    domElements.btnGenerate.prop('disabled', true);
                },
                dataFilter: function (response) {
                    return response;
                },
                complete: function () {
                    domElements.tblAssociation.find('tbody').removeClass('loading');
                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.tblAssociation.find('tbody').removeClass('loading');
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
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [0], width: "15%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" }
            ],            
            columns: [                
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustId);
                        return html = `<a class='click-view-name' href=${url}> ${obj.UserName} </a>`;
                    }
                },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.AccountStatus },
                { data: obj => obj.EvidenceCodes },
                { data: obj => obj.CategoryName },
                {
                    data: obj => () => {
                        let paramTemplate = "SubscriberId={0}&CTSCustId={1}&UserName={2}&FromDate={3}&ToDate={4}&AssociationStatus={5}&AssociationType={6}&AssociationTypeName={7}";
                        let associationTypeName = "";                  
                        domElements.cbbAssociationType.find('option:selected').each(function () { associationTypeName = $(this).text(); });

                        let statusOptions = domElements.cbbAssociationStatus.find('option');
                        let statusOptionSelecteds = domElements.cbbAssociationStatus.find('option:selected');
                        let associationStatus = statusOptions.length !== statusOptionSelecteds.length
                            ? domElements.cbbAssociationStatus.val()
                            : 0;
                        
                        let params = paramTemplate.format(obj.SubscriberId,
                            obj.CTSCustId, 
                            obj.UserName,
                            domElements.dpkFromDate.val(),
                            domElements.dpkToDate.val(),
                            associationStatus,
                            domElements.cbbAssociationType.val(),
                            associationTypeName);

                        return '<a class="click-view-details" href="AssociationReport/Detail?' + params + '" target="_blank">View</a>';
                    }
                }
            ]
        });
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

        domElements.cbbEvidence.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Evidence"
        });
        
        domElements.cbbSubscriber.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Subscriber",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.cbbAssociationStatus.multiselect({
            menuWidth: 200,
            menuHeight: 200,
            selectedText: "# of # selected",
            noneSelectedText: "Select Association Status"
        });

        domElements.btnGenerate.on('click', function () { GetAssociationReport(); });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(AssociationReport);