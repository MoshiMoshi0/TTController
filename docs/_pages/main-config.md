---
title: Main config
---

## Format

```
{
  "Profiles":  [<ProfileConfig>],
  "ComputerStateProfiles":  [<ComputerStateProfileConfig>],
  "PortConfigs":  [<PortConfig>],
  "SensorConfigs":   [<SensorConfig>],

  "SensorTimerInterval ": <int>,
  "DeviceSpeedTimerInterval": <int>,
  "DeviceRgbTimerInterval": <int>,
  "LoggingTimerInterval": <int>
}
```

## Variables

### Profiles
List of [ProfileConfig](/profile-config) objects

| Required | Default value | Usage 
|----------|---------------|--------
| **Yes** | <pre>[]</pre> | <pre>"Profiles": [<br>    {...},<br>    {...}<br>]</pre>

### ComputerStateProfiles
List of [ComputerStateProfileConfig](/computer-state-profile-config) objects

| Required | Default value | Usage 
|----------|---------------|--------
| No | <pre>[]</pre> | <pre>"ComputerStateProfiles": [<br>    {...},<br>    {...}<br>]</pre>

### PortConfigs 
List of [PortConfig](/port-config) objects

If [PortConfig](/port-config) is not configured for a port, a default [PortConfig](/port-config) will be used

| Required | Default value | Usage 
|----------|---------------|--------
| No | <pre>[]</pre> | <pre>"PortConfigs": [<br>    {...},<br>    {...}<br>]</pre>

### SensorConfigs
List of [SensorConfig](/sensor-config) objects

If [SensorConfig](/sensor-config) is not configured for a sensor, a default [SensorConfig](/sensor-config) will be used

| Required | Default value | Usage 
|----------|---------------|--------
| No | <pre>[]</pre> | <pre>"SensorConfigs": [<br>    {...},<br>    {...}<br>]</pre>

### SensorTimerInterval 
Determines timer delay for updating sensor values

Value in miliseconds

| Required | Default value | Usage 
|----------|---------------|--------
| No | 250 | <pre>"SensorTimerInterval": 500</pre>

### DeviceSpeedTimerInterval
Determines timer delay for updating speed of devices

Value in miliseconds

| Required | Default value | Usage 
|----------|---------------|--------
| No | 2500 | <pre>"DeviceSpeedTimerInterval": 3000</pre>

### DeviceRgbTimerInterval
Determines timer delay for updating led colors

Value in miliseconds

| Required | Default value | Usage 
|----------|---------------|--------
| No | 32 | <pre>"DeviceRgbTimerInterval": 16</pre>

### LoggingTimerInterval
Determines timer delay for logging port and sensor data

Used only in **console mode**
Value in miliseconds

| Required | Default value | Usage 
|----------|---------------|--------
| No | 5000 | <pre>"LoggingTimerInterval": 1000</pre>

## Example

```json
{
  "Profiles": [
    {
      "Name": "Default",
      "Guid": "10af9207-7e67-4581-9d13-506cad5d53c1",
      "Ports": [
        [9802, 8101, 1]
      ],
      "SpeedControllers": [
        {
          "Type": "PwmSpeedController", 
          "Config": {
            "CurvePoints": [
              [30, 30],
              [45, 50],
              [55, 60],
              [65, 75],
              [75, 100]
            ],
            "Sensors": ["/intelcpu/0/temperature/8"],
            "Trigger": {
              "Type": "AlwaysTrigger"
            }
          }
        }
      ],
      "Effects": [
        {
          "Type": "SensorEffect",
          "Config": {
            "Sensors": ["/intelcpu/0/temperature/8"],
            "ColorGradient": [
              [40, [16, 16, 128]],
              [60, [16, 16, 16]],
              [86, [128, 16, 16]]
            ],
            "Trigger": {
              "Type": "AlwaysTrigger"
            }
          }
        }
      ]
    }
  ],
  "ComputerStateProfiles": [
    {
      "StateType": "Shutdown",
      "Ports": [
        [9802, 8101, 1]
      ],
      "Speed": 35,
      "EffectType": "Full",
      "EffectColors": [
        [255, 0, 0]
      ]
    }
  ],
  "PortConfigs": [
    {
      "Ports": [[9802, 8101, 1]],
      "Config": {
        "Name": "Top Fan",
        "LedRotation": [11],
        "LedReverse": [false]
      }
    }
  ],
  "SensorConfigs": [
    {
      "Sensors": ["/intelcpu/0/temperature/8"],
      "Config": {
        "CriticalValue": 90
      }
    }
  ]
}
```