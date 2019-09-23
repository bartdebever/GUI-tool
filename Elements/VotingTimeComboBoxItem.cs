using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_SA_Chaos.Elements
{
    public class VotingTimeComboBoxItem
    {
        public readonly int VotingTime;
        public readonly string Text;

        public VotingTimeComboBoxItem(string text, int votingTime)
        {
            Text = text;
            VotingTime = votingTime;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
