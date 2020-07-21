function turnover_by_sport() {
    this.DisplayChart = function () {
        Highcharts.chart('chart-sport', {
            chart: {
                type: 'pie',
                height: 135,
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
    };
}