using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using Crypto;

namespace PasswordService
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private const string DefaultPassword = "admin";
        private const string MainPasswordFileName = "main.json";
        private const string UsersFileName = "users.json";

        private readonly ConcurrentDictionary<string, UserEntryDto> _users;
        private KeyIvPair _mainKeyIvPair;

        public AuthController()
        {
            if (!System.IO.File.Exists(UsersFileName))
            {
                _users = new();
            }
            else
            {
                _users = Deserialize<ConcurrentDictionary<string, UserEntryDto>>(UsersFileName) ?? new();
            }

            if (!System.IO.File.Exists(MainPasswordFileName))
            {
                _mainKeyIvPair = new KeyIvPair(DefaultPassword);
            }
            else
            {
                _mainKeyIvPair = Deserialize<KeyIvPair>(MainPasswordFileName) ?? new KeyIvPair(DefaultPassword);
            }
        }

        private T? Deserialize<T>(string path, object? lockingObject = null)
        {
            lock (lockingObject ?? path)
            {
                var json = System.IO.File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(json);
            }
        }

        private void Serialize(string path, object obj, object? lockingObject = null)
        {
            lock (lockingObject ?? path)
            {
                var json = JsonSerializer.Serialize(obj);
                System.IO.File.WriteAllText(path, json);
            }
        }

        private bool Authenticate(string password)
        {
            var checkKeyIvPair = new KeyIvPair(password, _mainKeyIvPair.Salt);
            return _mainKeyIvPair.Matches(checkKeyIvPair);
        }

        [HttpPost("SetPassword")]
        public IActionResult SetPassword([FromBody] SetPasswordDto dto)
        {
            if (!Authenticate(dto.Old))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Login incorrect\r\n");
            }

            _mainKeyIvPair = new KeyIvPair(dto.New);
            Serialize(MainPasswordFileName, _mainKeyIvPair);

            return Ok();
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] AddUserDto dto)
        {
            if (!Authenticate(dto.MainPass))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Login incorrect\r\n");
            }

            if (_users.ContainsKey(dto.UserName))
            {
                return StatusCode((int)HttpStatusCode.Conflict, $"User \"{dto.UserName}\" already exists\r\n");
            }

            var keyIvPair = new KeyIvPair(dto.UserPass);
            var encryptedMainKeyIvpair = Crypto.Encryption.Encrypt(_mainKeyIvPair.GetAsBytes(), keyIvPair);
            var userEntry = new UserEntryDto(keyIvPair.Salt, encryptedMainKeyIvpair);

            _users[dto.UserName] = userEntry;
            Serialize(UsersFileName, _users);

            return Ok();
        }

        [HttpGet("GetUser/{user}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserEntryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(string user)
        {
            if (!_users.TryGetValue(user, out var userEntry))
            {
                return NotFound($"User \"{user}\" not found");
            }

            return Ok(userEntry);
        }
    }
}
