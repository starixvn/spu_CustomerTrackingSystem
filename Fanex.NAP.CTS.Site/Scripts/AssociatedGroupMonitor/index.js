function AssociatedGroupMonitor() {
    let dataTable;    
    let domElements = {
        btnAddGroup: $('#btnAddGroup'),
        tblAssociatedGroup: $('tblAssociatedGroup'),
        inputTagUsername: $('#inputTagUsername'),
        divAddAssociatedGroup: $('#divAddAssociatedGroup'),
        txtCommentAssociatedAccount: $('#txtCommentAssociatedAccount'),
        btnSaveAssociatedGroup: $('#btnSaveAssociatedGroup'),
        btnCancelAssociatedGroup: $('#btnCancelAssociatedGroup'),
        btnSaveAssociatedAccount: $('#btnSaveAssociatedAccount'),
        btnCancelAssociatedAccount: $('#btnCancelAssociatedAccount'),
        spanAddAssociatedGroupUserName: $('#spanAddProblemAccountUserName'),
        spanAddAssociatedGroupUserNameMaxTag: $('#spanAddProblemAccountUserNameMaxTag'),
        spanAddAssociatedGroupChangeHidden: $('#spanAddProblemAccountChangeHidden'),
        maxTags: 1001
    };

    function RenderGrid() {
        let url = common.modifyRelativeUrl('AssociatedGroupMonitor/GetAssociatedGroupMonitor');

        return domElements.tblAssociatedGroup.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            ajax: {
                url: url,               
                type: "POST",
                beforeSend: function () {
                    domElements.tblAssociatedGroup.find('tbody').addClass('loading');
                },
                complete: function () {
                    domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                }
            },
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
                { targets: [0], width: "10%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "10%" },
                { targets: [9], width: "10%" }                
            ],
            columns: [
                { data: obj => obj.GroupID },
                { data: obj => obj.GroupName },
                { data: obj => obj.ABI },
                { data: obj => obj.TotalAccount },
                { data: obj => obj.Remark },
                { data: obj => obj.CreatedDate },
                { data: obj => obj.CreatedBy },
                { data: obj => "" },
                { data: obj => "" },
                { data: obj => "" }
            ]
        });
    }

    function GetAssociatedGroup() {
        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = RenderGrid();
    }

    function InitInputTagUsername() {

    }

    function Initial() {
        GetAssociatedGroup();

        domElements.btnAddGroup.on('click', function () {
            domElements.txtComment.val('');
            
            InitInputTagUsername();

            domElements.divAddAssociatedGroup.dialog({
                modal: true,
                width: '600px',
                title: 'CREATE NEW GROUP',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });
        });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(AssociatedGroupMonitor);