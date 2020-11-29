---
title: Blink Effect
permalink: /plugins/effects/blinkeffect
---

## Format

~~~
{
    "Type": "BlinkEffect",
    "Config": {
        "OnTime": <int>,
        "OffTime": <int>,
        "OnColor": <LedColorProvider>,
        "OffColor": <LedColorProvider>,

        "ColorGenerationMethod": <enum>,
        "Trigger": <Trigger>
    }
}
~~~

## Variables

### OnTime
<div class="variable-block" markdown="block">

Effect "on" state time.

**Note:** Value in miliseconds.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"OnTime": 500
~~~

</div>

### OffTime
<div class="variable-block" markdown="block">

Effect "off" state time.

**Note:** Value in miliseconds.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"OffTime": 500
~~~

</div>

### OnColor
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object used when the effect is in "on" state.

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

### OffColor
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object used when the effect is in "off" state.

**Required:** No<br>
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