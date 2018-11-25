using Mergen.Admin.Api.ViewModels.Errors;

namespace Mergen.Admin.Api.ViewModels
{
    public class ApiResultViewModel<T>
    {
        public T Data { get; set; }

        public ErrorViewModel Error { get; set; }

        public object Meta { get; set; }

        public ApiResultViewModel(T data)
        {
            Data = data;
        }

        public ApiResultViewModel()
        {
        }

        public static ApiResultViewModel<TData> FromData<TData>(TData data, object meta = null)
        {
            return new ApiResultViewModel<TData>
            {
                Data = data,
                Meta = meta
            };
        }

        public static ApiResultViewModel<object> FromError(ErrorViewModel error)
        {
            return new ApiResultViewModel<object>
            {
                Error = error
            };
        }
    }
}