### ColorGenerationMethod
<div class="variable-block" markdown="block">

Tells the effect how to generate colors for multiple devices.
The actual behaviour is depended on the effect implementation.

{% capture lerptype-values %}

**Allowed values:**

* `"PerPort"` - Duplicates the effect on each device.
* `"SpanPorts"` - Treats multiple devices as one.

{% endcapture %}

<div class="notice--warning">
  {{ lerptype-values | markdownify }}
</div>

**Required:** No<br>
**Default value:**
~~~
"PerPort"
~~~
**Example:**
~~~
"ColorGenerationMethod": "SpanPorts"
~~~

</div>