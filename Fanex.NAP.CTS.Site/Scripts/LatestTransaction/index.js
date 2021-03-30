function LatestTransaction() {
    let dataTable;

    let domElements = {
        cbbSubscriber: $('#cbbSubscriber'),
        cbbShowTheLast: $('#cbbShowTheLast'),
        btnGenerate: $('#btnGenerate'),
        tblLatestTransaction: $('#tblLatestTransaction')
    };

    function GetLatestTransaction() {
        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();
    }

    function GenerateGrid() {
        return domElements.tblLatestTransaction.DataTable({
            ajax: {
                url: common.modifyRelativeUrl('LatestTransaction/GetLatestTransaction'),
                data: {
                    subscriberId: function () {
                        return domElements.cbbSubscriber.val();
                    },
                    length: function () {
                        return domElements.cbbShowTheLast.val();
                    }
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblLatestTransaction.find('tbody').addClass('loading');
                    domElements.btnGenerate.prop('disabled', true);
                },
                dataFilter: function (response) {
                    return response;
                },
                complete: function () {
                    domElements.tblLatestTransaction.find('tbody').removeClass('loading');
                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.tblLatestTransaction.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
            language: {
                zeroRecords: "There's no data"
            },
            info: false,
            ordering: false,
            searching: false,
            scrollX: false,
            paging: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "5%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "5%" },
                { targets: [9], width: "10%" },
                { targets: [10], width: "10%" }
            ],
            columns: [               
                { data: obj => obj.TransTimeString },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);                        
                        return html = `<a class='click-view-name' href=${url}> ${obj.UserName} </a>`;
                    }
                },
                { data: obj => obj.LoginID },
                { data: obj => obj.Device },
                { data: obj => obj.IP },
                { data: obj => obj.RobotTracking },
                { data: obj => obj.Action },
                { data: obj => obj.ActionResult },
                { data: obj => obj.DeviceType },
                { data: obj => obj.Browser },
                { data: obj => obj.Domain }
            ]
        });
    }

    function Initial() {
        domElements.cbbSubscriber.select2();

        domElements.btnGenerate.on('click', function () { GetLatestTransaction(); });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(LatestTransaction);