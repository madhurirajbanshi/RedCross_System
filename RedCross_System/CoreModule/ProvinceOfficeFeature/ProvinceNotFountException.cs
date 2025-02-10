namespace RedCross_System.CoreModule.ProvinceFeature
{
	public class ProvinceNotFountException : Exception
	{
		public ProvinceNotFountException(int provinceId)
		{
			ProvinceId = provinceId;
		}
		public int ProvinceId { get; }
		public override string Message
		{
			get
			{
				return "Province with province id " + this.ProvinceId + " not found";
			}
		}
	}
}
