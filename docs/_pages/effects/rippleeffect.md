---
title: Ripple Effect
permalink: /effects/rippleeffect
---

## Format

~~~
{
    "Type": "RippleEffect",
    "Config": {
        "Color": <LedColor>,
        "Length": <int>,
        "TickInterval": <int>
    }
}
~~~

## Variables

### Color
<div class="variable-block" markdown="block">

Ripple base [Led Color]({{ "/common/led-color" | relative_url }}).

**Required:** **Yes**<br>
**Default value:**
~~~
[0, 0, 0]
~~~
**Example:**
~~~
"Color": [255, 0, 0]
~~~

</div>

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

## Example

~~~
~~~