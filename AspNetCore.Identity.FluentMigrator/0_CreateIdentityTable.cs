using FluentMigrator;

namespace AspNetCore.Identity.FluentMigrator;

[Migration(0)]
public class CreateIdentityTable : Migration
{
    public override void Up()
    {
        Create.Table("AspNetRoles")
            .WithColumn("Id").AsString().PrimaryKey().NotNullable()
            .WithColumn("ConcurrencyStamp").AsString().Nullable()
            .WithColumn("Name").AsString(256).Nullable()
            .WithColumn("NormalizedName").AsString(256).Nullable();

        Create.Table("AspNetUserTokens")
            .WithColumn("UserId").AsString().PrimaryKey().NotNullable()
            .WithColumn("LoginProvider").AsString().PrimaryKey().NotNullable()
            .WithColumn("Name").AsString().PrimaryKey().NotNullable()
            .WithColumn("Value").AsString().Nullable();

        Create.Table("AspNetUsers")
            .WithColumn("Id").AsString().PrimaryKey().NotNullable()
            .WithColumn("AccessFailedCount").AsInt32().NotNullable()
            .WithColumn("ConcurrencyStamp").AsString().Nullable()
            .WithColumn("Email").AsString(256).Nullable()
            .WithColumn("EmailConfirmed").AsBoolean().NotNullable()
            .WithColumn("LockoutEnabled").AsBoolean().NotNullable()
            .WithColumn("LockoutEnd").AsDateTimeOffset().Nullable()
            .WithColumn("NormalizedEmail").AsString(256).Nullable()
            .WithColumn("NormalizedUserName").AsString(256).Nullable()
            .WithColumn("PasswordHash").AsString().Nullable()
            .WithColumn("PhoneNumber").AsString().Nullable()
            .WithColumn("PhoneNumberConfirmed").AsBoolean().NotNullable()
            .WithColumn("SecurityStamp").AsString().Nullable()
            .WithColumn("TwoFactorEnabled").AsBoolean().NotNullable()
            .WithColumn("UserName").AsString(256).Nullable();

        Create.Table("AspNetRoleClaims")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ClaimType").AsString().Nullable()
            .WithColumn("ClaimValue").AsString().Nullable()
            .WithColumn("RoleId").AsString().NotNullable()
            .ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", "AspNetRoles", "Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Table("AspNetUserClaims")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ClaimType").AsString().Nullable()
            .WithColumn("ClaimValue").AsString().Nullable()
            .WithColumn("UserId").AsString().NotNullable()
            .ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", "AspNetUsers", "Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Table("AspNetUserLogins")
            .WithColumn("LoginProvider").AsString().PrimaryKey().NotNullable()
            .WithColumn("ProviderKey").AsString().PrimaryKey().NotNullable()
            .WithColumn("ProviderDisplayName").AsString().Nullable()
            .WithColumn("UserId").AsString().NotNullable()
            .ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", "AspNetUsers", "Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Table("AspNetUserRoles")
            .WithColumn("UserId").AsString().PrimaryKey().NotNullable()
            .WithColumn("RoleId").AsString().PrimaryKey().NotNullable()
            .ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", "AspNetRoles", "Id")
            .ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", "AspNetUsers", "Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("RoleNameIndex")
            .OnTable("AspNetRoles")
            .OnColumn("NormalizedName");

        Create.Index("IX_AspNetRoleClaims_RoleId")
            .OnTable("AspNetRoleClaims")
            .OnColumn("RoleId");

        Create.Index("IX_AspNetUserClaims_UserId")
            .OnTable("AspNetUserClaims")
            .OnColumn("UserId");

        Create.Index("IX_AspNetUserLogins_UserId")
            .OnTable("AspNetUserLogins")
            .OnColumn("UserId");

        Create.Index("IX_AspNetUserRoles_RoleId")
            .OnTable("AspNetUserRoles")
            .OnColumn("RoleId");

        Create.Index("IX_AspNetUserRoles_UserId")
            .OnTable("AspNetUserRoles")
            .OnColumn("UserId");

        Create.Index("EmailIndex")
            .OnTable("AspNetUsers")
            .OnColumn("NormalizedEmail");

        Create.Index("UserNameIndex")
            .OnTable("AspNetUsers")
            .OnColumn("NormalizedUserName")
            .Unique();
    }

    public override void Down()
    {
        Delete.Table("AspNetRoleClaims");
        Delete.Table("AspNetUserClaims");
        Delete.Table("AspNetUserLogins");
        Delete.Table("AspNetUserRoles");
        Delete.Table("AspNetUserTokens");
        Delete.Table("AspNetRoles");
        Delete.Table("AspNetUsers");
    }
}