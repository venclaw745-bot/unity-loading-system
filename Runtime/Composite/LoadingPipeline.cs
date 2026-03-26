// ═══════════════════════════════════════════════════════════════
// LoadingPipeline.cs
// ═══════════════════════════════════════════════════════════════
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LoadingSystem
{
    /// <summary>
    /// Sequential pipeline of loading steps.
    /// Each step can be a single operation or a parallel group.
    /// Progress is calculated as the weighted sum of all steps.
    /// </summary>
    public sealed class LoadingPipeline : ILoadingOperation
    {
        private readonly List<WeightedOperation> _steps = new();
        private float _totalWeight;
        private float _completedWeight;
        private int _currentStepIndex = -1;
        private bool _started;

        public float Progress
        {
            get
            {
                if (_totalWeight <= 0f) return IsCompleted ? 1f : 0f;
                if (IsCompleted) return 1f;
                if (_currentStepIndex < 0) return 0f;

                float current = _completedWeight;
                if (_currentStepIndex < _steps.Count)
                {
                    var step = _steps[_currentStepIndex];
                    current += step.Operation.Progress * step.Weight;
                }

                return Mathf.Clamp01(current / _totalWeight);
            }
        }

        public bool IsCompleted { get; private set; }

        // ────────────── Builder API ──────────────

        /// <summary>
        /// Adds a single operation as a sequential step.
        /// </summary>
        public LoadingPipeline Add(ILoadingOperation operation, float weight = 1f)
        {
            ThrowIfStarted();
            _steps.Add(new WeightedOperation(operation, weight));
            _totalWeight += weight;
            return this;
        }

        /// <summary>
        /// Adds multiple operations to run in parallel as a single step.
        /// Step weight = number of operations (each sub-operation contributes equally).
        /// </summary>
        public LoadingPipeline Combine(params ILoadingOperation[] operations)
        {
            ThrowIfStarted();
            var weighted = operations.Select(o => new WeightedOperation(o, 1f));
            var group = new ParallelLoadingGroup(weighted);
            float stepWeight = operations.Length;
            _steps.Add(new WeightedOperation(group, stepWeight));
            _totalWeight += stepWeight;
            return this;
        }

        /// <summary>
        /// Adds multiple weighted operations to run in parallel as a single step.
        /// </summary>
        public LoadingPipeline Combine(params WeightedOperation[] operations)
        {
            ThrowIfStarted();
            var group = new ParallelLoadingGroup(operations);
            float stepWeight = operations.Sum(o => o.Weight);
            _steps.Add(new WeightedOperation(group, stepWeight));
            _totalWeight += stepWeight;
            return this;
        }

        /// <summary>
        /// Adds a parallel group as a step with an explicit total step weight.
        /// </summary>
        public LoadingPipeline CombineWithWeight(float stepWeight, params ILoadingOperation[] operations)
        {
            ThrowIfStarted();
            var weighted = operations.Select(o => new WeightedOperation(o, 1f));
            var group = new ParallelLoadingGroup(weighted);
            _steps.Add(new WeightedOperation(group, stepWeight));
            _totalWeight += stepWeight;
            return this;
        }

        /// <summary>
        /// Appends all steps from another pipeline.
        /// </summary>
        public LoadingPipeline Append(LoadingPipeline other)
        {
            ThrowIfStarted();
            foreach (var step in other._steps)
            {
                _steps.Add(step);
                _totalWeight += step.Weight;
            }
            return this;
        }

        // ────────────── Execution ──────────────

        public async UniTask Execute(CancellationToken cancellationToken = default)
        {
            if (_started)
                throw new InvalidOperationException("LoadingPipeline has already been started.");

            _started = true;
            _completedWeight = 0f;

            for (int i = 0; i < _steps.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _currentStepIndex = i;
                var step = _steps[i];

                await step.Operation.Execute(cancellationToken);

                _completedWeight += step.Weight;
            }

            _currentStepIndex = _steps.Count;
            IsCompleted = true;
        }

        private void ThrowIfStarted()
        {
            if (_started)
                throw new InvalidOperationException(
                    "Cannot modify LoadingPipeline after execution has started.");
        }
    }
}