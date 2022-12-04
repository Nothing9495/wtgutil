# wtgutil _(WinToGo Utility)_
**wtgutil** is a utility that helps you adjust your WindowsToGo workstation settings more ealisy when you have specific needs like upgrade to newer version of Windosw etc.
# Why build this?
I have a WindowsToGo System installed in my portable hard drive, and I want upgrade it to Windows11. But I find it possible to upgrade due to Microsoft's restriction — or you can remove this restriction manually by modifying the specific registry key, which is a bit troublesome. So, I came up with the idea of this utility, made it come true, and plan to share it with others.
This is my first time programming in C#, and this is my first-ever C# work, not to mention that I have not yet received any systematic training. So the program code might suck a little bit. If you have suggestions on how to improve the codes, please contact me or submit an issue. you can also try Pull Request (although I don't know how to use it yet).
# Install & How to use
## System Requirements
- System: Windows 8 or newer.
- Achitecture: x86, amd64
- Runtime: .NET Framework 4.7.2 or newer.
## Install
Download the latest build from Github Release, rename it *wtgutil.exe*, then put it into `<SystemDrive>\Windows` dictionary. By doing so, you can use this utility in Windows Terminal or PowerShell etc. directly without extra steps.
## How to use?
Open Windows Terminal(Admin) or PowerShell(Admin) and type `wtgutil /?` or `wtgutil /help`. The program will show you it's usage.The program needs to be run in a terminal running with administrator privileges. If not, the program will tell you that it "Need to elevate privileges to run wtgutil."
# How does it work?

# Q&A
- Q: Could I put it into `System32` folder?
  - A:
- Q: Dose it possible to support Windows 7
  - A:
- Q: I excuted `wtgutil /mode -default` and restarted my workstation
  - A:
