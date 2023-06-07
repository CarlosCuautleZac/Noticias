using NoticiasAPP.Helpers;
using NoticiasAPP.ViewModels;

namespace NoticiasAPP
{
    public partial class App : Application
    {
        public static LoginViewModel loginViewModel { get; set; }
        public static ShellViewModel shellViewModel { get; set; }
        public static NoticiasViewModel noticiasViewModel  { get; set; }

        public App(AuthService auth, LoginService login)
        {
            loginViewModel = new(login);
            noticiasViewModel = new(login);
            shellViewModel = new(auth, login);
            

            InitializeComponent();
            MainPage = new AppShell(auth, login);
        }
    }
}