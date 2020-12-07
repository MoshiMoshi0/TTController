---
title: Schedule Trigger
permalink: /plugins/triggers/scheduletrigger
---

Triggers based on configured schedule on per minute, hour, day or week basis.

## Format

~~~
{
  "Type": "ScheduleTrigger",
  "Config": {
      "Scope": <enum>,
      "Value": <bool>,
      "UpdateInterval": <TimeSpan>,
      "Schedule": <string>
  }
}
~~~

## Variables

### Scope
<div class="variable-block" markdown="block">

Determines the repeat period of the schedule

For example when set to `"Day"` the [Schedule](#schedule) entries will have be set to time of day, and when set to `"Minute"` the [Schedule](#schedule) will have to be set to seconds of a minute.

**Allowed values:** `"Minute"`, `"Hour"`, `"Day"`, `"Week"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Day"
~~~
**Example:**
~~~
"Scope": "Minute"
~~~

</div>

### Value
<div class="variable-block" markdown="block">

What value to return when current time matches the schedule. This can be used to invert the schedule.

**Required:** No<br>
**Default value:**
~~~
true
~~~
**Example:**
~~~
"Value": false
~~~

</div>

### UpdateInterval
<div class="variable-block" markdown="block">

Determines how often to check the current time.

By default set automatically to:
* `"00:00:01"` - when [Scope](#scope) is set to `"Minute"` 
* `"00:01:00"` - when [Scope](#scope) is set to `"Hour"` 
* `"00:01:00"` - when [Scope](#scope) is set to `"Day"` 
* `"00:15:00"` - when [Scope](#scope) is set to `"Week"` 

**Required:** No<br>
**Default value:**
~~~
~~~
**Example:**
~~~
"UpdateInterval": "00:00:10"
~~~

</div>

### Schedule
<div class="variable-block" markdown="block">

List of schedule entries in `"<start time> -> <end time>"` format.

Allowed `<start time>` and `<end time>` formats: 
* `"d.hh:mm"` - when [Scope](#scope) is set to `"Week"`
* `"hh:mm"` - when [Scope](#scope) is set to `"Day"` or `"Hour"`
* `"ss"` - when [Scope](#scope) is set to `"Minute"`

**Required:** **Yes**<br>
**Default value:**
~~~
[]
~~~
**Example:**
~~~
"Schedule": [
  "00:10 -> 00:20",
  "00:30 -> 00:40"
]
~~~
~~~
"Schedule": [
  "10 -> 20",
  "40 -> 50"
]
~~~

</div>

## Examples
~~~ json
~~~