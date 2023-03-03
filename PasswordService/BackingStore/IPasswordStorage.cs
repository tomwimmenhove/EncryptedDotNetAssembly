namespace PasswordService.BackingStore;

public interface IPasswordStorage
{
    Task SetMainPassStore(MainPassStore mainPassStore);
    Task<MainPassStore> GetMainPassStore();
    Task<bool> HasUser(string username);
    Task SetUser(string username, UserEntryDto userEntry);
    Task<UserEntryDto?> GetUser(string username);
}

