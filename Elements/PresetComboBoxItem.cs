using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_SA_Chaos.Elements
{

    public class PresetComboBoxItem
    {
        public readonly string Text;
        public readonly bool Reversed;
        public readonly string[] EnabledEffects;

        public PresetComboBoxItem(string text, bool reversed, string[] enabledEffects)
        {
            Text = text;
            Reversed = reversed;
            EnabledEffects = enabledEffects;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
