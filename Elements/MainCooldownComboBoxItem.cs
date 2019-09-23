using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_SA_Chaos.Elements
{
    public class MainCooldownComboBoxItem
    {
        public readonly string Text;
        public readonly int Time;

        public MainCooldownComboBoxItem(string text, int time)
        {
            Text = text;
            Time = time;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
