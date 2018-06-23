using System;
using System.Windows.Forms;

namespace SDRSharp.HeatMapView
{
  public partial class SettingsPanel : UserControl
  {
    private HeatMapViewPlugin Plugin;

    public SettingsPanel(HeatMapViewPlugin plugin)
    {
      InitializeComponent();
      Plugin = plugin;

      ListBox.DataSource = Plugin.Settings.HeatMaps;
      ListBox.SelectedIndex = ListBox.FindString(Plugin.Settings.SelectedHeatMap);
      if (ListBox.SelectedIndex == -1) ListBox.SelectedIndex = Plugin.Settings.HeatMaps.Count - 1;

      EnabledCheckBox.Checked = Plugin.Settings.Enabled;
    }

    private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (EnabledCheckBox.Checked)
        try
        {
          Plugin.HeatMapPanel.LoadHeatMap((HeatMapInfo)ListBox.SelectedItem);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          EnabledCheckBox.Checked = false;
          return;
        }

      Plugin.HeatMapPanel.Visible = EnabledCheckBox.Checked;
      Plugin.Settings.Enabled = EnabledCheckBox.Checked;
    }

    private void PlusButton_Click(object sender, EventArgs e)
    {
      if (OpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      try
      {
        HeatMapInfo Info = HeatMapGenerator.Generate(OpenFileDialog.FileName);

        Plugin.Settings.HeatMaps.Add(Info);
        ListBox.SelectedIndex = ListBox.Items.Count-1;

        Plugin.HeatMapPanel.LoadHeatMap(Info);
        EnabledCheckBox.Enabled = true;
        EnabledCheckBox.Checked = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Unable to generate heat map. Error: " + ex.Message);
      }
    }

    private void MinusButton_Click(object sender, EventArgs e)
    {
      //item to delete
      int Idx = ListBox.SelectedIndex;
      if (Idx < 0) return;

      try
      {
        //is the item being viewed?
        if (Plugin.Settings.HeatMaps[Idx].Name == Plugin.Settings.SelectedHeatMap)
        {
          EnabledCheckBox.Checked = false;
          Plugin.HeatMapPanel.ReleaseImage();
        }

        //delete file
        System.IO.File.Delete(Settings.BuildFilePath(Plugin.Settings.HeatMaps[Idx].Name));

        //delete from list
        Plugin.Settings.HeatMaps.RemoveAt(Idx);
        ListBox.SelectedIndex = Math.Min(ListBox.Items.Count - 1, ListBox.SelectedIndex);
      }
      catch (Exception ex)
      {
        MessageBox.Show(String.Format("Unable to delete heat map, error: {0}", ex.Message));
      }
    }

    //edit button: [✔]
    private void EditButton_Click(object sender, EventArgs e)
    {
      //item to rename
      int Idx = ListBox.SelectedIndex;
      if (Idx < 0) return;

      //new name
      string OldName = Plugin.Settings.HeatMaps[Idx].Name;
      string NewName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name:", "Rename Heat Map", OldName, -1, -1);
      if (NewName == "") return;

      try
      {
        //rename
        NewName = Settings.EnsureUniqueName(NewName);

        //update heatmap name in HeatMapPanel
        if (Plugin.HeatMapPanel.Info.Name == OldName)
        {
          Plugin.HeatMapPanel.ReleaseImage();
          try
          {
            System.IO.File.Move(Settings.BuildFilePath(OldName), Settings.BuildFilePath(NewName));
            Plugin.HeatMapPanel.Info.Name = NewName;
          }
          finally
          {
            Plugin.HeatMapPanel.LoadImage();
          }
        }
        else
        {
          System.IO.File.Move(Settings.BuildFilePath(OldName), Settings.BuildFilePath(NewName));
          Plugin.Settings.HeatMaps[Idx].Name = NewName;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(String.Format("Unable to rename heat map, error: {0}", ex.Message));
      }

      //reflect changed name in the listbox
      ((CurrencyManager)ListBox.BindingContext[Plugin.Settings.HeatMaps]).Refresh();
    }

    private void ListBox_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        Plugin.HeatMapPanel.LoadHeatMap((HeatMapInfo)ListBox.SelectedItem);
        EnabledCheckBox.Checked = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(String.Format("Unable to load heat map, error: {0}", ex.Message));
      }
    }

    private void HelpButton_Click(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start("http://dxatlas.com/HeatMapView/");
    }
  }
}
