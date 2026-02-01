# RecipeUI .NET Framework 4.8 to .NET 10 Upgrade Tasks

## Overview

This document tracks the execution of RecipeUI project upgrade from .NET Framework 4.8 to .NET 10. The single project will be converted to SDK-style format and upgraded to net10.0-windows in one atomic operation.

**Progress**: 1/2 tasks complete (50%) ![0%](https://progress-bar.xyz/50)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-02-01 16:57)*
**References**: Plan §Migration Steps - Prerequisites

- [✓] (1) Verify .NET 10 SDK installed on development machine
- [✓] (2) .NET 10 SDK version meets minimum requirements (**Verify**)
- [✓] (3) Verify Visual Studio 2022 version 17.12 or later installed (if using Visual Studio)
- [✓] (4) Visual Studio version supports .NET 10 (**Verify**)

---

### [▶] TASK-002: Atomic SDK conversion and framework upgrade
**References**: Plan §Migration Steps, Plan §Breaking Changes Catalog, Plan §Package Update Reference

- [▶] (1) Convert RecipeUI.csproj from classic .NET Framework format to SDK-style format (use upgrade-assistant tool or manual conversion per Plan §Project File Conversion)
- [ ] (2) Project file converted to SDK-style format (**Verify**)
- [ ] (3) Update TargetFramework property to `net10.0-windows`
- [ ] (4) Target framework updated to net10.0-windows (**Verify**)
- [ ] (5) Verify all 4 NuGet package references are preserved (JsonConverter.Abstractions 0.8.0, JsonConverter.Newtonsoft.Json 0.8.0, Newtonsoft.Json 13.0.4, Stef.Validation 0.1.1)
- [ ] (6) All package references preserved in project file (**Verify**)
- [ ] (7) Restore NuGet packages
- [ ] (8) All packages restored successfully (**Verify**)
- [ ] (9) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus areas: Windows Forms API binary incompatibility resolves via recompilation; System.Drawing API may need System.Drawing.Common package; legacy configuration may need System.Configuration.ConfigurationManager package)
- [ ] (10) Solution builds with 0 errors (**Verify**)
- [ ] (11) Commit all changes with message: "Upgrade RecipeUI from .NET Framework 4.8 to .NET 10 - Convert to SDK-style and update target framework to net10.0-windows"

---


