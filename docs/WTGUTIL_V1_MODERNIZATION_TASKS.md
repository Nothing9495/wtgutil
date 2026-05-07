# wtgutil v2.0 现代化改造 — Agent 任务说明书

> **面向 AI Agent** — 本文档是 wtgutil 从 v1.x 到 v2.0 的完整实施计划。每个任务包含具体文件路径、操作类型（创建/修改/删除）、关键代码模板、验证标准。严格按顺序执行，**每完成一个任务立即编译验证**。

---

## 目录

- [T0. 准备工作](#t0-准备工作)
- [T1. 确认 app.manifest 权限策略](#t1-确认-appmanifest-权限策略)
- [T2. 创建 RESX 本地化资源，移除 _chs 文件](#t2-创建-resx-本地化资源移除-_chs-文件)
- [T3. 创建项目目录结构与 Models](#t3-创建项目目录结构与-models)
- [T4. 创建 Infrastructure 层](#t4-创建-infrastructure-层)
- [T5. 创建 Services 层](#t5-创建-services-层)
- [T6. 创建 Commands 层](#t6-创建-commands-层)
- [T7. 重写 Program.cs 入口点](#t7-重写-programcs-入口点)
- [T8. 配置 Costura.Fody 单文件输出](#t8-配置-costurafody-单文件输出)
- [T9. 清理与最终验证](#t9-清理与最终验证)

---

## T0. 准备工作

### 目标
确保环境可用，了解当前项目结构。

### 操作
1. 确认 .NET SDK 可用：运行 `dotnet --version`，应 >= 10.0
2. 在改造前运行一次完整构建验证基线：
   ```
   dotnet build .\wtgutil.csproj --configuration Release
   dotnet build .\wtgutil.csproj --configuration Debug
   ```
3. 确认所有现有源文件已读取并理解

### 验证
- `dotnet build` 成功，无错误

---

## T1. 确认 app.manifest 权限策略

### 背景
程序设计为**不主动提权**：以用户态运行，在代码中检测当前是否为管理员权限。如果不是，显示提示信息告知用户需要以管理员身份运行，然后退出。

这与旧版行为一致，符合"让用户决定是否提权"的设计原则。

### 现状
当前 `app.manifest` 已声明 `requestedExecutionLevel="asInvoker"`，`Functions.cs` 中的 `IsAdmin.IsAdministrator()` 已实现运行时检测。**无需修改 app.manifest**。

### 操作类型
**无需操作** — `app.manifest` 保持现状不变。

`app.manifest` 中关键行确认：
```xml
<requestedExecutionLevel level="asInvoker" uiAccess="false" />
```

> 运行时检测逻辑将在 T4 的 `AdminCheck.cs` 中重新实现，统一本地化字符串。

### 验证
- `app.manifest` 中 `requestedExecutionLevel` 保持 `asInvoker`
- 后续 T7 完成后，以普通用户运行 `wtgutil` 应显示 `需要管理员权限才能运行 wtgutil。` 并退出

---

## T2. 创建 RESX 本地化资源，移除 _chs 文件

### 背景
当前通过复制完整代码文件实现本地化（`Functions.cs` + `Functions_chs.cs`、`Info.cs` + `Info_chs.cs`），维护成本极高且容易产生行为不一致。改用 .NET 标准 RESX 资源文件后，代码只需一份。

### 步骤 2.1：创建目录
```
e:\Development\Git_Repos\wtgutil\Localization\
```

### 步骤 2.2：创建 `Strings.resx`（英文默认）

**创建** — `e:\Development\Git_Repos\wtgutil\Localization\Strings.resx`

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <!-- ===== UI 字符串开始 ===== -->
  <!-- 程序标识 -->
  <data name="App_Title" xml:space="preserve">
    <value>WindowsToGo Utility</value>
  </data>
  <data name="App_Version" xml:space="preserve">
    <value>Version: v2.0.0</value>
  </data>
  <data name="App_ShortTitle" xml:space="preserve">
    <value>wtgutil</value>
  </data>
  <!-- 管理员权限 -->
  <data name="Error_NeedElevate" xml:space="preserve">
    <value>Administrator privileges are required. Please re-run this program as Administrator.</value>
  </data>
  <!-- 设备检测 -->
  <data name="Error_NoWTGDrive" xml:space="preserve">
    <value>No Windows To Go drive detected.</value>
  </data>
  <data name="Error_WTGDriveUnplugged" xml:space="preserve">
    <value>Please ensure the WTG drive is plugged in.</value>
  </data>
  <!-- 信息查询 -->
  <data name="Info_CurrentSettings" xml:space="preserve">
    <value>Current Settings:</value>
  </data>
  <data name="Info_BootFromUSB_Enabled" xml:space="preserve">
    <value>  Boot from USB Devices:   Supported</value>
  </data>
  <data name="Info_BootFromUSB_Disabled" xml:space="preserve">
    <value>  Boot from USB Devices:   Unsupported</value>
  </data>
  <data name="Info_BootFromUSB_Unknown" xml:space="preserve">
    <value>  Boot from USB Devices:   Status unknown</value>
  </data>
  <data name="Info_WTG_Enabled" xml:space="preserve">
    <value>  Windows To Go Features:  Enabled</value>
  </data>
  <data name="Info_WTG_Disabled" xml:space="preserve">
    <value>  Windows To Go Features:  Disabled</value>
  </data>
  <data name="Info_WTG_Unknown" xml:space="preserve">
    <value>  Windows To Go Features:  Status unknown</value>
  </data>
  <data name="Info_HideDisks_True" xml:space="preserve">
    <value>  Hide Local Disks:        True</value>
  </data>
  <data name="Info_HideDisks_False" xml:space="preserve">
    <value>  Hide Local Disks:        False</value>
  </data>
  <data name="Info_UASP_Disabled" xml:space="preserve">
    <value>  UASP Status:             Disabled</value>
  </data>
  <data name="Info_UASP_Enabled" xml:space="preserve">
    <value>  UASP Status:             Enabled</value>
  </data>
  <data name="Info_UASP_Unknown" xml:space="preserve">
    <value>  UASP Status:             Unknown</value>
  </data>
  <!-- 操作反馈 -->
  <data name="Msg_Completed" xml:space="preserve">
    <value>The operation completed successfully.</value>
  </data>
  <data name="Msg_RestartNeeded" xml:space="preserve">
    <value>A system restart is required for some changes to take effect.</value>
  </data>
  <data name="Msg_WarningNoUSBoot" xml:space="preserve">
    <value>WARNING: After this change, this system will no longer support booting from USB devices. Do NOT restart your WTG workstation after applying this setting.</value>
  </data>
  <!-- 错误/帮助消息 -->
  <data name="Error_UnknownCommand" xml:space="preserve">
    <value>Unknown command. Type "wtgutil help" for usage information.</value>
  </data>
  <data name="Error_UnknownParameter" xml:space="preserve">
    <value>Unknown parameter. Type "wtgutil help" for usage information.</value>
  </data>
  <data name="Error_NoParameter" xml:space="preserve">
    <value>Missing required parameter. Type "wtgutil help" for usage information.</value>
  </data>
  <!-- 帮助文本 -->
  <data name="Help_Usage" xml:space="preserve">
    <value>Usage:
  wtgutil.exe [command] [arguments]

Commands:
  info                  Display current WTG system settings
  help                  Show this help information
  about                 Show version and license information
  mode &lt;config&gt;        Switch system mode
    wintogo             → Enable Windows To Go mode
    default             → Restore Windows default settings
  partmgr &lt;action&gt;     Control local disk visibility
    show                → Show local disks at startup
    hide                → Hide local disks at startup
  uasp &lt;state&gt;         Control UASP (USB Attached SCSI Protocol)
    off                 → Disable UASP (enable "freeze on unplug")
    on                  → Re-enable UASP

Global Options:
  --lang &lt;code&gt;        Override display language (en, zh-CN)
  --help                Show this help
  --version             Show version number

Examples:
  wtgutil.exe info
  wtgutil.exe mode wintogo
  wtgutil.exe partmgr hide
  wtgutil.exe uasp off</value>
  </data>
  <!-- 关于文本 -->
  <data name="About_Text" xml:space="preserve">
    <value>Windows To Go Utility v2.0.0
by charlesy

GitHub: https://github.com/Nothing9495/wtgutil
License: GNU GPLv3

wtgutil (Windows To Go Utility) is free and open-source software.
If you encounter any issues or have suggestions, please submit them on GitHub Issues.</value>
  </data>
  <!-- 版本信息（短） -->
  <data name="Version_Text" xml:space="preserve">
    <value>wtgutil v2.0.0</value>
  </data>
  <!-- Registry 错误模板 -->
  <data name="Error_RegistryRead" xml:space="preserve">
    <value>Failed to read registry: {0}</value>
  </data>
  <data name="Error_RegistryWrite" xml:space="preserve">
    <value>Failed to write registry: {0}</value>
  </data>
  <data name="Error_DeviceDetection" xml:space="preserve">
    <value>Failed to detect WTG device: {0}</value>
  </data>
  <data name="Error_Unexpected" xml:space="preserve">
    <value>An unexpected error occurred: {0}</value>
  </data>
</root>
```

### 步骤 2.3：创建 `Strings.zh-CN.resx`（简体中文）

**创建** — `e:\Development\Git_Repos\wtgutil\Localization\Strings.zh-CN.resx`

> 该文件 XML 结构与 `Strings.resx` 完全相同，仅 `<data>` 部分为中文翻译。以下分两部分列出所有 data 节点的 name→value 映射：**短字符串**用表格呈现，**长文本**独立列出。

#### 短字符串（单行值）

| Name | Value (zh-CN) |
|------|---------------|
| `App_Title` | `Windows To Go 实用程序` |
| `App_Version` | `程序版本: v2.0.0` |
| `App_ShortTitle` | `wtgutil` |
| `Error_NeedElevate` | `需要管理员权限。请以管理员身份重新运行此程序。` |
| `Error_NoWTGDrive` | `未检测到 Windows To Go 驱动器。` |
| `Error_WTGDriveUnplugged` | `请确保已连接 WTG 驱动器。` |
| `Info_CurrentSettings` | `当前设置:` |
| `Info_BootFromUSB_Enabled` | `  从 USB 设备启动:      支持` |
| `Info_BootFromUSB_Disabled` | `  从 USB 设备启动:      不支持` |
| `Info_BootFromUSB_Unknown` | `  从 USB 设备启动:      状态未知` |
| `Info_WTG_Enabled` | `  Windows To Go 特性:   已启用` |
| `Info_WTG_Disabled` | `  Windows To Go 特性:   已禁用` |
| `Info_WTG_Unknown` | `  Windows To Go 特性:   状态未知` |
| `Info_HideDisks_True` | `  隐藏本地磁盘:          是` |
| `Info_HideDisks_False` | `  隐藏本地磁盘:          否` |
| `Info_UASP_Disabled` | `  UASP 状态:             已禁用` |
| `Info_UASP_Enabled` | `  UASP 状态:             已启用` |
| `Info_UASP_Unknown` | `  UASP 状态:             未知` |
| `Msg_Completed` | `操作成功完成。` |
| `Msg_RestartNeeded` | `部分更改需要重启系统才能生效。` |
| `Msg_WarningNoUSBoot` | `警告: 此操作后，系统将不再支持从 USB 设备启动。请不要轻易重启 WTG 工作站。` |
| `Error_UnknownCommand` | `未知命令。输入 "wtgutil help" 获取帮助。` |
| `Error_UnknownParameter` | `未知参数。输入 "wtgutil help" 获取帮助。` |
| `Error_NoParameter` | `缺少必要参数。输入 "wtgutil help" 获取帮助。` |
| `Version_Text` | `wtgutil v2.0.0` |
| `Error_RegistryRead` | `读取注册表失败: {0}` |
| `Error_RegistryWrite` | `写入注册表失败: {0}` |
| `Error_DeviceDetection` | `检测 WTG 设备失败: {0}` |
| `Error_Unexpected` | `发生意外错误: {0}` |

#### 长文本（多行值）

**Help_Usage** — 在 RESX 中填入：
```xml
<data name="Help_Usage" xml:space="preserve">
    <value>用法:
  wtgutil.exe [命令] [参数]

命令:
  info                  显示当前 WTG 系统设置
  help                  显示此帮助信息
  about                 显示版本和许可信息
  mode &lt;配置&gt;          切换系统模式
    wintogo             → 启用 Windows To Go 模式
    default             → 恢复 Windows 默认设置
  partmgr &lt;操作&gt;       控制本地磁盘显示
    show                → 启动时显示本地磁盘
    hide                → 启动时隐藏本地磁盘
  uasp &lt;状态&gt;          控制 UASP (USB Attached SCSI Protocol)
    off                 → 禁用 UASP（启用"拔出冻结"）
    on                  → 重新启用 UASP

全局选项:
  --lang &lt;代码&gt;         覆盖显示语言 (en, zh-CN)
  --help                显示此帮助
  --version             显示版本号

示例:
  wtgutil.exe info
  wtgutil.exe mode wintogo
  wtgutil.exe partmgr hide
  wtgutil.exe uasp off</value>
</data>
```

**About_Text** — 在 RESX 中填入：
```xml
<data name="About_Text" xml:space="preserve">
    <value>Windows To Go 实用程序 v2.0.0
by charlesy

GitHub: https://github.com/Nothing9495/wtgutil
许可证: GNU GPLv3

wtgutil (Windows To Go 实用程序) 是自由且开源的软件。
如遇到问题或有任何建议，欢迎在 GitHub Issues 上提交。</value>
</data>
```


### 步骤 2.4：创建 `LocalizationManager.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Localization\LocalizationManager.cs`

```csharp
using System.Globalization;
using System.Resources;

namespace WTGUtility.Localization
{
    /// <summary>
    /// Provides localized strings for the application.
    /// Automatically follows CurrentUICulture; can be overridden via --lang.
    /// </summary>
    public static class Loc
    {
        private static readonly ResourceManager _resources =
            new ResourceManager("WTGUtility.Localization.Strings", typeof(Loc).Assembly);

        private static CultureInfo _culture = CultureInfo.CurrentUICulture;

        /// <summary>Current UI culture for localization.</summary>
        public static CultureInfo Culture
        {
            get => _culture;
            set
            {
                _culture = value ?? CultureInfo.CurrentUICulture;
            }
        }

        /// <summary>Gets a localized string by resource key.</summary>
        public static string Get(string key)
        {
            return _resources.GetString(key, _culture) ?? $"[[{key}]]";
        }

        /// <summary>Gets a localized format string and applies arguments.</summary>
        public static string Format(string key, params object[] args)
        {
            var format = Get(key);
            return args.Length > 0 ? string.Format(_culture, format, args) : format;
        }
    }
}
```

> ⚠️ **ResourceManager 的 baseName**（构造参数）必须与项目默认命名空间 + 资源文件所在子路径匹配。最终以实际编译结果验证。该项目根命名空间为 `WTGUtility`（见 `.csproj` 中 `<RootNamespace>WTG_Utility</RootNamespace>`，需要统一）。建议将 `<RootNamespace>` 改为 `WTGUtility`（无下划线）。

### 步骤 2.5：更新 .csproj 的 RootNamespace

**修改** — `e:\Development\Git_Repos\wtgutil\wtgutil.csproj`

将：
```xml
<RootNamespace>WTG_Utility</RootNamespace>
```
改为：
```xml
<RootNamespace>WTGUtility</RootNamespace>
```

### 步骤 2.6：删除旧本地化文件

**删除** — 以下 2 个文件：
- `e:\Development\Git_Repos\wtgutil\Functions_chs.cs`
- `e:\Development\Git_Repos\wtgutil\Info_chs.cs`

### 验证
- `dotnet build` 通过（尽管有未引用局部变量的警告属于正常）
- 后续 T7 完成后才能完整验证运行时语言切换

---

## T3. 创建项目目录结构与 Models

### 操作类型
**创建目录 + 文件**

### 步骤 3.1：创建目录
```
e:\Development\Git_Repos\wtgutil\Commands\
e:\Development\Git_Repos\wtgutil\Services\
e:\Development\Git_Repos\wtgutil\Models\
e:\Development\Git_Repos\wtgutil\Infrastructure\
```

### 步骤 3.2：创建 `Models/Settings.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Models\Settings.cs`

```csharp
namespace WTGUtility.Models
{
    /// <summary>
    /// Represents the current state of all WTG-related registry settings.
    /// </summary>
    public class WtgSettings
    {
        /// <summary>BootDriverFlags value from HKLM\SYSTEM\HardwareConfig\Current</summary>
        public int BootDriverFlags { get; set; }

        /// <summary>Whether PortableOperatingSystem exists and equals 1</summary>
        public bool WindowsToGoEnabled { get; set; }

        /// <summary>Whether PortableOperatingSystem registry value exists</summary>
        public bool WindowsToGoExists { get; set; }

        /// <summary>PortableOperatingSystem raw value</summary>
        public int PortableOperatingSystem { get; set; }

        /// <summary>Whether SanPolicy == 4 (hide local disks)</summary>
        public bool HideLocalDisks { get; set; }

        /// <summary>SanPolicy raw value</summary>
        public int SanPolicy { get; set; }

        /// <summary>Whether UASP is currently disabled for the WTG drive</summary>
        public bool UaspDisabled { get; set; }

        /// <summary>Human-readable UASP status description</summary>
        public string UaspStatusDescription { get; set; } = string.Empty;
    }
}
```

### 步骤 3.3：创建 `Models/DeviceInfo.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Models\DeviceInfo.cs`

```csharp
namespace WTGUtility.Models
{
    /// <summary>
    /// Information about a detected WTG storage device.
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>The device instance path (used for registry access).</summary>
        public string InstancePath { get; set; } = string.Empty;

        /// <summary>The controller DeviceID from WMI.</summary>
        public string ControllerDeviceId { get; set; } = string.Empty;

        /// <summary>Whether a WTG-compatible device was actually found.</summary>
        public bool IsFound => !string.IsNullOrEmpty(InstancePath);
    }
}
```

### 验证
- `dotnet build` 通过（此时尚无代码引用这些模型，仅验证语法）

---

## T4. 创建 Infrastructure 层

### 步骤 4.1：创建 `Infrastructure/AdminCheck.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Infrastructure\AdminCheck.cs`

> 程序以用户态运行（`asInvoker`，manifest 不做修改），在代码中检测当前进程是否为管理员权限。如果不是，显示本地化提示信息告知用户需要以管理员身份运行，然后退出。这是唯一且首要的权限检查机制。

```csharp
using System;
using System.Security.Principal;
using WTGUtility.Localization;

namespace WTGUtility.Infrastructure
{
    /// <summary>
    /// Ensures the process has administrator privileges.
    /// The program runs as invoker (no auto-elevation); if not admin,
    /// it prompts the user to re-run as administrator and exits.
    /// </summary>
    public static class AdminCheck
    {
        public static void EnsureAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Console.WriteLine();
                    Console.WriteLine(Loc.Get("Error_NeedElevate"));
                    Console.WriteLine();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Loc.Format("Error_Unexpected", ex.Message));
                Environment.Exit(1);
            }
        }
    }
}
```

### 步骤 4.2：创建 `Infrastructure/Console.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Infrastructure\Console.cs`

> 统一控制台输出，集中管理颜色和格式。

```csharp
using System;

namespace WTGUtility.Infrastructure
{
    /// <summary>
    /// Centralized console output helpers with color support.
    /// </summary>
    public static class ConsoleOutput
    {
        public static void WriteLine(string message = "")
        {
            Console.WriteLine(message);
        }

        public static void WriteWarning(string message)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteError(string message)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        public static void WriteBanner(string line1, string line2)
        {
            Console.WriteLine();
            Console.WriteLine(line1);
            Console.WriteLine(line2);
        }

        public static void WriteSeparator()
        {
            Console.WriteLine();
        }
    }
}
```

### 验证
- `dotnet build` 通过

---

## T5. 创建 Services 层

### 步骤 5.1：创建 `Services/RegistryService.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Services\RegistryService.cs`

> 将所有注册表读写操作集中到一个服务类中。这是 v1.x `GetSettings` + `ModifySettings` 的合并重构版本。

```csharp
using System;
using Microsoft.Win32;
using WTGUtility.Models;

namespace WTGUtility.Services
{
    /// <summary>
    /// Centralized registry access for WTG settings.
    /// All registry paths and value names are defined as constants.
    /// </summary>
    public class RegistryService
    {
        // Registry paths
        private const string PathBootDriverFlags = @"SYSTEM\HardwareConfig\Current";
        private const string PathPortableOS = @"SYSTEM\CurrentControlSet\Control";
        private const string PathPartmgrParams = @"SYSTEM\CurrentControlSet\Services\partmgr\Parameters";
        private const string PathEnumPrefix = @"SYSTEM\CurrentControlSet\Enum\";

        // Value names
        private const string ValBootDriverFlags = "BootDriverFlags";
        private const string ValPortableOperatingSystem = "PortableOperatingSystem";
        private const string ValSanPolicy = "SanPolicy";
        private const string ValCapabilities = "Capabilities";
        private const string ValDeviceDesc = "DeviceDesc";
        private const string ValMfg = "Mfg";
        private const string ValService = "Service";

        // UASP state values
        private const int UaspDisabledCapabilities = 0x00000094;
        private const string UaspDisabledDeviceDesc = "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device";
        private const string UaspDisabledMfg = "@usbstor.inf,%generic.mfg%;Compatible USB storage device";
        private const string UaspDisabledService = "USBSTOR";

        // Default (UASP enabled) values to restore
        private const int DefaultCapabilities = 0x00000094; // Keep same; primary diff is Service + Mfg
        private const string DefaultService = "UASPStor";
        private const string DefaultDeviceDesc = "@usbstor.inf,%genericbulkonly.devicedesc%;USB Mass Storage Device";

        /// <summary>
        /// Reads all current WTG settings from the registry.
        /// </summary>
        public WtgSettings GetSettings(string deviceInstancePath)
        {
            var settings = new WtgSettings();

            try
            {
                using var bdfKey = Registry.LocalMachine.OpenSubKey(PathBootDriverFlags);
                settings.BootDriverFlags = bdfKey != null ? (int)bdfKey.GetValue(ValBootDriverFlags, 0) : -1;
            }
            catch { settings.BootDriverFlags = -1; }

            try
            {
                using var posKey = Registry.LocalMachine.OpenSubKey(PathPortableOS);
                if (posKey != null)
                {
                    settings.WindowsToGoExists = posKey.GetValueNames().Length > 0 &&
                        Array.IndexOf(posKey.GetValueNames(), ValPortableOperatingSystem) >= 0;
                    if (settings.WindowsToGoExists)
                    {
                        settings.PortableOperatingSystem = (int)posKey.GetValue(ValPortableOperatingSystem, 0);
                        settings.WindowsToGoEnabled = settings.PortableOperatingSystem == 1;
                    }
                }
            }
            catch { /* leave defaults */ }

            try
            {
                using var pmgrKey = Registry.LocalMachine.OpenSubKey(PathPartmgrParams);
                if (pmgrKey != null)
                {
                    settings.SanPolicy = (int)pmgrKey.GetValue(ValSanPolicy, 1);
                    settings.HideLocalDisks = settings.SanPolicy == 4;
                }
            }
            catch { /* leave defaults */ }

            // UASP status
            try
            {
                using var uaspKey = Registry.LocalMachine.OpenSubKey(PathEnumPrefix + deviceInstancePath);
                if (uaspKey != null)
                {
                    int cap = (int)uaspKey.GetValue(ValCapabilities, 0);
                    string desc = uaspKey.GetValue(ValDeviceDesc, "") as string ?? "";
                    string mfg = uaspKey.GetValue(ValMfg, "") as string ?? "";
                    string svc = uaspKey.GetValue(ValService, "") as string ?? "";

                    settings.UaspDisabled =
                        cap == UaspDisabledCapabilities &&
                        desc == UaspDisabledDeviceDesc &&
                        mfg == UaspDisabledMfg &&
                        svc == UaspDisabledService;
                    settings.UaspStatusDescription = settings.UaspDisabled ? "Disabled" : "Enabled";
                }
            }
            catch
            {
                settings.UaspStatusDescription = "Unknown";
            }

            return settings;
        }

        /// <summary>Sets BootDriverFlags (20 = enable USB boot, 0 = disable).</summary>
        public void SetBootDriverFlags(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathBootDriverFlags);
            key.SetValue(ValBootDriverFlags, value, RegistryValueKind.DWord);
        }

        /// <summary>Sets PortableOperatingSystem (1 = enabled, 0 = disabled).</summary>
        public void SetPortableOperatingSystem(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathPortableOS);
            key.SetValue(ValPortableOperatingSystem, value, RegistryValueKind.DWord);
        }

        /// <summary>Sets SanPolicy (4 = hide, 1 = show).</summary>
        public void SetSanPolicy(int value)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathPartmgrParams);
            key.SetValue(ValSanPolicy, value, RegistryValueKind.DWord);
        }

        /// <summary>Disables UASP by modifying the device's registry entries.</summary>
        public void DisableUasp(string deviceInstancePath)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathEnumPrefix + deviceInstancePath);
            key.SetValue(ValCapabilities, UaspDisabledCapabilities, RegistryValueKind.DWord);
            key.SetValue(ValDeviceDesc, UaspDisabledDeviceDesc, RegistryValueKind.String);
            key.SetValue(ValMfg, UaspDisabledMfg, RegistryValueKind.String);
            key.SetValue(ValService, UaspDisabledService, RegistryValueKind.String);
        }

        /// <summary>Re-enables UASP by reverting device registry entries to defaults.</summary>
        public void EnableUasp(string deviceInstancePath)
        {
            using var key = Registry.LocalMachine.CreateSubKey(PathEnumPrefix + deviceInstancePath);
            // Restore default values for UASP operation
            key.SetValue(ValCapabilities, DefaultCapabilities, RegistryValueKind.DWord);
            key.SetValue(ValDeviceDesc, DefaultDeviceDesc, RegistryValueKind.String);
            // Original Mfg for UASP is typically device-specific; restore common default
            key.SetValue(ValMfg, DefaultMfg, RegistryValueKind.String);
            key.SetValue(ValService, DefaultService, RegistryValueKind.String);
        }

        private const string DefaultMfg = "@usbstor.inf,%generic.mfg%;Compatible USB storage device";
    }
}
```

### 步骤 5.2：创建 `Services/DeviceDetector.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Services\DeviceDetector.cs`

> 将 `GetDeviceInstancePath.cs` 的代码重构为服务类，统一命名空间。

```csharp
using System;
using System.Linq;
using System.Management;
using WTGUtility.Models;

