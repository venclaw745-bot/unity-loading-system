// ═══════════════════════════════════════════════════════════════
// SimulatedLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem.Operations
{
    /// <summary>
    /// Simulates loading over a fixed duration.
    /// Useful for enforcing a minimum loading screen display time
    /// or for placeholder/debug operations.
    /// </summary>
    public sealed class SimulatedLoadingOperation : LoadingOperationBase
    {
        private readonly float _durationSeconds;
        private readonly AnimationCurve _progressCurve;

        public SimulatedLoadingOperation(float durationSeconds)
        {
            _durationSeconds = durationSeconds;
            _progressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }

        public SimulatedLoadingOperation(float durationSeconds, AnimationCurve progressCurve)
        {
            _durationSeconds = durationSeconds;
            _progressCurve = progressCurve;
        }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            float elapsed = 0f;

            while (elapsed < _durationSeconds)
            {
                cancellationToken.ThrowIfCancellationRequested();
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _durationSeconds);
                ReportProgress(_progressCurve.Evaluate(t));
                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate, cancellationToken);
            }
        }
    }
}