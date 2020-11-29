---
title: ComputerStateProfile Configuration
permalink: /config/computerstateprofile
---

## Format

~~~
{
  "StateType":  [<enum>],
  "Ports": [<PortIdentifier>],
  "Speed": <byte>,
  "EffectType": <string>,
  "Color": <LedColorProvider>
}
~~~

## Variables

### StateType
<div class="variable-block" markdown="block">

Determines computer state that enables this profile.

**Allowed values:** `"Boot"`, `"Shutdown"`, `"Suspend"`
{: .notice--warning}

**Important:** If you want to update or add more profiles with **Boot** type, you need to modify `TTController.Service.exe.Config` file and remove `<add key="boot-profile-saved" value="true" />` line if its present or change its `value` to `false` and restart the service. This is done to save **Boot** profiles only once, otherwise the service will initialize slower and they will cause leds to briefly flash on boot.
{: .notice--warning}

**Required:** **Yes**<br>
**Default value:**
~~~
"Shutdown"
~~~
**Example:**
~~~
"StateType": "Boot"
~~~

</div>

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

### Speed
<div class="variable-block" markdown="block">

Speed to set the devices to when the profile is enabled.

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Speed": 35
~~~

</div>

### EffectType
<div class="variable-block" markdown="block">

Effect type to set the devices to when the profile is enabled.

**Note:** Supported effect types are different for each controller. You can use `Main Menu -> Debug -> Controllers` menu to find what types are supported.
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
"PerLed"
~~~
**Example:**
~~~
"EffectType": "Full"
~~~

</div>

### Color
<div class="variable-block" markdown="block">

A [Led Color Provider]({{ "/common/led-color-provider" | relative_url }}) object.

**Required:** No<br>
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

## Examples
~~~ json
{
  "StateType": "Shutdown",
  "Ports": [
    [9802, 8101, 1]
  ],
  "Speed": 35,
  "Color": {
    "Full": [255, 0, 0]
  }
}
~~~
~~~ json
{
  "StateType": "Suspend",
  "Ports": [
    [9802, 8101, 1],
    [9802, 8101, 3]
  ],
  "Color": {
    "Gradient": [
      [0, [0, 0, 0]],
      [0.5, [255, 0, 0]],
      [1, [0, 0, 0]]
    ]
  }
}
~~~