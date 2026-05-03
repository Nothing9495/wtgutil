# wtgutil — AI Agent Instructions

## 项目概述

**wtgutil**（Windows To Go Utility）是一个 CLI 工具，用于管理 Windows To Go 工作站的注册表设置。C# 项目，面向 .NET Framework 4.7.2，SDK 风格 `.csproj`。

## 构建命令

```bash
# 全部使用 dotnet build（非 msbuild）
dotnet build src\wtgutil.csproj --configuration Debug
dotnet build src\wtgutil.csproj --configuration Release
dotnet build src\wtgutil.csproj --configuration Release /p:Platform=x64
```

> 因同时存在 `.sln` 和 `.csproj`，构建时必须显式指定 `.csproj` 路径。

## 架构概览

| 层 | 目录 | 职责 |
|---|---|---|
| 入口 | `src/Program.cs` | 管理员检查 → Pure DI → 路由调度 |
| 命令 | `src/Commands/` | CLI 命令处理器（`ICommand` 接口 + 静态 Execute 方法） |
| 服务 | `src/Services/` | 业务逻辑：`WtgService`（编排）、`RegistryService`（注册表）、`DeviceDetector`（WMI） |
| 基础设施 | `src/Infrastructure/` | `AdminCheck`（权限检查）、`ConsoleOutput`（彩色输出） |
| 模型 | `src/Models/` | `DeviceInfo`、`WtgSettings`（数据实体） |
| 本地化 | `src/Localization/` | RESX 资源文件 + `LocalizationManager.cs` |

## 关键约定

- **命令路由**：`CommandRouter` 使用 `Dictionary<string, Func<>>` 将 CLI 参数映射到 `Commands/*Command.cs` 中的静态 `Execute` 方法
- **子命令模式**：`mode`、`partmgr`、`uasp` 等命令通过 `RunWithSubCommand()` 接收第二个参数
- **Pure DI**：无 DI 容器，`Program.cs` 中手动创建依赖
- **本地化**：通过 `Loc.Get("key")` 获取字符串，`--lang` 全局选项覆盖语言
- **管理员检查**：`app.manifest` 为 `asInvoker`，运行时在 `AdminCheck.EnsureAdministrator()` 中检测并退出
- **单文件分发**：Costura.Fody 内嵌依赖 DLL
- **安装/卸载**：`install` 复制自身到 `%ProgramFiles%\WTGUtility\`，通过 `mklink /h` 创建硬链接别名 `wtgu.exe`，并写入系统 PATH（`HKLM\...\Environment`）；卸载时逆向操作并清理 PATH
- **LangVersion**：9.0（平台无关时），x64 平台强制指定 9.0

## 重要文件

| 文件 | 说明 |
|---|---|
| `src/Program.cs` | 应用入口、DI 组装 |
| `src/Commands/CommandRouter.cs` | CLI 参数路由核心 |
| `src/Services/RegistryService.cs` | 所有注册表操作集中管理 |
| `src/Services/DeviceDetector.cs` | WMI 检测 WTG 设备 |
| `src/Localization/LocalizationManager.cs` | 双语（EN/ZH-CN）本地化管理 |
| `LICENSE` | GNU GPLv3 |

## 常见陷阱

- 构建必须指定 `.csproj` 路径，因目录下同时有 `.sln` 和 `.csproj`
- `ICommand.ExecuteAsync()` 方法当前未启用（返回 `Task.FromResult(0)`），实际执行走静态 `Execute` 方法
- `.resx` 中 `StringsZhCN.resx` 是独立资源（非卫星程序集），通过 `LocalizationManager.cs` 中双重 `ResourceManager` 加载
