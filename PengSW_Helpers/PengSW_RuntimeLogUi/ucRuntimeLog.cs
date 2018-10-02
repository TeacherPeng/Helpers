using System;
using System.Windows.Forms;

namespace PengSW.RuntimeLog
{
    public partial class ucRuntimeLog : UserControl
    {
        public ucRuntimeLog()
        {
            InitializeComponent();
            txtLevel.Text = RL.MinLevelToClarify.ToString();
        }

        public int MaxLength
        {
            get { return uciRuntimeLogView.MaxLength; }
            set { uciRuntimeLogView.MaxLength = value; }
        }

        public int ReserveDays { get; set; }

        public void Bind(RuntimeLog aRuntimeLog)
        {
            uciRuntimeLogView.Bind(aRuntimeLog);
            _Clearer.Bind(aRuntimeLog);
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            uciRuntimeLogView.Clear();
        }

        private void cmdOpen_Click(object sender, EventArgs e)
        {
            if (uciRuntimeLogView.RuntimeLog == null)
            {
                RL.OpenLog();
            }
            else
            {
                uciRuntimeLogView.RuntimeLog.OpenLog();
            }
        }

        private void cmdReserveDays_Click(object sender, EventArgs e)
        {
            mnuReserve03.Checked = ReserveDays == 3;
            mnuReserve07.Checked = ReserveDays == 7;
            mnuReserve10.Checked = ReserveDays == 10;
            mnuReserve30.Checked = ReserveDays == 30;
            mnuReserveDays.Checked = (ReserveDays != 3 && ReserveDays != 7 && ReserveDays != 10 && ReserveDays != 30);
        }

        public event System.EventHandler ReserveDaysChange;

        private Clearer _Clearer = new Clearer();

        private void SetReserveDays(int aDays)
        {
            ReserveDays = aDays;
            _Clearer.ReserveDays = aDays;
            if (ReserveDaysChange != null) ReserveDaysChange(this, null);
        }

        private void mnuReserveDays_Click(object sender, EventArgs e)
        {
            frmReserveDays aForm = new frmReserveDays(ReserveDays);
            if (aForm.ShowDialog(this) != DialogResult.OK) return;
            SetReserveDays((int)aForm.Days);
        }

        private void mnuReserve03_Click(object sender, EventArgs e)
        {
            SetReserveDays(3);
        }

        private void mnuReserve07_Click(object sender, EventArgs e)
        {
            SetReserveDays(7);
        }

        private void mnuReserve10_Click(object sender, EventArgs e)
        {
            SetReserveDays(10);
        }

        private void mnuReserve30_Click(object sender, EventArgs e)
        {
            SetReserveDays(30);
        }

        private void txtLevel_TextChanged(object sender, EventArgs e)
        {
            int aLevel = 9;
            if (int.TryParse(txtLevel.Text, out aLevel))
            {
                RL.MinLevelToClarify = aLevel;
            }
        }
    }
}
