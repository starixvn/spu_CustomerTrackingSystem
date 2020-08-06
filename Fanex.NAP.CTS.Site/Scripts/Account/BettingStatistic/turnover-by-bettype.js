function turnover_by_bettype() {
    let betTypeCategories = null;
    let liveIndicatorList = null;
    let cookiedata = null;
    let custId = $("#hdCustId").val();
    let username = $("#hdUsername").val();

    let domElements = {
        statisticLoading: $('#statistic_bettype_loading')
    }
    
    function GetTurnOverByBetTypeData(custId, expireCookie) {
        domElements.statisticLoading.removeClass('hide');

        if (cookiedata == 0) {
            $.ajax(common.modifyRelativeUrl('Account/GetPerformanceByBettype'), {
                type: 'POST',
                data: {
                    custId: custId,
                    getTop: 4
                },
                success: function (response) {
                    SetBettingStatisticToCookie(custId + '-' + username + "-ByBettype", response, expireCookie);
                    RenderDataToUI(response);                 
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
        else {
            RenderDataToUI(JSON.parse(cookiedata));
        }
    }

    function RenderDataToUI(response) {
        betTypeCategories = response.betTypeCategories;
        liveIndicatorList = response.liveIndicatorList;
        DisplayChart();

        domElements.statisticLoading.addClass('hide');
    }

    function DisplayChart() {
        Highcharts.setOptions({
            lang: {
                thousandsSep: ','
            }
        });
        Highcharts.chart('bet-type', {
            chart: {
                type: 'bar',
                marginRight: 0,
                spacingLeft: 32,
                style: {
                    fontFamily: '"Lato", Arial, Tahoma, sans-serif'
                }
            },
            credits: {
                enabled: false
            },
            colors: ['#EBEBEB','#FF4343', '#3773CC'],
            title: {
                text: ''
            },
            xAxis: {
                categories: betTypeCategories,
                startOnTick: false,
                endOnTick: false,
                lineWidth: 0,
                tickLength: 0,
                labels: {
                    style: {
                        color: '#333333',
                        fontWeight: 'normal',
                        fontSize: '12px',
                    }
                }
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
            series: liveIndicatorList
        });
    };

    function SetBettingStatisticToCookie(name, value, expireCookie) {
        var cookie = [name, '=', JSON.stringify(value), '; expires=' + expireCookie, , '; path=/;'].join('');
        document.cookie = cookie;
    }

    function GetStatisticByBetTypeFromCookie() {

        if (document.cookie.split(';').some((item) => item.trim().startsWith(custId + '-' + username + '-ByBettype='))) {
            return document.cookie
                .split('; ')
                .find(row => row.startsWith(custId + '-' + username + '-ByBettype='))
                .split('=')[1];
        }

        return 0;
    }


    this.Initial = function (custId, expireCookie) {
        cookiedata = GetStatisticByBetTypeFromCookie();
        GetTurnOverByBetTypeData(custId, expireCookie);
    }
}