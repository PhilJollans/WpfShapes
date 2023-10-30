using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfShapes
{
  public class SpokedWheel : Shape
  {
    private string _path = null;

    public static readonly DependencyProperty RimInnerRadiusProperty =
        DependencyProperty.Register("RimInnerRadius",
                                      typeof(double),
                                      typeof(SpokedWheel),
                                      new FrameworkPropertyMetadata(20.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged));

    public static readonly DependencyProperty RimRadiusProperty =
        DependencyProperty.Register("RimRadius",
                                      typeof(double),
                                      typeof(SpokedWheel),
                                      new FrameworkPropertyMetadata(80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged));

    public static readonly DependencyProperty HubRadiusProperty =
        DependencyProperty.Register("HubRadius",
                                      typeof(double),
                                      typeof(SpokedWheel),
                                      new FrameworkPropertyMetadata(80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged));

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register("Center",
                                      typeof(Point),
                                      typeof(SpokedWheel),
                                      new FrameworkPropertyMetadata(new Point(0, 0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged));

    public static readonly DependencyProperty SpokeThicknessProperty =
        DependencyProperty.Register("SpokeThickness",
                                      typeof(double),
                                      typeof(SpokedWheel),
                                      new FrameworkPropertyMetadata(80.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged));

    public static readonly DependencyProperty NumberOfSpokesProperty =
        DependencyProperty.Register("NumberOfSpokes",
                                      typeof(int),
                                      typeof(SpokedWheel),
                                      new FrameworkPropertyMetadata(3,
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnShapeChanged));

    public SpokedWheel()
    {
      // Initialise the geometry with the default parameters.
      InitializeGeometry();
    }

    protected override Geometry DefiningGeometry
    {
      get
      {
        return Geometry.Parse(_path);
      }
    }

    //-------------------------------------------------------------------------
    // Getters and setters for the dependency properties
    //-------------------------------------------------------------------------
    public double RimInnerRadius
    {
      get { return Convert.ToDouble(GetValue(RimInnerRadiusProperty)); }
      set { SetValue(RimInnerRadiusProperty, value); }
    }

    public double RimRadius
    {
      get { return Convert.ToDouble(GetValue(RimRadiusProperty)); }
      set { SetValue(RimRadiusProperty, value); }
    }

    public double HubRadius
    {
      get { return Convert.ToDouble(GetValue(HubRadiusProperty)); }
      set { SetValue(HubRadiusProperty, value); }
    }

    public double SpokeThickness
    {
      get { return Convert.ToDouble(GetValue(SpokeThicknessProperty)); }
      set { SetValue(SpokeThicknessProperty, value); }
    }

    public int NumberOfSpokes
    {
      get { return Convert.ToInt32(GetValue(NumberOfSpokesProperty)); }
      set { SetValue(NumberOfSpokesProperty, value); }
    }

    public Point Center
    {
      get { return (Point)GetValue(CenterProperty); }
      set { SetValue(CenterProperty, value); }
    }

    //-------------------------------------------------------------------------
    // Property changed callbacks
    //-------------------------------------------------------------------------
    public static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var ao = d as SpokedWheel;
      ao.InitializeGeometry();
    }

    //-------------------------------------------------------------------------
    // Member functions
    //-------------------------------------------------------------------------
    private void InitializeGeometry()
    {
      var offset = (Vector)Center;

      // All angle in radians
      // Determine the angle subtended by half a spoke.
      double halfSpokeAngleAtHub = Math.Atan2 ( SpokeThickness / 2.0, HubRadius ) ;
      double halfSpokeAngleAtRim = Math.Atan2 ( SpokeThickness / 2.0, RimInnerRadius ) ;
      double spokeSeparation = 2 * Math.PI / NumberOfSpokes;
      double spokeSeparationAtHub = spokeSeparation - ( 2.0 * halfSpokeAngleAtHub ) ;
      double spokeSeparationAtRim = spokeSeparation - ( 2.0 * halfSpokeAngleAtRim) ;

      // If there is no gap between the spokes at the hub, adjacent spokes will come
      // together at a single point, which may be further away from the centre.
      double ApexRadius = 0.0 ;
      if ( spokeSeparationAtHub <= 0 )
      {
        ApexRadius = ( SpokeThickness / 2.0 ) / Math.Sin ( spokeSeparation / 2.0 ) ;
      }

      var sb = new StringBuilder();

      // Use FillRule F0 = EvenOdd to make a holes
      sb.Append("F0 ");

      // To draw a circle using the path mini-language it appears neccessary to draw it using two arcs.
      // Draw the outer circle
      var p1 = new Point(0, -RimRadius) + offset;
      var p2 = new Point(0, +RimRadius) + offset;
      sb.AppendFormat(CultureInfo.InvariantCulture, "M {0:F3},{1:F3} ", p1.X, p1.Y);
      sb.AppendFormat(CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", RimRadius, Math.PI, p2.X, p2.Y);
      sb.AppendFormat(CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 1 1 {2:F3},{3:F3} ", RimRadius, Math.PI, p1.X, p1.Y);

      // We won't actually draw the spokes. Instead will draw the holes inbetween the spokes.
      for (int i = 0; i < NumberOfSpokes; i++)
      {
        // Get the angle of this spoke and the next spoke.
        double a1 = i * spokeSeparation ;
        double a2 = a1 + spokeSeparation ;

        // Handle the two cases separately.
        // I think this is more readable, even if some code is duplicated.
        if ( spokeSeparationAtHub > 0 )
        {
          // The hole has four points

          // Adjust for the spoke width at the rim
          double o1 = a1 + halfSpokeAngleAtRim;
          double o2 = a2 - halfSpokeAngleAtRim;

          // Adjust for the spoke width at the hub
          double i1 = a2 - halfSpokeAngleAtHub;
          double i2 = a1 + halfSpokeAngleAtHub;

          double co1 = Math.Cos(o1);
          double so1 = Math.Sin(o1);
          double co2 = Math.Cos(o2);
          double so2 = Math.Sin(o2);
          double ci1 = Math.Cos(i1);
          double si1 = Math.Sin(i1);
          double ci2 = Math.Cos(i2);
          double si2 = Math.Sin(i2);

          var q1 = new Point(RimInnerRadius * so1, -RimInnerRadius * co1) + offset;
          var q2 = new Point(RimInnerRadius * so2, -RimInnerRadius * co2) + offset;
          var q3 = new Point(HubRadius * si1, -HubRadius * ci1) + offset;
          var q4 = new Point(HubRadius * si2, -HubRadius * ci2) + offset;

          sb.AppendFormat(CultureInfo.InvariantCulture, "M {0:F3},{1:F3} ", q1.X, q1.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 1 {2:F3},{3:F3} ", RimInnerRadius, spokeSeparationAtRim, q2.X, q2.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", q3.X, q3.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 0 {2:F3},{3:F3} ", HubRadius, spokeSeparationAtHub, q4.X, q4.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", q1.X, q1.Y);
        }
        else
        {
          // The hole has three points

          // Adjust for the spoke width at the rim
          double o1 = a1 + halfSpokeAngleAtRim;
          double o2 = a2 - halfSpokeAngleAtRim;

          // Get the mid point angle
          double im = a1 + ( spokeSeparation / 2.0 ) ;

          double co1 = Math.Cos(o1);
          double so1 = Math.Sin(o1);
          double co2 = Math.Cos(o2);
          double so2 = Math.Sin(o2);
          double cim = Math.Cos(im);
          double sim = Math.Sin(im);

          var q1 = new Point(RimInnerRadius * so1, -RimInnerRadius * co1) + offset;
          var q2 = new Point(RimInnerRadius * so2, -RimInnerRadius * co2) + offset;
          var qm = new Point(ApexRadius * sim, -ApexRadius * cim) + offset;

          sb.AppendFormat(CultureInfo.InvariantCulture, "M {0:F3},{1:F3} ", q1.X, q1.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "A {0:F3},{0:F3} {1:F3} 0 1 {2:F3},{3:F3} ", RimInnerRadius, spokeSeparationAtRim, q2.X, q2.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", qm.X, qm.Y);
          sb.AppendFormat(CultureInfo.InvariantCulture, "L {0:F3},{1:F3} ", q1.X, q1.Y);
        }
      }

      sb.Append("Z ");

      _path = sb.ToString();

      Debug.WriteLine(_path);
    }
  }
}
