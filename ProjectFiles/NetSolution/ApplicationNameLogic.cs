#region Using directives
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.SerialPort;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.EventLogger;
using FTOptix.Store;
using FTOptix.SQLiteStore;
using FTOptix.DataLogger;
using FTOptix.Alarm;
using FTOptix.Report;
using FTOptix.Recipe;
#endregion

public class ApplicationNameLogic : BaseNetLogic
{
    public override void Start()
    {
        Label label = Owner as Label;
        label.Text = Project.Current.BrowseName;
    }

    public override void Stop()
    {
    }
}
