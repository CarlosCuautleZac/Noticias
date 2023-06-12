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
        public event Action CerrarBorder;

        //Para el usuario
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Foto { get; set; } = "";

        //para el programa
        public string Mensaje { get; set; } = "";
        public bool IsLoading { get; set; }


        private ImageSource _imagen;

        public ImageSource Imagen
        {
            get { return _imagen; }
            set
            {
                if (_imagen != value)
                {
                    _imagen = value;
                    OnPropertyChanged(nameof(Imagen));
                }
            }
        }

        public Command CerrarSesionCommand { get; set; }
        public Command RegistrarUsuarioCommand { get; set; }
        public Command CargarImagenCommand { get; set; }
        public Command CancelarCommand { get; set; }
        public Command IniciarSesionCommand { get; set; }


        Cifrado cifrado = new Cifrado();

        public LoginViewModel(LoginService login, AuthService auth)
        {
            this.login = login;

            IniciarSesionCommand = new Command(IniciarSesion);
            CancelarCommand = new Command(Cancelar);
            RegistrarUsuarioCommand = new Command(RegistrarUsuario);
            CargarImagenCommand = new Command(CargarImagen);
            //CerrarSesionCommand = new Command(CerrarSesion);
        }

        private async void CargarImagen(object obj)
        {
            var imagen = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Escoga una imagen"
            });

            if (imagen != null)
            {
                Stream stream = await imagen.OpenReadAsync();
                Imagen = ImageSource.FromStream(() => stream);
                Foto = GetBase64Image(imagen.FullPath);

                OnPropertyChanged();

            }
        }

        private string GetBase64Image(string ruta)
        {
            Byte[] bytes = File.ReadAllBytes(ruta);
            String file = Convert.ToBase64String(bytes);
            return file;
        }


        private async void RegistrarUsuario()
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
                            Mensaje += "El nombre de usuario no debe ir vacío" + Environment.NewLine;
                        }
                        if(Username.Contains(" "))
                        {
                            Mensaje += "El nombre de usuario no debe contener espacios" + Environment.NewLine;
                        }

                        if (string.IsNullOrEmpty(Password))
                        {
                            Mensaje += "La contraseña no debe ir vacía" + Environment.NewLine;
                        }
                        if (string.IsNullOrEmpty(Nombre))
                        {
                            Mensaje += "El nombre no debe ir vacío" + Environment.NewLine;
                        }
                        if (string.IsNullOrEmpty(Email))
                        {
                            Mensaje += "El correo no debe ir vacio" + Environment.NewLine;
                        }


                        if (Mensaje == "")
                        {
                            var passcifrada = cifrado.CifradoTexto(Password.Trim());
                            RegisterDTO registerDTO = new()
                            {
                                NombreUsuario = this.Username.Trim().ToUpper(),
                                Contraseña = passcifrada,
                                Email = Email.Trim(),
                                Nombre = this.Nombre,
                                Foto = this.Foto
                            };

                            var error = await login.Registar(registerDTO);

                            if (error == "Se registrado con exito al usuario")
                            {
                                Mensaje = error;
                                Username = "";
                                Password = "";
                                this.Nombre = "";
                                this.Foto = "";
                                Email = "";
                                CerrarBorder.Invoke();
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
                        Mensaje = "Espere un momento se esta registrando al usuario";
                    }


                    OnPropertyChanged();
                }
                else
                {
                    Mensaje = "No hay conexion a internet";
                }

                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                IsLoading = false;
                OnPropertyChanged();
            }

        }

        private void Cancelar()
        {
            Username = "";
            Password = "";
            Nombre = "";
            Foto = "";
            Imagen = null;
            OnPropertyChanged(nameof(Imagen));
            OnPropertyChanged();

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
            catch (Exception ex)
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