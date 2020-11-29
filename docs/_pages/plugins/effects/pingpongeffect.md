---
title: Ping Pong Effect
permalink: /plugins/effects/pingpongeffect
---

## Format

~~~
{
    "Type": "PingPongEffect",
    "Config": {
        "Step": <float>,
        "Height": <float>,
        "Width": <float>,
        "ColorGradient": <LedColorGradient>,
        "EnableSmoothing": <bool>,
        
        "ColorGenerationMethod": <enum>,
        "Trigger": <Trigger>
    }
}
~~~

## Variables

### Step
<div class="variable-block" markdown="block">

Speed of the ping pong region.

**Required:** No<br>
**Default value:**
~~~
0.01
~~~
**Example:**
~~~
"Step": 0.03
~~~

</div>

### Height
<div class="variable-block" markdown="block">

Height of the ping pong region.

**Note:** Value from `0.0` to `1.0`.
{: .notice--info} 

**Required:** No<br>
**Default value:**
~~~
0.2
~~~
**Example:**
~~~
"Height": 0.33
~~~

</div>

### Width
<div class="variable-block" markdown="block">

Width of the ping pong region.

**Note:** Value from `0.0` to `1.0`.
{: .notice--info} 

**Required:** No<br>
**Default value:**
~~~
0.5
~~~
**Example:**
~~~
"Width": 0.25
~~~

</div>

### ColorGradient
<div class="variable-block" markdown="block">

A [Led Color Gradient]({{ "/common/led-color-gradient" | relative_url }}) to translate ping pong region to led colors.

**Note:** The gradient `<location>` values are between `0.0` and `1.0` where `0.0` means bottom of first fan and `1.0` means top of last fan.
{: .notice--info}

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"ColorGradient": [
    [0.0, [16, 0, 0]],
    [0.5, [64, 0, 0]],
    [1.0, [256, 0, 0]],
    ...
]
~~~

</div>

### EnableSmoothing
<div class="variable-block" markdown="block">

Enables smoothing of edges of the ping pong region.

**Required:** No<br>
**Default value:**
~~~
true
~~~
**Example:**
~~~
"EnableSmoothing": false
~~~

</div>

{% include variables/colorgenerationmethod.md %}

{% include variables/trigger.md %}

## Example

~~~
~~~