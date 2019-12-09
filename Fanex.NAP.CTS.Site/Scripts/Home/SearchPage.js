function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
    var errorMessage = $('#errMessage');
    var frmSearch = $('form');

    function Initial() {
        errorMessage.hide();
        input.focus();
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

    Initial();
}

$(SearchPage);