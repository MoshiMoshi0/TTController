---
title: Blink Effect
permalink: /effects/blinkeffect
---

## Format

~~~
{
    "Type": "BlinkEffect",
    "Config": {
        "OnTime": <int>,
        "OffTime": <int>,
        "OnColor": <LedColor>,
        "OffColor": <LedColor>,
        "OnColors": [<LedColor>],
        "OffColors": [<LedColor>],
    }
}
~~~

## Variables

### OnTime
<div class="variable-block" markdown="block">

Effect "on" state time.

**Note:** Value in miliseconds.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"OnTime": 500
~~~

</div>

### OffTime
<div class="variable-block" markdown="block">

Effect "off" state time.

**Note:** Value in miliseconds.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"OffTime": 500
~~~

</div>

### OnColor
<div class="variable-block" markdown="block">

[Led Color]({{ "/common/led-color" | relative_url }}) to set on all leds when the effect is in "on" state. 

**Required:** Either [OnColor](#oncolor) or [OnColors](#oncolors) is required.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"OnColor": [255, 0, 0]
~~~

</div>

### OffColor
<div class="variable-block" markdown="block">

[Led Color]({{ "/common/led-color" | relative_url }}) to set on all leds when the effect is in "off" state.  

**Required:** Either [OffColor](#offcolor) or [OffColors](#offcolors) is required.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"OffColor": [255, 0, 0]
~~~

</div>

### OnColors
<div class="variable-block" markdown="block">

List of [Led Colors]({{ "/common/led-color" | relative_url }}) to set on the device when the effect is in "on" state.

**Required:** Either [OnColor](#oncolor) or [OnColors](#oncolors) is required.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"OnColors": [
    [255, 0, 0],
    [0, 255, 0],
    [0, 0, 255],
    ...
]
~~~

</div>

### OffColors
<div class="variable-block" markdown="block">

List of [Led Colors]({{ "/common/led-color" | relative_url }}) to set on the device when the effect is in "off" state.

**Required:** Either [OffColor](#offcolor) or [OffColors](#offcolors) is required.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"OffColors": [
    [255, 0, 0],
    [0, 255, 0],
    [0, 0, 255],
    ...
]
~~~

</div>

## Example

~~~
~~~