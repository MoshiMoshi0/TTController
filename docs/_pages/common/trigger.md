---
title: Trigger
permalink: /common/trigger
---

## Format

~~~
{
  "Type": <string>,
  "Config": <TriggerConfig>,
}
~~~

## Variables

### Type
<div class="variable-block" markdown="block">

Name of the trigger

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Type": "PulseTrigger"
~~~

</div>

### Config
<div class="variable-block" markdown="block">

Config of the trigger

**Note:** See page of the trigger you are configuring for detailed documentation
{: .notice--info}

**Required:** Depends on trigger<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Config": {
    "On": 1000,
    "Off": 1000
}
~~~

</div>

## Examples
~~~ json
{
    "Type": "AlwaysTrigger"
}
~~~
~~~ json
{
    "Type": "PulseTrigger",
    "Config": {
        "On": 1000,
        "Off": 1000
    }
}
~~~