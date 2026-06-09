namespace Assinments.Model
{
    public class ApiResponse<T>
    {

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public bool IsSuccess { get; set; }

        public static ApiResponse<T> Success(T _Data, string Message = null)
        {
            return new ApiResponse<T>
            {
                StatusCode = 200,
                Message = Message,
                Data = _Data,
                IsSuccess = true
            };

        }
        public static ApiResponse<T> Failure(string Message, List<string> Errors = null, int StatusCode = 400)
        {
            return new ApiResponse<T>
            {
                StatusCode = StatusCode,
                Message = Message,
                Data = default,
                IsSuccess = false,
                Errors = Errors
            };

        }
    }


}
