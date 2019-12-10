function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
    var errorMessage = $('#errMessage');
    var frmSearch = $('form');
    var searchFixed = $(".search-result > form");

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
    }

    input.keypress(function (event) {
        if (event.which === 13)
        {
            if (input.val().length <= 2) {
                errorMessage.show();

                return false;
            }
            else {
                frmSearch.submit();
            }            
        }
    });
    
    $(document).ready(function () {
        Initial();
        input
            .putCursorAtEnd() // should be chainable
            .on("focus", function () { // could be on any event
                searchInput.putCursorAtEnd()
            });
    });
}

$(SearchPage);


