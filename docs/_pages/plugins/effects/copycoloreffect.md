---
title: Copy Color Effect
permalink: /plugins/effects/copycoloreffect
---

Copies the led colors from another port.

**Note:** In some cases the copied colors can lag behind the [Target](#target) port one update tick. Put the profiles that use this effect as last in the list to prevent that.
{: .notice--info}

## Format

~~~
{
    "Type": "CopyColorEffect",
    "Config": {
        "Target": <PortIdentifier>
    }
}
~~~

## Variables

### Target
<div class="variable-block" markdown="block">

The [Port Identifier]({{ "/common/port-identifier" | relative_url }}) to copy the colors from.

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Target": [9802, 8101, 1]
~~~

</div>

## Example

~~~
{
    "Type": "CopyColorEffect",
    "Config": {
        "Target": [9802, 8101, 1]
    }
}
~~~