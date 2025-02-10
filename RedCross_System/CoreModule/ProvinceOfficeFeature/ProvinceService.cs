using Microsoft.EntityFrameworkCore;
using RedCross_System.CoreModule.ProvinceFeature;
using RedCross_System.CoreModule.ProvinceOfficeFeature;
using RedCross_System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossSystem.Core.src.ProvinceFeature;
public class ProvinceService
{

	private readonly ApplicationDbContext _appDbContext;

	public ProvinceService(ApplicationDbContext context)
	{
		_appDbContext = context;
	}


	public async Task Create(ProvinceCreateDto dto)
	{
		var province = new ProvinceOfficeEntity();
		province.Name = dto.Name;
		province.Description = dto.Description;
		_appDbContext.Add(province);
		await _appDbContext.SaveChangesAsync();
	}


	public async Task Update(ProvinceUpdateDto dto)
	{
		var province = await _appDbContext.ProvinceOfficeEntities.FindAsync(dto.Id);

		if (province == null)
		{
			throw new ProvinceNotFountException(dto.Id);  
		}

		province.Name = dto.Name;
		province.Description = dto.Description;
    await _appDbContext.SaveChangesAsync();
	}

	public async Task ToggleStatus(int id)
	{
		var province = _appDbContext.ProvinceOfficeEntities.Find(id);

		if (province == null)
		{
			throw new ProvinceNotFountException(id);
		}

		province.Status = province.Status == "Active" ? "Inactive" : "Active";

		await _appDbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<ProvinceIndexDto>> GetAll()
	{
		return await _appDbContext.ProvinceOfficeEntities
				.Select(p => new ProvinceIndexDto
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					Status = p.Status
				})
				.ToListAsync();
	}

}
