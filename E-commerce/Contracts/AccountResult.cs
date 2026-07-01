using E_Commerce.DTos.AccountDTOs;

namespace E_Commerce.Contracts
{
    public class AccountResult
    {
        public AccountOutcome outcome { get; set; }

        public ResponseTokenDTO? Data { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }

    }
}
