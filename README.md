# wtgutil _(WinToGo Utility)_
**wtgutil** is a utility that helps you adjust your WindowsToGo workstation settings more ealisy when you have specific needs like upgrade it to newer version of Windows etc.
# Contents
- [Background](#background)
- [Requirements](#requirements)
- [Install](#install)
- [Usage](#usage)
- [Principle](#principle)
- [Q&A](#questions)
# Background
I have a WindowsToGo System installed in my portable hard drive, and I want upgrade it to Windows11. But I find it impossible to upgrade due to Microsoft's restriction — or you can remove this restriction manually by modifying the specific registry key, which is a bit troublesome. So, I came up with the idea of this utility, made it come true, and plan to share it with others.  
Note: This is my first time programming in C#, and this is my first-ever C# work. Not to mention that I have not received any systematic training, the source code might be a bit suck. If you have any suggestions on how to improve the quality of the code, please contact me or submit an issue. You can also try Pull Request (although I don't know how to use it yet).
# Requirements
- System: Windows 8 or newer.
- Achitecture: x86, amd64
- Runtime: .NET Framework 4.7.2 or newer.
# Install
Download the latest build from Github Release, rename it *wtgutil.exe*, then put it into `<SystemDrive>\Windows` dictionary. By doing so, you can use this utility in Windows Terminal or PowerShell etc. directly without extra steps.
# Usage
Open Windows Terminal(Admin) or PowerShell(Admin) and type `wtgutil /?` or `wtgutil /help`. The program will show you it's usage.  
Note: The program needs to be run in Windows Terminal or PowerShell with administrator privileges. If not, it will tell you "Need to elevate privileges to run wtgutil."
# Principle
**wtgutil** realize its functions by modifying the value of specific registry keys. The details are as follows.  
Key 1: HKLM\SYSTEM\HardwareConfig\Current, key: BootDriverFlags, value: 1 or 20  
Key 2: HKLM\SYSTEM\CurrentControlSet\Control, key: PortableOperatingSystem, value: 1 or 0   
Key 3: HKLM\SYSTEM\CurrentControlSet\Services\partmgr\Parameters, key: SanPolicy, value: 1 or 4  
# Questions
- Q: Could I put it into `System32` folder?
  - A:
- Q: Dose it possible to support Windows 7?
  - A:
- Q: I excuted `wtgutil /mode -default` and restarted my workstation?j
  - A:
