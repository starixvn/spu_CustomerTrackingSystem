function customer_category() {
    let cbbCategorySetting = $('#cbbCategorySetting');
    let btnMarkCategory = $('#btnmarkcategory');
    let btnUnmarkCategory = $('#btnunmarkcategory');    
    let divMarkCustomerCategory = $('#divMarkCustomerCategory');    
    let divUnmarkCustomerCategory = $('#divUnmarkCustomerCategory');
    let tblcustomercategory = $('#customer-category-table');
    let btnDisplayCustomerCategoryBySport = $('#btnDisplayCustomerCategoryBySport');
    let divCustomerCategoryBySport = $('#divCustomerCategoryBySport');
    let tblCustomerCategoryBySport = $('#tblCustomerCategoryBySport');
    let navsTabs = $('.nav-controls li a');
    let isHostPermission;
    let cacheData = {
        isVVip: false,
        isExcludedTagAssPA: false,
        isProblemAccount: false,
        isPAProbation: false
    };

    let dataTable;
    let dataTableCustomerCategoryBySport;

    function CustomerSettingVerticalTabs(ctsCustId) {
        navsTabs.click(function (e) {
            let $this = $(this);
            let sportID = $this.attr('data-sport-id');

            e.preventDefault();
            $('.nav-controls a.active').removeClass('active');            
            $this.addClass('active');
            $($this.attr('href')).addClass('active');

            if (sportID == -1) {                
                $('#divCategorySetting').removeClass('hide');
                $('#divMarkCategory').removeClass('hide');
                $('#divUnmarkCategory').removeClass('hide');

                LoadGrid(ctsCustId);
            } else {
                $('#divCategorySetting').addClass('hide');
                $('#divMarkCategory').addClass('hide');
                $('#divUnmarkCategory').addClass('hide');

                GetCustomerCategoryHistoryBySport(ctsCustId, sportID);
            }                        
        });
    }

    function GetCustomerCategory(custId) {
        setTimeout(function () {
            $.ajax(common.modifyRelativeUrl('Account/GetLastestCustomerClassification'), {
                type: 'POST',
                data: {
                    custId: custId
                },
                success: function (response) {
                    $('#spanClass').html(response.customerClassification + "  ");
                },
                complete: function () {
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }, 10000);
    }

    function DisplayUnmarkCustomerCategory() {        
        $.ajax(common.modifyRelativeUrl('Account/UnmarkCustomerCategory'), {
            type: 'POST',
            data: {
                roleID: $("#hdRoleID").val(),
                userName: $("#hdUsername").val(),
                isVVIP: cacheData.isVVip,
                isLicensee: $("#hdIsLicensee").val()
            },
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                divUnmarkCustomerCategory.html(response);

                $('#btnCCCancelUnmark').on('click', function () {
                    divUnmarkCustomerCategory.dialog('close');
                });

                $('#btnCCSaveUnmark').on('click', function (e) {
                    if (cacheData.isVVip) {
                        SaveUnmarkVVIP();
                    } else {
                        SaveUnmarkCustomerCategory();
                    }                    
                });               
            },
            complete: function () {
                let title = cacheData.isVVip ? 'UNMARK VVIP CONFIRMATION' : 'UNMARK PROBLEM CATEGORIES CONFIRMATION';

                divUnmarkCustomerCategory.dialog({
                    modal: true,
                    width: '600px',
                    title: title,
                    resizable: true,
                    draggable: true,
                    close: function () { $(this).dialog('close'); }
                });

                common.closeLoading();
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function SaveUnmarkCustomerCategory() {
        let params = {};
        params.CustID = $("#hdCustId").val();
        params.CTSCustID = $("#hdCTSCustId").val();
        params.RoleID = $('#hdRoleID').val();
        params.SubscriberID = $('#hdSubscriberId').val();
        params.IsExceptDirectDownline = $('#inputPAUnmarkAffectToAllDownlines').is(":checked");
        params.Remark = $('#txtCCPAUnmarkComment').val().trim();

        $.ajax(common.modifyRelativeUrl('Account/SaveUnmarkCustomerCategory'), {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Unmark Category successful!', 2000);
                    divUnmarkCustomerCategory.dialog('close');
                }
                else {
                    common.showError(response.errorMessage);
                }

                LoadGrid(params.CTSCustID, true);
                GetCustomerCategory(params.CustID);
                LoadStatusRobotUser(params.CustID);
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (error) {
                console.log(error);
                common.closeLoading();
                divUnmarkCustomerCategory.dialog('close');
            }
        });
    }

    function SaveUnmarkVVIP() {        
        let params = {};
        params.CustId = $('#hdCustId').val();
        params.CTSCustId = $('#hdCTSCustId').val();
        params.SubscriberId = $('#hdSubscriberId').val();
        params.CategoryId = 210; // VVIP
        params.RoleId = $('#hdRoleID').val();
        params.IsAffectToAllDownlines = $('#inputVVIPUnmarkAffectToAllDownlines').is(":checked");

        $.ajax(common.modifyRelativeUrl('Account/SaveUnmarkVVIP'), {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Unmark VVIP successful!', 2000);
                    divUnmarkCustomerCategory.dialog('close');                    
                }
                else {
                    common.showError(response.errorMessage);
                }

                LoadGrid(params.CTSCustId, true);
                GetCustomerCategory(params.CustId);
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (error) {
                console.log(error);
                common.closeLoading();
                divUnmarkCustomerCategory.dialog('close');
            }
        });
    }   

    function LoadGrid(ctsCustId) {
        $(this).prop('disabled', true);

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = ReloadGrid(ctsCustId);
    }

    function RenderGrid() {
        tblcustomercategory.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                zeroRecords: "There's no data",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                }
            },
            info: false,
            ordering: false,
            paging: true,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], className: "text-center", defaultContent: "-" },
                { targets: [1], className: "text-center", defaultContent: "-" },
                { targets: [2], className: "text-center", defaultContent: "-" },
                { targets: [3], className: "text-center", defaultContent: "-" },
                { targets: [4], className: "text-center", defaultContent: "-" }
            ],
            columns: [
                { data: obj => obj.CreatedTimeString },
                { data: obj => obj.CategoryName },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.Reason },
                { data: obj => obj.CreatedBy }
            ]
        });
    }

    function ReloadGrid(ctsCustId) {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerClassification'), {
            type: 'POST',
            data: {
                ctsCustID: ctsCustId,
                roleID: $("#hdRoleID").val(),
            },
            dataFilter: function (response) {
                if (response !== undefined) {
                    let json = jQuery.parseJSON(response);

                    cacheData.isProblemAccount = json.isProblemAccount;
                    cacheData.isExcludedTagAssPA = json.isExcludedTagAssPA;
                    cacheData.isVVip = json.isVVip;
                    cacheData.isPAProbation = json.isPAProbation;

                    ResetCategorySetting();

                    RenderCategoryColorSummary(json.categoryList, json.isProblemAccount);

                    //let isProblemAccount = RenderProblemCategoryProfilePage(json.categoryList, json.isPAProbation);

                    //if (!isProblemAccount) {
                    //    RenderNormalCategoryProfilePage(json.categoryList, json.isPAProbation);
                    //}
                }
                return response;
            },
            beforeSend: function () {
                tblcustomercategory.find('tbody').addClass('loading');
            },
            success: function (response) {
                tblcustomercategory.DataTable().clear().rows.add(response.data).draw(true);
            },
            complete: function () {
                tblcustomercategory.find('tbody').removeClass('loading');
                
                let disabledUnmarkCategory = (!cacheData.isVVip && !cacheData.isProblemAccount) || (!cacheData.isVVip);                             

                let disabledMarkCategory = cacheData.isVVip
                    || ($("#hdIsLicensee").val() == 'TRUE' && $("#hdRoleID").val() > 1)
                    || $("#hdIsInternal").val() == 'TRUE';
                
                btnUnmarkCategory.prop('disabled', disabledUnmarkCategory);
                btnMarkCategory.prop('disabled', disabledMarkCategory);                

                if (!disabledMarkCategory) {                    
                    $("#btnProfileInsertPA").off('click').click(function () { DisplayMarkCustomerCategory() });
                    $("#btnProfileInsertPA").removeClass('disabled');
                } else {
                    $("#btnProfileInsertPA").addClass('disabled');
                }  
            }
        });
    }

    function RenderCategoryColorSummary(data, isProblemAccount) {        
        let html = BuildButtonProfileInsertPA("", data);
        let bgHeaderUsername = '';
        let bgHeaderInfo = '';
        if (data.length > 0) {
            data.map(function (x) {
                if (isProblemAccount && x.IsProblemAccount) {
                    html = BuildCategoryToHtmlDiv(html, "circle bg-" + x.CategoryColorDesc, x.CategoryName);
                } else if (!isProblemAccount) {
                    html = BuildCategoryToHtmlDiv(html, "circle bg-" + x.CategoryColorDesc, x.CategoryName);
                }

                bgHeaderUsername = 'bg-' + x.CategoryColorDesc;
                bgHeaderInfo = 'border-' + x.CategoryColorDesc;
            });
        } else {            
            bgHeaderUsername = 'bg-green';
            bgHeaderInfo = 'border-green';
        }        

        $('#divHeaderUsername').removeClass('bg-yellow').removeClass('bg-green')
            .removeClass('bg-green-line').removeClass('bg-yellow-line').removeClass('bg-red')
            .addClass(bgHeaderUsername);

        $('#divHeaderInfo').removeClass('border-yellow').removeClass('border-green').removeClass('border-red')
            .addClass(bgHeaderInfo);

        $('#divCustomerCategorySummary').html(html);
        $('#spanContentInfoLoading').css('visibility', 'hidden');
        $('#divHeaderInfo').css('visibility', 'visible');
    }
 
    function BuildButtonProfileInsertPA(html) {
        if (isHostPermission) {
            html += "<div class='btn-add-pa' id='btnProfileInsertPA' title='Mark Category'></div>";
        }
                
        return html;
    }

    function DisplayMarkCustomerCategory() {
        $.ajax(common.modifyRelativeUrl('Account/MarkCustomerCategory'), {
            type: 'POST',
            data: {
                ctsCustID: $("#hdCTSCustId").val(),
                roleID: $("#hdRoleID").val(),
                userName: $("#hdUsername").val(),
                isRobotUser: $("#hdIsRobotUser").val()               
            },
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                divMarkCustomerCategory.html(response);
            },
            complete: function () {
                divMarkCustomerCategory.dialog({
                    modal: true,
                    width: '800px',
                    title: 'MARK CUSTOMER CATEGORY',
                    resizable: true,
                    draggable: true,
                    close: function () { $(this).dialog('close'); }
                });

                common.closeLoading();

                $('#btnCCMarkCancel').on('click', function () {
                    divMarkCustomerCategory.dialog('close');
                });

                $('#btnCCMarkSave').on('click', function (e) {
                    if ($('#inputCCVVIP').is(":checked")) {
                        SaveMarkVVIP();
                    } else {
                        SaveMarkCustomerCategory();
                    }                          
                });

                if ($("#hdIsLicensee").val() == "TRUE" || $("#hdRoleID").val() == 1) {
                    $('#lblCCAffectToAllDownlines').addClass('hide');
                } else {
                    $('#lblCCAffectToAllDownlines').removeClass('hide');
                }
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }  

    function SaveMarkVVIP() {
        let params = {};
        params.CustID = $("#hdCustId").val();
        params.CTSCustID = $("#hdCTSCustId").val();
        params.SubscriberID = $("#hdSubscriberId").val();
        params.RoleID = $('#hdRoleID').val();
        params.IsVVip = true;
        params.IsAffectAllDownlines = $('#inputCCAffectToAllDownlines').is(":checked");
        params.Remark = $('#txtCCComment').val();
      
        $.ajax(common.modifyRelativeUrl('Account/SaveMarkVVIP') , {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Mark Customer Category successful!', 2000);
                    divMarkCustomerCategory.dialog('close');
                }
                else {
                    common.showError(response.errorMessage);
                }

                LoadGrid(params.CTSCustID, true);
                GetCustomerCategory(params.CustID);
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (error) {
                console.log(error);
                common.closeLoading();
            }
        });
    }

    function SaveMarkCustomerCategory() {
        let params = {};
        params.CategoryID = 0;
        params.CustID = $("#hdCustId").val();
        params.CTSCustID = $("#hdCTSCustId").val();
        params.SubscriberID = $("#hdSubscriberId").val();
        params.IsLicensee = $("#hdIsLicensee").val() == "TRUE";
        params.RoleID = $('#hdRoleID').val();
        params.IsVVip = false;          
        params.Remark = $('#txtCCComment').val();

        $('input[type=radio]:checked', '.radio-button').each(function () {
            if (!$(this).attr('disabled')) {
                params.CategoryID = $(this).val();
                params.CategoryGroup = $(this).attr('data-categorygroup');
                params.IsRobot = $(this).attr('data-isrobot');
            }
        });

        if (params.CategoryID != 0) {
            let url = params.IsRobot?.toLowerCase() == 'true'
                ? common.modifyRelativeUrl('Account/SaveMarkRobotAccount')
                : common.modifyRelativeUrl('Account/SaveMarkCustomerCategory');

            $.ajax(url, {
                type: 'POST',
                data: params,
                success: function (response) {
                    if (response.errorCode === 0) {
                        common.showSuccess('Mark Customer Category successful!', 2000);
                        divMarkCustomerCategory.dialog('close');
                    }
                    else {
                        common.showError(response.errorMessage);
                    }

                    LoadGrid(params.CTSCustID, true);
                    GetCustomerCategory(params.CustID);
                    LoadStatusRobotUser(params.CustID);
                },
                beforeSend: function () {
                    common.showLoading();
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (error) {
                    console.log(error);
                    common.closeLoading();
                }
            });
        }
    }

    function LoadStatusRobotUser(custID)
    {
        $.ajax(common.modifyRelativeUrl('Account/GetStatusRobotClassification'), {
            type: 'POST',
            data: {
                CustID: custID
            },
            success: function (response) {
                switch (response.RobotType) {
                    case 0:
                        $('span.icon-big-good-robot').addClass('hide');
                        $('span.icon-big-bad-robot').addClass('hide');
                        break;
                    case 1:
                        $('span.icon-big-good-robot').removeClass('hide');
                        $('span.icon-big-bad-robot').addClass('hide');
                        break;
                    case 2:
                        $('span.icon-big-good-robot').addClass('hide');
                        $('span.icon-big-bad-robot').removeClass('hide'); 
                        break;
                    default: break;
                }                         
            },         
            error: function (error) {
                console.log(error);                
            }
        });
    }

    function BuildCategoryToHtmlDiv(html, classIconColor, categoryName) {
        html += "<div class='info-status'>";
        html += "<div class='status-left'>";
        html += "<span class='" + classIconColor + "'></span>";
        html += "<span class='text-status text-left'>" + categoryName + "</span>";
        html += "</div>";
        html += "</div>";

        return html;
    }

    function ExcludedTagAssPA(isChecked, custId, ctsCustId, subscriberId, custSubId) {
        let html = "";      
        if (isChecked) {
            html = "<div style='text-align:center'>You are not allow to turn on this setting because of Pin Category.</div>";
            html += "<div style='text-align:center'>Please contact your system administrator for further assistance.</div>";

            common.showError(html, 2000);
            ResetCategorySetting();

            return;
        } else if (isChecked) {
            html = "<div style='text-align:center'>Are you sure about this action? </div>";
            html += "<div>By selecting this option, this account will not be classified with PA</div>";
            html += "<div>Associated Category by our service in future</div>";
        } else {
            html = "<div style='text-align:center'>Are you sure about this action? </div>";
            html += "<div>By unselecting this option, this account can be classified with PA</div>";
            html += "<div>Associated Category by our service in future</div>";
        }

        common.showFormConfirmation(html, 'Confirmation',
            function () {
                var url = common.modifyRelativeUrl('/Account/ChangeExcludedTagAssPA');
                var params = {};
                params.custId = custId;
                params.ctsCustId = ctsCustId;
                params.subscriberId = subscriberId;
                params.isExcludedTagAssPA = isChecked;
                params.isProblemAccount = cacheData.isProblemAccount;
                params.custSubId = custSubId;

                $.ajax(url, {
                    type: 'POST',
                    data: params,
                    success: function (response) {
                        if (response.errorCode === 0) {
                            let message = isChecked
                                ? "Mark category successfully!"
                                : "Unmark category successfully!";

                            common.showSuccess(message);

                            LoadGrid(ctsCustId, true);
                        }
                        else {
                            common.showError('Failed! ' + response.errorMessage);
                            ResetCategorySetting();
                        }
                    },
                    beforeSend: function () {
                        common.showLoading();
                    },
                    complete: function () {
                        common.closeLoading();
                    },
                    error: function (error) {
                        common.closeLoading();
                        console.log(error);
                        ResetCategorySetting();
                    }
                });
            },
            function () {
                ResetCategorySetting();
            }
        );
    }

    function ResetCategorySetting() {
        let cbbCategorySettingSelector = cbbCategorySetting.empty();
       
        cbbCategorySettingSelector.append("<option " + (cacheData.isExcludedTagAssPA ? "selected" : "") + " value='ExcludedTagAssPA'>Normal Category</option>");

        cbbCategorySetting.multiselect("refresh");
    }

    function DisplayCustomerCategoryBySport(ctsCustId) {
        divCustomerCategoryBySport.dialog({
            modal: true,
            width: '550px',
            title: 'SPORT DETAILS CATEGORY',
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
            }
        });

        if (dataTableCustomerCategoryBySport == null) {
            dataTableCustomerCategoryBySport = tblCustomerCategoryBySport.DataTable({
                language: {
                    zeroRecords: "There's no data"
                },
                info: false,
                ordering: false,
                searching: false,
                scrollX: false,
                paging: false,
                columnDefs: [
                    { targets: "_all", defaultContent: "-" },
                    { targets: [0], width: "35%", className: "text-center" },
                    { targets: [1], width: "35%", className: "text-center" },                   
                    { targets: [2], width: "30%", className: "text-center" }                  
                ],
                columns: [
                    { data: obj => obj.SportType },
                    { data: obj => obj.CategoryName },
                    { data: obj => obj.CustomerClass }                   
                ]
            });
        }

        GetCustomerCategoryBySport(ctsCustId);
    }

    function GetCustomerCategoryBySport(ctsCustId) {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerCategoryBySport'), {
            type: 'POST',
            data: {
                ctsCustID: ctsCustId
            },   
            beforeSend: function () {
                tblCustomerCategoryBySport.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (response.data != undefined) {
                    tblCustomerCategoryBySport.DataTable().clear().rows.add(response.data);
                    tblCustomerCategoryBySport.DataTable().columns.adjust().draw();                    
                }
            },
            complete: function () {  
                tblCustomerCategoryBySport.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                tblCustomerCategoryBySport.find('tbody').removeClass('loading');
                console.log(errMsg);
            }
        });
    }

    function GetCustomerCategoryHistoryBySport(ctsCustId, sportId) {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerCategoryHistoryBySport'), {
            type: 'POST',
            data: {
                ctsCustID: ctsCustId,
                sportID: sportId
            },
            beforeSend: function () {
                tblcustomercategory.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (response.data != undefined) {
                    tblcustomercategory.DataTable().clear().rows.add(response.data);
                    tblcustomercategory.DataTable().columns.adjust().draw();
                }
            },
            complete: function () {
                tblcustomercategory.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                tblcustomercategory.find('tbody').removeClass('loading');
                console.log(errMsg);
            }
        });
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission) {
            cbbCategorySetting.prop('disabled', true);                 
        }
        btnMarkCategory.prop('disabled', true);
        btnUnmarkCategory.prop('disabled', true);
        $("#btnProfileInsertPA").prop('disabled', true);        
    }

    this.Initial = function (custId, ctsCustId, subscriberId, username, hostPermission, custSubId) {
        DisableButton(hostPermission);

        cbbCategorySetting.multiselect({
            menuWidth: 160,
            menuHeight: 60,
            header: false,
            click: function (_, ui) {
                if (ui.value == "ExcludedTagAssPA")
                    //PinCategory(ui.checked, ctsCustId, subscriberId);
                    ExcludedTagAssPA(ui.checked, custId, ctsCustId, subscriberId, custSubId);                 
            },
            selectedText: "Category Setting",
            noneSelectedText: "Category Setting"
        });

        if (isHostPermission) {
            btnMarkCategory.click(function () { DisplayMarkCustomerCategory(); });

            btnUnmarkCategory.click(function () { DisplayUnmarkCustomerCategory(); });
        }       

        dataTable = RenderGrid();

        LoadGrid(ctsCustId);
        CustomerSettingVerticalTabs(ctsCustId);

        btnDisplayCustomerCategoryBySport.click(function () { DisplayCustomerCategoryBySport(ctsCustId); });
    };

    this.ReloadStatusRobotUser = function (custId) {
        LoadStatusRobotUser(custId);
    }

    this.ReloadTab = function (ctsCustId) {
        ReloadGrid(ctsCustId);
    };

}