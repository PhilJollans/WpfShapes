using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfShapes
{
  /// <summary>
  /// RingOfRectangularTicks is a ring of n rectangles, evenly spaced on a circle.
  /// The width of each rectangle is approximately the spacing between rectangles
  /// defined by SizeRatio.
  /// </summary>
  public class RingOfRectangularTicks : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register ( "InnerRadius",
                                      typeof(double),
                                      typeof(RingOfRectangularTicks),
                                      new FrameworkPropertyMetadata ( 70.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register ( "OuterRadius",
                                      typeof(double),
                                      typeof(RingOfRectangularTicks),
                                      new FrameworkPropertyMetadata ( 80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty SizeRatioProperty =
        DependencyProperty.Register ( "SizeRatio",
                                      typeof(double),
                                      typeof(RingOfRectangularTicks),
                                      new FrameworkPropertyMetadata ( 7.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty NumberOfTicksProperty =
        DependencyProperty.Register ( "NumberOfTicks",
                                      typeof(int),
                                      typeof(RingOfRectangularTicks),
                                      new FrameworkPropertyMetadata ( 32,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register ( "Center",
                                      typeof(Point),
                                      typeof(RingOfRectangularTicks),
                                      new FrameworkPropertyMetadata ( new Point(0,0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public RingOfRectangularTicks ()
    {
      // Initialise the geometry with the default parameters.
      InitializeGeometry() ;
    }

    protected override Geometry DefiningGeometry
    {
      get
      {
        return Geometry.Parse ( _path ) ;
      }
    }

    //-------------------------------------------------------------------------
    // Getters and setters for the dependency properties
    //-------------------------------------------------------------------------
    public double InnerRadius
    {
      get { return Convert.ToDouble(GetValue(InnerRadiusProperty)); }
      set { SetValue(InnerRadiusProperty, value); }
    }

    public double OuterRadius
    {
      get { return Convert.ToDouble(GetValue(OuterRadiusProperty)); }
      set { SetValue(OuterRadiusProperty, value); }
    }

    public double SizeRatio
    {
      get { return Convert.ToDouble(GetValue(SizeRatioProperty)); }
      set { SetValue(SizeRatioProperty, value); }
    }

    public int NumberOfTicks
    {
      get { return Convert.ToInt32(GetValue(NumberOfTicksProperty)); }
      set { SetValue(NumberOfTicksProperty, value); }
    }

    public Point Center
    {
      get { return (Point)GetValue(CenterProperty); }
      set { SetValue(CenterProperty, value); }
    }

    //-------------------------------------------------------------------------
    // Property changed callbacks
    //-------------------------------------------------------------------------
    public static void OnShapeChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      var ao = d as RingOfRectangularTicks ;
      ao.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var CenterVector = (Vector)Center ;

      double w = 2 * Math.PI * OuterRadius / ( NumberOfTicks * SizeRatio ) ;
      double h = w / 2 ;

      var sb = new StringBuilder() ;

      for ( int i = 0 ; i < NumberOfTicks ; i++ )
      {
        double a = Math.PI * 2 * i / NumberOfTicks ;
        double c = Math.Cos ( a ) ;
        double s = Math.Sin ( a ) ;

        var pOuterMiddle = new Point ( OuterRadius * s, OuterRadius * c ) + CenterVector ;
        var pInnerMiddle = new Point ( InnerRadius * s, InnerRadius * c ) + CenterVector;

        var offset = new Vector ( h * c, -h * s ) ;

        var p1 = pOuterMiddle + offset ;
        var p2 = pInnerMiddle + offset ;
        var p3 = pInnerMiddle - offset ;
        var p4 = pOuterMiddle - offset ;

        sb.AppendFormat ( CultureInfo.InvariantCulture, "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
        sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p2.X, p2.Y ) ;
        sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
        sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p4.X, p4.Y ) ;
        sb.Append ( "Z " ) ;
      }

      _path = sb.ToString() ;

      Debug.WriteLine ( _path ) ;
    }

  }
}
