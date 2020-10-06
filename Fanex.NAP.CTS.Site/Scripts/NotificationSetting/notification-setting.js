function NotificationSetting() {
    let domElements = {
        ctsCustId : $("#hdCTSCustId").val(),
        notificationBody: $(".notification-body"),
        isTurnOnAssociationStatus: $('#hdIsTurnOnAssociationStatus').val() === "True"? true : false,
        isTurnOnProbationStatus: $('#hdIsTurnOnProbationStatus').val() === "True" ? true : false,
        inputSwitchAAM: $("#inputSwitchAAM"),
        inputSwitchProbation: $("#inputSwitchProbation")
    };

    function SwitchPopup() {
        domElements.notificationBody.on("change", "input[type=checkbox]", function (e) {
            let funcID = ($(this)[0].id === "inputSwitchAAM") ? 1 : 2;
            let funcName = (funcID === 1) ? "Associated Account Monitor" : "Probation Management";

            if ($(this).is(":checked")) {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will receive new notification from <span>" + funcName + "</span> function</div>",
                    () => {
                        UpdateNotificationSettingStatus(funcID, true); 
                    }, () => {
                       return;
                });
            } else {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will not receive any new notification from <span>" + funcName + "</span> function</div>",
                    () => {
                        UpdateNotificationSettingStatus(funcID, false);
                    }, () => {
                        return;
                    });
            }
        });
    }

    function UpdateNotificationSettingStatus(funcId, isTurnOnPermission) {
        let notifiedSettingStatusModel = {
            FunctionId: funcId,
            IsTurningOnPermission: isTurnOnPermission
        };

        $.ajax(common.modifyRelativeUrl('NotificationSetting/UpdateNotificationSettingStatus'), {
            type: 'POST',
            data: notifiedSettingStatusModel,
            success: function (response) {
                if (response.ErrorCode !== 0) {
                    common.showError(response.ErrorMsg);
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function DisplayPermissionStatus()
    {
        domElements.inputSwitchAAM.prop('checked', domElements.isTurnOnAssociationStatus);
        domElements.inputSwitchProbation.prop('checked', domElements.isTurnOnProbationStatus);
    }

    function Initial() {
        SwitchPopup();
        DisplayPermissionStatus();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(NotificationSetting);