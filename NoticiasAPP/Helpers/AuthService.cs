using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.Helpers
{
    public class AuthService
    {
        //Normalmente tiene 3 metodos clasicos
        string token;

        public bool IsAuthenticated => ReadToken().Result != null;

        public IEnumerable<Claim> Cliams
        {
            get
            {
                JwtSecurityTokenHandler handler = new();
                var tk = handler.ReadJwtToken(token);
                return tk.Claims;
            }
        }

        public bool IsValid
        {
            get
            {
                JwtSecurityTokenHandler handler = new();
                var tk = handler.ReadJwtToken(token);
                return DateTime.UtcNow <= tk.ValidTo;
            }
        }

        //Write
        public async void WriteToken(string token)
        {
            this.token = token;
            await SecureStorage.SetAsync("JwtToken", token);
        }

        //Read

        public async Task<string> ReadToken()
        {
            token = await SecureStorage.GetAsync("JwtToken");
            return token;
        }

        //Remove
        public void RemoveToken()
        {
            SecureStorage.Remove("JwtToken");
            token = null;
        }
    }
}
