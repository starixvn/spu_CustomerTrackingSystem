function AssociationReport() {

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
    
    function LoadGridAssociationReport() {
        return domElements.tblAssociation.DataTable({
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
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
            ],
            data: [
                {
                    "no": "1",
                    "UserName": "12Bet001",
                    "SiteName": "12Bet",
                    "Status": "Open",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "2",
                    "UserName": "12Bet001",
                    "SiteName": "12Bet",
                    "Status": "Open",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "3",
                    "UserName": "12Bet001",
                    "SiteName": "12Bet",
                    "Status": "Close",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "4",
                    "UserName": "12Bet001",
                    "SiteName": "12Bet",
                    "Status": "Close",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "5",
                    "UserName": "12Bet00156565",
                    "SiteName": "12Bet",
                    "Status": "Open",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "6",
                    "UserName": "12Bet001",
                    "SiteName": "12Bet",
                    "Status": "Open",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "7",
                    "UserName": "12Bet00100",
                    "SiteName": "12Bet",
                    "Status": "Close",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
                {
                    "no": "8",
                    "UserName": "12Bet001",
                    "SiteName": "12Bet",
                    "Status": "Close",
                    "Evidence": "6.1",
                    "General_category": "Group Betting",
                    "Associated_accounts": "View"
                },
            ],
            columns: [
                { data: obj => obj.no },
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
                { data: obj => obj.Status },
                { data: obj => obj.Evidence },
                { data: obj => obj.General_category },
                {
                    data: obj => () => {
                        return html = '<a class="click-view-details" href="AssociationReport/Detail?id=1" target="_blank">View</a>';
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

        LoadGridAssociationReport();
    }

    $(document).ready(function () {
        Initial();
    });
 
}

$(AssociationReport);