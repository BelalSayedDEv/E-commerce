namespace E_Commerce.DTos.AccountDTOs
{
    public class ResponseTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
