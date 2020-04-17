using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfShapes
{
  /// <summary>
  /// GearWheelRing is circle with teeth, like a gear wheel.
  /// This version defines the shape of the teeth with the ratio of tooth size to tooth separation,
  /// for both the inner and outer circle (i.e. the outer edge of the tooth and the base of the tooth).
  /// (An alternative would be to define the tooth angle, but I did not implement that.)
  /// </summary>
  public class GearWheelRing : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register ( "InnerRadius",
                                      typeof(double),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( 70.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register ( "OuterRadius",
                                      typeof(double),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( 80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty InnerRingRadiusProperty =
        DependencyProperty.Register ( "InnerRingRadius",
                                      typeof(double),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( 60.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterToothSeparationRatioProperty =
        DependencyProperty.Register ( "OuterToothSeparationRatio",
                                      typeof(double),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( 0.4,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty InnerToothSeparationRatioProperty =
        DependencyProperty.Register ( "InnerToothSeparationRatio",
                                      typeof(double),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( 0.6,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty NumberOfTeethProperty =
        DependencyProperty.Register ( "NumberOfTeeth",
                                      typeof(int),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( 32,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register ( "Center",
                                      typeof(Point),
                                      typeof(GearWheelRing),
                                      new FrameworkPropertyMetadata ( new Point(0,0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public GearWheelRing ()
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

    public double InnerRingRadius
    {
      get { return Convert.ToDouble(GetValue(InnerRingRadiusProperty)); }
      set { SetValue(InnerRingRadiusProperty, value); }
    }

    public double OuterToothSeparationRatio
    {
      get { return Convert.ToDouble(GetValue(OuterToothSeparationRatioProperty)); }
      set { SetValue(OuterToothSeparationRatioProperty, value); }
    }

    public double InnerToothSeparationRatio
    {
      get { return Convert.ToDouble(GetValue(InnerToothSeparationRatioProperty)); }
      set { SetValue(InnerToothSeparationRatioProperty, value); }
    }

    public int NumberOfTeeth
    {
      get { return Convert.ToInt32(GetValue(NumberOfTeethProperty)); }
      set { SetValue(NumberOfTeethProperty, value); }
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
      var ao = d as GearWheelRing ;
      ao.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var offset = (Vector)Center ;

      // All angle in radians
      var tooth_separation = 2 * Math.PI / NumberOfTeeth ;
      var tooth_outer      = tooth_separation * OuterToothSeparationRatio ;
      var tooth_inner      = tooth_separation * InnerToothSeparationRatio ;
      var half_tooth_outer = tooth_outer / 2 ;
      var half_tooth_inner = tooth_inner / 2 ;

      // and in degrees for the Arc in the path
      var tooth_outer_deg  = tooth_outer * 180 / Math.PI ;
      var tooth_inner_deg  = tooth_inner * 180 / Math.PI ;

      // Start at the bottom of the first tooth.
      double istart  = -half_tooth_inner ;
      double cistart = Math.Cos ( istart ) ;
      double sistart = Math.Sin ( istart ) ;
      var    pstart  = new Point ( InnerRadius * sistart, -InnerRadius * cistart ) + offset ;

      var sb = new StringBuilder() ;
      sb.Append ( "F0 " ) ;
      sb.AppendFormat ( "M {0:F3},{1:F3} ", pstart.X, pstart.Y ) ;

      // In each loop go up the side of the tooth, along the outer arc, down the other side and along the inner arc to the next tooth.
      for ( int i = 0 ; i < NumberOfTeeth ; i++ )
      {
        double a  = i * tooth_separation ;
        double o1 = a - half_tooth_outer ;
        double o2 = a + half_tooth_outer ;
        double i1 = a + half_tooth_inner ;
        double i2 = a + tooth_separation - half_tooth_inner ;

        double co1 = Math.Cos ( o1 ) ;
        double so1 = Math.Sin ( o1 ) ;
        double co2 = Math.Cos ( o2 ) ;
        double so2 = Math.Sin ( o2 ) ;
        double ci1 = Math.Cos ( i1 ) ;
        double si1 = Math.Sin ( i1 ) ;
        double ci2 = Math.Cos ( i2 ) ;
        double si2 = Math.Sin ( i2 ) ;

        var p1 = new Point ( OuterRadius * so1, -OuterRadius * co1 ) + offset ;
        var p2 = new Point ( OuterRadius * so2, -OuterRadius * co2 ) + offset ;
        var p3 = new Point ( InnerRadius * si1, -InnerRadius * ci1 ) + offset ;
        var p4 = new Point ( InnerRadius * si2, -InnerRadius * ci2 ) + offset ;

        sb.AppendFormat ( "L {0:F3},{1:F3} ", p1.X, p1.Y );
        sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 1 {2:F3},{3:F3} ", OuterRadius, tooth_outer_deg, p2.X, p2.Y );
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p3.X, p3.Y );
        sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 1 {2:F3},{3:F3} ", InnerRadius, tooth_inner_deg, p4.X, p4.Y );
      }

      var pInner1 = new Point ( 0, -InnerRingRadius ) + offset ;
      var pInner2 = new Point ( 0, +InnerRingRadius ) + offset ;

      sb.AppendFormat ( "M {0:F3},{1:F3} ", pInner1.X, pInner1.Y ) ;
      sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", InnerRingRadius, Math.PI, pInner2.X, pInner2.Y ) ;
      sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", InnerRingRadius, Math.PI, pInner1.X, pInner1.Y ) ;

      sb.Append ( "Z " ) ;

      _path = sb.ToString() ;

      Debug.WriteLine ( _path ) ;
    }

  }
}
