let unreadMsgCounter = 0; 
let isFirstNotification = false;

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
        notificationIcon: $('#notification-icon'),
        notificationList: $('#notification-list'),
        notificationContent: $('#notification-content'),
        notificationBell: $('#icon-bell'),
        circle: $('#circle')
    };
    
    String.prototype.format = function () {
        var a = this;
        for (var k in arguments) {
            a = a.replace(new RegExp("\\{" + k + "\\}", 'g'), arguments[k]);
        }
        return a;
    }

    function GetMessage()
    {
        var messagesHtmlString = "<ul>";
        var htmlTemplateAssociatedRead = RenderMessageForAAM(1);
        var htmlTemplateAssociatedUnRead = RenderMessageForAAM(0);

        if (probationMessages.msgCounter != null) {
            for (var i = 0; i < probationMessages.msgCounter; i++) {
                if (probationMessages.messages[i].IsSeen) {
                    messagesHtmlString += RenderMessgaeForProbation(1, probationMessages.messages[i].NotificationType, probationMessages.messages[i].TotalFailed).format(probationMessages.messages[i].TotalFailed, probationMessages.messages[i].CreatedDateString);
                }
                else {
                    messagesHtmlString += RenderMessgaeForProbation(0, probationMessages.messages[i].NotificationType, probationMessages.messages[i].TotalFailed).format(probationMessages.messages[i].TotalFailed, probationMessages.messages[i].CreatedDateString);
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

        messagesHtmlString += "</ul>";
        domElements.notificationContent.html((associatedMessages.msgCounter + probationMessages.msgCounter) > 0 ? messagesHtmlString : "<div class='no-notification'>There is no notification.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function RenderMessageForAAM(isRead)
    {
        var url = common.modifyRelativeUrl('Account/Index/{3}');
        var content = '<a href="' + url + '"> Account <span class="name">{0}</span> has {1} new associated account(s). </a>';
        if (isRead == 1)
            return "<li class='read'>" + content + "<span class='datetime'> {2}</span ></li >";
        else
            return "<li class='unread'>" + content + "<span class='datetime'> {2}</span ></li >";
    }

    function RenderMessgaeForProbation(isRead, notificationType, totalFail)
    {
        var url = common.modifyRelativeUrl('ProbationManagement/Index?filterType=' + notificationType);
        var content = '<a href="' + url + '"> There are <span class="name">{0}</span> account(s) '; 

        if (notificationType == 1) 
            content = content + ' cannot pass Probation on {1}. </a>';
        else 
            content = content + ((totalFail > 1) ? 'who were' : 'who was') + ' tagged Probation and negative margin on {1}. </a>';

        if (isRead == 1)
            return "<li class='read'>" + content + "</li >";
        else
            return "<li class='unread'>" + content + "</li >";
    }

    function getUnreadMessageCount()
    {
        GetNewNotification(0,0);   //Default GET to load para = 0
    }

    domElements.notificationIcon.off("click");
    domElements.notificationIcon.on("click", function (event) {
        domElements.notificationBell.removeClass('bell-shake');
        domElements.circle.hide(); 
    });

    $(document).ready(function () {
        domElements.notificationIcon.on('click', function () {
            GetNewNotification(associatedMessages.maxAssociationId, probationMessages.maxProbationID);        //Get to update
            domElements.notificationList.toggle();
        });        
    });

    function GetNewNotification(maxAssociationID, maxProbationID) {
        var url = common.modifyRelativeUrl("/NotificationSetting/GetAllMessages");

        $.ajax(url, {
            type: 'POST',
            data: {
                maxAssociationID: maxAssociationID,
                maxProbationID: maxProbationID
            },
            success: function (json) {
                if (!CheckValid(json)) {
                    ReCheckin();
                } else {
                    CheckDifferentCurrentAndPreviousMessages(json);
                    SetUnReadCounterToCookie(json.unReadMessageCounter);

                    associatedMessages.messages = json.messages.AssociatedMessages;
                    associatedMessages.msgCounter = json.associatedMessagesCounter;
                    associatedMessages.msgUnreadCounter = json.associatedUnreadCounter;
                    associatedMessages.maxAssociationId = json.associatedMaxAssociationID;

                    probationMessages.messages = json.messages.ProbationMessages;
                    probationMessages.msgCounter = json.probationMessagesCounter;
                    probationMessages.msgUnreadCounter = json.probationUnreadCounter;
                    probationMessages.maxProbationID = json.probationMaxProbationID;

                    GetMessage();
                    SetupBellNotification();
                    isFirstNotification = json.isFirstNotification;
                }
            },
            complete: function () {
                if (isFirstNotification == false && GetUnReadFlagFromCookie() == "true") {
                    NotifyToDesktop();
                    document.cookie = "hasNewUnreadMessages=false; path=/";
                }

                if (unreadMsgCounter == 0) {
                    GetMessage();
                }
                
            },
            error: function (errMsg) {
                console.log('Get notification Error, ' + errMsg);
                SetUnReadCounterToCookie(0);
                msgCounter = 0;
                messages = null;
                GetMessage();
                console.log(errMsg);
            }
        });
    }

    function SetupBellNotification()
    {
        if (unreadMsgCounter <= 0) {
            domElements.notificationBell.removeClass('bell-shake');
            domElements.circle.hide();
        }
        else {
            if (unreadMsgCounter > 99) {
                var readTempHTML = "<span class='plus' id='plus-hide'>99+</span>"
                domElements.circle.html(readTempHTML);
            }
            else {
                domElements.circle.html(unreadMsgCounter);
            }

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

    function CheckDifferentCurrentAndPreviousMessages(json)
    {
         if (json.unReadMessageCounter != GetUnReadCounterFromCookie()
             && json.unReadMessageCounter != 0)
        {
             document.cookie = "hasNewUnreadMessages=true; path=/";
        }
        else {
             document.cookie = "hasNewUnreadMessages=false; path=/";
        }
    }

    function GetUnReadFlagFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith('hasNewUnreadMessages='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith('hasNewUnreadMessages'))
                .split('=')[1];
        } 
        return "false";
    }

    function GetUnReadCounterFromCookie() {

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
    });

    $(document).click(function (e) {
        if (!domElements.notificationIcon.is(e.target) && domElements.notificationIcon.has(e.target).length === 0
            && domElements.notificationList.is(":visible"))
        {
            domElements.notificationBell.removeClass('bell-shake');
            domElements.circle.hide();
            GetNewNotification(associatedMessages.maxAssociationId, probationMessages.maxProbationID);     
            domElements.notificationList.toggle();
        }
    });
}

$(notification);
