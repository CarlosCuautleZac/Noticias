using NoticiasAPP.Helpers;
using NoticiasAPP.ViewModels;

namespace NoticiasAPP
{
    public partial class App : Application
    {
        public App(AuthService auth, LoginService login)
        {
            
            InitializeComponent();
            MainPage = new AppShell(auth, login);
        }
    }
}