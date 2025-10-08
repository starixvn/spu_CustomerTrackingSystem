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

    this.downloadFileExcel = function (url, parameters) {
        $this.showLoading();
        let request = new XMLHttpRequest();
        request.open('POST', url, true);
        request.setRequestHeader('Content-type', 'application/json');
        request.responseType = 'blob';
        request.timeout = 600000;
        request.addEventListener("timeout", function (e) {
            $this.closeLoading();
            $this.showError("Export failed (timeout)");
        });
        request.onload = function (e) {
            if (this.status === 200) {
                let extension = ".xlsx";
                let date = $this.getFormattedDate();
                let filenameFormat = "Export_{0}{1}";
                let filename = filenameFormat.format(date, extension);
                let blob = this.response;

                if (window.navigator.msSaveOrOpenBlob) {
                    window.navigator.msSaveBlob(blob, filename);
                } else {
                    let downloadLink = window.document.createElement('a');
                    let contentTypeHeader = request.getResponseHeader("Content-Type");
                    downloadLink.href = window.URL.createObjectURL(new Blob([blob], { type: contentTypeHeader }));

                    var disposition = request.getResponseHeader('Content-Disposition');
                    if (disposition && disposition.indexOf('filename') !== -1) {
                        var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        var matches = filenameRegex.exec(disposition);
                        if (matches != null && matches[1]) {
                            filename = matches[1].replace(/['"]/g, '');
                        }
                    }

                    downloadLink.download = filename;
                    document.body.appendChild(downloadLink);
                    downloadLink.click();
                    document.body.removeChild(downloadLink);
                }
            } else {
                console.log(this);
                $this.showError("Export failed");
            }

            $this.closeLoading();
        }

        request.send(JSON.stringify(parameters));
    }

    this.getFormattedDate = function () {
        let date = new Date();

        let stringDateFormat = date.getUTCFullYear().toString()
            + ('0' + (date.getUTCMonth() + 1)).slice(-2).toString()
            + ('0' + date.getUTCDate()).slice(-2).toString()
            + '_'
            + ('0' + date.getUTCHours()).slice(-2).toString()
            + ('0' + date.getUTCMinutes()).slice(-2).toString()
            + ('0' + date.getUTCSeconds()).slice(-2).toString();

        return stringDateFormat;
    }

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

    this.showConfirmation2 = function (html, yesCallback, noCallback) {
        confirmation.empty().html(html).dialog("open");

        var noBtn =
        {
            'class': 'button -add',
            text: "No",
            click: function () {
                confirmation.dialog("close");

                if (noCallback != null) {
                    noCallback()
                }
            }
        };

        var yesBtn =
        {
            'class': 'button btn-cancel-outline',
            text: "Yes",
            click: function () {
                confirmation.dialog("close");

                if (yesCallback != null) {
                    yesCallback(this)
                }
            }
        }

        confirmation.dialog("option", {
            dialogClass: "dialog-confirmation",
            buttons: [noBtn, yesBtn],
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

    this.getTabId = function () {
        if (!sessionStorage.getItem("TabId")) {
            sessionStorage.setItem("TabId", "TabId" + this.getUID().toString());
        }

        return sessionStorage.getItem("TabId");
    }

    this.getUID = function () {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11)
            .replace(/[018]/g, c =>
                (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
            );
    }

    this.postMultipleAPI = function (requests) {
        // Convert each request object into an AJAX Promise
        var ajaxCalls = requests.map(req =>
            $.ajax({
                url: req.url,
                type: "POST",
                data: JSON.stringify(req.data),
                contentType: "application/json"
            })
        );

        // Use $.when() to handle multiple requests
        return $.when(...ajaxCalls);
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
        $(cell).addClass('td-bg-' + row.CategoryColorDesc);

        $(cell).prop('title', row.CategoryNames);
    }

    this.renderCategoryName = function (categoryNames) {
        return categoryNames;
    };

    this.CompareObjects = function (obj1, obj2) {
        let keys1 = Object.keys(obj1);
        let keys2 = Object.keys(obj2);

        if (keys1.length !== keys2.length) {
            return false;
        }

        for (let key of keys1) {
            if (Array.isArray(obj1[key]) && Array.isArray(obj2[key])) {
                if (!$this.CompareArraysOfObjects(obj1[key], obj2[key])) {
                    return false;
                }
            }
            else {
                if (obj1[key] !== obj2[key]) {
                    return false;
                }
            }
        }

        return true;
    }

    this.CompareArraysOfObjects = function (array1, array2) {
        if (array1.length !== array2.length) {
            return false;
        }

        for (let i = 0; i < array1.length; i++) {
            let obj1 = array1[i];
            let obj2 = array2[i];

            let keys1 = Object.keys(obj1);
            let keys2 = Object.keys(obj2);

            if (keys1.length != keys2.length) {
                return false;
            }

            for (let key of keys1) {
                if (obj1[key] !== obj2[key]) {
                    return false;
                }
            }
        }

        return true;
    }
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