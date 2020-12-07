---
title: Process Trigger
permalink: /plugins/triggers/processtrigger
---

Triggers if any of the configured processes is running.

## Format

~~~
{
  "Type": "ProcessTrigger",
  "Config": {
      "Processes": [<string>],
      "UpdateInterval": <int>
  }
}
~~~

## Variables

### Processes
<div class="variable-block" markdown="block">

List of process names.

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Processes": ["cmd", "notepad"]
~~~

</div>

### UpdateInterval
<div class="variable-block" markdown="block">

Determines how often to check the list of currently running processes.

**Note:** Value in miliseconds
{: .notice--info}

**Required:** No<br>
**Default value:**
~~~
2500
~~~
**Example:**
~~~
"UpdateInterval": 5000
~~~

</div>

## Examples
~~~ json
~~~