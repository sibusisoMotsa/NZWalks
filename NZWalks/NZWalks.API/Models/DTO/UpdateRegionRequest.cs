namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public double Lat { get; set; }
        public double Log { get; set; }
        public long Population { get; set; }
    }
}
