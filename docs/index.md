---
title: Home
---

Welcome to the TTController documentation!

**Note:** This documentation might not apply to develop builds!
{: .notice--warning}

**Note:** This documentation is still Work-in-Progress, if you have problems with configuring the service please make a new [<i class="fab fa-github"/> issue](https://github.com/MoshiMoshi0/TTController/issues/new/choose).
{: .notice--info}

## Quick start

### Installing the service

1. Unpack all files to a desired directory
2. Run **TTController.Service.exe**
3. Select **Manage Service** from the menu and install the service

**Note:** The service will start automatically and will create a default empty config. See [Configuring the service](#configuring-the-service) section on how to configure it.
{: .notice--info}

**Note:** A quick test to see if the service is working property is to run debug report found in `Main Menu -> Debug -> Report` menu. It should list all detected controllers, sensors and plugins.

**Note:** The service is by default installed to run as **LocalSystem** account, to change this edit `service-install-as` in `TTController.Service.exe.Config` file.
{: .notice--info}

### Configuring the service

1. If the service is running, stop it using **Manage Service** menu
2. Edit **config.json** file
3. Start the service using **Manage Service** menu

**Note:** You can use `Main Menu -> Debug -> Controllers` menu to find your port identifiers, and `Main Menu -> Debug -> Sensors` menu to find your sensor identifiers to be used in **config.json**.
{: .notice--info}

### Uninstalling the service
1. Run **TTController.Service.exe**
2. Uninstall the service from **Manage Service** menu
3. Remove the folder containing service files

### Updating the service
1. If the service is running, stop it using **Manage Service** menu
2. Remove all files except **config.json**
3. Unpack newest release to the same directory
4. Start the service using **Manage Service** menu

## How to read the docs

This section contains some tips on the format of this documentation.

### Value types

| Type | Description
|------|------------
| `<bool>` | `true` or `false` value
| `<byte>` | Value from `0` to `255`
| `<float>`, `<double>` | Floating point value, e.g. `1.5`
| `<int>` | Integer value, e.g. `42`
| `<string>` | Text value, e.g. `"Top Fan"`
| `<enum>` | Text value with strict allowed values
| `<Class>` | Object value with variables, e.g.<br>`{ "Name": <string> }`<br> `Class` will be replaced with a specific type name but will always start with a captial letter
| `[<type>]` | List of values with specific type, e.g.<br>`[<int>]` = `[10, 20, 3, 45, 100, ...]`<br>`[<string>]` = `["Top", "Bottom", "Left", "Right", ...]` 

## Device support matrix

| Device                      | Support            | Confirmed          | Notes
|-----------------------------|--------------------|--------------------|-------------------------------------------
| Riing controller            | :heavy_check_mark: | :heavy_check_mark: |
| Riing Plus controller       | :heavy_check_mark: | :heavy_check_mark: |
| Riing Trio controller       | :heavy_check_mark: | :heavy_check_mark: |
| Dpsg controller             | :heavy_minus_sign: | :heavy_minus_sign: | not fully implemented
| Riing Quad controller       | :x:                |                    |
|                             |                    |                    |
| Riing Plus 12/14/20         | :heavy_check_mark: | :heavy_check_mark: |
| Riing Trio 12/14            | :heavy_check_mark: | :heavy_check_mark: |
| Pure Plus 12/14             | :heavy_check_mark: | :heavy_check_mark: |
| Floe Riing RGB 240/280/360  | :heavy_check_mark: | :heavy_check_mark: |
| Pacific PR22-D5 Plus        | :heavy_check_mark: | :heavy_check_mark: |
| Pacific W4                  | :heavy_check_mark: | :heavy_check_mark: |
| Razer Connect               | :heavy_check_mark: | :heavy_check_mark: |
| Riing Duo 12/14             | :heavy_check_mark: | :heavy_check_mark: |
| WaterRam RGB                | :heavy_check_mark: | :x:                |
| Pacific CL360/RL360         | :heavy_check_mark: | :x:                |
| Pacific V-GTX/V-RTX         | :heavy_check_mark: | :x:                |
| Pacific Lumi/Rad/R1/Fitting | :heavy_check_mark: | :x:                |
| Toughpower iRGB PLUS        | :heavy_minus_sign: | :heavy_minus_sign: | controller not fully implemented
| Pacific W5/W6               | :heavy_minus_sign: | :x:                | temperature sensor unsupported
| Riing Quad 12/14            | :x:                | :x:                |

## Menu tree

```
.
├── Manage Service
│   ├── Start
│   ├── Stop
│   ├── Restart
│   ├── Uninstall
│   ├── Install
│   └── Back
├── Run in console
├── Debug
│   ├── Report
│   ├── Controllers
│   ├── Sensors
│   ├── Plugins
│   └── Back
└── Exit
```