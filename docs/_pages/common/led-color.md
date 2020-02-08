---
title: Led Color
permalink: /common/led-color
---

Color in RGB format.

**Note:** You can use any online RGB/HSV color picker (e.g. [google's color picker](https://www.google.com/search?q=color+picker)) to help you generate your desired colors
{: .notice--info}

**Note:** Brightness of the color is determined by how close the color is to black `[0, 0, 0]`. For example `[255, 0, 0]` is 100% bright red while `[64, 0, 0]` is 25% bright red.
{: .notice--info}

## Format
~~~
[<byte>, <byte>, <byte>]
~~~

## Examples

~~~ json
[255, 0, 0]
~~~
~~~ json
[64, 64, 64]
~~~