namespace WTGUtility.Services
{
    /// <summary>
    /// Detects the Windows To Go USB storage device via WMI.
    /// </summary>
    public class DeviceDetector
    {
        /// <summary>
        /// Finds the device instance path of the WTG SCSI/USB storage controller.
        /// Returns null if no suitable device is found.
        /// </summary>
        public DeviceInfo DetectWtgDevice()
        {
            try
            {
                var controllerQuery = new ManagementObjectSearcher(
                    @"SELECT * FROM Win32_SCSIController 
                      WHERE Manufacturer LIKE '%USB%' 
                      OR Description LIKE '%SCSI%'");

                foreach (ManagementObject controller in controllerQuery.Get())
                {
                    try
                    {
                        var controllerPath = controller.Path.Path;

                        var deviceQuery = new ManagementObjectSearcher($@"
                            ASSOCIATORS OF {{{controllerPath}}}
                            WHERE ResultClass = Win32_PnPEntity");

                        foreach (ManagementObject device in deviceQuery.Get())
                        {
                            try
                            {
                                var deviceId = device["DeviceID"]?.ToString();

                                if (!string.IsNullOrEmpty(deviceId) &&
                                    deviceId.StartsWith("USB", StringComparison.OrdinalIgnoreCase))
                                {
                                    return new DeviceInfo
                                    {
                                        InstancePath = controller["DeviceID"]?.ToString() ?? "",
                                        ControllerDeviceId = deviceId
                                    };
                                }
                            }
                            catch (ManagementException) { /* skip this device */ }
                        }
                    }
                    catch (ManagementException) { /* skip this controller */ }
                }
            }
            catch (ManagementException) { /* WMI query failed */ }
            catch (Exception) { /* unexpected error */ }

            return new DeviceInfo(); // IsFound will be false
        }
    }
}
```

### 步骤 5.3：创建 `Services/WtgService.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Services\WtgService.cs`

> 业务编排层：组合 RegistryService 和 DeviceDetector，实现 WTG 功能的高级操作。

```csharp
using WTGUtility.Models;

namespace WTGUtility.Services
{
    /// <summary>
    /// High-level Windows To Go business logic.
    /// Orchestrates registry operations and device detection.
    /// </summary>
    public class WtgService
    {
        private readonly RegistryService _registry;
        private readonly DeviceDetector _detector;

