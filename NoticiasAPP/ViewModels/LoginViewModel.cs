using NoticiasAPP.Helpers;
using NoticiasAPP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly LoginService login;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public Command IniciarSesionCommand { get; set; }
        public LoginViewModel(LoginService login)
        {
            this.login = login;

            IniciarSesionCommand = new Command(IniciarSesion);
        }

        private async void IniciarSesion()
        {
            LoginDTO loginDTO = new() { Username = this.Username, Password = this.Password };

             await login.IniciarSesion(loginDTO);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
