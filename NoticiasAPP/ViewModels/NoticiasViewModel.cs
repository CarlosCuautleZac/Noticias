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
        public Command FiltrarCategoriaCommad { get; set; }
        public Command FiltrarNoticiasByWordCommad { get; set; }

        //Propiedades
        public ObservableCollection<NoticiaDTO> Noticias { get; set; } = new();
        public ObservableCollection<NoticiaDTO> NoticiasFiltradas { get; set; } = new();

        public List<CategoriaDTO> Categorias { get; set; } = new();
        public string Mensaje { get; set; }
        public bool IsLoading { get; set; }
        public NoticiaDTO Noticia { get; set; }
        public DateTime Ahora { get; set; } = DateTime.Now;
        public CategoriaDTO CategoriaActual { get; set; }

        //Constructor
        public NoticiasViewModel(LoginService login, NoticiasService noticiasService, CategoriaService categoriaService)
        {
            this.login = login;
            this.noticiasService = noticiasService;
            this.categoriaService = categoriaService;
            CerrarSesionCommand = new Command(CerrarSesion);
            VerNoticiaCommand = new Command<NoticiaDTO>(VerNoticia);
            FiltrarCategoriaCommad = new Command<CategoriaDTO>(FiltrarCategoria);
            FiltrarNoticiasByWordCommad = new Command<string>(FiltrarNoticiasByWord);
            GetNoticias();
            GetCategorias();
        }

        private void FiltrarNoticiasByWord(string word)
        {
            if (Noticias.Count > 0)
            {
                NoticiasFiltradas.Clear();
                IEnumerable<NoticiaDTO> noticiasfiltradas;

                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (CategoriaActual.Id == 0)
                    {
                        noticiasfiltradas = Noticias.Where(x =>
                    (x.Titulo.ToUpper().Contains(word.ToUpper()) /*|| x.Descripcion.ToUpper().Contains(word.ToUpper())*/)).ToList();
                    }
                    else
                    {
                        noticiasfiltradas = Noticias.Where(x => x.IdCategoria == CategoriaActual.Id &&
                                           (x.Titulo.ToUpper().Contains(word.ToUpper()) /*|| x.Descripcion.ToUpper().Contains(word.ToUpper())*/)).ToList();
                    }

                    noticiasfiltradas.ToList().ForEach(x => NoticiasFiltradas.Add(x));
                }
                else
                {
                    FiltrarCategoria(CategoriaActual);
                }

                
            }
        }

        private void FiltrarCategoria(CategoriaDTO categoria)
        {
            CategoriaActual = categoria;
            NoticiasFiltradas.Clear();

            IEnumerable<NoticiaDTO> filtrados;

            if (CategoriaActual.Id != 0)
                filtrados = Noticias.Where(x => x.IdCategoria == CategoriaActual.Id);
            else
                filtrados = Noticias;

            filtrados.ToList().ForEach(x => NoticiasFiltradas.Add(x));

            OnPropertyChanged();
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
                    if (c.Count > 0)
                    {

                        Categorias.Add(new CategoriaDTO { Id = 0, Nombre = "Todo" });
                        c.ForEach(x => Categorias.Add(x));
                        Categorias = Categorias.OrderByDescending(x => x.Id).ToList();
                        CategoriaActual = Categorias[Categorias.Count - 1];
                        FiltrarCategoria(CategoriaActual);
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
