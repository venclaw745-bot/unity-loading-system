// ═══════════════════════════════════════════════════════════════
// AddressableAssetLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LoadingSystem.Operations
{
    public sealed class AddressableAssetLoadingOperation<T> : LoadingOperationBase, ILoadingOperation<T>, IDisposable
    {
        private readonly object _key;
        private AsyncOperationHandle<T> _handle;

        public AddressableAssetLoadingOperation(object key)
        {
            _key = key;
        }

        public T Result { get; private set; }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            _handle = Addressables.LoadAssetAsync<T>(_key);

            while (!_handle.IsDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ReportProgress(_handle.PercentComplete);
                await UniTask.Yield(cancellationToken);
            }

            if (_handle.Status == AsyncOperationStatus.Failed)
                throw _handle.OperationException
                    ?? new Exception($"Addressable load failed for key '{_key}'.");

            Result = _handle.Result;
        }

        public void Dispose()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
        }
    }
}