using Microsoft.Maui.Controls;
using NoticiasAPP.ViewModels;

namespace NoticiasAPP.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		this.BindingContext = App.noticiasViewModel;
		InitializeComponent();
        //bordereliminar.TranslationX = 0;
        //bordereliminar.TranslationY = 750;

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync(true);
    }

    private void buttonn_Clicked(object sender, EventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var categoria = ((Button)sender).BindingContext;

        viewmodel.FiltrarMisNoticiasPorCategoriaCommand.Execute(categoria);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var word = ((Entry)sender).Text;

        viewmodel.FiltrarMisNoticiasByWordCommand.Execute(word);
    }

    private void SwipeItem_Clicked(object sender, EventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var noticia = ((SwipeItem)sender).BindingContext;

        viewmodel.EliminarCommand.Execute(noticia);



        //bordereliminar.TranslateTo(0, -100, 500, Easing.SinInOut);
        //bordereliminar.TranslationX = 0;
        //bordereliminar.TranslationY = 750;

        //if (viewmodel.Modo == "ELIMINAR")
        //{
        //    bordereliminar.IsVisible = true;
        //}
    }

    private void SwipeItem_Clicked_1(object sender, EventArgs e)
    {
        var viewmodel = (NoticiasViewModel)this.BindingContext;

        var noticia = ((SwipeItem)sender).BindingContext;

        viewmodel.VerEditarNoticiaCommand.Execute(noticia);
    }
}