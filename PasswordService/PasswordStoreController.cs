using Microsoft.AspNetCore.Mvc;
using System.Net;
using PasswordService.BackingStore;
using Crypto;

namespace PasswordService
{
    [ApiController]
    //[Route("[controller]")]
    [Route("/")]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordStorage _storage;

        public AuthController(IPasswordStorage storage)
        {
            _storage = storage;
        }

        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
        {
            var mainPassStore = await _storage.GetMainPassStore();
            if (!mainPassStore.Test(dto.Old))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Login incorrect\r\n");
            }

            mainPassStore = MainPassStore.Generate(dto.New);
            await _storage.SetMainPassStore(mainPassStore);

            return Ok();
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto dto)
        {
            var mainPassStore = await _storage.GetMainPassStore();
            if (!mainPassStore.Test(dto.MainPass))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Login incorrect\r\n");
            }

            if (await _storage.HasUser(dto.UserName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, $"User \"{dto.UserName}\" already exists\r\n");
            }

            var mainKeyIvPair = new KeyIvPair(dto.MainPass, mainPassStore.Salt);
            var keyIvPair = new KeyIvPair(dto.UserPass);
            var encryptedMainKeyIvpair = Crypto.Encryption.Encrypt(mainKeyIvPair.GetAsBytes(), keyIvPair);
            var userEntry = new UserEntryDto
            {
                UserPassSalt = keyIvPair.Salt,
                EncryptedMainKeyIvPair = encryptedMainKeyIvpair
            };

            await _storage.SetUser(dto.UserName, userEntry);

            return Ok();
        }

        [HttpGet("GetUser/{user}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserEntryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(string user)
        {
            var userEntry = await _storage.GetUser(user);
            if (userEntry == null)
            {
                return NotFound($"User \"{user}\" not found");
            }

            return Ok(userEntry);
        }

        [HttpGet("GetRegion")]
        public IActionResult GetRegion() 
        {
            var region = Environment.GetEnvironmentVariable("AWS_REGION");

            return Ok(region);
        }
    }
}
