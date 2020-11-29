---
title: Pulse Effect
permalink: /plugins/effects/pulseeffect
---

## Format

~~~
{
    "Type": "PulseEffect",
    "Config": {
        "BrightnessStep": <float>
        "Color": <LedColorProvider>,

        "ColorGenerationMethod": <enum>,
        "Trigger": <Trigger>
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
{
    "Gradient": [
        [0, [255, 0, 0]],
        [0.5, [0, 255, 0]],
        [1.0 [255, 0, 0]]
    ]
}
~~~

</div>

{% include variables/colorgenerationmethod.md %}

{% include variables/trigger.md %}

## Example

~~~
~~~