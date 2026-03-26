using System.Threading;
using Cysharp.Threading.Tasks;
using LoadingSystem;
using UnityEngine;

namespace LoadingSystem.Samples
{
    public class ExampleUsage : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string _sceneToLoad = "GameScene";
        [SerializeField] private string _resourcePath = "Prefabs/MainPlayer";
        
        [Header("UI Reference")]
        [SerializeField] private LoadingProgressTracker _progressTracker;

        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            // Create a cancellation token source for this loading session
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Start the loading process
            StartLoadingAsync().Forget();
        }

        private void OnDestroy()
        {
            // Clean up cancellation token
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private async UniTaskVoid StartLoadingAsync()
        {
            Debug.Log("Starting loading process...");
            
            // Create a loading pipeline using the static entry point
            var pipeline = Loading.Create();
            
            // Step 1: Initial setup (10% weight)
            pipeline.Add(new SimulatedLoadingOperation(0.5f), 0.1f);
            
            // Step 2: Load resources in parallel (60% weight)
            pipeline.Combine(
                new ResourceLoadingOperation<GameObject>(_resourcePath).WithWeight(2f),
                new SimulatedLoadingOperation(1.5f).WithWeight(1f)
            );
            
            // Step 3: Load the scene (30% weight)
            pipeline.Add(new SceneLoadingOperation(_sceneToLoad), 0.3f);
            
            // If we have a progress tracker, use it
            if (_progressTracker != null)
            {
                _progressTracker.StartTracking(pipeline);
                
                // Wait for completion through the tracker
                while (!_progressTracker.IsCompleted)
                {
                    await UniTask.Yield();
                }
            }
            else
            {
                // Execute directly
                await pipeline.Execute(_cancellationTokenSource.Token);
            }
            
            Debug.Log("Loading completed!");
            
            // Here you would typically transition to the next game state
            OnLoadingComplete();
        }

        private void OnLoadingComplete()
        {
            Debug.Log("All loading operations completed successfully!");
            
            // Example: Enable game UI, spawn player, etc.
            // GameManager.Instance.StartGame();
        }

        // Public method to cancel loading (can be called from UI button)
        public void CancelLoading()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                Debug.Log("Loading cancelled by user");
                _cancellationTokenSource.Cancel();
            }
        }
    }
}