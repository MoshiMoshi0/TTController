---
title: Per Led Color Effect
permalink: /effects/perledcoloreffect
---

## Format

~~~
{
    "Type": "PerLedColorEffect",
    "Config": {
        "Colors": [<LedColor>]
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

## Example

~~~
~~~