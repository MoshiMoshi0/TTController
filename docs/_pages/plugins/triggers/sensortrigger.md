---
title: Sensor Trigger
permalink: /plugins/triggers/sensortrigger
---

Triggers based on configured sensor values.

## Format

~~~
{
  "Type": "SensorTrigger",
  "Config": {
      "Sensors": [<SensorIdentifier>],
      "SensorMixFunction": <enum>,
      "Value": <float>,
      "ComparsionType": <enum>
  }
}
~~~

## Variables

### Sensors
<div class="variable-block" markdown="block">

List of [Sensor Identifiers]({{ "/common/sensor-identifier" | relative_url }}) to read values from.

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Sensors": ["/intelcpu/0/temperature/8"]
~~~

</div>

### SensorMixFunction
<div class="variable-block" markdown="block">

Determines how to combine values from [Sensors](#sensors).

**Allowed values:** `"Maximum"`, `"Minimum"`, `"Average"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Maximum"
~~~
**Example:**
~~~
"SensorMixFunction": "Average"
~~~

</div>

### Value
<div class="variable-block" markdown="block">

Value to compare sensor values to.

**Required:** **Yes**<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"Value": 50.5
~~~

</div>

### ComparsionType
<div class="variable-block" markdown="block">

Determines how to compare current sensor values and the configured [Value](#value).

**Allowed values:** `"Equal"`, `"Greater"`, `"GreaterOrEqual"`, `"Less"`, `"LessOrEqual"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Greater"
~~~
**Example:**
~~~
"ComparsionType": "Less"
~~~

</div>

## Examples
~~~ json
~~~