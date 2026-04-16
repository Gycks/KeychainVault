using System.Diagnostics.CodeAnalysis;
using System.Text;
using KeychainVault.Operations.Options;

namespace KeychainVault.Test;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MacOSKeychainTests : IDisposable
{
    private readonly MacOSKeychain _keychain;
    private readonly string _testService;
    private readonly string _testAccount;
    private readonly string _testServer;

    public MacOSKeychainTests()
    {
        _keychain = new MacOSKeychain(false);
        var guid = Guid.NewGuid().ToString();
        _testService = $"TestService_{guid}";
        _testAccount = $"TestAccount_{guid}";
        _testServer = $"TestServer_{guid}";
    }

    public void Dispose()
    {
        TryDelete(() => _keychain.DeleteGenericPasswordItem(_testService, _testAccount));
        TryDelete(() => _keychain.DeleteInternetPasswordItem(_testServer, _testAccount));
    }

    private static void TryDelete(Action delete)
    {
        try
        {
            delete();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    [Fact]
    public void AddGenericPasswordItem_ShouldStoreAndRetrievePassword()
    {
        var secret = Encoding.UTF8.GetBytes("testpassword");
        var expected = secret.ToArray(); 
        
        _keychain.AddGenericPasswordItem(_testService, _testAccount, secret);
        var retrieved = _keychain.LoadGenericPasswordItem(_testService);
        
        Assert.NotNull(retrieved);
        Assert.Equal(expected, retrieved.Secret);
        Assert.Equal(_testAccount, retrieved.Account);
    }

    [Fact]
    public void AddGenericPasswordItem_WithOptions_ShouldStoreWithOptions()
    {
        var secret = Encoding.UTF8.GetBytes("testpassword");
        var expected = secret.ToArray(); 
        var option = new GenericPasswordOption
        {
            Label = "Test Label",
            Description = "Test Description"
        };
        
        _keychain.AddGenericPasswordItem(_testService, _testAccount, secret, option: option);
        var retrieved = _keychain.LoadGenericPasswordItem(_testService, option);
        
        Assert.NotNull(retrieved);
        Assert.Equal(expected, retrieved.Secret);
        Assert.Equal(_testAccount, retrieved.Account);
    }

    [Fact]
    public void AddGenericPasswordItem_UpdateIfExistsFalse_ShouldThrowOnDuplicate()
    {
        var secret1 = Encoding.UTF8.GetBytes("password1");
        var secret2 = Encoding.UTF8.GetBytes("password2");
        
        _keychain.AddGenericPasswordItem(_testService, _testAccount, secret1, updateIfExists: false);
        
        Assert.Throws<InvalidOperationException>(() =>
            _keychain.AddGenericPasswordItem(_testService, _testAccount, secret2, updateIfExists: false));
    }

    [Fact]
    public void AddGenericPasswordItem_UpdateIfExistsTrue_ShouldUpdate()
    {
        var secret1 = Encoding.UTF8.GetBytes("password1");
        var expected1 = secret1.ToArray();
        var secret2 = Encoding.UTF8.GetBytes("password2");
        var expected2 = secret2.ToArray();
        
        _keychain.AddGenericPasswordItem(_testService, _testAccount, secret1, updateIfExists: true);
        _keychain.AddGenericPasswordItem(_testService, _testAccount, secret2, updateIfExists: true);
        var retrieved = _keychain.LoadGenericPasswordItem(_testService);
        
        Assert.NotNull(retrieved);
        Assert.NotEqual(expected1, retrieved.Secret);
        Assert.Equal(expected2, retrieved.Secret);
        Assert.Equal(_testAccount, retrieved.Account);
    }

    [Fact]
    public void LoadGenericPasswordItem_NonExistent_ShouldReturnNull()
    {
        var retrieved = _keychain.LoadGenericPasswordItem("NonExistentService");
        
        Assert.Null(retrieved);
    }

    [Fact]
    public void DeleteGenericPasswordItem_Existing_ShouldReturnTrue()
    {
        var secret = Encoding.UTF8.GetBytes("testpassword");
        _keychain.AddGenericPasswordItem(_testService, _testAccount, secret);
        
        var result = _keychain.DeleteGenericPasswordItem(_testService, _testAccount);
        
        Assert.True(result);
    }

    [Fact]
    public void DeleteGenericPasswordItem_NonExistent_ShouldReturnFalse()
    {
        var result = _keychain.DeleteGenericPasswordItem("NonExistentService");
        
        Assert.False(result);
    }

    [Fact]
    public void AddInternetPasswordItem_ShouldStoreAndRetrievePassword()
    {
        var secret = Encoding.UTF8.GetBytes("testpassword");
        var expected = secret.ToArray();
        
        _keychain.AddInternetPasswordItem(_testServer, _testAccount, secret);
        var retrieved = _keychain.LoadInternetPasswordItem(_testServer);
        
        Assert.NotNull(retrieved);
        Assert.Equal(expected, retrieved.Secret);
        Assert.Equal(_testAccount, retrieved.Account);
    }

    [Fact]
    public void AddInternetPasswordItem_WithOptions_ShouldStoreWithOptions()
    {
        var secret = Encoding.UTF8.GetBytes("testpassword");
        var expected = secret.ToArray();
        var option = new InternetPasswordOption
        {
            Protocol = InternetProtocolOption.Https,
            Port = 443
        };
        
        _keychain.AddInternetPasswordItem(_testServer, _testAccount, secret, option: option);
        var retrieved = _keychain.LoadInternetPasswordItem(_testServer, option);
        
        Assert.NotNull(retrieved);
        Assert.Equal(expected, retrieved.Secret);
        Assert.Equal(_testAccount, retrieved.Account);
    }

    [Fact]
    public void LoadInternetPasswordItem_NonExistent_ShouldReturnNull()
    {
        var retrieved = _keychain.LoadInternetPasswordItem("NonExistentServer");
        
        Assert.Null(retrieved);
    }

    [Fact]
    public void DeleteInternetPasswordItem_Existing_ShouldReturnTrue()
    {
        var secret = Encoding.UTF8.GetBytes("testpassword");
        _keychain.AddInternetPasswordItem(_testServer, _testAccount, secret);
        
        var result = _keychain.DeleteInternetPasswordItem(_testServer);
        
        Assert.True(result);
    }

    [Fact]
    public void DeleteInternetPasswordItem_NonExistent_ShouldReturnFalse()
    {
        var result = _keychain.DeleteInternetPasswordItem("NonExistentServer", "NonExistentAccount");
        
        Assert.False(result);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void AddGenericPasswordItem_InvalidService_ShouldThrow(string invalidService)
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentException>(() =>
            _keychain.AddGenericPasswordItem(invalidService, _testAccount, secret));
    }
    
    [Fact]
    public void AddGenericPasswordItem_InvalidService_ShouldThrow_()
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentNullException>(() =>
            _keychain.AddGenericPasswordItem(null!, _testAccount, secret));
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void AddGenericPasswordItem_InvalidAccount_ShouldThrow(string invalidAccount)
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentException>(() =>
            _keychain.AddGenericPasswordItem(_testService, invalidAccount, secret));
    }

    [Fact]
    public void AddGenericPasswordItem_InvalidAccount_ShouldThrow_()
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentNullException>(() =>
            _keychain.AddGenericPasswordItem(_testService, null!, secret));
    }

