namespace airport.Utils.Results
{
    public static class ResultExtend
    {
        public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func)
        {
            return result.IsSuccess ? func(result.Value) : Result<TOut>.Failure(result.Error);
        }

        public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func)
        {
            return result.IsSuccess ? Result<TOut>.Success(func(result.Value)) : Result<TOut>.Failure(result.Error);
        }

        public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> func)
        {
            var result = await resultTask;
            return result.IsSuccess ? await func(result.Value) : Result<TOut>.Failure(result.Error);
        }

        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> func)
        {
            var result = await resultTask;
            if (!result.IsSuccess) return Result<TOut>.Failure(result.Error);

            var mappedValue = await func(result.Value);
            return Result<TOut>.Success(mappedValue);
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
        {
            if (!result.IsSuccess) return result;
            return predicate(result.Value) ? result : Result<T>.Failure(errorMessage);
        }
    }
}
