using NoticiasAPP.ViewModels;

namespace NoticiasAPP.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		this.BindingContext = App.noticiasViewModel;
		InitializeComponent();
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
}