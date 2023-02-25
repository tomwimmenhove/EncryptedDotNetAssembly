using System;

namespace PasswordService
{
    internal class UserEntryDto
    {
        public byte[] UserPassSalt { get; }
        public byte[] EncryptedMainKeyIvpair { get; }

        public UserEntryDto(byte[] userPassSalt, byte[] encryptedMainKeyIvpair)
        {
            UserPassSalt = userPassSalt;
            EncryptedMainKeyIvpair = encryptedMainKeyIvpair;
        }
    }

    public class SetPasswordDto
    {
        public string Old { get; set; } = "";
        public string New { get; set; } = "";
    }

    public class AddUserDto
    {
        public string MainPass { get; set; } = "";
        public string UserName { get; set; } = "";
        public string UserPass { get; set; } = "";
    }

    public class PasswordDto
    {
        public string Pass { get; set; } = "";
    }
}