    [Fact]
    public void AddGenericPasswordItem_NullSecret_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _keychain.AddGenericPasswordItem(_testService, _testAccount, null!));
    }

    [Fact]
    public void AddGenericPasswordItem_EmptySecret_ShouldThrow()
    {
        var secret = Array.Empty<byte>();
        
        Assert.Throws<ArgumentException>(() =>
            _keychain.AddGenericPasswordItem(_testService, _testAccount, secret));
    }
    
    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void AddInternetPasswordItem_InvalidServer_ShouldThrow(string invalidServer)
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentException>(() =>
            _keychain.AddInternetPasswordItem(invalidServer, _testAccount, secret));
    }
    
    [Fact]
    public void AddInternetPasswordItem_InvalidServer_ShouldThrow_()
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentNullException>(() =>
            _keychain.AddInternetPasswordItem(null!, _testAccount, secret));
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void AddInternetPasswordItem_InvalidAccount_ShouldThrow(string invalidAccount)
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentException>(() =>
            _keychain.AddInternetPasswordItem(_testService, invalidAccount, secret));
    }
    
    [Fact]
    public void AddInternetPasswordItem_InvalidAccount_ShouldThrow_()
    {
        var secret = Encoding.UTF8.GetBytes("test");
        
        Assert.Throws<ArgumentNullException>(() =>
            _keychain.AddInternetPasswordItem(_testService, null!, secret));
    }

    [Fact]
    public void AddInternetPasswordItem_NullSecret_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _keychain.AddInternetPasswordItem(_testService, _testAccount, null!));
    }

    [Fact]
    public void AddInternetPasswordItem_EmptySecret_ShouldThrow()
    {
        var secret = Array.Empty<byte>();
        
        Assert.Throws<ArgumentException>(() =>
            _keychain.AddInternetPasswordItem(_testService, _testAccount, secret));
    }
}
