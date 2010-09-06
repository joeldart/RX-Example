using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace ReactiveExtensionsDemo
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            SubscribeToDrag();
            SubscribeToKonami();
            SubscribeToTranslation();
        }

        private void SubscribeToDrag()
        {
            var mouseDown = Observable.FromEvent<MouseButtonEventArgs>(RedRect, "MouseLeftButtonDown");
            var mouseMove = from m in Observable.FromEvent<MouseEventArgs>(LayoutRoot, "MouseMove")
                            select m.EventArgs.GetPosition(LayoutRoot);
            var mouseUp = Observable.FromEvent<MouseButtonEventArgs>(LayoutRoot, "MouseLeftButtonUp");


            var drag = from d in mouseDown.Do(_=>LayoutRoot.CaptureMouse())
                       from m in mouseMove.Skip(1).Zip(mouseMove, (l, r) =>
                       {
                           return new
                           {
                               dx = l.X - r.X,
                               dy = l.Y - r.Y
                           };
                       }).TakeUntil(mouseUp)
                       select m;
            drag.Subscribe(delta =>
            {
                double X = Canvas.GetLeft(RedRect);
                double Y = Canvas.GetTop(RedRect);
                Canvas.SetLeft(RedRect, X + delta.dx);
                Canvas.SetTop(RedRect, Y + delta.dy);
            });
        }

        private void SubscribeToKonami()
        {
            var keyUp = Observable.FromEvent<KeyEventArgs>(TheTextBox, "KeyUp");
            Func<Key, IObservable<IEvent<KeyEventArgs>>> KeyIs = (Key key) =>
            {
                return (from k in keyUp
                        where k.EventArgs.Key == key
                        select k);
            };
            Func<Key, IObservable<IEvent<KeyEventArgs>>> KeyIsNot = (Key key) =>
            {
                return (from k in keyUp
                        where k.EventArgs.Key != key
                        select k);
            };

            var konamiCode = from u1 in KeyIs(Key.Up)
                             from u2 in KeyIs(Key.Up).TakeUntil(KeyIsNot(Key.Up))
                             from d1 in KeyIs(Key.Down).TakeUntil(KeyIsNot(Key.Down))
                             from d2 in KeyIs(Key.Down).TakeUntil(KeyIsNot(Key.Down))
                             from l1 in KeyIs(Key.Left).TakeUntil(KeyIsNot(Key.Left))
                             from r1 in KeyIs(Key.Right).TakeUntil(KeyIsNot(Key.Right))
                             from l2 in KeyIs(Key.Left).TakeUntil(KeyIsNot(Key.Left))
                             from r2 in KeyIs(Key.Right).TakeUntil(KeyIsNot(Key.Right))
                             from b in KeyIs(Key.B).TakeUntil(KeyIsNot(Key.B))
                             from a in KeyIs(Key.A).TakeUntil(KeyIsNot(Key.A))
                             select new Unit();
            konamiCode.Subscribe(_ =>
            {
                MessageBox.Show("Konami Code entered");
            });
        }

        //I can't actually give out my appid with the project, but you 
        //can get one from bing in a matter of minutes at http://www.microsofttranslator.com/dev/
        private string appId = null;

        private void SubscribeToTranslation()
        {
            /* this example came from the rx for JavaScript world, but I thought it did a great job showing how to 
             * use Subjects to create your own IObservables and is obviously very cool.  To read his excellent
             * explanation as well as see how nice this looks in JavaScript, go to http://codebetter.com/blogs/matthew.podwysocki/archive/2010/03/11/introduction-to-the-reactive-extensions-for-javascript-composing-callbacks.aspx
             */
            if (appId == null)
            {
                MessageBox.Show("To view the full translation demo, you must go get your own appid from http://www.microsofttranslator.com/dev/ currently using dummy data (check out readme.txt)");
                UseLocal = true;
            }
            var textChanged = from e in Observable.FromEvent<TextChangedEventArgs>(TheTextBox, "TextChanged")
                                        .Throttle(TimeSpan.FromSeconds(.25))
                                        .ObserveOnDispatcher()
                              select TheTextBox.Text;
            var translator = from textToDetect in textChanged
                             where !string.IsNullOrEmpty(textToDetect)
                             from lang in detect(textToDetect)
                             from translation in Translate(textToDetect, lang, "es")
                             select translation;
            translator.Subscribe(spanishText =>
            {
                Translations.Items.Insert(0, spanishText);
            });
        }

        private bool UseLocal = false;
        private string GetDetectUri(string text)
        {
            string detectUri = "http://api.microsofttranslator.com/v2/Http.svc/Detect?appId={0}&text={1}";
            //Okay, bit of an explanation here: I didn't have internet connectivity in the room, so there's a wcf service
            //which gives mostly incorrect translations (once again no internet).  if you need to demo without internet,
            //feel free to use it as a substitute
            if(UseLocal)
                detectUri = "http://localhost/examplerestservice/service1/detect/{1}" + HttpUtility.HtmlEncode(text);
            return detectUri;
        }
        private string GetTranslateUri(string text, string from, string to)
        {
            string detectUri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId={0}&text={1}&from={2}&to={3}";

            //Okay, bit of an explanation here: I didn't have internet connectivity in the room, so there's a wcf service
            //which gives mostly incorrect translations (once again no internet).  if you need to demo without internet,
            //feel free to use it as a substitute
            if (UseLocal)
                detectUri = "http://localhost/examplerestservice/service1/translate/{2}/es/{1}";
            return detectUri;
        }

        private IObservable<string> detect(string text)
        {
            var subject = new AsyncSubject<string>();
            string detectUri = String.Format(GetDetectUri(text), appId, HttpUtility.HtmlEncode(text));
            
            var wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler((obj, args) =>
            {
                if (args.Error != null)
                {
                    subject.OnError(args.Error);
                }
                else
                {
                    if (!args.Cancelled)
                    {
                        var xdoc = XDocument.Load(args.Result);
                        subject.OnNext(xdoc.Root.Value);
                    }
                    subject.OnCompleted();
                }
            });
            wc.OpenReadAsync(new Uri(detectUri));
            return subject;
        }

        private IObservable<string> Translate(string text, string from, string to)
        {
            string detectUri = String.Format(GetTranslateUri(text, from, to), appId, text, from, to);
            
            var subject = new AsyncSubject<string>();
            var wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler((obj, args) =>
            {
                if (args.Error != null)
                {
                    subject.OnError(args.Error);
                }
                else
                {
                    if (!args.Cancelled)
                    {
                        var xdoc = XDocument.Load(args.Result);
                        subject.OnNext(xdoc.Root.Value);
                    }
                    subject.OnCompleted();
                }
            });
            wc.OpenReadAsync(new Uri(detectUri));
            return subject;
        }
    }
}
