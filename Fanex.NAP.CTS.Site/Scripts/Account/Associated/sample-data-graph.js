function sample_data_graph() {
    var $this = this;

    $this.GenereateSampleShortest = function () {
        var data = {
            "Username": "Agent001",
            "Type": "Device",
            "children": [{
                "Username": "Master001",
                "Type": "Device",
                "children": [
                    {
                        "Username": "Member001",
                        "Type": "Device",
                        "children": [
                            {
                                "Username": "Member100",
                                "Type": "Device",
                                "children": [
                                    {
                                        "Username": "Member001",
                                        "Type": "Device",
                                        "children": [{
                                            "Username": "Member001",
                                            "Type": "Device",
                                            "children": [
                                                {
                                                    "Username": "Member001",
                                                    "Type": "Device",
                                                    "children": [
                                                        {
                                                            "Username": "Member100",
                                                            "Type": "Device",
                                                            "children": [
                                                                {
                                                                    "Username": "Member001",
                                                                    "Type": "Device",
                                                                    "children": [{
                                                                        "Username": "Member001",
                                                                        "Type": "Device"
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
            "Username": "Agent001",
            "Type": "Device",
            "children": [{
                "Username": "Master001",
                "Type": "Device",
                "children": [
                    {
                        "Username": "Member001",
                        "children": [
                            { "Username": "Member100", "Type": "Device" },
                            { "Username": "Member101", "Type": "IP" },
                            { "Username": "Member102", "Type": "Device" },
                            { "Username": "Member103", "Type": "Device" }
                        ]
                    },
                    {
                        "Username": "Member002",
                        "Type": "Device",
                        "children": [
                            { "Username": "Member104", "Type": "IP" },
                            { "Username": "Member105", "Type": "Device" },
                            { "Username": "Member106", "Type": "Device" },
                            { "Username": "Member107", "Type": "Device" },
                            { "Username": "Member108", "Type": "IP" }]
                    },
                    {
                        "Username": "Member003",
                        "Type": "Device",
                        "children": [{
                            "Username": "Member109",
                            "Type": "IP",
                            "children": [
                                { "Username": "Member110", "Type": "IP" },
                                {
                                    "Username": "Member111",
                                    "Type": "IP",
                                    "children": [
                                        { "Username": "Member112", "Type": "IP" }
                                    ]
                                }
                            ]
                        }]
                    }
                ]
            },
            {
                "Username": "Master002",
                "Type": "Device",
                "children": [
                    { "Username": "Member113", "Type": "IP" },
                    { "Username": "Member114", "Type": "IP" }
                ]
            },
            {
                "Username": "Master003",
                "Type": "IP",
                "children": [
                    { "Username": "Member115", "Type": "IP" }
                ]
            },
            {
                "Username": "Master004",
                "Type": "Device"
            },
            {
                "Username": "Master005",
                "Type": "IP"
            },
            {
                "Username": "Master006",
                "Type": "IP"
            }]
        }

        return data;
    }
}