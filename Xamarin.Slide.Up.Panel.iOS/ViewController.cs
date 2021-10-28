using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Slide.Up.Panel.iOS.Controls;
using Xamarin.Slide.Up.Panel.iOS.Utilities;
using Xamarin.Slide.Up.Panel.iOS.Views;

namespace Xamarin.Slide.Up.Panel.iOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            //View.BackgroundColor = UIColor.Green;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void ShowPanelButton_TouchUpInside(Foundation.NSObject sender)
        {
            var slideUpPanelViewController = new SlideUpPanelViewController
            {
                CanCollapseToInputAccessoryView = true
            };
            var toolbar = new UIToolbar
            {
                BackgroundColor = UIColor.White,
                Translucent = false,
                Items = new[]
                {
                    new UIBarButtonItem("Hello", UIBarButtonItemStyle.Plain, (s, e) => slideUpPanelViewController.PresentPannel(slideUpPanelViewController.ParentViewController))
                }
            };
            var panel = MenuView.LoadView();


            toolbar.SizeToFit();

            slideUpPanelViewController.SetPanelView(panel);
            slideUpPanelViewController.SetPanelInputAccessoryView(toolbar);
            slideUpPanelViewController.PresentPannel(this);
        }
    }
}
