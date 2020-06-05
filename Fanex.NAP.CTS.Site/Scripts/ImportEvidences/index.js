function ImportEvidences() {

    let domElements = {
        inputFileUpload: $('#inputFileUpload'),
        divResult: $('#divResult'),
        btnImport: $('#btnImport')
    };

    function ImportEvidences() {
        let fileName = domElements.inputFileUpload.val();

        if (ValidateInputFile(fileName)) {
            let formdata = new FormData();
            let fileUpload = domElements.inputFileUpload.get(0);
            let files = fileUpload.files;

            formdata.append(files[0].name, files[0]);
            
            $.ajax(common.modifyRelativeUrl('ImportEvidences/ImportFromExcel'), {
                type: 'POST',               
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (response) {
                    console.log(response);
                },
                complete: function () {
                    
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function ValidateInputFile(fileName) {
        let fileExtension = ['xls', 'xlsx'];
        let extension = fileName.replace(/^.*\./, '');

        if (fileName.length === 0) {
            common.showError("Please select a file");
            return false;
        }
        
        if ($.inArray(extension, fileExtension) === -1) {
            common.showError("Please select only excel files.");
            return false;
        }

        return true;
    }

    function Initial() {
        domElements.btnImport.on('click', () => ImportEvidences());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ImportEvidences);