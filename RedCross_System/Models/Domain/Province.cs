using System;
using System.ComponentModel.DataAnnotations;

namespace RedCross_System.Models.Domain
{
	public class Province
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CountryId {  get; set; }
		public Country Country { get; set; }
		
	}
}