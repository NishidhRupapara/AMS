namespace WebApplication1.Models.Admin
{
    internal class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AdminLogin? Data { get; set; }
    }
}