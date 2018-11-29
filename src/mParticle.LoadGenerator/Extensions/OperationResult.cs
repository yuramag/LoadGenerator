namespace mParticle.LoadGenerator.Extensions
{
    public sealed class OperationResult
    {
        private static readonly OperationResult _success = new OperationResult();

        private OperationResult()
        {
            Succeeded = true;
        }

        private OperationResult(string error)
        {
            Succeeded = false;
            Error = error ?? "Unexpected error";
        }

        public bool Succeeded { get; }

        public string Error { get; }

        public static OperationResult Success()
        {
            return _success;
        }

        public static OperationResult Failed(string error)
        {
            return new OperationResult(error);
        }
    }
}