function AccountDetail() {
    function LoadAssociationByAccount() {
        $.ajax("GetAssociatedAccountByAccount", {
            type: 'POST',            
            success: function (response) {
                $("#associated_account").html(response) 
            },
            error: function (errMsg) {
                
            }
        });
    }

    function Initial() {
        LoadAssociationByAccount();
    }  
    
    $(document).ready(function () {
        Initial();
    });
}
$(AccountDetail);