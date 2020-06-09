function ImportEvidences() {
    let dataTable;
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        inputFileUpload: $('#inputFileUpload'),
        divResult: $('#divResult'),
        divValidation: $('#divValidation'),
        divResultParseFile: $('#divResultParseFile'),
        tableResult: $('#tableResult'),
        btnDownloadTemplate: $('#btnDownloadTemplate'),
        btnImport: $('#btnImport')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

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
                beforeSend: function () {
                    common.showLoading();
                    domElements.divResultParseFile.prop('hidden', true);
                    domElements.divResult.prop('hidden', true);
                },
                success: function (response) {
                    ResultCheckAccountEvidence(response);
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    common.closeLoading();
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

    function ResultCheckAccountEvidence(data) {
        if (data.ErrorCode === 1) {
            domElements.divResult.prop('hidden', false);

            dataTable
                ? domElements.tableResult.DataTable().clear().rows.add(data.ResultCheckAccountEvidenceList).draw()
                : dataTable = GenerateGrid(data.ResultCheckAccountEvidenceList);
        } else if (data.ErrorCode === 2) {
            var html = "<span>" + data.ErrorMessage + "</span>";
            domElements.divResultParseFile.prop('hidden', false);
            domElements.divResultParseFile.html(html);
        } else {
            domElements.divValidation.prop('hidden', false);

            domElements.divValidation.dialog({
                modal: true,
                width: '400px',
                title: 'Successul Validation',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }
    }

    function GenerateGrid(data) {
        return domElements.tableResult.DataTable({
            language: {
                lengthMenu: "",
                info: "",
                infoEmpty: "",
                zeroRecords: ""
            },
            serverSide: false,
            paging: false,
            ordering: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "20%" }
            ],
            columns: [
                { data: obj => obj.RowId },
                { data: obj => obj.Username },
                { data: obj => obj.EvidenceCode },
                { data: obj => obj.RowStatus }
            ]
        }).rows.add(data).draw();
    }

    function Initial() {
        domElements.divResultParseFile.prop('hidden', true);
        domElements.divResult.prop('hidden', true);
        domElements.divValidation.prop('hidden', true);        
        domElements.btnImport.prop('disabled', !isHostPermission);

        if (isHostPermission)
            domElements.btnImport.on('click', () => ImportEvidences());

        domElements.btnDownloadTemplate.on('click', function () {
            common.downloadFile(common.modifyRelativeUrl('ImportEvidences/DownloadTemplateExcel'), '0');
        });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ImportEvidences);