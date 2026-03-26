// ═══════════════════════════════════════════════════════════════
// LoadingProgressTracker.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace LoadingSystem
{
    /// <summary>
    /// Monitors and reports progress of a loading operation.
    /// Can be attached to a UI component to display progress.
    /// </summary>
    public class LoadingProgressTracker : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private float _smoothTime = 0.3f;

        [Header("Events")]
        public UnityEvent<float> OnProgressChanged;
        public UnityEvent<string> OnStatusChanged;
        public UnityEvent OnCompleted;
        public UnityEvent OnCancelled;
        public UnityEvent<Exception> OnFailed;

        private ILoadingOperation _currentOperation;
        private CancellationTokenSource _cancellationTokenSource;
        private float _currentProgress;
        private float _displayProgress;
        private float _velocity;

        public float Progress => _displayProgress;
        public bool IsRunning { get; private set; }
        public bool IsCompleted => _currentOperation?.IsCompleted ?? false;

        private void Start()
        {
            if (_autoStart && _currentOperation != null)
            {
                StartTracking(_currentOperation);
            }
        }

        private void Update()
        {
            if (!IsRunning || _currentOperation == null) return;

            // Smooth progress display
            _displayProgress = Mathf.SmoothDamp(
                _displayProgress, 
                _currentOperation.Progress, 
                ref _velocity, 
                _smoothTime
            );

            OnProgressChanged?.Invoke(_displayProgress);
        }

        private void OnDestroy()
        {
            Cancel();
        }

        /// <summary>
        /// Start tracking a loading operation.
        /// </summary>
        public void StartTracking(ILoadingOperation operation)
        {
            if (IsRunning)
            {
                Debug.LogWarning("Already tracking an operation. Cancel first.");
                return;
            }

            _currentOperation = operation;
            _cancellationTokenSource = new CancellationTokenSource();
            IsRunning = true;

            ExecuteOperationAsync().Forget();
        }

        /// <summary>
        /// Cancel the current loading operation.
        /// </summary>
        public void Cancel()
        {
            if (!IsRunning) return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            IsRunning = false;

            OnCancelled?.Invoke();
        }

        private async UniTaskVoid ExecuteOperationAsync()
        {
            try
            {
                OnStatusChanged?.Invoke("Loading started...");
                
                await _currentOperation.Execute(_cancellationTokenSource.Token);
                
                OnStatusChanged?.Invoke("Loading completed!");
                OnCompleted?.Invoke();
            }
            catch (OperationCanceledException)
            {
                OnStatusChanged?.Invoke("Loading cancelled");
                OnCancelled?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Loading failed: {e}");
                OnStatusChanged?.Invoke($"Loading failed: {e.Message}");
                OnFailed?.Invoke(e);
            }
            finally
            {
                IsRunning = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Creates a simple loading pipeline for demonstration.
        /// </summary>
        public static LoadingPipeline CreateDemoPipeline()
        {
            var pipeline = new LoadingPipeline();
            
            // Simulate some loading steps
            pipeline.Add(new DelayOperation(1f), 0.2f)  // Initial delay
                   .Combine(
                       new DelayOperation(2f),
                       new DelayOperation(1.5f)
                   )  // Parallel operations
                   .Add(new DelayOperation(0.5f), 0.3f);  // Final step

            return pipeline;
        }
    }
}