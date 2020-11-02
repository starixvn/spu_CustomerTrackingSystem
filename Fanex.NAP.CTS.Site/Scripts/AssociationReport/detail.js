function AssociationReportDetail() {
    let dataTable;    
    let domElements = {
        hdSiteId: $('#hdSiteId'),
        hdCTSCustId: $('#hdCTSCustId'),
        hdUserName: $('#hdUserName'),
        hdFromDate: $('#hdFromDate'),
        hdToDate: $('#hdToDate'),
        hdIsHostPermission: $('#hdIsHostPermission'),
        hdAssociationType: $('#hdAssociationType'),
        lblAccount: $('#lblAccount'),
        lblRangeDate: $('#lblRangeDate'),
        lblAssociatedType: $('#lblAssociatedType'),
        tblAssociationDetail: $('#tblAssociationDetail')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

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
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
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
                        if (isHostPermission) {
                            $('.toggle', cell).click(function (e) {
                                e.preventDefault();

                                ActiveRemove(row, domElements.hdCTSCustId.val(), domElements.hdSiteId.val());
                            });
                        }
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
                        html += '<span class="column-group-site" data-site-id=' + data[i].SiteId + '><i class="icon icon-chevron-down icon-open"></i>' + group + '</span>';
                        html += '</td></tr>';

                        $(rows).eq(i).before(html);
                        last = group;
                    }       
                });
            }
        });
    }

    function RenderAssociationStatus(isLinked) {
        var html = '<div class="toggle ' + (isLinked ? 'toggle-on' : '') + '" id="switch">';
        html += '<div class="toggle-text-off' + (!isHostPermission ? ' disabled"' : '') + '"> Unlinked</div >';
        html += '<div class="glow-comp"></div> <div class="toggle-button ' + (!isHostPermission ? ' disabled"' : '') + '"></div>';
        html += '<div class="toggle-text-on' + (!isHostPermission ? ' disabled"' : '') + '"> Linked</div>';
        html += '</div>';

        return html;
    }

    function ActiveRemove(obj, fromSiteId, fromCTSCustId) {
        common.showConfirmation('Do you want to update Association Status?',
            function () {
                RemoveAssociation(fromSiteId, fromCTSCustId, obj.SiteId, obj.CTSCustId, !obj.IsLinked);
            });
    }

    function RemoveAssociation(fromSiteId, fromCTSCustId, toSiteId, toCTSCustId, isLinked) {
        var url = common.modifyRelativeUrl('AssociationReport/ActiveRemoveAssociation');
        var params = {};
        params.fromSiteId = parseInt(fromSiteId);
        params.fromCTSCustId = parseInt(fromCTSCustId);
        params.toSiteId = toSiteId;
        params.toCTSCustId = toCTSCustId;
        params.isLinked = isLinked;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Updated association successful!', 2000);
                }
                else if (response.errorCode === -1) {
                    common.showError('This association is already unlinked!');
                }
                else {
                    common.showError('Updated association fail!');
                }
            },
            beforeSend: function () {
                domElements.tblAssociationDetail.find('tbody').addClass('loading');
            },          
            complete: function () {
                domElements.tblAssociationDetail.find('tbody').removeClass('loading');
            },
            error: function (error) {
                domElements.tblAssociationDetail.find('tbody').removeClass('loading');
                console.log(error);
            }
        });
    }

    function Initial() {        
        GetAssociationDetailReport();
    }

    $(document).ready(function () {
        Initial();
    });

}

$(AssociationReportDetail);
