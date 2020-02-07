---
title: Flow Effect
permalink: /effects/floweffect
---

## Format

~~~
{
    "Type": "FlowEffect",
    "Config": {
        "FillStep": <float>,
        "HueStep": <int>,
        "Saturation": <float>,
        "Brightness": <float>
    }
}
~~~

## Variables

### FillStep
<div class="variable-block" markdown="block">

Determines how fast the colors fill the device

**Required:** No<br>
**Default value:**
~~~
0.05
~~~
**Example:**
~~~
"FillStep": 0.1
~~~

</div>

### HueStep
<div class="variable-block" markdown="block">

Determines color hue increment when the device gets completly filled

**Required:** No<br>
**Default value:**
~~~
30
~~~
**Example:**
~~~
"HueStep": 60
~~~

</div>

### Saturation
<div class="variable-block" markdown="block">

Saturation of colors

**Note:** Value from `0.0` to `1.0`
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1.0
~~~
**Example:**
~~~
"Saturation": 0.5
~~~

</div>

### Brightness
<div class="variable-block" markdown="block">

Brightness of colors

**Note:** Value from `0.0` to `1.0`
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1.0
~~~
**Example:**
~~~
"Brightness": 0.5
~~~

</div>

## Example

~~~
~~~