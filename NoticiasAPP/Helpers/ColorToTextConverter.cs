using NoticiasAPP.Models;
using NoticiasAPP.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoticiasAPP.Helpers
{
    public class ColorToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CategoriaDTO categoria = (CategoriaDTO)value;
            NoticiasViewModel dataContext = App.noticiasViewModel;


            if (dataContext != null && categoria != null)
            {
                if (categoria == dataContext.CategoriaActual)
                    return Color.FromHex("FFFFFF");

                else
                    return Color.FromHex("");
            }

            else
                return Color.FromHex("FFFFFF");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
