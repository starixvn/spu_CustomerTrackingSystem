function ProblemAccountManagement() {
    let dataTable;

    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbSite: $('#cbbSite'),
        btnGenerate: $('#btnGenerate'),
        tblProblemAccount: $('#tblProblemAccount'),
        //btnAddUsername: $('#btnAddUsername'),
        //txtAsscomment: $('#txtasscomment'),
        //inputTagUsername: $('#inputTagUsername'),
        //btnsaveusername: $('#btnsaveusername'),
        //btncancelusername: $('#btncancelusername'),
        //btnIconDelete: $('.sub-icon.-delete'),
        //divAddUsername: $('#divAddUsername'),
        //spanAddAssociationUserName: $('#spanAddAssociationUserName'),
        //spanAddAssociationUserNameMaxTag: $('#spanAddAssociationUserNameMaxTag'),
        //spanAddAssociationChangeHidden: $('#spanAddAssociationChangeHidden'),
        //searchInput: $('#username'),
        maxTags: 50
    };

    function RenderGrid() {
        var url = common.modifyRelativeUrl('ProblemAccountManagement/GetProblemAccount');

        return domElements.tblProblemAccount.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            ajax: {
                url: url,
                data: {
                    userNames : "member17"
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblProblemAccount.find('tbody').addClass('loading');
                },
                //dataFilter: function (response) {
                //    var json = jQuery.parseJSON(response);
                //    var newJson = {};
                //    newJson.data = json.data;
                //    newJson.recordsTotal = json.recordsTotal;
                //    newJson.recordsFiltered = json.recordsFiltered;

                //    return JSON.stringify(newJson);
                //},
                complete: function () {
                    domElements.tblProblemAccount.find('tbody').removeClass('loading');
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
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "25%" },
                { targets: [5], width: "15%" }               
            ],
            columns: [                
                { data: obj => obj.UserName },
                { data: obj => obj.Site },
                { data: obj => obj.Currency },
                { data: obj => obj.Category },
                { data: obj => obj.Comment },
                { data: obj => obj.CreatedDateStr }               
            ]
        });
    }

    function GetProblemAccountReport() {
        if (ValidateRangeDate()) {
            dataTable
                ? dataTable.ajax.reload(null, true)
                : dataTable = RenderGrid();
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

        domElements.cbbSite.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Site",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.btnGenerate.on('click', function () { GetProblemAccountReport(); });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemAccountManagement);