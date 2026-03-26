// ═══════════════════════════════════════════════════════════════
// AssetBundleLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem
{
    /// <summary>
    /// Loads an AssetBundle asynchronously with progress tracking.
    /// </summary>
    public class AssetBundleLoadingOperation : LoadingOperationBase
    {
        private readonly string _bundleName;
        private readonly string _assetPath;
        private AssetBundleCreateRequest _request;

        public AssetBundle Bundle { get; private set; }

        public AssetBundleLoadingOperation(string bundleName, string assetPath = null)
        {
            _bundleName = bundleName;
            _assetPath = assetPath;
        }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            // Load AssetBundle
            _request = AssetBundle.LoadFromFileAsync(_bundleName);
            
            while (!_request.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                    
                ReportProgress(_request.progress);
                await UniTask.Yield();
            }

            Bundle = _request.assetBundle;
            
            if (Bundle == null)
            {
                Debug.LogError($"Failed to load AssetBundle: {_bundleName}");
                throw new System.IO.FileNotFoundException($"AssetBundle not found: {_bundleName}");
            }

            ReportProgress(1f);
        }
    }
}