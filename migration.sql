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
CREATE TABLE [Admins] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [Categories] (
    [CategoryId] int NOT NULL IDENTITY,
    [ParentCategoryId] int NULL,
    [CategoryName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId]),
    CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE NO ACTION
);

CREATE TABLE [Countries] (
    [Id] int NOT NULL IDENTITY,
    [CountryName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Countries] PRIMARY KEY ([Id])
);

CREATE TABLE [OrderStatuses] (
    [Id] int NOT NULL IDENTITY,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_OrderStatuses] PRIMARY KEY ([Id])
);

CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id])
);

CREATE TABLE [Promotions] (
    [PromotionId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [DiscountRate] decimal(18,2) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([PromotionId])
);

CREATE TABLE [Sellers] (
    [SellerId] nvarchar(450) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [RegistrationDate] datetime2 NOT NULL,
    [EmailConfirmed] bit NOT NULL,
    CONSTRAINT [PK_Sellers] PRIMARY KEY ([SellerId])
);

CREATE TABLE [ShippingMethods] (
    [ShippingMethodId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ShippingMethods] PRIMARY KEY ([ShippingMethodId])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE
);

CREATE TABLE [Variations] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Variations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Variations_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE
);

CREATE TABLE [PromotionCategories] (
    [CategoryId] int NOT NULL,
    [PromotionId] int NOT NULL,
    CONSTRAINT [PK_PromotionCategories] PRIMARY KEY ([PromotionId], [CategoryId]),
    CONSTRAINT [FK_PromotionCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PromotionCategories_Promotions_PromotionId] FOREIGN KEY ([PromotionId]) REFERENCES [Promotions] ([PromotionId]) ON DELETE CASCADE
);

CREATE TABLE [ProductItems] (
    [Id] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [SKU] nvarchar(max) NOT NULL,
    [QtyInStock] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [ProductImage] nvarchar(max) NOT NULL,
    [SellerId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_ProductItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductItems_Sellers_SellerId] FOREIGN KEY ([SellerId]) REFERENCES [Sellers] ([SellerId]) ON DELETE CASCADE
);

CREATE TABLE [ProductVariationCategories] (
    [CategoryId] int NOT NULL,
    [VariationId] int NOT NULL,
    CONSTRAINT [PK_ProductVariationCategories] PRIMARY KEY ([CategoryId], [VariationId]),
    CONSTRAINT [FK_ProductVariationCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductVariationCategories_Variations_VariationId] FOREIGN KEY ([VariationId]) REFERENCES [Variations] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [VariationOptions] (
    [Id] int NOT NULL IDENTITY,
    [VariationId] int NOT NULL,
    [Value] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_VariationOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VariationOptions_Variations_VariationId] FOREIGN KEY ([VariationId]) REFERENCES [Variations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProductConfigurations] (
    [ProductConfigurationId] int NOT NULL IDENTITY,
    [ProductItemId] int NOT NULL,
    [VariationOptionId] int NOT NULL,
    CONSTRAINT [PK_ProductConfigurations] PRIMARY KEY ([ProductConfigurationId]),
    CONSTRAINT [FK_ProductConfigurations_ProductItems_ProductItemId] FOREIGN KEY ([ProductItemId]) REFERENCES [ProductItems] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductConfigurations_VariationOptions_VariationOptionId] FOREIGN KEY ([VariationOptionId]) REFERENCES [VariationOptions] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey])
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [RegistrationDate] datetime2 NOT NULL,
    [CartId] int NOT NULL,
    [ShoppingCartCartId] int NOT NULL,
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

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Carts] (
    [CartId] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([CartId]),
    CONSTRAINT [FK_Carts_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [PaymentMethods] (
    [PaymentMethodId] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [PaymentTypeId] int NOT NULL,
    [Provider] nvarchar(max) NOT NULL,
    [AccountNumber] nvarchar(max) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [IsDefault] bit NOT NULL,
    CONSTRAINT [PK_PaymentMethods] PRIMARY KEY ([PaymentMethodId]),
    CONSTRAINT [FK_PaymentMethods_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentMethods_Payments_PaymentTypeId] FOREIGN KEY ([PaymentTypeId]) REFERENCES [Payments] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ShippingAddresses] (
    [UserAddressId] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [AddressLine1] nvarchar(max) NOT NULL,
    [AddressLine2] nvarchar(max) NOT NULL,
    [City] nvarchar(max) NOT NULL,
    [Region] nvarchar(max) NOT NULL,
    [PostalCode] nvarchar(max) NOT NULL,
    [CountryId] int NOT NULL,
    [IsDefault] bit NOT NULL,
    CONSTRAINT [PK_ShippingAddresses] PRIMARY KEY ([UserAddressId]),
    CONSTRAINT [FK_ShippingAddresses_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ShippingAddresses_Countries_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Countries] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CartItems] (
    [CartItemId] int NOT NULL IDENTITY,
    [CartId] int NOT NULL,
    [ProductItemId] int NOT NULL,
    [Qty] int NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([CartItemId]),
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([CartId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_ProductItems_ProductItemId] FOREIGN KEY ([ProductItemId]) REFERENCES [ProductItems] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Orders] (
    [OrderId] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [PaymentMethodId] int NOT NULL,
    [ShippingAddressId] int NOT NULL,
    [ShippingMethodId] int NOT NULL,
    [OrderTotal] decimal(18,2) NOT NULL,
    [OrderStatusId] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_OrderStatuses_OrderStatusId] FOREIGN KEY ([OrderStatusId]) REFERENCES [OrderStatuses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_PaymentMethods_PaymentMethodId] FOREIGN KEY ([PaymentMethodId]) REFERENCES [PaymentMethods] ([PaymentMethodId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_ShippingAddresses_ShippingAddressId] FOREIGN KEY ([ShippingAddressId]) REFERENCES [ShippingAddresses] ([UserAddressId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_ShippingMethods_ShippingMethodId] FOREIGN KEY ([ShippingMethodId]) REFERENCES [ShippingMethods] ([ShippingMethodId]) ON DELETE NO ACTION
);

CREATE TABLE [OrderLines] (
    [OrderLineId] int NOT NULL IDENTITY,
    [ProductItemId] int NOT NULL,
    [OrderId] int NOT NULL,
    [Qty] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderLines] PRIMARY KEY ([OrderLineId]),
    CONSTRAINT [FK_OrderLines_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderLines_ProductItems_ProductItemId] FOREIGN KEY ([ProductItemId]) REFERENCES [ProductItems] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reviews] (
    [UserReviewId] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [RatingValue] int NOT NULL,
    [Comment] nvarchar(max) NOT NULL,
    [OrderLineId] int NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([UserReviewId]),
    CONSTRAINT [FK_Reviews_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_OrderLines_OrderLineId] FOREIGN KEY ([OrderLineId]) REFERENCES [OrderLines] ([OrderLineId]) ON DELETE NO ACTION
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE INDEX [IX_AspNetUsers_ShoppingCartCartId] ON [AspNetUsers] ([ShoppingCartCartId]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_CartItems_CartId] ON [CartItems] ([CartId]);

CREATE INDEX [IX_CartItems_ProductItemId] ON [CartItems] ([ProductItemId]);

CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]);

CREATE INDEX [IX_Categories_ParentCategoryId] ON [Categories] ([ParentCategoryId]);

CREATE INDEX [IX_OrderLines_OrderId] ON [OrderLines] ([OrderId]);

CREATE INDEX [IX_OrderLines_ProductItemId] ON [OrderLines] ([ProductItemId]);

CREATE INDEX [IX_Orders_OrderStatusId] ON [Orders] ([OrderStatusId]);

CREATE INDEX [IX_Orders_PaymentMethodId] ON [Orders] ([PaymentMethodId]);

CREATE INDEX [IX_Orders_ShippingAddressId] ON [Orders] ([ShippingAddressId]);

CREATE INDEX [IX_Orders_ShippingMethodId] ON [Orders] ([ShippingMethodId]);

CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);

CREATE INDEX [IX_PaymentMethods_PaymentTypeId] ON [PaymentMethods] ([PaymentTypeId]);

CREATE INDEX [IX_PaymentMethods_UserId] ON [PaymentMethods] ([UserId]);

CREATE INDEX [IX_ProductConfigurations_ProductItemId] ON [ProductConfigurations] ([ProductItemId]);

CREATE INDEX [IX_ProductConfigurations_VariationOptionId] ON [ProductConfigurations] ([VariationOptionId]);

CREATE INDEX [IX_ProductItems_ProductId] ON [ProductItems] ([ProductId]);

CREATE INDEX [IX_ProductItems_SellerId] ON [ProductItems] ([SellerId]);

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);

