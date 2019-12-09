function SearchPage() {
    var common = new UICommon();
    var input = $('#txtInput');
    var errorMessage = $('#errMessage');

    function Initial() {
        errorMessage.hide();
        input.focus();
    }

    input.keypress(function (event) {
        if (event.which === 13)
        {
            if (input.val().length <= 3) {
                errorMessage.show();
            }
            else {
                errorMessage.hide();

            }
        }
    });

    Initial();
}

$(SearchPage);