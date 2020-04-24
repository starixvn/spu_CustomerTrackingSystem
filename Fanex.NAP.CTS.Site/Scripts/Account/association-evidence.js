function AssociationEvidence() {

    var totalAssAccount = 0;
    var totalEvidence = 0

    function LoadAssociationByAccount(ctsCustId, username, subscriberId, subscriber) {
        $.ajax(common.modifyRelativeUrl('Account/AssociatedByAccount'), {
            type: 'POST',
            data: {
                username: username
            },
            success: function (response) {
                $("#associated_account").html(response);
            },
            complete: function () {
                var associationByAccount = new account_by_account();
                associationByAccount.Initial(ctsCustId, username, subscriberId, subscriber);
            },
            error: function (errMsg) {
                // common.showError(errMsg);
            }
        });
    }

    function LoadChainToEvidence(ctsCustId, username, subscriberId, subscriber) {
        $.ajax(common.modifyRelativeUrl('Account/ChainToEvidence'), {
            type: 'POST',
            success: function (response) {
                $("#chains_evidence").html(response);
            },
            complete: function () {
                var accountEvidence = new account_evidence();
                accountEvidence.Initial(ctsCustId, username, subscriberId, subscriber);

                var exceptions = new account_exceptions();
                exceptions.Initial(ctsCustId, username, subscriberId);
            },
            error: function (errMsg) {
                // common.showError(errMsg);
            }
        });
    }
    this.GetCustomerTotalAssociation = function(ctsCustId, username, subscriberId, subscriber) {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerTotalAssociationByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {

                totalAssAccount = response.TotalAccounts;

                $('#totalAssAccount').html(response.TotalAccounts);
                $('#totalAssDevice').html(response.TotalDevices);
                $('#totalAssIP').html(response.TotalIps);
            },
            complete: function () {
                GetCustomerEvidences(ctsCustId, username, subscriberId, subscriber);
            },
            error: function (errMsg) {
                // common.showError(errMsg);
            }
        });
    }

    function GetCustomerEvidences(ctsCustId, username, subscriberId, subscriber) {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerEvidenceByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                $('#evidences').html(response.evidences);
                totalEvidence = response.numberOfItems;

                if (response.numberOfItems == 0) {
                    $('#lnkEvidence').contents().unwrap();
                }
            },
            complete: function () {
                if (totalAssAccount > 0 && totalEvidence > 0) {
                    $('span.icon-Affected').removeClass('hide');
                }

                LoadAssociationByAccount(ctsCustId, username, subscriberId, subscriber);
                LoadChainToEvidence(ctsCustId, username, subscriberId, subscriber);
            },
            error: function (errMsg) {
                // common.showError(errMsg);
            }
        });
    }
}