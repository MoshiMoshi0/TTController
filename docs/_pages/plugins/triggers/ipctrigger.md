---
title: Ipc Trigger
permalink: /plugins/triggers/ipctrigger
---

## Format

~~~
{
  "Type": "IpcTrigger",
  "Config": {
    "IpcName": <string>,
    "EnabledByDefault": <bool>
  }
}
~~~

## Data Format

The plugin expects data sent to the [Ipc Server]({{ "/common/ipc-server" | relative_url }}) in this format.

~~~
{
  "Enabled": <bool>
}
~~~

## Variables

### IpcName
<div class="variable-block" markdown="block">

Client name that will be used when sending data to the [Ipc Server]({{ "/common/ipc-server" | relative_url }}).

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

### EnabledByDefault
<div class="variable-block" markdown="block">

Determines default trigger state before data is received.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"EnabledByDefault": true
~~~

</div>

## Example

~~~
~~~