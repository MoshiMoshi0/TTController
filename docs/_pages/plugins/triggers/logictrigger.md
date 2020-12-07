---
title: Logic Trigger
permalink: /plugins/triggers/logictrigger
---

Combines multiple triggers using boolean operators.

## Format

~~~
{
  "Type": "LogicTrigger",
  "Config": {
      "Operation": <enum>,
      "Negate": <bool>,
      "Triggers": [<Trigger>],
  }
}
~~~

## Variables

### Operation
<div class="variable-block" markdown="block">

Determines how to combine the [Triggers](#triggers).

**Allowed values:** `"And"`, `"Or"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"And"
~~~
**Example:**
~~~
"Operation": "Or"
~~~

</div>

### Negate
<div class="variable-block" markdown="block">

Determines if the combined value is negated.

**Required:** No<br>
**Default value:**
~~~
false
~~~
**Example:**
~~~
"Negate": true
~~~

</div>

### Triggers
<div class="variable-block" markdown="block">

List of [Triggers]({{ "/plugins/trigger" | relative_url }}) to combine the values of.

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Triggers": [
  {...},
  {...}
]
~~~

</div>

## Examples
~~~ json
~~~