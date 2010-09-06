In my infinite laziness, I only did a single Silverlight project when 
these could definitely be ported over to WPF and WinForms easily.  
Also, I have two xaml pages from the xaml, so to see both demos, change
App.Xaml.cs to not set the LayoutRoot as MainPage but instead RequestWithRetry.

The other project is simply the MSTest project that simulates asynchronous behavior 
and tests our retry code.

Finally, to be sure, these were created using Silverlight, but these demos could also be done 
in WPF, winforms, and javascript (without the "from" syntax, but as I'd mentioned SelectMany
makes more sense to me anyway).

The Microsoft translator demo was ported from a RXJS (Reactive Extensions for Javascript) demo 
by Matthew Podwysocki http://codebetter.com/blogs/matthew.podwysocki/archive/2010/03/11/introduction-to-the-reactive-extensions-for-javascript-composing-callbacks.aspx.
I cannot distribute my bing appid, so you'll have to go through the super-easy 
process yourself on http://www.microsofttranslator.com/dev/.  

Otherwise I have built in a fake translation service to the WCF project and am hosting my SL 
app from the WCF page so you don't have to worry about cross domain request policies (this is
an RX demo not wcf or silverlight cross domain).  You should be able to select WCFRestService
as startup project and select ReactiveExtensionsDemoTestPage.html as startup page and run 
all the demos.  