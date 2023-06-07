using NoticiasAPP.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.ViewModels
{
    public class NoticiasViewModel : INotifyPropertyChanged
    {
        private readonly LoginService login;

        public Command CerrarSesionCommand { get; set; }    

        public NoticiasViewModel(LoginService login)
        {
            this.login = login;
            CerrarSesionCommand = new Command(CerrarSesion);
        }

        private void CerrarSesion()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                login.Logout();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
