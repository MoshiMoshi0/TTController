---
title: Reverse Led Color Modifier
permalink: /plugins/modifiers/reverseledcolormodifier
---

Modifies colors by reversing the colors.

## Format

~~~
{
    "Type": ReverseLedColorModifier",
    "Config": {
        "Reverse": <bool>,
        "ZoneReverse": [<bool>]
    }
}
~~~

## Variables

### Reverse
<div class="variable-block" markdown="block">

If set to `true` the whole color list is reversed.

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Reverse": true
~~~

</div>

### ZoneReverse
<div class="variable-block" markdown="block">

Determines if led colors are reversed for each device zone.

**Note:** Multi zone devices are `RiingDuo`, `RiingTrio` and `RiingQuad`.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"ZoneReverse": [true, false]
~~~
~~~
"ZoneReverse": [true]
~~~

</div>

## Example

~~~
~~~