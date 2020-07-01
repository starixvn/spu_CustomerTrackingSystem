function AssociatedAccountMonitor() {
    let dataTable;

    let domElements = {
        cbbSubscriber: $('#cbbSubscriber'),
        btnSearch: $('#btnSearch'),
        asociatedAccountMonitorTable: $('#asociatedAccountMonitorTable'),
        btnIconDelete: $('.sub-icon.-delete'),
        divAddUsername: $('#divAddUsername'),
    };


    function GenerateGrid() {
        var smallTable = [
            {
                "id": "1",
                "username": "Kevin",
                "login-id": "123",
                "status": "Open",
                "subscriber": "Maxbwt",
                "remark": "Dangerous Ban Account",
                "add-by": "sa",
                "action": RenderAction(),
            },
            {
                "id": "2",
                "username": "Vinke",
                "login-id": "456",
                "status": "Active",
                "subscriber": "CN75",
                "remark": "Continuous Winning",
                "add-by": "sa",
                "action": RenderAction(),
            },
            {
                "id": "3",
                "username": "Venki",
                "login-id": "789",
                "status": "Close",
                "subscriber": "CN75",
                "remark": "Dangerous Ban Account",
                "add-by": "sa",
                "action": RenderAction(),
            },
            {
                "id": "4",
                "username": "Kinve",
                "login-id": "101",
                "status": "Inactive",
                "subscriber": "CN75",
                "remark": "Continuous Winning",
                "add-by": "sa",
                "action": RenderAction(),
            },
            {
                "id": "5",
                "username": "Nivek",
                "login-id": "112",
                "status": "Open",
                "subscriber": "Maxbwt",
                "remark": "Dangerous Ban Account",
                "add-by": "sa",
                "action": RenderAction(),
            },
            {
                "id": "6",
                "username": "Nevik",
                "login-id": "131",
                "status": "Active",
                "subscriber": "Maxbwt",
                "remark": "Continuous Winning",
                "add-by": "sa",
                "action": RenderAction(),
            },
        ];

        return domElements.asociatedAccountMonitorTable.DataTable({
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
                info: "Displaying _START_ to _END_ of _TOTAL_ item (s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            data: smallTable,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" },
                { targets: [0], width: "2%" },
                { targets: [1], width: "17%" },
                { targets: [2], width: "12%" },
                { targets: [3], width: "12%" },
                { targets: [4], width: "12%" },
                { targets: [5], width: "21%" },
                { targets: [6], width: "12%" },
                { targets: [7], width: "12%" }
            ],
            columns: [
                {'data':'id'},
                {'data':'username'},
                { 'data':'login-id'},
                { 'data':'status'},
                { 'data':'subscriber'},
                { 'data':'remark'},
                { 'data':'add-by'},
                {
                    'data': 'action',
                    createdCell: (cell, _, row) => {
                        $('.sub-icon.-delete', cell).click(() =>
                            common.showFormConfirmation(`<div class="popup-content">Do you want remove <strong>CN88$Kevin </strong> from Associated Account Monitor list? </div>`, 'Confirmation', function () {
                                
                            })
                        );
                    }
                },
            ]
        });
    }

    function RenderAction() {
        let html = "<span class='sub-icon -edit'><i class='icon-edit-sub'></i></span>";
        html += "<span class='sub-icon -delete'><i class='icon-delete'></i></span>";

        return html;
    }

    function LoadGrid() {
        $(this).prop('disabled', true);

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
    }

    function Initial() {
        domElements.cbbSubscriber.multiselect({
            header: ['checkAll', 'uncheckAll'],
            menuWidth: 170,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "4 of 4 selected"
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        LoadGrid();    
    }

    $(document).ready(function () {
        Initial();
        domElements.btnSearch.on('click', () => LoadGrid());
    });
}

$(AssociatedAccountMonitor);