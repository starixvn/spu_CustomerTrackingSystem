let unreadMsgCounter = 0; 
let isFirstNotification = false;
let selectionCheckbox = 1;

let associatedMessages = {
    messages : null,
    msgCounter: 0,
    msgUnreadCounter: 0,
    maxAssociationId: 0
};

let probationMessages = {
    messages : null,
    msgCounter: 0,
    msgUnreadCounter: 0,
    maxProbationID: 0
};

function notification() {
    let domElements = {
        selectBoxList: $('.select-box__list'),
        selectBoxIcon: $('.select-box__icon'),
        selectBoxNotification: $('.select-box__current'),
        notificationIcon: $('#notification-icon'),
        notificationList: $('#notification-list'),
        notificationContent: $('#notification-content'),
        notificationScrollBarArea: $('#scrollBarArea'),
        notificationBell: $('#icon-bell'),
        circle: $('#circle'),
        allSelection: $('#allSelection'),
        associationSelection: $('#associationSelection'),
        probationSelection: $('#probationSelection'),
        notificationTitle: $('#notification-title'),
        allMessAmount: $('#notification-allAmount'),
        associationAmount: $('#notification-AAMAmount'),
        probationAmount: $('#notification-ProbationAmount'),
        selectionAllMessAmount: $('#selection-allAmount'),
        selectionAssociationAmount: $('#selection-AAMAmount'),
        selectionProbationAmount: $('#selection-ProbationAmount'),
        selectionCheckBox: $('#select-checkBox')
    };
    
    String.prototype.format = function () {
        var a = this;
        for (var k in arguments) {
            a = a.replace(new RegExp("\\{" + k + "\\}", 'g'), arguments[k]);
        }
        return a;
    };

    domElements.selectBoxNotification.click(() => {
        domElements.selectBoxList.toggleClass('block-notification');
        domElements.selectBoxIcon.toggleClass('show-toggle');
    });

    function GetAllMessage()
    {
        var messagesHtmlString = "";

        var htmlTemplateAssociatedRead = RenderMessageForAAM(1);
        var htmlTemplateAssociatedUnRead = RenderMessageForAAM(0);

        if (probationMessages.msgCounter != null) {
            for (var i = 0; i < probationMessages.msgCounter; i++) {
                if (probationMessages.messages[i].IsSeen) {
                    messagesHtmlString += RenderMessgaeForProbation(1, probationMessages.messages[i].NotificationType,true).format(probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                    //messagesHtmlString += RenderMessgaeForProbation(1, probationMessages.messages[i].NotificationType,false).format(probationMessages.messages[i].TotalFailed - probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                }
                else {
                    messagesHtmlString += RenderMessgaeForProbation(0, probationMessages.messages[i].NotificationType,true).format(probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                    //messagesHtmlString += RenderMessgaeForProbation(0, probationMessages.messages[i].NotificationType,false).format(probationMessages.messages[i].TotalFailed - probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                }
            }
        }

        if (associatedMessages.msgCounter != null) {
            for (var i = 0; i < associatedMessages.msgCounter; i++)
            {
                if (associatedMessages.messages[i].IsSeen) {
                    messagesHtmlString += htmlTemplateAssociatedRead.format(associatedMessages.messages[i].UserName, associatedMessages.messages[i].NewAssocitionCount, associatedMessages.messages[i].CreatedDateString, associatedMessages.messages[i].CTSCustID);
                }
                else {
                    messagesHtmlString += htmlTemplateAssociatedUnRead.format(associatedMessages.messages[i].UserName, associatedMessages.messages[i].NewAssocitionCount, associatedMessages.messages[i].CreatedDateString, associatedMessages.messages[i].CTSCustID);
                }
            }
        }

        domElements.notificationScrollBarArea.html((associatedMessages.msgCounter + probationMessages.msgCounter) > 0 ? messagesHtmlString : "<div class='no-data'>You don't have any notifications right now.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function GetAssociationMessages()
    {
        var messagesHtmlString = "";

        var htmlTemplateAssociatedRead = RenderMessageForAAM(1);
        var htmlTemplateAssociatedUnRead = RenderMessageForAAM(0);

        if (associatedMessages.msgCounter != null) {
            for (var i = 0; i < associatedMessages.msgCounter; i++) {
                if (associatedMessages.messages[i].IsSeen) {
                    messagesHtmlString += htmlTemplateAssociatedRead.format(associatedMessages.messages[i].UserName, associatedMessages.messages[i].NewAssocitionCount, associatedMessages.messages[i].CreatedDateString, associatedMessages.messages[i].CTSCustID);
                }
                else {
                    messagesHtmlString += htmlTemplateAssociatedUnRead.format(associatedMessages.messages[i].UserName, associatedMessages.messages[i].NewAssocitionCount, associatedMessages.messages[i].CreatedDateString, associatedMessages.messages[i].CTSCustID);
                }
            }
        }

        domElements.notificationScrollBarArea.html(associatedMessages.msgCounter > 0 ? messagesHtmlString : "<div class='no-data'>You don't have any notifications right now.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function GetProbationMessages() {
        var messagesHtmlString = "";

        if (probationMessages.msgCounter != null) {
            for (var i = 0; i < probationMessages.msgCounter; i++) {
                if (probationMessages.messages[i].IsSeen) {
                    messagesHtmlString += RenderMessgaeForProbation(1, probationMessages.messages[i].NotificationType, true).format(probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                    //messagesHtmlString += RenderMessgaeForProbation(1, probationMessages.messages[i].NotificationType, false).format(probationMessages.messages[i].TotalFailed - probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                }
                else {
                    messagesHtmlString += RenderMessgaeForProbation(0, probationMessages.messages[i].NotificationType, true).format(probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                    //messagesHtmlString += RenderMessgaeForProbation(0, probationMessages.messages[i].NotificationType, false).format(probationMessages.messages[i].TotalFailed - probationMessages.messages[i].TotalGeneralFailed, probationMessages.messages[i].CreatedDateString);
                }
            }
        }

        domElements.notificationScrollBarArea.html(probationMessages.msgCounter > 0 ? messagesHtmlString : "<div class='no-data'>You don't have any notifications right now.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function GetMessages()
    {
        switch (selectionCheckbox) {
            case 2:
                GetAssociationMessages();
                break;
            case 3:
                GetProbationMessages();
                break;
            default:
                GetAllMessage();
        }
    }

    function RenderMessageForAAM(isRead)
    {
        var url = common.modifyRelativeUrl('Account/Index/{3}');
        var content =
            '<a href="' + url + '" class="link-notification">'
            + ' <div class="box-notification associated' + ((isRead == 1) ? '' : ' notification-new') + '">'
            + ' <div class="line-border">'
            //+ ' <h5 class="heading-category">Associated Acccount Monitor</h5>'
            + ' <div class="des-box">'
            + ' Account <span class="color-txt">{0}</span> has <span class="color-txt">{1}</span> new associated account(s). </span >'
            + ' </div>'
            + ' <div class="time-box">{2}'
            + ' </div >'
            + ' </div>'
            + ' </div>'
            + ' </a>';
            
        return content;
    }

    function RenderMessgaeForProbation(isRead, notificationType, isGeneral)
    {
        //var url = common.modifyRelativeUrl('ProbationManagement/Index?filterType=' + notificationType);
        var content = '<div class="box-notification link-notification general' + ((isRead == 1) ? '' : ' notification-new') + '">'
                        + '<div class="line-border">'
                        + '<h5 class="heading-category"> Category </h5>'
                        + '<div class="des-box">'
                        + ((notificationType == 1) ? ' There are <span class="color-txt">{0}</span> account(s) cannot pass Probation.'
                            : ' There are <span class="color-txt">{0}</span> account(s) have Probation and negative margin.')
                        + '</div>'
                        + '<div class="time-box">{1} </div >'
                        + '</div>'
                        + '</div>';

                        //'<a href="' + url + '" class="link-notification">'
                        //'<div class="box-notification link-notification' + ((isGeneral) ? " general" : " detailed") + ((isRead == 1) ? '' : ' notification-new') + '">'
                        //+ '<div class="line-border">'
                        //+ '<h5 class="heading-category">' + ((isGeneral) ? "General Category" : "Detailed Category") + '</h5>'
                        //+ '<div class="des-box">'
                        //+ ((notificationType == 1) ? ' There are <span class="color-txt">{0}</span> account(s) cannot pass Probation.'
                        //    : ' There are <span class="color-txt">{0}</span> account(s) have Probation and negative margin.')
                        //+ '</div>'
                        //+ '<div class="time-box">{1} </div >'
                        //+ '</div>'
                        //+ '</div>'
                        //+ '</a>';

        return content;
    }

    function getUnreadMessageCount()
    {
        GetNewNotification(0,0, true);   //Default GET to load para = 0
    }

    domElements.notificationIcon.off("click");
    domElements.notificationIcon.on("click", function (event) {
        domElements.notificationBell.removeClass('bell-shake');
        domElements.circle.hide(); 
    });

    $(document).ready(function () {
        domElements.notificationIcon.on('click', function () {
            var isSkip = domElements.notificationList.is(":visible");

            GetNewNotification(associatedMessages.maxAssociationId, probationMessages.maxProbationID, isSkip);        //Get to update
            domElements.notificationList.toggle();
        });        
    });

    function GetNewNotification(maxAssociationID, maxProbationID, isSkip) {
        var url = common.modifyRelativeUrl("/NotificationSetting/GetAllMessages");

        $.ajax(url, {
            type: 'POST',
            data: {
                maxAssociationID: maxAssociationID,
                maxProbationID: maxProbationID,
                isSkipTracking : isSkip
            },
            success: function (json) {
                if (!CheckValid(json)) {
                    ReCheckin();
                } else {
                    HasNewMessages(json);
                    SetUnReadCounterToCookie(json.unReadMessageCounter);

                    associatedMessages.messages = json.messages.AssociatedMessages;
                    associatedMessages.msgCounter = json.associatedMessagesCounter;
                    associatedMessages.msgUnreadCounter = json.associatedUnreadCounter;
                    associatedMessages.maxAssociationId = json.associatedMaxAssociationID;

                    probationMessages.messages = json.messages.ProbationMessages;
                    probationMessages.msgCounter = json.probationMessagesCounter;
                    probationMessages.msgUnreadCounter = json.probationUnreadCounter;
                    probationMessages.maxProbationID = json.probationMaxProbationID;

                    GetMessages();
                    SetupBellNotification();
                    isFirstNotification = json.isFirstNotification;
                }
            },
            complete: function () {
                if (isFirstNotification == false && IsNewMessageFromCookie() == "true") {
                    NotifyToDesktop();
                    document.cookie = "hasNewUnreadMessages=false; path=/";
                }

                if (unreadMsgCounter == 0) {
                    GetMessages();
                }
                
            },
            error: function (errMsg) {
                console.log('Get notification Error, ' + errMsg);
                SetUnReadCounterToCookie(0);
                msgCounter = 0;
                messages = null;
                GetMessages();
                console.log(errMsg);
            }
        });
    }

    function SetupBellNotification()
    {
        if (unreadMsgCounter <= 0) {
            domElements.notificationBell.removeClass('bell-shake');
            domElements.circle.hide();
            domElements.notificationTitle.hide();
            domElements.allMessAmount.hide();
            domElements.associationAmount.hide();
            domElements.probationAmount.hide();
            domElements.selectionAllMessAmount.hide();
            domElements.selectionAssociationAmount.hide();
            domElements.selectionProbationAmount.hide();
        }
        else {
            if (unreadMsgCounter > 99) {
                var readTempHTML = "<span class='plus' id='plus-hide'>99+</span>";
                domElements.circle.html(readTempHTML);
                domElements.notificationTitle.text("99+");
                domElements.allMessAmount.text("99+");
                domElements.selectionAllMessAmount.text("99+");
            }
            else {
                domElements.notificationTitle.text(unreadMsgCounter);
                domElements.allMessAmount.text(unreadMsgCounter);
                domElements.circle.html(unreadMsgCounter);
                domElements.selectionAllMessAmount.text(unreadMsgCounter);
            }


            if (associatedMessages.msgUnreadCounter > 99) {
                domElements.associationAmount.text("99+");
                domElements.selectionAssociationAmount.text("99+");
            }
            else {
                domElements.associationAmount.text(associatedMessages.msgUnreadCounter);
                domElements.selectionAssociationAmount.text(associatedMessages.msgUnreadCounter);
            }

            if (probationMessages.msgUnreadCounter > 99) {
                domElements.probationAmount.text("99+");
                domElements.selectionProbationAmount.text("99+");
            }
            else {
                domElements.probationAmount.text(probationMessages.msgUnreadCounter);
                domElements.selectionProbationAmount.text(probationMessages.msgUnreadCounter);
            }

            domElements.notificationTitle.show();
            domElements.allMessAmount.show();
            domElements.selectionAllMessAmount.show();

            domElements.associationAmount.show();
            domElements.selectionAssociationAmount.show();

            domElements.probationAmount.show();
            domElements.selectionProbationAmount.show();

            domElements.circle.show();
            domElements.notificationBell.addClass('bell-shake');
        }
    }

    function NotifyToDesktop() {
        if (!window.Notification)
            console.log('Browser does not support notifications.');
        else
            GenerateMessageNotify();
    }

    function GenerateMessageNotify() {
        var mess = "";
        if (associatedMessages.msgUnreadCounter > 0) {
            mess += associatedMessages.msgUnreadCounter > 1 ? associatedMessages.msgUnreadCounter + " new associated accounts were added.\n\ " : associatedMessages.msgUnreadCounter + " new associated accounts was added.\n\ ";
        }

        if (probationMessages.msgUnreadCounter > 0) {
            mess += probationMessages.msgUnreadCounter + " new probation notification message(s).";
        }

        if (Notification.permission === 'granted') {
            new Notification('CTS Notification', {
                body: mess
            });
        } else {
            Notification.requestPermission().then(function (p) {
                if (p === 'granted') {
                    new Notification('CTS Notification', {
                        body: mess
                    });
                } else {
                    console.log('User blocked notifications.');
                }
            }).catch(function (error) {
                console.error(error);
            });
        }
    }

    setInterval(getUnreadMessageCount, 10000);

    function HasNewMessages(json)
    {
        if (json.unReadMessageCounter != GetOldUnReadMessageFromCookie()
                && json.unReadMessageCounter != 0)
        {
             document.cookie = "hasNewUnreadMessages=true; path=/";
        }
        else {
             document.cookie = "hasNewUnreadMessages=false; path=/";
        }
    }

    function IsNewMessageFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith('hasNewUnreadMessages='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith('hasNewUnreadMessages'))
                .split('=')[1];
        } 
        return "false";
    }

    function GetOldUnReadMessageFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith('unreadMessagesFromCookie='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith('unreadMessagesFromCookie'))
                .split('=')[1];
        }

        return 0;
    }

    function SetUnReadCounterToCookie(counter) {
        document.cookie = "unreadMessagesFromCookie=" + counter + "; path=/";
        unreadMsgCounter = counter;
    }

    function CheckValid(json) {
        if (json.messages === undefined
            && json.unReadMessageCounter === undefined
            && json.associatedMessagesCounter === undefined)
        {
            return false;
        }

        return true;
    }

    function ReCheckin() {
        var urlRecheck = common.modifyRelativeUrl("UserLogin/Login/ReCheckLogin");
        var urlLogout = common.modifyRelativeUrl("UserLogin/Login/LogOut");

        $.ajax({
            url: urlRecheck,
            type: "POST",
            dataType: "json",
            success: function (result) {
                if (result.IsKickedOut === "1") {
                    //alert('You have logged in from another session or your login session has expired!');
                    window.location.href = urlLogout;
                }
            },
            error: function () {
                location.reload();
            }
        });
    }

    $(document).ready(function () {
        getUnreadMessageCount();
        domElements.allSelection.on('click', function() 
        {
            GetAllMessage();
            selectionCheckbox = 1;
        });

        domElements.associationSelection.on('click', function () {
            GetAssociationMessages();
            selectionCheckbox = 2;
        });   

        domElements.probationSelection.on('click', function () {
            GetProbationMessages();
            selectionCheckbox = 3;
        });   
    });

    $(document).click(function (e) {
        if (!domElements.notificationIcon.is(e.target) && domElements.notificationIcon.has(e.target).length === 0
            && domElements.notificationList.is(":visible")
            && !domElements.selectionCheckBox.is(e.target) && domElements.selectionCheckBox.has(e.target).length === 0)
        {
            domElements.notificationBell.removeClass('bell-shake');
            domElements.circle.hide();
            GetNewNotification(associatedMessages.maxAssociationId, probationMessages.maxProbationID, true);     
            domElements.notificationList.toggle();
        }
    });
}

$(notification);
