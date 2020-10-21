---
title: Zone Effect
permalink: /plugins/effects/zoneeffect
---

## Format

~~~
{
  "Effects": [<Effect>]
}
~~~

## Variables

### Effects
<div class="variable-block" markdown="block">

List of [Effect]({{ "/common/effect" | relative_url }}) objects.

**Allowed values:** Each effect will be matched to the device zone.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Effects": [
  {
    "Type": "StaticColorEffect",
    "Config": {
      "Color": {
        "Full": [255, 0, 0]
      }
    }
  },
  {
    "Type": "StaticColorEffect",
    "Config": {
      "Color": {
        "Full": [0, 255, 0]
      }
    }
  }
]
~~~

</div>

## Example

~~~
~~~