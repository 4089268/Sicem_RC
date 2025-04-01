using System.Globalization;

namespace SICEM_Blazor.SeguimientoCobros.Models
{
    public class OfficePushpinMap {

        public int Id {get;set;}
        public string Title {
            get {
                if(Income == 0){
                    return "0";
                }
                return Income.ToString("c2", new CultureInfo("es-MX") );
            }
        }
        public string Office {get;set;}
        public double Lat {get;set;}
        public double Lon {get;set;}
        public int Bills {get;set;}
        public decimal Income {get;set;}
        public double Radius {get;set;}
        

        public OfficePushpinMap(int id, string office)
        {
            this.Id = id;
            this.Office = office;
            this.Lat = 27.905077;
            this.Lon = -101.274589;
            this.Bills = 0;
            this.Income = 0m;
            this.Radius = 600;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Title: {Title}, Office: {Office}, Lat: {Lat}, Lon: {Lon}, Bills: {Bills}, Income: {Income.ToString("c2", new CultureInfo("es-MX"))}";
        }

    }
}