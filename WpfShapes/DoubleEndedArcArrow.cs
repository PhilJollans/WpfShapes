using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace WpfShapes
{
  /// <summary>
  /// DoubleEndedArcArrow is an double ended arrow along an arc, i.e. part of the edge of a circle
  /// </summary>
  public class DoubleEndedArcArrow : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register ( "StartAngle",
                                      typeof(double),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( -30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty EndAngleProperty =
        DependencyProperty.Register ( "EndAngle",
                                      typeof(double),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( +30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register ( "InnerRadius",
                                      typeof(double),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( 70.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register ( "OuterRadius",
                                      typeof(double),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( 80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty ArrowLengthRatioProperty =
        DependencyProperty.Register ( "ArrowLengthRatio",
                                      typeof(double),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( 0.25,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty ArrowWidthRatioProperty =
        DependencyProperty.Register ( "ArrowWidthRatio",
                                      typeof(double),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( 3.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register ( "Center",
                                      typeof(Point),
                                      typeof(DoubleEndedArcArrow),
                                      new FrameworkPropertyMetadata ( new Point(0,0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public DoubleEndedArcArrow ()
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

    public double ArrowLengthRatio
    {
      get { return Convert.ToDouble(GetValue(ArrowLengthRatioProperty)); }
      set { SetValue(ArrowLengthRatioProperty, value); }
    }

    public double ArrowWidthRatio
    {
      get { return Convert.ToDouble(GetValue(ArrowWidthRatioProperty)); }
      set { SetValue(ArrowWidthRatioProperty, value); }
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
      var aa = d as DoubleEndedArcArrow ;
      aa.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var offset = (Vector)Center ;

      double LineWidth          = OuterRadius - InnerRadius ;
      double ArrowWidth         = LineWidth * ArrowWidthRatio ;
      double centreRadius       = ( OuterRadius + InnerRadius ) / 2.0 ;
      double arrowOuterRadius   = centreRadius + ArrowWidth / 2.0 ;
      double arrowInnerRadius   = centreRadius - ArrowWidth / 2.0 ;

      double startArrowAngle    = StartAngle + ArrowLengthRatio * ( EndAngle - StartAngle ) ;
      double endArrowAngle      = EndAngle - ArrowLengthRatio * ( EndAngle - StartAngle ) ;
      bool   sweepDirectionFlag = ( EndAngle > StartAngle ) ;

      // For users, the angles are defined in degrees.
      // Convert them to radians
      double startRadians       = Math.PI * StartAngle      / 180 ;
      double endRadians         = Math.PI * EndAngle        / 180 ;
      double startArrowRadians  = Math.PI * startArrowAngle / 180 ;
      double endArrowRadians    = Math.PI * endArrowAngle   / 180 ;

      double c1 = Math.Cos ( startRadians ) ;
      double s1 = Math.Sin ( startRadians ) ;
      double c2 = Math.Cos ( startArrowRadians ) ;
      double s2 = Math.Sin ( startArrowRadians ) ;
      double c3 = Math.Cos ( endArrowRadians ) ;
      double s3 = Math.Sin ( endArrowRadians ) ;
      double c4 = Math.Cos ( endRadians ) ;
      double s4 = Math.Sin ( endRadians ) ;

      var p1 = new Point ( centreRadius     * s1, -centreRadius     * c1 ) + offset ;
      var p2 = new Point ( arrowOuterRadius * s2, -arrowOuterRadius * c2 ) + offset ;
      var p3 = new Point ( OuterRadius      * s2, -OuterRadius      * c2 ) + offset ;
      var p4 = new Point ( OuterRadius      * s3, -OuterRadius      * c3 ) + offset ;
      var p5 = new Point ( arrowOuterRadius * s3, -arrowOuterRadius * c3 ) + offset ;
      var p6 = new Point ( centreRadius     * s4, -centreRadius     * c4 ) + offset ;
      var p7 = new Point ( arrowInnerRadius * s3, -arrowInnerRadius * c3 ) + offset ;
      var p8 = new Point ( InnerRadius      * s3, -InnerRadius      * c3 ) + offset ;
      var p9 = new Point ( InnerRadius      * s2, -InnerRadius      * c2 ) + offset ;
      var p0 = new Point ( arrowInnerRadius * s2, -arrowInnerRadius * c2 ) + offset ;

      var sb = new StringBuilder() ;

      sb.AppendFormat ( CultureInfo.InvariantCulture, "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p2.X, p2.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", OuterRadius, endArrowRadians-endArrowRadians, sweepDirectionFlag ? 1 : 0, p4.X, p4.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p5.X, p5.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p6.X, p6.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p7.X, p7.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p8.X, p8.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", InnerRadius, endArrowRadians-endArrowRadians, sweepDirectionFlag ? 0 : 1, p9.X, p9.Y ) ;
      sb.AppendFormat ( CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", p0.X, p0.Y ) ;
      sb.Append ( "Z " ) ;

      _path = sb.ToString() ;

      Debug.WriteLine ( _path ) ;
    }

  }
}
