using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Xamarin.Slide.Up.Panel.iOS.Extensions
{
    public static class UIViewExtensions
    {
        public static UIView ToSafeAreaAwareInputAccessoryView(this UIView contentView)
        {
            return new InputAccessoryWrapperView(contentView);
        }

        private class InputAccessoryWrapperView : UIView
        {
            public InputAccessoryWrapperView(UIView contentView)
            {

                AutoresizingMask = UIViewAutoresizing.FlexibleHeight;

                Add(contentView);
                contentView.TranslatesAutoresizingMaskIntoConstraints = false;

                var contentViewConstraints = new List<NSLayoutConstraint>
                {
                    contentView.TopAnchor.ConstraintEqualTo(TopAnchor),
                    contentView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
                    contentView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor),
                };

                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    contentViewConstraints.Add(contentView.BottomAnchor.ConstraintEqualTo(SafeAreaLayoutGuide.BottomAnchor));
                }
                else
                {
                    contentViewConstraints.Add(contentView.BottomAnchor.ConstraintEqualTo(BottomAnchor));
                }

                NSLayoutConstraint.ActivateConstraints(contentViewConstraints.ToArray());
            }

            public override CGSize IntrinsicContentSize
            {
                get
                {
                    return CGSize.Empty;
                }
            }

            protected InputAccessoryWrapperView(IntPtr handle)
                : base(handle)
            {
            }
        }
    }
}
