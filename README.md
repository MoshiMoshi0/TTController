<p align="center"><img src="Assets\logo-512.png" alt="logo" width="192"/></p>

This is home to TTController, a windows service for controlling various Thermaltake RGB Plus devices. 

It allows you to fully customize them by using **speed controllers**, which control the speed, and  **effects**, which control the led color. Each can be enabled or disabled dynamically by using various **triggers**.

The source code also provides an easy way to write your own **speed controller**, **effect** or **trigger**.

---
<a href="https://github.com/MoshiMoshi0/TTController/releases/latest">
<img alt="undefined" src="https://img.shields.io/github/tag-date/MoshiMoshi0/ttcontroller.svg?colorB=blue&label=release&style=flat"></a>
<br/><br/>

# Quick start

### Install

* Unpack latest release to a desired directory
* Run **TTController.Service.exe**
* Select **Manage Service** from command line menu and install the service

> The service will start automatically and will create a default empty config. See **Configure** section on how to configure it.

### Configure

* If the service is running
  * Stop the service using either **Manage Service** menu, **services.msc** or **net stop** 
* Edit the **config.json** file located in the same directory as **TTController.Service.exe**
* Start the service using either **Manage Service** menu, **services.msc** or **net start**

> You can use **Show hardware info** option from the commandline menu to determine what controllers are detected and find out your port and temperature sensor identifiers to be used in **config.json**.

### Uninstall

* Run **TTController.Service.exe**
* Select **Manage Service** from command line menu and uninstall the service
* Remove the folder containing service files

### Update

* If the service is running
  * Stop the service using either **Manage Service** menu, **services.msc** or **net stop** 
* Remove all files but leave the **config.json** file
* Unpack newest release to the same directory
* Start the service using either **Manage Service** menu, **services.msc** or **net start** 

# Device support
| Device                      | Support            | Confirmed          | Notes
|-----------------------------|--------------------|--------------------|-------------------------------------------
| Riing Plus 12/14/20         | :heavy_check_mark: | :heavy_check_mark: |
| Riing Trio 12/14            | :heavy_check_mark: | :heavy_check_mark: |
| Pure Plus 12/14             | :heavy_check_mark: | :heavy_check_mark: |
| Floe Riing RGB 240/280/360  | :heavy_check_mark: | :heavy_check_mark: |
| Pacific PR22-D5 Plus        | :heavy_check_mark: | :heavy_check_mark: |
| Pacific W4                  | :heavy_check_mark: | :heavy_check_mark: |
| WaterRam RGB                | :heavy_check_mark: | :x:                |
| Pacific CL360/RL360         | :heavy_check_mark: | :x:                |
| Pacific V-GTX/V-RTX         | :heavy_check_mark: | :x:                |
| Pacific Lumi/Rad/R1/Fitting | :heavy_check_mark: | :x:                |
| Pacific W5/W6               | :heavy_minus_sign: | :x:                | temperature sensor unsupported
| Toughpower iRGB PLUS        | :x:                | :heavy_minus_sign: | not implemented

# Plugins
### Speed Controllers:
* [PwmSpeedController](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/SpeedControllers/TTController.Plugin.PwmSpeedController/PwmSpeedController.cs)
* [StaticSpeedController](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/SpeedControllers/TTController.Plugin.StaticSpeedController/StaticSpeedController.cs)
* [CopySpeedController](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/SpeedControllers/TTController.Plugin.CopySpeedController/CopySpeedController.cs)

