namespace NoticiasAPP
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            //ESTO ES LOGIN
            Init();
            this.BindingContext = App.loginViewModel;
           
        }

        private void Init()
        {
            border.TranslationX = 0;
            border.TranslationY = 750;


            borderregistrar.TranslationX = 0;
            borderregistrar.TranslationY = 750;
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            grid.FadeTo(0.6, 500);
            border.TranslateTo(0, 0, 500, Easing.SinInOut);
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            grid.FadeTo(0.6, 500);
            borderregistrar.TranslateTo(0, 0, 500, Easing.SinInOut);
        }

        private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
        {
            grid.FadeTo(0, 500);
            border.TranslationX = 0;
            border.TranslationY = 750;
        }

        private void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
        {
            grid.FadeTo(0, 500);

            borderregistrar.TranslationX = 0;
            borderregistrar.TranslationY = 750;
        }
    }
}