#See README - this code based on work by various other people
#Test usage:
#execfile("d:\\My Documents\\Resolver\\rslWPFCharts\\ResolverOneCode\\rslWPFChart.py")
#myChart = rslWPFChart("title","yaxis", ["a","b"], ["1/1/2009","1/2/2009"],[[0.1,0.2],[0.3,0.4]])
#myChart.Start()
import sys
import clr
clr.AddReference('System.Drawing')
clr.AddReference('System.Windows.Forms')
from System.Threading import Thread, ThreadStart, ApartmentState
from System.Windows.Forms import Application, Form
from System.Windows.Forms import Button as frmButton
from System.Windows.Forms import (ControlStyles, ContextMenuStrip, DockStyle, FormStartPosition, ImageList, Keys, MenuStrip, MessageBox, MessageBoxButtons, MessageBoxIcon, Panel, TabControl, TabAlignment, TabPage, ToolStrip, ToolStripButton, ToolStripMenuItem, ToolStripItemDisplayStyle, WebBrowser, Padding) 
from System.Drawing import Size
from System import Uri, Exception, DateTime
from System.Reflection import Assembly
from os.path import dirname

#Add assembly for hosting WPF in WinForm and reading in XAML
clr.AddReference("WindowsFormsIntegration")
from System.Windows.Forms.Integration import ElementHost
clr.AddReference("PresentationCore")
clr.AddReference("PresentationFramework")
clr.AddReference("WindowsBase")
from System.Windows.Media import Colors
from System.Windows import Point

#Import WPF chart from assembly
assemblyfile = dirname(__file__)+"\\WpfChart2.exe"
assembly = Assembly.LoadFile(assemblyfile)
clr.AddReference(assembly)
from WpfChart2 import WpfMultiChart
from WpfChart2.TimeSeriesDataLib import TimeSeriesData, TimeSeriesDataPoint

class WPFChartForm(Form):
    """Main Windows Form that will use an ElementHost to run the WPFChart. This form will run in it's own thread.
    See http://www.switchonthecode.com/tutorials/wpf-tutorial-using-wpf-in-winforms for more information"""
    
    def __init__(self):
        Form.__init__(self)
        self.Text = "WPF Chart Example"
        self.Name = "WPF Chart Example"
        self.Width = 640
        self.Height = 480
    
    def initElementHost(self):
        "See http://msdn.microsoft.com/en-us/library/ms788740.aspx for more info."
        self.wpfElementHost = ElementHost()
        self.wpfElementHost.Dock = DockStyle.Fill
        self.Controls.Add(self.wpfElementHost)    
    
    def initWPFChart(self, strTitle, strYLabel):
        self.wpfChart = WpfMultiChart()
        self.wpfChart.SetTitle(strTitle)
        self.wpfChart.SetStaticYLabel(strYLabel)
        self.wpfElementHost.Child = self.wpfChart
    
class rslWPFChart(object):
    "Base class for a WPF based chart for Resolver One"
    frmChart = None #Note: use static variables so that they can be accessed from multiple threads
    newThread = None
    bSingleInstance = True #only allow a single instance of the form to be open. If another instance it opened, it will bring the original to focus and close it.
    strTitle = None
    strYLabel = None
    data = None
    
    def __init__(self, ChartTitle, YAxisLabel, arrNames, arrTime, marrData):
        """Creates a new WPF Chart object and loads the data values."""
        if(self.bSingleInstance):
            for aForm in Application.OpenForms:
                if str(type(aForm)) == str(WPFChartForm):
                    print "Form already open so close previous and open a new one"
                    aForm.Close()
                    break
        rslWPFChart.newThread = Thread(ThreadStart(rslWPFChart.Worker))
        rslWPFChart.newThread.SetApartmentState (ApartmentState.STA);
        rslWPFChart.strTitle = ChartTitle
        rslWPFChart.strYLabel = YAxisLabel
        rslWPFChart.data = (arrNames, arrTime, marrData)
    
    def Start(self):
        "Call this method to start the Chart form"
        #Start seperate thread
        rslWPFChart.newThread.Start() 
        
    @staticmethod    
    def Worker():
        "Used to start the form on a seperate thread so that Resolver One still responds to events"
        try:
            rslWPFChart.frmChart = WPFChartForm()
            rslWPFChart.frmChart.initElementHost()
            rslWPFChart.frmChart.initWPFChart(rslWPFChart.strTitle, rslWPFChart.strYLabel)
            rslWPFChart.AddData(rslWPFChart.data[0],rslWPFChart.data[1],rslWPFChart.data[2])
            rslWPFChart.frmChart.CenterToParent()
            rslWPFChart.frmChart.ShowDialog() 
        except:
            MessageBox.Show("Exception:" + str(sys.exc_value)) #Catch and display any errors. Todo: add Stack frame for line numbers?
        
    @staticmethod 
    def ClearAllSeries():
        """Clears all data from the chart"""
        print rslWPFChart.frmChart
        rslWPFChart.frmChart.wpfChart.ClearAllSeries()

    @staticmethod 
    def AddData(arrNames, arrTime, marrData):
        """Adds a new data series to the chart
            arrNames is an arrar of the series names
            arrTime is an array of the time values (x axis)
            marrData is an multi-dimentional array of the data values
            NOTE: This must be called from the worker thread!"""
        lstTSData = []
        lstColors = [Colors.DarkBlue,Colors.Red,Colors.Green,Colors.Black,Colors.Gray]
        
        dtStart = DateTime.Parse(arrTime[0])
        dtEnd = DateTime.Parse(arrTime[len(arrTime)-1])
        
        rslWPFChart.frmChart.wpfChart.ClearAllSeries()
        #setup time series data objects
        for i in range(0, len(arrNames)):
            tsData = TimeSeriesData()
            tsData.Name = arrNames[i]
            tsData.StrokeColor = lstColors[i]
            tsData.IsAreaMode = False
            tsData.SetTimeRange(dtStart,dtEnd)
            lstTSData.append(tsData)
            tsData.Clear()
            rslWPFChart.frmChart.wpfChart.AddSeries(lstTSData[i]) #Note: add to chart before data is inserted so that chart will scale correctly
            
        #Add data points    
        for t in range(0, len(arrTime)): #loop through data grid
            for s in range(0, len(lstTSData)): #add data point for each series
                lstTSData[s].AddPoint(arrTime[t], marrData[s][t])
