---
title: Wave Effect
permalink: /effects/waveeffect
---

## Format

~~~
{
    "Type": "WaveEffect",
    "Config": {
        "Colors": [<LedColor>],
        "TickInterval": <int>
    }
}
~~~

## Variables

### Colors
<div class="variable-block" markdown="block">

List of [Led Colors]({{ "/common/led-color" | relative_url }}), one for each led 

**Required:** **Yes**<br>
**Default value:**
~~~
[]
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

### TickInterval
<div class="variable-block" markdown="block">

Number of updates before the wave advances to the next led

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