---
title: Static Speed Controller
permalink: /speed-controllers/staticspeedcontroller
---

Sets the speed to a constant value

## Format

~~~
{
    "Type": "StaticSpeedController",
    "Config": {
        "Speed": <int>
    }
}
~~~

## Variables

### Speed
<div class="variable-block" markdown="block">

The [Port Identifier]({{ "/common/port-identifier" | relative_url }}) to copy the speed from

**Required:** No<br>
**Default value:**
~~~
50
~~~
**Example:**
~~~
"Speed": 75
~~~

</div>

## Example

~~~
{
    "Type": "StaticSpeedController",
    "Config": {
        "Speed": 75
    }
}
~~~