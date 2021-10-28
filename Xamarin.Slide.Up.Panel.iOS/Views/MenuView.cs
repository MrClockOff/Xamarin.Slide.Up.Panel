using System;
using Foundation;
using UIKit;
using Xamarin.Slide.Up.Panel.iOS.Utilities;

namespace Xamarin.Slide.Up.Panel.iOS.Views
{
    [Register(nameof(MenuView))]
    public class MenuView : UIView
    {
        public static MenuView LoadView()
        {
            return ViewLoaderUtility.LoadFromNib<MenuView>(nameof(MenuView));
        }

        protected MenuView()
        {
        }

        protected MenuView(IntPtr handle)
            : base (handle)
        {
        }
    }
}
