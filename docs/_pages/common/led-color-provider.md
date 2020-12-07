---
title: Led Color Provider
permalink: /common/led-color-provider
---

A helper object to simplify configuration of colors for effects. 

## Format
~~~
{
  "Full": <LedColor>,
  "PerLed": [<LedColor>],
  "Gradient": <LedColorGradient>
}
~~~

**Important:** Only one of the above variables will be used.
{: .notice--warning}

## Examples

~~~ json
{
  "PerLed": [
    [255, 0, 0],
    [0, 255, 0],
    [255, 0, 0],
    ...
  ]
}
~~~
~~~ json
{
  "Full": [255, 0, 0]
}
~~~
~~~ json
{
  "Gradient": [
    [0, [255, 0, 0]],
    [0.5, [0, 255, 0]],
    [1.0, [255, 0, 0]]
  ]
}
~~~