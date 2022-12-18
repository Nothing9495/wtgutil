#### This Readme.md is partially written with reference to Standard Readme, which is a very nice project!  
# wtgutil
[![standard-readme compliant](https://img.shields.io/badge/readme%20style-standard-brightgreen.svg?style=flat-square)](https://github.com/RichardLitt/standard-readme)  
**wtgutil** _(WinToGo Utility)_ is a utility that helps you adjust your WindowsToGo workstation settings more ealisy when you have specific needs like upgrade it to newer versions of Windows etc.
# Contents
- [Background](#background)
- [Install & Usage](#iau)
  - [Requirements](#requirements)
  - [Install](#install)
  - [Usage](#usage)
- [Principle](#principle)
- [Q&A](#ques)
- [Contributing](#contributing)
- [License](#license)
# Background
I have a WindowsToGo System installed in my portable hard drive, and I want upgrade it to Windows11. But I find it impossible to upgrade due to Microsoft's restriction — or you can remove this restriction manually by modifying the specific registry key, which is a bit troublesome. So, I came up with the idea of this utility, made it come true, and plan to share it with others.  
Note: This is my first time programming in C#, and this is my first-ever C# work. Not to mention that I have not received any systematic training, the source code might be a bit suck. If you have any suggestions on how to improve the quality of the code, please contact me or submit an issue or create Pull Requests.
# <span id="iau">Install & Usage</span>
## Requirements
- System: Windows 7 (partially), Windows 8 or newer.
- Achitecture: x86, amd64.
- Runtime: .NET Framework 4.7.2 or newer.
## Install
Download the latest build from Github Release, rename it *wtgutil.exe*, then put it into `<SystemDrive>:\Windows` directory. By doing so, you can use this utility in Windows Terminal or PowerShell etc. directly without extra steps.
## Usage
Run Windows Terminal or PowerShell as admin and type `wtgutil /?` or `wtgutil /help`. The program will show you it's usage.  
Note: The program needs to be run in Windows Terminal or PowerShell with admin privileges. If not, it will refuse to start.
# Principle
**wtgutil** realizes its functions by modifying the value of specific registry keys. The details are as follows.  
- Path: `HKLM\SYSTEM\HardwareConfig\Current`  
  - key: `BootDriverFlags`  
  - value kind: dword
  - value: `1` or `20`  
- Path: `HKLM\SYSTEM\CurrentControlSet\Control`  
  - key: `PortableOperatingSystem`  
  - value kind: dword
  - value: `1` or `0`   
- Path: `HKLM\SYSTEM\CurrentControlSet\Services\partmgr\Parameters`  
  - key: `SanPolicy`  
  - value kind: dword
  - value: `1` or `4`  
# <span id="ques">Q&A</span>
See [Q&A](Ques.md)
# Contributing
**It is welcomed to make contributions to this project by create Pull Requests or submit an issue.**
# License
This project follows the [GNU GPLv3](LICENSE)