### Effects:
* [BlinkEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.BlinkEffect/BlinkEffect.cs)
* [ByLedColorEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.ByLedColorEffect/ByLedColorEffect.cs)
* [FlowEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.FlowEffect/FlowEffect.cs)
* [FullColorEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.FullColorEffect/FullColorEffect.cs)
* [PulseEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.PulseEffect/PulseEffect.cs)
* [SpectrumEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.SpectrumEffect/SpectrumEffect.cs)
* [RippleEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.RippleEffect/RippleEffect.cs)
* [WaveEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.WaveEffect/WaveEffect.cs)
* [SnakeEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.SnakeEffect/SnakeEffect.cs)
* [TemperatureEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.TemperatureEffect/TemperatureEffect.cs)
* [SoundEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.SoundEffect/SoundEffect.cs)
* [RawEffect](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Effects/TTController.Plugin.RawEffect/RawEffect.cs)

### Triggers
* [AlwaysTrigger](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Triggers/TTController.Plugin.AlwaysTrigger/AlwaysTrigger.cs)
* [OneTimeTrigger](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Triggers/TTController.Plugin.OneTimeTrigger/OneTimeTrigger.cs)
* [ProcessTrigger](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Triggers/TTController.Plugin.ProcessTrigger/ProcessTrigger.cs)
* [PulseTrigger](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Triggers/TTController.Plugin.PulseTrigger/PulseTrigger.cs)
* [LogicTrigger](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Triggers/TTController.Plugin.LogicTrigger/LogicTrigger.cs)
* [TemperatureTrigger](https://github.com/MoshiMoshi0/TTController/blob/master/Plugins/Triggers/TTController.Plugin.TemperatureTrigger/TemperatureTrigger.cs)

# Config

> ## The documentation is still WIP, if you have problems with configuration please make a new [issue](https://github.com/devcompl/TTController/issues/new/choose).

> The config is by default located in `config.json` file. You can modify this in `TTController.Service.exe.Config` file by changing the value of `config-file` key. 

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
              "Type": "AlwaysTrigger"
            }
          }
        }
      ],
      "Effects": [
        {
          "Type": "RippleEffect",
          "Config": {
            "Length": 4,
            "Color": [255, 0, 0],
            "TickInterval": 2,
            "Trigger": {
              "Type": "OneTimeTrigger"
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
  "DeviceRgbTimerInterval": "<int>",

  "Miliseconds between log update when running in console mode"
  "LoggingTimerInterval": "<int>"
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

  "Unique GUID string (8-4-4-4-12 format)"
  "Can be generated here: https://www.guidgen.com/"
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
      "Type": "AlwaysTrigger"
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
  "Type": "RippleEffect",
  "Config": {
    "Length": 4,
    "Color": [255, 0, 0],
    "TickInterval": 2,
    "Trigger": {
      "Type": "OneTimeTrigger"
    }
  }
}
```

---

### Trigger
```json
{
  "The class name of the trigger"
  "Type": "<string>", 

  "Config json for this trigger, depends on trigger type"
  "Config": {}
}
```

##### Examples:
```json
{
  "Type": "AlwaysTrigger"
}
```
```json
{
  "Type": "ProcessTrigger",
  "Config": {
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
  "If not set the speed is not changed" 
  "Speed": "<int>",

  "Effect type, depends on the controller type"
  "If not set the rgb effect is not changed" 
  "EffectType": "<string>",

  "List of LedColor that the effect should use."
  "EffectColors": ["<LedColor>"]
}
```

> If you want to update or add a profile with **Boot** type, you first need to remove `<add key="boot-profile-saved" value="true" />` line or change the value to `false` in `TTController.Service.exe.Config` file and restart the service.

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
  "EffectByte": 25,
  "EffectColors": [
    [0, 255, 0]
  ]
}
```

---

### Port Config
```json
{
  "List of port identifiers that this config applies to"
  "Ports": ["<PortIdentifier>"],

  "Port config"
  "Config": {
    "Port name"
    "Name": "<string>",

    "Port led count"
    "LedCount": "<int>",

    "Determines how to handle led color count"
    "mismatch generated by effects"
    "One of: [DoNothing, Lerp, Trim, Copy]"
    "LedCountHandling": "<string>",

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

# Credits

Logo based on icon by Freepik from [flaticon](https://flaticon.com)