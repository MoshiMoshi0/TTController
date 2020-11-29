---
title: Web Socket Ipc Server
permalink: /plugins/utils/websocketipcserver
---

A [Ipc Server]({{ "/plugins/ipc-server" | relative_url }}) using websockets for communicating between other processes.

## Format

~~~
{
    "Type": "WebSocketIpcServer",
    "Config": {
        "Address": <string>,
        "Port": <int>
    }
}
~~~

## Variables

### Address
<div class="variable-block" markdown="block">

Determines a IPv4 address to use for websocket. 

**Required:** No<br>
**Default value:**
~~~
"127.0.0.1"
~~~
**Example:**
~~~
"Address": "192.168.0.10"
~~~

</div>

### Port
<div class="variable-block" markdown="block">

Determines a port to use for websocket. 

**Required:** No<br>
**Default value:**
~~~
8888
~~~
**Example:**
~~~
"Port": 8080
~~~

</div>

## Example

~~~
~~~