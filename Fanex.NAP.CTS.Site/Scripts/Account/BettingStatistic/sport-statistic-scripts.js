function turnover_by_sport() {
    this.DisplayChart = function () {
        Highcharts.chart('chart-sport', {
            chart: {
                type: 'pie',
                height: 135,
                marginTop: 0,
                marginBottom: 0,
                marginRight: 0,
                marginLeft: -120,
                spacing: [12, 35, 35, 12],
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
                    dataLabels: {
                        enabled: true,
                        distance: -20,
                        format: '{point.percentage:.1f} %',
                        filter: {
                            property: 'percentage',
                            operator: '>',
                            value: 4
                        }
                    },
                    showInLegend: true
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
    };
}