# PEDoll
*Win32/Win64 程序行为分析器*

[English](README.md) | 简体中文

## 概述
**PEDoll**是@matrixcascade的作品 [PeDoll](https://github.com/matrixcascade/PeDoll)（下文称“原版PeDoll”）的一个翻版，其设计目标是支持x64、多客户端以及多线程程序。

PEDoll由三部分组成：

- **主控端（Controller）** 接受客户端的连接请求，并提供命令行及图形界面供用户对客户端及钩子进行管理。
- **监控端（Monitor）** 运行在目标机器上（主控端所在的机器，或一台虚拟机），主要目的是通过启动新进程或附加到进程的方式，创建新受控端。
- **受控端（Doll）** 是指被注入了*受控端模块*（*libDoll*，`libDoll.dll`）的进程。受控端模块与主控端进行通信、安装内联钩子并当钩子被激活时相应主控端的命令。

这三部分通过基于TCP协议的*Puppet Protocol*相互通信。

PEDoll的工作原理是对受控端进程所使用的Windows API或内部过程安装钩子。待钩子被激活后，即可读取钩子执行的上下文（如函数参数、内存缓冲区等），并给出进程能否继续执行的判定。

您可以在[“Releases”页面](https://github.com/EZForever/PEDoll/releases)下载编译好的PEDoll软件包，也可以按照下文中的说明自行编译。

## 特性

与原版PeDoll相比：

- **完整的x64支持：** x64支持是PEDoll的设计初衷之一。在功能使用上，x64客户端进程与x86客户端几乎没有任何区别。
- **任意数量的API钩子/特征码钩子：** 挂钩机制的改变使得API钩子不再需要受控端模块支持。特征码钩子（原版PeDoll的“二进制钩子”）同理。
- **更强大的上下文读取能力：** PEDoll支持使用C#表达式读取钩子的上下文。Lambda表达式、LINQ扩展，只需要一个命令。
- **多客户端支持：** 给每一个受控端单独开一个主控端会很麻烦。

## 系统需求

- 主控端：Windows 7 SP1及以上版本，安装有.NET Framework 4.5（Windows 8 及以上版本自带）。
- 监控端及受控端：Windows Vista及以上版本。

## 使用方法

[项目wiki提供了一个简单的例子。][wiki/example]

有关命令、表达式等的更多信息，请参阅[项目wiki][wiki]。

## 开发

开发使用的IDE是Visual Studio 2019社区版，需要`使用C++的桌面开发`（用于开发监控端及受控端）和`.NET桌面开发`（用于开发主控端）两个工作负载。

这个项目使用了[Detours](https://github.com/microsoft/Detours)，并将其作为git submodule包含在项目中。在编译监控端及受控端之前必须获取这个库。可以在clone本项目时使用`git clone --recursive https://github.com/EZForever/PEDoll.git`命令，或者在之后运行`git submodule update --init --recursive`命令。

调试运行主控端时，部分功能可能无法正常工作，原因是缺少了如监控端程序或脚本等的关键文件。在Visual Studio命令提示符下运行`GenerateRelease.cmd`，可以自动编译整个解决方案、把编译生成的文件放在合适的地方，并生成用于支持x64的API脚本。日常使用PEDoll时，请使用`GenerateRelease.cmd`构建的软件包。

有关具体实现等的更多信息，请参阅[项目wiki][wiki]。

## 常见问题

参见[项目wiki上的FAQ页面][wiki/faq]。

## 开源许可证

[The MIT License.](LICENSE.txt)

[wiki]: https://github.com/EZForever/PEDoll/wiki/Home.zh-CN
[wiki/faq]: https://github.com/EZForever/PEDoll/wiki/FAQ.zh-CN
[wiki/example]: https://github.com/EZForever/PEDoll/wiki/Simple-Example.zh-CN

