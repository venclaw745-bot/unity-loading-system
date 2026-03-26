// ═══════════════════════════════════════════════════════════════
// TaskLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace LoadingSystem.Operations
{
    /// <summary>
    /// Wraps an arbitrary <see cref="UniTask"/> as a binary (0 or 1) loading operation.
    /// </summary>
    public sealed class TaskLoadingOperation : LoadingOperationBase
    {
        private readonly Func<CancellationToken, UniTask> _taskFactory;

        public TaskLoadingOperation(Func<CancellationToken, UniTask> taskFactory)
        {
            _taskFactory = taskFactory;
        }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            ReportProgress(0f);
            await _taskFactory(cancellationToken);
        }
    }
    
    /// <summary>
    /// Wraps an arbitrary <see cref="UniTask{T}"/> and exposes its result.
    /// </summary>
    public sealed class TaskLoadingOperation<TResult> : LoadingOperationBase, ILoadingOperation<TResult>
    {
        private readonly Func<CancellationToken, UniTask<TResult>> _taskFactory;

        public TaskLoadingOperation(Func<CancellationToken, UniTask<TResult>> taskFactory)
        {
            _taskFactory = taskFactory;
        }

        public TResult Result { get; private set; }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            ReportProgress(0f);
            Result = await _taskFactory(cancellationToken);
        }
    }
}