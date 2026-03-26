// ═══════════════════════════════════════════════════════════════
// WeightedOperation.cs
// ═══════════════════════════════════════════════════════════════
namespace LoadingSystem
{
    public readonly struct WeightedOperation
    {
        public readonly ILoadingOperation Operation;
        public readonly float Weight;

        public WeightedOperation(ILoadingOperation operation, float weight = 1f)
        {
            Operation = operation;
            Weight = weight;
        }
    }
}