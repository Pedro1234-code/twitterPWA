# Twitter PWA app for Windows 10 Mobile

Twitter app revival for Windows 10 Mobile - based on WebView XAML control with custom user agent to make again it run on Windows 10 Mobile.

Currently not working features and known issues:

- Share button on apps (photos, etc)

- App sometimes gives "browser not supported" error, but restarting it fixes the problem.

- If there is a post that included a YouTube video, the frame inside the app where it should be playing the video will show "browser not supported" error. I am investigating the issue.

Things I am currently working:


- Make the app consume less battery, but that's really a problem as Twitter website consumes a lot.

If you have any tips I would be so grateful to have it shared!

Note: after building, you will have to unpack the package and add Twitter_Logo_Blue.png into the root of the package, and then repack again. not doing this will result on a missing image on network error page.
