using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mergen.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountFriends",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    FriendAccountId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFriends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountInvitations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    InvitationDateTime = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    RegisteredAccountId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    GenderId = table.Column<int>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    StatusNote = table.Column<string>(nullable: true),
                    AvatarImageId = table.Column<string>(nullable: true),
                    CoverImageId = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    EmailVerificationToken = table.Column<string>(nullable: true),
                    IsEmailVerified = table.Column<bool>(nullable: false),
                    EmailVerificationTokenGenerationTime = table.Column<DateTime>(nullable: true),
                    PhoneNumberVerificationToken = table.Column<string>(nullable: true),
                    PhoneNumberVerificationTokenGenerationTime = table.Column<DateTime>(nullable: true),
                    IsPhoneNumberVerified = table.Column<bool>(nullable: false),
                    ResetPasswordToken = table.Column<string>(nullable: true),
                    ResetPasswordTokenGenerationTime = table.Column<DateTime>(nullable: true),
                    Timezone = table.Column<string>(nullable: true),
                    ReceiveNotifications = table.Column<bool>(nullable: false),
                    SearchableByEmailAddressOrUsername = table.Column<bool>(nullable: false),
                    FriendsOnlyBattleInvitations = table.Column<bool>(nullable: false),
                    RegisterDateTime = table.Column<DateTime>(nullable: false),
                    IsBot = table.Column<bool>(nullable: false),
                    AvatarItemIds = table.Column<string>(nullable: true),
                    RoleIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatsSummaries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Sky = table.Column<int>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    Score = table.Column<decimal>(nullable: false),
                    Coins = table.Column<decimal>(nullable: false),
                    TotalBattlesPlayed = table.Column<int>(nullable: false),
                    WinCount = table.Column<int>(nullable: false),
                    WinRatio = table.Column<double>(nullable: false),
                    LoseCount = table.Column<int>(nullable: false),
                    LoseRatio = table.Column<double>(nullable: false),
                    AceWinCount = table.Column<int>(nullable: false),
                    ContinuousActiveDaysCount = table.Column<int>(nullable: false),
                    ContinuousActiveDaysRecord = table.Column<int>(nullable: false),
                    LastPlayDateTime = table.Column<DateTime>(nullable: true),
                    PurchasedItemsCount = table.Column<int>(nullable: false),
                    InvitedPlayersCount = table.Column<int>(nullable: false),
                    GiftedCoins = table.Column<decimal>(nullable: false),
                    UnlockedAchievements = table.Column<int>(nullable: false),
                    Top3Skills = table.Column<string>(nullable: true),
                    SuccessfulBattleInvitationsCount = table.Column<int>(nullable: false),
                    RemoveTwoAnswersHelperUsageCount = table.Column<int>(nullable: false),
                    AnswerHistoryHelperUsageCount = table.Column<int>(nullable: false),
                    AskMergenHelperUsageCount = table.Column<int>(nullable: false),
                    DoubleChanceHelperUsageCount = table.Column<int>(nullable: false),
                    TimeExtenderHelperUsageCount = table.Column<int>(nullable: false),
                    CoinsSpentOnAvatarItems = table.Column<long>(nullable: false),
                    CoinsSpentOnBoosterItems = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatsSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageFileId = table.Column<string>(nullable: true),
                    CategoryId = table.Column<long>(nullable: true),
                    CorrectAnswersCountInCategory = table.Column<long>(nullable: true),
                    WinnedBattlesCount = table.Column<long>(nullable: true),
                    AceWinCount = table.Column<long>(nullable: true),
                    NumberOfContinuousDaysPlaying = table.Column<long>(nullable: true),
                    GiftedCoinsAmount = table.Column<long>(nullable: true),
                    NumberOfTotalBattlesPlayed = table.Column<long>(nullable: true),
                    NumberOfRegisteredFriendsViaInviteLink = table.Column<long>(nullable: true),
                    NumberOfSuccessfulBattleInvitations = table.Column<long>(nullable: true),
                    RemoveTwoAnswersHelperUsageCount = table.Column<long>(nullable: true),
                    AnswerHistoryHelperUsageCount = table.Column<long>(nullable: true),
                    AskMergenHelperUsageCount = table.Column<long>(nullable: true),
                    DoubleChanceHelperUsageCount = table.Column<long>(nullable: true),
                    CoinsSpentOnAvatarItems = table.Column<long>(nullable: true),
                    CoinsSpentOnBooster = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    IconFileId = table.Column<string>(nullable: true),
                    CoverImageFileId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    SenderAccountId = table.Column<long>(nullable: false),
                    ReceiverAccountId = table.Column<long>(nullable: false),
                    MessageText = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    FromAccountId = table.Column<long>(nullable: false),
                    ToAccountId = table.Column<long>(nullable: false),
                    RequestDateTime = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    NotificationTypeId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    ShopItemId = table.Column<long>(nullable: false),
                    Quantity = table.Column<long>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    PurchasedByAccountId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    Answer1 = table.Column<string>(nullable: true),
                    Answer1ChooseHistory = table.Column<long>(nullable: false),
                    Answer2 = table.Column<string>(nullable: true),
                    Answer2ChooseHistory = table.Column<long>(nullable: false),
                    Answer3 = table.Column<string>(nullable: true),
                    Answer3ChooseHistory = table.Column<long>(nullable: false),
                    Answer4 = table.Column<string>(nullable: true),
                    Answer4ChooseHistory = table.Column<long>(nullable: false),
                    CorrectAnswerNumber = table.Column<int>(nullable: false),
                    CategoryIdsCache = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccessToken = table.Column<string>(nullable: true),
                    AccountId = table.Column<long>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    StateId = table.Column<int>(nullable: false),
                    SourceAppId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    TypeId = table.Column<int>(nullable: false),
                    AvatarCategoryId = table.Column<int>(nullable: true),
                    AvatarTypeId = table.Column<int>(nullable: true),
                    DefaultAvatar = table.Column<bool>(nullable: true),
                    PriceTypeId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    ImageFileId = table.Column<string>(nullable: true),
                    UnlockLevel = table.Column<int>(nullable: true),
                    UnlockSky = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    FileId = table.Column<string>(nullable: true),
                    CreatorAccountId = table.Column<long>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: true),
                    MimeType = table.Column<string>(nullable: true),
                    MimeTypeCategoryId = table.Column<int>(nullable: true),
                    Extension = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BattleInvitations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    InviterAccountId = table.Column<long>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleInvitations_Accounts_InviterAccountId",
                        column: x => x.InviterAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    AchievementTypeId = table.Column<long>(nullable: false),
                    AchieveDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_AchievementTypes_AchievementTypeId",
                        column: x => x.AchievementTypeId,
                        principalTable: "AchievementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountCategoryStats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false),
                    TotalQuestionsCount = table.Column<long>(nullable: false),
                    CorrectAnswersCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCategoryStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountCategoryStats_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    QuestionId = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionCategories_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    ShopItemId = table.Column<long>(nullable: false),
                    ItemTypeId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountItems_ShopItems_ShopItemId",
                        column: x => x.ShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    PaymentDateTime = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    ShopItemId = table.Column<long>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    RedirectUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_ShopItems_ShopItemId",
                        column: x => x.ShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Battles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    BattleType = table.Column<int>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    StartDateTime = table.Column<DateTime>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Player1Id = table.Column<long>(nullable: true),
                    Player2Id = table.Column<long>(nullable: true),
                    Round = table.Column<int>(nullable: true),
                    LastGameId = table.Column<long>(nullable: true),
                    WinnerPlayerId = table.Column<long>(nullable: true),
                    Player1CorrectAnswersCount = table.Column<int>(nullable: true),
                    Player2CorrectAnswersCount = table.Column<int>(nullable: true),
                    BattleStateId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battles_Accounts_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Battles_Accounts_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsArchived = table.Column<bool>(nullable: false),
                    CurrentTurnPlayerId = table.Column<long>(nullable: true),
                    BattleId = table.Column<long>(nullable: false),
                    SelectedCategoryId = table.Column<long>(nullable: true),
                    GameState = table.Column<int>(nullable: false),
                    OneToOneBattleId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Battles_BattleId",
                        column: x => x.BattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Accounts_CurrentTurnPlayerId",
                        column: x => x.CurrentTurnPlayerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Battles_OneToOneBattleId",
                        column: x => x.OneToOneBattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Categories_SelectedCategoryId",
                        column: x => x.SelectedCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameCategories",
                columns: table => new
                {
                    GameId = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCategories", x => new { x.GameId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_GameCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameCategories_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameQuestions",
                columns: table => new
                {
                    GameId = table.Column<long>(nullable: false),
                    QuestionId = table.Column<long>(nullable: false),
                    Player1SelectedAnswer = table.Column<int>(nullable: true),
                    Player2SelectedAnswer = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameQuestions", x => new { x.GameId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_GameQuestions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategoryStats_CategoryId",
                table: "AccountCategoryStats",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountItems_AccountId",
                table: "AccountItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountItems_ShopItemId",
                table: "AccountItems",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_AchievementTypeId",
                table: "Achievements",
                column: "AchievementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleInvitations_InviterAccountId",
                table: "BattleInvitations",
                column: "InviterAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_LastGameId",
                table: "Battles",
                column: "LastGameId");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Player1Id",
                table: "Battles",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Battles_Player2Id",
                table: "Battles",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameCategories_CategoryId",
                table: "GameCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameQuestions_QuestionId",
                table: "GameQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_BattleId",
                table: "Games",
                column: "BattleId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentTurnPlayerId",
                table: "Games",
                column: "CurrentTurnPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OneToOneBattleId",
                table: "Games",
                column: "OneToOneBattleId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_SelectedCategoryId",
                table: "Games",
                column: "SelectedCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AccountId",
                table: "Payments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ShopItemId",
                table: "Payments",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCategories_CategoryId",
                table: "QuestionCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCategories_QuestionId",
                table: "QuestionCategories",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Battles_Games_LastGameId",
                table: "Battles",
                column: "LastGameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Categories_SelectedCategoryId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Battles_Accounts_Player1Id",
                table: "Battles");

            migrationBuilder.DropForeignKey(
                name: "FK_Battles_Accounts_Player2Id",
                table: "Battles");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Accounts_CurrentTurnPlayerId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Battles_Games_LastGameId",
                table: "Battles");

            migrationBuilder.DropTable(
                name: "AccountCategoryStats");

            migrationBuilder.DropTable(
                name: "AccountFriends");

            migrationBuilder.DropTable(
                name: "AccountInvitations");

            migrationBuilder.DropTable(
                name: "AccountItems");

            migrationBuilder.DropTable(
                name: "AccountRoles");

            migrationBuilder.DropTable(
                name: "AccountStatsSummaries");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "BattleInvitations");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "GameCategories");

            migrationBuilder.DropTable(
                name: "GameQuestions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PurchaseLogs");

            migrationBuilder.DropTable(
                name: "QuestionCategories");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "AchievementTypes");

            migrationBuilder.DropTable(
                name: "ShopItems");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Battles");
        }
    }
}
