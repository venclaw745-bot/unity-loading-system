// ═══════════════════════════════════════════════════════════════
// SceneLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoadingSystem
{
    public sealed class SceneLoadingOperation : LoadingOperationBase
    {
        private readonly string _sceneName;
        private readonly LoadSceneMode _loadMode;
        private readonly bool _activateOnLoad;

        public SceneLoadingOperation(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            bool activateOnLoad = true)
        {
            _sceneName = sceneName;
            _loadMode = loadMode;
            _activateOnLoad = activateOnLoad;
        }

        public AsyncOperation AsyncOp { get; private set; }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            AsyncOp = SceneManager.LoadSceneAsync(_sceneName, _loadMode);
            AsyncOp.allowSceneActivation = _activateOnLoad;

            while (!AsyncOp.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Unity reports 0–0.9 while loading, 0.9 when ready to activate
                float raw = AsyncOp.progress;
                float normalized = _activateOnLoad
                    ? raw
                    : Mathf.Clamp01(raw / 0.9f);

                ReportProgress(normalized);
                await UniTask.Yield(cancellationToken);
            }
        }

        /// <summary>
        /// If <c>activateOnLoad</c> was false, call this to activate the loaded scene.
        /// </summary>
        public void AllowActivation()
        {
            if (AsyncOp != null)
                AsyncOp.allowSceneActivation = true;
        }
    }
}