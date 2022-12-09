function AssociatedGroupTransferAccount() {
    let domElementsTransAcc = {
        divTransferAccount: $('#divTransferAccount'),
        hdTransferAccountGroupID: $('#hdTransferAccountGroupID'),
        spanTransferAccountUserName: $('#spanTransferAccountUserName'),
        cbbTransferAccountNewGroup: $('#cbbTransferAccountNewGroup'),
        btnCancelTransferAccount: $('#btnCancelTransferAccount'),
        btnSaveTransferAccount: $('#btnSaveTransferAccount')
    };

    function SaveTransferAccount() {
        console.log("AssociatedGroupTransferAccount SaveTransferAccount");
    }

    function Initial() {
        console.log("AssociatedGroupTransferAccount Initial");

        domElementsTransAcc.btnCancelTransferAccount.on('click', () => domElementsTransAcc.divTransferAccount.dialog('close'));

        domElementsTransAcc.btnSaveTransferAccount.on('click', () => SaveTransferAccount());
    }

    $(document).ready(function () {
        Initial();
    })
}

$(AssociatedGroupTransferAccount);