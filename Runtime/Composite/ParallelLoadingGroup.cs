// ═══════════════════════════════════════════════════════════════
// ParallelLoadingGroup.cs
// ═══════════════════════════════════════════════════════════════
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem
{
    /// <summary>
    /// Executes multiple operations in parallel.
    /// Progress = weighted average of all child progresses.
    /// </summary>
    public sealed class ParallelLoadingGroup : ILoadingOperation
    {
        private readonly List<WeightedOperation> _operations;
        private readonly float _totalWeight;

        public ParallelLoadingGroup(IEnumerable<WeightedOperation> operations)
        {
            _operations = operations.ToList();
            _totalWeight = _operations.Sum(o => o.Weight);
        }

        public float Progress
        {
            get
            {
                if (_totalWeight <= 0f) return 1f;

                float weightedSum = 0f;
                foreach (var op in _operations)
                    weightedSum += op.Operation.Progress * op.Weight;
                return Mathf.Clamp01(weightedSum / _totalWeight);
            }
        }

        public bool IsCompleted => _operations.All(o => o.Operation.IsCompleted);

        public async UniTask Execute(CancellationToken cancellationToken = default)
        {
            var tasks = _operations.Select(o => o.Operation.Execute(cancellationToken));
            await UniTask.WhenAll(tasks);
        }
    }
}