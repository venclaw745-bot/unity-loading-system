// ═══════════════════════════════════════════════════════════════
// LoadingOperationExtensions.cs
// ═══════════════════════════════════════════════════════════════
namespace LoadingSystem
{
    public static class LoadingOperationExtensions
    {
        public static WeightedOperation WithWeight(this ILoadingOperation operation, float weight)
        {
            return new WeightedOperation(operation, weight);
        }
    }
}