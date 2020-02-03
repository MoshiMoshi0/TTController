---
title: Effect
permalink: /common/effect
---

## Format

~~~
{
  "Type": <string>,
  "Config": <EffectConfig>,
}
~~~

## Variables

### Type
<div class="variable-block" markdown="block">

Name of the effect

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Type": "SensorEffect"
~~~

</div>

### Config
<div class="variable-block" markdown="block">

Config of the effect

**Note:** The config has to have [Trigger]({{ "/common/trigger" | relative_url }}) object set to the required `Trigger` variable
{: .notice--warning}

**Note:** See page of the effect you are configuring for detailed documentation
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
~~~