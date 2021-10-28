using Android.Content;
using Android.Util;

namespace Xamarin.Slide.Up.Panel.Android.Utilities
{
    public static class LayoutAttributeHelper
    {
        public static int ReadSlideUpPanelLayoutCompactLayoutId(Context context, IAttributeSet attrs)
        {
            return ReadAttributeValue(context, attrs, Resource.Styleable.SlideUpPanelLayout,
                Resource.Styleable.SlideUpPanelLayout_CompactLayoutId);
        }

        public static int ReadAttributeValue(Context context, IAttributeSet attrs, int[] groupId,
            int requiredAttributeId)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, groupId);

            try
            {
                var numStyles = typedArray.IndexCount;

                for (var i = 0; i < numStyles; ++i)
                {
                    var attributeId = typedArray.GetIndex(i);

                    if (attributeId == requiredAttributeId)
                    {
                        return typedArray.GetResourceId(attributeId, 0);
                    }
                }

                return 0;
            }
            finally
            {
                typedArray.Recycle();
            }
        }
    }
}
