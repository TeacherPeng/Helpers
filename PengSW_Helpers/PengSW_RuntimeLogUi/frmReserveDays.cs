using System.Windows.Forms;

namespace PengSW.RuntimeLog
{
    public partial class frmReserveDays : Form
    {
        public frmReserveDays(decimal aDays)
        {
            InitializeComponent();
            if (aDays < udDays.Minimum) aDays = udDays.Minimum;
            if (aDays > udDays.Maximum) aDays = udDays.Maximum;
            udDays.Value = aDays;
        }

        public decimal Days { get { return udDays.Value; } }
    }
}
