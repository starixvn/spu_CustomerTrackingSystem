function sample_data_graph() {
    var $this = this;

    $this.GenereateSampleShortest = function () {
        var data = {
            "CustId": "001",
            "ItemCode": "001",
            "Username": "Agent001",
            "DisplayName": "Agent001",
            "AssociationType": "Device",
            "children": [{
                "CustId": "002",
                "ItemCode": "002",
                "Username": "Master001",
                "DisplayName": "Master001",
                "AssociationType": "Device",
                "children": [
                    {
                        "CustId": "003",
                        "ItemCode": "003",
                        "Username": "Member001",
                        "DisplayName": "Member001",
                        "AssociationType": "Device",
                        "children": [
                            {
                                "CustId": "004",
                                "ItemCode": "004",
                                "Username": "Member002",
                                "DisplayName": "Member002",
                                "AssociationType": "Device",
                                "children": [
                                    {
                                        "CustId": "005",
                                        "ItemCode": "005",
                                        "Username": "Member003",
                                        "DisplayName": "Member003",
                                        "AssociationType": "Device",
                                        "children": [{
                                            "CustId": "006",
                                            "ItemCode": "006",
                                            "Username": "Member004",
                                            "DisplayName": "Member004",
                                            "AssociationType": "Device",
                                            "children": [
                                                {
                                                    "CustId": "007",
                                                    "ItemCode": "007",
                                                    "Username": "Member005",
                                                    "DisplayName": "Member005",
                                                    "AssociationType": "Device",
                                                    "children": [
                                                        {
                                                            "CustId": "008",
                                                            "ItemCode": "008",
                                                            "Username": "Member006",
                                                            "DisplayName": "Member006",
                                                            "AssociationType": "Device",
                                                            "children": [
                                                                {
                                                                    "CustId": "009",
                                                                    "ItemCode": "009",
                                                                    "Username": "Member007",
                                                                    "DisplayName": "Member007",
                                                                    "AssociationType": "Device",
                                                                    "children": [{
                                                                        "CustId": "010",
                                                                        "ItemCode": "010",
                                                                        "Username": "Member008",
                                                                        "DisplayName": "Member008",
                                                                        "AssociationType": "Device"
                                                                    }]
                                                                }]
                                                        }]
                                                }]
                                        }]
                                    }]
                            }]
                    }]
            }]
        }

        return data;
    }

    $this.GenereateSampleFullPath = function () {
        var data = {
            "CustId": "001",
            "ItemCode": "001",
            "Username": "Agent001",
            "DisplayName": "Agent001",
            "AssociationType": "Device",
            "children": [{
                "CustId": "002",
                "ItemCode": "002",
                "Username": "Master001",
                "DisplayName": "Master001",
                "AssociationType": "Device",
                "children": [
                    {
                        "CustId": "003",
                        "ItemCode": "003",
                        "Username": "Member001",
                        "DisplayName": "Member001",
                        "children": [
                            { "CustId": "004", "ItemCode": "004", "Username": "Member100", "DisplayName": "Member100", "AssociationType": "Device" },
                            //{ "Username": "Member101", "Type": "IP" },
                            //{ "Username": "Member102", "Type": "Device" },
                            //{ "Username": "Member103", "Type": "Device" }
                        ]
                    }
                    //},
                    //{
                    //    "Username": "Member002",
                    //    "Type": "Device",
                    //    "children": [
                    //        { "Username": "Member104", "Type": "IP" },
                    //        { "Username": "Member105", "Type": "Device" },
                    //        { "Username": "Member106", "Type": "Device" },
                    //        { "Username": "Member107", "Type": "Device" },
                    //        { "Username": "Member108", "Type": "IP" }]
                    //},
                    //{
                    //    "Username": "Member003",
                    //    "Type": "Device",
                    //    "children": [{
                    //        "Username": "Member109",
                    //        "Type": "IP",
                    //        "children": [
                    //            { "Username": "Member110", "Type": "IP" },
                    //            {
                    //                "Username": "Member111",
                    //                "Type": "IP",
                    //                "children": [
                    //                    { "Username": "Member112", "Type": "IP" }
                    //                ]
                    //            }
                    //        ]
                    //    }]
                    //}
                ]
            }
            //    ,
            //{
            //    "Username": "Master002",
            //    "Type": "Device",
            //    "children": [
            //        { "Username": "Member113", "Type": "IP" },
            //        { "Username": "Member114", "Type": "IP" }
            //    ]
            //},
            //{
            //    "Username": "Master003",
            //    "Type": "IP",
            //    "children": [
            //        { "Username": "Member115", "Type": "IP" }
            //    ]
            //},
            //{
            //    "Username": "Master004",
            //    "Type": "Device"
            //},
            //{
            //    "Username": "Master005",
            //    "Type": "IP"
            //},
            //{
            //    "Username": "Master006",
            //    "Type": "IP"
                //}
            ]
        }

        return data;
    }
}