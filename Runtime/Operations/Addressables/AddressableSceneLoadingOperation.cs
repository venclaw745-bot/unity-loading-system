// ═══════════════════════════════════════════════════════════════
// AddressableSceneLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
#if UNITY_ADDRESSABLES
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace LoadingSystem.Operations
{
    public sealed class AddressableSceneLoadingOperation : LoadingOperationBase, IDisposable
    {
        private readonly object _key;
        private readonly LoadSceneMode _loadMode;
        private readonly bool _activateOnLoad;
        private AsyncOperationHandle<SceneInstance> _handle;

        public AddressableSceneLoadingOperation(
            object key,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            bool activateOnLoad = true)
        {
            _key = key;
            _loadMode = loadMode;
            _activateOnLoad = activateOnLoad;
        }

        public SceneInstance SceneInstance { get; private set; }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            _handle = Addressables.LoadSceneAsync(_key, _loadMode, _activateOnLoad);

            while (!_handle.IsDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ReportProgress(_handle.PercentComplete);
                await UniTask.Yield(cancellationToken);
            }

            if (_handle.Status == AsyncOperationStatus.Failed)
                throw _handle.OperationException
                    ?? new Exception($"Addressable scene load failed for key '{_key}'.");

            SceneInstance = _handle.Result;
        }

        public async UniTask UnloadAsync()
        {
            if (_handle.IsValid())
                await Addressables.UnloadSceneAsync(_handle).Task;
        }

        public void Dispose()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
        }
    }
}
#endif