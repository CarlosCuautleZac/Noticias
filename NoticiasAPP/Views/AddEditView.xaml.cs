namespace NoticiasAPP.Views;

public partial class AddEditView : ContentPage
{
	public AddEditView()
	{
		this.BindingContext = App.noticiasViewModel;
		InitializeComponent();
	}
}