namespace GtaChaos.Forms.Elements
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
