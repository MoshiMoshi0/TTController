---
title: ComputerStateProfile Configuration
permalink: /config/computerstateprofile
---

## Format

~~~
{
  "StateType":  [<StateType>],
  "Ports": [<PortIdentifier>],
  "Speed": <byte?>,
  "EffectType": <string>,
  "EffectColors": [<LedColor>]
  "EffectColor": <LedColor>
}
~~~

## Variables

### StateType
<div class="variable-block" markdown="block">

Determines computer state that enables this profile

**Allowed values:** `"Boot"`, `"Shutdown"`, `"Suspend"`
{: .notice--warning}

**Note:** If you want to update or add more profiles with **Boot** type, you need to modify `TTController.Service.exe.Config` file and remove `<add key="boot-profile-saved" value="true" />` line if its present or change its `value` to `false` and restart the service. This is done to save **Boot** profiles only once, otherwise the service will initialize slower and they will cause leds to briefly flash on boot.
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

List of [Port Identifiers]({{ "/common/port-identifier" | relative_url }}) modified by this config

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

Speed to set the devices to when the profile is enabled

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

Effect type to set the devices to when the profile is enabled

**Note:** Supported effect types are different for each controller. You can use `Main Menu -> Debug -> Controllers` menu to find what types are supported. 
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"EffectType": "Full"
~~~

</div>

### EffectColors
<div class="variable-block" markdown="block">

List of [Led Colors]({{ "/common/led-color" | relative_url }}) used for [EffectType](#effecttype)

**Required:** if [EffectType](#effecttype) is set, either [EffectColors](#EffectColors) or [EffectColor](#EffectColor) has to be set<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"EffectColors": [
    [255, 0, 0],
    [255, 0, 0],
    ...
]
~~~

</div>

### EffectColor
<div class="variable-block" markdown="block">

[Led Color]({{ "/common/led-color" | relative_url }}) used for [EffectType](#effecttype)

**Note:** This color will be cloned to match device led count. If you want to use only one [Led Color]({{ "/common/led-color" | relative_url }}), see [EffectColors](#EffectColors)
{: .notice--info}

**Required:** if [EffectType](#effecttype) is set, either [EffectColors](#EffectColors) or [EffectColor](#EffectColor) has to be set<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"EffectColor": [255, 0, 0]
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
  "EffectType": "Full",
  "EffectColors": [
    [255, 0, 0]
  ]
}
~~~
~~~ json
{
  "StateType": "Suspend",
  "Ports": [
    [9802, 8101, 1],
    [9802, 8101, 3]
  ],
  "EffectType": "Pulse_Fast",
  "EffectColor": [50, 0, 0]
}
~~~