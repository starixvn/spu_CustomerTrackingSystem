function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
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
        errorMessage.hide();   

        input.focus(function () {
            searchBox.addClass("has-shadow");
        }).focusout(function () {            
            searchBox.removeClass("has-shadow");
        });

        input.keypress(function (event) {
            if (event.which === 13) {
                if (input.val().length <= 2) {
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
    }      

    $(document).ready(function () {
        Initial();        
    });
}

$(SearchPage);


