using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace CMScoutIntrinsic {

    sealed class GroupBoxControl : ContentControl {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(String), typeof(GroupBoxControl), null);

        public String Header { get { return (String)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }



        public GroupBoxControl() {
            this.DefaultStyleKey = typeof(GroupBoxControl);

            this.SizeChanged += OnSizeChanged;
        }

        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _path             = (Path)GetTemplateChild("Path");
            _header           = (FrameworkElement)GetTemplateChild("Header");
            _contentPresenter = (ContentPresenter)GetTemplateChild("ContentPresenter");

            _header.SizeChanged += OnHeaderSizeChanged;
        }



        private void OnSizeChanged(Object sender, SizeChangedEventArgs args) {
            UpdatePath();
        }

        private void OnHeaderSizeChanged(Object sender, SizeChangedEventArgs args) {
            UpdatePath();

            UpdateContentPresenterMargin();
        }

        private void UpdatePath() {
            if(_path   == null) { return; }
            if(_header == null) { return; }

            Double w = this.ActualWidth;
            Double h = this.ActualHeight;

            Double hw = _header.ActualWidth;
            Double hh = _header.ActualHeight;

            PathFigure pathFigure = new PathFigure { IsClosed = false };

            Action<Double, Double> addStart = (x, y) => { pathFigure.StartPoint = new Point { X = x, Y = y }; };
            Action<Double, Double> addLine  = (x, y) => { pathFigure.Segments.Add(new LineSegment { Point = new Point { X = x, Y = y } }); };

            addStart(8 + hw, hh / 2);
            addLine(w - 1, hh / 2);
            addLine(w - 1, h - 1);
            addLine(1, h - 1);
            addLine(1, hh / 2);
            addLine(8, hh / 2);

            PathGeometry pathGeometry = new PathGeometry();

            pathGeometry.Figures.Add(pathFigure);

            _path.Data = pathGeometry;
        }

        private void UpdateContentPresenterMargin() {
            if(_contentPresenter == null) { return; }
            if(_header           == null) { return; }

            Double hw = _header.ActualWidth;
            Double hh = _header.ActualHeight;

            _contentPresenter.Margin = new Thickness(6, hh + 2, 6, 6);
        }



        private Path             _path;
        private FrameworkElement _header;
        private ContentPresenter _contentPresenter;
    }

}
