function SpecialCustomerClass() {
    let dataTable;    

    let domElements = {       
        specialCustomerClassTable: $('#tblSpecialCustomerClass'),      
        divCustomerClass: $('#divAddCustomerClass'),
        btnAddCustomerClass: $('#btnAdd'),
        btnGenerate: $('#btnGenerate'),
        btnSave: $('#btnSave'),
        btnCancel: $('#btnCancel'),       
        btnIsAffectToAssociation: $('#isAffectedToAssociation'),
        txtUsername: $('#txtUsername'),
        txtCustomerClass: $('#txtCustomerClass'),
        txtSearchUsername: $('#txtSearchUsername'),
        hdIsHostPermission: $('#hdIsHostPermission')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";
    
    function InitGrid() {        
        return domElements.specialCustomerClassTable.DataTable({
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
                { targets: [0, 1], className: "text-left" },
                { targets: [2], className: "text-right" },
                { targets: [3, 4, 5], className: "text-center" },
                { targets: [0], width: "25%" },
                { targets: [1], width: "25%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },               
                { targets: [5], width: "10%" }
            ],
            columns: [               
                { data: obj => obj.Username },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.CreatedBy },
                { data: obj => obj.CreatedDateString },
                {
                    data: obj => () => {
                        return html = '<span class="sub-icon - delete" id="sub-icon " title="Remove Account"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';

                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                $('td #sub-icon').addClass('none-pointer');
                                let html = "<div class='popup - content'>Do you want remove <strong>" + row.Username + "</strong> from Special Customer Class list? </div>";

                                common.showConfirmation(html, function () {
                                    RemoveCustomerClass(row.CustID, row.CTSCustID);
                                }, () => {
                                    $('td #sub-icon').removeClass('none-pointer');
                                });
                            });
                        }                        
                    }
                }
            ]
        });
    }

    function RemoveCustomerClass(custID, ctsCustID) {
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/RemoveCustomerClass');
        var params = {};
        params.custID = custID;
        params.ctsCustID = ctsCustID;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess('Remove the Customer Class successful');
                    LoadGrid("");
                }
                else {
                    common.showError('Failed! ' + response.ErrorMessage);
                }
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (err) {
                console.log(err);
                common.closeLoading();
            }
        });
    }

    function ValidateInputs() {
        if (domElements.txtUsername.val() === "") {
            common.showError('Username "' + domElements.txtUsername.val() + '" is invalid.');
            return false;
        }

        let inputClass = domElements.txtCustomerClass.val();

        if (inputClass === "") {
            common.showError('Class "' + domElements.txtCustomerClass.val() + '" is invalid.');
            return false;
        }

        let parseInputClass = parseInt(inputClass);

        if (parseInputClass === undefined) {
            common.showError('Class "' + domElements.txtCustomerClass.val() + '" is number.');

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
                    if (response !== undefined) {                        
                        if (response.ErrorCode === 0) {
                            common.showSuccess("Add the Customer Class successful");
                            domElements.divCustomerClass.dialog('close');
                            LoadGrid("");
                        } else {
                            common.showError("Failed! " + response.ErrorMessage);
                        }
                    }
                },
                beforeSend: function () {
                    common.showLoading();
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (err) {
                    console.log(err);
                    common.closeLoading();
                }
            });
        }        
    }

    function LoadGrid(username) {
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/GetCustomerClass?username=' + username);

        $.ajax(url, {
            type: 'GET',            
            success: function (response) {
                domElements.specialCustomerClassTable.DataTable().clear().rows.add(response.data).draw();
            },
            beforeSend: function () {
                domElements.btnGenerate.prop('disabled', true);
                domElements.specialCustomerClassTable.find('tbody').addClass('loading');
            },
            complete: function () {
                domElements.btnGenerate.prop('disabled', false);
                domElements.specialCustomerClassTable.find('tbody').removeClass('loading');
            },
            error: function (err) {
                console.log(err);
                domElements.specialCustomerClassTable.find('tbody').removeClass('loading');                
            }
        });        
    }

    function Initial() {
        domElements.btnAddCustomerClass.prop('disabled', !isHostPermission);
        InitGrid();

        if (isHostPermission) {
            domElements.btnAddCustomerClass.on('click', function () {
                domElements.txtUsername.val("");
                domElements.txtCustomerClass.val("");
                domElements.btnIsAffectToAssociation.prop('checked', false);

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

            domElements.btnGenerate.on('click', function () {
                LoadGrid(domElements.txtSearchUsername.val());
            });

            domElements.txtSearchUsername.keypress(function (event) {
                if (event.keyCode === 13) {
                    LoadGrid(domElements.txtSearchUsername.val());
                }
            }); 

            domElements.btnSave.on('click', function () {
                AddCustomerClass();
            });

            domElements.btnCancel.on('click', () => {
                domElements.divCustomerClass.dialog('close');
            });
        }
    }

    $(document).ready(function () {
        Initial();
    });
}

$(SpecialCustomerClass);
