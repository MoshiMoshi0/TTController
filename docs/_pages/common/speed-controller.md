---
title: Speed Controller
permalink: /common/speed-controller
---

## Format

~~~
{
  "Type": <string>,
  "Config": <SpeedControllerConfig>
}
~~~

## Variables

### Type
<div class="variable-block" markdown="block">

Name of the speed controller

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Type": "PwmSpeedController"
~~~

</div>

### Config
<div class="variable-block" markdown="block">

Config of the speed controller

**Note:** The config has to have a required [Trigger]({{ "/common/trigger" | relative_url }}) set to the `Trigger` variable
{: .notice--warning}

**Note:** See page of the speed controller you are configuring for detailed documentation
{: .notice--info}

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Config": {
    ...
    "Trigger": {...}
}
~~~

</div>

## Examples
~~~ json
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