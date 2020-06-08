function ImportEvidences() {

    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        inputFileUpload: $('#inputFileUpload'),
        divResult: $('#divResult'),
        tableResult: $('#tableResult'),
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
        let html = "<span>Import file succesful</span>";

        domElements.divResult.html(html);

        let accountEvidenceList = data.ResultCheckAccountEvidenceList;
        console.log(data);
        GenerateGrid(accountEvidenceList);
    }

    function GenerateGrid(data) {
        return domElements.problemAccountMonitorTable.DataTable({
            serverSide: false,
            ajax: data,
            pagingType: "full_numbers",
            language: {
                lengthMenu: "Page size _MENU_",                
                info: "Displaying _START_ to _END_ of _TOTAL_ item (s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
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
                { data: obj => obj.Error }
            ]
        });
    }

    function Initial() {
        domElements.divResult.prop('hidden', true);
        domElements.btnImport.prop('disabled', !isHostPermission);

        if (isHostPermission)
            domElements.btnImport.on('click', () => ImportEvidences());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ImportEvidences);