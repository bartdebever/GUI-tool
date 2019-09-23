﻿// Copyright (c) 2019 Lordmau5

using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.impl
{
    public class FunctionEffect : AbstractEffect
    {
        private readonly string type;
        private readonly string function;
        private readonly int duration;
        private readonly int multiplier;

        public FunctionEffect(Category category, string description, string word, string _type, string _function, int _duration = -1, int _multiplier = 1)
            : base(category, description, word)
        {
            type = _type;
            function = _function;
            duration = _duration;
            multiplier = _multiplier;
        }

        public override void RunEffect()
        {
            SendEffectToGame("set_seed", RandomHandler.Next(9999999).ToString());
            SendEffectToGame("cryptic_effects", (Config.Instance.CrypticEffects ? 1 : 0).ToString());
            SendEffectToGame(type, function, duration, "", multiplier);
        }
    }
}
