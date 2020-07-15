function turnover_by_bettype() {
    this.DisplayChart = function () {
        Highcharts.chart('bet-type', {
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Turnover By Bet Type'
            },
            xAxis: {
                categories: ['Handicap', 'Over/Under', 'Outright', 'Others'],
                startOnTick: false,
                endOnTick: false,
                lineWidth: 0,
            },
            yAxis: {
                min: 0,
                visible: false,
            },
            legend: {
                reversed: true
            },
            plotOptions: {
                series: {
                    stacking: 'normal'
                }
            },
            series: [{
                name: 'None-Live',
                data: [5, 3, 4, 7]
            }, {
                name: 'Live',
                data: [2, 2, 3, 2]
            }, {
                name: 'Joe',
                data: [3, 4, 4, 2]
            }]
        });
    };
}