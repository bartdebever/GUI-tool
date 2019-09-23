using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA_SA_Chaos.Elements;

namespace GTA_SA_Chaos.Presets
{
    public static class CooldownPresets
    {
        public static List<VotingTimeComboBoxItem> GetVotingTimePresets()
        {
            return new List<VotingTimeComboBoxItem>
            {
                new VotingTimeComboBoxItem("5 seconds", 1000 * 5),
                new VotingTimeComboBoxItem("10 seconds", 1000 * 10),
                new VotingTimeComboBoxItem("15 seconds", 1000 * 15),
                new VotingTimeComboBoxItem("20 seconds", 1000 * 20),
                new VotingTimeComboBoxItem("30 seconds", 1000 * 30),
                new VotingTimeComboBoxItem("1 minute", 1000 * 60)
            };
        }

        public static List<VotingCooldownComboBoxItem> GetCooldownTimePresets()
        {
            return new List<VotingCooldownComboBoxItem>
            {
                new VotingCooldownComboBoxItem("10 seconds", 1000 * 10),
                new VotingCooldownComboBoxItem("30 seconds", 1000 * 30),
                new VotingCooldownComboBoxItem("1 minute", 1000 * 60),
                new VotingCooldownComboBoxItem("2 minutes", 1000 * 60 * 2),
                new VotingCooldownComboBoxItem("5 minutes", 1000 * 60 * 5),
                new VotingCooldownComboBoxItem("10 minutes", 1000 * 60 * 10)
            };
        }

        public static List<MainCooldownComboBoxItem> GetMainCooldownTimePresets()
        {
            return new List<MainCooldownComboBoxItem>()
            {
                new MainCooldownComboBoxItem("10 seconds", 1000 * 10),
                new MainCooldownComboBoxItem("20 seconds", 1000 * 20),
                new MainCooldownComboBoxItem("30 seconds", 1000 * 30),
                new MainCooldownComboBoxItem("1 minute", 1000 * 60),
                new MainCooldownComboBoxItem("2 minutes", 1000 * 60 * 2),
                new MainCooldownComboBoxItem("5 minutes", 1000 * 60 * 5),
                new MainCooldownComboBoxItem("10 minutes", 1000 * 60 * 10),
                new MainCooldownComboBoxItem("DEBUG - 1 second", 1000)
            };
        }
    }
}
