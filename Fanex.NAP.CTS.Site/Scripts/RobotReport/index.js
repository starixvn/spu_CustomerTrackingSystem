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
        RobotMonthly: new RobotReportTable(common, DateTimeType.Montly),
        RobotDaily: new RobotReportTable(common, DateTimeType.Daily),
        AIRobotMonthly: new AIRobotReportTable(common, DateTimeType.Montly),
        AIRobotDaily: new AIRobotReportTable(common, DateTimeType.Daily)
    }

    function Initial() {
        var toDate, toMonth;
        toDate = toMonth = new Date();

        var minMonth = moment(Date.parse(`01/01/${toMonth.getFullYear()}`)).add(-10, 'year').toDate();
        var maxMonth = moment(Date.parse(`12/01/${toMonth.getFullYear()}`)).add(10, 'year').toDate();

        var fromDate = moment(toDate).add(-6, "day").toDate();
        var fromMonth = moment(toDate).add(-5, "month").toDate();

        domElements.dpkFromDate
            .datepicker({
                dateFormat: 'yy/mm/dd',
                changeMonth: true,
                changeYear: true,
            })
            .datepicker("setDate", fromDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: 'yy/mm/dd',
                changeMonth: true,
                changeYear: true,
            })
            .datepicker("setDate", toDate);

        domElements.dpkFromMonth
            .MonthPicker({
                SelectedMonth: fromMonth,
                Button: false,
                MonthFormat: 'yy/mm',
                AltFormat: 'yy/mm/dd',
                AltField: '#dpkFromMonthServer',
                MinMonth: minMonth,
                MaxMonth: maxMonth
            });

        domElements.dpkToMonth
            .MonthPicker({
                SelectedMonth: toMonth,
                Button: false,
                MonthFormat: 'yy/mm',
                AltFormat: 'yy/mm/dd',
                AltField: '#dpkToMonthServer',
                MinMonth: minMonth,
                MaxMonth: maxMonth
            });

        domElements.btnGenerateMonthly.on("click", function () {
            if (ValidateRangeMonth())
                GenerateMonthly(domElements.cbbRobotType.val());
        })

        domElements.btnGenerateDaily.on("click", function () {
            if (ValidateRangeDate())
                GenerateDaily(domElements.cbbRobotType.val());
        })
    }

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

    function ValidateRangeMonth() {

        let dateFromMonthYearVal = new Date($("#dpkFromMonthServer").val());
        let dateToMonthYearVal = new Date($("#dpkToMonthServer").val());

        if (dateFromMonthYearVal > dateToMonthYearVal) {
            common.showError("Please select a valid range within 12 months with the end month must not be smaller than the start month.");
            return false;
        }

        if (dateFromMonthYearVal < moment(dateToMonthYearVal).add(-11, "month").toDate()) {
            common.showError("Please select a valid range within 12 months with the end month must not be smaller than the start month.");
            return false;
        }

        return true;
    }

    function ValidateRangeDate() {
        let dateToDateVal = domElements.dpkToDate.datepicker("getDate");
        let dateFromDateVal = domElements.dpkFromDate.datepicker("getDate");

        if (dateFromDateVal > dateToDateVal) {
            common.showError("Please select a valid range within 30 days with the end date must not be smaller than the start date.");
            return false;
        }

        if (dateFromDateVal < moment(dateToDateVal).add(-29, "day").toDate()) {
            common.showError("Please select a valid range within 30 days with the end date must not be smaller than the start date.");
            return false;
        }

        return true;
    }

    $(document).ready(function () {
        Initial();

        domElements.cbbRobotType.val('1');
        domElements.cbbSite.val('A');
        domElements.btnGenerateMonthly.click();
        domElements.btnGenerateDaily.click();
    });
}

$(RobotReport);