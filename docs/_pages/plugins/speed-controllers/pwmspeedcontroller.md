---
title: Pwm Speed Controller
permalink: /plugins/speed-controllers/pwmspeedcontroller
---

Sets the speed based on a custom PWM curve.

## Format

~~~
{
    "Type": "PwmSpeedController",
    "Config": {
        "CurvePoints": [<CurvePoint>],
        "Sensors": [<SensorIdentifier>],
        "SensorMixFunction": <enum>,
        "MinimumChange": <int>,
        "MaximumChange": <int>,

        "Trigger": <Trigger>
    }
}
~~~

## Variables

### CurvePoints
<div class="variable-block" markdown="block">

List of pwm curve points in `[<value>, <speed>]` format.

**Note:** `<speed>` is a value from `0` to `100` in percent.
{: .notice--info}

**Note:** The `<value>` corresponds to the calculated value from the [Sensors](#sensors).
{: .notice--info}

**Important:** The `<speed>` can be set to `0` to stop the device but if a bad `<value>` is used, it can cause the device to turn on and off frequently and shorten its livespan.
{: .notice--warning}

**Note:** Any `<speed>` beetween `1` and `19` will be raised to `20` as this is the minimum operating speed.
{: .notice--info}

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"CurvePoints": [
    [30, 30],
    [45, 50],
    [55, 60],
    [65, 75],
    [75, 100]
]
~~~

</div>

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

Determines how to combine values from [Sensors](#sensors) if multiple sensors are configured.

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

### MinimumChange
<div class="variable-block" markdown="block">

Minimum allowed change of speed in percent.

**Note:** Use this to reduce device speed fluctuation.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
4
~~~
**Example:**
~~~
"MinimumChange": 2
~~~

</div>

### MaximumChange
<div class="variable-block" markdown="block">

Maximum allowed change of speed in percent.

**Note:** Use this to reduce device speed fluctuation.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
8
~~~
**Example:**
~~~
"MaximumChange": 5
~~~

</div>

{% include variables/trigger.md %}

## Example

~~~
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
~~~