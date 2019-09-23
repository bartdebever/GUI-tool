using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GtaChaos.Models.Effects.@abstract;

namespace GtaChaos.Forms.Timers
{
    public interface IChaosTimer
    {
        void AddEffectCallback(Action<AbstractEffect> callback);

        double ElapsedMillis();

        void Reset();

        void Tick();

        void Start();

        void Restart();

        void Stop();
    }
}
