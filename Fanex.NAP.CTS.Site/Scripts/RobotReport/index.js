const DateTimeType = {
    Daily: 0,
    Montly: 1,
}

const RobotType = {
    All: 0,
    Robot: 1,
    AIRobot: 2
}

function RobotReport() {
    let domElements = {
        cbbRobotType: $('#cbbRobotType'),
        cbbSite: $('#cbbSite'),
        btnGenerateMonthly: $('#btnGenerateMonthly'),
        btnGenerateDaily: $('#btnGenerateDaily'),
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        dpkFromMonth: $('#dpkFromMonth'),
        dpkToMonth: $('#dpkToMonth'),
    };

    let robots = {
        RobotMonthly: new RobotReportTable(DateTimeType.Montly),
        RobotDaily: new RobotReportTable(DateTimeType.Daily),
        AIRobotMonthly: new AIRobotReportTable(DateTimeType.Montly),
        AIRobotDaily: new AIRobotReportTable(DateTimeType.Daily)
    }

    function SetRestrictFromDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");

        domElements.dpkFromDate.datepicker("option", "maxDate", toDateValue);
    }

    function SetRestrictToDate() {
        let fromDateValue = domElements.dpkFromDate.datepicker("getDate");

        domElements.dpkToDate
            .datepicker("option", "minDate", fromDateValue)
            .datepicker("option", "maxDate", moment(fromDateValue).add(30, "day").toDate());
    }

    function SetRestrictFromMonth() {
        let toDateValue = domElements.dpkToMonth.datepicker("getDate");

        domElements.dpkFromMonth.datepicker("option", "maxDate", toDateValue);
    }

    function SetRestrictToMonth() {
        let fromDateValue = domElements.dpkFromMonth.datepicker("getDate");

        domElements.dpkToMonth
            .datepicker("option", "minDate", fromDateValue)
            .datepicker("option", "maxDate", moment(fromDateValue).add(1, "year").toDate());
    }

    function Initial() {
        var toDate, toMonth;
        toDate = toMonth = new Date();

        var fromDate = new Date(toDate);
        fromDate.setDate(toDate.getDate() - 7);
        var fromMonth = new Date(toMonth);
        fromMonth.setMonth(toMonth.getMonth() - 6);

        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", fromDate)
            .change(SetRestrictToDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", toDate)
            .change(SetRestrictFromDate);

        domElements.dpkFromMonth
            .datepicker( {
                format: "mm-yyyy",
                startView: "months", 
                minViewMode: "months"
            });

        domElements.dpkToMonth
            .datepicker( {
                format: "mm-yyyy",
                startView: "months", 
                minViewMode: "months"
            });

        domElements.btnGenerateMonthly.on("click", function () {
            GenerateMonthly(domElements.cbbRobotType.val());
        })

        domElements.btnGenerateDaily.on("click", function () {
            GenerateDaily(domElements.cbbRobotType.val());
        })
    }

    $(document).ready(function () {
        Initial();

        domElements.btnGenerateMonthly.click();
        domElements.btnGenerateDaily.click();
    });

    function GenerateMonthly(robotType) {
        switch (Number(robotType)) {
            case RobotType.Robot:
                {
                    robots.RobotMonthly.RenderData(domElements.cbbSite.val(), domElements.dpkFromMonth.val(), domElements.dpkToMonth.val());
                    robots.AIRobotMonthly.SetHidden(true);
                    break;
                }
            case RobotType.AIRobot:
                {
                    robots.RobotMonthly.SetHidden(true);
                    robots.AIRobotMonthly.RenderData(domElements.cbbSite.val(), domElements.dpkFromMonth.val(), domElements.dpkToMonth.val());
                    break;
                }
            default:
                {
                    robots.RobotMonthly.RenderData(domElements.cbbSite.val(), domElements.dpkFromMonth.val(), domElements.dpkToMonth.val());
                    robots.AIRobotMonthly.RenderData(domElements.cbbSite.val(), domElements.dpkFromMonth.val(), domElements.dpkToMonth.val());
                    break;
                }
        }
    }

    function GenerateDaily(robotType) {
        switch (Number(robotType)) {
            case RobotType.Robot:
                {
                    robots.RobotDaily.RenderData(domElements.cbbSite.val(), domElements.dpkFromDate.val(), domElements.dpkToDate.val());
                    robots.AIRobotDaily.SetHidden(true);
                    break;
                }
            case RobotType.AIRobot:
                {
                    robots.RobotDaily.SetHidden(true);
                    robots.AIRobotDaily.RenderData(domElements.cbbSite.val(), domElements.dpkFromDate.val(), domElements.dpkToDate.val());
                    break;
                }
            default:
                {
                    robots.RobotDaily.RenderData(domElements.cbbSite.val(), domElements.dpkFromDate.val(), domElements.dpkToDate.val());
                    robots.AIRobotDaily.RenderData(domElements.cbbSite.val(), domElements.dpkFromDate.val(), domElements.dpkToDate.val());
                    break;
                }
        }
    }
}

$(RobotReport);