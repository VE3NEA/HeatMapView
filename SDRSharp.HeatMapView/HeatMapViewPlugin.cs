using System.Windows.Forms;
using SDRSharp.Common;
using System.IO;
using SDRSharp.HeatMapView.Properties;
using System.Web.Script.Serialization;


namespace SDRSharp.HeatMapView
{
  public class HeatMapViewPlugin : ISharpPlugin
  {
    private const string ExampleInfoStr =
      "{\"StartFreq\":24000000,\"EndFreq\":1200000000,\"StartTime\":\"\\/Date(1529427222000)" + 
      "\\/\",\"EndTime\":\"\\/Date(1529430803000)\\/\",\"Name\":\"Example\"}";

    internal Settings Settings;
    internal ISharpControl Control;
    internal SettingsPanel SettingsPanel;
    internal HeatMapPanel HeatMapPanel;



    //-----------------------------------------------------------------------------------------
    //                                 ISharpPlugin 
    //-----------------------------------------------------------------------------------------
    public UserControl Gui
    {
      get { return SettingsPanel; }
    }

    public string DisplayName
    {
      get { return "HeatMap View"; }
    }

    public void Close()
    {
      Settings.Save();
    }

    public void Initialize(ISharpControl control)
    {
      Control = control;

      Directory.CreateDirectory(Settings.DataFolder());

      Settings = Settings.Load();

      //put example if no heat maps found
      if (Settings.HeatMaps.Count == 0)
      {
        Resources.Example.Save(Settings.BuildFilePath("Example"));
        Settings.HeatMaps.Add((new JavaScriptSerializer()).Deserialize<HeatMapInfo>(ExampleInfoStr));
      }

      HeatMapPanel = new HeatMapPanel(this);
      Control.RegisterFrontControl(HeatMapPanel, PluginPosition.Top);
      HeatMapPanel.Visible = false;

      SettingsPanel = new SettingsPanel(this);
    }
  }
}
