function ImportEvidences() {
    let dataTable;
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        spanFileName: $('#spanFileName'),
        inputFileUpload: $('#inputFileUpload'),
        btnBrowse: $('#btnBrowse'),
        btnNoteToggle: $('#btnNoteToggle'),
        divNote: $('#divNote'),
        divResult: $('#divResult'),
        divValidation: $('#divValidation'),
        divResultParseFile: $('#divResultParseFile'),
        tableResult: $('#tableResult'),
        btnDownloadTemplate: $('#btnDownloadTemplate'),
        btnImport: $('#btnImport'),
        btnResultValidationClose: $('#btnResultValidationClose')
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
            HiddenNote();

            domElements.divResult.prop('hidden', false);

            dataTable
                ? domElements.tableResult.DataTable().clear().rows.add(data.ResultCheckAccountEvidenceList).draw()
                : dataTable = GenerateGrid(data.ResultCheckAccountEvidenceList);

        } else if (data.ErrorCode === 2) {
            HiddenNote();

            var html = "<span style='color:red; font-weight: bold'>" + data.ErrorMessage + "</span>";
            domElements.divResultParseFile.prop('hidden', false);
            domElements.divResultParseFile.html(html);
        } else {
            domElements.divValidation.prop('hidden', false);

            domElements.divValidation.dialog({
                appendTo: '.import-evidences',
                modal: true,
                width: '600px',
                title: 'Successful Validation',
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
                { data: obj => SubString(obj.Username) },
                { data: obj => SubString(obj.EvidenceCode) },
                { data: obj => obj.RowStatus }
            ]
        }).rows.add(data).draw();
    }

    function SubString(text) {
        return text !== '' && text.length > 50
            ? text.substring(0, 47) + '...'
            : text;
    }

    function ToggleNote() {
        let classChevronDown = domElements.btnNoteToggle.hasClass('icon-chevron-down');
        if (classChevronDown) {
            domElements.btnNoteToggle.removeClass('icon-chevron-down').addClass('icon-chevron-right');
            domElements.divNote.prop('hidden', true);
        } else {
            domElements.btnNoteToggle.removeClass('icon-chevron-right').addClass('icon-chevron-down');
            domElements.divNote.prop('hidden', false);
        }        
    }

    function HiddenNote() {
        let classChevronDown = domElements.btnNoteToggle.hasClass('icon-chevron-down');
        if (classChevronDown) {
            domElements.btnNoteToggle.removeClass('icon-chevron-down').addClass('icon-chevron-right');
            domElements.divNote.prop('hidden', true);
        }
    }

    function Initial() {
        domElements.divNote.prop('hidden', false);
        domElements.divResultParseFile.prop('hidden', true);
        domElements.divResult.prop('hidden', true);
        domElements.divValidation.prop('hidden', true);        
        domElements.btnImport.prop('disabled', !isHostPermission);

        domElements.btnDownloadTemplate.on('click', function () {
            common.downloadFile(common.modifyRelativeUrl('ImportEvidences/DownloadTemplateExcel'), '0');
        });

        domElements.inputFileUpload.on('change', function () {
            domElements.spanFileName.text(domElements.inputFileUpload.get(0).files[0].name);
        });

        domElements.btnResultValidationClose.on('click', function () {
            domElements.divValidation.dialog('close');
        });

        domElements.btnNoteToggle.on('click', () => ToggleNote());        

        if(isHostPermission)
            domElements.btnImport.on('click', () => ImportEvidences());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ImportEvidences);