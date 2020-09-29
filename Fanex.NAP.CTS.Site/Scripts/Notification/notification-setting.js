function NotificationSetting() {
    let domElements = {
        notificationBody: $(".notification-body")
    };


    function SwitchPopup() {

        domElements.notificationBody.on("change", "input[type=checkbox]" , function (e) {
            if ($(this).is(":checked")) {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will recaive new notification from <span>Associated Account Monitor</span> function</div>");
            } else {
                common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure about this?</div>For confirm this action, you will not recaive any new notification from <span>Associated Account Monitor</span> function</div>");
            }
        });
    }

    function Initial() {
        SwitchPopup();
    }




    $(document).ready(function () {
        Initial();
    });
}

$(NotificationSetting);