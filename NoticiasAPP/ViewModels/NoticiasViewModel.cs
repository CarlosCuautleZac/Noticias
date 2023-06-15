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
        public Command VerNuevaNoticaCommand { get; set; }
        public Command FiltrarMisNoticiasPorCategoriaCommand { get; set; }
        public Command FiltrarMisNoticiasByWordCommand { get; set; }
        public Command EliminarCommand { get; set; }
        public Command VerEditarNoticiaCommand { get; set; }
        public Command RefreshCommand { get; set; }

        //Propiedades
        public ObservableCollection<NoticiaDTO> Noticias { get; set; } = new();
        public ObservableCollection<NoticiaDTO> NoticiasFiltradas { get; set; } = new();
        public ObservableCollection<CategoriaDTO> Categorias { get; set; } = new();
        public List<CategoriaDTO> CategoriasPost { get; set; } = new();//Es para el post, puesto a que la otra lista contiene una propiedad que solo
        //Jala en el cliente
        public ObservableCollection<NoticiaDTO> MisNoticiasFiltradas { get; set; } = new();

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
        public bool IsRefreshing { get; set; } = false;



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
            VerNuevaNoticaCommand = new Command(VerNuevaNoticia);
            FiltrarMisNoticiasPorCategoriaCommand = new Command<CategoriaDTO>(FiltrarMisNoticiasPorCategoria);
            FiltrarMisNoticiasByWordCommand = new Command<string>(FiltrarMisNoticiasByWord);
            EliminarCommand = new Command<NoticiaDTO>(Eliminar);
            VerEditarNoticiaCommand = new Command<NoticiaDTO>(VerEditar);
            RefreshCommand = new Command(Recargar);
        }

        private async void VerEditar(NoticiaDTO noticia)
        {
            Noticia = noticia;
            Modo = "EDITAR";
            Imagen = noticia.Imagen;
            OnPropertyChanged();
            await Shell.Current.Navigation.PushAsync(new AddEditView(), true);
        }

        private async void Eliminar(NoticiaDTO noticia)
        {

            bool answer = await App.Current.MainPage.DisplayAlert("Advertencia", "¿Estas seguro de eliminar esta noticia?", "Sí", "No");
            if (answer)
            {



                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var mensaje = await noticiasService.Delete(noticia.Id, Usuario.Id);
                    if (mensaje == "Se ha eliminado la noticia.")
                    {
                        await GetNoticias();
                        FiltrarCategoria(CategoriaActual);
                        FiltrarMisNoticiasPorCategoria(CategoriaActual);
                        //await Shell.Current.Navigation.PopAsync(true);
                    }
                }

                else
                {
                    Mensaje += "No hay conexion a internet";
                }
            }
        }

        private void VerEliminar(NoticiaDTO noticia)
        {
            Mensaje = "";

            if (noticia == null)
                Mensaje += "Eliga una noticia para eliminar";

            if (noticia.Id <= 0)
                Mensaje += "Eliga una noticia para eliminar";

            if (Usuario.Id <= 0)
                Mensaje += "Ha ocurrido un error. Vuelva a iniciar sesion para corregirlo.";

            if (Mensaje == "")
            {
                Modo = "ELIMINAR";

            }

            OnPropertyChanged();

        }

        private void FiltrarMisNoticiasByWord(string word)
        {
            if (Noticias.Count > 0)
            {
                MisNoticiasFiltradas.Clear();
                IEnumerable<NoticiaDTO> noticiasfiltradas;

                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (CategoriaActual.Id == 0)
                    {
                        noticiasfiltradas = Noticias.Where(x =>
                    (x.Titulo.ToUpper().Contains(word.ToUpper())) && x.IdAutor == Usuario.Id).ToList();
                    }
                    else
                    {
                        noticiasfiltradas = Noticias.Where(x => x.IdCategoria == CategoriaActual.Id &&
                                           (x.Titulo.ToUpper().Contains(word.ToUpper())) && x.IdAutor == Usuario.Id).ToList();
                    }

                    noticiasfiltradas.ToList().ForEach(x => MisNoticiasFiltradas.Add(x));
                }
                else
                {
                    FiltrarMisNoticiasPorCategoria(CategoriaActual);
                }

                OnPropertyChanged();
            }
        }

        private void FiltrarMisNoticiasPorCategoria(CategoriaDTO categoria)
        {
            var anterior = Categorias.FirstOrDefault(x => x.Id == CategoriaActual.Id);
            var idanterior = Categorias.IndexOf(anterior);

            CategoriaActual = categoria;
            MisNoticiasFiltradas.Clear();

            IEnumerable<NoticiaDTO> filtrados;

            if (CategoriaActual.Id != 0)
                filtrados = Noticias.Where(x => x.IdCategoria == CategoriaActual.Id && x.IdAutor == Usuario.Id);
            else
                filtrados = Noticias.Where(x => x.IdAutor == Usuario.Id); ;

            filtrados.ToList().ForEach(x => MisNoticiasFiltradas.Add(x));

            var encontrado = Categorias.FirstOrDefault(x => x.Id == CategoriaActual.Id);
            var idencontradp = Categorias.IndexOf(encontrado);

            Categorias[idanterior] = encontrado;
            Categorias[idencontradp] = anterior;

            OnPropertyChanged();
        }

        private async void VerNuevaNoticia()
        {
            try
            {
                Modo = "AGREGAR";
                Noticia = new NoticiaDTO();
                Imagen = null;
                OnPropertyChanged();
                await Shell.Current.Navigation.PushAsync(new AddEditView(), true);
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                OnPropertyChanged();
            }
        }

        private async void EnviarNoticia()
        {
            try
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

                        if (Noticia.Id == 0)
                            Mensaje = await noticiasService.Post(Noticia);
                        else
                            Mensaje = await noticiasService.Put(Noticia);

                        if (Mensaje == "Se ha enviado correctamente la noticia." || Mensaje == "Se ha modificado correctamente la noticia.")
                        {
                            await GetNoticias();


                            FiltrarCategoria(CategoriaActual);
                            FiltrarMisNoticiasPorCategoria(CategoriaActual);


                            await Shell.Current.Navigation.PopAsync(true);
                        }
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Advertencia", Mensaje, "OK");

                }
                else
                {
                    Mensaje = "No hay conexion a internet";
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", ex.Message, "OK");

            }
        }

        private async void CargarImagen()
        {
            Imagen = null;

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

                OnPropertyChanged();

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
            Modo = "PERFIL";
            FiltrarMisNoticiasPorCategoria(CategoriaActual);
            await Shell.Current.Navigation.PushAsync(new PerfilView(), true);
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

        [Obsolete]
        public  void Recargar()
        {
            Thread hiloRecarga = new(new ThreadStart( async () =>
            {
              
                    IsRefreshing = true;
                    OnPropertyChanged(nameof(IsRefreshing));
                    await GetNoticias();
                    IsRefreshing = false;
                    OnPropertyChanged();
               
                
            }));

            hiloRecarga.IsBackground = true;
            hiloRecarga.Start();
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
                    CategoriaActual = Categorias[Categorias.Count-1];
                    FiltrarCategoria(CategoriaActual);

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
