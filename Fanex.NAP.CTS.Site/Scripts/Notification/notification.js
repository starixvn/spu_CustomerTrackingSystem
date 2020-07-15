let unreadMsgCounter = 0; 
let messages = null;
let isRefreshNewMessage = false;
let isFirstRead = true;

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
        return a
    }

    function GetMessage()
    {
        var messagesHtmlString = "<ul>";
        var htmlTemplateRead = "<li class='unread'> <a href=''> Account <span class='name'>{0}</span> has {1} new associated accounts.</a> <span class='datetime'> {2}</span ></li >";
        for (var i = 0; i < unreadMsgCounter; i++) {
            messagesHtmlString += htmlTemplateRead.format(messages[i].UserName, messages[i].NewAssocitionCount, messages[i].CreatedDateString);
        }
        messagesHtmlString += "</ul>";
        domElements.notificationContent.html(unreadMsgCounter > 0 ? messagesHtmlString : "<div class='no-notification'>There is no notification.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function getUnreadMessageCount()
    {
        isFirstRead = true;
        GetNewNotification(false);   
        isRefreshNewMessage = true;
    }

    domElements.notificationIcon.off("click");
    domElements.notificationIcon.on("click", function (event) {
        domElements.notificationBell.removeClass('bell-shake');
        domElements.circle.hide(); 
        ChangeStatusMessage();
    });

    $(document).ready(function () {
        domElements.notificationIcon.on('click', function () {
            isFirstRead = false;
            isRefreshNewMessage = false;
            GetNewNotification(true); 
            domElements.notificationList.toggle();
        });        
    });

    function GetNewNotification(isRead)
    {
        var url = common.modifyRelativeUrl("/Notification/GetAllMessages");

        $.get(url, { isRead: isRead }, function (json) {
            if (isRefreshNewMessage) {
                if (json.length != unreadMsgCounter && json.length > 0) {
                    NotifyToDesktop(json.length);
                }

                unreadMsgCounter = json.length;
                messages = json
                GetMessage();
            }

            SetupBellNotification()
        });
    }

    function SetupBellNotification()
    {
        if (unreadMsgCounter <= 0) {
            domElements.notificationBell.removeClass('bell-shake');
            domElements.circle.hide();
        }
        else if (isRefreshNewMessage){
            domElements.circle.html(unreadMsgCounter);
            domElements.circle.show();
            domElements.notificationBell.addClass('bell-shake');

        }
    }

    function ChangeStatusMessage()
    {
        if (!isFirstRead)
        {
            $("li").removeClass("unread").addClass("read");
        }
    }

    function NotifyToDesktop(notificationCounter) {
        if (!window.Notification)
            console.log('Browser does not support notifications.');
        else
            GenerateMessageNotify(notificationCounter);
    }

    function GenerateMessageNotify(notificationCounter) {
        var mess = notificationCounter > 1 ? notificationCounter + " new associated accounts were added." : notificationCounter + " new associated accounts was added.";
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

    setInterval(getUnreadMessageCount, 300000);


    $(document).ready(function () {
        getUnreadMessageCount();
    });
}

$(notification);
