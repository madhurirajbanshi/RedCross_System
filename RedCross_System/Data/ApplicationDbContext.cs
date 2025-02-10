using Microsoft.EntityFrameworkCore;
using RedCross_System.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using RedCrossSystem.Core.src.ProvinceFeature;

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
	public DbSet<BloodIssue> BloodIssues { get; set; }
	public DbSet<BloodRequirement> BloodRequirements { get; set; }
	public DbSet<ProvinceOfficeEntity> ProvinceOfficeEntities { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<ProvinceOfficeEntity>().ToTable("ProvinceOfficeEntities");

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
				new Role { Id = 4, Name = "BranchUser" },
				new Role { Id=5,Name="NormalUser"}
		);

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

		var superAdminRole = new Role { Id = 1, Name = "SuperAdmin" };

		modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					Name = "Madhuri",
					Email = "admin@gmail.com",
					Phone = "98150999900",
					RoleId = superAdminRole.Id,
					Password = "$2a$12$RtLWqAxupkrPWLRUKn2gquzX1BwAYCPNZz.7lO/fBtCVRp.2h852q" ,
					BloodTypeId = 1,
					

				}
		);

	
	

		modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
	}

}
