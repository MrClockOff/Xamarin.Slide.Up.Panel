using System;
using System.Linq;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Slide.Up.Panel.iOS.Extensions;

namespace Xamarin.Slide.Up.Panel.iOS.Controls
{
    public enum SlideUpPanelPresentation
    {
        Default,
        Expanded,
        Collapsed
    }

    public class SlideUpPanelViewController : UIViewController
    {
        private const float PanelCornerRadius = 15.0f;
        private PanelView _panelView;
        private UIView _panelInputAccessoryView;
        private UIView _safeAreAwareInputAccessoryView;
        private CGPoint _panelViewStartPosition = CGPoint.Empty;
        private bool _disposed;

        public SlideUpPanelViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
            InitPanel();
        }

        public SlideUpPanelViewController(NSCoder coder)
            : base(coder)
        {
            InitPanel();
        }

        public SlideUpPanelViewController()
        {
            InitPanel();
        }

        public bool CanCollapseToInputAccessoryView { get; set; }

        public override bool CanBecomeFirstResponder
        {
            get
            {
                return CanCollapseToInputAccessoryView;
            }
        }

        public override UIView InputAccessoryView
        {
            get
            {
                if (_safeAreAwareInputAccessoryView == null)
                {
                    _safeAreAwareInputAccessoryView = _panelInputAccessoryView.ToSafeAreaAwareInputAccessoryView();
                }

                return _safeAreAwareInputAccessoryView;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupView();
        }

        public void SetPanelView(UIView view)
        {
            _panelView.SetContentView(view);
        }

        public void SetPanelInputAccessoryView(UIView view)
        {
            foreach (var subview in _panelInputAccessoryView.Subviews)
            {
                subview.RemoveFromSuperview();
            }

            _panelInputAccessoryView.Add(view);
            view.TranslatesAutoresizingMaskIntoConstraints = false;
            NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[]
            {
                view.TopAnchor.ConstraintEqualTo(_panelInputAccessoryView.TopAnchor),
                view.LeadingAnchor.ConstraintEqualTo(_panelInputAccessoryView.LeadingAnchor),
                _panelInputAccessoryView.BottomAnchor.ConstraintEqualTo(view.BottomAnchor),
                _panelInputAccessoryView.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor)
            });
        }

        public Task PresentPannel(UIViewController presentingController)
        {
            return PresentPannel(presentingController, SlideUpPanelPresentation.Default);
        }

        public Task PresentPannel(UIViewController presentingController, SlideUpPanelPresentation presentation)
        {
            PrepareForPanelPresentation(presentingController);

            if (CanBecomeFirstResponder)
            {
                ResignFirstResponder();
            }

            return UIView.AnimateNotifyAsync(
                0.5,
                0.0,
                1.0f,
                1.0f,
                UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    View.Alpha = 0.8f;
                    _panelView.Frame = GetPresentedPanelFrame(presentingController, presentation);
                });
        }

        public Task PresentPanelInputAccessoryView(UIViewController presentingController)
        {
            PrepareForPanelPresentation(presentingController);
            BecomeFirstResponder();

            return UIView.AnimateNotifyAsync(
                0.5,
                0.0,
                1.0f,
                1.0f,
                UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    View.Alpha = 0.8f;
                    _panelView.Frame = GetDismissedPanelFrame(presentingController);
                });
        }

        public async Task DismissPanel()
        {
            if (ParentViewController == null)
            {
                return;
            }

            await UIView.AnimateNotifyAsync(
                0.5,
                0.0,
                1.0f,
                1.0f,
                UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                    View.Alpha = 0.0f;
                    _panelView.Frame = GetDismissedPanelFrame(ParentViewController);
                });

            WillMoveToParentViewController(null);
            View.RemoveFromSuperview();
            _panelView.RemoveFromSuperview();
            RemoveFromParentViewController();
            DidMoveToParentViewController(null);

            if (CanBecomeFirstResponder)
            {
                ResignFirstResponder();
            }
        }

        protected SlideUpPanelViewController(IntPtr handle)
            : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                _panelView?.Dispose();
                _panelView = null;
                _panelInputAccessoryView?.Dispose();
                _panelInputAccessoryView = null;
                _safeAreAwareInputAccessoryView?.Dispose();
                _safeAreAwareInputAccessoryView = null;
    }
            base.Dispose(disposing);
        }

        private void InitPanel()
        {
            _panelView = CreatePanelView();
            _panelInputAccessoryView = new UIView();
        }

        private PanelView CreatePanelView()
        {
            var view = new PanelView();
            view.AddGestureRecognizer(new UIPanGestureRecognizer(PanelPanGestureRecognizer_Handler)); ;

            return view;
        }

        private void PrepareForPanelPresentation(UIViewController presentingController)
        {
            if (ParentViewController != null)
            {
                return;
            }

            WillMoveToParentViewController(presentingController);
            View.Alpha = 0.0f;
            View.Frame = presentingController.View.Frame;
            _panelView.Frame = GetDismissedPanelFrame(presentingController);
            presentingController.View.Add(View);
            presentingController.View.Add(_panelView);
            presentingController.AddChildViewController(this);
            DidMoveToParentViewController(presentingController);
        }

        private void SetupView()
        {
            View.BackgroundColor = UIColor.Clear;
            View.UserInteractionEnabled = false;
            InputAccessoryView.BackgroundColor = UIColor.White;
        }        

        private void PanelPanGestureRecognizer_Handler(UIPanGestureRecognizer sender)
        {
            switch (sender.State)
            {
                case UIGestureRecognizerState.Began:
                    OnPanelViewDraggingStarted(sender);
                    break;

                case UIGestureRecognizerState.Changed:
                    OnPanelViewDraggingChanged(sender);          
                    break;

                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                    OnPanelViewDraggingFinished(sender);
                    break;
            }
        }

        private void OnPanelViewDraggingStarted(UIPanGestureRecognizer sender)
        {
            _panelViewStartPosition = sender.LocationInView(_panelView);
        }

        private void OnPanelViewDraggingChanged(UIPanGestureRecognizer sender)
        {
            sender.SetTranslation(CGPoint.Empty, ParentViewController.View);
            var endPosition = sender.LocationInView(_panelView);
            var difference = endPosition.Y - _panelViewStartPosition.Y;
            var panelMoveFrame = _panelView.Frame;

            panelMoveFrame.Location = new CGPoint(_panelView.Frame.Location.X, _panelView.Frame.Location.Y + difference);
            panelMoveFrame.Size = new CGSize(_panelView.Frame.Size.Width, _panelView.Frame.Size.Height - difference);
            _panelView.Frame = panelMoveFrame;
        }

        private void OnPanelViewDraggingFinished(UIPanGestureRecognizer sender)
        {
            var velocity = sender.VelocityInView(_panelView);
            var panelViewDraggingDirection = velocity.Y > 0
                ? PanelViewDraggingDirection.Down
                : PanelViewDraggingDirection.Up;
            var panelResizeFrame = _panelView.Frame;

            // Special case to allow user to see whole panel when user pulls panel up and panel heigh is bigger than
            // actual screen height
            if (panelViewDraggingDirection == PanelViewDraggingDirection.Up
                && GetPresentationMinimumYPos() < GetPresentationYPos(SlideUpPanelPresentation.Expanded)
                && _panelView.Frame.Y < GetPresentationYPos(SlideUpPanelPresentation.Expanded)
                && _panelView.Frame.Y > GetPresentationMinimumYPos())
            {
                return;
            }

            // Special case when user pulls panel down too far and CanCollapseToInputAccessoryView is set to true
            if (panelViewDraggingDirection == PanelViewDraggingDirection.Down
                && CanCollapseToInputAccessoryView
                && _panelView.Frame.Y > GetPresentationYPos(SlideUpPanelPresentation.Collapsed))
            {
                _ = PresentPanelInputAccessoryView(ParentViewController);
                return;
            }

            switch (panelViewDraggingDirection)
            {
                case PanelViewDraggingDirection.Up:
                    // Prevent user from pulling panel up too far
                    if (GetPresentationMinimumYPos() < GetPresentationYPos(SlideUpPanelPresentation.Expanded)
                        && _panelView.Frame.Y < GetPresentationMinimumYPos())
                    {
                        panelResizeFrame = new CGRect(0.0f, GetPresentationMinimumYPos(), ParentViewController.View.Frame.Width, _panelView.EstimatedSize.Height);
                    }
                    else if (_panelView.Frame.Y > GetPresentationYPos(SlideUpPanelPresentation.Default))
                    {
                        panelResizeFrame = GetPresentedPanelFrame(ParentViewController, SlideUpPanelPresentation.Default);
                    }
                    else
                    {
                        panelResizeFrame = GetPresentedPanelFrame(ParentViewController, SlideUpPanelPresentation.Expanded);
                    }
                    break;

                case PanelViewDraggingDirection.Down:
                default:
                    // This check prevents panel going into Default presentation when user actidently pulls
                    // panel down upon pulling it up to achieve Expanded presentation
                    if (_panelView.Frame.Y < GetPresentationYPos(SlideUpPanelPresentation.Expanded))
                    {
                        panelResizeFrame = GetPresentedPanelFrame(ParentViewController, SlideUpPanelPresentation.Expanded);
                    }
                    else if (_panelView.Frame.Y > GetPresentationYPos(SlideUpPanelPresentation.Default))
                    {
                        panelResizeFrame = GetPresentedPanelFrame(ParentViewController, SlideUpPanelPresentation.Collapsed);
                    }
                    else
                    {
                        panelResizeFrame = GetPresentedPanelFrame(ParentViewController, SlideUpPanelPresentation.Default);
                    }

                    break;
            }

            UIView.AnimateNotify(
                0.5,
                0,
                0.7f,
                1.0f,
                UIViewAnimationOptions.CurveEaseOut,
                () =>
                {
                    _panelView.Frame = panelResizeFrame;
                },
                null);
        }

        private CGRect GetPresentedPanelFrame(UIViewController parentViewController, SlideUpPanelPresentation presentation)
        {
            return new CGRect(0.0f, GetPresentationYPos(presentation), parentViewController.View.Frame.Width, _panelView.EstimatedSize.Height);
        }

        private CGRect GetDismissedPanelFrame(UIViewController parentViewController)
        {
            return new CGRect(0.0f, parentViewController.View.Frame.Height,  parentViewController.View.Frame.Width, _panelView.EstimatedSize.Height);
        }

        private nfloat GetPresentationYPos(SlideUpPanelPresentation presentation)
        {
            var panelViewExpandedHeight = _panelView.EstimatedSize.Height > ParentViewController.View.Frame.Height * 0.8f
                ? ParentViewController.View.Frame.Height * 0.8f
                : _panelView.EstimatedSize.Height;
            var panelViewCollapsedHeight = panelViewExpandedHeight * 0.2f;
            var panelViewDefaultHeight = panelViewExpandedHeight * 0.4f;

            switch (presentation)
            {
                case SlideUpPanelPresentation.Collapsed:
                    return ParentViewController.View.Frame.Height - panelViewCollapsedHeight;
                case SlideUpPanelPresentation.Expanded:
                    return ParentViewController.View.Frame.Height - panelViewExpandedHeight;
                default:
                    return ParentViewController.View.Frame.Height - panelViewDefaultHeight;
            }
        }

        private nfloat GetPresentationMinimumYPos()
        {
            return ParentViewController.View.Frame.Height - _panelView.EstimatedSize.Height;
        }

        private enum PanelViewDraggingDirection
        {
            Up,
            Down
        }

        private class PanelView : UIView
        {
            private const int PullIndicatorTag = 1;
            private const int ContentWrapperTag = 2;
            private UIView _contentView;
            private bool _disposed;

            public PanelView()
            {
                BackgroundColor = UIColor.White;
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowOffset = CGSize.Empty;
                Layer.ShadowOpacity = 0.3f;
                Layer.ShadowRadius = 4.0f;
                Layer.ShouldRasterize = true;
                Layer.RasterizationScale = UIScreen.MainScreen.Scale;
                Layer.CornerRadius = PanelCornerRadius;
                Layer.MaskedCorners = CACornerMask.MaxXMinYCorner | CACornerMask.MinXMinYCorner;

                _contentView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                _contentView.Layer.MasksToBounds = true;
                _contentView.Layer.CornerRadius = PanelCornerRadius;
                _contentView.Layer.MaskedCorners = CACornerMask.MaxXMinYCorner | CACornerMask.MinXMinYCorner;

                Add(_contentView);

                NSLayoutConstraint.ActivateConstraints(new[]
                {
                    _contentView.TopAnchor.ConstraintEqualTo(TopAnchor),
                    _contentView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor),
                    BottomAnchor.ConstraintEqualTo(_contentView.BottomAnchor),
                    TrailingAnchor.ConstraintEqualTo(_contentView.TrailingAnchor)
                });

                SetPullIndicator();
            }

            public CGSize EstimatedSize
            {
                get
                {
                    var contentWrapperView = _contentView.Subviews.FirstOrDefault(view => view.Tag == ContentWrapperTag);
                    var minCompressedSize = contentWrapperView != null
                        ? contentWrapperView.SystemLayoutSizeFittingSize(UILayoutFittingCompressedSize)
                        : SystemLayoutSizeFittingSize(UILayoutFittingCompressedSize);
                    var maxHeight = minCompressedSize.Height;

                    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                    {
                        var bottomWindowInset = UIApplication.SharedApplication.Windows.First().SafeAreaInsets.Bottom;
                        maxHeight += bottomWindowInset;
                    }
                    else
                    {
                        var bottomWindowInset = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;
                        maxHeight += bottomWindowInset;
                    }

                    return new CGSize(minCompressedSize.Width, maxHeight);
                }
            }

            public void SetContentView(UIView view)
            {
                foreach (var subview in _contentView.Subviews)
                {
                    if (subview.Tag == PullIndicatorTag)
                    {
                        continue;
                    }

                    subview.RemoveFromSuperview();
                }

                var wrapperView = new UIView
                {
                    Tag = ContentWrapperTag,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };


                view.TranslatesAutoresizingMaskIntoConstraints = false;
                wrapperView.Add(view);
                NSLayoutConstraint.ActivateConstraints(new[]
                {
                    view.TopAnchor.ConstraintEqualTo(wrapperView.TopAnchor),
                    view.LeadingAnchor.ConstraintEqualTo(wrapperView.LeadingAnchor),
                    wrapperView.BottomAnchor.ConstraintGreaterThanOrEqualTo(view.BottomAnchor),
                    wrapperView.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor)
                });

                _contentView.Add(wrapperView);

                NSLayoutConstraint.ActivateConstraints(new []
                {
                    wrapperView.TopAnchor.ConstraintEqualTo(_contentView.TopAnchor, 20.0f),
                    wrapperView.LeadingAnchor.ConstraintEqualTo(_contentView.LeadingAnchor),
                    _contentView.TrailingAnchor.ConstraintEqualTo(wrapperView.TrailingAnchor)
                });
            }

            protected PanelView(IntPtr handle)
                : base(handle)
            {
            }

            protected override void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (disposing)
                {
                    _contentView?.Dispose();
                }

                base.Dispose(disposing);
            }

            private protected virtual void SetPullIndicator()
            {
                var pullIndicatorView = new UIView()
                {
                    BackgroundColor = UIColor.Gray,
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Tag = PullIndicatorTag
                };
                pullIndicatorView.Layer.MasksToBounds = true;
                pullIndicatorView.Layer.CornerRadius = 2.0f;
                _contentView.Add(pullIndicatorView);

                NSLayoutConstraint.ActivateConstraints(new[]
                {
                    pullIndicatorView.HeightAnchor.ConstraintEqualTo(4.0f),
                    pullIndicatorView.WidthAnchor.ConstraintEqualTo(30.0f),
                    pullIndicatorView.CenterXAnchor.ConstraintEqualTo(_contentView.CenterXAnchor),
                    pullIndicatorView.TopAnchor.ConstraintEqualTo(_contentView.TopAnchor, 8.0f)
                });
            }
        }
    }
}