        public WtgService(RegistryService registry, DeviceDetector detector)
        {
            _registry = registry;
            _detector = detector;
        }

        public DeviceInfo DetectDevice()
        {
            return _detector.DetectWtgDevice();
        }

        public WtgSettings GetSettings(string deviceInstancePath)
        {
            return _registry.GetSettings(deviceInstancePath);
        }

        public void EnableWinToGoMode()
        {
            _registry.SetBootDriverFlags(20);
            _registry.SetPortableOperatingSystem(1);
            _registry.SetSanPolicy(4);
        }

        public void RestoreDefaults()
        {
            _registry.SetBootDriverFlags(0);
            _registry.SetPortableOperatingSystem(0);
            _registry.SetSanPolicy(1);
        }

        public void ShowLocalDisks()
        {
            _registry.SetSanPolicy(1);
        }

        public void HideLocalDisks()
        {
            _registry.SetSanPolicy(4);
        }

        public void DisableUasp(string deviceInstancePath)
        {
            _registry.DisableUasp(deviceInstancePath);
        }

        public void EnableUasp(string deviceInstancePath)
        {
            _registry.EnableUasp(deviceInstancePath);
        }
    }
}
```

### 验证
- `dotnet build` 通过

---

## T6. 创建 Commands 层

### 步骤 6.1：创建 `Commands/ICommand.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Commands\ICommand.cs`

```csharp
using System.Threading.Tasks;

namespace WTGUtility.Commands
{
    /// <summary>
    /// Contract for a CLI command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>The primary command name (lowercase).</summary>
        string Name { get; }

