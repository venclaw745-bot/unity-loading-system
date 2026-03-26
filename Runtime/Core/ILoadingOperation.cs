// ═══════════════════════════════════════════════════════════════
// ILoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Threading;
using Cysharp.Threading.Tasks;

namespace LoadingSystem
{
    public interface ILoadingOperation
    {
        float Progress { get; }
        bool IsCompleted { get; }
        UniTask Execute(CancellationToken cancellationToken = default);
    }

    public interface ILoadingOperation<out TResult> : ILoadingOperation
    {
        TResult Result { get; }
    }
}