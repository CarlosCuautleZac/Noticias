namespace NoticiasAPP.Views;

public partial class NoticiaView : ContentPage
{
	public NoticiaView()
	{
		InitializeComponent();
		this.BindingContext = App.noticiasViewModel;
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.Navigation.PopAsync();
    }
}