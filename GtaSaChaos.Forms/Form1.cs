// Copyright (c) 2019 Lordmau5

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GtaChaos.Forms.Elements;
using GtaChaos.Forms.Presets;
using GtaChaos.Models.Effects;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Forms
{
    public partial class Form1 : Form
    {
        #region Constants
        private const int MaxListBoxEffects = 7;
        private const string ConfigFileName = "config.cfg";
        #endregion Constants
        private readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);

        private readonly Stopwatch stopwatch;
        private readonly Dictionary<string, EffectTreeNode> idToEffectNodeMap = new Dictionary<string, EffectTreeNode>();
        private TwitchConnection twitch;

        private int elapsedCount;
        private readonly System.Timers.Timer autoStartTimer;
        private int introState = 1;

        private int timesUntilRapidFire;

        public Form1()
        {
            InitializeComponent();

            Text = "GTA:SA Chaos v1.1.3";
            tabSettings.TabPages.Remove(tabDebug);

            stopwatch = new Stopwatch();
            autoStartTimer = new System.Timers.Timer()
            {
                Interval = 50,
                AutoReset = true
            };
            autoStartTimer.Elapsed += AutoStartTimer_Elapsed;

            PopulateEffectTreeList();

            PopulateMainCooldowns();
            PopulatePresets();

            tabSettings.TabPages.Remove(tabTwitch);

            PopulateVotingTimes();
            PopulateVotingCooldowns();

            TryLoadConfig();

            timesUntilRapidFire = new Random().Next(10, 15);
        }

        private void AutoStartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ProcessHooker.HasExited())
            {
                return;
            }

            if (Config.Instance.Enabled)
            {
                return;
            }

            MemoryHelper.Read((IntPtr)0xA4ED04, out int newIntroState);
            MemoryHelper.Read((IntPtr)0xB7CB84, out int playingTime);

            if (introState == 0 && newIntroState == 1 && playingTime < 1000 * 60)
            {
                buttonAutoStart.Invoke(new Action(SetAutostart));
            }

            introState = newIntroState;
        }

        #region UI Callbacks
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
            ProcessHooker.CloseProcess();
        }

        private void ButtonAutoStart_Click(object sender, EventArgs e)
        {
            if (ProcessHooker.HasExited()) // Make sure we are hooked
            {
                ProcessHooker.HookProcess();
            }

            if (ProcessHooker.HasExited())
            {
                MessageBox.Show("The game needs to be running!", "Error");

                buttonAutoStart.Enabled = Config.Instance.IsTwitchMode && twitch?.Client != null && twitch.Client.IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance.ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
                }

                return;
            }

            ProcessHooker.AttachExitedMethod((_, __) => buttonAutoStart.Invoke(new Action(() =>
            {
                buttonAutoStart.Enabled = Config.Instance.IsTwitchMode && twitch?.Client != null && twitch.Client.IsConnected;
                buttonAutoStart.Text = "Auto-Start";

                if (!Config.Instance.ContinueTimer)
                {
                    SetEnabled(false);

                    elapsedCount = 0;
                    stopwatch.Reset();

                    buttonMainToggle.Enabled = true;
                    buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
                }

                ProcessHooker.CloseProcess();
            })));

            buttonAutoStart.Enabled = false;
            buttonAutoStart.Text = "Waiting...";

            Config.Instance.Enabled = false;
            autoStartTimer.Start();
            buttonMainToggle.Enabled = false;
            buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
        }


        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Config.Instance.IsTwitchMode)
            {
                TickTwitch();
            }
            else
            {
                TickMain();
            }
        }


        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PresetComboBoxItem item = (PresetComboBoxItem)presetComboBox.SelectedItem;

            LoadPreset(item.Reversed, item.EnabledEffects);
        }

        private void ComboBoxVotingTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingTimeComboBoxItem item = (VotingTimeComboBoxItem)comboBoxVotingTime.SelectedItem;
            Config.Instance.TwitchVotingTime = item.VotingTime;
        }

        private void ComboBoxVotingCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            VotingCooldownComboBoxItem item = (VotingCooldownComboBoxItem)comboBoxVotingCooldown.SelectedItem;
            Config.Instance.TwitchVotingCooldown = item.VotingCooldown;
        }

        private void MainCooldownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainCooldownComboBoxItem item = (MainCooldownComboBoxItem)comboBoxMainCooldown.SelectedItem;
            Config.Instance.MainCooldown = item.Time;

            if (!Config.Instance.Enabled)
            {
                progressBarMain.Value = 0;
                progressBarMain.Maximum = Config.Instance.MainCooldown;
                elapsedCount = 0;
                stopwatch.Reset();
            }
        }

        private void ButtonMainToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance.Enabled);
        }

        private void EnabledEffectsView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node is EffectTreeNode etn)
                {
                    EffectDatabase.SetEffectEnabled(etn.Effect, etn.Checked);
                }

                if (e.Node.Nodes.Count > 0)
                {
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }

                foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
                {
                    node.UpdateCategory();
                }
            }
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadPresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Preset File|*.cfg",
                Title = "Save Preset"
            };

            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                string content = File.ReadAllText(dialog.FileName);
                string[] enabledEffects = content.Split(',');

                List<string> enabledEffectList = new List<string>();

                foreach (string effect in enabledEffects)
                {
                    enabledEffectList.Add(effect);
                }

                LoadPreset(false, enabledEffectList.ToArray());
            }

            dialog.Dispose();
        }

        private void SavePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> enabledEffects = new List<string>();
            foreach (EffectTreeNode node in idToEffectNodeMap.Values)
            {
                if (node.Checked)
                {
                    enabledEffects.Add(node.Effect.Id);
                }
            }

            string joined = string.Join(",", enabledEffects);

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Preset File|*.cfg",
                Title = "Save Preset"
            };
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                File.WriteAllText(dialog.FileName, joined);
            }

            dialog.Dispose();
        }

        private void ButtonConnectTwitch_Click(object sender, EventArgs e)
        {
            if (twitch != null && twitch.Client.IsConnected)
            {
                twitch?.Kill();
                twitch = null;

                comboBoxVotingTime.Enabled = true;
                comboBoxVotingCooldown.Enabled = true;

                textBoxTwitchChannel.Enabled = true;
                textBoxTwitchUsername.Enabled = true;
                textBoxTwitchOAuth.Enabled = true;

                buttonConnectTwitch.Text = "Connect to Twitch";

                if (!tabSettings.TabPages.Contains(tabEffects))
                {
                    tabSettings.TabPages.Insert(tabSettings.TabPages.IndexOf(tabTwitch), tabEffects);
                }

                return;
            }

            if (Config.Instance.TwitchChannel != "" && Config.Instance.TwitchUsername != "" && Config.Instance.TwitchOAuthToken != "")
            {
                buttonConnectTwitch.Enabled = false;

                twitch = new TwitchConnection();

                twitch.OnRapidFireEffect += (_sender, rapidFireArgs) =>
                {
                    Invoke(new Action(() =>
                    {
                        if (Config.Instance.TwitchVotingMode == 2)
                        {
                            rapidFireArgs.Effect.RunEffect();
                            AddEffectToListBox(rapidFireArgs.Effect);
                        }
                    }));
                };

                twitch.Client.OnIncorrectLogin += (_sender, _e) =>
                {
                    MessageBox.Show("There was an error trying to log in to the account. Wrong username / OAuth token?", "Twitch Login Error");
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                    }));
                    twitch.Kill();
                };

                twitch.Client.OnConnected += (_sender, _e) =>
                {
                    Invoke(new Action(() =>
                    {
                        buttonConnectTwitch.Enabled = true;
                        buttonTwitchToggle.Enabled = true;

                        buttonAutoStart.Enabled = true;

                        buttonConnectTwitch.Text = "Disconnect";

                        textBoxTwitchChannel.Enabled = false;
                        textBoxTwitchUsername.Enabled = false;
                        textBoxTwitchOAuth.Enabled = false;
                    }));
                };
            }
        }

        private void TextBoxTwitchChannel_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchChannel = textBoxTwitchChannel.Text;
            UpdateConnectTwitchState();
        }

        private void TextBoxUsername_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchUsername = textBoxTwitchUsername.Text;
            UpdateConnectTwitchState();
        }

        private void TextBoxOAuth_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchOAuthToken = textBoxTwitchOAuth.Text;
            UpdateConnectTwitchState();
        }

        private void ButtonSwitchMode_Click(object sender, EventArgs e)
        {
            TabPage pageToAdd;
            TabPage pageToRemove;

            if (Config.Instance.IsTwitchMode)
            {
                Config.Instance.IsTwitchMode = false;
                buttonSwitchMode.Text = "Twitch";

                pageToAdd = tabMain;
                pageToRemove = tabTwitch;
                listLastEffectsMain.Items.Clear();
                progressBarMain.Value = 0;
            }
            else
            {
                Config.Instance.IsTwitchMode = true;
                buttonSwitchMode.Text = "Main";
                buttonAutoStart.Enabled = twitch?.Client != null && twitch.Client.IsConnected;

                pageToAdd = tabTwitch;
                pageToRemove = tabMain;
                listLastEffectsTwitch.Items.Clear();
                progressBarTwitch.Value = 0;
            }

            tabSettings.TabPages.Insert(0, pageToAdd);
            tabSettings.SelectedIndex = 0;
            tabSettings.TabPages.Remove(pageToRemove);
            elapsedCount = 0;
            stopwatch.Reset();
            SetEnabled(false);

        }

        private void ButtonTwitchToggle_Click(object sender, EventArgs e)
        {
            SetEnabled(!Config.Instance.Enabled);
        }

        private void TextBoxSeed_TextChanged(object sender, EventArgs e)
        {
            Config.Instance.Seed = textBoxSeed.Text;
            RandomHandler.SetSeed(Config.Instance.Seed);
        }

        private void ButtonTestSeed_Click(object sender, EventArgs e)
        {
            labelTestSeed.Text = $"{RandomHandler.Next(100, 999)}";
        }

        private void ButtonGenericTest_Click(object sender, EventArgs e)
        {
            ProcessHooker.SendEffectToGame("effect", "set_vehicle_on_fire", 60000, "Set Vehicle On Fire");
            ProcessHooker.SendEffectToGame("timed_effect", "one_hit_ko", 60000, "One Hit K.O.", "25characterusernamehanice");
            //ProcessHooker.SendEffectToGame("timed_effect", "fail_mission", 60000, "Fail Current Mission", "lordmau5");
        }

        private void ButtonResetMain_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            stopwatch.Reset();
            elapsedCount = 0;
            progressBarMain.Value = 0;
            buttonMainToggle.Enabled = true;
            buttonMainToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = true;
            buttonAutoStart.Text = "Auto-Start";
        }

        private void CheckBoxContinueTimer_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.ContinueTimer = checkBoxContinueTimer.Checked;
        }

        private void CheckBoxCrypticEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.CrypticEffects = checkBoxCrypticEffects.Checked;
        }

        private void CheckBoxShowLastEffectsMain_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.MainShowLastEffects
                = listLastEffectsMain.Visible
                = checkBoxShowLastEffectsMain.Checked;
        }

        private void CheckBoxShowLastEffectsTwitch_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchShowLastEffects
                = listLastEffectsTwitch.Visible
                = checkBoxShowLastEffectsTwitch.Checked;
        }

        private void CheckBoxTwitchAllowOnlyEnabledEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchAllowOnlyEnabledEffectsRapidFire = checkBoxTwitchAllowOnlyEnabledEffects.Checked;
        }

        private void CheckBoxTwitchMajorityVoting_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.TwitchMajorityVoting = checkBoxTwitchMajorityVoting.Checked;
        }

        private void ButtonResetTwitch_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            stopwatch.Reset();
            elapsedCount = 0;
            progressBarTwitch.Value = 0;
            buttonTwitchToggle.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
            buttonTwitchToggle.Text = "Start / Resume";
            buttonAutoStart.Enabled = twitch?.Client != null && twitch.Client.IsConnected;
            buttonAutoStart.Text = "Auto-Start";
        }

        private void CheckBoxTwitch3TimesCooldown_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.Twitch3TimesCooldown = checkBoxTwitch3TimesCooldown.Checked;
        }
        #endregion

        // Done
        #region Configurations
        private void TryLoadConfig()
        {
            Config.LoadConfig(configPath);

            UpdateInterface();
        }

        private void SaveConfig()
        {
            if (Config.Instance == null)
            {
                return;
            }

            Config.Instance.SaveConfig(configPath);
        }
        #endregion Configuration

        // Done
        #region Presets
        private void PopulateVotingTimes()
        {
            // AddRange can cause an exception, lets not do that.
            foreach (var votingTime in CooldownPresets.GetVotingTimePresets())
            {
                comboBoxVotingTime.Items.Add(votingTime);
            }

            // Set default value.
            comboBoxVotingTime.SelectedIndex = 2;

            // 15 Seconds.
            Config.Instance.TwitchVotingTime = 1000 * 15;
        }


        private void PopulateVotingCooldowns()
        {
            // AddRange can cause an exception, lets not do that.
            foreach (var votingTime in CooldownPresets.GetCooldownTimePresets())
            {
                comboBoxVotingTime.Items.Add(votingTime);
            }

            comboBoxVotingCooldown.SelectedIndex = 2;

            // 2 Minutes
            Config.Instance.TwitchVotingCooldown = 1000 * 60 * 2;
        }
        private void PopulateMainCooldowns()
        {
            // AddRange can cause an exception, lets not do that.
            foreach (var mainCooldown in CooldownPresets.GetMainCooldownTimePresets())
            {
                comboBoxVotingTime.Items.Add(mainCooldown);
            }

            comboBoxMainCooldown.SelectedIndex = 3;

            Config.Instance.MainCooldown = 1000 * 60;
        }

        private void PopulateEffectTreeList()
        {
            // Add Categories
            foreach (var categoryTreeNode in EffectPresets.GetCategoryTreeNodes())
            {
                enabledEffectsView.Nodes.Add(categoryTreeNode);
            }

            // Add Effects
            foreach (AbstractEffect effect in EffectDatabase.Effects)
            {
                TreeNode node = enabledEffectsView.Nodes.Find(effect.Category.Name, false).FirstOrDefault();
                EffectTreeNode addedNode = new EffectTreeNode(effect)
                {
                    Checked = true
                };
                node.Nodes.Add(addedNode);
                idToEffectNodeMap.Add(effect.Id, addedNode);
            }
        }

        private void PopulatePresets()
        {
            foreach (var preset in EffectPresets.GetPresets())
            {
                presetComboBox.Items.Add(preset);
            }

            presetComboBox.SelectedIndex = 0;
        }
        #endregion

        // Checked
        private void UpdateInterface()
        {
            foreach (MainCooldownComboBoxItem item in comboBoxMainCooldown.Items)
            {
                if (item.Time == Config.Instance.MainCooldown)
                {
                    comboBoxMainCooldown.SelectedItem = item;
                    break;
                }
            }

            checkBoxTwitchAllowOnlyEnabledEffects.Checked = Config.Instance.TwitchAllowOnlyEnabledEffectsRapidFire;

            foreach (VotingTimeComboBoxItem item in comboBoxVotingTime.Items)
            {
                if (item.VotingTime == Config.Instance.TwitchVotingTime)
                {
                    comboBoxVotingTime.SelectedItem = item;
                    break;
                }
            }

            foreach (VotingCooldownComboBoxItem item in comboBoxVotingCooldown.Items)
            {
                if (item.VotingCooldown == Config.Instance.TwitchVotingCooldown)
                {
                    comboBoxVotingCooldown.SelectedItem = item;
                    break;
                }
            }

            textBoxTwitchChannel.Text = Config.Instance.TwitchChannel;
            textBoxTwitchUsername.Text = Config.Instance.TwitchUsername;
            textBoxTwitchOAuth.Text = Config.Instance.TwitchOAuthToken;

            checkBoxContinueTimer.Checked = Config.Instance.ContinueTimer;
            checkBoxCrypticEffects.Checked = Config.Instance.CrypticEffects;

            checkBoxShowLastEffectsMain.Checked = Config.Instance.MainShowLastEffects;
            checkBoxShowLastEffectsTwitch.Checked = Config.Instance.TwitchShowLastEffects;
            checkBoxTwitchMajorityVoting.Checked = Config.Instance.TwitchMajorityVoting;
            checkBoxTwitch3TimesCooldown.Checked = Config.Instance.Twitch3TimesCooldown;

            textBoxSeed.Text = Config.Instance.Seed;
        }

        // Checked
        public void AddEffectToListBox(AbstractEffect effect)
        {
            var description = "Invalid";
            if (effect != null)
            {
                description = effect.GetDescription();

                if (!string.IsNullOrEmpty(effect.Word))
                {
                    description += $" ({effect.Word})";
                }
            }

            var listBox = Config.Instance.IsTwitchMode ? listLastEffectsTwitch : listLastEffectsMain;

            listBox.Items.Insert(0, description);
            if (listBox.Items.Count > MaxListBoxEffects)
            {
                listBox.Items.RemoveAt(MaxListBoxEffects);
            }
        }

        // Checked
        private void CallEffect(AbstractEffect effect = null)
        {
            if (effect == null)
            {
                effect = EffectDatabase.RunEffect(EffectDatabase.GetRandomEffect(true));
                effect.ResetVoter();
            }
            else
            {
                EffectDatabase.RunEffect(effect);
            }

            AddEffectToListBox(effect);
        }

        #region Timer logic

        private void TickMain()
        {
            if (!Config.Instance.Enabled)
            {
                return;
            }

            var value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);

            // Hack to fix Windows' broken-ass progress bar handling
            progressBarMain.Value = Math.Min(value, progressBarMain.Maximum);
            progressBarMain.Value = Math.Min(value - 1, progressBarMain.Maximum);

            if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
            {
                long remaining = Math.Max(0, Config.Instance.MainCooldown - stopwatch.ElapsedMilliseconds);
                int intRemaning = (int)((float)remaining / Config.Instance.MainCooldown * 1000f);

                ProcessHooker.SendEffectToGame("time", intRemaning.ToString());

                elapsedCount = (int)stopwatch.ElapsedMilliseconds;
            }

            if (stopwatch.ElapsedMilliseconds >= Config.Instance.MainCooldown)
            {
                progressBarMain.Value = 0;
                CallEffect();
                elapsedCount = 0;
                stopwatch.Restart();
            }
        }

        private void TickTwitch()
        {
            if (!Config.Instance.Enabled)
            {
                return;
            }

            // Voting Mode: 
            // 0: Cooldown
            // 1: Voting
            // 2: Rapid Fire
            switch (Config.Instance.TwitchVotingMode)
            {
                case 1:
                {
                    // If the maximum of the progress bar is wrong, fix it.
                    if (progressBarTwitch.Maximum != Config.Instance.TwitchVotingTime)
                    {
                        progressBarTwitch.Maximum = Config.Instance.TwitchVotingTime;
                    }

                    // Set progress bar to be the correct values.
                    // Hack to fix Windows' broken-ass progress bar handling
                    int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);
                    progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                    progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                    // Not enough time has passed.
                    if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                    {
                        // Calculate the remaining time and send it to the game
                        long remaining = Math.Max(0, Config.Instance.TwitchVotingTime - stopwatch.ElapsedMilliseconds);
                        int iRemaining = (int)((float)remaining / Config.Instance.TwitchVotingTime * 1000f);

                        // Send time remaining to the game.
                        ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                        // Send the effects being voted on to the game.
                        twitch?.SendEffectVotingToGame();

                        // Increase the elapsed timer.
                        elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                    }

                    // Enough time has passed for voting.
                    if (stopwatch.ElapsedMilliseconds >= Config.Instance.TwitchVotingTime)
                    {
                        // Reset the timer.
                        ResetTimer();

                        // Change the mode to cooldown.
                        Config.Instance.TwitchVotingMode = 0;

                        labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                        // If Twitch is not being used.
                        if (twitch != null)
                        {
                            TwitchConnection.VotingElement element = twitch.GetRandomVotedEffect(out string username);

                            // End voting and send the effect to the game.
                            twitch.SetVoting(0, timesUntilRapidFire, element, username);
                            CallEffect(element.Effect);
                        }
                    }

                    break;
                }
                case 2:
                {
                    // Correct the maximum value of the progress bar.
                    if (progressBarTwitch.Maximum != 1000 * 10)
                    {
                        progressBarTwitch.Maximum = 1000 * 10;
                    }

                    // Set the current percentage in the UI.
                    // Hack to fix Windows' broken-ass progress bar handling
                    int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);
                    progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value, 0);
                    progressBarTwitch.Value = Math.Max(progressBarTwitch.Maximum - value - 1, 0);

                    // if it hasn't reached the full time.
                    if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                    {
                        long remaining = Math.Max(0, (1000 * 10) - stopwatch.ElapsedMilliseconds);
                        int iRemaining = (int)((float)remaining / (1000 * 10) * 1000f);

                        // Update the time.
                        ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                        elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                    }

                    // If it has reached the time.
                    if (stopwatch.ElapsedMilliseconds >= 1000 * 10) // Set 10 seconds
                    {
                        ResetTimer();
                        Config.Instance.TwitchVotingMode = 0;

                        labelTwitchCurrentMode.Text = "Current Mode: Cooldown";

                        twitch?.SetVoting(0, timesUntilRapidFire);
                    }

                    break;
                }
                case 0:
                {
                    if (progressBarTwitch.Maximum != Config.Instance.TwitchVotingCooldown)
                    {
                        progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;
                    }

                    // Hack to fix Windows' broken-ass progress bar handling
                    int value = Math.Max(1, (int)stopwatch.ElapsedMilliseconds);
                    progressBarTwitch.Value = Math.Min(value + 1, progressBarTwitch.Maximum);
                    progressBarTwitch.Value = Math.Min(value, progressBarTwitch.Maximum);

                    if (stopwatch.ElapsedMilliseconds - elapsedCount > 100)
                    {
                        long remaining = Math.Max(0, Config.Instance.TwitchVotingCooldown - stopwatch.ElapsedMilliseconds);
                        int iRemaining = Math.Min(1000, 1000 - (int)((float)remaining / Config.Instance.TwitchVotingCooldown * 1000f));

                        ProcessHooker.SendEffectToGame("time", iRemaining.ToString());

                        elapsedCount = (int)stopwatch.ElapsedMilliseconds;
                    }

                    if (stopwatch.ElapsedMilliseconds >= Config.Instance.TwitchVotingCooldown)
                    {
                        elapsedCount = 0;

                        if (--timesUntilRapidFire == 0)
                        {
                            progressBarTwitch.Value = progressBarTwitch.Maximum = 1000 * 10;

                            timesUntilRapidFire = new Random().Next(10, 15);

                            Config.Instance.TwitchVotingMode = 2;
                            labelTwitchCurrentMode.Text = "Current Mode: Rapid-Fire";

                            twitch?.SetVoting(2, timesUntilRapidFire);
                        }
                        else
                        {
                            progressBarTwitch.Value = progressBarTwitch.Maximum = Config.Instance.TwitchVotingTime;

                            Config.Instance.TwitchVotingMode = 1;
                            labelTwitchCurrentMode.Text = "Current Mode: Voting";

                            twitch?.SetVoting(1, timesUntilRapidFire);
                        }
                        stopwatch.Restart();
                    }

                    break;
                }
            }
        }

        private void ResetTimer()
        {
            ProcessHooker.SendEffectToGame("time", "0");
            elapsedCount = 0;

            progressBarTwitch.Value = 0;
            progressBarTwitch.Maximum = Config.Instance.TwitchVotingCooldown;

            stopwatch.Restart();
        }

        #endregion

        private void SetAutostart()
        {
            buttonAutoStart.Enabled = Config.Instance.IsTwitchMode && twitch != null && twitch.Client != null && twitch.Client.IsConnected;
            buttonAutoStart.Text = "Auto-Start";
            stopwatch.Reset();
            SetEnabled(true);
        }

        private void SetEnabled(bool enabled)
        {
            Config.Instance.Enabled = enabled;
            if (Config.Instance.Enabled)
            {
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
            }
            autoStartTimer.Stop();
            buttonMainToggle.Enabled = true;
            (Config.Instance.IsTwitchMode ? buttonTwitchToggle : buttonMainToggle).Text = Config.Instance.Enabled ? "Stop / Pause" : "Start / Resume";

            comboBoxMainCooldown.Enabled =
                buttonSwitchMode.Enabled =
                buttonResetMain.Enabled =
                buttonResetTwitch.Enabled = !Config.Instance.Enabled;

            comboBoxVotingTime.Enabled =
                comboBoxVotingCooldown.Enabled = !Config.Instance.Enabled;
        }
        
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node is EffectTreeNode etn)
                {
                    EffectDatabase.SetEffectEnabled(etn.Effect, etn.Checked);
                }

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void LoadPreset(bool reversed, string[] enabledEffects)
        {
            foreach (TreeNode node in enabledEffectsView.Nodes)
            {
                node.Checked = !reversed;
                CheckAllChildNodes(node, reversed);
            }

            foreach (string effect in enabledEffects)
            {
                if (idToEffectNodeMap.TryGetValue(effect, out EffectTreeNode node))
                {
                    node.Checked = !reversed;
                    EffectDatabase.SetEffectEnabled(node.Effect, !reversed);
                }
            }

            foreach (CategoryTreeNode node in enabledEffectsView.Nodes)
            {
                node.UpdateCategory();
            }
        }

        private void UpdateConnectTwitchState()
        {
            buttonConnectTwitch.Enabled =
                textBoxTwitchChannel.Text != "" &&
                textBoxTwitchUsername.Text != "" &&
                textBoxTwitchOAuth.Text != "";

            textBoxTwitchChannel.Enabled = textBoxTwitchUsername.Enabled = textBoxTwitchOAuth.Enabled = true;
        }
    }
}
