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
        public string Mensaje { get; set; } = "";

        public Command IniciarSesionCommand { get; set; }
        public Command CerrarSesionCommand { get; set; }

        public LoginViewModel(LoginService login)
        {
            this.login = login;

            IniciarSesionCommand = new Command(IniciarSesion);
            //CerrarSesionCommand = new Command(CerrarSesion);
        }

       

        private async void IniciarSesion()
        {
            Mensaje = "";

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrEmpty(Username))
                {
                    Mensaje = "El nombre de usuario no debe ir vacío";
                }
                if (string.IsNullOrEmpty(Password))
                {
                    Mensaje = "La contraseña no debe ir vacía";
                }

                if (Mensaje == "")
                {
                    LoginDTO loginDTO = new() { Username = this.Username.Trim().ToUpper(), Password = this.Password };

                    await login.IniciarSesion(loginDTO);
                    Username = "";
                    Password = "";
                }           
            }
            else
            {
                Mensaje = "No hay conexion a internet";
            }

            OnPropertyChanged();
        }

        public void OnPropertyChanged(string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
