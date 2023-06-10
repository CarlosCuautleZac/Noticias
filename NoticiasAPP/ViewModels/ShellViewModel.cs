using NoticiasAPP.Helpers;
using NoticiasAPP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        private readonly AuthService auth;
        private readonly LoginService login;
        MainPage loginview;
        NoticiasView noticiasView;
        public ContentPage Vista { get; set; }

        public ShellViewModel(AuthService auth, LoginService login)
        {
            this.auth = auth;
            this.login = login;


            if(auth.IsAuthenticated && auth.IsValid)
            {
                NavigateTo("Noticias");
            }
            else
            {
                NavigateTo("Login");
            }

        }

        private void NavigateTo(string page)
        {
            if (page == "Login")
            {
                loginview = new();
                Vista = loginview;
            }
            else if(page == "Noticias")
            {
                App.noticiasViewModel.GetNoticias();
                App.noticiasViewModel.GetCategorias();
                noticiasView = new();
                Vista = noticiasView;
            }
        }

        public void OnPropertyChanged(string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
