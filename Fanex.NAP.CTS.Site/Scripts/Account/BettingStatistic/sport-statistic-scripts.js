function turnover_by_sport() {
    let custId = $("#hdCustId").val();
    let username = $("#hdUsername").val();
    let cookiedata = null;

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
        summaryTop: $('#summary-top'),
        summaryBottom: $('#summary-bottom'),
        summaryStatisticLoading: $('#statistic_summary_loading'),
        sportStatisticLoading: $('#statistic_bysport_loading')
    }

    function GetPerformanceBySport(ctsCustId, expireCookie) {
        domElements.summaryStatisticLoading.removeClass('hide');
        domElements.sportStatisticLoading.removeClass('hide');
        if (cookiedata == 0) {
            $.ajax(common.modifyRelativeUrl('Account/GetPerformanceBySport'), {
                type: 'POST',
                data: {
                    custId: ctsCustId,
                    getTop: 3
                },
                success: function (response) {
                    SetBettingStatisticToCookie(custId + '-' + username + "-BySport", response, expireCookie);
                    RenderDataToUI(response);
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
        else
        {
            RenderDataToUI(JSON.parse(cookiedata));
        }
    }

    function RenderDataToUI(response)
    {
        if (response.totalSportBettingStatistic.BetCount == 0) {
            domElements.chartDefault.addClass('hide');
            domElements.chartEmpty.removeClass('hide');
        }
        else {
            domElements.chartDefault.removeClass('hide');
            domElements.chartEmpty.addClass('hide');
        }

        domElements.summaryTop.html(response.totalSportBettingStatistic.StakeLevel);
        HighlightStakeLevel(response.totalSportBettingStatistic.StakeLevel, domElements.summaryTop);
        domElements.totalstakeTurnOver.html(RoundNumber(response.totalSportBettingStatistic.TurnOver));
        domElements.totalstakeWinLoss.html(RoundNumber(response.totalSportBettingStatistic.WinLoss));
        domElements.totalstakeMargin.html(response.totalSportBettingStatistic.Margin.toFixed(0) + "%");
        domElements.totalstakeBetCount.html(response.totalSportBettingStatistic.BetCount);

        domElements.bestsportSportName.html(response.bestSportBettingStatistic.SportName);
        domElements.summaryBottom.html(response.bestSportBettingStatistic.StakeLevel);
        HighlightStakeLevel(response.bestSportBettingStatistic.StakeLevel, domElements.summaryBottom);
        domElements.bestsportTurnOver.html(RoundNumber(response.bestSportBettingStatistic.TurnOver));
        domElements.bestsportWinLoss.html(RoundNumber(response.bestSportBettingStatistic.WinLoss));
        domElements.bestsportMargin.html(response.bestSportBettingStatistic.Margin.toFixed(0) + "%");
        domElements.bestsportBetCount.html(response.bestSportBettingStatistic.BetCount);

        domElements.summaryStatisticLoading.addClass('hide');

        DisplayPieChart(response.pieChartSportStatisticSessions);
        domElements.sportStatisticLoading.addClass('hide');
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
            return (num / 1000000000).toFixed(0) + "B";
        }

        return num;
    }

    function HighlightStakeLevel(stakeLevel, targetElement)
    {
        if (stakeLevel == "High Stake")
        {
            targetElement.addClass('high-stake');
        }
        else if (stakeLevel == "Medium Stake")
        {
            targetElement.addClass('medium-stake');
        }
        else
        {
            targetElement.addClass('normal-stake');
        }
    }

    function SetBettingStatisticToCookie(name, value, expireCookie) {
        var cookie = [name, '=', JSON.stringify(value), '; expires=' + expireCookie, , '; path=/;'].join('');
        document.cookie = cookie;
    }

    function GetStatisticBySportFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith(custId + '-' + username + '-BySport='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith(custId + '-' + username + '-BySport='))
                .split('=')[1];
        }

        return 0;
    }

    function DisplayPieChart(resource) {
        Highcharts.setOptions({
            lang: {
                thousandsSep: ','
            }
        });
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
            tooltip: {
                formatter: function () {
                    return '<b><small>' + this.key + '</small></b><br>' + 'Turnover: <b>' + Highcharts.numberFormat(this.point.turnOver, 0, '', ',')  +
                        '</b>';
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

    this.Initial = function (ctsCustId, expireCookie) {
        cookiedata = GetStatisticBySportFromCookie();
        GetPerformanceBySport(ctsCustId, expireCookie);
    }
}