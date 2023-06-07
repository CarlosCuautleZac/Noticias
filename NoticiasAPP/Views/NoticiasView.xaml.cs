namespace NoticiasAPP.Views;

public partial class NoticiasView : ContentPage
{
	public NoticiasView()
	{
        this.BindingContext = App.noticiasViewModel;
        InitializeComponent();
		
	}
}