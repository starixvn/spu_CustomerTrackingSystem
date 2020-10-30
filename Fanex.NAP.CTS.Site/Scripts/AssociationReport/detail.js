function AssociationReportDetail() {
    let dataTable;
    let domElements = {
        hdUserName: $('#hdUserName'),
        hdFromDate: $('#hdFromDate'),
        hdToDate: $('#hdToDate'),
        hdAssociationType: $('#hdAssociationType'),
        lblAccount: $('#lblAccount'),
        lblRangeDate: $('#lblRangeDate'),
        lblAssociatedType: $('#lblAssociatedType'),
        tblAssociationDetail: $('#tblAssociationDetail')
    };

    function GetAssociationDetailReport() {
        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = GenerateGrid();

        $('tbody', domElements.tblAssociationDetail).on('click', 'tr.group', function () {
            let trGroup = $(this);
            let siteId = trGroup.find('.column-group-site').attr('data-site-id');

            if (siteId !== undefined) {
                $('tr[data-site-id="' + siteId + '"]', domElements.tblAssociationDetail).each(function () {
                    let trCurrent = $(this);
                    if (trCurrent.hasClass('hide')) {
                        trCurrent.removeClass('hide');
                        $('i.icon', trGroup).removeClass('icon-chevron-right').addClass('icon-chevron-down');
                    } else {
                        trCurrent.addClass('hide');
                        $('i.icon', trGroup).removeClass('icon-chevron-down').addClass('icon-chevron-right');
                    }
                });
            }
        });
    } 

    function GenerateGrid() {
        return domElements.tblAssociationDetail.DataTable({
            serverSide: true,
            ajax: {
                url: common.modifyRelativeUrl('AssociationReport/GetAssociationDetail'),
                data: {
                    UserName: domElements.hdUserName.val(),
                    FromDate: domElements.hdFromDate.val(),
                    ToDate: domElements.hdToDate.val(),
                    AssociationType: domElements.hdAssociationType.val()
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblAssociationDetail.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    return response;
                },
                complete: function () {
                    domElements.tblAssociationDetail.find('tbody').removeClass('loading');
                },
                error: function (error) {
                    domElements.tblAssociationDetail.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
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
            lengthMenu: [2, 50, 100, 200],
            pageLength: 2,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [0], width: "10%", visible: false },
                { targets: [1], width: "10%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" }                
            ],
            columns: [
                { data: obj => obj.SiteName },
                { data: obj => obj.UserName },               
                { data: obj => obj.AccountStatus },
                { data: obj => obj.AssociatedDateString },
                {
                    data: obj => RenderAssociationStatus(obj.IsLinked), createdCell: (cell, _, row) => {
     
                            $('.toggle', cell).click(function (e) {
                                e.preventDefault();
                            });
                    }
                },
                { data: obj => obj.AssociaionType }
            ],
            createdRow: function (row, obj) {
                $(row).attr('data-site-id', obj.SiteId);
            },
            drawCallback: function () {
                let api = this.api();
                let rows = api.rows({ page: 'current' }).nodes();
                let data = api.rows({ page: 'current' }).data();
                let last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        let html = '<tr class="group group-site">';
                        html += '<td colspan="5">';
                        html += '<span class="column-group-site" data-site-id=' + data[i].SiteId + '><i class="icon icon-chevron-right icon-open">' + group + '</i></span>';
                        html += '</td></tr>';

                        $(rows).eq(i).before(html);
                        last = group;
                    }       
                });
            }
        });
    }

    function RenderAssociationStatus(isLinked) {
        var html = '<div class="toggle toggle-on" id="switch">';
        html += '<div class="toggle-text-off"> Unlinked</div >';
        html += '<div class="glow-comp"></div> <div class="toggle-button"></div>';
        html += '<div class="toggle-text-on"> Linked</div>';
        html += '</div>';

        return html;
    }

    function Initial() {
        GetAssociationDetailReport();
    }

    $(document).ready(function () {
        Initial();
    });

}

$(AssociationReportDetail);
