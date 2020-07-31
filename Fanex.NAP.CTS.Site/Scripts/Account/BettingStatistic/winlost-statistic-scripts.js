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
                categories: [1,2,3,4,5,6,7,8, 9, 10, 11, 12, 13,
                    14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
                    27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44,
                    45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63,
                    64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                    81, 82, 83, 84, 85, 86, 87, 88, 89, 90],
                crosshair: true,
                labels: {
                    style: {
                        'color': '#333333',
                        'font-size': '12px'
                    }
                },
                gridLineColor: '#C4C4C4',
                lineWidth: 2
            }],
            yAxis: [{
                labels: {
                    format: '{value}K',
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
                    format: '{value}K',
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
                data: [50, 30, 41, 32, 41, 50, 58, 70, 88, 77, 58, 77, 50, 59, 77, 70, 70, 81, 52, 51, 80, 58, 70, 98, 67, 68, 77, 40, 58, 75
                    , 40, 80, 31, 52, 31, 40, 48, 60, 78, 67, 48, 67, 45, 54, 72, 42, 23, 34, 75, 35, 43, 56, 68, 86, 75, 56, 75, 48, 57, 75
                    , 60, 80, 91, 62, 51, 50, 38, 50, 68, 57, 38, 57, 90, 39, 57, 60, 50, 61, 62, 51, 60, 68, 80, 98, 87, 68, 87, 60, 69, 87],
                tooltip: {
                    valueSuffix: 'K'
                },
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
                tooltip: {
                    valueSuffix: 'K'
                },

                data: [200, 150, 150, 130, 180, 200, 150, 150, 170, 100, 230, 125, 100, 130, 190, -50, 50, 100, 120, -100, -50, -50, -20, 50, 100, 130, -100, -120, -200, 390
                    , -150, -110, -120, -100, -50, -100, -150, -125, -70, -25, 10, -20, 50, -30, 100, 80, 60, 100, 70, 60, 150, 120, 135, 120, 100, 75, 65, 75, 85, 95
                    , 100, 110, 90, 120, 130, 120, 60, 150, 170, 100, 230, 56, 100, 30, 290, 150, 210, 100, 120, 115, 95, 200, 150, 170, 100, 130, 90, 100, 30, 190],

            }]
        });

        domElements.iconFullScreen.on('click', function (e) {
            e.preventDefault();
            chart.fullscreen.toggle();
        })
    };
}