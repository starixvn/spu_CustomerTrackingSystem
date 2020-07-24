function turnover_by_sport() {
    this.DisplayChart = function () {
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
            colors: ['#F15D21', '#1D2E4F', '#37C2CC'],
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
                        distance: -30,
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
                    },
                    showInLegend: true,
                },
                series: {
                    shadow: true
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
                    dataLabels: {
                        color: '#fff'
                    }
                }, {
                    name: 'Other Sport',
                    y: 23,
                    selected: true,
                }]
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
}