---
title: Sensor Configuration
permalink: /config/sensor
---

## Format

~~~
{
  "Sensors": [<SensorIdentifier>],
  "Config": {
    "Offset": <float?>,
    "CriticalValue": <float?>
  }
}
~~~

## Variables

### Ports
<div class="variable-block" markdown="block">

List of [Sensor Identifier]({{ "/sensor-identifier" | relative_url }}) objects modified by this config

**Required:** **Yes**<br>
**Default value:**<br>
~~~
[]
~~~
**Example:**<br>
~~~
"Sensors": ["/intelcpu/0/temperature/8"]
~~~

</div>

### Offset
<div class="variable-block" markdown="block">

Sensor value offset

**Required:** No<br>
**Default value:**<br>
~~~
~~~
**Example:**<br>
~~~
"Offset": 5
~~~

</div>

### CriticalValue
<div class="variable-block" markdown="block">

If any sensor exceeds this value, all devices will be set to 100% speed

**Required:** No<br>
**Default value:**<br>
~~~
~~~
**Example:**<br>
~~~
"CriticalValue": 90
~~~

</div>

## Examples
~~~ json
{
  "Sensors": [
    "/intelcpu/0/temperature/8",
    "/intelcpu/0/temperature/0"
  ],
  "Config": {
    "Offset": "-3",
    "CriticalValue": 90
  }
}
~~~