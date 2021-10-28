using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Shape;
using Xamarin.Slide.Up.Panel.Android.Extensions;
using Xamarin.Slide.Up.Panel.Android.Utilities;
using static Google.Android.Material.BottomSheet.BottomSheetBehavior;

namespace Xamarin.Slide.Up.Panel.Android.Controls
{
    public enum SlideUpPanelPresentation
    {
        Default,
        Expanded,
        Collapsed
    }

    [Register("xamarin.slide.up.panel.android.controls.SlideUpPanelLayout")]
    public class SlideUpPanelLayout : LinearLayout
    {
        private int _compactViewLayoutId;
        private View _compactView;
        private ViewGroup _compactViewContainer;
        private bool _disposed;

        public SlideUpPanelLayout(Context context)
            : this(context, default)
        {
        }

        public SlideUpPanelLayout(Context context, IAttributeSet attrs)
            : this(context, attrs, default)
        {
        }

        public SlideUpPanelLayout(Context context, IAttributeSet attrs, int defStyleAttr)
            : this(context, attrs, defStyleAttr, default)
        {
        }

        public SlideUpPanelLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            SetupView(context);
            CompactLayoutViewId = LayoutAttributeHelper.ReadSlideUpPanelLayoutCompactLayoutId(context, attrs);
        }

        public int CompactLayoutViewId
        {
            get
            {
                return _compactViewLayoutId;
            }

            set
            {
                if (_compactViewLayoutId == value)
                {
                    return;
                }

                _compactViewLayoutId = value;
                var compactView = LayoutInflater.From(Context).Inflate(value, _compactViewContainer, false);
                CompactView = compactView;
            }
        }

        public View CompactView
        {
            get
            {
                return _compactView;
            }

            set
            {
                if (_compactView == value)
                {
                    return;
                }

                var oldView = _compactView;
                _compactView = value;
                UpdateCompactPanelView(oldView, value);
            }
        }

        public Task PresentPanel()
        {
            return PresentPanel(SlideUpPanelPresentation.Default);
        }

        public Task PresentPanel(SlideUpPanelPresentation presentation)
        {
            PrepareForPanelPresentation();

            _compactViewContainer.Visibility = ViewStates.Gone;
            CurrentBehavior.State = presentation.ToBottomSheetBehaviorState();

            return Task.CompletedTask;
        }

        public Task PresentCompactPanel()
        {
            if (CanPresentCompactPanel)
            {
                _compactViewContainer.Visibility = ViewStates.Visible;
                CurrentBehavior.State = BottomSheetBehavior.StateHidden;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public Task DismissPanel()
        {
            CurrentBehavior.State = BottomSheetBehavior.StateHidden;
            _compactViewContainer.Visibility = ViewStates.Gone;

            return Task.CompletedTask;
        }

        protected SlideUpPanelLayout(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected BottomSheetBehavior CurrentBehavior
        {
            get
            {
                return BottomSheetBehavior.From(this);
            }
        }

        protected bool CanPresentCompactPanel
        {
            get
            {
                return CompactLayoutViewId != 0;
            }
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
                _compactView?.Dispose();
                _compactView = null;
                _compactViewContainer?.Dispose();
                _compactViewContainer = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (!(Parent is CoordinatorLayout))
            {
                throw new Exception($"{nameof(SlideUpPanelLayout)} parent has to be {nameof(CoordinatorLayout)}");
            }

            PrepareForPanelPresentation();
        }

        private void SetupView(Context context)
        {
            _compactViewContainer = new BottomNavigationView(context)
            {
                Background = new ColorDrawable(Color.White),
                LayoutParameters = new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Visibility = ViewStates.Gone
            };

            ((CoordinatorLayout.LayoutParams)_compactViewContainer.LayoutParameters).Gravity = (int)GravityFlags.Bottom;

            var backgroundShapeModel = new ShapeAppearanceModel.Builder()
                .SetTopLeftCorner(CornerFamily.Rounded, context.ConvertDpToPx(15.0f))
                .SetTopRightCorner(CornerFamily.Rounded, context.ConvertDpToPx(15.0f))
                .Build();

            var backgroundDrawable = new MaterialShapeDrawable(backgroundShapeModel)
            {
                FillColor = ColorStateList.ValueOf(Color.White),
                StrokeWidth = context.ConvertDpToPx(1.0f),
                StrokeColor = ColorStateList.ValueOf(Color.LightGray)
            };

            var barDrawable = new GradientDrawable();
            barDrawable.SetColor(Color.Gray);
            barDrawable.SetCornerRadius(context.ConvertDpToPx(2.0f));

            var layerDrawable = new LayerDrawable(new[] { backgroundDrawable, (Drawable)barDrawable });
            layerDrawable.SetLayerGravity(1, GravityFlags.CenterHorizontal | GravityFlags.Top);
            layerDrawable.SetLayerWidth(1, (int)context.ConvertDpToPx(20.0f));
            layerDrawable.SetLayerHeight(1, (int)context.ConvertDpToPx(4.0f));
            layerDrawable.SetLayerInsetTop(1, (int)context.ConvertDpToPx(8.0f));

            Background = layerDrawable;
        }

        private void PrepareForPanelPresentation()
        {
            if (_compactViewContainer.Parent == null)
            {
                ((ViewGroup)Parent).AddView(_compactViewContainer);
            }

            BottomSheetBehavior behavior;

            try
            {
                behavior = CurrentBehavior;
            }
            catch (Java.Lang.IllegalArgumentException e)
            {
                var coordinatorLayoutParams = LayoutParameters as CoordinatorLayout.LayoutParams;
                coordinatorLayoutParams.Behavior = behavior = new BottomSheetBehavior(Context, null);
                behavior.PeekHeight = 300;
                behavior.Hideable = true;
                behavior.HalfExpandedRatio = 0.33f;
                behavior.FitToContents = false;
                behavior.State = BottomSheetBehavior.StateHidden;
                behavior.AddBottomSheetCallback(new SlideUpPanelBottomSheetCallback(this));
            }            
        }

        private void UpdateCompactPanelView(View oldView, View newView)
        {
            for (int i = 0; i < _compactViewContainer.ChildCount; i++)
            {
                var childView = _compactViewContainer.GetChildAt(i);

                if (childView == oldView)
                {
                    _compactViewContainer.RemoveView(oldView);
                }
            }

            _compactViewContainer.AddView(newView);
        }

        private class SlideUpPanelBottomSheetCallback : BottomSheetCallback
        {
            SlideUpPanelLayout _panel;

            public SlideUpPanelBottomSheetCallback(SlideUpPanelLayout panel)
            {
                _panel = panel;
            }

            public override void OnSlide(View bottomSheet, float newState)
            {
            }

            public override void OnStateChanged(View p0, int p1)
            {                
                if (p1 == StateHidden)
                {
                    _panel.PresentCompactPanel();
                }
            }

            protected SlideUpPanelBottomSheetCallback(IntPtr javaReference,
                JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }
        }
    }
}
