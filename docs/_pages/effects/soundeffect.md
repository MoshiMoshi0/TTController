---
title: Sound Effect
permalink: /effects/soundeffect
---

## Format

~~~
{
    "Type": "SoundEffect",
    "Config": {
        "UseAverage": <bool>,
        "MinimumFrequency": <int>,
        "MaximumFrequency": <int>,
        "ScalingStrategy": <enum>,
        "ScalingFactor": <float>,
        "ColorGradient": <LedColorGradient>
    }
}
~~~

## Variables

### UseAverage
<div class="variable-block" markdown="block">

Determines if FFT values are averaged

**Required:** No<br>
**Default value:**
~~~
true
~~~
**Example:**
~~~
"UseAverage": false
~~~

</div>

### MinimumFrequency
<div class="variable-block" markdown="block">

Mimimum frequency for FFT

**Required:** No<br>
**Default value:**
~~~
100
~~~
**Example:**
~~~
"MinimumFrequency": 1000
~~~

</div>

### MaximumFrequency
<div class="variable-block" markdown="block">

Maximum frequency for FFT

**Required:** No<br>
**Default value:**
~~~
10000
~~~
**Example:**
~~~
"MaximumFrequency": 12000
~~~

</div>

### ScalingStrategy
<div class="variable-block" markdown="block">

FFT value scaling function

**Allowed values:** `"Decibel"`, `"Linear"`, `"Sqrt"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Sqrt"
~~~
**Example:**
~~~
"ScalingStrategy": "Decibel"
~~~

</div>

### ScalingFactor
<div class="variable-block" markdown="block">

FFT scaling scaling factor

**Note:** Used for `"Linear"` and `"Sqrt"` [ScalingStrategy](#scalingstrategy)
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
2.0
~~~
**Example:**
~~~
"ScalingFactor": 4.0
~~~

</div>

### ColorGradient
<div class="variable-block" markdown="block">

A [Led Color Gradient]({{ "/common/led-color-gradient" | relative_url }}) to translate FFT values to led colors

**Note:** The gradient `<location>` values are FFT values from `0.0` to `1.0`
{: .notice--info}

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"ColorGradient": [
    [0.0, [0, 0, 0]],
    [0.75, [255, 255, 255]],
    [1.0, [255, 0, 0]]
]
~~~

</div>

## Example

~~~
~~~