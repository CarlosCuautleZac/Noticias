using Microsoft.Extensions.DependencyInjection;
using NoticiasAPP.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NoticiasAPP.ViewModels;

namespace NoticiasAPP.Helpers
{
    public class ColorToLabelConverter : IValueConverter
    {
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           CategoriaDTO categoria = (CategoriaDTO)value;
            NoticiasViewModel dataContext = App.noticiasViewModel;


            if (dataContext != null && categoria!=null)
            {
                if (categoria == dataContext.CategoriaActual)
                    return Color.FromHex("fc445e");

                else
                    return Color.FromHex("f6f5f8");
            }

            else
                return Color.FromHex("f6f5f8");



        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
