---
title: Trigger Plugin
permalink: /common/trigger
---

Triggers cause other plugins to be enabled or disabled dynamically.

## Format

~~~
{
  "Type": <string>,
  "Config": <TriggerConfig>
}
~~~

## Variables

### Type
<div class="variable-block" markdown="block">

Name of the trigger.

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

Config of the trigger.

**Note:** See page of the trigger you are configuring for detailed documentation.
{: .notice--info}

**Required:** Depends on the trigger type.<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Config": {
    "OnTime": 1000,
    "OffTime": 1000
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
        "OnTime": 1000,
        "OffTime": 1000
    }
}
~~~