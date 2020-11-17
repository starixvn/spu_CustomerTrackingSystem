function ProbationManagement() {
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        hdProbationFilterType: $('#hdProbationFilterType').val(),
        tabs: $("#tabs-probation-detail"),        
        inputUserNameGeneral: $('#inputUserNameGeneral'),
        cbbSiteGeneral: $('#cbbSiteGeneral'),
        cbbAccountStatusGeneral: $('#cbbAccountStatusGeneral'),
        cbbDaysLeft: $('#cbbDaysLeft'),
        cbbAssociatedWithPAGeneral: $('#cbbAssociatedWithPAGeneral'),
        inputMinWinLossGeneral: $('#inputMinWinLossGeneral'),
        inputMaxWinLossGeneral: $('#inputMaxWinLossGeneral'),
        inputMinMarginGeneral: $('#inputMinMarginGeneral'),
        inputMaxMarginGeneral: $('#inputMaxMarginGeneral'),
        btnExportGeneral: $('#btnExportGeneral')
    };
  
    function GetParamFilters() {
        let params = {};       
        let subscriberSelected = [];
        let statusSelected = [];
        let associatedWithPASelected = [];

        domElements.cbbSiteGeneral.find('option:selected').each(function () { subscriberSelected.push($(this).text()); });
        domElements.cbbAccountStatusGeneral.find('option:selected').each(function () { statusSelected.push($(this).text()); });
        domElements.cbbAssociatedWithPAGeneral.find('option:selected').each(function () { associatedWithPASelected.push($(this).text()); });

        params.Username = domElements.inputUserNameGeneral.val().trim();
        params.SubscriberID = domElements.cbbSiteGeneral.val();
        params.SubscriberName = subscriberSelected.join('-');
        params.StatusIDs = domElements.cbbAccountStatusGeneral.val();
        params.StatusNames = statusSelected.join('-');
        params.DaysLeft = domElements.cbbDaysLeft.val();
        params.IsAssociatedPA = domElements.cbbAssociatedWithPAGeneral.val();
        params.AssociatedWithPA = associatedWithPASelected.join(',');
        params.MinWinLoss = domElements.inputMinWinLossGeneral.val();
        params.MaxWinLoss = domElements.inputMaxWinLossGeneral.val();
        params.MinMargin = domElements.inputMinMarginGeneral.val();
        params.MaxMargin = domElements.inputMaxMarginGeneral.val();
        params.CategoryMode = 1;

        return params;
    }

    function ExportGeneral() {
        let url = common.modifyRelativeUrl('ProbationManagement/ExportProbationCategory');
        let params = GetParamFilters();              
        let downloadUrl = url + '?' + $.param(params);
        let request = new XMLHttpRequest();

        common.showLoading();

        request.open('GET', downloadUrl, true);
        request.setRequestHeader('Content-Type', 'text/plain; charset=UTF-8');
        request.responseType = 'blob';
        request.timeout = 600000;
        request.addEventListener("timeout", function (e) {
            common.closeLoading();
            common.showError("Failed (timeout): too many records");
        });
        request.onload = function (e) {
            if (this.status === 200) {
                let extension = ".csv";
                let date = getFormattedDate();
                let filenameFormat = "Probations_{0}_{1}{2}";
                let filename = filenameFormat.format(params.SubscriberID, date, extension);
                let blob = this.response;

                if (window.navigator.msSaveOrOpenBlob) {
                    window.navigator.msSaveBlob(blob, filename);
                }
                else {
                    let downloadLink = window.document.createElement('a');
                    let contentTypeHeader = request.getResponseHeader("Content-Type");
                    downloadLink.href = window.URL.createObjectURL(new Blob([blob], { type: contentTypeHeader }));
                    downloadLink.download = filename;
                    document.body.appendChild(downloadLink);
                    downloadLink.click();
                    document.body.removeChild(downloadLink);
                }
            } else {
                console.log(this);
                common.showError("Failed: too many records");
            }
            common.closeLoading();
        };
        request.send();
    }

    function getFormattedDate() {
        let date = new Date();
        let stringDateFormat = date.getFullYear().toString()
            + (date.getMonth() + 1).toString()
            + date.getDate().toString()
            + date.getHours().toString()
            + date.getMinutes().toString()
            + date.getSeconds().toString();

        return stringDateFormat;
    }

    function Initial() {
        domElements.tabs.tabs();          
        domElements.btnExportGeneral.on('click', function () { ExportGeneral(); });

        domElements.cbbSiteGeneral.select2();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProbationManagement);
