using System;
using System.Diagnostics;
using System.Windows.Forms;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Forms.Timers
{
    public class DefaultTimer : IChaosTimer
    {
        private readonly Stopwatch _stopwatch;
        private readonly ProgressBar _progressBar;
        private int _elapsedCount;
        private Action<AbstractEffect> _callback;

        public DefaultTimer(ProgressBar progressBar)
        {
            _progressBar = progressBar;
            _stopwatch = new Stopwatch();
        }

        public void AddEffectCallback(Action<AbstractEffect> callback)
        {
            _callback = callback;
        }

        public double ElapsedMillis()
        {
            return _stopwatch.ElapsedMilliseconds;
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public void Tick()
        {
            if (!Config.Instance.Enabled)
            {
                return;
            }

            var value = Math.Max(1, (int)_stopwatch.ElapsedMilliseconds);

            _progressBar.Value = Math.Min(value, _progressBar.Maximum);
            _progressBar.Value = Math.Min(value - 1, _progressBar.Maximum);

            if (_stopwatch.ElapsedMilliseconds - _elapsedCount > 100)
            {
                long remaining = Math.Max(0, Config.Instance.MainCooldown - _stopwatch.ElapsedMilliseconds);
                int intRemaning = (int)((float)remaining / Config.Instance.MainCooldown * 1000f);

                ProcessHooker.SendEffectToGame("time", intRemaning.ToString());

                _elapsedCount = (int)_stopwatch.ElapsedMilliseconds;
            }

            if (_stopwatch.ElapsedMilliseconds >= Config.Instance.MainCooldown)
            {
                _progressBar.Value = 0;
                _callback.Invoke(null);
                _elapsedCount = 0;
                _stopwatch.Restart();
            }
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }
    }
}
