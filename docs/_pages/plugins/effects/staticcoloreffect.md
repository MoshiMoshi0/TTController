---
title: Static Color Effect
permalink: /plugins/effects/staticcoloreffect
---

## Format

~~~
{
    "Type": "StaticColorEffect",
    "Config": {
        "Color": <LedColorProvider>
    }
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
{
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