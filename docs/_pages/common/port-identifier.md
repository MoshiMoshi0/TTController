---
title: Port Identifier
permalink: /common/port-identifier
---

Port identifiers are used to precisely specify what device and what port you want to target.

**Note:** You can use `Main Menu -> Debug -> Controllers` menu to find your port identifiers.
{: .notice--info}

## Format

Controllers with ports:
~~~
[<vendor-id>, <product-id>, <port-id>]
~~~

Controllers without ports:
~~~
[<vendor-id>, <product-id>]
~~~

## Examples
~~~json
[9802, 8101, 1]
~~~