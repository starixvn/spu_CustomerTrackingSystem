function turnover_by_bettype() {
    this.DisplayChart = function () {
        Highcharts.chart('bet-type', {
            chart: {
                type: 'bar',    
                height: 125,
                marginRight: 0,
                spacingLeft: 30,
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
                data: [2, 3, 2, 1],
            }, {
                name: 'None-Live',
                data: [1, 0, 1, 1],
            }, {
                name: 'Live',
                data: [0, 0, 0, 1],
            }]
        });
    };
}