---
title: Aurora Effect
permalink: /plugins/effects/auroraeffect
---

## Format

~~~
{
    "Type": "AuroraEffect",
    "Config": {
        "Step": <float>,
        "Length": <int>,
        "Mirror": <bool>,
        "Brightness": <float>,
        "Saturation": <float>,
        "Gradient": <LedColorGradient>,

        "ColorGenerationMethod": <enum>,
        "Trigger": <Trigger>
    }
}
~~~

## Variables

### Step
<div class="variable-block" markdown="block">

Gradient advance speed.

**Required:** No<br>
**Default value:**
~~~
0.003
~~~
**Example:**
~~~
"Step": 0.01
~~~

</div>

### Length
<div class="variable-block" markdown="block">

Gradient length.

**Note:** Higher values improve color smoothness.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
64
~~~
**Example:**
~~~
"Length": 256
~~~

</div>

### Mirror
<div class="variable-block" markdown="block">

If set to `true`, the gradient is mirrored across device center axis. 

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"Mirror": true
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

### Gradient
<div class="variable-block" markdown="block">

A [Led Color Gradient]({{ "/common/led-color-gradient" | relative_url }}).

**Note:** The gradient `<location>` values are values from `0.0` to `1.0`.
{: .notice--info}

**Note:** If not set, a default rainbow gradient will be generated.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Gradient": [
    [0.0, [0, 0, 0]],
    [0.75, [255, 255, 255]],
    [1.0, [255, 0, 0]]
]
~~~

</div>

{% include variables/colorgenerationmethod.md %}

{% include variables/trigger.md %}

## Example

~~~
~~~