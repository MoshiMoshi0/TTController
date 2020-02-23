---
title: Raw Effect
permalink: /effects/raweffect
---

Allows usage of effects saved on the controller box.

## Format

~~~
{
    "Type": "RawEffect",
    "Config": {
        "EffectType": <string>,
        "Colors": [<LedColor>]
    }
}
~~~

## Variables

### EffectType
<div class="variable-block" markdown="block">

Effect type name.

**Note:** Supported effect type names are listed in the `Main Menu -> Debug -> Controllers` menu.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
"Full"
~~~
**Example:**
~~~
"EffectType": "Pulse_Fast"
~~~

</div>

### Colors
<div class="variable-block" markdown="block">

List of [Led Colors]({{ "/common/led-color" | relative_url }}), one for each led.

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