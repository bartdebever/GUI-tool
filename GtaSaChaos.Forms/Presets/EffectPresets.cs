using System.Collections.Generic;
using GtaChaos.Forms.Elements;
using GtaChaos.Models.Utils;

namespace GtaChaos.Forms.Presets
{
    public static class EffectPresets
    {
        public static List<CategoryTreeNode> GetCategoryTreeNodes()
        {
            return new List<CategoryTreeNode>()
            {
                new CategoryTreeNode(Category.WeaponsAndHealth),
                new CategoryTreeNode(Category.WantedLevel),
                new CategoryTreeNode(Category.Weather),
                new CategoryTreeNode(Category.Spawning),
                new CategoryTreeNode(Category.Time),
                new CategoryTreeNode(Category.VehiclesTraffic),
                new CategoryTreeNode(Category.PedsAndCo),
                new CategoryTreeNode(Category.PlayerModifications),
                new CategoryTreeNode(Category.Stats),
                new CategoryTreeNode(Category.CustomEffects),
                new CategoryTreeNode(Category.Teleportation)
            };
        }

        public static List<PresetComboBoxItem> GetPresets()
        {
            return new List<PresetComboBoxItem>
            {
                new PresetComboBoxItem("Speedrun", reversed: false, new string[]
                {
                    "HE1", "HE2", "HE3", "HE4", "HE5", "HE7",

                    "WA1", "WA2", "WA3", "WA4",

                    "WE1", "WE2", "WE3", "WE4", "WE5", "WE6", "WE7",

                    "SP1", "SP2", "SP19",

                    "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                    "VE1", "VE2", "VE3", "VE4", "VE5", "VE6", "VE7", "VE8", "VE9", "VE10",
                    "VE11", "VE12", "VE13", "VE14",

                    "PE1", "PE2", "PE3", "PE4", "PE5", "PE6", "PE7", "PE8", "PE9", "PE10",
                    "PE11", "PE12", "PE14", "PE15", "PE16", "PE17", "PE18",

                    "MO1", "MO2", "MO3", "MO4", "MO5",

                    "ST1", "ST2", "ST3", "ST4", "ST5", "ST6", "ST7", "ST8", "ST9", "ST10",
                    "ST11", "ST12",

                    "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                    "CE11", "CE12", "CE13", "CE14", "CE16", "CE17", "CE18", "CE19",
                    "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29", "CE30",
                    "CE31", "CE32", "CE33", "CE34", "CE35", "CE36", "CE37", "CE38", "CE39", "CE40",
                    "CE41", "CE43", "CE44", "CE45", "CE46", "CE47", "CE48", "CE49", "CE50",
                    "CE51", "CE52",

                    "TP1"
                }),
                new PresetComboBoxItem("Harmless", reversed: false, new string[]
                {
                    "HE1", "HE2", "HE3", "HE4", "HE5", "HE7",

                    "WA2", "WA3",

                    "WE1", "WE2",

                    "VE2", "VE3", "VE4", "VE5", "VE7", "VE8",
                    "VE11", "VE12", "VE13", "VE14", "VE15",

                    "PE3", "PE5", "PE8", "PE10",
                    "PE11", "PE12", "PE13", "PE14", "PE15", "PE16", "PE17",

                    "MO1", "MO2", "MO3", "MO4", "MO5",

                    "ST2", "ST4", "ST6", "ST8", "ST10",
                    "ST11", "ST12",

                    "CE11", "CE12",
                    "CE22", "CE23", "CE30",
                    "CE40",
                    "CE46", "CE47", "CE49",
                    "CE51", "CE52"
                }),
                new PresetComboBoxItem("Harmful", reversed: false, new string[]
                {
                    "HE6",

                    "WA1", "WA4",

                    "WE3", "WE4", "WE5", "WE6", "WE7",

                    "SP1", "SP2", "SP3", "SP4", "SP5", "SP6", "SP7", "SP8", "SP9", "SP10",
                    "SP11", "SP12", "SP13", "SP14", "SP15", "SP16", "SP17", "SP18", "SP19",

                    "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                    "VE1", "VE6", "VE9", "VE10",

                    "PE1", "PE2", "PE4", "PE6", "PE7", "PE9",
                    "PE18",

                    "ST1", "ST3", "ST5", "ST7", "ST9",

                    "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                    "CE11", "CE12", "CE13", "CE14", "CE15", "CE16", "CE17", "CE18", "CE19", "CE20",
                    "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29", "CE30",
                    "CE31", "CE32", "CE33", "CE34", "CE35", "CE36", "CE37", "CE38", "CE39",
                    "CE41", "CE43", "CE44", "CE45", "CE48", "CE50",

                    "TP1", "TP2", "TP3", "TP4", "TP5", "TP6", "TP7", "TP8", "TP9", "TP10",
                    "TP11", "TP12"
                }),
                new PresetComboBoxItem("Good Luck", reversed: false, new string[]
                {
                    "HE6",

                    "WA4",

                    "WE5", "WE6", "WE7",

                    "SP10",
                    "SP11", "SP15", "SP16", "SP17", "SP19",

                    "TI1", "TI2", "TI3", "TI4", "TI5", "TI6", "TI7",

                    "VE1", "VE4", "VE6", "VE7", "VE9", "VE10",
                    "VE14",

                    "PE1", "PE2", "PE6", "PE7", "PE8", "PE9",
                    "PE18",

                    "ST1", "ST3", "ST5", "ST7", "ST9",

                    "CE1", "CE2", "CE3", "CE4", "CE5", "CE6", "CE7", "CE8", "CE9", "CE10",
                    "CE11", "CE12", "CE13", "CE14", "CE15", "CE16", "CE17", "CE18", "CE19", "CE20",
                    "CE21", "CE22", "CE23", "CE24", "CE25", "CE26", "CE27", "CE28", "CE29", "CE30",
                    "CE31", "CE32", "CE33", "CE34", "CE35", "CE36", "CE37", "CE38", "CE39",
                    "CE41", "CE42", "CE43", "CE44", "CE45", "CE48", "CE50",

                    "TP1", "TP2", "TP3", "TP4", "TP5", "TP6", "TP7", "TP8", "TP9", "TP10",
                    "TP11", "TP12"
                }),
                new PresetComboBoxItem("Everything", reversed: true, new string[] { }),
                new PresetComboBoxItem("Twitch Voting", reversed: true, new string[]
                {
                    "CE41"
                }),
                new PresetComboBoxItem("Nothing", reversed: false, new string[] { })
            };
        }
    }
}
