---
title: Pulse Effect
permalink: /effects/pulseeffect
---

## Format

~~~
{
    "Type": "PulseEffect",
    "Config": {
        "BrightnessStep": <float>
        "Color": <LedColorProvider>
    }
}
~~~

## Variables

### BrightnessStep
<div class="variable-block" markdown="block">

Determines how fast the colors pulse.

**Required:** No<br>
**Default value:**
~~~
0.025
~~~
**Example:**
~~~
"BrightnessStep": 0.33
~~~

</div>

### Color
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object with colors when the pulse is in full brightness.

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Color": {
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