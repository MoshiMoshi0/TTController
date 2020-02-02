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
}
~~~

## Variables

### StateType
<div class="variable-block" markdown="block">

Determines computer state that enables this profile

**Allowed values:** `"Boot"`, `"Shutdown"`, `"Suspend"`
{: .notice--warning}

**Note:** If you want to update or add more profiles with **Boot** type, you need to modify `TTController.Service.exe.Config` file and remove `<add key="boot-profile-saved" value="true" />` line if its present or change its `value` to `false` and restart the service. This is to save **Boot** profiles only once otherwise they will blink when the service loads.
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

List of [Port Identifier]({{ "/common/port-identifier" | relative_url }}) objects modified by this config

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

List of [Led Color]({{ "/common/led-color" | relative_url }}) objects used for [EffectType](#effecttype)

**Required:** **Yes** if [EffectType](#effecttype) is set<br>
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