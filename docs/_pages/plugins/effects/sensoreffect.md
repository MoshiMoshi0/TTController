---
title: Sensor Effect
permalink: /plugins/effects/sensoreffect
---

## Format

~~~
{
    "Type": "SensorEffect",
    "Config": {
        "Sensors": [<SensorIdentifier>],
        "SensorMixFunction": <enum>,
        "SmoothingFactor": <float>,
        "ColorGradient": <LedColorGradient>
    }
}
~~~

## Variables

### Sensors
<div class="variable-block" markdown="block">

List of [Sensor Identifiers]({{ "/common/sensor-identifier" | relative_url }}) to read values from.

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Sensors": ["/intelcpu/0/temperature/8"]
~~~

</div>

### SensorMixFunction
<div class="variable-block" markdown="block">

Determines how to combine values from [Sensors](#sensors).

**Allowed values:** `"Maximum"`, `"Minimum"`, `"Average"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Maximum"
~~~
**Example:**
~~~
"SensorMixFunction": "Average"
~~~

</div>

### SmoothingFactor
<div class="variable-block" markdown="block">

Determines how fast should the current color blend to the target [ColorGradient](#colorgradient) color.

**Required:** No<br>
**Default value:**
~~~
0.05
~~~
**Example:**
~~~
"SmoothingFactor": 0.35
~~~

</div>

### ColorGradient
<div class="variable-block" markdown="block">

A [Led Color Gradient]({{ "/common/led-color-gradient" | relative_url }}) to translate [Sensors](#sensors) values to led colors.

**Note:** The gradient `<location>` values are calculated [Sensors](#sensors) values.
{: .notice--info}

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"ColorGradient": [
    [30, [16, 0, 0]],
    [50, [64, 0, 0]],
    [70, [256, 0, 0]],
    ...
]
~~~

</div>


## Example

~~~
~~~