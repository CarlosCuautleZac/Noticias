using NoticiasAPP.Models;
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
        public string url = "https://noticias.sistemas19.com/";
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

        public async Task<string> IniciarSesion(LoginDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                throw new ArgumentException("Escriba el nombre de usuario o contraseña");
            }

            var request = await client.PostAsJsonAsync("api/login", login);
            if (request.IsSuccessStatusCode)
            {

                //read token
                var token = await request.Content.ReadAsStringAsync();

                auth.WriteToken(token);

                await Shell.Current.GoToAsync("//Noticias",true);

                return "";
            }
            else
            {
               return await request.Content.ReadAsStringAsync();
                
            }
        }


        public async Task<string> Registar(RegisterDTO register)
        {

            var request = await client.PostAsJsonAsync("api/login/registrar", register);
            if (request.IsSuccessStatusCode)
            {
                return "Se registrado con exito al usuario";
            }
            else
            {
                return await request.Content.ReadAsStringAsync();

            }
        }

        public async void Logout()
        {
            auth.RemoveToken();
            App.loginViewModel.IsLoading = false;
            await Shell.Current.GoToAsync("//Login",true);
        }
    }

}