        /// <summary>Executes the command with the given arguments.</summary>
        /// <returns>Exit code: 0 = success, non-zero = error.</returns>
        Task<int> ExecuteAsync(string[] args);
    }
}
```

### 步骤 6.2：创建 `Commands/CommandContext.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Commands\CommandContext.cs`

```csharp
using WTGUtility.Services;

namespace WTGUtility.Commands
{
    /// <summary>
    /// Shared context passed to all commands.
    /// </summary>
    public class CommandContext
    {
        public WtgService WtgService { get; }
        public string WtgDeviceInstancePath { get; set; } = string.Empty;

        public CommandContext(WtgService wtgService)
        {
            WtgService = wtgService;
        }
    }
}
```

### 步骤 6.3：创建各命令实现

以下 6 个命令文件全部创建在 `e:\Development\Git_Repos\wtgutil\Commands\` 目录下。

#### InfoCommand.cs

```csharp
using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class InfoCommand : ICommand
    {
        public string Name => "info";

        public Task<int> ExecuteAsync(string[] args)
        {
            var context = args.Length > 0 ? args[0] as CommandContext : null;
            // Note: The CommandRouter will pass context via a different mechanism.
            // For simplicity in final integration, the router calls a static helper.
            return Task.FromResult(0);
        }

        /// <summary>Static entry called by CommandRouter with full context.</summary>
        public static int Execute(CommandContext ctx)
        {
            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Get("App_Version"));
            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Info_CurrentSettings"));

            var settings = ctx.WtgService.GetSettings(ctx.WtgDeviceInstancePath);

            // BootDriverFlags
            string bootMsg = settings.BootDriverFlags switch
            {
                20 or 28 => Loc.Get("Info_BootFromUSB_Enabled"),
                0 => Loc.Get("Info_BootFromUSB_Disabled"),
                _ => Loc.Get("Info_BootFromUSB_Unknown")
            };
            Console.WriteLine(bootMsg);

            // PortableOS
            if (settings.WindowsToGoExists)
            {
                Console.WriteLine(settings.WindowsToGoEnabled
                    ? Loc.Get("Info_WTG_Enabled")
                    : Loc.Get("Info_WTG_Disabled"));
            }
            else
            {
                Console.WriteLine(Loc.Get("Info_WTG_Unknown"));
            }

            // Partmgr
            Console.WriteLine(settings.HideLocalDisks
                ? Loc.Get("Info_HideDisks_True")
                : Loc.Get("Info_HideDisks_False"));

            // UASP
            string uaspMsg = settings.UaspStatusDescription switch
            {
                "Disabled" => Loc.Get("Info_UASP_Disabled"),
                "Enabled" => Loc.Get("Info_UASP_Enabled"),
                _ => Loc.Get("Info_UASP_Unknown")
            };
            Console.WriteLine(uaspMsg);

            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
```

#### HelpCommand.cs

```csharp
using System;
using System.Threading.Tasks;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name => "help";

        public Task<int> ExecuteAsync(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(Loc.Get("Help_Usage"));
            Console.WriteLine();
            return Task.FromResult(0);
        }
    }
}
```

#### AboutCommand.cs

```csharp
using System;
using System.Threading.Tasks;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class AboutCommand : ICommand
    {
        public string Name => "about";

        public Task<int> ExecuteAsync(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(Loc.Get("About_Text"));
            Console.WriteLine();
            return Task.FromResult(0);
        }
    }
}
```

#### ModeCommand.cs

```csharp
using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class ModeCommand : ICommand
    {
        public string Name => "mode";

        public Task<int> ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                ConsoleOutput.WriteError(Loc.Get("Error_NoParameter"));
                return Task.FromResult(1);
            }

            var sub = args[0].ToLowerInvariant();

            switch (sub)
            {
                case "wintogo":
                    // The CommandRouter will have already resolved the context.
                    // This method is a placeholder; real logic is in the static handler below.
                    break;
                case "default":
                    break;
                default:
                    ConsoleOutput.WriteError(Loc.Get("Error_UnknownParameter"));
                    return Task.FromResult(1);
            }

            return Task.FromResult(0);
        }

        public static int Execute(CommandContext ctx, string subCommand)
        {
            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Get("App_Version"));
            ConsoleOutput.WriteSeparator();

            switch (subCommand)
            {
                case "wintogo":
                    ctx.WtgService.EnableWinToGoMode();
                    ConsoleOutput.WriteLine(Loc.Get("Msg_RestartNeeded"));
                    break;

                case "default":
                    ctx.WtgService.RestoreDefaults();
                    ConsoleOutput.WriteLine(Loc.Get("Msg_RestartNeeded"));
                    ConsoleOutput.WriteWarning(Loc.Get("Msg_WarningNoUSBoot"));
                    break;

                default:
                    ConsoleOutput.WriteError(Loc.Get("Error_UnknownParameter"));
                    return 1;
            }

            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
