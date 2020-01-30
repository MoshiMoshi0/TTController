<p align="center"><img src="Assets\logo-512.png" alt="logo" width="256"/><br/><h1 align="center"><b>TTController</b></h1></p>


<a href="https://github.com/MoshiMoshi0/TTController/releases/latest"><img alt="download" src="https://img.shields.io/github/tag-date/MoshiMoshi0/ttcontroller.svg?color=green&label=download&style=flat"></a>
<img alt="downloads" src="https://img.shields.io/github/downloads/MoshiMoshi0/TTController/total.svg?color=yellow">
<a href="https://paypal.me/MoshiMoshi0"><img alt="donate" src="https://img.shields.io/badge/donate-paypal-blue.svg?style=flat"></a>
<a href="https://ci.appveyor.com/project/MoshiMoshi0/ttcontroller"><img alt="ci" src="https://ci.appveyor.com/api/projects/status/shinpu4cd2sjrs0c?svg=true"></a>
<a href="https://ci.appveyor.com/project/MoshiMoshi0/ttcontroller/build/artifacts"><img alt="download-develop" src="https://img.shields.io/badge/download-develop-red.svg?logo=appveyor&logoColor=ccc"></a>
<br/><br/>


This is home to TTController, a windows service for controlling various Thermaltake RGB Plus devices. 
It works as an alternative to the official TT RGB Plus software.

