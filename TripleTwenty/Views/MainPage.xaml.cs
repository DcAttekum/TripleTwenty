using TripleTwenty.ViewModels;

namespace TripleTwenty.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.Services.GetService<MainViewModel>();
        }
    }
}
