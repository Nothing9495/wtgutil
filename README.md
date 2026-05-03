# wtgutil

[![standard-readme compliant](https://img.shields.io/badge/readme%20style-standard-brightgreen.svg?style=flat-square)](https://github.com/RichardLitt/standard-readme)
[![License: GPLv3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)

**wtgutil** (Windows To Go Utility) is a command-line tool that helps you manage and optimize your Windows To Go (WTG) workstation. It allows you to toggle WTG-specific registry settings, control local disk visibility, and manage the UASP (USB Attached SCSI Protocol) behavior of your WTG drive — all without manual registry editing.

## Contents

- [Background](#background)
- [Features](#features)
- [Install & Usage](#install--usage)
  - [System Requirements](#system-requirements)
  - [Install](#install)
  - [Usage](#usage)
- [Commands](#commands)
  - [install](#install-1)
  - [uninstall](#uninstall)
  - [info](#info)
  - [mode](#mode)
  - [partmgr](#partmgr)
  - [uasp](#uasp)
  - [help & about](#help--about)
- [Global Options](#global-options)
- [How It Works](#how-it-works)
- [Q&A](#qa)
- [Contributing](#contributing)
- [License](#license)

## Background

Windows To Go allows you to run Windows from a USB drive, but Microsoft has progressively restricted this feature. For example, upgrading a WTG installation to Windows 11 is blocked by default unless certain registry keys are modified.

This tool automates those modifications — originally created to simplify upgrading a WTG system to Windows 11, it has grown into a utility for general WTG system management.

> **Note:** This is the author's first C# project. The code may not be idiomatic; suggestions and contributions are welcome!

## Features

- **View current settings** — Inspect all WTG-related registry values at a glance
- **Toggle WTG mode** — Switch between Windows To Go mode and standard desktop mode
- **Hide/show local disks** — Prevent the WTG system from mounting internal drives to avoid accidental modifications
- **Manage UASP** — Enable or disable USB Attached SCSI Protocol on the WTG drive (disabling UASP improves stability on unexpected disconnection)
- **Multi-language support** — Switch display language via `--lang` flag (English and Simplified Chinese)

## Install & Usage

### System Requirements

- **OS:** Windows 10 or newer
- **Architecture:** amd64
- **Runtime:** .NET Framework 4.7.2 or newer
- **Privileges:** Administrator rights (required for registry modifications)

### Install

You can run `wtgutil` directly without installation — just download and go:

1. Download the latest binary from the [Releases](https://github.com/Nothing9495/wtgutil/releases) page.
2. Rename the downloaded file to `wtgutil.exe`.
3. Run it directly from the download location:

   ```cmd
   .\wtgutil help
   ```

> [!TIP]
> For convenience, run `wtgutil install` **as Administrator** to install it into your system, following operations will be carried out:
> - Copies `wtgutil.exe` to `%ProgramFiles%\WTGUtility\`
> - Creates a `wtgu` alias (hard link) so you can use `wtgu` in place of `wtgutil`
> - Adds the install directory to the system `PATH`
> - Adds `HKLM/Software/wtgutil/InstallFlag` key
>
> After one-time installation, you can use `wtgutil` or `wtgu` from anywhere.
> If you want to remove wtgutil, simply run `wtgutil uninstall`

### Usage

Run Windows Terminal, PowerShell, or Command Prompt **as Administrator**, then type:

```cmd
wtgutil <command> [arguments]
```

The program will exit immediately if not run with administrator privileges.

To get started quickly:

```cmd
wtgutil help
```

## Commands

### `install`

Install `wtgutil` into your system for global access.

```cmd
wtgutil install
```

What it does:
- Copies `wtgutil.exe` to `%ProgramFiles%\wtgutil\`
- Creates a `wtgu` hard link alias (`wtgu` works the same as `wtgutil`)
- Adds the install directory to the system `PATH`
- Sets an install flag in `HKLM\Software\wtgutil`

After installation, the original executable is deleted automatically. You can then use `wtgutil` or `wtgu` from any terminal.

> [!TIP]
> A terminal restart may be required for the `PATH` change to take effect.

### `uninstall`

Remove `wtgutil` from your system.

```cmd
wtgutil uninstall
```

What it does:
- Deletes `wtgutil.exe` and the `wtgu` alias from `%ProgramFiles%\wtgutil\`
- Removes the install directory from the system `PATH`
- Cleans up the `HKLM\Software\wtgutil` registry key

> If `wtgutil` is running from the installed location, the executable is scheduled for deletion after the process exits.

> [!TIP]
> Need it again? Re-download from the [Releases page](https://github.com/Nothing9495/wtgutil/releases) and run `wtgutil install`.

> [!WARNING]
> `uninstall` will refuse to run unless `wtgutil` was installed via `wtgutil install`. This prevents accidental file deletion.

### `info`

Display all current WTG-related system settings, including:
- USB boot status (`BootDriverFlags`)
- Windows To Go flag (`PortableOperatingSystem`)
- Local disk visibility (`SanPolicy`)
- UASP status for the WTG drive

```cmd
wtgutil info
```

### `mode`

Switch the system between Windows To Go mode and default desktop mode.

| Subcommand   | Effect |
|-------------|--------|
| `wintogo`   | Enable Windows To Go mode — sets `BootDriverFlags=20`, `PortableOperatingSystem=1`, `SanPolicy=4` |
| `default`   | Restore standard desktop settings — sets `BootDriverFlags=0`, `PortableOperatingSystem=0`, `SanPolicy=1` |

```cmd
wtgutil mode wintogo
wtgutil mode default
```

> [!WARNING]
> Switching to `default` mode will disable USB boot support. **Do not restart your WTG system** after applying this setting, or it may fail to boot from the USB drive.

### `partmgr`

Control whether local (internal) disks are visible to the WTG system.

| Subcommand | Effect |
|-----------|--------|
| `show`     | Local disks are visible at startup (`SanPolicy=1`) |
| `hide`     | Local disks are hidden at startup (`SanPolicy=4`) — prevents accidental writes to internal drives |

```cmd
wtgutil partmgr show
wtgutil partmgr hide
```

> A system restart is required for changes to take effect.

### `uasp`

Enable or disable UASP (USB Attached SCSI Protocol) on the WTG drive.

| Subcommand | Effect |
|-----------|--------|
| `off`      | Disable UASP — forces the USB mass storage driver (`USBSTOR`), which reduces data corruption risk when the drive is unexpectedly unplugged |
| `on`       | (Experimental) Re-enable UASP — uses the native `UASPStor` driver for better performance |

```cmd
wtgutil uasp off
wtgutil uasp on
```

> [!NOTE]
> The `uasp` command requires a WTG drive to be detected. If no WTG drive is found, the command will report an error.

### `help` & `about`

```cmd
wtgutil help       # Show usage information and all available commands
wtgutil about      # Display version and license information
```

## Global Options

These options can be placed before any command:

| Option               | Description |
|----------------------|-------------|
| `--help`             | Show help information |
| `--version`          | Display the version number |
| `--lang <code>`      | Override display language. Supported codes: `en` (English), `zh-CN` (Simplified Chinese) |

```cmd
wtgutil --lang zh-CN info
wtgutil --version
```

## How It Works

**wtgutil** operates by modifying specific registry keys that control Windows To Go behavior. Below is the complete reference of affected registry entries.

|Registry Key|Registry Path|Type|Value|Description|Value Description|
|:-----------|:-----------:|:--:|:---:|:---------:|:---------------:|
|`BootDriverFlags`|`HKLM\SYSTEM\HardwareConfig\Current`|`REG_DWORD`|`20`or`0`|Controls whether the system allows booting from USB devices.|`0` - System defaults<br>`20` - Allow booting from USB devices|
|`PortableOperatingSystem`|`HKLM\SYSTEM\CurrentControlSet\Control`|`REG_DWORD`|`0`or`1`|Tells Windows it is running from a portable installation and enables Windows To Go features.|`0` - Disabled<br>`1` - Enable|
|`SanPolicy`|`HKLM\SYSTEM\CurrentControlSet\Services\partmgr\Parameters`|`REG_DWORD`|`1`or`4`|Controls whether the partition manager exposes local/internal disks.|`1` - Show local disks<br>`4` - Hide local disks|
|`Capabilities`<br>`DeviceDesc`<br>`Mfg`<br>`Service`|`HKLM\SYSTEM\CurrentControlSet\Enum\<deviceInstancePath>`|N/A|N/A|Disabling UASP modifies the device's registry entries under `HKLM\SYSTEM\CurrentControlSet\Enum\<DeviceInstancePath>` to use the `USBSTOR` driver instead of `UASPStor`. This improves resilience to sudden disconnection at the cost of peak I/O performance.|N/A|

## Q&A

**Q: Why does the tool require administrator privileges?**

Because it reads and writes to `HKLM` (local machine) registry hives, which requires elevated access.

**Q: What happens if I disable UASP?**

The WTG drive switches from the UASP driver to the generic USB mass storage driver. This slightly reduces performance but eliminates the risk of filesystem corruption when the drive is unplugged without being safely removed first.

**Q: Can I undo all changes?**

Yes. Run `wtgutil mode default` and/or `wtgutil uasp on` to revert the settings, then restart your system.

**Q: How do I know my current settings?**

Run `wtgutil info` to display all current WTG-related values.

**Q: Is the WTG drive auto-detected?**

Yes. The tool uses WMI to query `Win32_SCSIController` and finds the USB controller associated with the WTG drive.

## Contributing

Contributions are welcome! Feel free to:

- [Open an issue](https://github.com/Nothing9495/wtgutil/issues) for bug reports or feature requests
- Submit a Pull Request with improvements

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).
