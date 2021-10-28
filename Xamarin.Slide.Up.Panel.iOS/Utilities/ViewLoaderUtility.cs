using Foundation;
using ObjCRuntime;
using UIKit;

namespace Xamarin.Slide.Up.Panel.iOS.Utilities
{
    public static class ViewLoaderUtility
    {
        public static T LoadFromNib<T>(string nibName) where T : UIView
        {
            var objects = NSBundle.MainBundle.LoadNib(nibName, null, null);
            var root = Runtime.GetNSObject(objects.ValueAt(0)) as T;

            return root;
        }
    }
}
