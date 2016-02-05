Use the migration sample in scenario:
You want to migrate a MVC4 project with simple membership settings to use identity API,
AND you want to upgrade the database scheme to identity scheme.

To migrate, follow the following steps:

1. Run SimpleMembershipToIdentityMigration.sql script on the simple membership database
2. Install Microsoft.AspNet.Identity.EntityFramework 1.0.0 package
3. Copy Models/IdentityModels.cs to MVC4 project Models folder
4. Copy Controllers/AccountController.cs to override MVC4 project controller

After the migration, you are able to use default identity EF models and UserManager API 
to manage your memebership database.