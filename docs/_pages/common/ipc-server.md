---
title: Ipc Server Plugin
permalink: /common/ipc-server
---

Ipc servers are used to exchange data with the service either localy or over network.

## Format
~~~
{
  "Type": <string>,
  "Config": <IpcServerConfig>
}
~~~

## Examples

~~~ json
{
  "Type": "WebSocketIpcServer",
  "Config": {
    "Address": "127.0.0.1",
    "Port": 8888
  }
}
~~~