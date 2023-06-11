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
}