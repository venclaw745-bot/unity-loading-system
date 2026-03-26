// ═══════════════════════════════════════════════════════════════
// LoadingOperationBase.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem
{
    public abstract class LoadingOperationBase : ILoadingOperation
    {
        private float _progress;
        private bool _started;

        public float Progress => _progress;
        public bool IsCompleted { get; private set; }

        protected void ReportProgress(float progress)
        {
            _progress = Mathf.Clamp01(progress);
        }

        public async UniTask Execute(CancellationToken cancellationToken = default)
        {
            if (_started)
                throw new InvalidOperationException(
                    $"Loading operation [{GetType().Name}] has already been started.");

            _started = true;

            try
            {
                await OnExecute(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError($"Loading operation [{GetType().Name}] failed: {e}");
                throw;
            }
            finally
            {
                _progress = 1f;
                IsCompleted = true;
            }
        }

        protected abstract UniTask OnExecute(CancellationToken cancellationToken);
    }
}