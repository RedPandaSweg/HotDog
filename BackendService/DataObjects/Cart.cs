using Microsoft.Azure.Mobile.Server;

namespace BackendService.DataObjects
{
    public class Cart : EntityData
    {
        public string Identifier { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Position { get; set; }

        public string CartString { get; set; }
    }
}
