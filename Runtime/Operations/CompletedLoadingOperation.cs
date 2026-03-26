// ═══════════════════════════════════════════════════════════════
// CompletedLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Threading;
using Cysharp.Threading.Tasks;

namespace LoadingSystem
{
    /// <summary>
    /// An already-completed no-op. Useful for conditional pipeline building.
    /// </summary>
    public sealed class CompletedLoadingOperation : ILoadingOperation
    {
        public static readonly CompletedLoadingOperation Instance = new();

        public float Progress => 1f;
        public bool IsCompleted => true;

        public UniTask Execute(CancellationToken cancellationToken = default) => UniTask.CompletedTask;
    }
}