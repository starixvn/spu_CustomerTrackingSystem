function ManageSubscriber() {
    let dataTable;

    let domElements = {
        btnAdd: $('#btnAdd'),
        btnSave: $('#btnSave'),
        btnCancel: $('#btnCancel'),
        manageSubscriberTable: $('#manageSubscriberTable'),
        btnIconDelete: $('.sub-icon.-delete'),
        divAddSubscriber: $('#divAddSubscriber'),
    };

    function RenderGrid() {

        return domElements.manageSubscriberTable.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "simple",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "",
                    last: "",
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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], className: "text-center" },
                { targets: [0], width: "2%" },
                { targets: [1], width: "9%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "9%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "9%" },
                { targets: [6], width: "9%" },
                { targets: [7], width: "9%" },
                { targets: [8], width: "9%" },
                { targets: [9], width: "9%" },
                { targets: [10], width: "10%" }
            ],
            data: [
                {
                    "no": "1",
                    "sub": "123",
                    "subscriber_name": "Vano",
                    "IBCUserName_Prefix": "Vano$",
                    "subscriber_group": "Credit Site",
                    "role": 1,
                    "site_id": 239,
                    "site": "NovaCash",
                    "IsActive": "NovaCash",
                    "Release": 1,
                },
                {
                    "no": "2",
                    "sub": "123",
                    "subscriber_name": "Vano",
                    "IBCUserName_Prefix": "Vano$",
                    "subscriber_group": "Credit Site",
                    "role": 1,
                    "site_id": 239,
                    "site": "NovaCash",
                    "IsActive": "NovaCash",
                    "Release": 1
                },
                {
                    "no": "3",
                    "sub": "123",
                    "subscriber_name": "Vano",
                    "IBCUserName_Prefix": "Vano$",
                    "subscriber_group": "Credit Site",
                    "role": 1,
                    "site_id": 239,
                    "site": "NovaCash",
                    "IsActive": "NovaCash",
                    "Release": 1
                },
            ],
            columns: [
                { data: obj => obj.no },
                { data: obj => obj.sub },
                { data: obj => obj.subscriber_name },
                { data: obj => obj.IBCUserName_Prefix },
                { data: obj => obj.subscriber_group },
                { data: obj => obj.role },
                { data: obj => obj.site_id },
                { data: obj => obj.site },
                { data: obj => obj.IsActive },
                { data: obj => obj.Release },
                {
                    data: obj => () => {
                        return html = '<span class="sub-icon delete test" id="sub-icon " title="Remove Account"><i class="icon-delete ' + ('') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        console.log('aaaaaa');
                        $('.icon-delete', cell).click(() => {
                            $('td #sub-icon').addClass('none-pointer');
                            common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, this account will not automatically update by our service in the future </div>", function () {
                                console.log("remove");
                            }, () => {
                                $('td #sub-icon').removeClass('none-pointer');
                            });
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
        LoadGrid();

        domElements.btnAdd.on('click', function () {
            domElements.divAddSubscriber.dialog({
                modal: true,
                width: '800px',
                title: 'ADD Subscriber',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });
        });

        domElements.btnCancel.on('click', () => {
            domElements.divAddSubscriber.dialog('close');
        });

    }

    $(document).ready(function () {
        Initial();
    });
}

$(ManageSubscriber);