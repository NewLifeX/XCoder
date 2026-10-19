# NewLife.XCoder - XCoder Developer Toolbox

![GitHub License](https://img.shields.io/github/license/NewLifeX/NewLife.XCoder?logo=github)
![NuGet Downloads](https://img.shields.io/nuget/dt/NewLife.XCoder?logo=nuget)
![NuGet](https://img.shields.io/nuget/v/NewLife.XCoder?logo=nuget)
![NuGet prerelease](https://img.shields.io/nuget/vpre/NewLife.XCoder?label=dev%20nuget&logo=nuget)
[![test](https://github.com/NewLifeX/NewLife.XCoder/actions/workflows/test.yml/badge.svg?branch=master)](https://github.com/NewLifeX/NewLife.XCoder/actions/workflows/test.yml)
[![locales](https://img.shields.io/badge/locales-11-2ea44f?logo=dotnet)](Doc/多语言本地化.md)
![platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-blue)
![arch](https://img.shields.io/badge/arch-x64%20%7C%20arm64-orange)
[![contributors](https://img.shields.io/github/contributors/NewLifeX/NewLife.XCoder?logo=github)](https://github.com/NewLifeX/NewLife.XCoder/graphs/contributors)

<!-- lang:start -->
🌐 [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [Español](README.es.md) | [Deutsch](README.de.md) | [日本語](README.ja.md) | [Français](README.fr.md) | [Português](README.pt.md) | [Русский](README.ru.md) | [한국어](README.ko.md) | [العربية](README.ar.md)
<!-- lang:end -->

NewLife XCoder is a developer toolbox covering code generation, network, API, serial port, regex, icon, encryption/decryption and map APIs.

Documentation: https://newlifex.com/core/coder

## NewLife Project Matrix

All projects support net9.0/netstandard2.1/netstandard2.0/net4.62/net4.5 by default; older versions (2024.0801) support net4.0/net2.0.

| Project | Year | Description |
| :------ | :--: | :---------- |
| Foundation components | | Support other middleware and product projects |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Core library: logging, configuration, caching, networking, serialization, APM performance tracing |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | Big data middleware, hundreds of billions of rows per table, MySQL/SQLite/SQLServer/Oracle/PostgreSQL/DaMeng, automatic sharding, read-write separation |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Networking library, tens of millions of TPS per server (22.66 million tps), millions of connections per server (4 million long-lived TCP connections) |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | Protocol communication library, provides a client-server application communication framework, supports HTTP/RPC frameworks, high throughput, low overhead and easy access for IoT devices |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Cube rapid development platform, integrates user permissions, SSO login, OAuth server, etc., validated in projects with 10 billion rows per table |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | Service management component that installs applications as OS daemons: Windows services and Linux systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero-generation scaffold: project templates NewLife.Templates based on the NewLife component ecosystem, for Web, WebApi and Service |
| Middleware | | Integrates with well-known middleware platforms |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redis client, microsecond latency, million-level throughput, rich message queues, validated in projects with tens of billions of records |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | Fully managed RocketMQ client, supports Apache RocketMQ and Alibaba Cloud Message Queue, validated in billion-level projects |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | IoT messaging protocol, MqttClient/MqttServer, the client supports Alibaba Cloud IoT |
| [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT) | 2022 | IoT standard library, defines various communication protocol specifications for the IoT domain |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | ModbusTcp/ModbusRTU/ModbusASCII, implemented on the IoT standard library, supports the ZeroIoT platform and IoTEdge gateways |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Siemens PLC protocol, implemented on the IoT standard library, supports the IoT platform and IoTEdge |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | Map component library wrapping Baidu Maps, Amap, Tencent Maps and Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | Audio codec library: PCM/ADPCMA/G711A/G722U/WAV/AAC |
| Product platforms | | Product-platform level, ready to compile and deploy, customizable |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Stardust distributed service platform: node management, APM monitoring center, configuration center, registry center and release center |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | AntJob distributed big data computing platform (real-time/offline), ant-moving sharding approach, validated in trillion-level data projects |
| [NewLife.ERP](https://github.com/NewLifeX/NewLife.ERP) | 2021 | Enterprise ERP: product, customer, sales and supplier management |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | 2006 | XCoder developer toolbox: network, serial port, encryption/decryption, regex, Modbus, MQTT and many other tools |
| [EasyIO](https://github.com/NewLifeX/EasyIO) | 2023 | Simple file storage, supports centralized file storage in distributed systems. |
| [XProxy](https://github.com/NewLifeX/XProxy) | 2005 | Product-grade reverse proxy: NAT proxy and HTTP proxy |
| [HttpMeter](https://github.com/NewLifeX/HttpMeter) | 2022 | HTTP stress testing tool |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | 2015 | Git source code management system |
| [SmartOS](https://github.com/NewLifeX/SmartOS) | 2014 | Embedded operating system, fully independent, supports the ARM Cortex-M chip architecture |
| [SmartA2](https://github.com/NewLifeX/SmartA2) | 2019 | Embedded industrial computer and IoT edge gateway, high-performance .NET 8 host, used in industry, agriculture, transportation and healthcare |
| FIoT IoT platform | 2020 | End-to-end IoT solution for construction, environmental protection and agriculture, integrating hardware, software and big data analytics, validated in projects with 100,000+ points per server |
| UWB high-precision indoor positioning | 2020 | Centimeter-level (10~20 cm) high-precision indoor positioning, integrated hardware and software, interoperates with other systems, validated in large exhibition hall projects |

## NewLife Development Team
![XCode](https://newlifex.com/logo.png)  

NewLife Team, founded in 2002, is an IoT industry solution provider for the new era, dedicated to software and hardware application consulting, system architecture planning and development services.  
More than 80 open-source projects led by the team have been widely used in various industries, with cumulative NuGet downloads exceeding 4 million.  
The team's big data middleware NewLife.XCode, AntJob distributed computing platform, Stardust distributed platform, NewLife.Redis cache and queue component, and FIoT IoT platform have all been successfully applied in power, universities, Internet, telecom, transportation, logistics, industrial control, healthcare, museums and other industries, providing customers with a large number of advanced, reliable, secure, high-quality and easily extensible products and system integration services.  

We will continuously improve our services, become a long-term trusted partner of our customers, and through continuous innovation and development, become an outstanding domestic IoT service provider.  

`NewLife Team started in 2002; some open-source projects have a history of more than 20 years, and the source code repository retains all modification records since 2010`  
Website: https://newlifex.com  
Open source: https://github.com/newlifex  
QQ groups: 1600800/1600838  
WeChat official account:  
![智能大石头](https://www.newlifex.com/stone.jpg)
