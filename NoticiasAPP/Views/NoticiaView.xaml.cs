namespace NoticiasAPP.Views;

public partial class NoticiaView : ContentPage
{
	public NoticiaView()
	{
        this.BindingContext = App.noticiasViewModel;
        InitializeComponent();
		
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync(true);
    }
}