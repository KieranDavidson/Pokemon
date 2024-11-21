using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pokemon.Server.Models;
using System;
using System.Text;

namespace Pokemon.Server.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokemonController : ControllerBase
    {
        Random random = new Random();

        [HttpGet("tournament/statistics")]
        public IEnumerable<Pokemon.Server.Models.Pokemon> Get(string? orderBy, string? sortDirection)
        {
            if (orderBy == null || orderBy == "")
            {
                Response.StatusCode = 400;
                var bytes = Encoding.UTF8.GetBytes("sortBy parameter is required");
                HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);            
                return null;
            }

            if (orderBy != "name" && orderBy != "id" && orderBy != "losses" && orderBy != "wins" && orderBy != "ties")
            {
                Response.StatusCode = 400;
                var bytes = Encoding.UTF8.GetBytes("sortBy parameter is invalid");
                HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);         
                return null;
            }

            if (sortDirection != "asc" && sortDirection != "desc")
            {
                Response.StatusCode = 400;
                var bytes = Encoding.UTF8.GetBytes("sortDirection parameter is invalid");
                HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);            
                return null;
            }
           
            List<int> availableIds = new List<int>();
            
            for (int i = 1; i <= 151; i++)
            {
                availableIds.Add(i);
            }

            List<Pokemon.Server.Models.Pokemon> pokemonList = new List<Models.Pokemon>();
            for (int i = 0; i < 8; i++)
            {
                // Get the id for this pokemon
                int idIndex = random.Next(0, availableIds.Count);

                // Remove from list to make unique
                int Id = availableIds[idIndex];
                availableIds.Remove(idIndex);

                Pokemon.Server.Models.Pokemon pk = GetPokemon(Id).Result;
                pokemonList.Add(pk);
            }

            PerformTournament(pokemonList);

            if (sortDirection == "asc")
            {
                switch (orderBy)
                {
                    case "wins":
                        pokemonList = pokemonList.OrderBy(obj => obj.wins).ToList();
                        break;
                    case "ties":
                        pokemonList = pokemonList.OrderBy(obj => obj.ties).ToList();
                        break;
                    case "losses":
                        pokemonList = pokemonList.OrderBy(obj => obj.losses).ToList();
                        break;
                    case "name":
                        pokemonList = pokemonList.OrderBy(obj => obj.name).ToList();
                        break;
                    case "id":
                        pokemonList = pokemonList.OrderBy(obj => obj.id).ToList();
                        break;
                }
            }
            else
            {
                switch (orderBy)
                {
                    case "wins":
                        pokemonList = pokemonList.OrderByDescending(obj => obj.wins).ToList();
                        break;
                    case "ties":
                        pokemonList = pokemonList.OrderByDescending(obj => obj.ties).ToList();
                        break;
                    case "losses":
                        pokemonList = pokemonList.OrderByDescending(obj => obj.losses).ToList();
                        break;
                    case "name":
                        pokemonList = pokemonList.OrderByDescending(obj => obj.name).ToList();
                        break;
                    case "id":
                        pokemonList = pokemonList.OrderByDescending(obj => obj.id).ToList();
                        break;
                }
            }

            return pokemonList.ToArray();
        }

        private async Task<Pokemon.Server.Models.Pokemon> GetPokemon(int id)
        {
            // Get pokemon from pokemon API

            HttpClient httpClient = new HttpClient();

            using HttpResponseMessage response = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon/" + id);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            Pokemon.Server.Models.Pokemon pkm = JsonSerializer.Deserialize<Pokemon.Server.Models.Pokemon>(jsonResponse);

            return pkm;
        }

        private void PerformTournament(List<Pokemon.Server.Models.Pokemon> pokemonList)
        {
            // Preparing all indices
            List<int> availableIndices = new List<int>();
            for (int i = 0; i < pokemonList.Count; i++)
            {
                availableIndices.Add(i);
            }

            // Implementing the circle method for round robin tournament
            // First index will stay still
            // Remaining pokemon are in a list that we march through using the round index
            List<int> topRow = new List<int>();
            List<int> bottomRow = new List<int>();

            for (int i = 0; i < pokemonList.Count / 2; i++)
            {
                int whichIndex = random.Next(availableIndices.Count);
                topRow.Add(availableIndices[whichIndex]);
                availableIndices.RemoveAt(whichIndex);                
            }

            for (int i = 0; i < pokemonList.Count / 2; i++)
            {
                int whichIndex = random.Next(availableIndices.Count);
                bottomRow.Add(availableIndices[whichIndex]);
                availableIndices.RemoveAt(whichIndex);
            }

            // One round per pokemon in the list
            for (int roundIndex = 0; roundIndex < (pokemonList.Count - 1); roundIndex++)
            {
                // Handle the queue battles
                for (int i = 0; i < topRow.Count; i++)
                {
                    PerformBattle(pokemonList[topRow[i]], pokemonList[bottomRow[i]]);
                }

                // Move the rows along
                topRow.Insert(1, bottomRow[0]);
                bottomRow.RemoveAt(0);

                bottomRow.Add(topRow[topRow.Count - 1]);
                topRow.RemoveAt(topRow.Count - 1);

            }
        }

        void PerformBattle(Pokemon.Server.Models.Pokemon primary, Pokemon.Server.Models.Pokemon challenger)
        {
            bool challengerWins = false;
            bool wasTie = false;

            if (DetermineIfWinThroughType(primary.types[0].type, challenger.types[0].type))
            {
                challengerWins = false;
            }
            else if (DetermineIfWinThroughType(challenger.types[0].type, primary.types[0].type))
            {
                challengerWins = true;
            }
            else
            {
                if (primary.base_experience > challenger.base_experience)
                {
                    challengerWins = false;
                }
                else if (challenger.base_experience > primary.base_experience)
                {
                    challengerWins = true;
                }
                else
                {
                    wasTie = true;
                }
            }

            if (wasTie)
            {
                primary.ties++;
                challenger.ties++;
            }
            else
            {
                if (challengerWins)
                {
                    challenger.wins++;
                    primary.losses++;
                }
                else
                {
                    primary.wins++;
                    challenger.losses++;
                }
            }
        }

        bool DetermineIfWinThroughType(Pokemon.Server.Models.PokemonType potentialWinnerType, Pokemon.Server.Models.PokemonType targetType)
        {
            if (potentialWinnerType.name == "water" && targetType.name == "fire")
            {
                return true;
            }
            else if (potentialWinnerType.name == "fire" && targetType.name == "grass")
            {
                return true;
            }
            else if (potentialWinnerType.name == "grass" && targetType.name == "electric")
            {
                return true;
            }
            else if (potentialWinnerType.name == "electric" && targetType.name == "water")
            {
                return true;
            }
            else if (potentialWinnerType.name == "ghost" && targetType.name == "psychic")
            {
                return true;
            }
            else if (potentialWinnerType.name == "psychic" && targetType.name == "fighting")
            {
                return true;
            }
            else if (potentialWinnerType.name == "fighting" && targetType.name == "dark")
            {
                return true;
            }
            else if (potentialWinnerType.name == "dark" && targetType.name == "ghost")
            {
                return true;
            }

            return false;
        }
    }
}
