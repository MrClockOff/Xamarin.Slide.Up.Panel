using Android.Content;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;

namespace Xamarin.Slide.Up.Panel.Android.Extensions
{
    /// <summary>
    /// Android context extensions
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Get drawable resource Id by resource name using default package
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static int GetDrawableIdByName(this Context context, string resourceName)
        {
            return GetDrawableIdByName(context, resourceName, context.PackageName);
        }

        /// <summary>
        /// Get drawable resource Id by resource name using specific package
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceName"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static int GetDrawableIdByName(this Context context, string resourceName, string packageName)
        {
            var resourceId = context.Resources.GetIdentifier(resourceName.Split(".")[0], "drawable", packageName);
            return resourceId;
        }

        /// <summary>
        /// Get drawable resource by resource name using default package
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Drawable GetDrawableByName(this Context context, string resourceName)
        {
            return GetDrawableByName(context, resourceName, context.PackageName);
        }

        /// <summary>
        /// Get drawable resource by resource name using specific package
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceName"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static Drawable GetDrawableByName(this Context context, string resourceName, string packageName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            var resourceId = GetDrawableIdByName(context, resourceName, packageName);
            var drawable = ContextCompat.GetDrawable(context, resourceId);
            return drawable;
        }

        /// <summary>
        /// Convert regular pixels to density independent pixels
        /// </summary>
        /// <param name="context"></param>
        /// <param name="px"></param>
        /// <returns></returns>
        public static float ConvertPxToDp(this Context context, float px)
        {
            var density = context.Resources.DisplayMetrics.Density;
            var dp = px / density;
            return dp;
        }

        /// <summary>
        /// Convert density independent pixels to regular pixels
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static float ConvertDpToPx(this Context context, float dp)
        {
            var density = context.Resources.DisplayMetrics.Density;
            var px = dp * density;
            return px;
        }
    }
}

