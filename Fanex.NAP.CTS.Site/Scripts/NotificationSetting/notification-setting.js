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
        associationPer: { isTurnOff: false },
        probationPer: { isTurnOff: false }
    };

    function SwitchPopup() {
        domElements.notificationBody.on("change", "input[type=checkbox]", function (e) {
            let funcID = ($(this)[0].id === "inputSwitchAAM") ? 1 : 2;
            let funcName = (funcID === 1) ? "Associated Account Monitor" : "Probation Management";

            if ($(this).is(":checked")) {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will receive new notification from <span>" + funcName + "</span> function</div>",
                    () => {
                        UpdateTempPermission(funcID, false); 
                    }, () => {
                       return;
                });
            } else {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will not receive any new notification from <span>" + funcName + "</span> function</div>",
                    () => {
                        UpdateTempPermission(funcID, true);
                    }, () => {
                        return;
                    });
            }
        });
    }

    function UpdateTempPermission(funcID, isTurnOffPermission)
    {
        //TODO: call controller to update message + notification


        //update temp variables
        if (funcID === 1)
            notificationPermissions.associationPer.isTurnOff = isTurnOffPermission;
        else
            notificationPermissions.probationPer.isTurnOff = isTurnOffPermission;

        // storage local state
        setStorageNotificationSettingBehavior(domElements.ctsCustId + NS_CONST, JSON.stringify(notificationPermissions));
    }

    function getStorageNotificationSettingBehavior(key) {
        if (typeof Storage !== "undefined") {
            return localStorage.getItem(key);
        }
        return '';
    };

    function setStorageNotificationSettingBehavior(key, value) {
        if (typeof Storage !== "undefined") {
            localStorage.setItem(key, value);
        }
    };

    function RetrieveCurrentPermissions() {
        //Retrieve from local storage
        let retrievedObj = getStorageNotificationSettingBehavior(domElements.ctsCustId + NS_CONST);

        if (retrievedObj !== undefined) {
            notificationPermissions = JSON.parse(retrievedObj);
        }
        //Retrieve from API


        //Compare permission (high priority for API), assign temp variable


        //Dislay and effect permission
        if (notificationPermissions !== undefined && notificationPermissions !== null) {
            domElements.inputSwitchAAM.prop('checked', !notificationPermissions.associationPer.isTurnOff);
            domElements.inputSwitchProbation.prop('checked', !notificationPermissions.probationPer.isTurnOff);
        }
    }

    function Initial() {
        SwitchPopup();
        RetrieveCurrentPermissions();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(NotificationSetting);