---
title: Ipc Speed Controller
permalink: /plugins/speed-controllers/ipcspeedcontroller
---

## Format

~~~
{
  "Type": "IpcSpeedController",
  "Config": {
    "IpcName": <string>,
    "DefaultSpeed": <int>,

    "Trigger": <Trigger>
  }
}
~~~

## Data Format

The plugin expects data sent to the [Ipc Server]({{ "/plugins/ipc-server" | relative_url }}) in this format.

~~~
[
  {
    "Port": <PortIdentifier>,
    "Speed": <int>
  }
]
~~~

## Variables

### IpcName
<div class="variable-block" markdown="block">

Client name that will be used when sending data to the [Ipc Server]({{ "/plugins/ipc-server" | relative_url }}).

**Allowed values:** Value must be unique from all plugins using ipc.
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"IpcName": "MySpeedController"
~~~

</div>

### DefaultSpeed
<div class="variable-block" markdown="block">

Fallback speed value that will be used if data for a specific port has not been yet received.

**Required:** No<br>
**Default value:**
~~~
50
~~~
**Example:**
~~~
"DefaultSpeed": 30
~~~

</div>

{% include variables/trigger.md %}

## Example

~~~
~~~