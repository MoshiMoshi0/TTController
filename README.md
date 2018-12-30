# About
TTController is a service that allows you to control all Thermaltake devices.

TODO:
* Write full documentation
* Add GUI to control the service (maybe)

# Example config
```json
{
  "Profiles": [
    {
      "Name": "Default",
      "Guid": "10af9207-7e67-4581-9d13-506cad5d53c1",
      "Ports": [
        [9802, 8101, 3],
        [9802, 8101, 4],
        [9802, 8101, 5],
        [9802, 8101, 2],
        [9802, 8101, 1]
      ],
      "SpeedControllers": [
        {
          "Type": "StaticSpeedController", 
          "Config": {
            "Speed": 50,
            "Trigger": {
              "ProcessTrigger": {
                "Processes": ["cmd"]
              }
            }
          }
        },
        {
          "Type": "PwmSpeedController", 
          "Config": {
            "CurvePoints": [
              [50, 20],
              [60, 25],
              [70, 40],
              [80, 75],
              [85, 100]
            ],
            "Sensors": [
              "/intelcpu/0/temperature/8"
            ],
            "SensorMixFunction": "Maximum",
            "MinimumChange": 4,
            "MaximumChange": 8,
            "Trigger": {
                "AlwaysTrigger": {}
            }
          }
        }
      ],
      "Effects": [
        {
          "Type": "FullColorEffect",
          "Config": {
            "Color": [255, 0, 0],
            "Trigger": {
              "OneTimeTrigger": {}
            }
          }
        }
      ]
    }
  ],
  "PortConfig": [
    {
      "Key": [9802, 8101, 2],
      "Value": {
        "Name": "Top Left",
        "LedCount": 12,
        "LedRotation": 10,
        "LedReverse": true
      }
    },
    {
      "Key": [9802, 8101, 1],
      "Value": {
        "Name": "Top Right",
        "LedCount": 12,
        "LedRotation": 10,
        "LedReverse": false
      }
    }
  ],
  "TemperatureTimerInterval": 250,
  "DeviceSpeedTimerInterval": 2500,
  "DeviceRgbTimerInterval": 32
}
```
