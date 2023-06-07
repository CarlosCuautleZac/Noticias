using Newtonsoft.Json;
using NoticiasAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.Helpers
{
    public class NoticiasService
    {
        public string url = "https://noticias.sistemas19.com/";
        private readonly AuthService auth;
        private readonly LoginService login;
        HttpClient client;

        public NoticiasService(AuthService auth, LoginService login)
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };
            this.auth = auth;
            this.login = login;
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.ReadToken().Result);
        }

        public async Task<IEnumerable<NoticiaDTO>> Get()
        {
            var request =  client.GetAsync("api/noticias");
            request.Wait();

            if(request.Result.IsSuccessStatusCode)
            {
               return await request.Result.Content.ReadFromJsonAsync<List<NoticiaDTO>>();
            }
            else
                return new List<NoticiaDTO>(); 
        }

        public async Task<string> Post(NoticiaDTO noticia)
        {
            var request = await client.PostAsJsonAsync("api/noticias", noticia);
            if (request.IsSuccessStatusCode)
            {
                return "Se ha enviado correctamente la noticia.";
            }
            else
                return await request.Content.ReadAsStringAsync();
        }

        public async Task<string> Put(NoticiaDTO noticia)
        {
            var request = await client.PutAsJsonAsync("api/noticias", noticia);
            if (request.IsSuccessStatusCode)
            {
                return "Se ha modificado correctamente la noticia.";
            }
            else
                return await request.Content.ReadAsStringAsync();
        }

        public async Task<string> Put(int idnoticia, int idautor)
        {
            var request = await client.DeleteAsync($"api/noticias/{idnoticia}/{idautor}");
            if (request.IsSuccessStatusCode)
            {
                return "Se ha eliminado la noticia.";
            }
            else
                return await request.Content.ReadAsStringAsync();
        }
    }
}
