---
title: Wave Effect
permalink: /plugins/effects/waveeffect
---

## Format

~~~
{
    "Type": "WaveEffect",
    "Config": {
        "TickInterval": <int>,
        "Color": <LedColorProvider>,
    }
}
~~~

## Variables

### TickInterval
<div class="variable-block" markdown="block">

Number of updates before the wave advances to the next led.

**Required:** No<br>
**Default value:**
~~~
3
~~~
**Example:**
~~~
"TickInterval": 2
~~~

</div>

### Color
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object.

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
{
    "Gradient": [
        [0, [255, 0, 0]],
        [0.5, [0, 255, 0]],
        [1.0 [255, 0, 0]]
    ]
}
~~~

</div>

## Example

~~~
~~~