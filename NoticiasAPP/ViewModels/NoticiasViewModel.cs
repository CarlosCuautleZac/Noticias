using NoticiasAPP.Helpers;
using NoticiasAPP.Models;
using NoticiasAPP.Services;
using NoticiasAPP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
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
        private readonly AuthService auth;

        //Comandos
        public Command CerrarSesionCommand { get; set; }
        public Command VerNoticiaCommand { get; set; }
        public Command VerPefilCommand { get; set; }
        public Command FiltrarCategoriaCommad { get; set; }
        public Command FiltrarNoticiasByWordCommad { get; set; }
        public Command VerPerfilCommand { get; set; }
        public Command CargarImagenCommand { get; set; }
        public Command EnviarNoticiaCommand { get; set; }

        //Propiedades
        public ObservableCollection<NoticiaDTO> Noticias { get; set; } = new();
        public ObservableCollection<NoticiaDTO> NoticiasFiltradas { get; set; } = new();
        public ObservableCollection<CategoriaDTO> Categorias { get; set; } = new();
        public List<CategoriaDTO> CategoriasPost { get; set; } = new();//Es para el post, puesto a que la otra lista contiene una propiedad que solo
        //Jala en el cliente

        public string Mensaje { get; set; }
        public bool IsLoading { get; set; }
        public NoticiaDTO Noticia { get; set; }
        public DateTime Ahora { get; set; } = DateTime.Now;
        public CategoriaDTO CategoriaActual { get; set; }//Esta es para navegar entre noticias
        public CategoriaDTO Categoria { get; set; }//Esta es para el post
        public ObservableCollection<string> Evidencias;
        public string Modo { get; set; } = "";
        public ImageSource Imagen { get; set; }
        public Usuario Usuario { get; set; } = new();



        //Constructor
        public NoticiasViewModel(LoginService login, NoticiasService noticiasService, CategoriaService categoriaService, AuthService auth)
        {
            //Inyecciones
            this.login = login;
            this.noticiasService = noticiasService;
            this.categoriaService = categoriaService;
            this.auth = auth;

            //Comandos
            CerrarSesionCommand = new Command(CerrarSesion);
            VerNoticiaCommand = new Command<NoticiaDTO>(VerNoticia);
            FiltrarCategoriaCommad = new Command<CategoriaDTO>(FiltrarCategoria);
            FiltrarNoticiasByWordCommad = new Command<string>(FiltrarNoticiasByWord);
            VerPefilCommand = new Command(VerPerfil);
            CargarImagenCommand = new Command(CargarImagen);
            EnviarNoticiaCommand = new Command(EnviarNoticia);


        }

        private async void EnviarNoticia()
        {
            Mensaje = "";

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrWhiteSpace(Noticia.Titulo))
                    Mensaje = "El titulo de la noticia no debe ir vacio." + Environment.NewLine;

                if (string.IsNullOrWhiteSpace(Noticia.Descripcion))
                    Mensaje = "El cuerpo de la noticia no debe ir vacio." + Environment.NewLine;

                if (Categoria == null)
                    Mensaje = "Seleccione una categoria." + Environment.NewLine;

                if (string.IsNullOrWhiteSpace(Noticia.Imagen))
                {
                    Mensaje = "La noticia debe contener una imagen." + Environment.NewLine;
                }

                if (Mensaje == "")
                {
                    Noticia.IdCategoria = Categoria.Id;
                    Noticia.IdAutor = Usuario.Id;

                    Mensaje = await noticiasService.Post(Noticia);
                    if (Mensaje == "Se ha enviado correctamente la noticia.")
                    {
                        await GetNoticias();
                        FiltrarCategoria(CategoriaActual);
                        await Shell.Current.Navigation.PopAsync();
                    }
                }

            }
            else
            {
                Mensaje = "No hay conexion a internet";
            }
        }

        private async void CargarImagen()
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
                Noticia.Imagen = GetBase64Image(imagen.FullPath);

                OnPropertyChanged();
            }
        }

        private string GetBase64Image(string ruta)
        {
            Byte[] bytes = File.ReadAllBytes(ruta);
            String file = Convert.ToBase64String(bytes);
            return file;
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
            var anterior = Categorias.FirstOrDefault(x => x.Id == CategoriaActual.Id);
            var idanterior = Categorias.IndexOf(anterior);

            CategoriaActual = categoria;
            NoticiasFiltradas.Clear();

            IEnumerable<NoticiaDTO> filtrados;

            if (CategoriaActual.Id != 0)
                filtrados = Noticias.Where(x => x.IdCategoria == CategoriaActual.Id);
            else
                filtrados = Noticias;

            filtrados.ToList().ForEach(x => NoticiasFiltradas.Add(x));

            var encontrado = Categorias.FirstOrDefault(x => x.Id == CategoriaActual.Id);
            var idencontradp = Categorias.IndexOf(encontrado);

            Categorias[idanterior] = encontrado;
            Categorias[idencontradp] = anterior;

            OnPropertyChanged();
        }

        private async void VerPerfil()
        {
            Modo = "AGREGAR";
            Noticia = new NoticiaDTO();
            OnPropertyChanged();
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
                    CategoriasPost.Clear();
                    var c = categoriaService.Get().Result.ToList();
                    if (c.Count > 0)
                    {

                        Categorias.Add(new CategoriaDTO { Id = 0, Nombre = "Todo" });
                        c.ForEach(x => Categorias.Add(x));
                        c.ForEach(x => CategoriasPost.Add(x));
                        Categorias = new(Categorias.OrderByDescending(x => x.Id));
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

        public async Task GetNoticias()
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
                    var noticias = await noticiasService.Get();
                    noticias.ToList().ForEach(x => Noticias.Add(x));


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

        public void CargarDatosUsuario()
        {
            var claims = auth.Cliams;

            Usuario.Id = int.Parse(claims.FirstOrDefault(x => x.Type == "Id").Value);
            Usuario.NombreUsuario = claims.FirstOrDefault(x => x.Type == "Usuario").Value;
            Usuario.Nombre = claims.FirstOrDefault(x => x.Type == "unique_name").Value;
            Usuario.Email = claims.FirstOrDefault(x => x.Type == "email").Value;
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
