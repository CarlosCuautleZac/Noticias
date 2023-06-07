namespace NoticiasAPP.Views;

public partial class NoticiaView : ContentPage
{
	public NoticiaView()
	{
		InitializeComponent();
		this.BindingContext = App.noticiasViewModel;
	}
}