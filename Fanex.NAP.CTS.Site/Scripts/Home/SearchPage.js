function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
    var iconSearch = $('.icon-magnify');
    var errorMessage = $('#errMessage');
    var frmSearch = $('form');
    var searchFixed = $(".search-result > form");
    var searchBox = $(".search-box");

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
                if (input.val().length <= 1) {
                    errorMessage.show();

                    return false;
                }
                else {
                    frmSearch.submit();
                }
            }
        });

        input
            .putCursorAtEnd() // should be chainable
            .on("focus", function () { // could be on any event
                input.putCursorAtEnd()
            });

        iconSearch.on('click', function () {
            frmSearch.submit();
        });
    }      

    $(document).ready(function () {
        Initial();        
    });
}

$(SearchPage);


