function win_lost() {
    var chart;
    this.DisplayChart = function () {
        let domElements = {
            iconFullScreen: $("#icon-full-screen"),
            wrapperChartWinLost: $("#chart-win-lost"),
        };

        chart = Highcharts.chart('chart-win-lost', {
            chart: {
                zoomType: 'xy',
                spacing: [10, 45, 10, 30],
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
                categories: ['8', '9', '10', '11', '12', '13',
                    '14', '15', '16', '17', '18', '19', '20', '21', '22', '23'],
                crosshair: true,
                labels: {
                    style: {
                        'color': '#333333',
                        'font-size': '12px'
                    }
                },
            }],
            yAxis: [{ // Primary yAxis
                labels: {
                    format: '{value}K',
                    style: {
                        'color': '#333333',
                        'font-size': '12px'
                    }
                },
                title: {
                    text: '',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                },
                maxPadding: 0.01,
                gridLineColor: '#E7E7E7',
                gridLineWidth: 1
            }, { // Secondary yAxis
                title: {
                    text: '',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value}K',
                    style: {
                        'color': '#333333',
                        'font-size': '12px'
                    }
                },
                opposite: true,
                maxPadding: 0.01,
                gridLineColor: '#E7E7E7',
                gridLineWidth: 1
            }],
            tooltip: {
                shared: true
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
            series: [{
                name: 'Turnover',
                type: 'column',
                yAxis: 1,
                pointWidth: 8,
                data: [50, 75, 41, 22, 41, 50, 58, 70, 88, 77, 58, 77, 50, 59, 77],
                color: "#3773CC",
                tooltip: {
                    valueSuffix: 'K'
                }

            }, {
                name: 'Win Lost',
                type: 'line',
                data: [-50, 210, 50, 20, 180, 20, -50, 150, 170, 100, 230, -110, 50, -30, 190],
                color: "#F15D21",
                lineWidth: 1,
                tooltip: {
                    valueSuffix: 'K'
                }
                }],
        });

        domElements.iconFullScreen.on('click', function (e) {
            e.preventDefault();
            chart.fullscreen.toggle();
        })
    };
}