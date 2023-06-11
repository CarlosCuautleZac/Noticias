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
        public bool IsLoading { get; set; }

        public Command IniciarSesionCommand { get; set; }
        public Command CerrarSesionCommand { get; set; }


        Cifrado cifrado = new Cifrado();

        public LoginViewModel(LoginService login, AuthService auth)
        {
            this.login = login;

            IniciarSesionCommand = new Command(IniciarSesion);
            //CerrarSesionCommand = new Command(CerrarSesion);
        }

       

        private async void IniciarSesion()
        {
            try
            {
                Mensaje = "";

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    
                    OnPropertyChanged();

                    if (!IsLoading)
                    {
                        IsLoading = true;

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
                            var passcifrada = cifrado.CifradoTexto(Password.Trim());
                            LoginDTO loginDTO = new() { Username = this.Username.Trim().ToUpper(), Password = passcifrada };

                            var error = await login.IniciarSesion(loginDTO);
                            if (error == "")
                            {
                                App.noticiasViewModel.GetNoticias();
                                App.noticiasViewModel.GetCategorias();
                                App.noticiasViewModel.CargarDatosUsuario();
                                Username = "";
                                Password = "";
                            }
                            else
                            {
                                Mensaje = error;
                            }
                        }

                        IsLoading = false;
                    }
                    else
                    {
                        Mensaje = "Espere un momento se esta iniciando la sesión";
                    }

                    
                    OnPropertyChanged();
                }
                else
                {
                    Mensaje = "No hay conexion a internet";
                }

                OnPropertyChanged();
            }
            catch(Exception ex)
            {
                Mensaje = ex.Message;
                IsLoading = false;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged(string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
