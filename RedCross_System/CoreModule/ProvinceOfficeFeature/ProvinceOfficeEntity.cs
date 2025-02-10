using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossSystem.Core.src.ProvinceFeature;
public class ProvinceOfficeEntity
{
    public ProvinceOfficeEntity()
    {
        Status = "Active";
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
}
