---
title: Ripple Effect
permalink: /effects/rippleeffect
---

## Format

~~~
{
    "Type": "RippleEffect",
    "Config": {
        "Length": <int>,
        "TickInterval": <int>,
        "RippleColor": <LedColorProvider>,
        "BackgroundColor": <LedColorProvider>
    }
}
~~~

## Variables

### Length
<div class="variable-block" markdown="block">

Ripple length in number of leds.

**Required:** No<br>
**Default value:**
~~~
5
~~~
**Example:**
~~~
"Length": 3
~~~

</div>

### TickInterval
<div class="variable-block" markdown="block">

Number of updates before the ripple advances to the next led.

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

### RippleColor
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object for the ripple.

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

### BackgroundColor
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object for the background.

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