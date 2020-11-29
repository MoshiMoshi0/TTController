---
title: Trim Led Color Modifier
permalink: /plugins/modifiers/trimledcolormodifier
---

## Format

~~~
{
  "Type": ReverseLedColorModifier",
  "Config": {
    "WrapRemainder": <bool>
  }
}
~~~

## Variables

### WrapRemainder
<div class="variable-block" markdown="block">

If set to `true` the colors that exceed the desired length will be wrapped to the beginning, otherwise remove excess colors.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"WrapRemainder": true
~~~

</div>

## Example

~~~
~~~