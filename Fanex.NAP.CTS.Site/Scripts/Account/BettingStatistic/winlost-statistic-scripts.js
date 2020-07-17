function win_lost() {
    this.DisplayChart = function () {
        Highcharts.chart('chart-win-lost', {
            chart: {
                zoomType: 'xy',
                height: 124,
                width: 400,
                spacingLeft: 30,
                spacingRight: 0,
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
                enabled: false
            },
            series: [{
                name: '',
                type: 'column',
                yAxis: 1,
                pointWidth: 8,
                data: [50, 75, 41, 22, 41, 50, 58, 70, 88, 77, 58, 77, 50, 59, 77],
                color: "#3773CC",
                tooltip: {
                    valueSuffix: 'K'
                }

            }, {
                name: '',
                type: 'line',
                data: [-50, 210, 50, 20, 180, 20, -50, 150, 170, 100, 230, -110, 50, -30, 190],
                color: "#F15D21",
                lineWidth: 1,
                tooltip: {
                    valueSuffix: 'K'
                }
            }]
        });
    };
}