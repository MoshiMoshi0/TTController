# About
TTController is a service that can control usb connected Thermaltake devices.

It allows great configurability by using various speed controllers and rgb effects.

# Device support
| Device                      | Support            | Confirmed          |
|-----------------------------|--------------------|--------------------|
| Riing Plus 12/14/20         | :heavy_check_mark: | :heavy_check_mark: |
| Pure Plus 12/14             | :heavy_check_mark: | :x:                |
| Floe Riing RGB 240/280/360  | :heavy_check_mark: | :x:                |
| WaterRam RGB                | :heavy_check_mark: | :x:                |
| Pacific PR22-D5 Plus        | :heavy_check_mark: | :x:                |
| Pacific CL360/RL360         | :heavy_check_mark: | :x:                |
| Pacific W4/W5/W6            | :heavy_check_mark: | :x:                |
| Pacific V-GTX/V-RTX         | :heavy_check_mark: | :x:                |
| Pacific Lumi/Rad/R1/Fitting | :heavy_check_mark: | :x:                |
| Riing Trio 12/14            | :x:                | :x:                |
| Toughpower iRGB PLUS        | :x:                | :heavy_check_mark: |
| TT Premium X1               | :x:                | :heavy_check_mark: |
| Nemesis/Iris                | :x:                | :heavy_check_mark: |

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

  "List of computer state profiles"
  "ComputerStateProfiles": ["<ComputerStateProfile>"],

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

### ComputerStateProfile
```json
{
  "State type. One of:"
  "[Boot, Shutdown, Suspend]" 
  "StateType": "<string>",

  "List of port identifiers that this profile controls"
  "Ports": ["<PortIdentifier>"],

  "Speed from 0% to 100%"
  "Speed": "<int>",

  "Rgb effect type. One of:"
  "[Flow, Spectrum, Ripple, Blink, Pulse, Wave, ByLed, Full]"
  "EffectType": "<string>",

  "Rgb effect speed. One of:"
  "[Slow, Normal, Fast, Extreme]"
  "EffectSpeed": "Normal",

  "List of LedColor that the effect should use."
  "EffectColors": ["<LedColor>"]
}
```

##### Examples:
```json
{
  "StateType": "Boot",
  "Ports": [
    [9802, 8101, 3],
    [9802, 8101, 2],
    [9802, 8101, 1]
  ],
  "Speed": 35,
  "EffectType": "Full",
  "EffectSpeed": "Normal",
  "EffectColors": [
    [0, 255, 0]
  ]
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