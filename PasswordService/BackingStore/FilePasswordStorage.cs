using System.Text.Json;
using System.Collections.Concurrent;

namespace PasswordService.BackingStore;

public class FilePasswordStorage : IPasswordStorage
{
    private const string DefaultPassword = "admin";
    private const string MainPasswordFileName = "main.json";
    private const string UsersFileName = "users.json";

    private readonly ConcurrentDictionary<string, UserEntryDto> _users;
    private MainPassStore _mainPassStore;

    public FilePasswordStorage()
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
            _mainPassStore = MainPassStore.Generate(DefaultPassword);
        }
        else
        {
            _mainPassStore = Deserialize<MainPassStore>(MainPasswordFileName) ?? MainPassStore.Generate(DefaultPassword);
        }
    }

    public Task SetMainPassStore(MainPassStore mainPassStore)
    {
        _mainPassStore = mainPassStore;
        Serialize(MainPasswordFileName, mainPassStore);

        return Task.CompletedTask;
    }

    public Task<MainPassStore> GetMainPassStore() => Task.FromResult(_mainPassStore);

    public Task<bool> HasUser(string username) => Task.FromResult(_users.ContainsKey(username));

    public Task SetUser(string username, UserEntryDto userEntry)
    {
        _users[username] = userEntry;
        Serialize(UsersFileName, _users);

        return Task.CompletedTask;
    }

    public Task<UserEntryDto?> GetUser(string username)
    {
        if (_users.TryGetValue(username, out var userEntry))
        {
            return Task.FromResult<UserEntryDto?>(userEntry);
        }

        return Task.FromResult<UserEntryDto?>(null);
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
}
