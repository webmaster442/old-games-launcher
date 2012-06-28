using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VersionCreator
{
    public partial class VersionSet : Form
    {
        public VersionSet()
        {
            InitializeComponent();
        }

        public DateTime SelectedDate
        {
            get
            {
                DateTime selected = monthCalendar1.TodayDate;
                selected = selected.AddHours((double)numericUpDown1.Value);
                selected = selected.AddMinutes((double)numericUpDown2.Value);
                selected = selected.AddSeconds((double)numericUpDown3.Value);
                return selected;
            }
        }
    }
}
