IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FirstName] nvarchar(80) NOT NULL,
        [LastName] nvarchar(80) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [ActorId] nvarchar(450) NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(100) NOT NULL,
        [Details] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Title] nvarchar(160) NOT NULL,
        [Message] nvarchar(1200) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ReadAt] datetime2 NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [PlatformSettings] (
        [Key] nvarchar(80) NOT NULL,
        [NumericValue] decimal(9,2) NOT NULL,
        CONSTRAINT [PK_PlatformSettings] PRIMARY KEY ([Key])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceAreas] (
        [Id] int NOT NULL IDENTITY,
        [CityMunicipality] nvarchar(120) NOT NULL,
        [Province] nvarchar(120) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_ServiceAreas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Services] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [BasePrice] decimal(10,2) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Services] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [SupportTickets] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(max) NOT NULL,
        [Subject] nvarchar(180) NOT NULL,
        [Message] nvarchar(3000) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_SupportTickets] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Addresses] (
        [Id] int NOT NULL IDENTITY,
        [Street] nvarchar(200) NOT NULL,
        [Barangay] nvarchar(100) NOT NULL,
        [CityMunicipality] nvarchar(100) NOT NULL,
        [Province] nvarchar(100) NOT NULL,
        [Region] nvarchar(100) NOT NULL,
        [Landmark] nvarchar(200) NULL,
        [IsDefault] bit NOT NULL,
        [CustomerId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Addresses_AspNetUsers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [CustomerProfiles] (
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_CustomerProfiles] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_CustomerProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [PrivacyRequests] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Approved] bit NULL,
        [ReviewedById] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ReviewedAt] datetime2 NULL,
        CONSTRAINT [PK_PrivacyRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PrivacyRequests_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [TechnicianAvailabilities] (
        [Id] int NOT NULL IDENTITY,
        [TechnicianId] nvarchar(450) NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [IsAvailable] bit NOT NULL,
        CONSTRAINT [PK_TechnicianAvailabilities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TechnicianAvailabilities_AspNetUsers_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [TechnicianProfiles] (
        [UserId] nvarchar(450) NOT NULL,
        [Skills] nvarchar(1000) NOT NULL,
        [CoverageAreas] nvarchar(300) NOT NULL,
        [Status] int NOT NULL,
        [IsOnline] bit NOT NULL,
        [DecisionReason] nvarchar(max) NULL,
        [ReviewedAt] datetime2 NULL,
        CONSTRAINT [PK_TechnicianProfiles] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_TechnicianProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [VerificationRecords] (
        [Id] int NOT NULL IDENTITY,
        [TechnicianId] nvarchar(450) NOT NULL,
        [DocumentType] nvarchar(60) NOT NULL,
        [PrivateStorageKey] nvarchar(500) NOT NULL,
        [Status] int NOT NULL,
        [DecisionReason] nvarchar(500) NULL,
        [ReviewedById] nvarchar(max) NULL,
        [SubmittedAt] datetime2 NOT NULL,
        [ReviewedAt] datetime2 NULL,
        CONSTRAINT [PK_VerificationRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VerificationRecords_AspNetUsers_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Bookings] (
        [Id] int NOT NULL IDENTITY,
        [Reference] nvarchar(30) NOT NULL,
        [CustomerId] nvarchar(450) NOT NULL,
        [TechnicianId] nvarchar(450) NULL,
        [AddressId] int NOT NULL,
        [ServiceId] int NOT NULL,
        [ProblemDescription] nvarchar(2000) NOT NULL,
        [ScheduledAt] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [EstimatedTotal] decimal(10,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bookings_Addresses_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_AspNetUsers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_AspNetUsers_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [AdditionalWorkRequests] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [TechnicianId] nvarchar(max) NOT NULL,
        [Description] nvarchar(800) NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [DecidedAt] datetime2 NULL,
        CONSTRAINT [PK_AdditionalWorkRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdditionalWorkRequests_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [BookingAssignments] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [TechnicianId] nvarchar(max) NOT NULL,
        [AssignedAt] datetime2 NOT NULL,
        [AssignedById] nvarchar(max) NOT NULL,
        [Note] nvarchar(300) NULL,
        CONSTRAINT [PK_BookingAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingAssignments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [BookingPhotos] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [UploadedById] nvarchar(max) NOT NULL,
        [PrivateStorageKey] nvarchar(500) NOT NULL,
        [ContentType] nvarchar(10) NOT NULL,
        [Phase] nvarchar(20) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_BookingPhotos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingPhotos_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [BookingStatusHistories] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [Status] int NOT NULL,
        [ChangedByUserId] nvarchar(450) NOT NULL,
        [ChangedAt] datetime2 NOT NULL,
        [Note] nvarchar(500) NULL,
        CONSTRAINT [PK_BookingStatusHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingStatusHistories_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Disputes] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [ReportedById] nvarchar(max) NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [Status] int NOT NULL,
        [Resolution] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Disputes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Disputes_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [Method] int NOT NULL,
        [Status] int NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [TransactionReference] nvarchar(48) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [PaidAt] datetime2 NULL,
        [IsSimulation] bit NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [PriceLines] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [Label] nvarchar(120) NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_PriceLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PriceLines_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [CustomerId] nvarchar(max) NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(1200) NULL,
        [State] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE TABLE [RefundRequests] (
        [Id] int NOT NULL IDENTITY,
        [PaymentId] int NOT NULL,
        [RequestedById] nvarchar(max) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [Approved] bit NULL,
        [ReviewedById] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_RefundRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefundRequests_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''fixit-admin-role'', N''fixit-admin-role'', N''Administrator'', N''ADMINISTRATOR''),
    (N''fixit-customer-role'', N''fixit-customer-role'', N''Customer'', N''CUSTOMER''),
    (N''fixit-technician-role'', N''fixit-technician-role'', N''Technician'', N''TECHNICIAN'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Key', N'NumericValue') AND [object_id] = OBJECT_ID(N'[PlatformSettings]'))
        SET IDENTITY_INSERT [PlatformSettings] ON;
    EXEC(N'INSERT INTO [PlatformSettings] ([Key], [NumericValue])
    VALUES (N''CancellationHours'', 24.0),
    (N''PlatformFeePercent'', 15.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Key', N'NumericValue') AND [object_id] = OBJECT_ID(N'[PlatformSettings]'))
        SET IDENTITY_INSERT [PlatformSettings] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CityMunicipality', N'IsActive', N'Province') AND [object_id] = OBJECT_ID(N'[ServiceAreas]'))
        SET IDENTITY_INSERT [ServiceAreas] ON;
    EXEC(N'INSERT INTO [ServiceAreas] ([Id], [CityMunicipality], [IsActive], [Province])
    VALUES (1, N''Makati'', CAST(1 AS bit), N''Metro Manila''),
    (2, N''Quezon City'', CAST(1 AS bit), N''Metro Manila''),
    (3, N''Pasig'', CAST(1 AS bit), N''Metro Manila''),
    (4, N''Cebu City'', CAST(1 AS bit), N''Cebu'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CityMunicipality', N'IsActive', N'Province') AND [object_id] = OBJECT_ID(N'[ServiceAreas]'))
        SET IDENTITY_INSERT [ServiceAreas] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BasePrice', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Services]'))
        SET IDENTITY_INSERT [Services] ON;
    EXEC(N'INSERT INTO [Services] ([Id], [BasePrice], [Description], [IsActive], [Name])
    VALUES (1, 450.0, N''Cleaning, inspection, and repair for home air conditioning units.'', CAST(1 AS bit), N''Air conditioning''),
    (2, 350.0, N''Diagnosis and repair for household refrigerators.'', CAST(1 AS bit), N''Refrigerator repair''),
    (3, 350.0, N''Troubleshooting and repair for washing machines.'', CAST(1 AS bit), N''Washing machine repair''),
    (4, 400.0, N''Minor home electrical repairs and fixture troubleshooting.'', CAST(1 AS bit), N''Electrical repairs''),
    (5, 350.0, N''Leak, drain, and fixture maintenance.'', CAST(1 AS bit), N''Plumbing''),
    (6, 500.0, N''In-home installation of household appliances.'', CAST(1 AS bit), N''Appliance installation''),
    (7, 450.0, N''Minor carpentry and furniture repair.'', CAST(1 AS bit), N''Carpentry and furniture''),
    (8, 500.0, N''Small-area interior paint touch-ups and maintenance.'', CAST(1 AS bit), N''Painting''),
    (9, 400.0, N''General maintenance inspection and minor fixes.'', CAST(1 AS bit), N''General home maintenance'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BasePrice', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Services]'))
        SET IDENTITY_INSERT [Services] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AdditionalWorkRequests_BookingId] ON [AdditionalWorkRequests] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Addresses_CustomerId] ON [Addresses] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BookingAssignments_BookingId] ON [BookingAssignments] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BookingPhotos_BookingId] ON [BookingPhotos] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_AddressId] ON [Bookings] ([AddressId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_CustomerId] ON [Bookings] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bookings_Reference] ON [Bookings] ([Reference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_ServiceId] ON [Bookings] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bookings_TechnicianId_ScheduledAt] ON [Bookings] ([TechnicianId], [ScheduledAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BookingStatusHistories_BookingId] ON [BookingStatusHistories] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Disputes_BookingId] ON [Disputes] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId_ReadAt] ON [Notifications] ([UserId], [ReadAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PriceLines_BookingId] ON [PriceLines] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PrivacyRequests_UserId] ON [PrivacyRequests] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RefundRequests_PaymentId] ON [RefundRequests] ([PaymentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reviews_BookingId] ON [Reviews] ([BookingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ServiceAreas_CityMunicipality_Province] ON [ServiceAreas] ([CityMunicipality], [Province]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TechnicianAvailabilities_TechnicianId] ON [TechnicianAvailabilities] ([TechnicianId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_VerificationRecords_TechnicianId] ON [VerificationRecords] ([TechnicianId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025172238_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261025172238_InitialCreate', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173305_AddTechnicianJobDeclines'
)
BEGIN
    CREATE TABLE [TechnicianJobDeclines] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [TechnicianId] nvarchar(450) NOT NULL,
        [Reason] nvarchar(300) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_TechnicianJobDeclines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TechnicianJobDeclines_AspNetUsers_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TechnicianJobDeclines_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173305_AddTechnicianJobDeclines'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TechnicianJobDeclines_BookingId_TechnicianId] ON [TechnicianJobDeclines] ([BookingId], [TechnicianId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173305_AddTechnicianJobDeclines'
)
BEGIN
    CREATE INDEX [IX_TechnicianJobDeclines_TechnicianId] ON [TechnicianJobDeclines] ([TechnicianId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173305_AddTechnicianJobDeclines'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261025173305_AddTechnicianJobDeclines', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173501_AddBookingWorkReport'
)
BEGIN
    ALTER TABLE [Bookings] ADD [CompletionNotes] nvarchar(4000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173501_AddBookingWorkReport'
)
BEGIN
    ALTER TABLE [Bookings] ADD [PartsUsed] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173501_AddBookingWorkReport'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261025173501_AddBookingWorkReport', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173856_NormalizeTechnicianSkills'
)
BEGIN
    CREATE TABLE [TechnicianSkills] (
        [TechnicianId] nvarchar(450) NOT NULL,
        [ServiceId] int NOT NULL,
        CONSTRAINT [PK_TechnicianSkills] PRIMARY KEY ([TechnicianId], [ServiceId]),
        CONSTRAINT [FK_TechnicianSkills_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TechnicianSkills_TechnicianProfiles_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [TechnicianProfiles] ([UserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173856_NormalizeTechnicianSkills'
)
BEGIN
    CREATE INDEX [IX_TechnicianSkills_ServiceId] ON [TechnicianSkills] ([ServiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025173856_NormalizeTechnicianSkills'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261025173856_NormalizeTechnicianSkills', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025174245_NormalizeTechnicianCoverageAreas'
)
BEGIN
    CREATE TABLE [TechnicianCoverageAreas] (
        [TechnicianId] nvarchar(450) NOT NULL,
        [ServiceAreaId] int NOT NULL,
        CONSTRAINT [PK_TechnicianCoverageAreas] PRIMARY KEY ([TechnicianId], [ServiceAreaId]),
        CONSTRAINT [FK_TechnicianCoverageAreas_ServiceAreas_ServiceAreaId] FOREIGN KEY ([ServiceAreaId]) REFERENCES [ServiceAreas] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TechnicianCoverageAreas_TechnicianProfiles_TechnicianId] FOREIGN KEY ([TechnicianId]) REFERENCES [TechnicianProfiles] ([UserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025174245_NormalizeTechnicianCoverageAreas'
)
BEGIN
    CREATE INDEX [IX_TechnicianCoverageAreas_ServiceAreaId] ON [TechnicianCoverageAreas] ([ServiceAreaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025174245_NormalizeTechnicianCoverageAreas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261025174245_NormalizeTechnicianCoverageAreas', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025175037_SnapshotPaymentFeeAllocation'
)
BEGIN
    ALTER TABLE [Payments] ADD [PlatformFeeAmount] decimal(10,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025175037_SnapshotPaymentFeeAllocation'
)
BEGIN
    ALTER TABLE [Payments] ADD [PlatformFeePercent] decimal(9,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025175037_SnapshotPaymentFeeAllocation'
)
BEGIN
    ALTER TABLE [Payments] ADD [TechnicianEarnings] decimal(10,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025175037_SnapshotPaymentFeeAllocation'
)
BEGIN
    UPDATE p
    SET PlatformFeePercent = COALESCE(s.NumericValue, 15.00),
        PlatformFeeAmount = ROUND(p.Amount * COALESCE(s.NumericValue, 15.00) / 100.00, 2),
        TechnicianEarnings = p.Amount - ROUND(p.Amount * COALESCE(s.NumericValue, 15.00) / 100.00, 2)
    FROM Payments AS p
    LEFT JOIN PlatformSettings AS s ON s.[Key] = 'PlatformFeePercent'
    WHERE p.Status = 2;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261025175037_SnapshotPaymentFeeAllocation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261025175037_SnapshotPaymentFeeAllocation', N'10.0.13');
END;

COMMIT;
GO

