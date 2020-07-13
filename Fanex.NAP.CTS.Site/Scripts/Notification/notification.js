let unreadMsgCounter = 0; 
let messages;
let isNotificationListOpen = false;

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

    function getMessage()
    {
        var messagesHtmlString = "";
        var htmlTemplateRead = "<li class='unread'> <a href=''> Account <span class='name'>{0}</span> has {1} new associated accounts.</a> <span class='datetime'> {2}</span ></li >";
        for (var i = 0; i < unreadMsgCounter; i++) {
            messagesHtmlString += htmlTemplateRead.format(messages[i].UserName, messages[i].NewAssocitionCount, messages[i].CreatedDateString);
        }

        console.log(unreadMsgCounter);
        domElements.notificationContent.html(unreadMsgCounter > 0 ? messagesHtmlString : "<div class='no-notification'>There is no notification.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function getUnreadMessageCount()
    {
        if ($('#notification-list').is(':visible')) {
            isNotificationListOpen = true
            GetNewNotification(true);
        }
        else
           GetNewNotification(false);      
    }

    domElements.notificationIcon.off("click");
    domElements.notificationIcon.on("click", function (event) {
          GetNewNotification(true);
    });


    getUnreadMessageCount();
    setInterval(getUnreadMessageCount, 500000);

    $(document).ready(function () {
        domElements.notificationIcon.on('click', function () {
            domElements.notificationList.toggle();

        });        
    });

    function GetNewNotification(isRead)
    {
        var url = common.modifyRelativeUrl("/Notification/GetAllMessages");

        $.get(url, { isRead: isRead}, function (json) {
            unreadMsgCounter = json.length;
            messages = json
            getMessage();

            if (unreadMsgCounter <= 0
                || (isNotificationListOpen == true && isRead == true && !$('#notification-list').is(':visible'))) {
                domElements.notificationBell.remove('bell-shake');
                domElements.circle.hide();  
                isNotificationListOpen = false;
            }
            else {
                domElements.notificationBell.addClass('bell-shake');
                domElements.circle.html(unreadMsgCounter);
                domElements.circle.show();
            }
        });
    }
}

$(notification);
