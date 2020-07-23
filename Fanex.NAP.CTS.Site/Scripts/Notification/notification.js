let unreadMsgCounter = 0; 
let msgCounter = 0;
let messages = null;
let maxAssociationId = 0;
let isNoticedAlready = false;
let isHostPermission;

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
        var htmlTemplateRead = "<li class='read'>" + RenderUsername() +"<span class='datetime'> {2}</span ></li >";
        var htmlTemplateUnRead = "<li class='unread'>" + RenderUsername()+"<span class='datetime'> {2}</span ></li >";

        if (msgCounter != null) {
            for (var i = 0; i < msgCounter; i++) {
                if (messages[i].IsSeen) {
                    messagesHtmlString += htmlTemplateRead.format(messages[i].UserName, messages[i].NewAssocitionCount, messages[i].CreatedDateString, messages[i].CTSCustID);
                }
                else {
                    messagesHtmlString += htmlTemplateUnRead.format(messages[i].UserName, messages[i].NewAssocitionCount, messages[i].CreatedDateString, messages[i].CTSCustID);
                }
            }
        }

        messagesHtmlString += "</ul>";
        domElements.notificationContent.html(msgCounter > 0 ? messagesHtmlString : "<div class='no-notification'>There is no notification.</div>");
        $('.notification-icon').addClass('has-arrow-down');
    }

    function RenderUsername() {
            var url = common.modifyRelativeUrl('Account/Index/{3}');

            return '<a href="' + url + '"> Account <span class="name">{0}</span> has {1} new associated accounts. </a>';
    }

    function getUnreadMessageCount()
    {
        GetNewNotification(0);   //Default GET to load para = 0
    }

    domElements.notificationIcon.off("click");
    domElements.notificationIcon.on("click", function (event) {
        domElements.notificationBell.removeClass('bell-shake');
        domElements.circle.hide(); 
    });

    $(document).ready(function () {
        domElements.notificationIcon.on('click', function () {
            GetNewNotification(maxAssociationId);        //Get to update
            domElements.notificationList.toggle();
        });        
    });

    function GetNewNotification(maxAssociationID) {
        var url = common.modifyRelativeUrl("/Notification/GetAllMessages");

        if (isHostPermission === undefined)
        {
            GetCTSCustPermission();
        }

        if (isHostPermission == true) {
            $.ajax(url, {
                type: 'GET',
                data: { maxAssociationID: maxAssociationID },
                success: function (json) {
                    if (!CheckValid(json)) {
                        ReCheckin();
                    } else {
                        isHostPermission = json.isHostPermission;
                        CheckDifferentCurrentAndPreviousMessages(json);
                        SetUnReadCounterToCookie(json.unReadMessageCounter);
                        maxAssociationId = json.maxAssociationID;
                        messages = json.messages;
                        msgCounter = json.messageCounter;
                        GetMessage();
                        SetupBellNotification();
                    }
                },
                complete: function () {
                    if (GetUnReadFlagFromCookie() == "true") {
                        NotifyToDesktop(unreadMsgCounter);
                        document.cookie = "isUnReadStatusChanged=false";
                    }
                },
                error: function (errMsg) {
                    console.log('Get notification Error, ' + ex);
                    SetUnReadCounterToCookie(0);
                    msgCounter = 0;
                    messages = null;
                    GetMessage();
                    console.log(errMsg);
                }
            });
        }
        else {
            messages = null;
            msgCounter = 0;
            unreadMsgCounter = 0;
            document.cookie = "isUnReadStatusChanged=false";
            GetMessage();
            SetupBellNotification();
        }
    }

    function GetCTSCustPermission() {
        $.ajax(common.modifyRelativeUrl('/Notification/GetUserPermission'), {
            type: 'GET',
            data: {},
            success: function (response) {
                isHostPermission = response;
            },
            error: function (errMsg) {
                console.log(errMsg);
            },
            async: false
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

    setInterval(getUnreadMessageCount, 10000);

    function CheckDifferentCurrentAndPreviousMessages(json)
    {
         if (json.unReadMessageCounter != GetUnReadCounterFromCookie()
             && json.unReadMessageCounter != 0)
        {
            document.cookie = "isUnReadStatusChanged=true";
        }
        else {
            document.cookie = "isUnReadStatusChanged=false";
        }
    }

    function GetUnReadFlagFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith('isUnReadStatusChanged='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith('isUnReadStatusChanged'))
                .split('=')[1];
        } 
        return "false";
    }

    function GetUnReadCounterFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith('unReadCountFromCookie='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith('unReadCountFromCookie'))
                .split('=')[1];
        }

        return 0;
    }

    function SetUnReadCounterToCookie(counter) {
        document.cookie = "unReadCountFromCookie=" + counter;
        unreadMsgCounter = counter;
    }

    function CheckValid(json) {
        if (json.maxAssociationID === undefined
            && json.unReadMessageCounter === undefined
            && json.messageCounter === undefined)
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
        GetCTSCustPermission();
        getUnreadMessageCount();
    });
}

$(notification);
