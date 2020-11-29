---
title: Lerp Led Color Modifier
permalink: /plugins/modifiers/lerpledcolormodifier
---

Modifies colors to match the desired length using interpolation.

## Format

~~~
{
    "Type": LerpLedColorModifier",
    "Config": {
        "LerpType": <enum>
    }
}
~~~

## Variables

### LerpType
<div class="variable-block" markdown="block">

Determines interpolation type.

{% capture lerptype-values %}

**Allowed values:**

* `"Smooth"` - Stretches or shrinks effect colors list to match the device led count using a gradient.
* `"Nearest"` - Stretches or shrinks effect colors list to match the device led count by copying/removing the nearest color.

{% endcapture %}

<div class="notice--warning">
  {{ lerptype-values | markdownify }}
</div>


**Required:** No<br>
**Default value:**
~~~
"Smooth"
~~~
**Example:**
~~~
"LerpType": "Nearest"
~~~

</div>

## Example

~~~
~~~