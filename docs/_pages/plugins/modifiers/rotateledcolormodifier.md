---
title: Rotate Led Color Modifier
permalink: /plugins/modifiers/rotateledcolormodifier
---

## Format

~~~
{
  "Type": RotateLedColorModifier",
  "Config": {
    "Rotation": <int>,
    "ZoneRotation": [<int>]
  }
}
~~~

## Variables

### Rotation
<div class="variable-block" markdown="block">

Determines how much to rotate whole color list.

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Rotation": 3
~~~

</div>

### LedRotation
<div class="variable-block" markdown="block">

List of led rotations for each device zone.

**Note:** Multi zone devices are `RiingDuo`, `RiingTrio` and `RiingQuad`.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"LedRotation": [8, 2]
~~~
~~~
"LedRotation": [3]
~~~

</div>

## Example

~~~
~~~