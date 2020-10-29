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
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('AssociationReport/GetAssociations'),
                data: function () {
                    var paramsFilter = {};
                    paramsFilter.FromDate = domElements.dpkFromDate.val();
                    paramsFilter.ToDate = domElements.dpkToDate.val();
                    paramsFilter.AccountStatusIds = "1";
                    paramsFilter.SiteId = 1;
                    paramsFilter.AssociationStatuses = "1";
                    paramsFilter.EvidenceId = 1;
                    paramsFilter.AssociationType = 1;

                    return paramsFilter;
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
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" }
            ],            
            columns: [
                { data: obj => obj.UserName },
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
                        return html = '<a class="click-view-details" href="AssociationReport/Detail/' + obj.CTSCustId +'" target="_blank">View</a>';
                    }
                }
            ]
        });
    }
  
    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",              
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date());

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",                
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date());

        domElements.cbbEvidence.select2();
        domElements.cbbSite.select2();

        domElements.btnGenerate.on('click', function () { GetAssociationReport(); });
    }

    $(document).ready(function () {
        Initial();
    });
 
}

$(AssociationReport);