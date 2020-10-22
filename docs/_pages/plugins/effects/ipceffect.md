---
title: Ipc Effect
permalink: /plugins/effects/ipceffect
---

## Format

~~~
{
  "Type": "IpcEffect",
  "Config": {
    "IpcName": <string>
  }
}
~~~

## Data Format

The plugin expects data sent to the [Ipc Server]({{ "/plugins/ipc-server" | relative_url }}) in this format.

~~~
[
  {
    "Port": <PortIdentifier>,
    "Colors": [<LedColor>]
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
"IpcName": "MyEffect"
~~~

</div>

## Example

~~~
~~~