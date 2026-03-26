# PROJECT-INDEX.md - Unity Loading Progress System

## Project Overview

**Name:** Unity Loading Progress System  
**Type:** Unity Package (Library)  
**Status:** Published to GitHub  
**Repository:** https://github.com/venclaw745-bot/unity-loading-system  
**Created:** 2026-03-26  
**Last Updated:** 2026-03-26  

## Description

A professional, flexible, and extensible loading system for Unity that provides fine-grained control over loading operations with progress tracking, parallel execution, weighted progress calculation, and seamless integration with Unity's async systems.

## Key Features

- **🎯 Modular Design** - Clean interfaces and base classes for easy extension
- **⚡ Parallel & Sequential Execution** - Combine operations in any configuration
- **⚖️ Weighted Progress** - Assign weights to operations for accurate progress calculation
- **🚫 Cancellation Support** - Proper cancellation token integration
- **🎨 UI Integration** - Ready-to-use UI components for progress display
- **🔧 Editor Tools** - Visual pipeline editor for debugging and testing
- **📦 Multiple Backends** - Support for Scenes, Resources, Addressables, and custom operations
- **🔄 Async/Await** - Built on UniTask for modern async programming

## Installation

### Via Unity Package Manager (Git URL)

1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **"Add package from git URL..."**
3. Enter the following URL:
   ```
   https://github.com/venclaw745-bot/unity-loading-system.git
   ```
4. Click **Add**

### Via manifest.json (Recommended for Teams)

Add this to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.ven.loadingprogress": "https://github.com/venclaw745-bot/unity-loading-system.git",
    "com.cysharp.unitask": "2.3.3"
  }
}
```

### Dependencies

- **Unity 2020.3** or later
- **UniTask 2.3.3** or later (automatically installed via Package Manager)
- **Addressables** (optional, for Addressable operations)

## Project Structure

```
unity-loading-system/
├── package.json                    # Unity Package Manager configuration
├── README.md                       # Comprehensive documentation
├── LICENSE                         # MIT License
├── Runtime/                        # Runtime code
│   ├── Core/                       # Core interfaces and base classes
│   │   ├── ILoadingOperation.cs
│   │   ├── LoadingOperationBase.cs
│   │   ├── WeightedOperation.cs
│   │   ├── Loading.cs
│   │   └── LoadingOperationExtensions.cs
│   ├── Composite/                  # Composite operations
│   │   ├── LoadingPipeline.cs
│   │   └── ParallelLoadingGroup.cs
│   ├── Operations/                 # Concrete operation implementations
│   │   ├── SceneLoadingOperation.cs
│   │   ├── ResourceLoadingOperation.cs
│   │   ├── AsyncOperationLoadingOperation.cs
│   │   ├── TaskLoadingOperation.cs
│   │   ├── CallbackLoadingOperation.cs
│   │   ├── SimulatedLoadingOperation.cs
│   │   ├── CompletedLoadingOperation.cs
│   │   ├── SequenceLoadingOperation.cs
│   │   ├── AssetBundleLoadingOperation.cs
│   │   └── Addressables/           # Addressable system operations
│   └── UI/                         # UI components
│       ├── LoadingProgressTracker.cs
│       └── LoadingProgressBar.cs
├── Editor/                         # Editor tools
│   └── LoadingPipelineEditor.cs
└── Samples~/                       # Usage examples
    └── BasicExample/
        └── ExampleUsage.cs
```

## Quick Start Example

```csharp
using LoadingSystem;
using LoadingSystem.Operations;
using Cysharp.Threading.Tasks;

// Create a pipeline
var pipeline = Loading.Create();

// Add operations with weights
pipeline.Add(new SceneLoadingOperation("MainMenu"), 0.3f)
        .Add(new SimulatedLoadingOperation(1f), 0.2f)
        .Add(new ResourceLoadingOperation<Texture2D>("Background"), 0.5f);

// Execute
await pipeline.Execute(cancellationToken);
```

## Development History

### 2026-03-26: Initial Creation
- **Created:** Complete loading system based on user requirements
- **Structured:** Organized as Unity Package Manager package
- **Published:** Pushed to GitHub as public repository
- **Documented:** Comprehensive README and samples

### GitHub Push Issues
During initial publication, encountered authentication issues:
- **Problem:** Token authentication failures with empty remote repository
- **Solution:** Created fresh repository with proper credential configuration
- **Documentation:** See `docs/github-push-issues.md` for detailed analysis

## Usage Scenarios

### Game Initialization
```csharp
var initPipeline = Loading.Create()
    .Add(LoadConfig(), 0.1f)
    .Combine(LoadEssentialAssets(), 0.6f)
    .Add(InitializeSystems(), 0.3f);
```

### Level Loading
```csharp
var levelPipeline = Loading.Create()
    .Combine(
        new SceneLoadingOperation("Level_01"),
        new AddressableAssetLoadingOperation<GameObject>("level_assets")
    )
    .Add(SpawnPlayers(), 0.1f);
```

### Custom Operations
```csharp
public class DatabaseLoadingOperation : LoadingOperationBase
{
    protected override async UniTask OnExecute(CancellationToken cancellationToken)
    {
        // Your database loading logic
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.Delay(10, cancellationToken: cancellationToken);
            ReportProgress(i / 100f);
        }
    }
}
```

## Editor Tools

Access the pipeline visualizer via: `Tools → Loading System → Pipeline Visualizer`

This tool helps:
- Visualize pipeline structure
- Debug progress calculation
- Test pipeline configurations
- Generate sample pipelines

## Related Documentation

- **GitHub Push Issues:** `docs/github-push-issues.md` - Analysis of authentication problems
- **Unity Package Manager:** Official Unity documentation
- **UniTask Documentation:** https://github.com/Cysharp/UniTask

## Maintenance Notes

- **Package Updates:** Update version in `package.json`
- **Dependencies:** Keep UniTask version current
- **Testing:** Test with Unity 2020.3+ versions
- **Documentation:** Keep README.md updated with new features

## Future Enhancements

1. **More Operation Types:** Web request operations, file I/O operations
2. **Advanced UI:** More progress bar styles, animation presets
3. **Performance Profiling:** Built-in performance metrics
4. **Visual Scripting:** Unity Visual Scripting integration
5. **Build Integration:** Automated build pipeline integration

## Contact & Support

- **GitHub Issues:** https://github.com/venclaw745-bot/unity-loading-system/issues
- **Repository:** https://github.com/venclaw745-bot/unity-loading-system
- **License:** MIT (see LICENSE file)

---
**Maintained by:** VClaw  
**Project Type:** Unity Package  
**Status:** Active / Published