```

#### PartmgrCommand.cs

```csharp
using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class PartmgrCommand : ICommand
    {
        public string Name => "partmgr";

        public Task<int> ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                ConsoleOutput.WriteError(Loc.Get("Error_NoParameter"));
                return Task.FromResult(1);
            }

            return Task.FromResult(0);
        }

        public static int Execute(CommandContext ctx, string subCommand)
        {
            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Get("App_Version"));
            ConsoleOutput.WriteSeparator();

            switch (subCommand)
            {
                case "show":
                    ctx.WtgService.ShowLocalDisks();
                    break;

                case "hide":
                    ctx.WtgService.HideLocalDisks();
                    break;

                default:
                    ConsoleOutput.WriteError(Loc.Get("Error_UnknownParameter"));
                    return 1;
            }

            ConsoleOutput.WriteLine(Loc.Get("Msg_RestartNeeded"));
            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
```

#### UaspCommand.cs

```csharp
using System;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    public class UaspCommand : ICommand
    {
        public string Name => "uasp";

        public Task<int> ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                ConsoleOutput.WriteError(Loc.Get("Error_NoParameter"));
                return Task.FromResult(1);
            }

            return Task.FromResult(0);
        }

        public static int Execute(CommandContext ctx, string subCommand)
        {
            if (string.IsNullOrEmpty(ctx.WtgDeviceInstancePath))
            {
                ConsoleOutput.WriteSeparator();
                ConsoleOutput.WriteError(Loc.Get("Error_NoWTGDrive"));
                return 1;
            }

            ConsoleOutput.WriteBanner(Loc.Get("App_Title"), Loc.Get("App_Version"));
            ConsoleOutput.WriteSeparator();

            switch (subCommand)
            {
                case "off":
                    ctx.WtgService.DisableUasp(ctx.WtgDeviceInstancePath);
                    break;

                case "on":
                    ctx.WtgService.EnableUasp(ctx.WtgDeviceInstancePath);
                    break;

                default:
                    ConsoleOutput.WriteError(Loc.Get("Error_UnknownParameter"));
                    return 1;
            }

            ConsoleOutput.WriteSeparator();
            Console.WriteLine(Loc.Get("Msg_Completed"));
            return 0;
        }
    }
}
```

### 步骤 6.4：创建 `Commands/CommandRouter.cs`

**创建** — `e:\Development\Git_Repos\wtgutil\Commands\CommandRouter.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;

