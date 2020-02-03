---
title: Service Configuration
permalink: /config/service
---

**Note:** The config is by default located in `config.json` file. You can modify this in `TTController.Service.exe.Config` file by changing the value of `config-file` key.
{: .notice--info}

## Format

~~~
{
  "Profiles":  [<ProfileConfig>],
  "ComputerStateProfiles":  [<ComputerStateProfileConfig>],
  "PortConfigs":  [<PortConfig>],
  "SensorConfigs":   [<SensorConfig>],

  "SensorTimerInterval ": <int>,
  "DeviceSpeedTimerInterval": <int>,
  "DeviceRgbTimerInterval": <int>,
  "LoggingTimerInterval": <int>
}
~~~

## Variables

### Profiles
<div class="variable-block" markdown="block">

List of [Profile Config]({{ "/config/profile" | relative_url }}) objects

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Profiles": [
    {...},
    {...}
]
~~~

</div>

### ComputerStateProfiles
<div class="variable-block" markdown="block">

List of [ComputerStateProfile Config]({{ "/config/computerstateprofile" | relative_url }}) objects

**Required:** No<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"ComputerStateProfiles": [
    {...},
    {...}
]
~~~

</div>

### PortConfigs 
<div class="variable-block" markdown="block">

List of [Port Config]({{ "/config/port" | relative_url }}) objects<br>

**Note:** If [Port Config]({{ "/config/port" | relative_url }}) is not configured for a port, a default [Port Config]({{ "/config/port" | relative_url }}) will be used
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"PortConfigs": [
    {...},
    {...}
]
~~~

</div>

### SensorConfigs
<div class="variable-block" markdown="block">

List of [Sensor Config]({{ "/config/sensor" | relative_url }}) objects

**Note:** If [Sensor Config]({{ "/config/sensor" | relative_url }}) is not configured for a sensor, a default [Sensor Config]({{ "/config/sensor" | relative_url }}) will be used
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"SensorConfigs": [
    {...},
    {...}
]
~~~

</div>

### SensorTimerInterval 
<div class="variable-block" markdown="block">

Determines timer delay for updating sensor values

**Note:** Value in miliseconds
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
250
~~~
**Example:**
~~~
"SensorTimerInterval": 500
~~~

</div>

### DeviceSpeedTimerInterval
<div class="variable-block" markdown="block">

Determines timer delay for updating speed of devices

**Note:** Value in miliseconds
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
2500
~~~
**Example:**
~~~
"DeviceSpeedTimerInterval": 3000
~~~

</div>

### DeviceRgbTimerInterval
<div class="variable-block" markdown="block">

Determines timer delay for updating led colors

**Note:** Value in miliseconds
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
32
~~~
**Example:**
~~~
"DeviceRgbTimerInterval": 16
~~~

</div>

### LoggingTimerInterval
<div class="variable-block" markdown="block">

Determines timer delay for logging port and sensor data

**Note:** Used only in **console mode**
{: .notice--info}

**Note:** Value in miliseconds
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
5000
~~~
**Example:**
~~~
"LoggingTimerInterval": 1000
~~~

</div>

## Example

~~~ json
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
~~~