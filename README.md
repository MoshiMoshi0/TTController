# About
TTController is a service that can control usb connected Thermaltake devices.

It allows great configurability by using various speed controllers and rgb effects.

# Config
## Example
```json
{
  "Profiles": [
    {
      "Name": "Default",
      "Guid": "10af9207-7e67-4581-9d13-506cad5d53c1",
      "Ports": [
        [9802, 8101, 1]
      ],
      "SpeedControllers": [
        {
          "Type": "StaticSpeedController", 
          "Config": {
            "Speed": 50,
            "Trigger": {
              "AlwaysTrigger": {}
            }
          }
        }
      ],
      "Effects": [
        {
          "Type": "DefaultEffect",
          "Config": {
            "Type": "Ripple",
            "Speed": "Normal",
            "Colors": [
              [255, 0, 0]
            ],
            "Trigger": {
              "OneTimeTrigger": {}
            }
          }
        }
      ]
    }
  ],
  "PortConfig": [],
  "CriticalTemperature": {
    "/intelcpu/0/temperature/8": 90
  },
  "TemperatureTimerInterval": 250,
  "DeviceSpeedTimerInterval": 2500,
  "DeviceRgbTimerInterval": 32
}
```

## Documentation
### Speed Controllers:
* StaticSpeedController
* PwmSpeedController

### Effects:
* DefaultEffect
* SnakeEffect
* TemperatureEffect

### Triggers
* AlwaysTrigger
* OneTimeTrigger
* ProcessTrigger
* PulseTrigger
* LogicTrigger

### Root
```json
{
  "List of profiles"
  "Profiles": ["<Profile>"],

  "List of port configs"
  "The values in this list are optional, if PortConfig for a port is not present"
  "the default values will be used"
  "PortConfig": ["<PortConfig>"],

  "Sensor -> Critical Temperature map"
  "If the temperature of a sensor exceeds critical temperature"
  "the speed on all ports is set to 100% ignoring speed controllers"
  "CriticalTemperature": {},

  "Miliseconds between temperature updates"
  "TemperatureTimerInterval": "<int>",

  "Miliseconds between speed updates"
  "DeviceSpeedTimerInterval": "<int>",

  "Miliseconds between rgb updates"
  "DeviceRgbTimerInterval": "<int>"
}
```

##### Examples:
```json
{
  "Profiles": ["..."],
  "PortConfig": ["..."],
  "CriticalTemperature": {
    "/intelcpu/0/temperature/8": 90
  },
  "TemperatureTimerInterval": 250,
  "DeviceSpeedTimerInterval": 2500,
  "DeviceRgbTimerInterval": 32
}
```

---

### Profile
```json
{
  "Name of the profile"
  "Name": "<string>",

  "GUID string"
  "Guid": "<string>",

  "List of port identifiers that this profile controls"
  "Ports": ["<PortIdentifier>"],

  "List of speed controllers"
  "SpeedControllers": ["<SpeedController>"],

  "List of effects"
  "Effects": ["<Effect>"]
}
```

##### Examples:
```json
{
  "Name": "Default",
  "Guid": "10af9207-7e67-4581-9d13-506cad5d53c1",
  "Ports": [
    [9802, 8101, 3],
    [9802, 8101, 2],
    [9802, 8101, 1]
  ],
  "SpeedControllers": ["..."],
  "Effects": ["..."]
}
```

---

### Speed Controller
```json
{
  "The class name of the speed controller"
  "Type": "<string>", 

  "Config json for this speed controller, depends on controller type"
  "Must contain a 'Trigger' property"
  "Config": {}
}
```

##### Examples:
```json
{
  "Type": "StaticSpeedController", 
  "Config": {
    "Speed": 50,
    "Trigger": {
      "AlwaysTrigger": {}
    }
  }
}
```

---

### Effect
```json
{
  "The class name of the effect"
  "Type": "<string>", 

  "Config json for this effect, depends on effect type"
  "Must contain a 'Trigger' property"
  "Config": {}
}
```

##### Examples:
```json
{
  "Type": "DefaultEffect",
  "Config": {
    "Type": "Ripple",
    "Speed": "Normal",
    "Colors": [
      [255, 0, 0]
    ],
    "Trigger": {
      "OneTimeTrigger": {}
    }
  }
}
```

---

### Trigger
```json
{
  "Property where key is trigger class name and value is trigger config"
  "Config depends on trigger type"
  "<type>": {}
}
```

##### Examples:
```json
{
  "AlwaysTrigger": {}
}
```
```json
{
  "ProcessTrigger": {
    "Processes": ["cmd"]
  }
}
```

---

### Port Config
```json
{
  "Port identifier that this config applies to"
  "Key": "<PortIdentifier>",

  "Port config"
  "Value": {
    "Port name"
    "Name": "<string>",

    "Port led count"
    "LedCount": "<int>",

    "Led rotation/offset for rgb effects"
    "LedRotation": "<int>",

    "If true led indexes are reversed"
    "LedReverse": "<bool>"
  }
}
```

##### Examples:
```json
{
  "Key": [9802, 8101, 1],
  "Value": {
    "Name": "Top Left",
    "LedCount": 12,
    "LedRotation": 10,
    "LedReverse": true
  }
}
```