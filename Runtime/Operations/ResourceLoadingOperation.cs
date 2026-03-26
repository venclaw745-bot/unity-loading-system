// ═══════════════════════════════════════════════════════════════
// ResourceLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem.Operations
{
    public sealed class ResourceLoadingOperation<T> : LoadingOperationBase, ILoadingOperation<T>
        where T : Object
    {
        private readonly string _path;

        public ResourceLoadingOperation(string path)
        {
            _path = path;
        }

        public T Result { get; private set; }

        protected override async UniTask OnExecute(CancellationToken cancellationToken)
        {
            var request = Resources.LoadAsync<T>(_path);

            while (!request.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ReportProgress(request.progress);
                await UniTask.Yield(cancellationToken);
            }

            Result = request.asset as T;

            if (Result == null)
                throw new System.Exception($"Failed to load resource at path '{_path}' as {typeof(T).Name}.");
        }
    }
}