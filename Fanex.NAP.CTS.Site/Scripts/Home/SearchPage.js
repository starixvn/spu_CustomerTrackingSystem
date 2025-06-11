function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
    var iconSearch = $('.icon-magnify');
    var errorMessage = $('#errMessage');
    var frmSearch = $('form');
    var searchFixed = $(".search-result > form");
    var searchBox = $(".search-box");
    var fileInput = $('#fileUpload');
    var uploadedFile = $('.uploaded-file');
    var fileNameSpan = $('.file-name');
    var removeFile = $('.remove-file');

    $(window).scroll(function () {
        var scroll = $(window).scrollTop();

        if (scroll >= 200) {
            searchFixed.addClass("has-shadow");
        } else {
            searchFixed.removeClass("has-shadow");
        }
    });

    function Initial() {        
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

        // Drag and Drop events
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
            errorMessage.addClass('hide');

            $('.search-icon').attr('src', '/Content/scss/images/search-icon.png');
        });
    }    

    function handleFiles(files) {
        if (files.length > 0) {
            const file = files[0];
            if (file.name.endsWith('.xls')) {
                errorMessage.addClass('hide');
                fileNameSpan.text(file.name); 
                uploadedFile.show();

                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);
                fileInput[0].files = dataTransfer.files;

                $('.search-icon').attr('src', '/Content/scss/images/search-icon-enable.png');
            } else {
                errorMessage.textContent = 'Please upload a .xls file!';
                errorMessage.removeClass('hide');
                fileInput.val(''); 
            }
        }
    }

    function SearchActive() {
        if (input.val().length <= 1) {
            errorMessage.show();

            return false;
        }
        else {
            frmSearch.submit();
        }
    }

    $(document).ready(function () {
        Initial();        
    });
}

$(SearchPage);


