---
title: Razer Connect Effect
permalink: /plugins/effects/razerconnecteffect
---

Receives 5 colors from razer chroma.

## Format

~~~
{
    "Type": "RazerConnectEffect",
    "Config": {
        "Layer": <enum>
    }
}
~~~

## Variables

### Layer
<div class="variable-block" markdown="block">

Specifies which razer connect layer colors to use.

**Allowed values:** `"Base"`, `"Custom"`, `"Both"`
{: .notice--warning}

**Required:** No<br>
**Default value:**
~~~
"Custom"
~~~
**Example:**
~~~
"Layer": "Both"
~~~

</div>

## Example

~~~
{
    "Type": "RazerConnectEffect"
}
~~~