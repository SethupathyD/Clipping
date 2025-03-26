
namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            scrollView.Scrolled += ScrollView_Scrolled;
        }

        private void ScrollView_Scrolled(object? sender, ScrolledEventArgs e)
        {
            container.ScrollX = e.ScrollX;
        }
    }
}
