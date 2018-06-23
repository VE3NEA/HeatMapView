# HeatMap View plugin for SDRSharp

The plugin allows one to generate **heatmaps** from the data collected
using the [rtl_power](http://kmkeen.com/rtl-power/) software and 
[rtl-sdr](https://www.rtl-sdr.com/about-rtl-sdr/) receiver, and browse them in 
[SDR#](https://airspy.com/download/).

Heatmaps is a popular way of finding signals of interest in the large segments
of frequencies. With this plugin, you can browse the heatmap and instantly tune
to the frequencies where interesting activity was recorded.

### Installation
1. Place these two files in the folder where SDR# is installed:
  * [HeatMapGenerator.dll](HeatMapGenerator.Delphi/HeatMapGenerator.dll)
  * [SDRSharp.HeatMapView.dll](SDRSharp.HeatMapView/bin/Release/SDRSharp.HeatMapView.dll)
  
2. Open the *Plugins.xml* file in the SDR# folder using a text editor and
add this line just before the `</sharpPlugins>` line:
`<add key="HeatMap View" value="SDRSharp.HeatMapView.HeatMapViewPlugin,SDRSharp.HeatMapView" />`

3. Start SDR# and verify that the HeatMap View plugin appears in the sidebar.

### Browsing a Heatmap
Tick the Enabled check box in the plugin settings panel, or double-click
on a heatmap name in the list of heat maps. A new panel with a heatmap 
will appear above the band scope. Use these commands to browse the heatmap:
  * zoom in and out using the mouse wheel;
  * pan the heatmap by dragging the mouse;
  * tune the radio by clicking on a signal trace.
  
### Generating a Heatmap
The plugin comes with a sample heatmap that you can start exploring right away,
but eventually you will want to make your own heatmaps, as described below. 

1. Run the **rtl_power** program to collect some spectra.
2. Click on the Plus button [+], select a CSV file produced by **rtl_power**, 
and click on OK.

The following command line is recommended for running **rtl_power**:

`rtl_power.exe -f 24M:1700M:50k -i 1s -c 0.25 -g 50 -e 1h "Example.csv"`

Multiple heatmaps may be generated, with different frequency and time coverage.

Use the Minus button [-] to delete the selected heatmap, and Edit button [✔]
to rename it.

### Source Code
The source code of the plugin is available on the terms of the 
[MIT license](https://opensource.org/licenses/MIT).

For those who are just starting with SDR# plugin development, I have included
step by step [instructions](SDRPluginSteps.htm) 
how to create a blank plugin project in Visual Studio.


  
