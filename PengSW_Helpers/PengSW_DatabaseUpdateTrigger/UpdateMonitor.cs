using System;
using System.Data.SqlClient;
using System.Windows.Threading;
using static PengSW.RuntimeLog.RL;

namespace PengSW.DatabaseUpdateTrigger
{
    /// <summary>
    /// 数据库更新监视器
    ///     通过定时调用实现IUpdateCheck的检查器的检查方法，得到数据库的更新信息。
    ///     定时器采用DispatcherTimer，因此需要在WPF应用中创建。
    ///     基本用法是根据需要，创建一个合适的UpdateCheck实例，并以其为参数创建一个UpdateMonitor的实例，加入到窗体或模型中，由宿主响应Update来获得更新通知。
    ///     目前可用的IUpdateCheck实现类有DatabaseUpdateCheck和DatacontextUpdateCheck。
    /// </summary>
    public class UpdateMonitor : IDisposable
    {
        public UpdateMonitor(string aConnectionString, string aUpdateTableName, TimeSpan aInterval)
        {
            _ConnectionString = aConnectionString;
            _UpdateCheck = new DatabaseUpdateTrigger.DatabaseUpdateCheck(aConnectionString, aUpdateTableName);
            _Interval = aInterval;
        }

        public void Start()
        {
            _SqlConnection = new SqlConnection(_ConnectionString);
            _SqlConnection.Open();
            _Timer = new System.Threading.Timer(new System.Threading.TimerCallback(this.OnTimer_Callback), null, TimeSpan.Zero, _Interval);
        }

        public void Stop()
        {
            _Timer?.Dispose();
            _Timer = null;
            _SqlConnection?.Close();
            _SqlConnection?.Dispose();
            _SqlConnection = null;
        }

        private void OnTimer_Callback(object state)
        {
            if (_IsCallbacking) return;
            _IsCallbacking = true;
            try
            {
                string[] aUpdateTableNames = _UpdateCheck.GetUpdatedTableNames(_SqlConnection);
                if (aUpdateTableNames != null && aUpdateTableNames.Length > 0) Update?.Invoke(this, aUpdateTableNames);
            }
            catch (Exception ex)
            {
                E(ex, "Update Timer Callback");
            }
            _IsCallbacking = false;
        }
        private volatile bool _IsCallbacking = false;

        public event Action<UpdateMonitor, string[]> Update;

        private SqlConnection _SqlConnection;
        private string _ConnectionString;
        private TimeSpan _Interval;
        private System.Threading.Timer _Timer;
        private DatabaseUpdateCheck _UpdateCheck;

        public void Dispose()
        {
            Stop();
        }
    }
}
