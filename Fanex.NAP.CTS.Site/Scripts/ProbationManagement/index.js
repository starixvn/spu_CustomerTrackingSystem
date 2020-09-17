function ProbationManagement() {

    let dataTable;

    let domElements = {
        probationManagament: $('#probation-managament'),
        btnIconEdit: $('.sub-icon.icon-edit'),
    };

    function RenderGrid() {

        return domElements.probationManagament.DataTable({
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
            lengthMenu: [ 20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], className: "text-center" },
                { targets: [0], width: "4%" },
                { targets: [1], width: "9%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "9%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "9%" },
                { targets: [6], width: "9%" },
                { targets: [7], width: "9%" },
                { targets: [8], width: "9%" },
                { targets: [9], width: "4%" },

            ],
            data: [
                {
                    "No": "1",
                    "Username": "Luffy",
                    "Site" : "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date" : "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "2",
                    "Username": "Zoro",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "3",
                    "Username": "Sanji",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "4",
                    "Username": "Nami",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "5",
                    "Username": "Robbin",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "6",
                    "Username": "Chopper",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "7",
                    "Username": "Usup",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "8",
                    "Username": "Franky",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
                {
                    "No": "9",
                    "Username": "Brook",
                    "Site": "12Bet",
                    "Sport": "Vano",
                    "Target_Category": "Smart Punter",
                    "Tagged_Date": "2020",
                    "First_Margin": "0.5",
                    "Current_margin": 1,
                    "Probation_Day_Left": 30
                },
            ],
            columns: [
                { data: obj => obj.No },
                { data: obj => obj.Username },
                { data: obj => obj.Site },
                { data: obj => obj.Sport },
                { data: obj => obj.Target_Category },
                { data: obj => obj.Tagged_Date },
                { data: obj => obj.First_Margin },
                { data: obj => obj.Current_margin },
                { data: obj => obj.Probation_Day_Left },
                {
                    data: obj => () => {
                        return html = '<span class="sub-icon delete test" id="sub-icon " title="Remove Account"><i class="icon-edit ' + ('') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        $('.icon-edit', cell).click(() => {
                            //console.log('1111');
                        });
                    }
                }
            ]
        });
    }

    function LoadGrid() {
        $(this).prop('disabled', true);
        dataTable = RenderGrid();
    }
    

    function Initial() {
        $("#tabs-probation-detail").tabs();
        LoadGrid();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProbationManagement);