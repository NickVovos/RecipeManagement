# .NET Framework 4.8 to .NET 10 Migration Plan
## RecipeUI Project Upgrade

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Risk Management](#risk-management)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade the RecipeUI Windows Forms project from .NET Framework 4.8 to .NET 10 (Long Term Support).

### Scope
- **Projects Affected**: 1 project (RecipeUI.csproj)
- **Current State**: .NET Framework 4.8, classic (non-SDK-style) project format
- **Target State**: .NET 10 with Windows Desktop support, SDK-style project format

### Discovered Metrics
- **Total Projects**: 1
- **Total Lines of Code**: 2,205
- **Estimated LOC to Modify**: 1,298+ (58.9% of codebase)
- **Total API Issues**: 1,298
  - Binary Incompatible: 1,244 (primarily Windows Forms APIs)
  - Source Incompatible: 45 (System.Drawing types)
  - Behavioral Changes: 9
- **NuGet Packages**: 4 (all compatible with .NET 10)
- **Security Vulnerabilities**: 0

### Complexity Classification
**Simple Solution** - Single standalone project with no dependencies

**Justification**:
- Only one project to upgrade
- Zero project dependencies
- All NuGet packages are already compatible
- API issues are primarily due to Windows Forms framework differences (binary incompatibility is expected and resolved through recompilation after SDK conversion)
- No circular dependencies or complex relationships

### Selected Strategy
**All-At-Once Strategy** - Single atomic upgrade operation

**Rationale**:
- Single project eliminates coordination complexity
- All packages already compatible (no version conflicts)
- Windows Forms APIs marked as "binary incompatible" will resolve automatically once project targets net10.0-windows and uses Windows Desktop SDK
- Straightforward conversion path: SDK-style conversion → target framework update → recompile

### Critical Issues
- **Project Conversion Required**: Must convert from classic .NET Framework project format to SDK-style format
- **Windows Desktop Support**: Must target `net10.0-windows` (not just `net10.0`) to access Windows Forms APIs
- **API Compatibility**: 1,298 API issues are expected for Windows Forms migration and will resolve through proper SDK targeting

### Expected Iterations
This plan will be completed in 5-6 iterations:
- Phase 1: Discovery & Classification (complete)
- Phase 2: Foundation (dependency analysis, strategy, project stubs) - 3 iterations
- Phase 3: Detailed specifications - 1-2 iterations (single project, batch approach)

---

## Migration Strategy

### Approach Selection
**All-At-Once Strategy** - Single atomic upgrade operation

### Justification

**Why All-At-Once:**
1. **Single Project Scope**: Only one project eliminates any coordination complexity
2. **No Dependency Chains**: Zero inter-project dependencies means no ordering constraints
3. **Compatible Packages**: All 4 NuGet packages are already compatible with .NET 10
4. **Clear Migration Path**: Windows Forms has well-established migration patterns to .NET

**Why Not Incremental:**
- No benefit to phasing when there's only one project
- No intermediate states needed
- No dependency ordering to manage

### All-At-Once Strategy Rationale

This solution is an ideal candidate for All-At-Once migration:
- **Small solution**: 1 project (< 5 projects threshold)
- **Simple structure**: No project dependencies
- **Low package complexity**: All packages compatible, no version conflicts
- **Homogeneous codebase**: Single technology stack (Windows Forms)
- **Expected API issues**: The 1,298 API compatibility issues are expected for Windows Forms migration and resolve automatically through SDK conversion and proper targeting

### Migration Sequence

The upgrade will proceed as a single coordinated operation:

1. **Convert to SDK-Style Project** (automated via .NET Upgrade Assistant tooling)
   - Modernize project file format
   - Remove unnecessary references
   - Simplify project structure

2. **Update Target Framework** (in same operation)
   - Change from `net48` to `net10.0-windows`
   - Ensure Windows Desktop SDK support is enabled
   - Verify package references remain compatible

3. **Restore and Build** (immediate validation)
   - Restore NuGet packages for .NET 10
   - Build project to identify any remaining issues
   - Address compilation errors if any

4. **Verification** (confirms success)
   - Zero compilation errors
   - Zero warnings
   - Application launches and runs

### Parallel vs Sequential Execution
Not applicable - single project upgrade is inherently atomic.

### Phase Definitions
**Single Phase: Atomic Upgrade**
- Convert RecipeUI.csproj to SDK-style
- Update target framework to net10.0-windows
- Build and verify

---

## Detailed Dependency Analysis

### Dependency Graph Summary
The solution contains a single project with no inter-project dependencies.

```
RecipeUI.csproj (net48 → net10.0-windows)
  └─ No project dependencies
```

### Project Groupings by Migration Phase
Since this is a single-project solution, there is only one migration phase.

**Phase 1: Atomic Upgrade**
- RecipeUI.csproj (Windows Forms application)

### Critical Path Identification
With only one project, the critical path is straightforward:
1. Convert project to SDK-style format
2. Update target framework to net10.0-windows
3. Recompile and address any remaining compatibility issues

### Dependency Relationships
- **RecipeUI.csproj**: Standalone project
  - **External Dependencies**: 4 NuGet packages (all compatible)
  - **Project Dependencies**: None
  - **Dependents**: None
  - **Migration Order**: N/A (single project)

### Circular Dependencies
None detected.

---

## Project-by-Project Migration Plans

### Project: RecipeUI.csproj

**Current State**: 
- Target Framework: net48
- SDK-style: False (classic .NET Framework project)
- Project Type: ClassicWinForms
- Lines of Code: 2,205
- Files: 24
- Files with API Issues: 11
- NuGet Packages: 4
  - JsonConverter.Abstractions 0.8.0
  - JsonConverter.Newtonsoft.Json 0.8.0
  - Newtonsoft.Json 13.0.4
  - Stef.Validation 0.1.1

**Target State**:
- Target Framework: net10.0-windows
- SDK-style: True
- Project Type: Windows Forms (.NET)
- NuGet Packages: 4 (same versions - all compatible)

---

### Migration Steps

#### 1. Prerequisites
- .NET 10 SDK installed on development machine
- Visual Studio 2022 version 17.12 or later (with .NET 10 support)
- Backup/commit current state (already done on branch `upgrade-to-NET10`)

#### 2. Project File Conversion (SDK-Style)

**Action**: Convert RecipeUI.csproj from classic .NET Framework format to SDK-style format

**Conversion Tool**: Use `upgrade-assistant` CLI or manual conversion

**Expected Changes**:
- Replace verbose project file with minimal SDK-style format
- Remove explicit file includes (SDK-style uses implicit glob patterns)
- Remove unnecessary assembly references (Windows Forms references now implicit)
- Add `<TargetFramework>net10.0-windows</TargetFramework>`
- Add `<UseWindowsForms>true</UseWindowsForms>` (or use `net10.0-windows` which implies this)
- Preserve NuGet package references

**Manual Verification Required**:
- Ensure all 4 NuGet packages are preserved
- Verify app.config settings are migrated if needed
- Check embedded resources are included
- Confirm designer files (.Designer.cs) are properly referenced

#### 3. Target Framework Update

**Action**: Update TargetFramework property

**Current**: `<TargetFramework>net48</TargetFramework>` (or TargetFrameworkVersion in classic format)

**Target**: `<TargetFramework>net10.0-windows</TargetFramework>`

**Critical**: Must use `net10.0-windows` (not just `net10.0`) to access Windows Forms APIs

#### 4. Package References

No package updates required - all packages are compatible:

| Package | Version | Status | Action |
|---------|---------|--------|--------|
| JsonConverter.Abstractions | 0.8.0 | ✅ Compatible | No change |
| JsonConverter.Newtonsoft.Json | 0.8.0 | ✅ Compatible | No change |
| Newtonsoft.Json | 13.0.4 | ✅ Compatible | No change |
| Stef.Validation | 0.1.1 | ✅ Compatible | No change |

#### 5. Expected Breaking Changes

**Windows Forms API Binary Incompatibility (1,244 issues)**:
- **Nature**: Binary incompatibility between .NET Framework and .NET implementations
- **Resolution**: Automatic via recompilation with correct targeting
- **Action Required**: None - recompile resolves these
- **Why**: Windows Forms APIs exist in different assemblies (.NET Framework vs .NET), causing binary incompatibility flags, but APIs are functionally equivalent

**System.Drawing API Source Incompatibility (45 issues)**:
- **Nature**: System.Drawing types may need System.Drawing.Common package
- **Common APIs Affected**:
  - `System.Drawing.Image` (10 occurrences)
  - `System.Drawing.ContentAlignment` (9 occurrences)
  - Other GDI+ types
- **Resolution**: SDK-style Windows Forms projects automatically reference System.Drawing.Common
- **Action Required**: Verify no compilation errors after conversion; add package explicitly if needed

**Legacy Configuration System (2 issues)**:
- **Nature**: app.config based configuration
- **Assessment**: Minimal usage (only 2 instances)
- **Resolution**: 
  - Keep existing app.config for backward compatibility
  - Consider migrating to `Microsoft.Extensions.Configuration` in future enhancement
- **Action Required**: No immediate action; monitor for deprecation warnings

**Behavioral Changes (9 issues)**:
- **Nature**: Subtle runtime behavior differences
- **Impact**: Low
- **Action Required**: Thorough testing of UI interactions, image rendering, and event handling

#### 6. Code Modifications

**Expected Modifications**: Minimal to none

Most API issues (1,244 out of 1,298) are binary incompatibility flags that resolve through recompilation. Actual code changes needed only if:

1. **System.Drawing.Common is required but not auto-referenced**
   - Add package: `<PackageReference Include="System.Drawing.Common" Version="9.0.0" />`

2. **Configuration system issues**
   - If ConfigurationManager is used, may need package: `System.Configuration.ConfigurationManager`
   - Check if app.config values are properly read

3. **Namespace adjustments** (unlikely but possible)
   - Most Windows Forms types remain in `System.Windows.Forms`
   - Most drawing types remain in `System.Drawing`

**Files with API Issues** (11 files to monitor during compilation):
- Review compiler output for these files specifically
- Most issues will clear automatically upon recompilation

#### 7. Testing Strategy

**Smoke Tests** (after successful compilation):
- [ ] Application launches without errors
- [ ] Main form displays correctly
- [ ] All buttons and controls are functional
- [ ] ListView controls display data properly
- [ ] Images render correctly (PictureBox controls)
- [ ] Dialog boxes open and close properly
- [ ] Form events (Click, etc.) fire correctly

**Functional Tests**:
- [ ] Recipe creation workflow
- [ ] Recipe editing workflow
- [ ] Recipe deletion workflow
- [ ] ListView interactions (selection, sorting)
- [ ] All form controls work as expected
- [ ] Configuration loading (if app.config is used)

**Regression Tests**:
- [ ] Compare behavior with .NET Framework 4.8 version
- [ ] Verify image quality and rendering
- [ ] Check for any performance differences
- [ ] Validate form positioning and sizing

#### 8. Validation Checklist

**Build Success**:
- [ ] Project builds without errors
- [ ] Project builds without warnings (or warnings understood and acceptable)
- [ ] All 24 files compile successfully
- [ ] Output executable is generated

**Runtime Validation**:
- [ ] Application starts without exceptions
- [ ] No missing assembly errors
- [ ] No Windows Forms initialization errors
- [ ] All NuGet package dependencies resolved

**API Compatibility**:
- [ ] No compilation errors from Windows Forms APIs
- [ ] No compilation errors from System.Drawing APIs
- [ ] Configuration system works (if used)
- [ ] No obsolete API warnings that block functionality

**Quality**:
- [ ] No new warnings introduced
- [ ] Code quality maintained
- [ ] Application behavior matches .NET Framework version

---

## Package Update Reference

### Summary
All NuGet packages currently in use are compatible with .NET 10. No package updates are required.

### Package Compatibility Matrix

| Package | Current Version | Target Version | Projects | Status | Notes |
|---------|----------------|----------------|----------|--------|-------|
| JsonConverter.Abstractions | 0.8.0 | 0.8.0 | RecipeUI | ✅ Compatible | No update needed |
| JsonConverter.Newtonsoft.Json | 0.8.0 | 0.8.0 | RecipeUI | ✅ Compatible | No update needed |
| Newtonsoft.Json | 13.0.4 | 13.0.4 | RecipeUI | ✅ Compatible | No update needed |
| Stef.Validation | 0.1.1 | 0.1.1 | RecipeUI | ✅ Compatible | No update needed |

### Potential Additional Package

**System.Drawing.Common** (may be auto-referenced):
- **Current**: Not explicitly referenced
- **Target**: Auto-referenced by Windows Forms SDK or add explicitly if needed
- **Version**: 9.0.0 (if explicit reference needed)
- **Reason**: Required for System.Drawing APIs on .NET
- **Action**: Monitor during compilation; add only if compilation errors occur

---

## Breaking Changes Catalog

### Overview
The assessment identified 1,298 API compatibility issues. However, the vast majority (95.8%) are Windows Forms binary incompatibility issues that resolve automatically through SDK conversion and proper targeting.

### Category Breakdown

| Category | Count | Impact | Resolution |
|----------|-------|--------|-----------|
| Binary Incompatible | 1,244 | **Auto-resolved** | Recompilation with net10.0-windows |
| Source Incompatible | 45 | Low - Potential package reference | Verify System.Drawing.Common auto-referenced |
| Behavioral Changes | 9 | Low - Testing required | Runtime testing and validation |

---

### 1. Windows Forms API Binary Incompatibility (1,244 issues)

**Root Cause**: Windows Forms APIs exist in different assemblies between .NET Framework and .NET

**Impact**: Binary incompatibility flags during analysis, but APIs are functionally equivalent

**Resolution**: **Automatic** - Recompilation against .NET 10 Windows Forms assemblies

**Most Frequent APIs** (represent typical Windows Forms development):
- `System.Windows.Forms.Button` (167 occurrences)
- `System.Windows.Forms.Label` (71 occurrences)
- `System.Windows.Forms.Control.Location` (70 occurrences)
- `System.Windows.Forms.Control.Width` (64 occurrences)
- `System.Windows.Forms.TextBox` (52 occurrences)
- `System.Windows.Forms.ListView` (47 occurrences)
- `System.Windows.Forms.DialogResult` (44 occurrences)

**Action Required**: None - these resolve automatically when project targets net10.0-windows

**Verification**: Compilation succeeds without errors for these APIs

---

### 2. System.Drawing API Source Incompatibility (45 issues)

**Root Cause**: System.Drawing moved to separate package (System.Drawing.Common) in .NET

**Impact**: May require explicit package reference

**Affected APIs**:
- `System.Drawing.Image` (10 occurrences)
- `System.Drawing.ContentAlignment` (9 occurrences)
- Other GDI+ graphics types

**Resolution**:
1. **First**: Rely on automatic reference (Windows Forms SDK includes System.Drawing.Common)
2. **If compilation fails**: Add explicit package reference

```xml
<PackageReference Include="System.Drawing.Common" Version="9.0.0" />
```

**Action Required**: 
- Monitor compilation output
- Add package only if compiler errors appear for System.Drawing types

**Verification**: 
- No compilation errors for System.Drawing.Image, ContentAlignment, etc.
- Image rendering works correctly at runtime

---

### 3. Legacy Configuration System (2 issues)

**Root Cause**: app.config based configuration replaced by Microsoft.Extensions.Configuration in .NET

**Impact**: Minimal (only 2 API references detected)

**Affected APIs**:
- Configuration file reading
- App settings access

**Resolution Options**:

**Option 1 (Recommended for now)**: Keep app.config compatibility
```xml
<PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
```

**Option 2 (Future enhancement)**: Migrate to modern configuration
- Use Microsoft.Extensions.Configuration
- Migrate from app.config to appsettings.json
- Use environment variables and dependency injection

**Action Required**: 
- Add System.Configuration.ConfigurationManager package if ConfigurationManager API is used
- Monitor for deprecation warnings

**Verification**:
- Application settings load correctly
- Configuration values are read properly

---

### 4. Behavioral Changes (9 issues)

**Root Cause**: Subtle runtime behavior differences between .NET Framework and .NET

**Impact**: Low - requires thorough testing

**Potential Areas**:
- Graphics rendering differences
- Control event timing
- Layout engine subtle changes
- Font rendering variations

**Resolution**: Testing and validation

**Action Required**:
- Comprehensive UI testing
- Visual comparison with .NET Framework version
- Validate all user interactions
- Check image quality and rendering

**Verification**:
- UI looks and behaves identically
- No visual regressions
- No functional differences
- Performance is acceptable

---

### Migration Path Summary by Technology

| Technology | Issues | Migration Approach |
|-----------|--------|-------------------|
| **Windows Forms** | 1,244 | Target net10.0-windows; recompile; verify UI functionality |
| **GDI+/System.Drawing** | 41 | Ensure System.Drawing.Common is referenced (usually automatic); test image rendering |
| **Legacy Configuration** | 2 | Add System.Configuration.ConfigurationManager package; consider future migration to modern config |

---

### Known Breaking Changes Reference

**From .NET Framework 4.8 to .NET 10**:

1. **Windows Forms Designer**: May need Visual Studio 2022 with .NET 10 SDK
2. **App Domains**: Not supported (impact: low for typical Windows Forms apps)
3. **Code Access Security (CAS)**: Removed (impact: low unless explicitly used)
4. **BinaryFormatter**: Deprecated and unsafe (impact: only if used for serialization)
5. **Windows-specific APIs**: Windows Forms is Windows-only; cross-platform not applicable

**Not Expected to Impact RecipeUI**:
- No server-side components
- No ASP.NET dependencies
- No WCF dependencies
- No complex serialization detected

---

### Files Requiring Attention

The following 11 files contain API references flagged by analysis:

1. Monitor these files specifically during compilation
2. Review compiler output for errors in these files
3. Most issues will auto-resolve; flag any persistent errors

**Note**: File names not provided in assessment; identify during compilation phase.

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

Since this is a single-project upgrade, testing occurs in one comprehensive phase after the atomic upgrade operation.

---

### Phase Testing: Post-Atomic Upgrade

**Timing**: After SDK conversion, target framework update, and successful compilation

**Objective**: Verify RecipeUI.csproj builds and functions correctly on .NET 10

---

#### Level 1: Build Validation

**Criteria**:
- [ ] Project builds without errors
- [ ] Project builds without warnings (or warnings are acceptable and documented)
- [ ] All 24 source files compile successfully
- [ ] NuGet package restore succeeds
- [ ] Output executable is generated in bin folder

**Expected Issues**:
- Possibly System.Drawing.Common reference needed
- Possibly System.Configuration.ConfigurationManager reference needed

**Resolution**:
- Add missing package references as identified
- Rebuild until zero errors

---

#### Level 2: Smoke Tests (Quick Validation)

**Purpose**: Verify basic application functionality after upgrade

**Test Cases**:

1. **Application Launch**
   - [ ] Application starts without exceptions
   - [ ] Main form loads and displays
   - [ ] No missing assembly errors

2. **UI Rendering**
   - [ ] All forms display correctly
   - [ ] Controls are properly positioned
   - [ ] Labels show correct text
   - [ ] Buttons are visible and enabled
   - [ ] Images render (if PictureBox controls used)

3. **Basic Interactions**
   - [ ] Button clicks work
   - [ ] Form can be moved and resized
   - [ ] Dialog boxes open and close
   - [ ] ListView controls display (if used)

**Duration**: 5-10 minutes

**Abort Criteria**: If smoke tests fail, do not proceed to comprehensive testing

---

#### Level 3: Functional Tests (Comprehensive Validation)

**Purpose**: Validate all application features work as expected

**Test Cases**:

**Recipe Management**:
- [ ] Create new recipe
- [ ] Edit existing recipe
- [ ] Delete recipe
- [ ] View recipe details

**UI Controls**:
- [ ] TextBox input and validation
- [ ] ComboBox selection
- [ ] NumericUpDown controls
- [ ] ListView selection and display
- [ ] Button actions
- [ ] Form navigation

**Data Operations**:
- [ ] Data persists correctly
- [ ] JSON serialization/deserialization (JsonConverter packages)
- [ ] Validation works (Stef.Validation package)

**Dialog Interactions**:
- [ ] Modal dialogs (ShowDialog)
- [ ] Dialog results (OK, Cancel)
- [ ] Form closure

**Configuration** (if applicable):
- [ ] App settings load correctly
- [ ] Configuration values are accessible

---

#### Level 4: Regression Testing

**Purpose**: Ensure no functionality was lost in migration

**Approach**: Side-by-side comparison

1. **Run .NET Framework 4.8 version** (from `george` branch)
   - Document behavior and appearance
   - Capture screenshots if needed

2. **Run .NET 10 version** (from `upgrade-to-NET10` branch)
   - Compare behavior and appearance
   - Note any differences

**Comparison Checklist**:
- [ ] UI layout identical
- [ ] Colors and fonts match
- [ ] Image quality equivalent
- [ ] Performance acceptable (startup time, responsiveness)
- [ ] No new bugs introduced

**Acceptable Differences**:
- Slight rendering variations (font anti-aliasing, etc.)
- Minor performance improvements/changes
- Updated system dialogs (native .NET 10 dialogs)

**Unacceptable Differences**:
- Missing functionality
- Broken features
- Data loss
- Crashes or exceptions

---

#### Level 5: Performance Validation

**Purpose**: Ensure .NET 10 version performs acceptably

**Metrics**:
- [ ] Application startup time (< 5 seconds for typical Windows Forms app)
- [ ] UI responsiveness (no lag on button clicks)
- [ ] Memory usage (within reasonable limits)
- [ ] Form load time

**Comparison**: Compare with .NET Framework 4.8 baseline if available

**Expected**: .NET 10 should be comparable or faster

---

### Testing Checklist Summary

Use this consolidated checklist for the upgrade validation:

#### Build Phase
- [ ] Zero compilation errors
- [ ] Zero warnings (or documented acceptable warnings)
- [ ] All files compile
- [ ] Executable generated

#### Smoke Test Phase
- [ ] Application launches
- [ ] Main form displays
- [ ] Basic interactions work

#### Functional Test Phase  
- [ ] All recipe operations work
- [ ] All UI controls functional
- [ ] Data operations succeed
- [ ] Dialogs work correctly

#### Regression Phase
- [ ] Behavior matches .NET Framework version
- [ ] UI appearance equivalent
- [ ] No functionality lost

#### Performance Phase
- [ ] Startup time acceptable
- [ ] Responsiveness good
- [ ] Memory usage reasonable

---

### Test Environment

**Required**:
- Windows 10 or Windows 11
- .NET 10 SDK installed
- Visual Studio 2022 (17.12+) or .NET 10 runtime

**Recommended**:
- Test on clean Windows environment
- Test with different display scaling (100%, 125%, 150%)
- Test on both development and deployment machines

---

### Success Criteria for Testing

**Mandatory**:
- All build validation passes
- All smoke tests pass
- All functional tests pass
- No critical regressions identified

**Optional** (can be addressed post-migration):
- Minor UI rendering differences
- Performance optimizations
- Code quality improvements

---

### Issue Tracking During Testing

**For each issue found**:

1. **Document**:
   - Description
   - Steps to reproduce
   - Expected vs actual behavior
   - Severity (Critical, High, Medium, Low)

2. **Categorize**:
   - Build issue
   - Functional issue
   - UI issue
   - Performance issue

3. **Resolve**:
   - Critical/High: Must fix before completion
   - Medium: Should fix before completion
   - Low: Can defer to future enhancement

**Issue Log Location**: Track in GitHub issues or project tracking system

---

## Complexity & Effort Assessment

### Project Complexity Table

| Project | Complexity | Dependencies | Risk | Primary Challenge |
|---------|-----------|--------------|------|-------------------|
| RecipeUI.csproj | Medium | 0 projects, 4 packages | Medium | SDK conversion + Windows Forms API recompilation |

### Phase Complexity Assessment

**Phase 1: Atomic Upgrade**
- **Complexity**: Medium
- **Rationale**: 
  - SDK conversion is well-documented but requires verification
  - Large number of API issues (1,298) are expected and resolve through proper targeting
  - No actual code changes required for most issues (binary incompatibility resolves via recompilation)
  - Package updates not required (all compatible)

### Resource Requirements

**Skills Required:**
- .NET Framework to .NET Core/.NET migration experience
- Windows Forms application development knowledge
- SDK-style project format familiarity
- Understanding of Windows Desktop targeting in .NET

**Parallel Work Capacity:**
- Not applicable (single project)

**Dependencies on External Teams:**
- None

---

## Risk Management

### High-Level Risk Assessment

| Risk Factor | Level | Mitigation |
|-------------|-------|------------|
| Project conversion (SDK-style) | Medium | Use automated conversion tool; manual verification of project file |
| Windows Forms API compatibility | Low | APIs are binary incompatible but fully supported in .NET 10 with proper targeting |
| Package compatibility | Low | All packages already compatible |
| Code modifications required | Medium | 58.9% of code has API references; most will resolve via recompilation |

### Security Vulnerabilities
None detected in current package dependencies.

### Contingency Plans

**If SDK conversion fails:**
- Manual conversion using conversion guide
- Gradual migration of project file elements
- Create new SDK-style project and migrate files

**If compilation errors persist:**
- Identify specific API incompatibilities
- Check for System.Drawing.Common package requirement
- Verify net10.0-windows targeting (not just net10.0)
- Review Windows Desktop SDK enablement

**If runtime issues occur:**
- Check behavioral changes in System.Drawing APIs
- Test configuration system changes
- Validate Windows Forms event handling

---

## Source Control Strategy

### Branching Strategy

**Main Branch**: `george` (original development branch)

**Upgrade Branch**: `upgrade-to-NET10` (created and switched to during assessment)

**Workflow**:
1. ✅ **Complete**: Committed pending changes on `george` branch
2. ✅ **Complete**: Created and switched to `upgrade-to-NET10` branch
3. **In Progress**: Perform all upgrade work on `upgrade-to-NET10` branch
4. **Future**: Merge back to `george` after successful testing and validation

### Commit Strategy

**Approach**: Single atomic commit for the entire upgrade

**Rationale**:
- Single project upgrade
- All changes are interdependent (SDK conversion + targeting)
- Easier to review as one coherent change
- Easier to rollback if needed
- Matches All-At-Once strategy

**Recommended Commit Message**:
```
Upgrade RecipeUI from .NET Framework 4.8 to .NET 10

- Convert project to SDK-style format
- Update target framework to net10.0-windows
- Verify all packages compatible (no updates needed)
- All tests passing

Closes #[issue-number]
```

**Alternative Approach** (if preferred):
If you prefer more granular commits for easier review:

1. **Commit 1**: SDK-style conversion
   ```
   Convert RecipeUI.csproj to SDK-style format

   - Replace classic .NET Framework project format
   - Use implicit file globbing
   - Simplify project structure
   ```

2. **Commit 2**: Target framework update
   ```
   Update RecipeUI target framework to .NET 10

   - Change from net48 to net10.0-windows
   - Verify Windows Desktop SDK enabled
   - All packages remain compatible
   ```

3. **Commit 3**: Final verification
   ```
   Verify RecipeUI .NET 10 upgrade complete

   - Build succeeds
   - All tests pass
   - Application runs correctly
   ```

### Review and Merge Process

**Pre-Merge Checklist**:
- [ ] All commits on `upgrade-to-NET10` branch
- [ ] Build succeeds on `upgrade-to-NET10` branch
- [ ] All tests pass
- [ ] Code review completed (if team process requires)
- [ ] Testing validation checklist complete

**Pull Request** (if using PR workflow):
- **Title**: "Upgrade RecipeUI to .NET 10"
- **Description**: 
  - Link to assessment.md
  - Link to plan.md
  - Summary of changes
  - Test results
  - Any known issues or follow-up items

**Merge Criteria**:
- All validation tests pass
- No regression issues
- Documentation updated (if applicable)
- Team approval (if required)

**Merge Method**: 
- **Recommended**: Merge commit (preserves upgrade history)
- **Alternative**: Squash and merge (if team prefers linear history)

### Rollback Plan

**If upgrade fails or critical issues found**:

1. **Immediate Rollback**:
   ```bash
   git checkout george
   # Upgrade branch remains for investigation
   ```

2. **Investigate Issues**:
   - Review error logs
   - Identify root cause
   - Determine if fixable

3. **Retry or Abort**:
   - If fixable: Fix issues on `upgrade-to-NET10` branch and retest
   - If not fixable: Document issues, keep branch for future attempt

**Branch Cleanup** (after successful merge):
```bash
# After merge to george is complete and verified
git branch -d upgrade-to-NET10  # Delete local branch
git push origin --delete upgrade-to-NET10  # Delete remote branch (if pushed)
```

### Continuous Integration Considerations

**If CI/CD pipeline exists**:
- Update pipeline to build .NET 10 projects
- Ensure .NET 10 SDK available on build agents
- Update test execution for .NET 10 runtime
- Update deployment scripts for .NET 10 publish

**Pipeline Changes Required**:
```yaml
# Example: Update SDK version in CI config
sdk:
  version: 10.0.x  # Updated from 4.8
```

### Documentation Updates

**Files to Update Post-Merge**:
- [ ] README.md (update .NET version requirements)
- [ ] CONTRIBUTING.md (update build instructions)
- [ ] Deployment documentation (update runtime requirements)
- [ ] Any developer setup guides

**Version Requirements**:
- .NET 10 SDK for development
- .NET 10 Runtime for deployment  
- Visual Studio 2022 17.12+ for development
- Windows 10/11 for running the application

---

## Success Criteria

### Technical Criteria

The .NET 10 upgrade is technically successful when:

#### Build Success
- [x] .NET 10 SDK installed and verified
- [ ] RecipeUI.csproj converted to SDK-style format
- [ ] Project targets `net10.0-windows`
- [ ] Project builds without errors
- [ ] Project builds without warnings (or warnings documented and acceptable)
- [ ] All 24 source files compile successfully
- [ ] Executable generated in output directory

#### Package Compatibility
- [ ] All 4 NuGet packages remain at current versions:
  - JsonConverter.Abstractions 0.8.0
  - JsonConverter.Newtonsoft.Json 0.8.0
  - Newtonsoft.Json 13.0.4
  - Stef.Validation 0.1.1
- [ ] Package restore succeeds
- [ ] No package version conflicts
- [ ] No security vulnerabilities in packages

#### API Compatibility
- [ ] Windows Forms APIs compile successfully (1,244 binary incompatibility issues resolved via targeting)
- [ ] System.Drawing APIs compile successfully (45 source incompatibility issues resolved)
- [ ] Configuration system works (2 legacy config API issues addressed)
- [ ] Behavioral changes tested and validated (9 issues)
- [ ] No compilation errors from API incompatibilities

#### Runtime Success
- [ ] Application launches without exceptions
- [ ] Main form displays correctly
- [ ] All UI controls render properly
- [ ] All functionality works as expected
- [ ] No missing assembly errors
- [ ] No runtime crashes

---

### Quality Criteria

The upgrade maintains or improves code quality when:

#### Code Quality Maintained
- [ ] No new code smells introduced
- [ ] Code structure preserved (SDK conversion doesn't alter logic)
- [ ] Naming conventions maintained
- [ ] File organization preserved

#### Test Coverage Maintained
- [ ] All existing functionality tested
- [ ] Smoke tests pass
- [ ] Functional tests pass
- [ ] Regression tests show no degradation

#### Documentation Updated
- [ ] README reflects .NET 10 requirements
- [ ] Build instructions updated
- [ ] Deployment documentation current
- [ ] Developer setup guide updated

---

### Process Criteria

The upgrade process is complete when:

#### Strategy Followed
- [x] Assessment completed (assessment.md generated)
- [ ] Plan created and reviewed (plan.md - this document)
- [ ] All-At-Once strategy applied (single atomic upgrade)
- [ ] Atomic upgrade executed (SDK conversion + targeting in one operation)
- [ ] Validation testing completed

#### Source Control Followed
- [x] Working on `upgrade-to-NET10` branch
- [ ] Changes committed with clear messages
- [ ] Pull request created (if team process requires)
- [ ] Code review completed (if team process requires)
- [ ] Merged to `george` branch after validation

#### All-At-Once Strategy Principles Applied
- [ ] Single project upgraded atomically
- [ ] SDK conversion and target framework update done together
- [ ] All validation performed before declaring complete
- [ ] No intermediate states left uncommitted

---

### Validation Gates

Progress through these gates in order:

#### Gate 1: Build Gate ✅
**Criteria**: Project builds successfully  
**Actions if Failed**: 
- Review compiler errors
- Add missing package references
- Fix project file syntax
- Re-attempt build

#### Gate 2: Launch Gate ✅
**Criteria**: Application launches without errors  
**Actions if Failed**:
- Check for missing assemblies
- Verify runtime dependencies
- Review exception details
- Fix and rebuild

#### Gate 3: Functional Gate ✅
**Criteria**: All application features work  
**Actions if Failed**:
- Identify broken functionality
- Review breaking changes catalog
- Apply necessary fixes
- Retest

#### Gate 4: Regression Gate ✅
**Criteria**: No degradation vs .NET Framework 4.8 version  
**Actions if Failed**:
- Document regressions
- Investigate root causes
- Apply fixes
- Re-validate

#### Gate 5: Merge Gate ✅
**Criteria**: All above gates passed + team approval  
**Actions if Passed**: Merge to `george` branch  
**Actions if Failed**: Continue fixing until all criteria met

---

### Definition of Done

The RecipeUI .NET 10 upgrade is **DONE** when:

✅ **All Technical Criteria met**  
✅ **All Quality Criteria met**  
✅ **All Process Criteria met**  
✅ **All Validation Gates passed**  
✅ **Changes merged to main development branch (`george`)**  
✅ **Team sign-off received (if required)**

---

### Post-Upgrade Enhancements (Optional)

These are **not required** for upgrade completion but are recommended future improvements:

#### Configuration Modernization
- Migrate from app.config to appsettings.json
- Adopt Microsoft.Extensions.Configuration
- Use dependency injection for configuration

#### Code Improvements
- Adopt async/await patterns where beneficial
- Use modern C# language features (records, pattern matching, etc.)
- Consider nullable reference types

#### Performance Optimization
- Profile application performance
- Optimize slow code paths
- Leverage .NET 10 performance improvements

#### Deployment Modernization
- Create single-file deployment
- Use self-contained deployment option
- Implement ClickOnce or MSIX packaging

**Note**: These enhancements should be tracked as separate work items, not part of the core upgrade.
