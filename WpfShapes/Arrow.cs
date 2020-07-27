using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace WpfShapes
{
  public class Arrow : Shape
  {
    private string _path = null ;

    public static readonly DependencyProperty StartProperty =
        DependencyProperty.Register ( "Start",
                                      typeof(Point),
                                      typeof(Arrow),
                                      new FrameworkPropertyMetadata ( new Point(0,100),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty EndProperty =
        DependencyProperty.Register ( "End",
                                      typeof(Point),
                                      typeof(Arrow),
                                      new FrameworkPropertyMetadata ( new Point(100,100),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty ShaftWidthProperty =
        DependencyProperty.Register ( "ShaftWidth",
                                      typeof(double),
                                      typeof(Arrow),
                                      new FrameworkPropertyMetadata ( 20.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty ArrowLengthRatioProperty =
        DependencyProperty.Register ( "ArrowLengthRatio",
                                      typeof(double),
                                      typeof(Arrow),
                                      new FrameworkPropertyMetadata ( 0.25,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public static readonly DependencyProperty ArrowWidthRatioProperty =
        DependencyProperty.Register ( "ArrowWidthRatio",
                                      typeof(double),
                                      typeof(Arrow),
                                      new FrameworkPropertyMetadata ( 3.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged ) ) ;

    public Arrow ()
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
    public Point Start
    {
      get { return (Point)GetValue(StartProperty); }
      set { SetValue(StartProperty, value); }
    }

    public Point End
    {
      get { return (Point)GetValue(EndProperty); }
      set { SetValue(EndProperty, value); }
    }

    public double ShaftWidth
    {
      get { return (double)GetValue(ShaftWidthProperty); }
      set { SetValue(ShaftWidthProperty, value); }
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

    //-------------------------------------------------------------------------
    // Property changed callbacks
    //-------------------------------------------------------------------------
    public static void OnShapeChanged ( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      var aa = d as Arrow ;
      aa.InitializeGeometry() ;
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      double len_x  = End.X - Start.X ;
      double len_y  = End.Y - Start.Y ;
      double length = Math.Sqrt ( len_x*len_x + len_y*len_y ) ;

      // Don't go dividing by zero
      if ( length > 0.0 )
      {
        // In this shape we measure the angle CLOCKWISE from the 'X' axis and
        // define sine and cosine correspondingly. An arrow starting at (0,0) and
        // pointing to the right would have the angle 0.
        double s1 = len_y / length ;
        double c1 = len_x / length ;

        double halfWidth   = ShaftWidth / 2.0 ;
        double shaftLength = length * ( 1 - ArrowLengthRatio ) ;
        double headWidth   = ShaftWidth * ArrowWidthRatio ;
        double headStep    = ( headWidth - ShaftWidth ) / 2.0 ;

        var p1 = new Point ( Start.X + halfWidth   * s1 , Start.Y - halfWidth   * c1 ) ;
        var p2 = new Point ( p1.X    + shaftLength * c1 , p1.Y    + shaftLength * s1 ) ;
        var p3 = new Point ( p2.X    + headStep    * s1 , p2.Y    - headStep    * c1 ) ;
        var p4 = End ;
        var p5 = new Point ( p3.X    - headWidth   * s1 , p3.Y    + headWidth   * c1 ) ;
        var p6 = new Point ( p2.X    - ShaftWidth  * s1 , p2.Y    + ShaftWidth  * c1 ) ;
        var p7 = new Point ( p1.X    - ShaftWidth  * s1 , p1.Y    + ShaftWidth  * c1 ) ;

        var sb = new StringBuilder() ;

        sb.AppendFormat ( "M {0:F3},{1:F3} ", p1.X, p1.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p2.X, p2.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p3.X, p3.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p4.X, p4.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p5.X, p5.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p6.X, p6.Y ) ;
        sb.AppendFormat ( "L {0:F3},{1:F3} ", p7.X, p7.Y ) ;
        sb.Append ( "Z " ) ;

        _path = sb.ToString() ;

        Debug.WriteLine ( _path ) ;
      }
    }

  }
}
