namespace SICEM_Blazor.Data {
    public interface IEnlace {

        public int Id {get;}
        public string Nombre {get;}

        public string GetConnectionString();
    }
}