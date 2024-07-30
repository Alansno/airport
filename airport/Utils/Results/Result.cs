namespace airport.Utils.Results
{
    public struct Unit
    {
        public static readonly Unit Value = new Unit();
    }
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        protected Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, null);

        public static Result<T> Failure(string error) => new Result<T>(default, false, error);
    }

    public static class Result
    {
        public static Result<Unit> Success() => Result<Unit>.Success(Unit.Value);
        public static Result<Unit> Failure(string error) => Result<Unit>.Failure(error);
    }
}
