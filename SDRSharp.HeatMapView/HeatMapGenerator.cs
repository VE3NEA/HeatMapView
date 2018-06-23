using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;


namespace SDRSharp.HeatMapView
{
  class HeatMapGenerator
  {
    [DllImport("HeatMapGenerator.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    private static extern bool GenerateHeatMap(string InFileName, string OutFileName, int NoiseModel, StringBuilder Info, int InfoSize, bool PngFormat);

    public static HeatMapInfo Generate(string InputFileName)
    {
      Directory.CreateDirectory(Settings.DataFolder());
      string HeatMapName = Settings.EnsureUniqueName(Path.GetFileNameWithoutExtension(InputFileName));
      string OutputFileName = Settings.BuildFilePath(HeatMapName);

      StringBuilder Builder = new StringBuilder(256);

      Cursor.Current = Cursors.WaitCursor;
      try
      {
        if (GenerateHeatMap(InputFileName, OutputFileName, 1, Builder, 256, Settings.IMAGE_EXT == ".png"))
          return (new JavaScriptSerializer()).Deserialize<HeatMapInfo>(Builder.ToString());
        else
          throw new Exception(Builder.ToString());
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }
  }
}
