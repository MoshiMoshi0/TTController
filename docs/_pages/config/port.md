---
title: Port Configuration
permalink: /config/port
---

## Format

~~~
{
  "Ports": [<PortIdentifier>],
  "Config": {
    "Name": <string>,
    "DeviceType": <string>,
    "ColorModifiers": [<LedColorModifier>],
    "IgnoreColorCache": <bool>,
    "IgnoreSpeedCache": <bool>
  }
}
~~~

## Variables

### Ports
<div class="variable-block" markdown="block">

List of [Port Identifiers]({{ "/common/port-identifier" | relative_url }}) modified by this config.

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Ports": [
  [9802, 9101, 1],
  [9802, 9101, 3]
]
~~~

</div>

### Name
<div class="variable-block" markdown="block">

Name of this port config.

**Note:** Unused.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
"Unknown"
~~~
**Example:**
~~~
"Name": "Top Fans"
~~~

</div>

### DeviceType
<div class="variable-block" markdown="block">

Used by effects to know exact led count and zones of each device<br>This ensures that the colors are generated properly.

**Allowed values:** `"Default"`, `"RiingQuad"`, `"RiingTrio"`, `"RiingDuo"`, `"Riing"`, `"FloeRiing"`, `"PurePlus"`, `"Level20Desk"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Default"
~~~
**Example:**
~~~
"DeviceType": "RiingTrio"
~~~

</div>

### ColorModifiers
<div class="variable-block" markdown="block">

List of [Led Color Modifier]({{ "/plugins/led-color-modifier" | relative_url }}) objects to use for modifying colors before they are set on [Ports](#ports).

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
[
  {
    "Type": "LerpLedColorModifier"
  },
  {
    "Type": "RotateLedColorModifier",
    "Config": {
      "ZoneRotation": [1, 2, 3]
    }
  }
]
~~~

</div>

### IgnoreColorCache
<div class="variable-block" markdown="block">

By default the service stores the led colors in a cache and only sends data to the controller if there is a mismatch to improve the performance.
Setting `"IgnoreColorCache"` to `true` ignores this optimization.

**Note:** Newer controllers like `Riing Quad controller` or `Level 20 Desk controller` require this option to be set, otherwise they will fallback to a default rainbow effect after some time of inactivity.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"IgnoreColorCache": true
~~~

</div>

### IgnoreSpeedCache
<div class="variable-block" markdown="block">

By default the service stores the port speed in a cache and only sends data to the controller if there is a mismatch to improve the performance.
Setting `"IgnoreSpeedCache"` to `true` ignores this optimization.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"IgnoreSpeedCache": true
~~~

## Examples
~~~ json
{
  "Ports": [
  	[9802, 8101, 1],
  	[9802, 8101, 2]
  ],
  "Config": {
    "DeviceType": "RiingQuad",
    "IgnoreColorCache": true,
    "ColorModifiers": [
      {
        "Type": "LerpLedColorModifier"
      }
    ]
  }
}
~~~