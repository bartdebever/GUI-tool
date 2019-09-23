﻿// Copyright (c) 2019 Lordmau5

using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.extra
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID)
            : base(Category.Weather, description, word)
        {
            weatherID = _weatherID;
        }

        public override void RunEffect()
        {
            SendEffectToGame("weather", weatherID.ToString());
        }
    }
}
