---
title: Copy Speed Controller
permalink: /plugins/speed-controllers/copyspeedcontroller
---

Copies the speed from another port.

**Note:** In some cases the copied speed can lag behind the [Target](#target) port one update tick. Put the profiles that use this speed controller as last in the list to prevent that.
{: .notice--info}

## Format

~~~
{
  "Type": "CopySpeedController",
  "Config": {
    "Target": <PortIdentifier>,

    "Trigger": <Trigger>
  }
}
~~~

## Variables

### Target
<div class="variable-block" markdown="block">

The [Port Identifier]({{ "/common/port-identifier" | relative_url }}) to copy the speed from.

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Target": [9802, 8101, 1]
~~~

</div>

{% include variables/trigger.md %}

## Example

~~~
{
  "Type": "CopySpeedController",
  "Config": {
    "Target": [9802, 8101, 1]
  }
}
~~~