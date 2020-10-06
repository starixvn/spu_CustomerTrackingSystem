function NotificationSetting() {
    let domElements = {
        ctsCustId : $("#hdCTSCustId").val(),
        notificationBody: $(".notification-body"),
        isAssociationPermission: $('#hdIsAssociationPermission'),
        isProbationPermission: $('#hdIsProbationPermission'),
        inputSwitchAAM: $("#inputSwitchAAM"),
        inputSwitchProbation: $("#inputSwitchProbation")
    };

    let NS_CONST = "-NotificationSetting";

    let notificationPermissions = {
        associationPermission: { isTurnOn: true },
        probationPermission: { isTurnOn: true }
    };

    function SwitchPopup() {
        domElements.notificationBody.on("change", "input[type=checkbox]", function (e) {
            let funcID = ($(this)[0].id === "inputSwitchAAM") ? 1 : 2;
            let funcName = (funcID === 1) ? "Associated Account Monitor" : "Probation Management";

            if ($(this).is(":checked")) {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will receive new notification from <span>" + funcName + "</span> function</div>",
                    () => {
                        UpdateTempPermission(funcID, true); 
                    }, () => {
                       return;
                });
            } else {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will not receive any new notification from <span>" + funcName + "</span> function</div>",
                    () => {
                        UpdateTempPermission(funcID, false);
                    }, () => {
                        return;
                    });
            }
        });
    }

    function UpdateTempPermission(funcID, isTurnOnPermission)
    {
        //TODO: call controller to update message + notification
        UpdateNotificationSettingStatus(funcID, isTurnOnPermission);

        //update temp variables
        //if (funcID === 1) {
        //    notificationPermissions.associationPermisson.isTurnOn = isTurnOnPermission;
        //}
        //else
        //    notificationPermissions.probationPermisson.isTurnOn = isTurnOnPermission;

        // storage local state
        //setStorageNotificationSettingBehavior(domElements.ctsCustId + NS_CONST, JSON.stringify(notificationPermissions));
    }

    //function getStorageNotificationSettingBehavior(key) {
    //    if (typeof Storage !== "undefined") {
    //        return localStorage.getItem(key);
    //    }
    //    return '';
    //};

    //function setStorageNotificationSettingBehavior(key, value) {
    //    if (typeof Storage !== "undefined") {
    //        localStorage.setItem(key, value);
    //    }
    //};

    function RetrieveCurrentPermissions() {
        //Retrieve from local storage
        //let retrievedObj = getStorageNotificationSettingBehavior(domElements.ctsCustId + NS_CONST);

        //if (retrievedObj !== undefined) {
        //    notificationPermissions = JSON.parse(retrievedObj);
        //}

        ///TODO: Retrieve from API  && display permission setting status
        GetPermissionSettingStatus();
    }

    function GetPermissionSettingStatus()
    {
        $.ajax(common.modifyRelativeUrl('ManageSubscriber/GetPermissionSettingStatus'), {
            type: 'GET',
            success: function (response) {
                DisplayPermissionStatus(response);
                //SaveTempPermissionStatus();
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function UpdateNotificationSettingStatus(funcId, isTurnOnPermission) {
        let notifiedSettingStatusModel = {
            FuncId: funcId,
            IsTurnOnPermission: isTurnOnPermission
        };

        $.ajax(common.modifyRelativeUrl('NotificationSetting/UpdateNotificationSettingStatus'), {
            type: 'POST',
            data: notifiedSettingStatusModel,
            success: function (response) {
                if (response.ErrorCode !== 0) {
                    //SaveTempPermissionStatus();
                    common.showError(response.ErrorMsg);
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function DisplayPermissionStatus(response)
    {
        domElements.inputSwitchAAM.prop('checked', response.IsTurnOnAssociationPermission);
        domElements.inputSwitchProbation.prop('checked', response.IsTurnOnProbationPermission);
    }

    //function SaveTempPermissionStatus()
    //{
    //    notificationPermissions.associationPermisson.isTurnOn = response.IsTurnOnAssociationPermission;
    //    notificationPermissions.associationPermisson.isTurnOn = response.IsTurnOnProbationPermission;
    //}

    function Initial() {
        SwitchPopup();
        RetrieveCurrentPermissions();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(NotificationSetting);