using Google.Android.Material.BottomSheet;
using Xamarin.Slide.Up.Panel.Android.Controls;

namespace Xamarin.Slide.Up.Panel.Android.Extensions
{
    public static class SlideUpPanelPresentationExtensions
    {
        public static int ToBottomSheetBehaviorState(this SlideUpPanelPresentation presentation)
        {
            switch (presentation)
            {
                case SlideUpPanelPresentation.Expanded:
                    return BottomSheetBehavior.StateExpanded;
                case SlideUpPanelPresentation.Collapsed:
                    return BottomSheetBehavior.StateCollapsed;
                default:
                    return BottomSheetBehavior.StateHalfExpanded;
            }
        }
    }
}
