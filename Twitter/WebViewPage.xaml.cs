using Windows.UI.Xaml.Controls;

namespace Twitter
{
    public sealed partial class WebViewPage : Page
    {
        public WebViewPage()
        {
            this.InitializeComponent();
        }

        private void MyWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            Frame.Navigate(typeof(ErrorPage));
        }
    }
}
