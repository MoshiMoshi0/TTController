---
title: Ipc Server Plugin
permalink: /plugins/ipc-server
---

Ipc servers are used to exchange data with the service either localy or over network.

## Format
~~~
{
    "Type": <string>,
    "Config": <IpcServerConfig>
}
~~~

## Types

* [WebSocketIpcServer]({{ "/plugins/utils/websocketipcserver" | relative_url}})