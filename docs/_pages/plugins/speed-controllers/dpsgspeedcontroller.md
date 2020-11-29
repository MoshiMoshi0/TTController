---
title: Dpsg Speed Controller
permalink: /plugins/speed-controllers/dpsgspeedcontroller
---

Sets the speed of DPSG controller to one of the internal profiles.

**Important:** Only usable with DPSG controllers.
{: .notice--warning}

## Format

~~~
{
    "Type": "DpsgSpeedController",
    "Config": {
        "FanMode": <enum>,

        "Trigger": <Trigger>
    }
}
~~~

## Variables

### FanMode
<div class="variable-block" markdown="block">

Internal fan mode name.

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

{% include variables/trigger.md %}

## Example

~~~
{
    "Type": "DpsgSpeedController",
    "Config": {
        "FanMode": "Performance"
    }
}
~~~