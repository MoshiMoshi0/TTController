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
        "Colors": <[LedColor]>
        "Color": <LedColor>
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

### Colors
<div class="variable-block" markdown="block">

List of [Led Colors]({{ "/common/led-color" | relative_url }}) when the pulse is in full brightness.

**Required:** Either [Color](#color) or [Colors](#colors) is required.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Colors": [
    [255, 0, 0],
    [0, 255, 0],
    [0, 0, 255],
    ...
]
~~~

</div>

### Color
<div class="variable-block" markdown="block">

[Led Color]({{ "/common/led-color" | relative_url }}) to set on all leds when the pulse is in full brighness.

**Required:** Either [Color](#color) or [Colors](#colors) is required.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Color": [255, 0, 0]
~~~

</div>

## Example

~~~
~~~