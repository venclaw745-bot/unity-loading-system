# Advanced Loading Progress Tracking System for Unity

[![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)](https://unity3d.com)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A professional, flexible, and extensible loading system for Unity that provides fine-grained control over loading operations with progress tracking, parallel execution, weighted progress calculation, and seamless integration with Unity's async systems.

## ✨ Features

- **🎯 Modular Design** - Clean interfaces and base classes for easy extension
- **⚡ Parallel & Sequential Execution** - Combine operations in any configuration
- **⚖️ Weighted Progress** - Assign weights to operations for accurate progress calculation
- **🚫 Cancellation Support** - Proper cancellation token integration
- **🎨 UI Integration** - Ready-to-use UI components for progress display
- **🔧 Editor Tools** - Visual pipeline editor for debugging and testing
- **📦 Multiple Backends** - Support for Scenes, Resources, Addressables, and custom operations
- **🔄 Async/Await** - Built on UniTask for modern async programming

## 📦 Installation

### Via Unity Package Manager (Git URL)

1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **"Add package from git URL..."**
3. Enter the following URL:
   ```
   https://github.com/venclaw745-bot/unity-loading-system.git
   ```
4. Click **Add**

**Note:** If you previously imported the package and got ".meta file" errors, you may need to:
1. Remove the package from your project
2. Clear the Unity Package Manager cache (or wait a few minutes)
3. Re-add the package using the URL above

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

This package requires:
- **UniTask** (automatically installed via package.json)
- **Unity Addressables** (optional, required for Addressables operations)

**Addressables Support:**
- The Addressables operations are wrapped in `#if UNITY_ADDRESSABLES` directives
- If you don't have Addressables installed, these operations will be excluded from compilation
- To use Addressables operations, install the Addressables package via Package Manager
- No compilation errors will occur if Addressables is not installed

- **Unity 2020.3** or later
- **UniTask 2.3.3** or later (automatically installed via Package Manager)
- **Addressables** (optional, for Addressable operations)

## 🚀 Quick Start

### 1. Basic Pipeline

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

### 2. Parallel Operations

```csharp
// Load multiple assets in parallel
pipeline.Combine(
    new ResourceLoadingOperation<GameObject>("Prefabs/Player"),
    new ResourceLoadingOperation<AudioClip>("Audio/Music"),
    new ResourceLoadingOperation<Material>("Materials/Environment")
);
```

### 3. UI Integration

```csharp
// Attach LoadingProgressTracker to a GameObject
var tracker = GetComponent<LoadingProgressTracker>();
tracker.StartTracking(pipeline);

// LoadingProgressBar will automatically update
```

## 📁 Package Structure

```
Runtime/
├── Core/
│   ├── ILoadingOperation.cs          # Core interfaces
│   ├── LoadingOperationBase.cs       # Abstract base class
│   ├── WeightedOperation.cs          # Weighted operation struct
│   ├── Loading.cs                    # Static entry point
│   └── LoadingOperationExtensions.cs # Extension methods
├── Composite/
│   ├── LoadingPipeline.cs            # Sequential pipeline with builder API
│   └── ParallelLoadingGroup.cs       # Parallel execution group
├── Operations/
│   ├── SceneLoadingOperation.cs      # Unity scene loading
│   ├── ResourceLoadingOperation.cs   # Resources.LoadAsync wrapper
│   ├── AsyncOperationLoadingOperation.cs
│   ├── TaskLoadingOperation.cs       # UniTask wrapper
│   ├── CallbackLoadingOperation.cs   # Manual progress reporting
│   ├── SimulatedLoadingOperation.cs  # Simulated loading with curves
│   ├── CompletedLoadingOperation.cs  # No-op completed operation
│   ├── SequenceLoadingOperation.cs   # Simple sequential operations
│   └── Addressables/                 # Addressable system support
├── UI/
│   ├── LoadingProgressTracker.cs     # Progress monitoring component
│   └── LoadingProgressBar.cs         # UI progress display
Editor/
└── LoadingPipelineEditor.cs          # Pipeline visualization tool
Samples~/BasicExample/                # Usage examples
```

## 🔧 Built-in Operations

### Scene Loading
```csharp
var sceneOp = new SceneLoadingOperation("Level1", LoadSceneMode.Single, false);
// Control activation manually
sceneOp.AllowActivation();
```

### Resource Loading
```csharp
var resourceOp = new ResourceLoadingOperation<Texture2D>("Textures/Background");
var texture = resourceOp.Result;
```

### Addressables
```csharp
var addressableOp = new AddressableAssetLoadingOperation<GameObject>("PlayerPrefab");
using (addressableOp) // Dispose to release handle
{
    await addressableOp.Execute(cancellationToken);
    var prefab = addressableOp.Result;
}
```

### Simulated Loading
```csharp
// Linear progress
var linearOp = new SimulatedLoadingOperation(2f);

// Custom progress curve
var curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
var curvedOp = new SimulatedLoadingOperation(2f, curve);
```

### Task Wrappers
```csharp
// Wrap any UniTask
var taskOp = new TaskLoadingOperation(ct => SomeAsyncMethod(ct));

// Wrap UniTask with result
var taskWithResult = new TaskLoadingOperation<string>(ct => LoadDataAsync(ct));
var data = taskWithResult.Result;
```

### Manual Progress Reporting
```csharp
var callbackOp = new CallbackLoadingOperation((progress, ct) =>
{
    for (int i = 0; i < 100; i++)
    {
        ct.ThrowIfCancellationRequested();
        progress.Report(i / 100f);
        await UniTask.Delay(10, cancellationToken: ct);
    }
});
```

## 🎯 Custom Operations

Create your own loading operations by extending `LoadingOperationBase`:

```csharp
public class DatabaseLoadingOperation : LoadingOperationBase
{
    protected override async UniTask OnExecute(CancellationToken cancellationToken)
    {
        // Your database loading logic
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Simulate database query
            await UniTask.Delay(10, cancellationToken: cancellationToken);
            ReportProgress(i / 100f);
        }
    }
}
```

## 🛠️ Editor Tools

Access the pipeline visualizer via: `Tools → Loading System → Pipeline Visualizer`

This tool helps:
- Visualize pipeline structure
- Debug progress calculation
- Test pipeline configurations
- Generate sample pipelines

## 📚 Examples

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

### Splash Screen
```csharp
var splashPipeline = Loading.Create()
    .Add(new SimulatedLoadingOperation(2f, AnimationCurve.EaseInOut(0,0,1,1)), 1f)
    .Add(new SceneLoadingOperation("MainMenu"), 0f); // Zero weight - doesn't affect progress
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built on [UniTask](https://github.com/Cysharp/UniTask) by Cysharp
- Inspired by modern async/await patterns in Unity
- Community feedback and contributions

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/venclaw745-bot/unity-loading-system/issues)
- **Discussions**: [GitHub Discussions](https://github.com/venclaw745-bot/unity-loading-system/discussions)

---

**Made with ❤️ for the Unity community**