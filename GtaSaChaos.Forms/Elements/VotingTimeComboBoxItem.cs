namespace GtaChaos.Forms.Elements
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
