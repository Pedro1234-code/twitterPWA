using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using Windows.Storage;



namespace Twitter
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested += CurrentView_BackRequested;
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            CheckInternetConnection();

        }


        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppLoaderWB.CanGoBack) 
            {
                AppLoaderWB.GoBack();
                e.Handled = true; 
            }
        }

        private async void CheckInternetConnection()
        {
            bool isConnected = await CheckForInternetConnectionAsync();
            if (isConnected)
            {
                NavigateWithHeader(new Uri("https://nitter.net"));
            }
            else
            {
                Frame.Navigate(typeof(ErrorPage));
            }
        }

        private async Task<bool> CheckForInternetConnectionAsync()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = (connections != null) && (connections.GetNetworkConnectivityLevel()
                == NetworkConnectivityLevel.InternetAccess);
            if (!internet)
            {
                await new MessageDialog("No Internet Connection Found").ShowAsync();
            }
            return internet;
        }

        private void NavigateWithHeader(Uri uri)
        {
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            var httpClient = new HttpClient(filter);
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, uri);
            requestMsg.Headers.Add("User-Agent", "Mozilla / 5.0(Linux; U; Android 13.0; en - us; SCH - I535 Build / KOT49H) AppleWebKit / 534.30(KHTML, like Gecko) Version / 4.0 Mobile Safari/ 534.30");
            requestMsg.Headers.Add("Accept-Encoding", "gzip"); 
            AppLoaderWB.NavigateWithHttpRequestMessage(requestMsg);

        }

        }
    }
