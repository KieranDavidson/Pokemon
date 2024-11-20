namespace Pokemon.Server.Models
{
    public class PokemonTypeContainer
    {
        public PokemonType type { get; set; }
    }

    public class PokemonType
    {
        public string name { get; set; }
    }

    public class Pokemon
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<PokemonTypeContainer> types { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int ties { get; set; }
        public int base_experience { get; set; }
    }
}
