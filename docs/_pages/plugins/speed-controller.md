---
title: Speed Controller Plugin
permalink: /plugins/speed-controller
---

Speed controllers generate speed values for specific ports.

## Format

~~~
{
  "Type": <string>,
  "Config": <SpeedControllerConfig>
}
~~~

## Types

* [CopySpeedController]({{ "/plugins/speed-controllers/copyspeedcontroller" | relative_url}})
* [DpsgSpeedController]({{ "/plugins/speed-controllers/dpsgspeedcontroller" | relative_url}})
* [IpcSpeedController]({{ "/plugins/speed-controllers/ipcspeedcontroller" | relative_url}})
* [PwmSpeedController]({{ "/plugins/speed-controllers/pwmspeedcontroller" | relative_url}})
* [StaticSpeedController]({{ "/plugins/speed-controllers/staticspeedcontroller" | relative_url}})