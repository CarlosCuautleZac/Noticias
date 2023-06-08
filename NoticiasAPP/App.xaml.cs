using NoticiasAPP.Helpers;
using NoticiasAPP.ViewModels;
using NoticiasAPP.Views;

namespace NoticiasAPP
{
    public partial class App : Application
    {
        public static LoginViewModel loginViewModel { get; set; }
        public static ShellViewModel shellViewModel { get; set; }
        public static NoticiasViewModel noticiasViewModel  { get; set; }

        public App(AuthService auth, LoginService login, NoticiasService noticiasService)
        {
            loginViewModel = new(login, auth);
            noticiasViewModel = new(login, noticiasService);
            shellViewModel = new(auth, login);
            

            InitializeComponent();
            MainPage = new AppShell(auth, login);

            //Registro la ruta nwn
            Routing.RegisterRoute("detallesnoticia", typeof(NoticiaView));
        }
    }
}