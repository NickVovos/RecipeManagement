namespace Common.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string Message { get; private set; }
        public ValidationLevel Level { get; private set; }

        private ValidationResult(bool isValid, string message, ValidationLevel level)
        {
            IsValid = isValid;
            Message = message;
            Level = level;
        }

        public static ValidationResult Success() => new ValidationResult(true, string.Empty, ValidationLevel.Success);
        public static ValidationResult Failure(string message) => new ValidationResult(false, message, ValidationLevel.Error);
        public static ValidationResult Warning(string message) => new ValidationResult(true, message, ValidationLevel.Warning);
    }

    public enum ValidationLevel
    {
        Success,
        Warning,
        Error
    }
}
