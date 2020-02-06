---
title: Dpsg Speed Controller
permalink: /triggers/dpsgspeedcontroller
---

Sets the speed of DPSG controller to one of the internal profiles

**Note:** Only usable with DPSG controllers
{: .notice--warning}

## Format

~~~
{
    "Type": "DpsgSpeedController",
    "Config": {
        "FanMode": <enum>
    }
}
~~~

## Variables

### FanMode
<div class="variable-block" markdown="block">

Internal fan mode name

**Allowed values:** `"Off"`, `"Silent"`, `"Performance"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Silent"
~~~
**Example:**
~~~
"FanMode": "Performance"
~~~

</div>

## Example

~~~
{
    "Type": "DpsgSpeedController",
    "Config": {
        "FanMode": "Performance"
    }
}
~~~