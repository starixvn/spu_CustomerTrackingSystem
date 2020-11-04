function AssociationReport() {
    let dataTable;
    let domElements = {
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbAccountStatus: $('#cbbAccountStatus'),
        cbbSite: $('#cbbSite'),
        cbbAssociationStatus: $('#cbbAssociationStatus'),
        cbbEvidence: $('#cbbEvidence'),
        cbbAssociationType: $('#cbbAssociationType'),
        btnGenerate: $('#btnGenerate'),
        tblAssociation: $('#tblAssociation')
    };

    function GetAssociationReport() {
        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
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
                    SiteName: function () {
                        var selectedSites = [];
                        domElements.cbbSite.find('option:selected').each(function () { selectedSites.push($(this).text()); });

                        return selectedSites.length !== 0 ? selectedSites.join(',') : "";
                    },
                    AssociationStatus: function () {
                        let options = domElements.cbbAssociationStatus.find('option');
                        let optionSelecteds = domElements.cbbAssociationStatus.find('option:selected');

                        return options.length !== optionSelecteds.length
                            ? domElements.cbbAssociationStatus.val()
                            : 0;
                    },
                    EvidenceCode: function () { return domElements.cbbEvidence.val(); },
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
                        return html = `<a class='click-view-name'> ${obj.UserName} </a>`;
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-name', cell).click(() => {
                            console.log(row, 'click name');
                        });
                    }
                },
                { data: obj => obj.SiteName },
                { data: obj => obj.AccountStatus },
                { data: obj => obj.EvidenceCode },
                { data: obj => obj.CategoryNames },
                {
                    data: obj => () => {
                        let paramTemplate = "SiteId={0}&CTSCustId={1}&UserName={2}&FromDate={3}&ToDate={4}&AssociationStatus={5}&AssociationType={6}&AssociationTypeName={7}";
                        let associationTypeName = "";                  
                        domElements.cbbAssociationType.find('option:selected').each(function () { associationTypeName = $(this).text(); });

                        let statusOptions = domElements.cbbAssociationStatus.find('option');
                        let statusOptionSelecteds = domElements.cbbAssociationStatus.find('option:selected');
                        let associationStatus = statusOptions.length !== statusOptionSelecteds.length
                            ? domElements.cbbAssociationStatus.val()
                            : 0;
                        
                        let params = paramTemplate.format(obj.SiteId,
                            obj.CTSCustId, obj.UserName,
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
            .datepicker("option", "maxDate", moment(fromDateValue).add(2, "day").toDate());
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

        domElements.cbbEvidence.select2();
        domElements.cbbSite.select2();
        domElements.cbbAssociationStatus.multiselect({
            menuWidth: 250,
            menuHeight: 250,
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