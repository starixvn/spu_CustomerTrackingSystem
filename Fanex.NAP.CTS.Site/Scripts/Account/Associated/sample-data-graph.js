function sample_data_graph() {
    this.genereateSampleShortest = function () {
        return {
            "nodes": [
                {
                    "properties": {
                        "Username": "Agent007"
                    },
                    "labels": ["Customer"],
                    "id": 0
                },
                {
                    "properties": {
                        "Username": "Super001"
                    },
                    "labels": ["Customer"],
                    "id": 1
                },
                {
                    "properties": {
                        "Username": "Member17"
                    },
                    "labels": ["Customer"],
                    "id": 2
                },
                {
                    "properties": {
                        "Username": "M4A1000"
                    },
                    "labels": ["Customer"],
                    "id": 3
                },
                {
                    "properties": {
                        "Username": "SujuMninha"
                    },
                    "labels": ["Customer"],
                    "id": 4
                },
                {
                    "properties": {
                        "Username": "FCManches"
                    },
                    "labels": ["Customer"],
                    "id": 5
                },
                {
                    "properties": {
                        "DeviceId": "000100"
                    },
                    "labels": ["Device"],
                    "id": 100
                }, 
                {
                    "properties": {
                        "DeviceId": "000101"
                    },
                    "labels": ["Device"],
                    "id": 101
                },                
                {
                    "properties": {
                        "DeviceId": "000102"
                    },
                    "labels": ["Device"],
                    "id": 102
                },
                {
                    "properties": {
                        "DeviceId": "000103"
                    },
                    "labels": ["Device"],
                    "id": 103
                },
                {
                    "properties": {
                        "DeviceId": "000104"
                    },
                    "labels": ["Device"],
                    "id": 104
                },
                {
                    "properties": {
                        "DeviceId": "000105"
                    },
                    "labels": ["Device"],
                    "id": 105
                },
                {
                    "properties": {
                        "DeviceId": "000106"
                    },
                    "labels": ["Device"],
                    "id": 106
                }
            ],
            "edges": [
                {
                    "id": 0,
                    "type": "USED",
                    "startNode": 0,
                    "endNode": 100,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },    
                {
                    "id": 2,
                    "type": "USED",
                    "startNode": 1,
                    "endNode": 100,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 3,
                    "type": "USED",
                    "startNode": 1,
                    "endNode": 101,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 4,
                    "type": "USED",
                    "startNode": 2,
                    "endNode": 101,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 5,
                    "type": "USED",
                    "startNode": 2,
                    "endNode": 102,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 6,
                    "type": "USED",
                    "startNode": 3,
                    "endNode": 102,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 7,
                    "type": "USED",
                    "startNode": 3,
                    "endNode": 103,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 8,
                    "type": "USED",
                    "startNode": 4,
                    "endNode": 103,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 9,
                    "type": "USED",
                    "startNode": 4,
                    "endNode": 104,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 10,
                    "type": "USED",
                    "startNode": 5,
                    "endNode": 104,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 11,
                    "type": "USED",
                    "startNode": 5,
                    "endNode": 105,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 12,
                    "type": "USED",
                    "startNode": 0,
                    "endNode": 106,
                    "properties": {
                        "roles": ["Neo"]
                    }
                }
            ]
        }
    }

    this.genereateSampleFullPath = function () {
        return {
            "nodes": [
                {
                    "properties": {
                        "Username": "Agent007"
                    },
                    "labels": ["Customer"],
                    "id": 0
                },
                {
                    "properties": {
                        "Username": "Super001"
                    },
                    "labels": ["Customer"],
                    "id": 1
                },
                {
                    "properties": {
                        "Username": "Member17"
                    },
                    "labels": ["Customer"],
                    "id": 2
                },
                {
                    "properties": {
                        "Username": "M4A1000"
                    },
                    "labels": ["Customer"],
                    "id": 3
                },
                {
                    "properties": {
                        "Username": "SujuMninha"
                    },
                    "labels": ["Customer"],
                    "id": 4
                },
                {
                    "properties": {
                        "Username": "FCMU"
                    },
                    "labels": ["Customer"],
                    "id": 5
                },
                {
                    "properties": {
                        "Username": "FCManCity"
                    },
                    "labels": ["Customer"],
                    "id": 6
                },
                {
                    "properties": {
                        "Username": "FCTottenham"
                    },
                    "labels": ["Customer"],
                    "id": 7
                },
                {
                    "properties": {
                        "Username": "FCTEverton"
                    },
                    "labels": ["Customer"],
                    "id": 8
                },
                {
                    "properties": {
                        "Username": "FCArsenal"
                    },
                    "labels": ["Customer"],
                    "id": 9
                },
                {
                    "properties": {
                        "Username": "FCChelsea"
                    },
                    "labels": ["Customer"],
                    "id": 10
                },
                {
                    "properties": {
                        "Username": "FCFullham"
                    },
                    "labels": ["Customer"],
                    "id": 11
                }
            ],
            "edges": [
                {
                    "id": 0,
                    "type": "IP",
                    "startNode": 0,
                    "endNode": 1,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 1,
                    "type": "Device",
                    "startNode": 0,
                    "endNode": 2,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 2,
                    "type": "Device/IP",
                    "startNode": 0,
                    "endNode": 3,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 3,
                    "type": "IP",
                    "startNode": 0,
                    "endNode": 4,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 4,
                    "type": "Device",
                    "startNode": 0,
                    "endNode": 5,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 5,
                    "type": "IP",
                    "startNode": 0,
                    "endNode": 6,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 6,
                    "type": "IP",
                    "startNode": 0,
                    "endNode": 7,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 7,
                    "type": "Device/IP",
                    "startNode": 0,
                    "endNode": 8,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 8,
                    "type": "IP",
                    "startNode": 0,
                    "endNode": 9,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 9,
                    "type": "Device/IP",
                    "startNode": 0,
                    "endNode": 10,
                    "properties": {
                        "roles": ["Neo"]
                    }
                },
                {
                    "id": 10,
                    "type": "Device",
                    "startNode": 0,
                    "endNode": 11,
                    "properties": {
                        "roles": ["Neo"]
                    }
                }                
            ]
        }
    }
}