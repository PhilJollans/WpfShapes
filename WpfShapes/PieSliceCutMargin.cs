using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfShapes
{
  /// <summary>
  /// PieSliceCutMargin is equivalent to the PieSliceCut shape, escpet that the two radial
  /// sides have been shrunk inwards with a margin parallel to the original shape.
  /// </summary>
  public class PieSliceCutMargin : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register ( "StartAngle",
                                      typeof(double),
                                      typeof(PieSliceCutMargin),
                                      new FrameworkPropertyMetadata ( -30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty EndAngleProperty =
        DependencyProperty.Register ( "EndAngle",
                                      typeof(double),
                                      typeof(PieSliceCutMargin),
                                      new FrameworkPropertyMetadata ( +30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty RadiusMarginProperty =
        DependencyProperty.Register ( "RadiusMargin",
                                      typeof(double),
                                      typeof(PieSliceCutMargin),
                                      new FrameworkPropertyMetadata ( 3.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register ( "InnerRadius",
                                      typeof(double),
                                      typeof(PieSliceCutMargin),
                                      new FrameworkPropertyMetadata ( 20.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register ( "OuterRadius",
                                      typeof(double),
                                      typeof(PieSliceCutMargin),
                                      new FrameworkPropertyMetadata ( 80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register ( "Center",
                                      typeof(Point),
                                      typeof(PieSliceCutMargin),
                                      new FrameworkPropertyMetadata ( new Point(0,0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public PieSliceCutMargin ()
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

    public double RadiusMargin
    {
      get { return Convert.ToDouble(GetValue(RadiusMarginProperty)); }
      set { SetValue(RadiusMarginProperty, value); }
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
      var ao = d as PieSliceCutMargin ;
      ao.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var offset = (Vector)Center ;

      double startRadians       = Math.PI * StartAngle / 180 ;
      double endRadians         = Math.PI * EndAngle   / 180 ;
      double theta              = endRadians - startRadians ;

      if ( ( theta != 0 ) && ( OuterRadius != 0 ) && ( InnerRadius != 0 ) )
      {
        // alpha is the angle subtended by the margin on the outer radius.
        // It must be less than half of theta.
        double alpha = Math.Asin ( RadiusMargin / OuterRadius ) ;

        if ( alpha < 0.5 * theta )
        {
          double beta = 0 ;

          // If the margin is too big, then there will only be one point at the centre.
          // Indicate this with the flag NoCut.
          bool  NoCut = false ;

          if ( InnerRadius == 0 )
          {
            NoCut = true ;
          }
          else
          {
            // beta is the angle subtended by the margin on the inner radius.
            beta = Math.Asin ( RadiusMargin / InnerRadius ) ;

            if ( beta > 0.5 * theta )
            {
              NoCut = true ;
            }
          }

          double c1 = Math.Cos ( startRadians ) ;
          double s1 = Math.Sin ( startRadians ) ;
          double c2 = Math.Cos ( endRadians ) ;
          double s2 = Math.Sin ( endRadians ) ;
          double st = Math.Sin ( theta ) ;

          double c3 = Math.Cos ( startRadians + alpha ) ;
          double s3 = Math.Sin ( startRadians + alpha  ) ;
          double c4 = Math.Cos ( endRadians - alpha ) ;
          double s4 = Math.Sin ( endRadians - alpha ) ;

          double c5 = Math.Cos ( startRadians + beta ) ;
          double s5 = Math.Sin ( startRadians + beta  ) ;
          double c6 = Math.Cos ( endRadians - beta ) ;
          double s6 = Math.Sin ( endRadians - beta ) ;

          if ( NoCut )
          {
            // This is identical to the shape PieSliceMargin

            // Imagine the margin lines parallel to the two sides forming a small diamond shape in the
            // centre of the circle. b is the length of each side of this diamond. Create vectors for
            // the two sides of the diamond, which we can add to the centre point.
            var b  = RadiusMargin / st ;
            var v1 = new Vector ( b * s1, -b * c1 ) ;
            var v2 = new Vector ( b * s2, -b * c2 ) ;

            var p1 = new Point ( OuterRadius      * s3, -OuterRadius * c3 ) + offset ;
            var p2 = new Point ( OuterRadius      * s4, -OuterRadius * c4 ) + offset ;
            var p3 = Center + v1 + v2 ;

            var sb = new StringBuilder() ;

            sb.AppendFormat ( "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
            sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", OuterRadius, endRadians-startRadians, 1, p2.X, p2.Y ) ;
            sb.AppendFormat ( "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
            sb.Append ( "Z " ) ;

            _path = sb.ToString() ;
          }
          else
          {
            var p1 = new Point ( OuterRadius      * s3, -OuterRadius * c3 ) + offset ;
            var p2 = new Point ( OuterRadius      * s4, -OuterRadius * c4 ) + offset ;
            var p3 = new Point ( InnerRadius      * s6, -InnerRadius * c6 ) + offset ;
            var p4 = new Point ( InnerRadius      * s5, -InnerRadius * c5 ) + offset ;

            var sb = new StringBuilder() ;

            sb.AppendFormat ( "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
            sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", OuterRadius, endRadians-startRadians, 1, p2.X, p2.Y ) ;
            sb.AppendFormat ( "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
            sb.AppendFormat ( "L {0:F3},{1:F3} ", p4.X, p4.Y ) ;
            sb.Append ( "Z " ) ;

            _path = sb.ToString() ;
          }

          Debug.WriteLine ( _path ) ;
        }
      }
    }

  }
}
