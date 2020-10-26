function AssociationReport() {
    let domElements = {
        tblGeneral: $('#tblGeneral'),
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
                { targets: [0], width: "9%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },

            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => obj.UserName },
                { data: obj => obj.SiteName },
                { data: obj => obj.SportName },
                { data: obj => obj.TargetCategoryName },
                { data: obj => obj.TaggedDate },
                { data: obj => obj.FirstMargin },
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