using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;

namespace Twitter
{
    public sealed partial class MainPage : Page
    {
        private ToastNotifier _notifier;
        public MainPage()
        {
            SystemNavigationManager currentView =
            SystemNavigationManager.GetForCurrentView();
            this.InitializeComponent();
            CheckInternetConnection();
            currentView.BackRequested += CurrentView_BackRequested;
            wb.NavigationStarting += Wb_NavigationStarting;
            NavigateWithHeader(new Uri("https://mobile.twitter.com"));
            ElementCompositionPreview.SetIsTranslationEnabled(MyFrame, true);

        }

        private async void CheckInternetConnection()
        {
            bool isConnected = await CheckForInternetConnection();
            if (isConnected)
            {
                NavigateWithHeader(new Uri("https://mobile.twitter.com"));
            }
            else
            {
                NavigateWithHeader(new Uri("ms-appx-web:///ErrorPage.html"));
            }
        }

        private async Task<bool> CheckForInternetConnection()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = (connections != null) && (connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
            if (!internet)
            {
                await new MessageDialog("No Internet Connection Found").ShowAsync();
            }
            return internet;
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                bool canGoBack = MyFrame.CanGoBack;

            }
            else
            {
                e.Handled = true;
            }
        }

        private void NavigateWithHeader(Uri uri)
        {
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            var httpClient = new HttpClient(filter);
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, uri);
            requestMsg.Headers.Add("User-Agent", "Mozilla / 5.0(Linux; U; Android 4.4.2; en - us; SCH - I535 Build / KOT49H) AppleWebKit / 534.30(KHTML, like Gecko) Version / 4.0 Mobile Safari/ 534.30");
            requestMsg.Headers.Add("Accept-Encoding", "gzip"); // Optimize HTTP requests
            wb.NavigateWithHttpRequestMessage(requestMsg);
        }

        private void MyWebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            NavigateWithHeader(new Uri("ms-appx-web:///ErrorPage.html"));
        }

        private void Wb_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            wb.NavigationStarting -= Wb_NavigationStarting;
            args.Cancel = true;
            NavigateWithHeader(args.Uri);
        }

        private async void RenderWebViewToImageAsync()
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(MyFrame);
            var brush = new ImageBrush { ImageSource = bitmap };
            // Set the brush as the background of a UI element in your app
        }

    }

}