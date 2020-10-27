function AssociationReportDetail() {
    let domElements = {
        tblGeneralDetail: $('#tblGeneralDetail'),
    };


    function LoadGridAssociationReportDetail() {
        return domElements.tblGeneralDetail.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 20,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [0], width: "9%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
            ],
            data: [
                {
                    "no": "1",
                    "UserName": "12Bet001",
                    "Status": "Open",
                    "Account_Date": "27/10/2020",
                    "Associated_Status": "...",
                    "Associated_Type": "Device",
                },
                {
                    "no": "1",
                    "UserName": "12Bet001",
                    "Status": "Open",
                    "Account_Date": "27/10/2020",
                    "Associated_Status": "...",
                    "Associated_Type": "Device",
                },
                {
                    "no": "1",
                    "UserName": "12Bet001",
                    "Status": "Open",
                    "Account_Date": "27/10/2020",
                    "Associated_Status": "...",
                    "Associated_Type": "Device",
                },
                {
                    "no": "1",
                    "UserName": "12Bet001",
                    "Status": "Open",
                    "Account_Date": "27/10/2020",
                    "Associated_Status": "...",
                    "Associated_Type": "Device",
                },
            ],
            columns: [
                { data: obj => obj.no },
                {
                    data: obj => () => {
                        return html = `<a class='click-view-name'> ${obj.UserName} </a>`;
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-name', cell).click(() => {
                            console.log(row, 'click name');
                        });
                    }
                },
                { data: obj => obj.Status },
                { data: obj => obj.Account_Date },
                {
                    data: obj => AssociationStatus(obj.IsLinked), createdCell: (cell, _, row) => {
     
                            $('.toggle', cell).click(function (e) {
                                e.preventDefault();
    
                            });
                    }
                },
                { data: obj => obj.Associated_Type },
            ],
            createdRow: function (row, obj) {
                $(row).attr('evidence-code', obj.EvidenceCode);
                if (obj.IsRetract) {
                    $(row).attr('cust-retract', true);
                }
            },
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        var html = '';
                        if (data[i].IsRetract) {
                            html = '<tr class="affected" evidence-code="' + data[i].EvidenceCode + '">';
                            html += '<td colspan="5" class="has-tooltip">';
                            html += '<span class="evidence-retract">' + group + '</span>';
                            html += '<td class="text-center">';
                            html += '</td></tr>';
                        } else {
                            html = '<tr class="affected" evidence-code="' + data[i].EvidenceCode + '">';
                            html += '<td class="affected-group" colspan="5">';
                            html += '<span class="icon icon-chevron-right""></span>';
                            html += "<span class='name-group'> 12Bet </span>";
                            html += '<td class="text-left">';
                            html += "<span class='name-group total-accounts'> Total: 3 Association Accounts </span>"
                            html += '</td></tr>';
                        }
                        $(rows).eq(i).before(html);
                        last = group;
                        $('.affected').click(() => {
                            $('.affected-group .icon').toggleClass('active-icon');
                           
                        });
                    }
              
                });
            }

        });
    }

    function AssociationStatus(isLinked) {
        var html = '<div class="toggle toggle-on" id="switch">';
        html += '<div class="toggle-text-off"> Unlinked</div >';
        html += '<div class="glow-comp"></div> <div class="toggle-button"></div>';
        html += '<div class="toggle-text-on"> Linked</div>';
        html += '</div>';

        return html;
    }

    function Initial() {
        LoadGridAssociationReportDetail();
    }


    $(document).ready(function () {
        Initial();
    });
}
$(AssociationReportDetail);