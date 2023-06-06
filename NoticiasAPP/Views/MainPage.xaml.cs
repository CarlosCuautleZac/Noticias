namespace NoticiasAPP
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            border.TranslateTo(0, 750, 400, Easing.Linear);
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            grid.FadeTo(0.6, 500);
            border.TranslateTo(0, 0, 800, Easing.SinInOut);
        }
    }
}