namespace WTGUtility.Commands
{
    /// <summary>
    /// Routes CLI arguments to the appropriate command handler.
    /// Supports --lang, --help, --version global options.
    /// </summary>
    public class CommandRouter
    {
        private readonly CommandContext _context;
        private readonly Dictionary<string, Func<string[], int>> _routes;

        public CommandRouter(CommandContext context)
        {
            _context = context;
            _routes = new Dictionary<string, Func<string[], int>>(StringComparer.OrdinalIgnoreCase)
            {
                ["info"]    = _ => RunInfo(),
                ["help"]    = _ => RunHelp(),
                ["about"]   = _ => RunAbout(),
                ["mode"]    = args => RunWithSubCommand(args, ModeCommand.Execute),
                ["partmgr"] = args => RunWithSubCommand(args, PartmgrCommand.Execute),
                ["uasp"]    = args => RunWithSubCommand(args, UaspCommand.Execute),
            };
        }

        /// <summary>
        /// Parses global options and dispatches to the appropriate command.
        /// </summary>
        public int Route(string[] rawArgs)
        {
            var args = new List<string>(rawArgs);

            // Parse global options
            while (args.Count > 0 && args[0].StartsWith("--"))
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "--lang":
                        if (args.Count > 1)
                        {
                            try
                            {
                                Loc.Culture = new System.Globalization.CultureInfo(args[1]);
                            }
                            catch { /* ignore invalid culture */ }
                            args.RemoveAt(0);
                        }
                        args.RemoveAt(0);
                        break;

                    case "--help":
                        return RunHelp();

                    case "--version":
                        return RunVersion();

                    default:
                        ConsoleOutput.WriteError(Loc.Get("Error_UnknownCommand"));
                        return 1;
                }
            }

