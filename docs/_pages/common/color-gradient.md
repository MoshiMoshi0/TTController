---
title: Color Gradient
permalink: /common/color-gradient
---

List of color gradient points in `[<location>, <LedColor>]` format

**Note:** The `<location>` values can depend on gradient use case and should be properly explained on the page where the gradient is used
{: .notice--info}

## Format
~~~
[<ColorGradientPoint>]
~~~

## Examples

~~~ json
[
    [0,  [0, 0, 0]],
    [30, [64, 0, 0]],
    [60  [128, 0, 0]],
    [70, [256, 0, 0]],
]
~~~
~~~ json
[
    [0,   [0, 0, 256]],
    [0.3, [64, 0, 128]],
    [0.6  [128, 0, 64]],
    [1.0, [256, 0, 0]],
]
~~~