using System;
using Windows.ApplicationModel.Activation;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading.Tasks;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.Storage;

namespace Twitter
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
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
            requestMsg.Headers.Add("User-Agent", "Mozilla / 5.0(Linux; U; Android 13.0; en - us; SCH - I535 Build / KOT49H) AppleWebKit / 534.30(KHTML, like Gecko) Version / 4.0 Mobile Safari/ 534.30");
            requestMsg.Headers.Add("Accept-Encoding", "gzip"); // Optimize HTTP requests
            wb.NavigateWithHttpRequestMessage(requestMsg);
        }

        private async void Wb_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.Scheme == "ms-appx-web")
            {
                args.Cancel = true;

                if (args.Uri.AbsolutePath == "/ErrorPage.html")
                {
                    // Handle error page navigation
                }
                else if (args.Uri.AbsolutePath == "/FilePicker.html")
                {
                    // Handle file picker navigation
                    var filePicker = new FileOpenPicker();
                    filePicker.ViewMode = PickerViewMode.Thumbnail;
                    filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    filePicker.FileTypeFilter.Add(".jpg");
                    filePicker.FileTypeFilter.Add(".jpeg");
                    filePicker.FileTypeFilter.Add(".png");
                    filePicker.FileTypeFilter.Add(".gif");
                    filePicker.FileTypeFilter.Add(".bmp");
                    var file = await filePicker.PickSingleFileAsync();
                    if (file != null)
                    {
                        // Process the picked file (e.g., upload to Twitter)
                        using (var stream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            var httpClient = new HttpClient();
                            var multipartContent = new HttpMultipartFormDataContent();
                            var streamContent = new HttpStreamContent(stream);
                            streamContent.Headers.Add("Content-Type", "image/jpeg"); // Adjust content type based on the file type
                            multipartContent.Add(streamContent, "media");

                            var uploadUri = new Uri("https://api.twitter.com/1.1/media/upload.json");
                            var response = await httpClient.PostAsync(uploadUri, multipartContent);

                            if (response.IsSuccessStatusCode)
                            {
                                // Image uploaded successfully, get the media ID
                                var mediaId = await response.Content.ReadAsStringAsync();

                                // Now you can include the media ID in your tweet/post
                                var tweetContent = new HttpStringContent($"status=Hello%20world&media_ids={mediaId}");
                                var tweetUri = new Uri("https://api.twitter.com/1.1/statuses/update.json");
                                var tweetResponse = await httpClient.PostAsync(tweetUri, tweetContent);

                                if (tweetResponse.IsSuccessStatusCode)
                                {
                                    // Tweet posted successfully
                                }
                                else
                                {
                                    // Failed to post tweet
                                }
                            }
                            else
                            {
                                // Failed to upload image
                            }
                        }
                    }
                    else
                    {
                        // No file picked
                    }
                }
                else
                {
                    // Handle other navigation
                }
            }
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
