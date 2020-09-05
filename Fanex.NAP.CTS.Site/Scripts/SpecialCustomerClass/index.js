function SpecialCustomerClass() {
    let dataTable;
    
    let domElements = {       
        specialCustomerClassTable: $('#tblSpecialCustomerClass')       
    };

    function GenerateGrid() {
        
        return domElements.specialCustomerClassTable.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('/SpecialCustomerClass/GetCustomerClass'),                
                type: "GET",      
                dataFilter: function (response) {
                    console.log(response);
                    return response;
                },
                complete: function () {                    
                },
                error: function (error) {                    
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
                info: "Displaying _START_ to _END_ of _TOTAL_ item (s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,            
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [2], className: "text-right" },
                { targets: [0, 1], className: "text-left" },
                { targets: [3, 4], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "20%" },
                { targets: [3], width: "20%" },
                { targets: [4], width: "20%" }                
            ],
            columns: [               
                { data: obj => obj.Username },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.CreatedBy },
                { data: obj => obj.CreatedDate }
            ]
        });
    }

    function AddCustomerClass() {
        if (username.val() == "") {
            common.showError('Username "' + username.val() + '" is invalid.');

            return;
        }

        var url = common.modifyRelativeUrl('/SpecialCustomerClass/AddCustomerClass');
        var params = {};
        params.UserName = username.val();
        params.custClass = custClass.val();
        params.isAffectToAss = btnIsAffectToAssociation.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                //if (response.errorCode == 0) {
                //    common.showSuccess('Added exception successfully!');
                //    divException.dialog('close');
                //}
                //else if (response.errorCode == 1) {
                //    common.showError('Username/Login ID "' + params.ToUserName + '" is invalid.');
                //}
                //else if (response.errorCode == 1062) {
                //    common.showError('Username/Login ID already exists in "' + params.ToUserName + '" exceptions list.');
                //}
                //else {
                //    common.showError('Added exception fail!');
                //}
                username.val("");
                custClass.val("");

            },
            complete: function () {
                //GetException();
                //var accountEvidence = new account_evidence();
                //accountEvidence.ReloadAccountEvidence(_ctsCustId, _rootUsername);
            }
        });
    }  

    function LoadGrid() {
        $(this).prop('disabled', true);

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
    }

    function Initial() {
        LoadGrid();
    };

    $(document).ready(function () {
        Initial();
    });
}

$(SpecialCustomerClass);