function UICommon() {
    $("#progress_bar").progressbar({ value: false });

    var reg = /[<>]/;

    var loading = $("#loading").hide();

    var formconfirmation = $("#formconfirmation").dialog({
        autoOpen: false,
        modal: true,
        draggable: true,
        resizable: false,
    });

    var confirmation = $("#confirmation").dialog({
        autoOpen: false,
        modal: true,
        draggable: false,
        resizable: false,
    });

    var success = $("#success").dialog({
        autoOpen: false,
        draggable: false,
        resizable: false,
        height: 100,
        position: { my: "top", at: "top", of: window }
    });

    var error = $("#error").dialog({
        autoOpen: false,
        draggable: false,
        resizable: false,
        height: 100,
        position: { my: "top", at: "top", of: window }
    });

    var defaultTableOptions = {
        ordering: false,
        searching: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: ""
        }
    }

    var language =
    {
        lengthMenu: "Page size _MENU_",
        paginate: {
            previous: "Back"
        },
        info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
        infoEmpty: "",
        zeroRecords: "There's no data"
    }

    var $this = this;

    // $(document).ajaxError(() => this.showError());

    this.categoryColor = function () {
        var list = [];

        list.push({ 'id': '205', 'indexColor': 1, 'name': 'Good Member' });
        list.push({ 'id': '210', 'indexColor': 1, 'name': 'VVIP' });
        list.push({ 'id': '201', 'indexColor': 1, 'name': 'New Member' });
        list.push({ 'id': '202', 'indexColor': 1, 'name': 'Normal Account' });

        list.push({ 'id': '206', 'indexColor': 2, 'name': 'Probation' });
        list.push({ 'id': '203', 'indexColor': 2, 'name': 'Smart Punter' });
        list.push({ 'id': '204', 'indexColor': 2, 'name': 'High Risk Punter' });

        list.push({ 'id': '211', 'indexColor': 3, 'name': 'Good Member (111)' });
        list.push({ 'id': '212', 'indexColor': 3, 'name': 'Good Member (112)' });
        list.push({ 'id': '213', 'indexColor': 3, 'name': 'Good Member (113)' });
        list.push({ 'id': '221', 'indexColor': 3, 'name': 'Normal Account (121)' });
        list.push({ 'id': '222', 'indexColor': 3, 'name': 'Normal Account (122)' });
        list.push({ 'id': '223', 'indexColor': 3, 'name': 'Normal Account (123)' });
        list.push({ 'id': '231', 'indexColor': 3, 'name': 'New Member (31)' });
        list.push({ 'id': '232', 'indexColor': 3, 'name': 'New Member (32)' });
        list.push({ 'id': '233', 'indexColor': 3, 'name': 'New Member (33)' });

        list.push({ 'id': '251', 'indexColor': 4, 'name': 'Probation (51)' });
        list.push({ 'id': '252', 'indexColor': 4, 'name': 'Probation (52)' });
        list.push({ 'id': '253', 'indexColor': 4, 'name': 'Probation (53)' });
        list.push({ 'id': '261', 'indexColor': 4, 'name': 'Smart Punter (61)' });
        list.push({ 'id': '262', 'indexColor': 4, 'name': 'Smart Punter (62)' });
        list.push({ 'id': '263', 'indexColor': 4, 'name': 'Smart Punter (63)' });
        list.push({ 'id': '271', 'indexColor': 4, 'name': 'High Risk Punter (71)' });
        list.push({ 'id': '272', 'indexColor': 4, 'name': 'High Risk Punter (72)' });
        list.push({ 'id': '273', 'indexColor': 4, 'name': 'High Risk Punter (73)' });

        return list;
    };

    this.showLoading = function () {
        loading.show();
    };

    this.closeLoading = function () {
        loading.hide();
    }

    this.downloadFile = function (url, parameters) {
        if (url && parameters) {
            var downloadUrl = url + '?';
            var iframe = $('body iframe');

            parameters = typeof parameters === 'string' ? parameters : $.param(parameters);
            downloadUrl += parameters;

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr('src', downloadUrl);
        }
    };

    this.downloadFilePostMethod = function (url, parameters) {
        if (url && parameters) {
            $('body').append('<form action="' + url + '" method="post" target="output_frame" id="postToIframe"></form>');
            $.each(parameters, function (n, v) {
                $('#postToIframe').append('<input type="hidden" name="' + n + '" value="' + v + '" />');
            });
            $('body').append('<iframe  name="output_frame" width="0" height="0" class="display-none"></iframe>');
            $('#postToIframe').submit().remove();
        }
    };

    this.hasSpecialCharacter = function (str = "") {
        if (str == null) {
            return false;
        }

        return reg.test(str);
    }

    this.handleSpecialCharacter = function (str) {
        if (str != null) {
            return str.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;");
        }
        return str;
    }

    this.showFormConfirmation = function (html, title, yesCallback, noCallback) {
        formconfirmation.empty().html(html).dialog("open");

        var yesBtn =
        {
            'class': 'button -add',
            text: "Confirm",
            click: function () {
                formconfirmation.dialog("close");

                if (yesCallback != null) {
                    yesCallback(this)
                }
            }
        };

        var noBtn =
        {
            'class': 'button btn-cancel-outline',
            text: "Cancel",
            click: function () {
                formconfirmation.dialog("close");

                if (noCallback != null) {
                    noCallback()
                }
            }
        }

        formconfirmation.dialog("option", {
            dialogClass: "dialog-confirmation",
            title: title,
            buttons: [noBtn, yesBtn],
            width: 460,
            close: noCallback
        });
    }

    this.showConfirmation = function (html, yesCallback, noCallback) {
        confirmation.empty().html(html).dialog("open");

        var yesBtn =
        {
            'class': 'button -add',
            text: "Yes",
            click: function () {
                confirmation.dialog("close");

                if (yesCallback != null) {
                    yesCallback(this)
                }
            }
        };

        var noBtn =
        {
            'class': 'button btn-cancel-outline',
            text: "No",
            click: function () {
                confirmation.dialog("close");

                if (noCallback != null) {
                    noCallback()
                }
            }
        }

        confirmation.dialog("option", {
            dialogClass: "dialog-confirmation",
            buttons: [yesBtn, noBtn],
            width: 560,
            close: noCallback
        });
    }

    this.showSuccess = function (str = "Success", timeout = 5000) {
        $.toast({
            text: `<div class='toast-content'>${str}</div>`, // Text that is to be shown in the toast
            heading: 'Successful!', // Optional heading to be shown on the toast
            icon: 'success', // Type of toast icon
            showHideTransition: 'fade', // fade, slide or plain
            allowToastClose: true, // Boolean value true or false
            hideAfter: timeout, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
            stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
            position: 'top-center', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
            textAlign: 'left',  // Text alignment i.e. left, right or center
            loader: true,  // Whether to show loader or not. True by default
            loaderBg: '#fff',  // Background color of the toast loader
        });
    }

    this.showError = function (str = "Internal Error", timeout = 3000) {
        $.toast({
            text: `<div class='toast-content'>${str}</div>`, // Text that is to be shown in the toast
            heading: 'Error!', // Optional heading to be shown on the toast
            icon: 'error', // Type of toast icon
            showHideTransition: 'fade', // fade, slide or plain
            allowToastClose: true, // Boolean value true or false
            hideAfter: timeout, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
            stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
            position: 'top-center', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
            textAlign: 'left',  // Text alignment i.e. left, right or center
            loader: true,  // Whether to show loader or not. True by default
            loaderBg: '#fff',  // Background color of the toast loader
        });
    }

    this.getTableDefaultOptions = () => Object.assign({}, defaultTableOptions);

    this.modifyRelativeUrl = function (url) {
        var host = window.location.origin;

        return host + config.contextPath + url;
    }

    this.makeNonSearchSelect = select => $(select).select2({ minimumResultsForSearch: -1 })

    this.makeDatePicker = function (input, options) {
        var $input = $();

        if (input instanceof jQuery) {
            $input = input;
        }
        else {
            $input = $(input);
        }

        var next_element = $input.next();

        if (next_element.is("span")) {
            next_element.addClass("icon-calendar-text");
        }
        else {
            next_element = $('<span class="icon-calendar-text"></span>');
            $input.after(next_element);
        }

        next_element.click(() => $input.focus());

        $input.addClass("control").datepicker(options);
    }

    this.makeWrapString = function (str = "", maxLength = 0) {
        if (str == null) return str;

        if (str.length > maxLength) return `<span title="${str}">${str.substring(0, maxLength - 3)}...</span>`;

        return str;
    }

    this.formatNumber = (n = 0, format = "0,0.00") => n == null ? n : numeral(n).format(format);

    this.encodeString = encodeURIComponent;

    this.registerAutoLoading = function () {
        $(document).ajaxStart(() => loading.show());
        $(document).ajaxComplete(() => loading.hide());
    }

    this.formatGridNumber = (n = 0, format = "0,0.00") => n == null ? null : n >= 0 ? numeral(n).format(format) : `<span class="text-red">${numeral(n).format(format)}</span>`;

    this.handleUpdateAjax = function (successMsg, successFunc, failMsg, failFunc) {
        return function (result) {
            if (result.ErrorCode == 0) {
                if (successMsg != null) {
                    $this.showSuccess(successMsg);
                }
                else {
                    $this.showSuccess();
                }

                if (successFunc != null) {
                    successFunc(result);
                }
            }
            else {
                if (failMsg != null) {
                    $this.showError(failMsg);
                }
                else {
                    $this.showError(result.ErrorMsg);
                }

                if (failFunc != null) {
                    failFunc(result);
                }
            }
        }
    }

    this.language = () => Object.assign({}, language);

    this.renderCategoryColor = function (cell, row) {
        let indexColorForDiv = 0;
        if (row.IsProblemAccount) {
            indexColorForDiv = 5;
        } else {

            let categoryColorCached = $this.categoryColor();
            let categoryList = row.CategoryIds != undefined
                ? row.CategoryIds.split(',')
                : [];

            for (let i = 0; i < categoryColorCached.length; i++) {
                categoryList.map(function (x) {
                    let isEquals = x === categoryColorCached[i].id;
                    if (isEquals) {
                        indexColorForDiv = indexColorForDiv < categoryColorCached[i].indexColor
                            ? categoryColorCached[i].indexColor
                            : indexColorForDiv;
                    }
                })
            }
        }
        switch (indexColorForDiv) {
            case 1:
                $(cell).addClass('td-bg-green');
                break;
            case 2:
                $(cell).addClass('td-bg-yellow');
                break;
            case 3:
                $(cell).addClass('green-line');
                break;
            case 4:
                $(cell).addClass('yellow-line');
                break;
            case 5:
                $(cell).addClass('td-bg-red');
                break;
            default:
                break;
        }

        $(cell).prop('title', row.CategoryNames);
    }

    this.renderCategoryName = function (categoryNames) {      
        return categoryNames;
    };
}

var common = new UICommon();

jQuery.fn.putCursorAtEnd = function () {

    return this.each(function () {

        // Cache references
        var $el = $(this),
            el = this;

        // Only focus if input isn't already
        if (!$el.is(":focus")) {
            $el.focus();
        }

        // If this function exists... (IE 9+)
        if (el.setSelectionRange) {

            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
            var len = $el.val().length * 2;

            // Timeout seems to be required for Blink
            setTimeout(function () {
                el.setSelectionRange(len, len);
            }, 1);

        } else {

            // As a fallback, replace the contents with itself
            // Doesn't work in Chrome, but Chrome supports setSelectionRange
            $el.val($el.val());

        }

        // Scroll to the bottom, in case we're in a tall textarea
        // (Necessary for Firefox and Chrome)
        this.scrollTop = 999999;

    });

};