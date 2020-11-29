---
title: Trigger Plugin
permalink: /plugins/trigger
---

Triggers cause other plugins to be enabled or disabled dynamically.

## Format

~~~
{
    "Type": <string>,
    "Config": <TriggerConfig>
}
~~~

## Types

* [AlwaysTrigger]({{ "/plugins/triggers/alwaystrigger" | relative_url}})
* [IpcTrigger]({{ "/plugins/triggers/ipctrigger" | relative_url}})
* [OneTimeTrigger]({{ "/plugins/triggers/onetimetrigger" | relative_url}})
* [ProcessTrigger]({{ "/plugins/triggers/processtrigger" | relative_url}})
* [PulseTrigger]({{ "/plugins/triggers/pulsetrigger" | relative_url}})
* [LogicTrigger]({{ "/plugins/triggers/logictrigger" | relative_url}})
* [SensorTrigger]({{ "/plugins/triggers/sensortrigger" | relative_url}})
* [ScheduleTrigger]({{ "/plugins/triggers/scheduletrigger" | relative_url}})