class AIRobotReportTable {
    dateTimeType = null;
    dataTable = null;
    domElements = {
        tblRobotReport: null,
        tblFootRobotReport: null,
        robotReport: null,
        btnExportExcel: null
    };
    data = null;
    common = null;

    GenerateGrid() {
        return this.domElements.tblRobotReport.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table">t',
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
            scrollCollapse: true,
            scrollY: '22vh',
            paging: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "5%" },
                { targets: [2], width: "5%" },
                { targets: [3], width: "5%" },
                { targets: [4], width: "5%" },
                { targets: [5], width: "10%" }
            ],
            columns: [
                {
                    data: obj => {
                        if (this.dateTimeType == DateTimeType.Montly) {
                            return obj.GetMonthlyTime;
                        }

                        if (this.dateTimeType == DateTimeType.Daily) {
                            return obj.GetDailyTime;
                        }

                        return "N/A";
                    }
                },
                { data: obj => obj.TI },
                { data: obj => obj.SCE },
                { data: obj => obj.TD },
                { data: obj => obj.OCRD },
                { data: obj => obj.TotalAccount }
            ]
        });
    }

    Initial() {
        var myclass = this;
        this.dataTable = this.GenerateGrid();
        this.domElements.btnExportExcel.on("click", function () {
            myclass.common.downloadFilePostMethod(common.modifyRelativeUrl("RobotReport/ExportAIRobotReport"), myclass.data);
        });
        this.SetTFootHidden(null);
    }

    SetHidden(isHidden) {
        this.domElements.robotReport.attr("hidden", isHidden);
    }

    constructor(common, type) {
        this.common = common;
        this.dateTimeType = type;
        this.domElements.tblRobotReport = $(`#tblAIRobot${type}Report`);
        this.domElements.tblFootRobotReport = $(`#tblAIRobot${type}Report > tfoot > tr`)[0];
        this.domElements.robotReport = $(`#AIRobot${type}ReportDatatable`);
        this.domElements.btnExportExcel = $(`#btnExportExcelAIRobot${type}`);
        this.Initial();
    }

    RenderData(site, fromDate, toDate) {
        this.SetHidden(false);

        this.data = {
            isMonthly: this.dateTimeType == DateTimeType.Montly,
            isDaily: this.dateTimeType == DateTimeType.Daily,
            fromDate: fromDate,
            toDate: toDate,
            sites: site
        };

        var myclass = this;

        $.ajax(common.modifyRelativeUrl('RobotReport/GetAIRobotReport'), {
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(this.data),
            beforeSend: function () {
                myclass.domElements.tblRobotReport.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (response?.data) {
                    myclass.dataTable.clear().rows.add(response?.data.Items).draw();

                    let tFootData = response?.data.Items && response?.data.Items.length > 0 ? response.data.TFoot : null;
                    myclass.SetTFootHidden(tFootData);
                }
                else {
                    myclass.SetTFootHidden(null);
                }
            },
            complete: function (response) {
                myclass.domElements.tblRobotReport.find('tbody').removeClass('loading');
            },
            error: function (xhr, textStatus, error) {
                if (textStatus == 'timeout') {
                    this.tryCount++;
                    if (this.tryCount <= this.retryLimit) {
                        //try again
                        $.ajax(this);
                        return;
                    }
                }

                console.log(error);
            }
        });

        $(".dataTables_scrollBody thead").attr("hidden", true);
    }

    SetTFootHidden(data) {
        let height = data ? 'auto' : 0;

        if (data) {
            var nCells = this.domElements.tblFootRobotReport.getElementsByTagName('th');
            nCells[1].innerHTML = data.TI;
            nCells[2].innerHTML = data.SCE;
            nCells[3].innerHTML = data.TD;
            nCells[4].innerHTML = data.OCRD;
        }
        $(`#tblAIRobot${this.dateTimeType}Report_wrapper .dataTables_scrollFoot`).height(height);
    }
}