CREATE INDEX [IX_ProductVariationCategories_VariationId] ON [ProductVariationCategories] ([VariationId]);

CREATE INDEX [IX_PromotionCategories_CategoryId] ON [PromotionCategories] ([CategoryId]);

CREATE INDEX [IX_Reviews_OrderLineId] ON [Reviews] ([OrderLineId]);

CREATE INDEX [IX_Reviews_UserId] ON [Reviews] ([UserId]);

CREATE INDEX [IX_ShippingAddresses_CountryId] ON [ShippingAddresses] ([CountryId]);

CREATE INDEX [IX_ShippingAddresses_UserId] ON [ShippingAddresses] ([UserId]);

CREATE INDEX [IX_VariationOptions_VariationId] ON [VariationOptions] ([VariationId]);

CREATE INDEX [IX_Variations_CategoryId] ON [Variations] ([CategoryId]);

ALTER TABLE [AspNetUserClaims] ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;

ALTER TABLE [AspNetUserLogins] ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;

ALTER TABLE [AspNetUserRoles] ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;

ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Carts_ShoppingCartCartId] FOREIGN KEY ([ShoppingCartCartId]) REFERENCES [Carts] ([CartId]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241202192139_initial', N'9.0.1');

ALTER TABLE [AspNetUsers] DROP CONSTRAINT [FK_AspNetUsers_Carts_ShoppingCartCartId];

DROP INDEX [IX_AspNetUsers_ShoppingCartCartId] ON [AspNetUsers];

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'ShoppingCartCartId');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [ShoppingCartCartId];

CREATE TABLE [WishlistItems] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ProductItemId] int NOT NULL,
    [AddedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_WishlistItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WishlistItems_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WishlistItems_ProductItems_ProductItemId] FOREIGN KEY ([ProductItemId]) REFERENCES [ProductItems] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_WishlistItems_ProductItemId] ON [WishlistItems] ([ProductItemId]);

CREATE INDEX [IX_WishlistItems_UserId] ON [WishlistItems] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241206154741_wishlistAdded', N'9.0.1');

ALTER TABLE [WishlistItems] ADD [IsNotified] bit NOT NULL DEFAULT CAST(0 AS bit);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250202143019_ModifyingDeployedDB', N'9.0.1');

COMMIT;
GO

