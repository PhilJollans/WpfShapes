using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfShapes
{
  /// <summary>
  /// Oval is the shape of a running track, with two straight sides and semicircles at each end.
  /// </summary>
  public class Oval : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty TopProperty =
        DependencyProperty.Register ( "Top",
                                      typeof(double),
                                      typeof(Oval),
                                      new FrameworkPropertyMetadata ( 10.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty BottomProperty =
        DependencyProperty.Register ( "Bottom",
                                      typeof(double),
                                      typeof(Oval),
                                      new FrameworkPropertyMetadata ( 30.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty LeftProperty =
        DependencyProperty.Register ( "Left",
                                      typeof(double),
                                      typeof(Oval),
                                      new FrameworkPropertyMetadata ( 10.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty RightProperty =
        DependencyProperty.Register ( "Right",
                                      typeof(double),
                                      typeof(Oval),
                                      new FrameworkPropertyMetadata ( 90.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public Oval ()
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
    public double Top
    {
      get { return Convert.ToDouble(GetValue(TopProperty)); }
      set { SetValue(TopProperty, value); }
    }

    public double Bottom
    {
      get { return Convert.ToDouble(GetValue(BottomProperty)); }
      set { SetValue(BottomProperty, value); }
    }

    public double Left
    {
      get { return Convert.ToDouble(GetValue(LeftProperty)); }
      set { SetValue(LeftProperty, value); }
    }

    public double Right
    {
      get { return Convert.ToDouble(GetValue(RightProperty)); }
      set { SetValue(RightProperty, value); }
    }


    //-------------------------------------------------------------------------
    // Property changed callbacks
    //-------------------------------------------------------------------------
    public static void OnShapeChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      var o = d as Oval ;
      o.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      // Use local variables, because sorting the actual properties leads to unexpected effects.
      double internalLeft   = Left ;
      double internalRight  = Right ;
      double internalTop    = Top ;
      double internalBottom = Bottom ;

      // Strt by ensuring that Left < Right and Top < Bottom
      if ( Left > Right )
      {
        internalLeft   = Right ;
        internalRight  = Left ;
      }

      if ( Top > Bottom )
      {
        internalTop    = Bottom ;
        internalBottom = Top ;
      }

      var Width  = internalRight - internalLeft ;
      var Height = internalBottom - internalTop ;

      if ( Width < Height )
      {
        // Vertical format
        var Radius = Width / 2 ;

        var p1 = new Point ( internalLeft,  internalTop + Radius ) ;
        var p2 = new Point ( internalRight, internalTop + Radius ) ;
        var p3 = new Point ( internalRight, internalBottom - Radius ) ;
        var p4 = new Point ( internalLeft, internalBottom - Radius ) ;

        var sb = new StringBuilder() ;

        sb.AppendFormat ( "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
        sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", Radius, Math.PI, 1, p2.X, p2.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
        sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", Radius, Math.PI, 1, p4.X, p4.Y ) ;
        sb.Append ( "Z " ) ;

        _path = sb.ToString() ;
      }
      else
      {
        // Horizontal format
        var Radius = Height / 2 ;

        var p1 = new Point ( internalRight - Radius,  internalTop ) ;
        var p2 = new Point ( internalRight - Radius, internalBottom ) ;
        var p3 = new Point ( internalLeft + Radius, internalBottom ) ;
        var p4 = new Point ( internalLeft + Radius, internalTop) ;

        var sb = new StringBuilder() ;

        sb.AppendFormat ( "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
        sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", Radius, Math.PI, 1, p2.X, p2.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
        sb.AppendFormat ( "A {0:F3},{0:F3} {1:F3} 0 {2} {3:F3},{4:F3} ", Radius, Math.PI, 1, p4.X, p4.Y ) ;
        sb.Append ( "Z " ) ;

        _path = sb.ToString() ;
      }

      Debug.WriteLine ( _path ) ;
    }

  }
}
