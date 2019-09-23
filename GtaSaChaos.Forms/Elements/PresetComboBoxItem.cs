namespace GtaChaos.Forms.Elements
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
