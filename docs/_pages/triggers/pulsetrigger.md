---
title: Pulse Trigger
permalink: /triggers/pulsetrigger
---

Triggers on and off based on configured timing.

## Format

~~~
{
  "Type": "PulseTrigger",
  "Config": {
      "OnTime": <int>,
      "OffTime": <int>
  }
}
~~~

## Variables

### OnTime
<div class="variable-block" markdown="block">

Trigger on time

**Note:** Value in miliseconds
{: .notice--note}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"OnTime": 250
~~~

</div>

### OffTime
<div class="variable-block" markdown="block">

Trigger off time

**Note:** Value in miliseconds
{: .notice--note}

**Required:** No<br>
**Default value:**
~~~
1000
~~~
**Example:**
~~~
"OffTime": 250
~~~

</div>

## Examples
~~~ json
~~~