            if (args.Count == 0)
            {
                return RunHelp();
            }

            string command = args[0].ToLowerInvariant();
            args.RemoveAt(0);

            if (_routes.TryGetValue(command, out var handler))
            {
                try
                {
                    return handler(args.ToArray());
                }
                catch (Exception ex)
                {
                    ConsoleOutput.WriteError(Loc.Format("Error_Unexpected", ex.Message));
                    return 1;
                }
            }
            else
            {
                ConsoleOutput.WriteError(Loc.Get("Error_UnknownCommand"));
                return 1;
            }
        }

        private int RunInfo()
        {
            try
            {
                return InfoCommand.Execute(_context);
            }
            catch (Exception ex)
            {
                ConsoleOutput.WriteError(ex.Message);
                return 1;
            }
        }

        private int RunHelp()
        {
            Console.WriteLine();
            Console.WriteLine(Loc.Get("Help_Usage"));
            Console.WriteLine();
            return 0;
        }

        private int RunAbout()
        {
            Console.WriteLine();
            Console.WriteLine(Loc.Get("About_Text"));
            Console.WriteLine();
            return 0;
        }

        private int RunVersion()
        {
            Console.WriteLine(Loc.Get("Version_Text"));
            return 0;
        }

        private int RunWithSubCommand(string[] subArgs, Func<CommandContext, string, int> handler)
        {
            if (subArgs.Length == 0)
            {
                ConsoleOutput.WriteError(Loc.Get("Error_NoParameter"));
                return 1;
            }

            string sub = subArgs[0].ToLowerInvariant();
            return handler(_context, sub);
        }
    }
}
```

### 验证
- `dotnet build` 通过（某些未引用方法可能有警告，T7 重写 Program.cs 后消除）

---

## T7. 重写 Program.cs 入口点

### 操作类型
**重写** — `e:\Development\Git_Repos\wtgutil\Program.cs`

### 新内容

```csharp
using System;
using WTGUtility.Commands;
using WTGUtility.Infrastructure;
using WTGUtility.Localization;
using WTGUtility.Services;

namespace WTGUtility
{
    class Program
    {
        static int Main(string[] args)
        {
            // 1. Check administrator privileges (runs as invoker; exits if not admin)
            AdminCheck.EnsureAdministrator();

            // 2. Set console title
            Console.Title = Loc.Get("App_ShortTitle");

            // 3. Initialize services (Pure DI)
            var registry = new RegistryService();
            var detector = new DeviceDetector();
            var wtgService = new WtgService(registry, detector);

            // 4. Detect WTG device
            var deviceInfo = detector.DetectWtgDevice();

            // 5. Build command context
            var context = new CommandContext(wtgService)
            {
                WtgDeviceInstancePath = deviceInfo.InstancePath
            };

            // 6. Route and execute
            var router = new CommandRouter(context);
            return router.Route(args);
        }
    }
}
```

### 验证
- `dotnet build` 通过，无错误无警告（或仅有 RESX 相关初始化警告）

---

## T8. 配置 Costura.Fody 单文件输出

### 步骤 8.1：安装 NuGet 包

在终端中运行：
```powershell
cd e:\Development\Git_Repos\wtgutil
dotnet add package Costura.Fody --version 5.7.0
dotnet add package Fody --version 6.8.0
```

或者通过编辑 `.csproj` 添加：

```xml
<ItemGroup>
  <PackageReference Include="Costura.Fody" Version="5.7.0">
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <PackageReference Include="Fody" Version="6.8.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

### 步骤 8.2：创建 Fody 配置文件

**创建** — `e:\Development\Git_Repos\wtgutil\FodyWeavers.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Weavers>
  <Costura />
</Weavers>
```

### 步骤 8.3：移除 App.config（不再需要）

**删除** — `e:\Development\Git_Repos\wtgutil\App.config`

> .NET Framework 4.7.2 是 Windows 10 1803+ 的默认运行时，`supportedRuntime` 配置不再必需。如需兼容更旧系统，保留 App.config 但接受输出目录多一个 `.config` 文件。

### 步骤 8.4：确保 RESX 嵌入正确

`.csproj` 中无需额外配置 — SDK 风格项目会自动将 `.resx` 编译为嵌入资源。

### 验证
```powershell
dotnet build --configuration Release
# 检查 bin\Release\ 目录中仅有 wtgutil.exe（无 System.Management.dll 等依赖）
```

---

## T9. 清理与最终验证

