namespace Ecommerce.API.Helper
{
    public class APIResponse
    {
        public APIResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetMessageFromStatusCode(StatusCode);
        }

        private string GetMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Done successfully",
                201 => "Created successfully",
                400 => "Bad request",
                401 => "Unauthorized",
                404 => "Not found",
                500 => "Server error",
                _ => null
            };
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}
