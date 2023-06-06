using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.Helpers
{
    public class LoginService
    {
        private readonly AuthService auth;
        public string url = "https://seguridaddocentes.itesrc.net/";
        HttpClient client;
        public LoginService(AuthService auth)
        {
            //Le paso la url al httpclient
            client = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };
            this.auth = auth;


        }

        public async Task<bool> IniciarSesion(LoginDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Contraseña))
            {
                throw new ArgumentException("Escriba el nombre de usuario o contraseña");
            }

            var request = await client.PostAsJsonAsync("api/login", login);
            if (request.IsSuccessStatusCode)
            {

                //read token
                var token = await request.Content.ReadAsStringAsync();

                auth.WriteToken(token);
                return true;
            }
            else
            {
                var message = request.Content.ReadAsStringAsync();
                return false;
            }
        }

        public async void Logout()
        {
            auth.RemoveToken();
            await Shell.Current.GoToAsync("login");
        }
    }

}
