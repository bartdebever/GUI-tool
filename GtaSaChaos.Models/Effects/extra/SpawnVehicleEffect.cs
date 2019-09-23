﻿// Copyright (c) 2019 Lordmau5

using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects.extra
{
    internal class SpawnVehicleEffect : AbstractEffect
    {
        private readonly int vehicleID;

        public SpawnVehicleEffect(string word, int _vehicleID)
            : base(Category.Spawning, "Spawn Vehicle", word)
        {
            vehicleID = _vehicleID;

            if (vehicleID == 569)
            {
                vehicleID = 537; // 569 Crashes when being placed as a boat, so replace with 537
            }
        }

        public override string GetDescription()
        {
            string randomVehicle = "Random Vehicle";
            return $"Spawn {((vehicleID == -1) ? randomVehicle : VehicleNames.GetVehicleName(vehicleID))}";
        }

        public override void RunEffect()
        {
            int actualID = vehicleID;
            if (actualID == -1)
            {
                actualID = RandomHandler.Next(400, 611);
            }

            string spawnString = $"Spawn {VehicleNames.GetVehicleName(actualID)}";
            SendEffectToGame("spawn_vehicle", actualID.ToString(), -1, spawnString);
        }
    }
}
