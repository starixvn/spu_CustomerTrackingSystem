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

    function Initial() {
        var toDate, toMonth;
        toDate = toMonth = new Date();

        var fromDate = moment(toDate).add(-7, "day").toDate();
        var fromMonth = moment(toDate).add(-6, "month").toDate();

        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
            })
            .datepicker("setDate", fromDate)
            .change(SetRestrictToDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true,
            })
            .datepicker("setDate", toDate)
            .change(SetRestrictFromDate);

        domElements.dpkFromMonth
            .MonthPicker({
                SelectedMonth: fromMonth,
                Button: false,
                MonthFormat: 'M, yy',
                AltFormat: 'yy/mm/dd',
                AltField: '#dpkFromMonthServer',
                OnAfterChooseMonth: function (selectedDate) {
                    // Do something with selected JavaScript date.
                    domElements.dpkToMonth.MonthPicker('option', 'MinMonth', selectedDate);
                    domElements.dpkToMonth.MonthPicker('option', 'MaxMonth', moment(selectedDate).add(1, "year").toDate())

                    var dateToMonthYearVal = new Date($("#dpkToMonthServer").val());
                    if (selectedDate > dateToMonthYearVal) {
                        domElements.dpkToMonth.MonthPicker('option', 'SelectedMonth', selectedDate);
                    }

                    if (selectedDate < moment(dateToMonthYearVal).add(-1, "year").toDate()) {
                        domElements.dpkToMonth.MonthPicker('option', 'SelectedMonth', moment(selectedDate).add(1, "year").toDate());
                    }
                }
            });

        domElements.dpkToMonth
            .MonthPicker({
                SelectedMonth: toMonth,
                Button: false,
                MonthFormat: 'M, yy',
                AltFormat: 'yy/mm/dd',
                AltField: '#dpkToMonthServer',
                OnAfterChooseMonth: function (selectedDate) {
                    // Do something with selected JavaScript date.
                    domElements.dpkFromMonth.MonthPicker('option', 'MinMonth', moment(selectedDate).add(-1, "year").toDate());
                    domElements.dpkFromMonth.MonthPicker('option', 'MaxMonth', selectedDate)

                    var dateFromMonthYearVal = new Date($("#dpkFromMonthServer").val());
                    if (selectedDate < dateFromMonthYearVal) {
                        domElements.dpkFromMonth.MonthPicker('option', 'SelectedMonth', selectedDate);
                    }

                    if (selectedDate > moment(dateFromMonthYearVal).add(1, "year").toDate()) {
                        domElements.dpkFromMonth.MonthPicker('option', 'SelectedMonth', moment(selectedDate).add(-1, "year").toDate());
                    }
                }
            });

        domElements.btnGenerateMonthly.on("click", function () {
            GenerateMonthly(domElements.cbbRobotType.val());
        })

        domElements.btnGenerateDaily.on("click", function () {
            GenerateDaily(domElements.cbbRobotType.val());
        })

        // $('#NoIconDemo').MonthPicker({
        //     Button: false,
        //     MonthFormat: 'M, yy'
        // });
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