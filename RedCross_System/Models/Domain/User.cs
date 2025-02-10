using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.Models.Domain
{
	public class User
	{

		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public Role Role { get; set; }
		public int RoleId { get; set; }
		public BloodType BloodType { get; set; }
		public int BloodTypeId { get; set; }
		public List<Donation> Donations { get; set; } = new List<Donation>();

		public int DonationCount => Donations?.Count ?? 0;

		public DateTime LastDonationDate => Donations?.OrderByDescending(d => d.DonationDate).FirstOrDefault()?.DonationDate ?? DateTime.MinValue;
		public decimal TotalAmount => Donations?.Sum(d => d.Quantity) ?? 0;

	}
}
