using Microsoft.EntityFrameworkCore;
using RedCross_System.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace RedCross_System.Data;

public class ApplicationDbContext : DbContext
{

	private readonly IConfigurationBuilder _builder;

	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
	}

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
		IConfigurationBuilder builder) : base(options)
	{
		_builder = builder;
	}

	public ApplicationDbContext()
	{
	}

	public DbSet<Branch> Branches { get; set; }
	public DbSet<Province> Provinces { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<Donor> Donors { get; set; }
	public DbSet<BloodType> BloodTypes { get; set; }
	public DbSet<Campaign> Campaigns { get; set; }
	public DbSet<Donation> Donations { get; set; }
	public DbSet<Country> Countries { get; set; }
	public DbSet<TestBlood> TestBloods { get; set; }



	//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	//{
	//	optionsBuilder.UseSeeding((context, _) =>
	//					{
	//						var country = new Country { Name = "Nepal" };

	//						var provinces = new List<Province>
	//							{
	//											new Province { Name = "Koshi", Country = country },
	//											new Province { Name = "Madhesh", Country = country },
	//											new Province { Name = "Bagmati", Country = country },
	//											new Province { Name = "Gandaki", Country = country },
	//											new Province { Name = "Lumbini", Country = country },
	//											new Province { Name = "Karnali", Country = country },
	//											new Province { Name = "Sudurpashchim", Country = country }
	//							};

	//						var testProvince = context.Set<Province>().FirstOrDefault();
	//						if (testProvince == null)
	//						{
	//							foreach (var province in provinces)
	//							{
	//								context.Set<Province>().Add(province);
	//							}
	//							context.SaveChanges();
	//						}

	//						List<Role> roles = new List<Role>
	//						{
	//						new(){Id = 1, Name="SuperAdmin"},
	//						new(){Id = 2, Name="ProvinceUser"},
	//						new(){Id = 3, Name="DistrictUser"},
	//						new(){Id = 4, Name="BranchUser"},
	//						};



	//						var role = context.Set<Role>().FirstOrDefault();
	//						if (role == null)
	//						{
	//							foreach (var ro in roles)
	//							{
	//								context.Set<Role>().Add(ro);
	//							}
	//							context.SaveChanges();
	//						}

	//						User user = new User()
	//						{
	//							Name = "Madhuri",
	//							Email = "admin@gmail.com",
	//							Id = 1,
	//							Role = roles[0],
	//							Password = "$2a$12$RtLWqAxupkrPWLRUKn2gquzX1BwAYCPNZz.7lO/fBtCVRp.2h852q",
	//							Phone = "98150999900",

	//						};

	//						var usr = context.Set<User>().FirstOrDefault();
	//						if (usr == null)
	//						{
	//							context.Set<User>().Add(user);
	//							context.SaveChanges();
	//						}

	//						List<BloodType> bloodTypes = new List<BloodType>
	//{
	//	new BloodType { Name = "A+" },
	//	new BloodType { Name = "A-" },
	//	new BloodType { Name = "B+" },
	//	new BloodType { Name = "B-" },
	//	new BloodType { Name = "AB+" },
	//	new BloodType { Name = "AB-" },
	//	new BloodType { Name = "O+" },
	//	new BloodType { Name = "O-" },
	//};

	//						var bloodType = context.Set<BloodType>().FirstOrDefault();
	//						if (bloodType == null)
	//						{
	//							foreach (var bt in bloodTypes)
	//							{
	//								context.Set<BloodType>().Add(bt);
	//							}
	//							context.SaveChanges();
	//						}

	//					})
	//					.UseAsyncSeeding(async (context, _, cancellationToken) =>
	//					{
	//						var country = new Country { Name = "Nepal" };
	//						List<Province> provinces = new List<Province>
	//							{
	//											new Province { Name = "Koshi", Country = country },
	//											new Province { Name = "Madhesh", Country = country },
	//											new Province { Name = "Bagmati", Country = country },
	//											new Province { Name = "Gandaki", Country = country },
	//											new Province { Name = "Lumbini", Country = country },
	//											new Province { Name = "Karnali", Country = country },
	//											new Province { Name = "Sudurpashchim", Country = country }
	//							};

	//						var testProvince = await context.Set<Province>().FirstOrDefaultAsync(cancellationToken);
	//						if (testProvince == null)
	//						{
	//							foreach (var province in provinces)
	//							{
	//								await context.Set<Province>().AddAsync(province, cancellationToken);
	//							}
	//							await context.SaveChangesAsync(cancellationToken);
	//						}
	//						List<Role> roles = new List<Role>
	//						{
	//						new(){Id = 1, Name="SuperAdmin"},
	//						new(){Id = 2, Name="ProvinceUser"},
	//						new(){Id = 3, Name="DistrictUser"},
	//						new(){Id = 4, Name="BranchUser"},
	//						};
	//						var role = context.Set<Role>().FirstOrDefault();
	//						if (role == null)
	//						{
	//							foreach (var ro in roles)
	//							{
	//								context.Set<Role>().Add(ro);
	//							}
	//							context.SaveChanges();
	//						}
	//						User user = new User()
	//						{
	//							Name = "Madhuri",
	//							Email = "admin@gmail.com",
	//							Id = 1,
	//							Role = roles[0],
	//							Password = "$2a$12$RtLWqAxupkrPWLRUKn2gquzX1BwAYCPNZz.7lO/fBtCVRp.2h852q",

	//						};
	//						var usr = context.Set<User>().FirstOrDefault();
	//						if (usr == null)
	//						{
	//							context.Set<User>().Add(user);
	//							context.SaveChanges();
	//						}
	//						List<BloodType> bloodTypes = new List<BloodType>
	//						{
	//							new BloodType { Name = "A+" },
	//							new BloodType { Name = "A-" },
	//							new BloodType { Name = "B+" },
	//							new BloodType { Name = "B-" },
	//							new BloodType { Name = "AB+" },
	//							new BloodType { Name = "AB-" },
	//							new BloodType { Name = "O+" },
	//							new BloodType { Name = "O-" },
	//						};

	//						var bloodType = await context.Set<BloodType>().FirstOrDefaultAsync(cancellationToken);
	//						if (bloodType == null)
	//						{
	//							foreach (var bt in bloodTypes)
	//							{
	//								await context.Set<BloodType>().AddAsync(bt, cancellationToken);
	//							}
	//							await context.SaveChangesAsync(cancellationToken);
	//						}




	//					});
	//}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);


		// Seeding Country
		var country = new Country { Id = 1, Name = "Nepal" };
		modelBuilder.Entity<Country>().HasData(country);

		// Seeding Provinces
		modelBuilder.Entity<Province>().HasData(
				new Province { Id = 1, Name = "Koshi", CountryId = country.Id },
				new Province { Id = 2, Name = "Madhesh", CountryId = country.Id },
				new Province { Id = 3, Name = "Bagmati", CountryId = country.Id },
				new Province { Id = 4, Name = "Gandaki", CountryId = country.Id },
				new Province { Id = 5, Name = "Lumbini", CountryId = country.Id },
				new Province { Id = 6, Name = "Karnali", CountryId = country.Id },
				new Province { Id = 7, Name = "Sudurpashchim", CountryId = country.Id }
		);

		// Seeding Roles
		modelBuilder.Entity<Role>().HasData(
				new Role { Id = 1, Name = "SuperAdmin" },
				new Role { Id = 2, Name = "ProvinceUser" },
				new Role { Id = 3, Name = "DistrictUser" },
				new Role { Id = 4, Name = "BranchUser" }
		);

		// Seeding User
		var superAdminRole = new Role { Id = 1, Name = "SuperAdmin" };  // Make sure the role is added
		modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					Name = "Madhuri",
					Email = "admin@gmail.com",
					Phone = "98150999900",
					RoleId = superAdminRole.Id,
					Password = "$2a$12$RtLWqAxupkrPWLRUKn2gquzX1BwAYCPNZz.7lO/fBtCVRp.2h852q" ,// Make sure password is properly hashed
          PasswordResetToken = "default-reset-token"
				}
		);

		// Seeding Blood Types
		modelBuilder.Entity<BloodType>().HasData(
				new BloodType { Id = 1, Name = "A+" },
				new BloodType { Id = 2, Name = "A-" },
				new BloodType { Id = 3, Name = "B+" },
				new BloodType { Id = 4, Name = "B-" },
				new BloodType { Id = 5, Name = "AB+" },
				new BloodType { Id = 6, Name = "AB-" },
				new BloodType { Id = 7, Name = "O+" },
				new BloodType { Id = 8, Name = "O-" }
		);

		modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
	}

}
