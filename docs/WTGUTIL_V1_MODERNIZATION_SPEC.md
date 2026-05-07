# wtgutil v2.0 现代化改造说明

> **面向用户** — 本文档描述 wtgutil v2.0 相较于 v1.x 的变更内容、新用法、以及迁移指南。

---

## 目录

- [1. 概述](#1-概述)
- [2. 命令行用法变更](#2-命令行用法变更)
- [3. 功能变更](#3-功能变更)
- [4. 本地化](#4-本地化)
- [5. 单文件分发](#5-单文件分发)
- [6. 从 v1.x 迁移](#6-从-v1x-迁移)
- [7. 兼容性说明](#7-兼容性说明)

---

## 1. 概述

wtgutil v2.0 是一次全面的现代化重构，核心目标：

| 目标 | 说明 |
|------|------|
| **更直觉的命令行** | 子命令模型取代 `/` `-` 混用风格 |
| **自动跟随系统语言** | 中英双语自动切换，不再需要选择语言版本 |
| **单文件分发** | 一个 `wtgutil.exe` 包含所有依赖，无需额外配置文件 |
| **更清晰的代码结构** | 为后续功能扩展和维护奠定基础 |

**不变的部分**：

- 仍然需要管理员权限运行
- 仍然通过修改注册表实现功能
- 仍然面向 .NET Framework 4.7.2（Windows 10 1803+ 已预装）
- 仍然是控制台应用程序
- 功能逻辑与 v1.x 完全等价

---

## 2. 命令行用法变更

### 2.1 新旧对照速查表

| 操作 | v1.x（旧） | v2.0（新） |
|------|-----------|-----------|
| 查看系统信息 | `wtgutil /info` | `wtgutil info` |
| 查看帮助 | `wtgutil /help` 或 `/?` | `wtgutil help` 或 `wtgutil --help` |
| 关于程序 | `wtgutil /about` | `wtgutil about` 或 `wtgutil --version` |
| 启用 WTG 模式 | `wtgutil /mode -wintogo` | `wtgutil mode wintogo` |
| 恢复默认模式 | `wtgutil /mode -default` | `wtgutil mode default` |
| 显示本地磁盘 | `wtgutil /partmgr -showlocaldisks` | `wtgutil partmgr show` |
| 隐藏本地磁盘 | `wtgutil /partmgr -hidelocaldisks` | `wtgutil partmgr hide` |
| 禁用 UASP | `wtgutil /uasp -disable` | `wtgutil uasp off` |
| 启用 UASP | *(无对应命令)* | `wtgutil uasp on` |
| 强制禁用 UASP | `wtgutil /uasp --disable-force` | **已移除** |
| 恢复强制禁用 | `wtgutil /uasp --disable-force-restore` | **已移除** |

### 2.2 完整命令参考

```
wtgutil [--lang <code>] [--help] [--version] <命令> [参数]

全局选项：
  --lang <code>     覆盖系统语言，支持 en, zh-CN
  --help            显示帮助
  --version         显示版本信息

命  令：
  info              显示当前系统 WTG 相关设置状态
  help              显示完整帮助信息
  about             显示关于信息（版本、作者、仓库地址）
  mode <配置>       切换系统模式
     wintogo           → 启用 Windows To Go 模式
     default           → 恢复 Windows 默认设置
  partmgr <动作>     控制本地磁盘显示
     show               → 显示本地磁盘
     hide               → 隐藏本地磁盘
  uasp <状态>        控制 UASP（USB Attached SCSI Protocol）
     off                → 禁用 UASP（启用"拔出冻结"）
     on                 → 重新启用 UASP
```

### 2.3 设计原则

- **子命令模型**：`wtgutil <动词> [宾语]`，与 `git`、`dotnet` 等现代工具一致
- **无前缀混淆**：不再混用 `/` `-` `--` 三种前缀
- **全局选项前置**：`--lang`、`--help`、`--version` 可在任意位置
- **PowerShell 友好**：自然语言风格，便于 TAB 补全

---

## 3. 功能变更

### 3.1 UASP 功能简化

v1.x 中 `/uasp` 命令有三个参数：

| 参数 | 用途 | 风险 |
|------|------|------|
| `-disable` | 修改设备注册表，切换到 BOT 模式 | 低 |
| `--disable-force` | 修改 UASPStor 驱动配置 | **高（可能导致 BSOD）** |
| `--disable-force-restore` | 还原 `--disable-force` 的修改 | — |

v2.0 **仅保留安全的 `-disable` 逻辑**，并新增反向操作 `on`：

- `wtgutil uasp off` — 禁用 UASP（等价于旧版 `-disable`）
- `wtgutil uasp on` — 重新启用 UASP（恢复注册表原始值）

> ⚠️ `--disable-force` 和 `--disable-force-restore` 已彻底移除。如果你曾使用过强制禁用功能，请在升级前使用旧版本运行 `wtgutil /uasp --disable-force-restore` 进行还原。

### 3.2 新增：版本信息

- `wtgutil --version` 显示当前版本号
- `wtgutil about` 显示完整关于信息（含构建日期、仓库地址）

### 3.3 新增：语言覆盖

- `wtgutil --lang zh-CN info` — 以简体中文显示信息（即使系统语言是英文）
- `wtgutil --lang en info` — 以英文显示信息（即使系统语言是中文）

---

## 4. 本地化

### 4.1 v1.x 的问题

旧版通过**复制全部代码文件**实现中英双语：

```
Functions.cs      ← 英文逻辑 + 英文字符串
Functions_chs.cs  ← 完全重复的代码 + 中文字符串
Info.cs           ← 英文逻辑 + 英文字符串
Info_chs.cs       ← 完全重复的代码 + 中文字符串
```

切换语言需要修改 `Program.cs` 的 `using` 语句并重新编译。

### 4.2 v2.0 的方案

使用 .NET 标准的 **RESX 资源文件**：

```
Localization/
├── Strings.resx          ← 英文（默认/回退）
└── Strings.zh-CN.resx    ← 简体中文
```

- **代码只有一份**，所有 UI 字符串通过 `Strings.ResourceManager.GetString()` 获取
- 运行时**自动检测**系统 UI 语言，无需手动选择
- 可通过 `--lang` 参数临时覆盖
- 未来添加新语言只需新增一个 `.resx` 文件

---

## 5. 单文件分发

### 5.1 变更

| 旧版文件 | v2.0 |
|----------|------|
| `wtgutil.exe` | `wtgutil.exe`（内嵌所有依赖） |
| `wtgutil.exe.config` | ❌ 不再需要 |
| `wtgutil.exe.manifest` | ❌ 内嵌到 EXE |

### 5.2 实现方式

使用 **Costura.Fody** 在编译时将依赖的 DLL 作为嵌入资源打包进 `wtgutil.exe`。

### 5.3 安装方式

与旧版完全一致：将 `wtgutil.exe` 放入 `C:\Windows` 目录即可全局使用。

---

## 6. 从 v1.x 迁移

### 6.1 如果你用过 `--disable-force`

在替换为新版之前，**必须**先用旧版还原：

```powershell
# 使用旧版 wtgutil 还原强制禁用
wtgutil /uasp --disable-force-restore

# 确认还原成功后，再用新版替换
```

### 6.2 脚本/批处理迁移

如果你在脚本中使用了 wtgutil，需要更新命令行参数：

```batch
:: 旧版
wtgutil /mode -wintogo
wtgutil /partmgr -hidelocaldisks
wtgutil /uasp -disable

:: 新版
wtgutil mode wintogo
wtgutil partmgr hide
wtgutil uasp off
```

### 6.3 UASP 启用（新功能）

旧版没有直接启用 UASP 的命令。新版使用：

```powershell
wtgutil uasp on
```

这会恢复 WTG 驱动器的默认 USB 存储驱动配置。

---

## 7. 兼容性说明

| 项目 | v1.12.3 | v2.0 |
|------|---------|------|
| 目标框架 | .NET Framework 4.7.2 | .NET Framework 4.7.2 |
| 最低 Windows | Windows 7 | Windows 8 |
| 架构 | x86 / amd64 | x64 / AnyCPU |
| 管理员权限 | 必需 | 必需（用户态运行时检测并提示手动提权） |
| 注册表修改 | `HKLM\SYSTEM\...` | 完全相同 |
| License | GPLv3 | GPLv3 |

---

> 📌 项目仓库：<https://github.com/Nothing9495/wtgutil>
