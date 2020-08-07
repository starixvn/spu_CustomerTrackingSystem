function turnover_by_bettype() {
    let betTypeCategories = null;
    let liveIndicatorList = null;
    let custId = $("#hdCustId").val();
    let username = $("#hdUsername").val();

    let domElements = {
        statisticLoading: $('#statistic_bettype_loading')
    }
    
    function GetTurnOverByBetTypeData(custId) {
        domElements.statisticLoading.removeClass('hide');

        $.ajax(common.modifyRelativeUrl('Account/GetPerformanceByBettype'), {
            type: 'POST',
            data: {
                custId: custId,
                getTop: 3
            },
            success: function (response) {
                RenderDataToUI(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function RenderDataToUI(response) {
        betTypeCategories = response.betTypeCategories;
        liveIndicatorList = response.liveIndicatorList;
        DisplayChart();

        domElements.statisticLoading.addClass('hide');
    }

    function DisplayChart() {
        Highcharts.setOptions({
            lang: {
                thousandsSep: ','
            }
        });
        Highcharts.chart('bet-type', {
            chart: {
                type: 'bar',
                marginRight: 0,
                spacingLeft: 32,
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                }
            },
            credits: {
                enabled: false
            },
            colors: ['#EBEBEB','#FF4343', '#3773CC'],
            title: {
                text: ''
            },
            xAxis: {
                categories: betTypeCategories,
                startOnTick: false,
                endOnTick: false,
                lineWidth: 0,
                tickLength: 0,
                labels: {
                    style: {
                        color: '#333333',
                        fontWeight: 'normal',
                        fontSize: '12px',
                    }
                }
            },
            yAxis: {
                min: 0,
                visible: false,
            },
            legend: {
                enabled: false
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: "{series.name}: {point.y}<br/>"
            },
            plotOptions: {
                series: {
                    stacking: 'normal',
                    pointWidth: 8,
                    pointPadding: 0,
                    groupPadding: 0.2,
                    borderWidth: 0,
                    shadow: false,
                },
            },
            series: liveIndicatorList
        });
    };

    this.Initial = function (custId) {
        GetTurnOverByBetTypeData(custId);
    }
}