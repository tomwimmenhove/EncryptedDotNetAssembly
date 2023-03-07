namespace PasswordService
{
    public class UserEntryDto
    {
        public byte[] UserPassSalt { get; set;} = default!;
        public byte[] EncryptedMainKeyIvPair { get; set; } = default!;
    }

    public class SetPasswordDto
    {
        public string Old { get; set; } = default!;
        public string New { get; set; } = default!;
    }

    public class AddUserDto
    {
        public string MainPass { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string UserPass { get; set; } = default!;
    }

    public class PasswordDto
    {
        public string Pass { get; set; } = default!;
    }
}
