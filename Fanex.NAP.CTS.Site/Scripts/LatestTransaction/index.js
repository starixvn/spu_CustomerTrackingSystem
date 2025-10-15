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
            scrollCollapse: true,          
            paging: false,
            searching: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "8%" },
                { targets: [2], width: "8%" },
                { targets: [3], width: "8%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "6%" },
                { targets: [8], width: "6%" },
                { targets: [9], width: "7%" },
                { targets: [10], width: "7%" },
                { targets: [11] }
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
                { data: obj => obj.SubscriberName },
                { data: obj => obj.Device },
                { data: obj => obj.IP },
                { data: obj => obj.RobotTracking },
                { data: obj => obj.Action },
                { data: obj => obj.ActionResult },
                { data: obj => obj.DeviceType },
                { data: obj => obj.Browser },
                { data: obj => GetHostName(obj.Domain) }
            ]
        });
    }

    function GetHostName(url) {
        if (url != null) {
            let m = url.match(/^https?:\/\/[^/]+/);
            return m ? m[0] : null;
        }

        return null;
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