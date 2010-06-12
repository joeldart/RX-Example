In my infinite laziness, I only did a single Silverlight project when 
these could definitely be ported over to WPF and WinForms easily.  
Also, I have two xaml pages from the xaml, so to see both demos, change
App.Xaml.cs to not set the LayoutRoot as MainPage but instead RequestWithRetry.

The other project is simply the MSTest project that simulates asynchronous behavior 
and tests our retry code.
