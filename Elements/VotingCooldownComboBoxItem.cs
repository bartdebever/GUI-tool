using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_SA_Chaos.Elements
{
    public class VotingCooldownComboBoxItem
    {
        public readonly int VotingCooldown;
        public readonly string Text;

        public VotingCooldownComboBoxItem(string text, int votingCooldown)
        {
            Text = text;
            VotingCooldown = votingCooldown;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
