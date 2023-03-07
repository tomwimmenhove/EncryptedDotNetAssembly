using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace PasswordService.BackingStore;

public class DynamoPasswordStorage : IPasswordStorage
{
    private const string MainPassTable = "PasswordController_MainPass";
    private const string UserTable = "PasswordController_Users";
    private const string DefaultPassword = "admin";

    private readonly AmazonDynamoDBClient _client;

    public DynamoPasswordStorage()
    {
        //_client = new AmazonDynamoDBClient("XXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", Amazon.RegionEndpoint.EUCentral1);
        _client = new AmazonDynamoDBClient();
    }

    public async Task SetMainPassStore(MainPassStore mainPassStore)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            { "username", new AttributeValue { S = "root" }  },
            { "hash", new AttributeValue { S = Convert.ToBase64String(mainPassStore.Hash) }  },
            { "salt", new AttributeValue { S = Convert.ToBase64String(mainPassStore.Salt) }  }
        };

        await _client.PutItemAsync(MainPassTable, item);
    }

    public async Task<MainPassStore> GetMainPassStore()
    {
        var searchItem = new Dictionary<string, AttributeValue>
        {
            { "username", new AttributeValue { S = "root" }  },
        };

        var item = await _client.GetItemAsync(MainPassTable, searchItem);
        if (!item.Item.Any())
        {
            return MainPassStore.Generate(DefaultPassword);
        }

        return new MainPassStore
        {
            Hash = Convert.FromBase64String(item.Item["hash"].S),
            Salt = Convert.FromBase64String(item.Item["salt"].S),
        };
    }

    private async Task<GetItemResponse> FindUserItem(string username)
    {
        var searchItem = new Dictionary<string, AttributeValue>
        {
            { "username", new AttributeValue { S = username }  },
        };

        return await _client.GetItemAsync(UserTable, searchItem);
    }

    public async Task<bool> HasUser(string username)
    {
        var item = await FindUserItem(username);

        return item.Item.Any();
    }

    public async Task SetUser(string username, UserEntryDto userEntry)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            { "username", new AttributeValue { S = username }  },
            { "userPassSalt", new AttributeValue { S = Convert.ToBase64String(userEntry.UserPassSalt) }  },
            { "encryptedMainKeyIvPair", new AttributeValue { S = Convert.ToBase64String(userEntry.EncryptedMainKeyIvPair) }  }
        };

        await _client.PutItemAsync(UserTable, item);
    }

    public async Task<UserEntryDto?> GetUser(string username)
    {
        var item = await FindUserItem(username);
        if (!item.Item.Any())
        {
            return null;
        }

        return new UserEntryDto(
            Convert.FromBase64String(item.Item["userPassSalt"].S),
            Convert.FromBase64String(item.Item["encryptedMainKeyIvPair"].S)
        );
    }
}
