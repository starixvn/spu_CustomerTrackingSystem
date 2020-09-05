function SpecialCustomerClass() {
    let dataTable;  
    
    let domElements = {       
        specialCustomerClassTable: $('#tblSpecialCustomerClass'),      
        divCustomerClass: $('#divAddCustomerClass'),
        btnAddCustomerClass : $('#btnAdd'),
        btnSave: $('#btnSave'),
        btnCancel: $('#btnCancel'),       
        btnIsAffectToAssociation: $('#isAffectedToAssociation'),
        txtUsername: $('#txtUsername'),
        txtCustomerClass: $('#txtCustomerClass')
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
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,            
            scrollX: false,
            scrollCollapse: false,
            paging: false,
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
                { data: obj => obj.CreatedDateString }
            ]
        });
    }

    function ValidateInputs() {
        if (domElements.txtUsername.val() === "") {
            common.showError('Username "' + username.val() + '" is invalid.');
            return false;
        }

        let inputClass = domElements.txtCustomerClass.val();

        if (inputClass === "") {
            common.showError('Class "' + username.val() + '" is invalid.');
            return false;
        }

        let parseInputClass = parseInt(inputClass);

        if (parseInputClass === undefined) {
            common.showError('Class "' + username.val() + '" is number.');

            return false;
        }

        return true;
    }

    function AddCustomerClass() {
        if (ValidateInputs()) {
            var url = common.modifyRelativeUrl('/SpecialCustomerClass/AddCustomerClass');
            var params = {};
            params.UserName = domElements.txtUsername.val();
            params.custClass = domElements.txtCustomerClass.val();
            params.isAffectToAss = domElements.btnIsAffectToAssociation.is(':checked');

            $.ajax(url, {
                type: 'POST',
                data: params,
                success: function (response) {                   
                    common.showSuccess("Add successful");
                    domElements.txtUsername.val("");
                    domElements.txtCustomerClass.val("");
                    domElements.divCustomerClass.dialog('close');
                },
                error: function (err) {
                    console.log(err);
                }
            });
        }        
    }  

    function LoadGrid() {
        $(this).prop('disabled', true);

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
    }

    function Initial() {
        LoadGrid();

        domElements.btnAddCustomerClass.on('click', function () {
            domElements.divCustomerClass.dialog({
                modal: true,
                width: '500px',
                title: 'ADD CUSTOMER CLASS',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });
        });

        domElements.btnSave.on('click', function () {
            AddCustomerClass();
        });

        domElements.btnCancel.on('click', () => {
            domElements.divCustomerClass.dialog('close');
        });
    };

    $(document).ready(function () {
        Initial();
    });
}

$(SpecialCustomerClass);