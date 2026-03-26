// ═══════════════════════════════════════════════════════════════
// AsyncOperationLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem.Operations
{
    /// <summary>
    /// Wraps any <see cref="UnityEngine.AsyncOperation"/> as a loading operation.
    /// </summary>
    public sealed class AsyncOperationLoadingOperation : LoadingOperationBase
    {
        private readonly Func<AsyncOperation> _operationFactory;

        public AsyncOperationLoadingOperation(Func<AsyncOperation> operationFactory)
        {
            _operationFactory = operationFactory;
        }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            var asyncOp = _operationFactory.Invoke();

            while (!asyncOp.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ReportProgress(asyncOp.progress);
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}