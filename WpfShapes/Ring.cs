using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfShapes
{
  /// <summary>
  /// Ring is a circle with a circular hole.
  /// </summary>
  public class Ring : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register ( "InnerRadius",
                                      typeof(double),
                                      typeof(Ring),
                                      new FrameworkPropertyMetadata ( 20.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register ( "OuterRadius",
                                      typeof(double),
                                      typeof(Ring),
                                      new FrameworkPropertyMetadata ( 80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register ( "Center",
                                      typeof(Point),
                                      typeof(Ring),
                                      new FrameworkPropertyMetadata ( new Point(0,0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public Ring ()
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
      var ao = d as Ring ;
      ao.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var offset = (Vector)Center ;

      //
      // To draw a circle using the path mini-language it appears neccessary to draw it using two arcs.
      // Use FillRule F0 = EvenOdd to make a hole.
      //
      // By the way, you can use a CombinedGeomerty to achieve a similar effect.
      //
      var p1 = new Point ( 0, -OuterRadius ) + offset ;
      var p2 = new Point ( 0, +OuterRadius ) + offset ;
      var p3 = new Point ( 0, -InnerRadius ) + offset ;
      var p4 = new Point ( 0, +InnerRadius ) + offset ;

      var sb = new StringBuilder() ;

      sb.Append ( "F0 " ) ;
      sb.AppendFormat ( "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
      sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", OuterRadius, Math.PI, p2.X, p2.Y ) ;
      sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", OuterRadius, Math.PI, p1.X, p1.Y ) ;
      sb.AppendFormat ( "M {0:F3},{1:F3} ", p3.X, p3.Y ) ;
      sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", InnerRadius, Math.PI, p4.X, p4.Y ) ;
      sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", InnerRadius, Math.PI, p3.X, p3.Y ) ;
      sb.Append ( "Z " ) ;

      _path = sb.ToString() ;

      Debug.WriteLine ( _path ) ;
    }

  }
}
