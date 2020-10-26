function AssociationReport() {
    let domElements = {
        tblGeneral: $('#tblGeneral'),
        dpkDate: $('#dpkDate'),
    };


    

    function LoadGirdAssociationReport() {
        return domElements.tblGeneral.DataTable({
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
                { targets: [0], width: "9%" },
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
                    "Associated_accounts" : "View"
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
                        return html = '<a class="click-view-details">View</a>';
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            console.log( row, 'click view');
                        });
                    }
                }
            ],  
        });
    }

    function Initial() {
        LoadGirdAssociationReport();
    }


    $(document).ready(function () {
        Initial();
    });
 
}


$(AssociationReport);