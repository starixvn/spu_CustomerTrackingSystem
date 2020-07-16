function turnover_by_bettype() {
    this.DisplayChart = function () {
        Highcharts.chart('bet-type', {
            chart: {
                type: 'bar',    
                height: 95,
                marginRight: 0,
                spacingRight: 0,
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                }
            },
            colors: ['#EBEBEB', '#3773CC', '#FF4343'],
            title: {
                text: ''
            },
            xAxis: {
                categories: ['Handicap', 'Over/Under', 'Outright', 'Others'],
                startOnTick: false,
                endOnTick: false,
                lineWidth: 0,
                tickLength: 0
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
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
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
            series: [{
                name: 'None',
                data: [5, 3, 2, 0],
            }, {
                name: 'None-Live',
                data: [2, 2, 2, 7],
            }, {
                name: 'Live',
                data: [3, 5, 6, 3],
            }]
        });

        Highcharts.chart('chart-sport',{
            chart: {
                type: 'pie',
                height: 134,
                margin: [0, 0, 0, 0],
                spacing: [0, 0, 0, 0],
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                }
            },
            colors: ['#F15D21', '#1D2E4F', '#37C2CC'],
            title: {
                text: ''
            },
            accessibility: {
                point: {
                    valueSuffix: '%'
                }
            },

            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        distance: -20,
                        format: '{point.percentage:.1f} %',
                        filter: {
                            property: 'percentage',
                            operator: '>',
                            value: 4
                        }
                    }
                }
            },

            series: [{
                data: [{
                    name: 'Soccer',
                    y: 32,
                    selected: true,
                }, {
                    name: 'Basket ball',
                    y: 45,
                    selected: true,
                }, {
                    name: 'Other Sport',
                    y: 23,
                    selected: true,
                }]
            }]
        });

        Highcharts.chart('chart-win-lost', {
            chart: {
                zoomType: 'xy',
                height: 134,
                margin: [0, 0, 0, 0],
                spacing: [0, 0, 0, 0],
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                }
            },
            title: {
                text: ''
            },
            xAxis: [{
                categories: ['8', '9', '10', '11', '12', '13',
                    '14', '15', '16', '17', '18', '19', '20','21', '22', '23'],
                crosshair: true
            }],
            yAxis: [{ // Primary yAxis
                labels: {
                    format: '{value}K',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                },
                title: {
                    text: 'Win Lost',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                }
            }, { // Secondary yAxis
                title: {
                    text: 'Turnover',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value}K',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                opposite: true
            }],
            tooltip: {
                shared: true
            },
            legend: {
                enabled: false
            },
            series: [{
                type: 'column',
                yAxis: 1,
                data: [-10, 15, 10, 12, 14, 17, 13, 48, 21, 19, 56, 54],
                tooltip: {
                    valueSuffix: 'K'
                }

            }, {
                type: 'spline',
                data: [100, 200, 210, 250, 160, 210, 225, 260, 230, 180, 130, 90],
                tooltip: {
                    valueSuffix: 'K'
                }
            }]
        });
    };
}