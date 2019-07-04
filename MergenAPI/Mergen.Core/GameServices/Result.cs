namespace Mergen.Core.GameServices
{
    public class Result
    {
        public Result(StatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public bool IsSuccess => StatusCode == StatusCode.Success;
        public StatusCode StatusCode { get; protected set; }

        public static Result Error(StatusCode statusCode)
        {
            return new Result(statusCode);
        }

        public static Result<T> Error<T>(StatusCode statusCode)
        {
            return new Result<T>(statusCode, default);
        }

        public static Result Success()
        {
            return new Result(StatusCode.Success);
        }

        public static Result<T> Success<T>(T value)
        {
            return new Result<T>(StatusCode.Success, value);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        public Result(StatusCode statusCode, T value) : base(statusCode)
        {
            Value = value;
        }
    }
}