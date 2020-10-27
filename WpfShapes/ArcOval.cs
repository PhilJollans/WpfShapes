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
  /// ArcOval is like an oval, bent to the form of an arc.
  /// </summary>
  public class ArcOval : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register ( "StartAngle",
                                      typeof(double),
                                      typeof(ArcOval),
                                      new FrameworkPropertyMetadata ( -30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty EndAngleProperty =
        DependencyProperty.Register ( "EndAngle",
                                      typeof(double),
                                      typeof(ArcOval),
                                      new FrameworkPropertyMetadata ( +30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register ( "InnerRadius",
                                      typeof(double),
                                      typeof(ArcOval),
                                      new FrameworkPropertyMetadata ( 70.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register ( "OuterRadius",
                                      typeof(double),
                                      typeof(ArcOval),
                                      new FrameworkPropertyMetadata ( 80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register ( "Center",
                                      typeof(Point),
                                      typeof(ArcOval),
                                      new FrameworkPropertyMetadata ( new Point(0,0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public ArcOval ()
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
    public double StartAngle
    {
      get { return Convert.ToDouble(GetValue(StartAngleProperty)); }
      set { SetValue(StartAngleProperty, value); }
    }

    public double EndAngle
    {
      get { return Convert.ToDouble(GetValue(EndAngleProperty)); }
      set { SetValue(EndAngleProperty, value); }
    }

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
      var ao = d as ArcOval ;
      ao.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var offset = (Vector)Center ;

      double EndDiameter        = OuterRadius - InnerRadius ;
      double EndRadius          = EndDiameter / 2 ;
      double centreRadius       = ( OuterRadius + InnerRadius ) / 2.0 ;
      double semiCircleAngle    = Math.Asin ( EndRadius / centreRadius ) ;

      double startRadians       = Math.PI * StartAngle / 180 ;
      double endRadians         = Math.PI * EndAngle   / 180 ;
      double a1                 = startRadians + semiCircleAngle ;
      double a2                 = endRadians - semiCircleAngle ;

      double c1 = Math.Cos ( a1 ) ;
      double s1 = Math.Sin ( a1 ) ;
      double c2 = Math.Cos ( a2 ) ;
      double s2 = Math.Sin ( a2 ) ;

      var p1 = new Point ( OuterRadius      * s1, -OuterRadius * c1 ) + offset ;
      var p2 = new Point ( OuterRadius      * s2, -OuterRadius * c2 ) + offset ;
      var p3 = new Point ( InnerRadius      * s2, -InnerRadius * c2 ) + offset ;
      var p4 = new Point ( InnerRadius      * s1, -InnerRadius * c1 ) + offset ;

      var sb = new StringBuilder() ;

      sb.AppendFormat ( CultureInfo.InvariantCulture, "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", OuterRadius, a2-a1, 1, p2.X, p2.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", EndRadius, Math.PI, 1, p3.X, p3.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", InnerRadius, a2-a1, 0, p4.X, p4.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", EndRadius, Math.PI, 1, p1.X, p1.Y ) ;
      sb.Append ( "Z " ) ;

      _path = sb.ToString() ;

      Debug.WriteLine ( _path ) ;
    }

  }
}
