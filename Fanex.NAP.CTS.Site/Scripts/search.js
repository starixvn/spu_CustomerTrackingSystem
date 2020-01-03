function search() {
    var common = new UICommon();  
    var input = $('#txtInput');   
    
    function Initial() {
        //input.keypress(function (event) {
        //    if (event.which === 13) {
        //        if (input.val().length <= 1) {
        //            errorMessage.show();

        //            return false;
        //        }
        //        else {
        //            frmSearch.submit();
        //        }
        //    }
        //});
    }

    $(document).ready(function () {
        Initial();
    });
}

$(search);