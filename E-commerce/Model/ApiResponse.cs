namespace E_Commerce.Model
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public bool IsSuccess { get; set; }

        public static ApiResponse<T> Success(T? _Data, string? Message = null)
        {
            return new ApiResponse<T>
            {
                Message = Message ?? string.Empty,
                Data = _Data,
                IsSuccess = true
            };
        }
        public static ApiResponse<T> Failure(string Message, List<string>? Errors = null)
        {
            return new ApiResponse<T>
            {

                Message = Message,
                Data = default,
                IsSuccess = false,
                Errors = Errors
            };

        }
    }
}
