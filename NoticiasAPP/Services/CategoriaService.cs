using NoticiasAPP.Helpers;
using NoticiasAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.Services
{
    public class CategoriaService
    {
        public string url = "https://noticias.sistemas19.com/";
        private readonly AuthService auth;
        private readonly LoginService login;
        HttpClient client;

        public CategoriaService(AuthService auth, LoginService login)
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };
            this.auth = auth;
            this.login = login;


        }

        public async Task<IEnumerable<CategoriaDTO>> Get()
        {
            Verificar();

            var request = client.GetAsync("api/categoria");
            request.Wait();

            if (request.Result.IsSuccessStatusCode)
            {
                return await request.Result.Content.ReadFromJsonAsync<List<CategoriaDTO>>();
            }
            else
                return new List<CategoriaDTO>();
        }

        private void Verificar()
        {
            if (auth.IsAuthenticated && auth.IsValid && client.DefaultRequestHeaders.Authorization == null)
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.ReadToken().Result);
        }
    }
}
