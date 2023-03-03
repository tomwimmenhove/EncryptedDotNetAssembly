using System;

namespace PasswordService
{
    public class UserEntryDto
    {
        public byte[] UserPassSalt { get; }
        public byte[] EncryptedMainKeyIvPair { get; }

        public UserEntryDto(byte[] userPassSalt, byte[] encryptedMainKeyIvPair)
        {
            UserPassSalt = userPassSalt;
            EncryptedMainKeyIvPair = encryptedMainKeyIvPair;
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
