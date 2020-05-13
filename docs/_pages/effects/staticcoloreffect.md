---
title: Static Color Effect
permalink: /effects/staticcoloreffect
---

## Format

~~~
{
    "Color": <LedColorProvider>
}
~~~

## Variables

### Color
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object.

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