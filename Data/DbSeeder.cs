using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Models;

namespace VehicleShield.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Synchronize migration history if database already exists but history is empty
            try
            {
                // Create __EFMigrationsHistory if it doesn't exist
                await context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                    BEGIN
                        CREATE TABLE [__EFMigrationsHistory] (
                            [MigrationId] nvarchar(150) NOT NULL,
                            [ProductVersion] nvarchar(32) NOT NULL,
                            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                        );
                    END;
                ");

                // Check if AspNetRoles table exists
                var hasAspNetRoles = false;
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT TOP 1 1 FROM [AspNetRoles]");
                    hasAspNetRoles = true;
                }
                catch
                {
                    hasAspNetRoles = false;
                }

                if (hasAspNetRoles)
                {
                    // Seed the migration history for existing tables
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260614110825_InitialCreate')
                        BEGIN
                            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                            VALUES ('20260614110825_InitialCreate', '9.0.0');
                        END;
                        
                        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260617181614_AddInsurancePlans')
                        BEGIN
                            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                            VALUES ('20260617181614_AddInsurancePlans', '9.0.0');
                        END;
                    ");
                }
            }
            catch (Exception)
            {
                // Ignore seeding sync issues and let MigrateAsync handle it
            }

            // Migrate database automatically
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@vehicleshield.in",
                    FullName = "System Administrator",
                    Role = "Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "admin123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Customer User
            var customerUser = await userManager.FindByNameAsync("customer");
            Customer? customerProfile = null;
            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = "customer",
                    Email = "customer@vehicleshield.in",
                    FullName = "Rajesh Kumar",
                    Role = "Customer",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(customerUser, "cust123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");

                    // Create Customer profile
                    customerProfile = new Customer
                    {
                        CustomerName = "Rajesh Kumar",
                        CustomerAddress = "123, Connaught Place, New Delhi",
                        CustomerPhone = "9876543210",
                        UserId = customerUser.Id
                    };
                    context.Customers.Add(customerProfile);
                    await context.SaveChangesAsync();

                    // Update User link
                    customerUser.CustomerId = customerProfile.CustomerId;
                    await userManager.UpdateAsync(customerUser);
                }
            }
            else
            {
                customerProfile = await context.Customers.FirstOrDefaultAsync(c => c.UserId == customerUser.Id);
            }

            // Seed Sample Data if empty
            if (customerProfile != null && !context.Vehicles.Any())
            {
                // Seed Vehicles
                var vehicle1 = new Vehicle
                {
                    VehicleName = "Toyota Fortuner",
                    OwnerName = "Rajesh Kumar",
                    VehicleModel = "Fortuner 2.8L",
                    VehicleVersion = "4x4 AT",
                    VehicleRate = 3800000,
                    BodyNumber = "TFT123456789",
                    EngineNumber = "ENG987654321",
                    VehicleNumber = "DL3C-AB-1234",
                    CustomerId = customerProfile.CustomerId
                };

                var vehicle2 = new Vehicle
                {
                    VehicleName = "Hyundai Creta",
                    OwnerName = "Rajesh Kumar",
                    VehicleModel = "Creta SX",
                    VehicleVersion = "1.5L Petrol",
                    VehicleRate = 1500000,
                    BodyNumber = "CRT987654321",
                    EngineNumber = "ENG123456789",
                    VehicleNumber = "DL3C-XY-5678",
                    CustomerId = customerProfile.CustomerId
                };

                context.Vehicles.AddRange(vehicle1, vehicle2);
                await context.SaveChangesAsync();

                // Seed Policies
                var policy1 = new Policy
                {
                    PolicyNumber = "VS-2024-00892",
                    PolicyDate = DateTime.Now.AddMonths(-6),
                    PolicyDurationYears = 1,
                    PolicyEndDate = DateTime.Now.AddMonths(6),
                    PolicyType = "Comprehensive",
                    Status = "Active",
                    CustomerId = customerProfile.CustomerId,
                    CustomerName = customerProfile.CustomerName,
                    CustomerAddress = customerProfile.CustomerAddress,
                    CustomerPhone = customerProfile.CustomerPhone,
                    CustomerAddProve = "Aadhaar Card: 1234-5678-9012",
                    VehicleId = vehicle1.VehicleId,
                    VehicleNumber = vehicle1.VehicleNumber,
                    VehicleName = vehicle1.VehicleName,
                    VehicleModel = vehicle1.VehicleModel,
                    VehicleVersion = vehicle1.VehicleVersion,
                    VehicleRate = vehicle1.VehicleRate,
                    VehicleWarranty = "3 Years / 100,000 km",
                    VehicleBodyNumber = vehicle1.BodyNumber,
                    VehicleEngineNumber = vehicle1.EngineNumber
                };

                var policy2 = new Policy
                {
                    PolicyNumber = "VS-2023-00104",
                    PolicyDate = DateTime.Now.AddMonths(-13),
                    PolicyDurationYears = 1,
                    PolicyEndDate = DateTime.Now.AddMonths(-1),
                    PolicyType = "Third Party",
                    Status = "Expired",
                    CustomerId = customerProfile.CustomerId,
                    CustomerName = customerProfile.CustomerName,
                    CustomerAddress = customerProfile.CustomerAddress,
                    CustomerPhone = customerProfile.CustomerPhone,
                    CustomerAddProve = "Voter ID: XYZ987654",
                    VehicleId = vehicle2.VehicleId,
                    VehicleNumber = vehicle2.VehicleNumber,
                    VehicleName = vehicle2.VehicleName,
                    VehicleModel = vehicle2.VehicleModel,
                    VehicleVersion = vehicle2.VehicleVersion,
                    VehicleRate = vehicle2.VehicleRate,
                    VehicleWarranty = "2 Years",
                    VehicleBodyNumber = vehicle2.BodyNumber,
                    VehicleEngineNumber = vehicle2.EngineNumber
                };

                context.Policies.AddRange(policy1, policy2);
                await context.SaveChangesAsync();

                // Seed Billings
                var billing1 = new Billing
                {
                    CustomerId = customerProfile.CustomerId,
                    CustomerName = customerProfile.CustomerName,
                    PolicyId = policy1.PolicyId,
                    PolicyNumber = policy1.PolicyNumber,
                    CustomerAddProve = policy1.CustomerAddProve,
                    CustomerPhone = customerProfile.CustomerPhone,
                    BillNo = "BILL-89211",
                    VehicleName = vehicle1.VehicleName,
                    VehicleModel = vehicle1.VehicleModel,
                    VehicleRate = vehicle1.VehicleRate,
                    VehicleBodyNumber = vehicle1.BodyNumber,
                    VehicleEngineNumber = vehicle1.EngineNumber,
                    BillDate = policy1.PolicyDate,
                    Amount = 28500,
                    PaymentStatus = "Paid"
                };

                var billing2 = new Billing
                {
                    CustomerId = customerProfile.CustomerId,
                    CustomerName = customerProfile.CustomerName,
                    PolicyId = policy2.PolicyId,
                    PolicyNumber = policy2.PolicyNumber,
                    CustomerAddProve = policy2.CustomerAddProve,
                    CustomerPhone = customerProfile.CustomerPhone,
                    BillNo = "BILL-10423",
                    VehicleName = vehicle2.VehicleName,
                    VehicleModel = vehicle2.VehicleModel,
                    VehicleRate = vehicle2.VehicleRate,
                    VehicleBodyNumber = vehicle2.BodyNumber,
                    VehicleEngineNumber = vehicle2.EngineNumber,
                    BillDate = policy2.PolicyDate,
                    Amount = 4500,
                    PaymentStatus = "Paid"
                };

                context.Billings.AddRange(billing1, billing2);
                await context.SaveChangesAsync();

                // Seed Claim
                var claim = new Claim
                {
                    ClaimNumber = "CLM-10024",
                    PolicyId = policy1.PolicyId,
                    PolicyNumber = policy1.PolicyNumber,
                    PolicyStartDate = policy1.PolicyDate,
                    PolicyEndDate = policy1.PolicyEndDate,
                    CustomerName = customerProfile.CustomerName,
                    PlaceOfAccident = "Connaught Place, New Delhi",
                    DateOfAccident = DateTime.Now.AddMonths(-3),
                    InsuredAmount = 3800000,
                    ClaimableAmount = 45000,
                    Status = "Approved",
                    DateFiled = DateTime.Now.AddMonths(-3)
                };

                context.Claims.Add(claim);
                await context.SaveChangesAsync();
            }


            // Seed Insurance Plans if empty
            if (!context.InsurancePlans.Any())
            {
                context.InsurancePlans.AddRange(
                    new InsurancePlan
                    {
                        PlanName = "Comprehensive Cover",
                        Description = "Complete protection including own damage, third-party liability, and personal accident cover.",
                        Price = 5.0m,
                        Features = "Own Damage Coverage, Theft Protection, Fire & Natural Disasters, Third-Party Liability, Personal Accident Cover",
                        IsPopular = true,
                        IsActive = true
                    },
                    new InsurancePlan
                    {
                        PlanName = "Third Party Cover",
                        Description = "Basic legal compliance plan covering damages to third-party properties and persons.",
                        Price = 1.5m,
                        Features = "Third-Party Property Damage, Third-Party Injury Cover, Legal Compliance, Quick Settlement",
                        IsPopular = false,
                        IsActive = true
                    },
                    new InsurancePlan
                    {
                        PlanName = "Premium Elite",
                        Description = "Top-tier premium plan with zero depreciation, roadside assistance, engine protection and key replacement cover.",
                        Price = 8.0m,
                        Features = "Zero Depreciation, Roadside Assistance, Engine Protection, Key Replacement Cover, High Claim Priority",
                        IsPopular = false,
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Expenses if empty
            if (!context.Expenses.Any())
            {
                context.Expenses.AddRange(
                    new Expense { DateOfExpense = DateTime.Now.AddDays(-20), TypeOfExpense = "Office Rent", AmountOfExpense = 75000 },
                    new Expense { DateOfExpense = DateTime.Now.AddDays(-15), TypeOfExpense = "Electricity Bill", AmountOfExpense = 12000 },
                    new Expense { DateOfExpense = DateTime.Now.AddDays(-5), TypeOfExpense = "Stationery", AmountOfExpense = 3500 },
                    new Expense { DateOfExpense = DateTime.Now.AddDays(-2), TypeOfExpense = "Marketing Banner", AmountOfExpense = 15000 }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
