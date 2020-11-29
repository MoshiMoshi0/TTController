---
title: Spectrum Effect
permalink: /plugins/effects/spectrumeffect
---

## Format

~~~
{
    "Type": "SpectrumEffect",
    "Config": {
        "Saturation": <float>,
        "Brightness": <float>,
        "HueStep": <float>,

        "ColorGenerationMethod": <enum>,
        "Trigger": <Trigger>
    }
}
~~~

## Variables

### Saturation
<div class="variable-block" markdown="block">

Saturation of colors.

**Note:** Value from `0.0` to `1.0`.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1.0
~~~
**Example:**
~~~
"Saturation": 0.5
~~~

</div>

### Brightness
<div class="variable-block" markdown="block">

Brightness of colors.

**Note:** Value from `0.0` to `1.0`.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1.0
~~~
**Example:**
~~~
"Brightness": 0.5
~~~

</div>

### HueStep
<div class="variable-block" markdown="block">

Color hue increment each update tick.

**Required:** No<br>
**Default value:**
~~~
1.0
~~~
**Example:**
~~~
"HueStep": 1.75
~~~

</div>

{% include variables/colorgenerationmethod.md %}

{% include variables/trigger.md %}

## Example

~~~
~~~