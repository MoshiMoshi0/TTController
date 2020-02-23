---
title: Service Configuration
permalink: /config/service
---

**Note:** The config is located in `config.json` file.
{: .notice--info}

## Format

~~~
{
  "Profiles":  [<ProfileConfig>],
  "ComputerStateProfiles":  [<ComputerStateProfileConfig>],
  "PortConfigs":  [<PortConfig>],
  "SensorConfigs":   [<SensorConfig>],

  "CpuSensorsEnabled": <bool>,
  "GpuSensorsEnabled": <bool>,
  "StorageSensorsEnabled": <bool>,
  "MotherboardSensorsEnabled": <bool>,
  "MemorySensorsEnabled": <bool>,
  "NetworkSensorsEnabled": <bool>,
  "ControllerSensorsEnabled": <bool>,

  "SensorTimerInterval ": <int>,
  "DeviceSpeedTimerInterval": <int>,
  "DeviceRgbTimerInterval": <int>,
  "LoggingTimerInterval": <int>
}
~~~

## Variables

### Profiles
<div class="variable-block" markdown="block">

List of [Profile Configs]({{ "/config/profile" | relative_url }}).

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

List of [ComputerStateProfile Configs]({{ "/config/computerstateprofile" | relative_url }}).

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

List of [Port Configs]({{ "/config/port" | relative_url }}).

**Note:** If [Port Config]({{ "/config/port" | relative_url }}) is not configured for a port, a default [Port Config]({{ "/config/port" | relative_url }}) will be used.
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

List of [Sensor Configs]({{ "/config/sensor" | relative_url }}).

**Note:** If [Sensor Config]({{ "/config/sensor" | relative_url }}) is not configured for a sensor, a default [Sensor Config]({{ "/config/sensor" | relative_url }}) will be used.
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

### CpuSensorsEnabled
<div class="variable-block" markdown="block">

Enables support for CPU sensors.

**Required:** No<br>
**Default value:**
~~~
true
~~~
**Example:**
~~~
"CpuSensorsEnabled": false
~~~

</div>

### GpuSensorsEnabled
<div class="variable-block" markdown="block">

Enables support for GPU sensors.

**Required:** No<br>
**Default value:**
~~~
true
~~~
**Example:**
~~~
"GpuSensorsEnabled": false
~~~

</div>

### StorageSensorsEnabled
<div class="variable-block" markdown="block">

Enables support for HDD/SDD sensors.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"StorageSensorsEnabled": false
~~~

</div>

### MotherboardSensorsEnabled
<div class="variable-block" markdown="block">

Enables support for Motherboard sensors.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"MotherboardSensorsEnabled": false
~~~

</div>

### MemorySensorsEnabled
<div class="variable-block" markdown="block">

Enables support for RAM sensors.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"MemorySensorsEnabled": false
~~~

</div>

### NetworkSensorsEnabled
<div class="variable-block" markdown="block">

Enables support for NIC sensors.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"MemorySensorsEnabled": false
~~~

</div>

### ControllerSensorsEnabled
<div class="variable-block" markdown="block">

Enables support for hid fan controller/pump sensors.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"ControllerSensorsEnabled": false
~~~

</div>

### SensorTimerInterval 
<div class="variable-block" markdown="block">

Determines timer delay for updating sensor values.

**Note:** Value in miliseconds.
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

Determines timer delay for updating speed of devices.

**Note:** Value in miliseconds.
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

Determines timer delay for updating led colors.

**Note:** Value in miliseconds.
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

Determines timer delay for logging port and sensor data.

**Note:** Used only in **console mode**.
{: .notice--info}

**Note:** Value in miliseconds.
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