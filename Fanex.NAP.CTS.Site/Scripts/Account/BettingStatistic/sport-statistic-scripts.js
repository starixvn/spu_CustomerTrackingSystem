function turnover_by_sport() {
    let domElements = {
        totalstakeTurnOver: $('#totalstake_turnover'),
        totalstakeWinLoss: $('#totalstake_winloss'),
        totalstakeMargin: $('#totalstake_margin'),
        totalstakeBetCount: $('#totalstake_betcount'),
        bestsportSportName: $('#bestsport_sportname'),
        bestsportTurnOver: $('#bestsport_turnover'),
        bestsportWinLoss: $('#bestsport_winloss'),
        bestsportMargin: $('#bestsport_margin'),
        bestsportBetCount: $('#bestsport_betcount'),
        chartDefault: $('#chart-default'),
        chartEmpty: $('#chart_empty'),

    }

    function DisplayPieChart(resource) {
        Highcharts.chart('chart-sport', {
            chart: {
                type: 'pie',
                marginTop: 0,
                marginBottom: 0,
                marginRight: 10,
                marginLeft: 0,
                spacing: [12, 35, 35, 12],
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                },
            },
            credits: {
                enabled: false
            },
            colors: ['#F15D21', '#FFD643', '#1D2E4F', '#37C2CC'],
            title: {
                text: ''
            },
            accessibility: {
                point: {
                    valueSuffix: '%'
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                y: 14,
                x: 0,
                padding: 0,
                itemMarginTop: 0,
                itemMarginBottom: 5,
                itemStyle: {
                    fontSize: '13px',
                },
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    borderWidth: 1,
                    borderColor: null,
                    dataLabels: {
                        color: '#000000',
                        enabled: true,
                        distance: -25,
                        format: '{point.percentage} %',
                        filter: {
                            property: 'percentage',
                            operator: '>',
                            value: 1
                        },
                        style: {
                            fontWeight: 'normal',
                            fontSize: '12px',
                        },
                        connectorColor: 'inherit',
                        connectorPadding: 0
                    },
                    showInLegend: true,
                },
                series: {
                    shadow: true
                }
            },

            series: [{
                data: resource
            }],
            responsive: {
                rules: [{
                    condition: {
                        maxWidth: 350
                    },
                    chartOptions: {
                        chart: {
                            marginLeft: -110,
                        },
                    }
                }]
            }
        });
    };

    function GetPerformanceBySport(ctsCustId) {
        $.ajax(common.modifyRelativeUrl('Account/GetPerformanceBySport'), {
            type: 'POST',
            data: {
                custId: ctsCustId,
                getTop: 3
            },
            success: function (response) {
                if (response.totalSportBettingStatistic.BetCount == 0) {
                    domElements.chartDefault.addClass('hide');               
                    domElements.chartEmpty.removeClass('hide');
                }
                else {
                    domElements.chartDefault.removeClass('hide');
                    domElements.chartEmpty.addClass('hide');
                }

                domElements.totalstakeTurnOver.html(RoundNumber(response.totalSportBettingStatistic.TurnOver));
                domElements.totalstakeWinLoss.html(RoundNumber(response.totalSportBettingStatistic.WinLoss));
                domElements.totalstakeMargin.html(response.totalSportBettingStatistic.Margin.toFixed(0) + "%");
                domElements.totalstakeBetCount.html(response.totalSportBettingStatistic.BetCount);

                domElements.bestsportSportName.html(response.bestSportBettingStatistic.SportName);
                domElements.bestsportTurnOver.html(RoundNumber(response.bestSportBettingStatistic.TurnOver));
                domElements.bestsportWinLoss.html(RoundNumber(response.bestSportBettingStatistic.WinLoss));
                domElements.bestsportMargin.html(response.bestSportBettingStatistic.Margin.toFixed(0) + "%");
                domElements.bestsportBetCount.html(response.bestSportBettingStatistic.BetCount);

                DisplayPieChart(response.pieChartSportStatisticSessions);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function RoundNumber(number)
    {
        if (number == 0) return 0;

        var num = number.toFixed(0);

        if ((num > 999 && num <= 999999) || (num <= -1000 && num > -999999)) {
            return (num / 1000).toFixed(0) + "K";
        }
        else if ((num > 999999 && num <= 999999999) || (num <= -1000000 && num > -999999999)) {
            return (num / 1000000).toFixed(0) + "M";
        }
        else if (num >= 1000000000 || num <= -1000000000) {
            return (num / 1000000).toFixed(0) + "M";
        }

        return num;
    }

    this.Initial = function (ctsCustId) {
        GetPerformanceBySport(ctsCustId);
    }
}