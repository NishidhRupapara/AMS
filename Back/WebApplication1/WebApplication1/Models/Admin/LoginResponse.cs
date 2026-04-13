namespace WebApplication1.Models.Admin
{
    internal class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AdminLogin Data { get; set; }
    }
}