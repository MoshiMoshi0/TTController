---
title: Raw Effect
permalink: /plugins/effects/raweffect
---

Allows usage of effects saved on the controller box.

## Format

~~~
{
  "Type": "RawEffect",
  "Config": {
    "EffectType": <string>,
    "Color": <LedColorProvider>,
        
    "ColorGenerationMethod": <enum>,
    "Trigger": <Trigger>
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

{% include variables/colorgenerationmethod.md %}

{% include variables/trigger.md %}

## Example

~~~
~~~