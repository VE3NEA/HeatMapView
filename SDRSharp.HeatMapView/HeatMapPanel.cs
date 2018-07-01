using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SDRSharp.HeatMapView
{
  public partial class HeatMapPanel : UserControl
  {
    private const int ScaleHeight = 25;
    private static readonly float[] TickMults = {2f, 2.5f, 2f};

    private HeatMapViewPlugin Plugin;
    internal HeatMapInfo Info;
    private Image HeatMap;
    private Point MouseDownPos, MouseMovePos;
    private bool MouseDragging = false;
    private float HeatMapHertzPerPixel;
    private double TimeTicksPerPixel;
    private Rectangle VisibleRectangle;
    private long PanFreq;
    private long PanTicks;
    private float Zoom;


    //-----------------------------------------------------------------------------------------
    //                                     Init
    //-----------------------------------------------------------------------------------------
    public HeatMapPanel(HeatMapViewPlugin plugin)
    {
      InitializeComponent();
      Plugin = plugin;
      Plugin.Control.PropertyChanged += PropertyChangedEventHandler;
      VisibleRectangle = GetVisibleRectangle();
    }
       
    internal void LoadHeatMap(HeatMapInfo NewInfo)
    {
      try
      {
        if (NewInfo == null)
        {
          ReleaseImage();
          return;
        }

        //already loaded
        if (HeatMap != null && NewInfo.Name == Info.Name) return;

        Info = NewInfo;
        LoadImage();

        PanFreq = 0;
        PanTicks = 0;
        Zoom = 1;
        HeatMapHertzPerPixel = (Info.EndFreq - Info.StartFreq) / (float)HeatMap.Width;
        TimeTicksPerPixel = (Info.EndTime - Info.StartTime).Ticks / (float)HeatMap.Height;

        if (Double.IsNaN(TimeTicksPerPixel) || Double.IsInfinity(TimeTicksPerPixel) || TimeTicksPerPixel <= 0 ||
            Single.IsNaN(HeatMapHertzPerPixel) || Single.IsInfinity(HeatMapHertzPerPixel) || HeatMapHertzPerPixel <= 0)
          throw new Exception("Invalid dimensions");

        Plugin.Settings.SelectedHeatMap = Info.Name;

        Invalidate();
      }
      catch (Exception ex)
      {
        ReleaseImage();
        throw new Exception(String.Format("Unable to load heat map '{0}', error: {1}", NewInfo.Name, ex.Message));
      }
    }
       
    internal void ReleaseImage()
    {
      if (HeatMap != null) HeatMap.Dispose();
      HeatMap = null;
    }

    internal void LoadImage()
    {
      Cursor.Current = Cursors.WaitCursor;
      try
      {
        ReleaseImage();
        HeatMap = Image.FromFile(Settings.BuildFilePath(Info.Name), true);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }


    //-----------------------------------------------------------------------------------------
    //                                     Paint
    //-----------------------------------------------------------------------------------------
    private void HitMapViewFrontControl_Paint(object sender, PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      DrawBackground(g);

      if (HeatMap != null)
      {
        DrawScale(g);
        DrawTriangle(g);
        if (e.ClipRectangle.Bottom > GetScaleRectangle().Bottom) DrawHeatMap(g);
      }
    }

    private void DrawBackground(Graphics g)
    {
      g.FillRectangle(Brushes.LightSlateGray, GetVisibleRectangle());
      g.FillRectangle(Brushes.White, GetScaleRectangle());
    }

    private void DrawScale(Graphics g)
    {
      //tick steps
      float TickStep = 200;
      float LabelStep = 1000;
      for (int i = 0; i <= 24; ++i)
      {
        if (LabelStep / DisplayHertzPerPixel() > 60) break;
        LabelStep *= TickMults[i % 3];
        TickStep *= TickMults[(i + 1) % 3];
      }

      //label format
      string LabelFormat;
      if (LabelStep % 1000000 == 0) LabelFormat = "F0";
      else if (LabelStep % 100000 == 0) LabelFormat = "F1";
      else if (LabelStep % 10000 == 0) LabelFormat = "F2";
      else LabelFormat = "F3";

      //draw lagre ticks and labels
      double Freq = Convert.ToInt64(Math.Truncate((Info.StartFreq + CurrentPanFreq()) / LabelStep) * LabelStep);
      while (true)
      {
        int x = VisibleRectangle.Left + Convert.ToInt32((Freq - Info.StartFreq - CurrentPanFreq()) / DisplayHertzPerPixel());
        if (x > Width) break;
        g.FillRectangle(Brushes.Black, x, 10, 1, ScaleHeight - 10);
        g.DrawString((Freq * 1e-6).ToString(LabelFormat), Font, Brushes.Black, new Point(x + 2, 0));
        Freq += LabelStep;
      }

      //draw small ticks
      Freq = Convert.ToInt64(Math.Truncate((Info.StartFreq + CurrentPanFreq()) / LabelStep) * LabelStep);
      while (true)
      {
        int x = VisibleRectangle.Left + Convert.ToInt32((Freq - Info.StartFreq - CurrentPanFreq()) / DisplayHertzPerPixel());
        if (x > Width) break;
        g.FillRectangle(Brushes.Black, x, 18, 1, ScaleHeight - 18);
        Freq += TickStep;
      }
    }

    private void DrawHeatMap(Graphics g)
    {
      Rectangle Rd = VisibleRectangle;
      Rd.Location = new Point(Rd.Left, ScaleHeight);
      Rd.Height -= ScaleHeight;
      g.FillRectangle(Brushes.LightSlateGray, Rd);

      Rectangle Rs = Rd;
      Rs.Location = new Point(
          Convert.ToInt32(CurrentPanFreq() / HeatMapHertzPerPixel),
          Convert.ToInt32(CurrentPanTicks() / TimeTicksPerPixel));
      Rs.Width = Convert.ToInt32(Rs.Width / Zoom);

      g.DrawImage(HeatMap, Rd, Rs, GraphicsUnit.Pixel);

    }

    private void DrawTriangle(Graphics g)
    {
      long Offset = Plugin.Control.Frequency - Info.StartFreq - CurrentPanFreq();
      int x = VisibleRectangle.X + Convert.ToInt32(Offset / DisplayHertzPerPixel());
      int y = ScaleHeight - 1;
      Point p = new Point(x, y);
      if (!VisibleRectangle.Contains(p)) return;

      Point[] Points = {new Point(p.X - 6, p.Y - 6), p, new Point(p.X + 6, p.Y - 6)};
      g.FillPolygon(Brushes.Lime, Points);
    }

    private void ShowLabel()
    {
      Point MousePos = PointToClient(Cursor.Position);
      double DisplayFreq = FreqAtMouse();
      DateTime DisplayTime = TimeAtMouse();

      label1.Visible =
        !MouseDragging && 
        VisibleRectangle.Contains(MousePos) && 
        MousePos.Y > ScaleHeight &&
        DisplayFreq >= Info.StartFreq && 
        DisplayFreq < Info.EndFreq &&
        DisplayTime >= Info.StartTime &&
        DisplayTime < Info.EndTime;

      if (label1.Visible) label1.Text = String.Format(" {0:f3} MHz  {1:s}", 
        DisplayFreq * 1e-6, DisplayTime).Replace("T", "  ");
    }

    private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Frequency") Invalidate(GetScaleRectangle());
    }

    private void HeatMapPanel_ClientSizeChanged(object sender, EventArgs e)
    {
      VisibleRectangle = GetVisibleRectangle();

      //increase zoom if heatmap occupies less than half of the panel
      if (HeatMap != null)
      {
        while (HeatMap.Width * Zoom < 0.5 * VisibleRectangle.Width) Zoom *= 2;
        ValidatePanFreq(ref PanFreq);
        ValidatePanTime(ref PanTicks);
        Invalidate();
      }
    }


    //-----------------------------------------------------------------------------------------
    //                                     Mouse
    //-----------------------------------------------------------------------------------------
    private void HeatMapPanel_MouseDown(object sender, MouseEventArgs e)
    {
      if (FreqAtMouse() > Info.EndFreq) return;
      if (TimeAtMouse() > Info.EndTime) return;
      if (e.Button == MouseButtons.Left) MouseDownPos = e.Location;
    }

    private void HeatMapPanel_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Location.Equals(MouseMovePos)) return;

      MouseMovePos = e.Location;
      if (!MouseDragging && e.Button == MouseButtons.Left)
      {
        MouseDragging = true;
        Cursor = Cursors.NoMove2D;
        label1.Visible = false;
      }

      if (MouseDragging)
        Invalidate();
      else
        ShowLabel();
    }

    private void HeatMapPanel_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left) return;

      if (MouseDragging)
      {
        PanFreq = CurrentPanFreq();
        PanTicks = CurrentPanTicks();
        MouseDragging = false;
        Cursor = Cursors.Cross;
        ShowLabel();
      }
      else
        Plugin.Control.Frequency = FreqAtMouse();
    }

    private void HeatMapPanel_MouseLeave(object sender, EventArgs e)
    {
      ShowLabel();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      if (MouseDragging) return;

      long MouseFreq = FreqAtMouse();
      if (MouseFreq > Info.EndFreq) return;

      if (e.Delta > 0)
      {
        if (Zoom == 4) return;
        Zoom *= 2;
      }
      else
      {
        if (HeatMap.Width * Zoom < VisibleRectangle.Width) return;
        Zoom /= 2;
      }

      PanFreq = Convert.ToInt64(MouseFreq - Info.StartFreq - e.X * DisplayHertzPerPixel());
      ValidatePanFreq(ref PanFreq);
      Invalidate();
    }
    

    //-----------------------------------------------------------------------------------------
    //                                  Helper func   
    //-----------------------------------------------------------------------------------------
    private Rectangle GetVisibleRectangle()
    {
      //3 leftmost pixels are not visible for unknown reason.
      Rectangle Result = ClientRectangle;
      Result.Offset(3, 0);
      Result.Width -= 3;
      return Result;
    }

    protected long FreqAtMouse()
    {
      long Freq = Info.StartFreq + CurrentPanFreq() + Convert.ToInt64((MouseMovePos.X - VisibleRectangle.Left) * DisplayHertzPerPixel());
      return (long)(Math.Truncate(Freq / HeatMapHertzPerPixel) * HeatMapHertzPerPixel);
    }

    protected DateTime TimeAtMouse()
    {
      return Info.StartTime + new TimeSpan(CurrentPanTicks() + Convert.ToInt64((MouseMovePos.Y - ScaleHeight) * TimeTicksPerPixel));
    }

    protected void ValidatePanFreq(ref long PanFreq)
    {
      long WindowWidthHz = Convert.ToInt64(VisibleRectangle.Width * DisplayHertzPerPixel());
      PanFreq = Math.Max(0, Math.Min(Info.EndFreq - Info.StartFreq - WindowWidthHz, PanFreq));
      PanFreq = (long)(Math.Truncate(PanFreq / HeatMapHertzPerPixel) * HeatMapHertzPerPixel);
    }

    protected void ValidatePanTime(ref long PanTicks)
    {
      long WindowHeightTicks = Convert.ToInt64(VisibleRectangle.Height * TimeTicksPerPixel);
      long HeatMapHeightTicks = (Info.EndTime - Info.StartTime).Ticks;
      PanTicks = Math.Max(0, Math.Min(HeatMapHeightTicks - WindowHeightTicks, PanTicks));
    }

    protected float DisplayHertzPerPixel()
    {
      return HeatMapHertzPerPixel / Zoom;
    }

    protected long CurrentPanFreq()
    {
      long Freq = PanFreq;

      if (MouseDragging)
      {
        Freq -= Convert.ToInt64(DisplayHertzPerPixel() * (MouseMovePos.X - MouseDownPos.X));
        ValidatePanFreq(ref Freq);
      }

      return Freq;
    }

    protected long CurrentPanTicks()
    {
      long Ticks = PanTicks;
      if (MouseDragging)
      {
        Ticks -= Convert.ToInt64(TimeTicksPerPixel * (MouseMovePos.Y - MouseDownPos.Y));
        ValidatePanTime(ref Ticks);
      }
      return Ticks;
    }

    protected Rectangle GetScaleRectangle()
    {
      Rectangle r = VisibleRectangle;
      r.Height = ScaleHeight;
      return r;
    }

  }
}