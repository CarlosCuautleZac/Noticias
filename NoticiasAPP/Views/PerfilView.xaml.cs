namespace NoticiasAPP.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		this.BindingContext = App.shellViewModel;
		InitializeComponent();
	}
}