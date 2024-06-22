#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.Alarm;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.DataLogger;
using FTOptix.EventLogger;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.UI;
using FTOptix.Core;
using System.IO;
using System.Threading;
using FTOptix.Modbus;
using FTOptix.Recipe;
#endregion

public class eChartLogic : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        if (!startCompleted)
        {
            eChartRefreshTimeVariable = LogicObject.GetVariable("RefreshTime");
            if (eChartRefreshTimeVariable == null)
                throw new CoreConfigurationException("Server RefreshTime variable not found");

            myPeriodicTask = new PeriodicTask(RefresheChartGraph, eChartRefreshTimeVariable.Value * 1000, LogicObject);
            myPeriodicTask.Start();

            startCompleted = true;
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        //myPeriodicTask.Dispose();
        //startCompleted = false;
    }

    [ExportMethod]
    public void RefresheChartGraph()
    {
        for (int i = 1; i <= 20; i++)
            eChartValue[i - 1] = LogicObject.GetVariable("eChartsValue/eChart" + i).Value;

        for (int j = 1; j <= 10; j++)
        {
            eChartControl(j);
        }
    }

    public void eChartControl(int i)
    {
        eChart = LogicObject.GetVariable("eChart" + i);
        if (eChart.Value)
        {
            eChartTemplateNameVariable = LogicObject.GetVariable("eChart" + i + "/TemplateName");
            if (eChartTemplateNameVariable == null)
                throw new CoreConfigurationException("Server eChart" + i + "TemplateName variable not found");

            eChartDataNameVariable = LogicObject.GetVariable("eChart" + i + "/DataName");
            if (eChartDataNameVariable == null)
                throw new CoreConfigurationException("Server eChart" + i + "DataName variable not found");

            eChartWebNameVariable = LogicObject.GetVariable("eChart" + i + "/WebName");
            if (eChartWebNameVariable == null)
                throw new CoreConfigurationException("Server eChart" + i + "WebName variable not found");

            eChartNoStartVariable = LogicObject.GetVariable("eChart" + i + "/eChartNoStart");
            if (eChartNoStartVariable == null)
                throw new CoreConfigurationException("Server eChart" + i + "eChartNoStart variable not found");

            eChartNoEndVariable = LogicObject.GetVariable("eChart" + i + "/eChartNoEnd");
            if (eChartNoEndVariable == null)
                throw new CoreConfigurationException("Server eChart" + i + "eChartNoEnd variable not found");

            UpdateChartGraph(eChartTemplateNameVariable.Value, eChartDataNameVariable.Value, eChartNoStartVariable.Value, eChartNoEndVariable.Value, eChartWebNameVariable.Value);
        }
    }

    public void UpdateChartGraph(string templateName, string fileName, int eChartNoStart, int eChartNoEnd, string wbName)
    {
        //Owner.Get<WebBrowser>(wbName).Visible = false;
        
        Log.Debug("eCharts", "Starting");
        String projectPath = (ResourceUri.FromProjectRelativePath("").Uri);
        String folderSeparator = Path.DirectorySeparatorChar.ToString();

        // Get template name and create destination path
        string templatePath = projectPath + folderSeparator + "eCharts" + folderSeparator + templateName;
        string filePath = projectPath + folderSeparator + "eCharts" + folderSeparator + fileName;

        // Read template page content
        string text = File.ReadAllText(templatePath);

        // Insert values
        for (int i = eChartNoStart; i <= eChartNoEnd; i++)
        {
            //text = text.Replace(i < 10 ? "$0" + i : "$" + i, (Project.Current.GetVariable(modelPath + i).Value * 1).ToString());
            text = text.Replace(i < 10 ? "$0" + i : "$" + i, eChartValue[i - 1].ToString());
        }

        // Write to file
        File.WriteAllText(filePath, text);

        // Refresh WebBrowser page
        Owner.Get<WebBrowser>(wbName).Refresh();

        Log.Debug("eCharts", "Finished");
        Thread.Sleep(1);
        //Owner.Get<WebBrowser>(wbName).Visible = true;
    }

    private PeriodicTask myPeriodicTask;

    private IUAVariable eChartRefreshTimeVariable;
    private IUAVariable eChart;
    private IUAVariable eChartTemplateNameVariable;
    private IUAVariable eChartDataNameVariable;
    private IUAVariable eChartWebNameVariable;
    private IUAVariable eChartNoStartVariable;
    private IUAVariable eChartNoEndVariable;
    float[] eChartValue = new float[20];

    Boolean startCompleted;
}
