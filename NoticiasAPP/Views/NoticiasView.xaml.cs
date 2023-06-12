using NoticiasAPP.Helpers;
using NoticiasAPP.Models;
using NoticiasAPP.ViewModels;
using System;
using System.Globalization;

namespace NoticiasAPP.Views;

public partial class NoticiasView : ContentPage
{
	public NoticiasView()
	{
        this.BindingContext = App.noticiasViewModel;
        
        InitializeComponent();
		
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var noticia = ((Grid)sender).BindingContext;

        viewmodel.VerNoticiaCommand.Execute(noticia);
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var noticia = ((Image)sender).BindingContext;

        viewmodel.VerNoticiaCommand.Execute(noticia);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var categoria = ((Button)sender).BindingContext;

        viewmodel.FiltrarCategoriaCommad.Execute(categoria);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var word = ((Entry)sender).Text;

        viewmodel.FiltrarMisNoticiasByWordCommand.Execute(word);
    }
}