### 步骤 9.1：删除旧文件清单

确认以下文件**已删除**：
- `Functions_chs.cs` ✅ (T2.6)
- `Info_chs.cs` ✅ (T2.6)
- `App.config` ✅ (T8.3)

以下文件**保留但已被取代**（后续手动清理）：
- `Functions.cs` — 逻辑已迁移至 `Services/`，待确认编译无引用后删除
- `Info.cs` — UI 字符串已迁移至 RESX，待确认编译无引用后删除
- `GetDeviceInstancePath.cs` — 逻辑已迁移至 `Services/DeviceDetector.cs`，待确认编译无引用后删除

### 步骤 9.2：最终编译验证

```powershell
# 清理
dotnet clean

# Release 构建
dotnet build .\wtgutil.csproj --configuration Release

# Debug 构建
dotnet build .\wtgutil.csproj --configuration Debug

# 检查输出
Get-ChildItem .\bin\Release\
# 期望输出：仅有 wtgutil.exe（或 wtgutil.exe + wtgutil.pdb）
```

### 步骤 9.3：功能验证清单

| 命令 | 预期行为 |
|------|----------|
| `wtgutil` | 显示帮助 |
| `wtgutil help` | 显示帮助 |
| `wtgutil --help` | 显示帮助 |
| `wtgutil --version` | 显示 `wtgutil v2.0.0` |
| `wtgutil about` | 显示关于信息 |
| `wtgutil info` | 显示当前系统设置 |
| `wtgutil mode wintogo` | 设置 WTG 模式 |
| `wtgutil mode default` | 恢复默认模式 |
| `wtgutil partmgr show` | 设置显示本地磁盘 |
| `wtgutil partmgr hide` | 设置隐藏本地磁盘 |
| `wtgutil uasp off` | 禁用 UASP |
| `wtgutil uasp on` | 启用 UASP |
| `wtgutil --lang zh-CN info` | 以中文显示系统信息 |
| `wtgutil --lang en info` | 以英文显示系统信息 |
| `wtgutil unknown` | 显示未知命令错误 |

### 步骤 9.4：清理旧文件

确认以下源文件不再被任何代码引用后（通过编译验证），可安全删除：

- `e:\Development\Git_Repos\wtgutil\Functions.cs`
- `e:\Development\Git_Repos\wtgutil\Info.cs`
- `e:\Development\Git_Repos\wtgutil\GetDeviceInstancePath.cs`

---

## 附录 A：完整文件变更汇总

| 操作 | 文件路径 | 说明 |
|------|----------|------|
| ✏️ 修改 | `wtgutil.csproj` | RootNamespace 统一 + Costura 包引用 |
| ➖ 不变 | `app.manifest` | 保持 `asInvoker`，运行时检测提权 |
| ✅ 新建 | `Localization/Strings.resx` | 英文资源 |
| ✅ 新建 | `Localization/Strings.zh-CN.resx` | 中文资源 |
| ✅ 新建 | `Localization/LocalizationManager.cs` | 语言管理器 |
| ✅ 新建 | `Models/Settings.cs` | 设置数据模型 |
| ✅ 新建 | `Models/DeviceInfo.cs` | 设备信息模型 |
| ✅ 新建 | `Infrastructure/AdminCheck.cs` | 管理员权限守卫 |
| ✅ 新建 | `Infrastructure/Console.cs` | 控制台输出辅助 |
| ✅ 新建 | `Services/RegistryService.cs` | 注册表服务 |
| ✅ 新建 | `Services/DeviceDetector.cs` | 设备检测服务 |
| ✅ 新建 | `Services/WtgService.cs` | WTG 业务编排 |
| ✅ 新建 | `Commands/ICommand.cs` | 命令接口 |
| ✅ 新建 | `Commands/CommandContext.cs` | 命令上下文 |
| ✅ 新建 | `Commands/CommandRouter.cs` | 命令路由器 |
| ✅ 新建 | `Commands/InfoCommand.cs` | info 命令 |
| ✅ 新建 | `Commands/HelpCommand.cs` | help 命令 |
| ✅ 新建 | `Commands/AboutCommand.cs` | about 命令 |
| ✅ 新建 | `Commands/ModeCommand.cs` | mode 命令 |
| ✅ 新建 | `Commands/PartmgrCommand.cs` | partmgr 命令 |
| ✅ 新建 | `Commands/UaspCommand.cs` | uasp 命令 |
| ✅ 新建 | `FodyWeavers.xml` | Costura 配置 |
| ✅ 新建 | `docs/MODERNIZATION_SPEC.md` | 用户说明文档 |
| ✅ 新建 | `docs/MODERNIZATION_TASKS.md` | Agent 任务文档（本文件） |
| 🗑️ 删除 | `Functions_chs.cs` | 中文旧版（已 RESX 化） |
| 🗑️ 删除 | `Info_chs.cs` | 中文旧版（已 RESX 化） |
| 🗑️ 删除 | `App.config` | 不再需要 |
| 🗑️ 删除 | `Functions.cs` | 逻辑已迁移至 Services/ |
| 🗑️ 删除 | `Info.cs` | UI 字符串已 RESX 化 |
| 🗑️ 删除 | `GetDeviceInstancePath.cs` | 逻辑已迁移至 Services/DeviceDetector.cs |

---

## 附录 B：设计决策记录

| 决策 | 理由 |
|------|------|
| 不主动提权（asInvoker），运行时检测 | 让用户自主决定是否提权；避免在某些受限环境下自动弹出 UAC 造成困惑 |
| 使用纯手工 DI 而非 IoC 容器 | 项目规模小，仅 3 个服务，NuGet 容器引入不必要复杂性 |
| 使用 RESX 而非 JSON 本地化 | .NET Framework 原生支持，Visual Studio 内置编辑器，编译时类型安全 |
| 保留 .NET Framework 4.7.2 而非升级 .NET 8 | 用户要求使用 Windows 预安装运行时，.NET 8 需额外安装 |
| 使用 Costura.Fody 而非 ILMerge | Costura 更现代，支持 SDK 风格项目，配置更简单 |
| 命令路由使用 Dictionary 而非反射 | 简单、快速、明确，无性能开销 |
| UASP on/off 而非 disable/enable | 更短、更直觉，与其他命令风格一致 |
| 移除 `--disable-force` 系列 | 高风险操作，用户明确要求仅保留 -disable |
