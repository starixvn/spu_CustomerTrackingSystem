function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
    var iconSearch = $('.search-icon');
    var errorMessage = $('#errMessage');
    var frmSearch = $('form');
    var searchFixed = $(".search-result > form");
    var searchBox = $(".search-box");
    var fileInput = $('#fileUpload');
    var uploadedFile = $('.uploaded-file');
    var fileNameSpan = $('.file-name');
    var removeFile = $('.remove-file');
    let hasDraggedFile = false;

    $(window).scroll(function () {
        var scroll = $(window).scrollTop();

        if (scroll >= 200) {
            searchFixed.addClass("has-shadow");
        } else {
            searchFixed.removeClass("has-shadow");
        }
    });

    function Initial() {
        fileInput.val('');
        uploadedFile.hide();
        fileNameSpan.text('').attr('title', '');
        errorMessage.addClass('hide').text('');
        hasDraggedFile = false;
        iconSearch.attr('src', $('#searchIcon').data('default'));
        AppendTabIdSubmitForm();

        input.focus(function () {
            searchBox.addClass("has-shadow");
        }).focusout(function () {
            searchBox.removeClass("has-shadow");
        });

        input.keypress(function (event) {
            if (event.which === 13) {
                SearchActive();
            }
        });

        input
            .putCursorAtEnd() // should be chainable
            .on("focus", function () { // could be on any event
                input.putCursorAtEnd()
            });

        iconSearch.on('click', function () {
            SearchActive();
        });

        searchBox.on('dragover', function (e) {
            e.preventDefault();
            searchBox.addClass('dragover');
        });

        searchBox.on('dragleave', function (e) {
            e.preventDefault();
            searchBox.removeClass('dragover');
        });

        searchBox.on('drop', function (e) {
            e.preventDefault();
            searchBox.removeClass('dragover');
            const files = e.originalEvent.dataTransfer.files;
            handleFiles(files);
        });

        fileInput.on('change', function (e) {
            handleFiles(e.target.files);
        });

        removeFile.on('click', function () {
            uploadedFile.hide();
            fileInput.val('');
            hasDraggedFile = false;
            errorMessage.addClass('hide').text('');

            iconSearch.attr('src', $('#searchIcon').data('default'));
        });
    }

    function handleFiles(files) {
        if (files.length > 0) {
            const maxMBSize = 500;
            const maxSize = maxMBSize * 1024 * 1024; // 500MB in bytes
            const file = files[0];

            if (file.size > maxSize) {
                common.showError('Exceeds the allowed limit of ' + maxMBSize + " MB");

                fileInput.val('');
                uploadedFile.hide();
                hasDraggedFile = false;
                iconSearch.attr('src', $('#searchIcon').data('default'));

                return;
            }

            if (/\.(xls|xlsx)$/i.test(file.name)) {
                const dataTransfer = new DataTransfer();

                errorMessage.addClass('hide').text('');
                fileNameSpan.text(file.name).attr('title', file.name);
                uploadedFile.show();
                dataTransfer.items.add(file);
                fileInput[0].files = dataTransfer.files;
                hasDraggedFile = true;
                iconSearch.attr('src', $('#searchIcon').data('enabled'));

                common.showSuccess('File uploaded successfully: ' + file.name, 2000);
            } else {
                common.showError('Please upload a .xls or .xlsx file!');

                fileInput.val('');
                uploadedFile.hide();
                hasDraggedFile = false;
                iconSearch.attr('src', $('#searchIcon').data('default'));
            }
        }
    }

    function SearchActive() {
        const text = input.val().trim();
        const hasText = text.length >= 2;
        const hasFile = fileInput[0].files.length > 0 || hasDraggedFile;

        if (!hasText && !hasFile) {
            errorMessage.show();
            errorMessage.removeClass('hide');
            return false;
        }
        else {
            errorMessage.addClass('hide');
            frmSearch.submit();
        }
    }

    function AppendTabIdSubmitForm() {
       frmSearch.on('submit', function(e) {
            $('<input>').attr({
                type: 'hidden',
                name: 'TabId',
                value: common.getTabId()
            }).appendTo(this);
        });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(SearchPage);


