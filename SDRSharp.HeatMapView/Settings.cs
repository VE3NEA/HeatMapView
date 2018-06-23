using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace SDRSharp.HeatMapView
{
  public class HeatMapInfo
  {
    //Name needs to be a property to appear in Listbox
    public string Name { get; set; }
    public long StartFreq, EndFreq;
    public DateTime StartTime, EndTime;
  }

  internal class Settings
  {
    private const string DEFAULT_FILENAME = "settings.json";

    //either bmp or png format may be used for the heatmaps.
    //bmp heatmaps are twice as large as png, 
    //but they are generated and loaded faster

    public const string IMAGE_EXT = ".bmp"; //".png";

    public bool Enabled = false;
    public string SelectedHeatMap;
    public BindingList<HeatMapInfo> HeatMaps; 

    internal void Save(string fileName = DEFAULT_FILENAME)
    {
      System.IO.Directory.CreateDirectory(Settings.DataFolder());
      fileName = Settings.DataFolder() + fileName;
      File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
    }

    internal static Settings Load(string fileName = DEFAULT_FILENAME)
    {
      Settings settings = new Settings();
      settings.HeatMaps = new BindingList<HeatMapInfo>();

      fileName = Settings.DataFolder() + fileName;
      if (File.Exists(fileName))
        settings = (new JavaScriptSerializer()).Deserialize<Settings>(File.ReadAllText(fileName));

      settings.VerifyHeatMaps();

      return settings;
    }

    internal static string DataFolder()
    {
      return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Afreet\Products\HeatMapView\";
    }

    internal static string BuildFilePath(string HeatMapName)
    {
      return DataFolder() + HeatMapName + IMAGE_EXT;
    }

    internal static string EnsureUniqueName(string Name)
    {
      //remove invalid chars
      Name = String.Concat(Name.Split(Path.GetInvalidFileNameChars()));

      //name is unique
      if (!File.Exists(BuildFilePath(Name))) return Name;

      //make the name unique 
      for (int i = 1; i < 100; ++i)
      {
        string NewName = String.Format("{0} ({1})", Name, i).Trim();
        if (!File.Exists(BuildFilePath(NewName))) return NewName;
      }

      throw new Exception(String.Format("Invalid heat map name: '{0}'" + Name));
    }

    private void VerifyHeatMaps()
    {
      for (int i = HeatMaps.Count - 1; i >= 0; --i)
        if (!File.Exists(BuildFilePath(HeatMaps[i].Name))) HeatMaps.RemoveAt(i);
    }
  }
}
