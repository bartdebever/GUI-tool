namespace GtaChaos.Forms.Elements
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
