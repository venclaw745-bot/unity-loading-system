// ═══════════════════════════════════════════════════════════════
// SequenceLoadingOperation.cs
// ═══════════════════════════════════════════════════════════════
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem.Operations
{
    public sealed class SequenceLoadingOperation : ILoadingOperation
    {
        private readonly List<WeightedOperation> _operations;
        private readonly float _totalWeight;
        private float _completedWeight;
        private int _currentIndex = -1;
        private bool _started;

        public SequenceLoadingOperation(IEnumerable<WeightedOperation> operations)
        {
            _operations = operations.ToList();
            _totalWeight = _operations.Sum(o => o.Weight);
        }

        public float Progress
        {
            get
            {
                if (_totalWeight <= 0f) return 1f;
                if (_currentIndex < 0) return 0f;

                float current = _completedWeight;
                if (_currentIndex < _operations.Count)
                {
                    var op = _operations[_currentIndex];
                    current += op.Operation.Progress * op.Weight;
                }

                return Mathf.Clamp01(current / _totalWeight);
            }
        }

        public bool IsCompleted { get; private set; }

        public async UniTask Execute(CancellationToken cancellationToken = default)
        {
            if (_started)
                throw new System.InvalidOperationException("Sequence has already been started.");

            _started = true;
            _completedWeight = 0f;

            for (int i = 0; i < _operations.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _currentIndex = i;
                var operation = _operations[i];

                await operation.Operation.Execute(cancellationToken);

                _completedWeight += operation.Weight;
            }

            _currentIndex = _operations.Count;
            IsCompleted = true;
        }
    }
}