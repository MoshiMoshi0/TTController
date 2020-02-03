---
title: Profile Configuration
permalink: /config/profile
---

## Format

~~~
{
  "Name": <string>,
  "Guid": <guid>,
  "Ports": [<PortIdentifier>],

  "SpeedControllers": [<SpeedController>],
  "Effect": [<Effect>]
}
~~~

## Variables

### Name
<div class="variable-block" markdown="block">

Name of the profile

**Note:** The name has to be unique
{: .notice--warning}

**Required:** **Yes**<br>
**Default value:**
~~~
"Default"
~~~
**Example:**
~~~
"Name": "Top Fans"
~~~

</div>

### Ports
<div class="variable-block" markdown="block">

List of [Port Identifier]({{ "/common/port-identifier" | relative_url }}) objects modified by this config

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Ports": [
    [9802, 9101, 1],
    [9802, 9101, 3]
]
~~~

</div>

### SpeedControllers
<div class="variable-block" markdown="block">

List of [Speed Controller]({{ "/common/speed-controller" | relative_url }}) objects

**Required:** No<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"SpeedControllers": [
    {...},
    {...}
]
~~~

</div>

### Effects
<div class="variable-block" markdown="block">

List of [Effect]({{ "/common/effect" | relative_url }}) objects

**Required:** No<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Effects": [
    {...},
    {...}
]
~~~

</div>

## Examples
~~~ json
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
~~~