It allows you to fully customize your Thermaltake devices by using [_speed controllers_](https://github.com/MoshiMoshi0/TTController#speed-controllers), which control the speed, and  [_effects_](https://github.com/MoshiMoshi0/TTController#effects), which control the led colors. Each can be enabled or disabled dynamically by using [_triggers_](https://github.com/MoshiMoshi0/TTController#triggers).


<br><br>

# Quick start

<details><summary>How to install?</summary>

* Unpack latest release to a desired directory
* Run **TTController.Service.exe**
* Select **Manage Service** from command line menu and install the service

> The service is by default installed to run as **LocalSystem** account, to change this edit `service-install-as` in `TTController.Service.exe.Config` file.

> The service will start automatically and will create a default empty config. See **Configure** section on how to configure it.

> A quick test to see if the service is working is to use `Main Menu -> Debug -> Report` menu, it should list all detected controllers, sensors and plugins. 
</details>

<details><summary>How to configure?</summary>

* If the service is running
  * Stop the service using either **Manage Service** menu, **services.msc** or **net stop** 
* Edit the **config.json** file located in the same directory as **TTController.Service.exe**
* Start the service using either **Manage Service** menu, **services.msc** or **net start**

> You can use `Main Menu -> Debug -> Controllers` menu to find your port identifiers, and `Main Menu -> Debug -> Sensors` menu to find your sensor identifiers to be used in **config.json**.
</details>

<details><summary>How to uninstall?</summary>

* Run **TTController.Service.exe**
* Select **Manage Service** from command line menu and uninstall the service
* Remove the folder containing service files
</details>

<details><summary>How to update?</summary>

* If the service is running
  * Stop the service using either **Manage Service** menu, **services.msc** or **net stop** 
* Remove all files but leave the **config.json** file
* Unpack newest release to the same directory
* Start the service using either **Manage Service** menu, **services.msc** or **net start** 
</details>

# Support matrix

### Controllers

| Controller      | Support            | Notes
|-----------------|--------------------|---------------------------------------
| Riing           | :heavy_check_mark: |
| Riing Plus      | :heavy_check_mark: |
| Riing Trio      | :heavy_check_mark: |
| Dpsg            | :heavy_minus_sign: | not fully implemented
| Riing Quad      | :x:                |

### Devices

| Device                      | Support            | Confirmed          | Notes
|-----------------------------|--------------------|--------------------|-------------------------------------------
| Riing Plus 12/14/20         | :heavy_check_mark: | :heavy_check_mark: |
| Riing Trio 12/14            | :heavy_check_mark: | :heavy_check_mark: |
| Pure Plus 12/14             | :heavy_check_mark: | :heavy_check_mark: |
| Floe Riing RGB 240/280/360  | :heavy_check_mark: | :heavy_check_mark: |
| Pacific PR22-D5 Plus        | :heavy_check_mark: | :heavy_check_mark: |
| Pacific W4                  | :heavy_check_mark: | :heavy_check_mark: |
| Razer Connect               | :heavy_check_mark: | :heavy_check_mark: |
| Riing Duo 12/14             | :heavy_check_mark: | :heavy_check_mark: |
| WaterRam RGB                | :heavy_check_mark: | :x:                |
| Pacific CL360/RL360         | :heavy_check_mark: | :x:                |
| Pacific V-GTX/V-RTX         | :heavy_check_mark: | :x:                |
| Pacific Lumi/Rad/R1/Fitting | :heavy_check_mark: | :x:                |
| Toughpower iRGB PLUS        | :heavy_minus_sign: | :heavy_minus_sign: | controller not fully implemented
| Pacific W5/W6               | :heavy_minus_sign: | :x:                | temperature sensor unsupported
| Riing Quad 12/14            | :x:                | :x:                |

---

# Config

> ## This documentation is still WIP, if you have problems with configuration please make a new [issue](https://github.com/devcompl/TTController/issues/new/choose).

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
          "Type": "PwmSpeedController", 
          "Config": {
            "CurvePoints": [
              [30, 30],
              [45, 50],
              [55, 60],
              [65, 75],
              [75, 100]
            ],
            "Sensors": ["/intelcpu/0/temperature/8"],
            "Trigger": {
              "Type": "AlwaysTrigger"
            }
          }
        }
      ],
      "Effects": [
        {
          "Type": "SensorEffect",
          "Config": {
            "Sensors": ["/intelcpu/0/temperature/8"],
            "ColorGradient": [
              [40, [16, 16, 128]],
              [60, [16, 16, 16]],
              [86, [128, 16, 16]]
            ],
            "Trigger": {
              "Type": "AlwaysTrigger"
            }
          }
        }
      ]
    }
  ],
  "ComputerStateProfiles": [
    {
      "StateType": "Shutdown",
      "Ports": [
        [9802, 8101, 1]
      ],
      "Speed": 35,
      "EffectType": "Full",
      "EffectColors": [
        [255, 0, 0]
      ]
    }
  ],
  "PortConfigs": [
    {
      "Ports": [[9802, 8101, 1]],
      "Config": {
        "Name": "Top Fan",
        "LedRotation": [11],
        "LedReverse": [false]
      }
    }
  ],
  "SensorConfigs": [
    {
      "Sensors": ["/intelcpu/0/temperature/8"],
      "Config": {
        "CriticalValue": 90
      }
    }
  ]
}
```

## Plugins
### Speed Controllers:

<details><summary>PwmSpeedController</summary>

Sets the speed based on a custom PWM curve

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| CurvePoints | List of PWM curve points with format: <pre>[\<value\>, \<speed\>]</pre> | <pre>{}</pre> | <pre>"CurvePoints": {<br>    [30, 50],<br>    [40, 60],<br>    [45, 75],<br>    [55, 100]<br>}</pre>
| Sensors | List of sensor identifiers to get the value from | <pre>[]</pre> | <pre>"Sensors": \[<br>    "/intelcpu/0/temperature/0",<br>    "/gpu/0/temperature/0"<br>\]</pre>
| SensorMixFunction | Determines how to combine values from **Sensors** if multiple sensors are configured<br>Allowed values: <pre>"Maximum", "Minimum", "Average"</pre> | <pre>"Maximum"</pre> | <pre>"SensorMixFunction": "Average"</pre>
| MinimumChange | Minimum allowed change of speed<br>This reduces speed fluctuation | <pre>4</pre> | <pre>"MinimumChange": 2</pre>
| MaximumChange | Maximum allowed change of speed<br>This reduces speed fluctuation | <pre>8</pre> | <pre>"MaximumChange": 10</pre>
</details>
<details><summary>StaticSpeedController</summary>

Sets the speed to a constant value

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Speed | Constant speed value | <pre>50</pre> | <pre>"Speed": 35</pre>
</details>
<details><summary>CopySpeedController</summary>

Copies the speed from another port

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Target | Target port identifier | | <pre>"Target": [9802, 8101, 1]</pre>
</details>
<details><summary>DpsgSpeedController</summary>

Sets the speed of DPSG psu to one of the internal profiles. Not usable with other fans!

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| FanMode | Internal fan profile name<br>Allowed values: <pre>"Off", "Silent", "Performance"</pre> | <pre>"Silent"</pre> | <pre>"FanMode": "Performance"</pre>
</details>

### Effects:

<details><summary>BlinkEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| On | Effect on time in ms | <pre>1000</pre> | <pre>"On": 250</pre>
| Off | Effect off time in ms | <pre>1000</pre> | <pre>"Off": 250</pre>
| Colors | List of colors when the effect is on | <pre>[]</pre> | <pre>"Colors": [<br>    [255, 0, 0],<br>    [255, 0, 0],<br>    [255, 0, 0],<br>    ...<br>]</pre>
</details>
<details><summary>PerLedColorEffect</summary>

Static per led colors

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Colors | List of colors | <pre>[]</pre> | <pre>"Colors": [<br>    [255, 0, 0],<br>    [0, 255, 0],<br>    [0, 0, 255],<br>    ...<br>]</pre>
</details>
<details><summary>FlowEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| FillStep | Determines how fast the colors fills the device | <pre>0.05</pre> | <pre>"FillStep": 0.12</pre>
| HueStep | Determines color hue increment when the colors fills the device completly | <pre>30</pre> | <pre> "HueStep": 60</pre>
| Saturation | Saturation of colors | <pre>1.0</pre> | <pre>"Saturation": 0.5</pre>
| Brightness | Brightness of colors | <pre>1.0</pre> | <pre>"Brightness": 0.75</pre> 
</details>
<details><summary>FullColorEffect</summary>

Static color on all leds

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Color | | <pre>[0, 0, 0]</pre> | <pre>"Color": [255, 0, 0]</pre>
</details>
<details><summary>PulseEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Colors | List of colors | <pre>[]</pre> | <pre>"Colors": [<br>    [255, 0, 0],<br>    [0, 255, 0],<br>    [0, 0, 255],<br>    ...<br>]</pre>
| BrightnessStep | Determines how fast the colors go to and from black | <pre>0.025</pre> | <pre>"BrightnessStep": 0.033</pre>
</details>
<details><summary>SpectrumEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Saturation | Saturation of colors | <pre>1.0</pre> | <pre>"Saturation": 0.5</pre>
| Brightness | Brightness of colors | <pre>1.0</pre> | <pre>"Brightness": 0.75</pre> 
| HueStep | Color hue increment | <pre>1.0</pre> | <pre> "HueStep": 2.0</pre>
</details>
<details><summary>RippleEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Color | Ripple color | <pre>[0, 0, 0]</pre> | <pre>"Color": [255, 0, 0]</pre>
| Length | Ripple length | <pre>5</pre> | <pre>"Length": 3</pre>
| TickInterval | Determines how many updates before the ripple advances to the next led | <pre>3</pre> | <pre>"TickInterval": 2</pre>
</details>
<details><summary>WaveEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Colors | List of colors | <pre>[]</pre> | <pre>"Colors": [<br>    [255, 0, 0],<br>    [0, 255, 0],<br>    [0, 0, 255],<br>    ...<br>]</pre>
| TickInterval | Determines how many updates before the wave advances | <pre>3</pre> | <pre>"TickInterval": 2</pre>
</details>
<details><summary>SensorEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Sensors | List of sensor identifiers to get the value from | <pre>[]</pre> | <pre>"Sensors": \[<br>    "/intelcpu/0/temperature/0",<br>    "/gpu/0/temperature/0"<br>\]</pre>
| SensorMixFunction | Determines how to combine values from **Sensors** if multiple sensors are configured<br>Allowed values: <pre>"Maximum", "Minimum", "Average"</pre> | <pre>"Maximum"</pre> | <pre>"SensorMixFunction": "Average"</pre>
| ColorGradient | List of color gradient entries<br>Entry format: <pre>[\<value\>, \<color\>]</pre> | | <pre>"ColorGradient": [<br>    [30, [0, 255, 0]],<br>    [60, [255, 0, 0]]<br>]</pre>
</details>
<details><summary>SoundEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| UseAverage | Determines if fft values are averaged | <pre>true</pre> | <pre>"UseAverage": false</pre>
| MinimumFrequency | Mimimum frequency for fft | <pre>100</pre> | <pre>"MinimumFrequency": 1000</pre>
| MaximumFrequency | Maximum frequency for fft | <pre>10000</pre> | <pre>"MaximumFrequency": 12000</pre>
| ScalingStrategy | Fft value scaling function<br>Allowed values: <pre>"Decibel", "Linear", "Sqrt"</pre> | <pre>"Sqrt"</pre> | <pre>"ScalingStrategy": "Decibel"</pre>
| ScalingFactor | Fft scaling scaling factor<br>Used for "Linear" and "Sqrt" **ScalingStrategy** | <pre>2.0</pre> | <pre>"ScalingFactor": 4.0</pre>
| ColorGradient | List of color gradient entries<br>Entry format: <pre>[\<fft value\>, \<color\>]</pre> | | <pre>"ColorGradient": [<br>    [0, [0, 255, 0]],<br>    [1.0, [255, 0, 0]]<br>]</pre>
</details>
<details><summary>RawEffect</summary>

Sets controller built-in effects

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| EffectType | Effect type name | | <pre>"EffectType": "Full"</pre>
| Colors | List of colors | <pre>[]</pre> | <pre>"Colors": [<br>    [255, 0, 0],<br>    [0, 255, 0],<br>    [0, 0, 255],<br>    ...<br>]</pre>
</details>
<details><summary>RazerConnectEffect</summary>

Receives 5 colors from razer chroma
</details>
<details><summary>PingPongEffect</summary>

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Step | Determines how fast the region moves | <pre>0.01</pre> | <pre>"Step": 0.03</pre>
| Height | Height of the region | <pre>0.2</pre> | <pre>"Height": 0.2</pre>
| Width | Width of the region | <pre>0.5</pre> | <pre>"Width": 0.33</pre>
</details>

### Triggers

<details><summary>AlwaysTrigger</summary>

Always triggers
</details>
<details><summary>OneTimeTrigger</summary>

Triggers only one time

Usefull for internal speed profiles or effects
</details>
<details><summary>ProcessTrigger</summary>

Triggers when any of the specified processes is running

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Processes | List of process names | <pre>[]</pre> | <pre>"Processes": ["cmd", "notepad"]</pre>
</details>
<details><summary>PulseTrigger</summary>

Pulses the trigger on and off

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| On | Trigger on time in ms | <pre>1000</pre> | <pre>"On": 250</pre>
| Off | Trigger off time in ms | <pre>1000</pre> | <pre>"Off": 250</pre>
</details>
<details><summary>LogicTrigger</summary>

Combines multiple triggers

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Operation | Determines how to combine the triggers<br>Allowed values: <pre>"And", "Or"</pre> | <pre>"And"</pre> | <pre>"Operation": "Or"</pre>
| Negate | Determines if the combined value is negated | <pre>false</pre> | <pre>"Negate": true</pre>
| Triggers | List of triggers | <pre>[]</pre> | <pre>"Triggers": [<br>    {...},<br>    {...}<br>]</pre>
</details>
<details><summary>SensorTrigger</summary>

Triggers based on sensor value

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Sensors | List of sensor identifiers to get the value from | <pre>[]</pre> | <pre>"Sensors": \[<br>    "/intelcpu/0/temperature/0",<br>    "/gpu/0/temperature/0"<br>\]</pre>
| SensorMixFunction | Determines how to combine values from **Sensors** if multiple sensors are configured<br>Allowed values: <pre>"Maximum", "Minimum", "Average"</pre> | <pre>"Maximum"</pre> | <pre>"SensorMixFunction": "Average"</pre>
| Value | Trigger value | | <pre>"Value": 50</pre>
| ComparsionType | Determines how to compare current sensor value and the trigger value<br>Allowed values: <pre>"Equals", "Greater", "Less"</pre> | <pre>"Greater"</pre> | <pre>"ComparsionType": "Less"</pre>  
</details>
<details><summary>ScheduleTrigger</summary>

Triggers based on configured schedule

#### Configuration

| Variable | Description | Default value | Example
|----------|-------------|---------------|---------
| Scope | Determines the repeat period of the schedule<br>Allowed values:<br><pre>"Minute", "Hour", "Day", "Week"</pre> | <pre>"Day"</pre> | <pre>"Scope": "Day"</pre>
| Value | What value to return when current time matches the schedule | <pre>true</pre> | <pre>"Value": true</pre>
| UpdateInterval | Determines how often to check the current time<br>If no value is set (default), **UpdateInterval** is set automatically based on **Scope** | | <pre>"UpdateInterval": "00:05:00"</pre>
| Schedule | List of schedule entries<br>Entry format: <pre>[\<start time\>, \<end time\>]</pre>Allowed time formats: <pre>"d.hh:mm", "hh:mm", "ss"</pre> | <pre>[]</pre> | <pre>"Schedule": [<br>    ["00:00", "08:00"],<br>    ["20:00", "23:59"]<br>]</pre>
</details>

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
  "PortConfigs": ["<PortConfig>"],

  "List of sensor configs"
  "The values in this list are optional, if SensorConfig for a sesnor is not present"
  "the default values will be used"
  "SensorConfigs": ["<SensorConfig"],

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

  "Config json for this speed controller"
  "Config": {
    "Required"
    "Trigger": {"..."},

    "Other properties depending on speed controller type"
    "..."
  }
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

  "Config json for this effect"
  "Config": {
    "Required"
    "Trigger": {"..."},

    "Optional, defaults to 'PerPort'"
    "One of: [SpanPorts, PerPort]" 
    "ColorGenerationMethod": "<string>",

    "Other properties depending on effect type"
    "..."
  }
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
  "See \"Main Menu -> Debug -> Controllers\" for avaible effect types for each controller"
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
  "EffectType": "Full",
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
    "Port name, unused"
    "Name": "<string>",

    "Port led count"
    "LedCount": "<int>",

    "Device type"
    "One of: [Default, RiingTrio, RiingDuo, FloeRiing, PurePlus]"
    "DeviceType": "<string>",

    "Determines how to handle led color count"
    "mismatch generated by effects"
    "One of: [DoNothing, Lerp, Nearest, Wrap, Trim, Copy]"
    
    "DoNothing: do nothing"
    "Lerp: stretches or shrinks effect colors list to match the device led count using a gradient"
    "Nearest: stretches or shrinks effect colors list to match the device led count by copying/removing the nearest color"
    "Wrap: if effect colors list is bigger than the device led count, wrap the remainder to the beginning, otherwise do nothing"
    "Trim: if effect colors list is bigger than the device led count, trim the excess, otherwise do nothing"
    "Copy: if effect colors list is smaller than the device led count, copy the colors untill they are equal, otherwise do nothing"
    "LedCountHandling": "<string>",

    "Array of led rotations per device zone"
    "LedRotation": ["<int>"],

    "Array of reverse flags per device zone"
    "If true led indexes are reversed on that zone"
    "LedReverse": ["<bool>"]
  }
}
```

##### Examples:
```json
{
  "Ports": [[9802, 8101, 1]],
  "Config": {
    "Name": "Top Left",
    "LedCount": 12,
    "LedCountHandling": "Lerp",
    "LedRotation": 10,
    "LedReverse": true
  }
}
```

---


### Sensor Config
```json
{
  "List of sensor identifiers that this config applies to"
  "Sensors": ["<string>"],

  "Sensor config"
  "Config": {
    "If the value of the sensor exceeds this value"
    "all fans will be set to 100% speed"
    "CriticalValue": "<float>",

    "Sensor value offset"
    "Offset": "<float>"
  }
}
```

##### Examples:
```json
{
  "Sensors": ["/intelcpu/0/temperature/8"],
  "Config": {
    "CriticalValue": 90
  }
}
```

# Credits

Logo based on icon by Freepik from [flaticon](https://flaticon.com)