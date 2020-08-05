function win_lost() {
    let turnOverAreaData = null;
    let winLossAreaDate = null;
    let days = null;

    let domElements = {
        statisticWinlostLoading: $('#statistic_winlost_loading')
    }
    
    function  DisplayChart (){
        let domElements = {
            iconFullScreen: $("#icon-full-screen"),
            wrapperChartWinLost: $("#chart-win-lost"),
        };
        Highcharts.setOptions({
            lang: {
                thousandsSep: ','
            }
        });

        chart = Highcharts.chart('chart-win-lost', {
            chart: {
                zoomType: 'xy',
                spacing: [10, 23, 10, 10],
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                },
            },
            credits: {
                enabled: false
            },
            title: {
                text: ''
            },
            xAxis: [{
                categories: days,
                crosshair: true,
                labels: {
                    style: {
                        'color': '#333333',
                        'font-size': '9px'
                    },
                    distance: 0,
                    padding: 0
                },
                gridLineColor: '#C4C4C4',
                lineWidth: 2,
                type: 'datetime',
             
            }],
            yAxis: [{
                labels: {
                    formatter: function () {
                        return this.axis.defaultLabelFormatter.call(this);
                    },
                    style: {
                        'color': '#333333',
                        'font-size': '12px'
                    }
                },
                title: {
                    text: 'Turnover',
                    style: {
                        'color': '#333333',
                        'font-size': '13px'
                    }
                },
                plotLines: [{
                    color: '#FF3D3D',
                    width: 2,
                    value: 0
                }],
                maxPadding: 0.01,
                gridLineColor: '#C4C4C4',
                gridLineWidth: 1,
                crosshair: true,
                lineWidth: 1,
                lineColor: '#C4C4C4'
            }, {
                title: {
                    text: 'Win Lost',
                    style: {
                        'color': '#333333',
                        'font-size': '13px'
                    }
                },
                labels: {
                    formatter: function () {
                        return this.axis.defaultLabelFormatter.call(this);
                    } ,
                    style: {
                        'color': '#333333',
                        'font-size': '12px'
                    }
                },
                plotLines: [{
                    color: '#FF3D3D',
                    width: 2,
                    value: 0
                }],
                opposite: true,
                maxPadding: 0.01,
                gridLineColor: '#C4C4C4',
                gridLineWidth: 1,
                crosshair: true,
                lineWidth: 1,
                lineColor: '#C4C4C4'
            }],
            tooltip: {
                shared: true,
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                itemMarginTop: 0,
                itemMarginBottom: 5,
                itemStyle: {
                    fontSize: '13px',
                },
            },
            plotOptions: {
                series: {
                    marker: {
                        radius: 4,
                        fillColor: '#F15D21',
                        lineWidth: 1,
                        lineColor: '#FFFFFF'
                    }
                }
            },

            series: [{
                name: 'Turnover',
                type: 'area',
                opacity: 0.8,
                lineWidth: 0.5,
                color: "#3773CC",
                data: turnOverAreaData,
                marker: {
                    enabled: false,
                    symbol: 'circle',
                    radius: 2
                },
            }, {
                name: 'Win Lost',
                type: 'line',
                yAxis: 1,
                color: "#F15D21",
                lineWidth: 0.8,
               data: winLossAreaDate,
            }]
        });

        domElements.iconFullScreen.on('click', function (e) {
            e.preventDefault();
            chart.fullscreen.toggle();
        })
    };

    function GetTurnOverByBetTypeData(custId) {
        domElements.statisticWinlostLoading.removeClass('hide');

        $.ajax(common.modifyRelativeUrl('Account/GetPerformanceDaily'), {
            type: 'POST',
            data: {
                custId: custId
            },
            success: function (response) {
                turnOverAreaData = response.turnOverAreaData; 
                winLossAreaDate = response.winLossAreaDate;
                days = response.days;
                DisplayChart();

                domElements.statisticWinlostLoading.addClass('hide');
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    this.Initial = function (custId) {
        GetTurnOverByBetTypeData(custId);
    }
}