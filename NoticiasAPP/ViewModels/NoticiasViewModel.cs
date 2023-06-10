using NoticiasAPP.Helpers;
using NoticiasAPP.Models;
using NoticiasAPP.Services;
using NoticiasAPP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.ViewModels
{
    public class NoticiasViewModel : INotifyPropertyChanged
    {
        //Campos de services
        private readonly LoginService login;
        private readonly NoticiasService noticiasService;
        private readonly CategoriaService categoriaService;

        //Comandos
        public Command CerrarSesionCommand { get; set; }
        public Command VerNoticiaCommand { get; set; }
        public Command VerPefilCommand { get; set; }

        //Propiedades
        public ObservableCollection<NoticiaDTO> Noticias { get; set; } = new();
        public List<CategoriaDTO> Categorias { get; set; } = new();
        public string Mensaje { get; set; }
        public bool IsLoading { get; set; }
        public NoticiaDTO Noticia { get; set; }
        public DateTime Ahora { get; set; } = DateTime.Now;

        //Constructor
        public NoticiasViewModel(LoginService login, NoticiasService noticiasService, CategoriaService categoriaService)
        {
            this.login = login;
            this.noticiasService = noticiasService;
            this.categoriaService = categoriaService;
            CerrarSesionCommand = new Command(CerrarSesion);
            VerNoticiaCommand = new Command<NoticiaDTO>(VerNoticia);
            VerPefilCommand = new Command(VerPerfil);

            GetNoticias();
            GetCategorias();
        }

        private async void VerPerfil()
        {
          await Shell.Current.Navigation.PushAsync(new PerfilView());
        }

        public void GetCategorias()
        {
            Mensaje = "";

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {

                OnPropertyChanged();

                if (!IsLoading)
                {
                    IsLoading = true;
                    OnPropertyChanged();

                    Categorias.Clear();
                    var c = categoriaService.Get().Result.ToList();
                    c.ForEach(x => Categorias.Add(x));


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

        private async void VerNoticia(NoticiaDTO noticia)
        {
            if (noticia != null)
            {
                Noticia = noticia;
                OnPropertyChanged();
                await Shell.Current.Navigation.PushAsync(new NoticiaView());
            }
            else
            {
                Mensaje = "Seleccione una noticia para continuar";
            }
        }

        public void GetNoticias()
        {
            Mensaje = "";

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {

                OnPropertyChanged();

                if (!IsLoading)
                {
                    IsLoading = true;
                    OnPropertyChanged();

                    Noticias.Clear();
                    var noticias = noticiasService.Get().Result.ToList();
                    noticias.ForEach(x => Noticias.Add(x));


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

        //Metodos
        private void CerrarSesion()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                login.Logout();
            }
        }

        public void OnPropertyChanged(string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
