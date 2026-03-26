// ═══════════════════════════════════════════════════════════════
// CallbackLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace LoadingSystem
{
    /// <summary>
    /// Allows external code to manually drive progress via <see cref="IProgress{T}"/>.
    /// The factory receives a progress reporter and a cancellation token.
    /// </summary>
    public sealed class CallbackLoadingOperation : LoadingOperationBase
    {
        private readonly Func<IProgress<float>, CancellationToken, UniTask> _taskFactory;

        public CallbackLoadingOperation(Func<IProgress<float>, CancellationToken, UniTask> taskFactory)
        {
            _taskFactory = taskFactory;
        }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            var reporter = new Progress<float>(ReportProgress);
            await _taskFactory(reporter, cancellationToken);
        }
    }
}