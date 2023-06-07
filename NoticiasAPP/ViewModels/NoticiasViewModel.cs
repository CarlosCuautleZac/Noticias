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
        //Campos de services
        private readonly LoginService login;

        //Comandos
        public Command CerrarSesionCommand { get; set; }    


        //Constructor
        public NoticiasViewModel(LoginService login)
        {
            this.login = login;
            CerrarSesionCommand = new Command(CerrarSesion);
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
