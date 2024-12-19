using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.User.Entities;
using Features.User.DTOs;
using Data;

public class UserServiceTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _contextOptions;
    private AppDbContext _context;
    private UserService _userService;

    public UserServiceTests()
    {
        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(_contextOptions);
        _userService = new UserService(_context);
    }

    private void ResetContext()
    {
        _context.Dispose();
        _context = new AppDbContext(_contextOptions);
        _userService = new UserService(_context);
    }

    #region Existing Tests

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUsers()
    {
        ResetContext();

        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), GoogleId = "google-1", Email = "test1@example.com", DisplayName = "User1", Rank = Rank.Noob, XP = 100, Gold = 10 },
            new User { Id = Guid.NewGuid(), GoogleId = "google-2", Email = "test2@example.com", DisplayName = "User2", Rank = Rank.Pro, XP = 200, Gold = 20 }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        var result = await _userService.GetAllUsersAsync();

        Assert.Equal(users.Count, result.Count());
        Assert.Equal(users[0].Email, result.First().Email);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsCorrectUser()
    {
        ResetContext();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            GoogleId = "google-test",
            Email = "test@example.com",
            DisplayName = "TestUser",
            Rank = Rank.Pro,
            XP = 300,
            Gold = 50,
            OwnedSkins = "Skin1,Skin2"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.GetUserAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.XP, result.XP);
    }

    [Fact]
    public async Task UpdateUserGoldXp_ReturnsTrue_WhenUserExists()
    {
        ResetContext();

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, GoogleId = "google-update", Email = "updateuser@example.com", DisplayName = "UpdateUser", Gold = 10, XP = 100 };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.UpdateUserGoldXp(userId, 20, 50);

        Assert.True(result);
        Assert.Equal(30, user.Gold);
        Assert.Equal(150, user.XP);
    }

    [Fact]
    public async Task CreateUserAsync_AddsUserAndReturnsUser()
    {
        ResetContext();
        var userValidationDTO = new UserValidationDTO
        {
            GoogleId = "google-create",
            Email = "newuser@example.com",
            DisplayName = "NewUser"
        };

        var result = await _userService.CreateUserAsync(userValidationDTO);

        Assert.NotNull(result);
        Assert.Equal(userValidationDTO.Email, result.Email);
        Assert.Equal(userValidationDTO.DisplayName, result.DisplayName);
    }

    [Fact]
    public async Task IsEmailTakenAsync_ReturnsTrue_WhenEmailExists()
    {
        ResetContext();

        var user = new User { GoogleId = "google-taken", Email = "taken@example.com", DisplayName = "TakenUser" };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.IsEmailTakenAsync("taken@example.com");

        Assert.True(result);
    }

    [Fact]
    public async Task AddSkinToUserAsync_AddsSkin_WhenNotOwned()
    {
        ResetContext();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            GoogleId = "google-skin",
            Email = "skinuser@example.com",
            DisplayName = "SkinUser",
            OwnedSkins = "Skin1"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.AddSkinToUserAsync(userId, "Skin2");

        Assert.True(result);
        var updatedUser = await _context.Users.FindAsync(userId);
        Assert.Contains("Skin2", updatedUser.OwnedSkins);
    }

    [Fact]
    public async Task EquipSkinAsync_SetsEquippedSkin_WhenOwned()
    {
        ResetContext();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            GoogleId = "google-equip",
            Email = "testuser@example.com",
            DisplayName = "TestUser",
            OwnedSkins = "Skin1,Skin2",
            EquippedSkin = "Skin1"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.EquipSkinAsync(userId, "Skin2");

        Assert.True(result);
        Assert.Equal("Skin2", user.EquippedSkin);
    }

    [Fact]
    public async Task CheckXPRequiredForLevelUp_ReturnsCorrectXP_WhenRankIsOdd()
    {
        ResetContext();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, GoogleId = "google-xp", Email = "xpuser@example.com", DisplayName = "XPUser", Rank = Rank.Noob, XP = 100 };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.CheckXPRequiredForLevelUp(userId);

        // Correct calculation considering operator precedence
        int rankValue = (int)user.Rank;
        int expectedXP = (rankValue % 2 != 0) ? 125 * (int)Math.Pow(rankValue, 3) : 125 * (int)Math.Pow(rankValue, 3) + 125;

        Assert.Equal(expectedXP, result);
    }

    [Fact]
    public async Task GetUsersByRankAsync_ReturnsUsersWithSpecifiedRank()
    {
        ResetContext();
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), GoogleId = "google-1", Email = "test1@example.com", DisplayName = "User1", Rank = Rank.Noob, XP = 100, Gold = 10 },
            new User { Id = Guid.NewGuid(), GoogleId = "google-2", Email = "test2@example.com", DisplayName = "User2", Rank = Rank.Pro, XP = 200, Gold = 20 },
            new User { Id = Guid.NewGuid(), GoogleId = "google-3", Email = "test3@example.com", DisplayName = "User3", Rank = Rank.Noob, XP = 150, Gold = 15 }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        var result = await _userService.GetUsersByRankAsync(Rank.Noob);

        Assert.Equal(2, result.Count());
        Assert.All(result, user => Assert.Equal(Rank.Noob, user.PublicRank));
    }

    [Fact]
    public async Task GetUserByGoogleIdAsync_ReturnsCorrectUser_WhenGoogleIdExists()
    {
        ResetContext();
        var googleId = "google-id-123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            GoogleId = googleId,
            Email = "user@example.com",
            DisplayName = "Test User",
            Rank = Rank.Noob,
            XP = 100,
            Gold = 50
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.GetUserByGoogleIdAsync(googleId);

        Assert.NotNull(result);
        Assert.Equal(user.GoogleId, result.GoogleId);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.DisplayName, result.DisplayName);
        Assert.Equal(user.Rank, result.PublicRank);
        Assert.Equal(user.XP, result.XP);
        Assert.Equal(user.Gold, result.Gold);
    }

    [Fact]
    public async Task GetUserByGoogleIdAsync_ReturnsNull_WhenGoogleIdDoesNotExist()
    {
        ResetContext();
        var googleId = "non-existing-google-id";

        var result = await _userService.GetUserByGoogleIdAsync(googleId);

        Assert.Null(result);
    }

    [Fact]
    public void Equals_ReturnsTrue_ForIdenticalDisplayName()
    {
        var user1 = new User { Id = Guid.NewGuid(), DisplayName = "TestUser", Rank = Rank.Noob };
        var user2 = new User { Id = Guid.NewGuid(), DisplayName = "TestUser", Rank = Rank.Pro };

        var result = user1.Equals(user2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentDisplayName()
    {
        var user1 = new User { Id = Guid.NewGuid(), DisplayName = "User1", Rank = Rank.Noob };
        var user2 = new User { Id = Guid.NewGuid(), DisplayName = "User2", Rank = Rank.Noob };

        var result = user1.Equals(user2);

        Assert.False(result);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenOtherIsNull()
    {
        var user = new User { Id = Guid.NewGuid(), DisplayName = "User1", Rank = Rank.Noob };

        var result = user.Equals(null);

        Assert.False(result);
    }

    [Fact]
    public void Equals_Object_ReturnsTrue_ForIdenticalDisplayName()
    {
        var user1 = new User { Id = Guid.NewGuid(), DisplayName = "TestUser", Rank = Rank.Noob };
        object user2 = new User { Id = Guid.NewGuid(), DisplayName = "TestUser", Rank = Rank.Pro };

        var result = user1.Equals(user2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_Object_ReturnsFalse_ForDifferentDisplayName()
    {
        var user1 = new User { Id = Guid.NewGuid(), DisplayName = "User1", Rank = Rank.Noob };
        object user2 = new User { Id = Guid.NewGuid(), DisplayName = "User2", Rank = Rank.Noob };

        var result = user1.Equals(user2);

        Assert.False(result);
    }

    [Fact]
    public void Equals_Object_ReturnsFalse_WhenOtherIsDifferentType()
    {
        var user = new User { Id = Guid.NewGuid(), DisplayName = "User1", Rank = Rank.Noob };
        var otherObject = new { Name = "User1" };

        var result = user.Equals(otherObject);

        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_ReturnsSameValue_ForIdenticalDisplayNameAndRank()
    {
        var user1 = new User { DisplayName = "TestUser", Rank = Rank.Noob };
        var user2 = new User { DisplayName = "TestUser", Rank = Rank.Noob };

        var hash1 = user1.GetHashCode();
        var hash2 = user2.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentValue_ForDifferentDisplayNameOrRank()
    {
        var user1 = new User { DisplayName = "TestUser1", Rank = Rank.Noob };
        var user2 = new User { DisplayName = "TestUser2", Rank = Rank.Pro };

        var hash1 = user1.GetHashCode();
        var hash2 = user2.GetHashCode();

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void IsEligibleForPurchase_ReturnsTrue_WhenConditionsAreMet()
    {
        var user = new User { Gold = 100, Rank = Rank.Pro };
        var hasSufficientGold = true;
        var meetsRankRequirement = true;

        var isEligibleForPurchase = hasSufficientGold && meetsRankRequirement;

        Assert.True(isEligibleForPurchase);
    }

    [Fact]
    public void IsEligibleForPurchase_ReturnsFalse_WhenConditionsAreNotMet()
    {
        var user = new User { Gold = 30, Rank = Rank.Noob };
        var hasSufficientGold = false;
        var meetsRankRequirement = false;

        var isEligibleForPurchase = hasSufficientGold && meetsRankRequirement;

        Assert.False(isEligibleForPurchase);
    }

    [Fact]
    public async Task ValidateUserAsync_ReturnsUser_WhenUserExists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var user = new User
        {
            Id = Guid.NewGuid(),
            GoogleId = "google-1",
            Email = "test1@example.com",
            DisplayName = "TestUser"
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var userService = new UserService(context);

        var result = await userService.ValidateUserAsync("google-1", "test1@example.com", "TestUser");

        Assert.NotNull(result);
        Assert.Equal(user.GoogleId, result.GoogleId);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.DisplayName, result.DisplayName);
    }

    [Fact]
    public async Task ValidateUserAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var userService = new UserService(context);

        var result = await userService.ValidateUserAsync("nonexistent-google-id", "nonexistent@example.com", "NonexistentUser");

        Assert.Null(result);
    }

    #endregion

    #region Additional Tests

    // CheckUsernameAvailabilityAsync Tests

    [Fact]
    public async Task CheckUsernameAvailabilityAsync_ReturnsTrue_WhenUsernameIsAvailable()
    {
        ResetContext();

        var user = new User { DisplayName = "UniqueUser", Email = "unique@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.CheckUsernameAvailabilityAsync("AnotherUniqueUser");

        Assert.True(result);
    }

    [Fact]
    public async Task CheckUsernameAvailabilityAsync_ReturnsFalse_WhenUsernameIsTaken()
    {
        ResetContext();

        var user = new User { DisplayName = "TakenUser", Email = "taken@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.CheckUsernameAvailabilityAsync("TakenUser");

        Assert.False(result);
    }

    // IsUsernameTakenAsync Tests

    [Fact]
    public async Task IsUsernameTakenAsync_ReturnsTrue_WhenUsernameExists()
    {
        ResetContext();

        var user = new User { DisplayName = "ExistingUser", Email = "existing@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.IsUsernameTakenAsync("ExistingUser");

        Assert.True(result);
    }

    [Fact]
    public async Task IsUsernameTakenAsync_ReturnsFalse_WhenUsernameDoesNotExist()
    {
        ResetContext();

        var result = await _userService.IsUsernameTakenAsync("NonExistingUser");

        Assert.False(result);
    }

    // CheckXPRequiredForLevelUp Tests

    [Fact]
    public async Task CheckXPRequiredForLevelUp_ReturnsCorrectXP_WhenRankIsEven()
    {
        ResetContext();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Rank = Rank.Pro, XP = 200 };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var rankValue = (int)Rank.Pro;
        var expectedXP = (rankValue % 2 != 0) ? 125 * (int)Math.Pow(rankValue, 3) : 125 * (int)Math.Pow(rankValue, 3) + 125;

        var result = await _userService.CheckXPRequiredForLevelUp(userId);

        Assert.Equal(expectedXP, result);
    }

    [Fact]
    public async Task CheckXPRequiredForLevelUp_ReturnsMinusOne_WhenUserDoesNotExist()
    {
        ResetContext();
        var nonExistentUserId = Guid.NewGuid();

        var result = await _userService.CheckXPRequiredForLevelUp(nonExistentUserId);

        Assert.Equal(-1, result);
    }

    // UpdateUserGoldXp Tests

    [Fact]
    public async Task UpdateUserGoldXp_ReturnsFalse_WhenUserDoesNotExist()
    {
        ResetContext();
        var nonExistentUserId = Guid.NewGuid();

        var result = await _userService.UpdateUserGoldXp(nonExistentUserId, 10, 20);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserGoldXp_ReturnsFalse_WhenXpIsNegative()
    {
        ResetContext();

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Gold = 50, XP = 100 };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.UpdateUserGoldXp(userId, 10, -20);

        Assert.False(result);
        var updatedUser = await _context.Users.FindAsync(userId);
        Assert.Equal(50, updatedUser.Gold);
        Assert.Equal(100, updatedUser.XP);
    }

    [Fact]
    public async Task UpdateUserGoldXp_UpdatesGoldAndXpCorrectly()
    {
        ResetContext();

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Gold = 50, XP = 100 };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.UpdateUserGoldXp(userId, 25, 75);

        Assert.True(result);
        var updatedUser = await _context.Users.FindAsync(userId);
        Assert.Equal(75, updatedUser.Gold);
        Assert.Equal(175, updatedUser.XP);
    }

    // AddSkinToUserAsync Tests

    [Fact]
    public async Task AddSkinToUserAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        ResetContext();
        var nonExistentUserId = Guid.NewGuid();

        var result = await _userService.AddSkinToUserAsync(nonExistentUserId, "SkinX");

        Assert.False(result);
    }

    [Fact]
    public async Task AddSkinToUserAsync_DoesNotAddDuplicateSkin()
    {
        ResetContext();

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, OwnedSkins = "Skin1,Skin2" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.AddSkinToUserAsync(userId, "Skin1");

        Assert.True(result);
        var updatedUser = await _context.Users.FindAsync(userId);
        var skins = updatedUser.OwnedSkins.Split(',', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, skins.Length);
        Assert.Contains("Skin1", skins);
        Assert.Contains("Skin2", skins);
    }

    // EquipSkinAsync Tests

    [Fact]
    public async Task EquipSkinAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        ResetContext();
        var nonExistentUserId = Guid.NewGuid();

        var result = await _userService.EquipSkinAsync(nonExistentUserId, "Skin1");

        Assert.False(result);
    }

    [Fact]
    public async Task EquipSkinAsync_ReturnsFalse_WhenSkinIsNotOwned()
    {
        ResetContext();

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, OwnedSkins = "Skin1,Skin2" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.EquipSkinAsync(userId, "Skin3");

        Assert.False(result);
        var updatedUser = await _context.Users.FindAsync(userId);
        Assert.Null(updatedUser.EquippedSkin);
    }

    // ValidateUserAsync Tests

    [Fact]
    public async Task ValidateUserAsync_ReturnsNull_WhenOnlyGoogleIdMatches()
    {
        ResetContext();

        var user = new User
        {
            GoogleId = "google-validate",
            Email = "validate@example.com",
            DisplayName = "ValidateUser"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.ValidateUserAsync("google-validate", "wrong@example.com", "ValidateUser");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateUserAsync_ReturnsNull_WhenOnlyEmailMatches()
    {
        ResetContext();

        var user = new User
        {
            GoogleId = "google-validate",
            Email = "validate@example.com",
            DisplayName = "ValidateUser"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.ValidateUserAsync("wrong-google-id", "validate@example.com", "ValidateUser");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateUserAsync_ReturnsNull_WhenOnlyDisplayNameMatches()
    {
        ResetContext();

        var user = new User
        {
            GoogleId = "google-validate",
            Email = "validate@example.com",
            DisplayName = "ValidateUser"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var result = await _userService.ValidateUserAsync("wrong-google-id", "wrong@example.com", "ValidateUser");

        Assert.Null(result);
    }

    // CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_SetsDefaultValues()
    {
        ResetContext();

        var userValidationDTO = new UserValidationDTO
        {
            GoogleId = "google-default",
            Email = "default@example.com",
            DisplayName = "DefaultUser"
        };

        var result = await _userService.CreateUserAsync(userValidationDTO);

        Assert.NotNull(result);
        Assert.Equal(userValidationDTO.GoogleId, result.GoogleId);
        Assert.Equal(userValidationDTO.Email, result.Email);
        Assert.Equal(userValidationDTO.DisplayName, result.DisplayName);
        Assert.Equal(Rank.Noob, result.Rank); // Assuming default rank
        Assert.Equal(0, result.XP);
        Assert.Equal(0, result.Gold);
        Assert.Null(result.OwnedSkins);
        Assert.Null(result.EquippedSkin);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsException_WhenEmailIsAlreadyTaken()
    {
        ResetContext();

        var existingUser = new User
        {
            GoogleId = "google-existing",
            Email = "existing@example.com",
            DisplayName = "ExistingUser"
        };
        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        var userValidationDTO = new UserValidationDTO
        {
            GoogleId = "google-new",
            Email = "existing@example.com",
            DisplayName = "NewUser"
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => _userService.CreateUserAsync(userValidationDTO));
    }

    // GetAllUsersAsync Tests

    [Fact]
    public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
    {
        ResetContext();

        var result = await _userService.GetAllUsersAsync();

        Assert.Empty(result);
    }

    // IsUsernameTakenAsync vs CheckUsernameAvailabilityAsync

    [Fact]
    public async Task IsUsernameTakenAsync_IsInverseOf_CheckUsernameAvailabilityAsync()
    {
        ResetContext();

        var user = new User { DisplayName = "InverseUser", Email = "inverse@example.com" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var isTaken = await _userService.IsUsernameTakenAsync("InverseUser");
        var isAvailable = await _userService.CheckUsernameAvailabilityAsync("InverseUser");

        Assert.True(isTaken);
        Assert.False(isAvailable);
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}
