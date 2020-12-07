---
title: Color Sequence Effect
permalink: /plugins/effects/colorsequenceeffect
---

## Format

~~~
{
  "Type": "ColorSequenceEffect",
  "Config": {
    "Sequence": [<ColorSequenceEntry>],

    "ColorGenerationMethod": <enum>,
    "Trigger": <Trigger>
  }
}
~~~

### ColorSequenceEntry
~~~
{
  "TransitionTime": <int>,
  "HoldTime": <int>,
  "Color": <LedColorProvider>
}
~~~

## Variables

### TransitionTime
<div class="variable-block" markdown="block">

Determines transition time to the next sequence entry.
The colors will be blended from current to next sequence entry while in transition state.

**Allowed values:** Value in milliseconds.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"TransitionTime": "500"
~~~

</div>

### HoldTime
<div class="variable-block" markdown="block">

Determines hold time of the current sequence entry colors.

**Allowed values:** Value in miliseconds.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"HoldTime": "500"
~~~

</div>

### Color
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object with colors when the pulse is in full brightness.

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