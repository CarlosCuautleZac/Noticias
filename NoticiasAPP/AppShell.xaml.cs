using NoticiasAPP.Helpers;
using NoticiasAPP.ViewModels;

namespace NoticiasAPP
{
    public partial class AppShell : Shell
    {
        public AppShell(AuthService auth, LoginService login)
        {
            this.BindingContext = new ShellViewModel(auth, login);
            InitializeComponent();  
        }
    }
}