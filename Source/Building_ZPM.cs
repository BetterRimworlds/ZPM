/*
 * This file is part of Uplifted Animals, a Better Rimworlds Project.
 *
 * Copyright © 2021-2024 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *   GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41
 *   https://github.com/BetterRimworlds/ZPM
 *
 * This file is licensed under the Creative Commons No-Derivations v4.0 License.
 * Most rights are reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using RimWorld;

namespace BetterRimworlds.ZPM
{
    [StaticConstructorOnStartup]
    class Building_ZPM : Building
    {

        #region Constants

        const int ADDITION_DISTANCE = 3;

        #endregion

        #region Variables

        private static Dictionary<string, Graphic> chargeGraphics = new Dictionary<string, Graphic>();

        CompPowerBattery power;

        private int darkEnergyReserve = 7500;
        private int maxDarkEnergy = -1;

        protected Map currentMap;

        #endregion

        static Building_ZPM()
        {
            if (Building_ZPM.chargeGraphics.Any())
            {
                return;
            }

            string[] powerStates = { "Depleted", "25%", "50%", "75%", "Full" };
            foreach (var powerState in powerStates)
            {
                var graphic = new Graphic_Single();
#if RIMWORLD12
                GraphicRequest request = new GraphicRequest(Type.GetType("Graphic_Single"),
                    $"Things/Buildings/ZPM-{powerState}", ShaderDatabase.DefaultShader, new Vector2(1, 2), Color.white,
                    Color.white, new GraphicData(), 0, null);
#else
                GraphicRequest request = new GraphicRequest(Type.GetType("Graphic_Single"),
                    $"Things/Buildings/ZPM-{powerState}", ShaderDatabase.DefaultShader, new Vector2(1, 2), Color.white,
                    Color.white, new GraphicData(), 0, null, null);
#endif
                graphic.Init(request);
                chargeGraphics.Add(powerState, graphic);
            }
        }

        #region Override

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            this.currentMap = map;
            base.SpawnSetup(map, respawningAfterLoad);

            this.power = base.GetComp<CompPowerBattery>();
            this.maxDarkEnergy = (int) Math.Ceiling(this.power.Props.storedEnergyMax * 1.25);
        }

        public override void TickRare()
        {
            if (this.power.PowerNet.CurrentEnergyGainRate() > 0.01f)
            {
                // Charge using all the excess energy on the grid.
                darkEnergyReserve += 100;
            }
            
            if (darkEnergyReserve > maxDarkEnergy)
            {
                darkEnergyReserve = maxDarkEnergy;
            }
            
            if (this.power.StoredEnergyPct < 0.75f && darkEnergyReserve >= 1000)
            {
                this.power.AddEnergy(darkEnergyReserve);
                darkEnergyReserve = 0;
            }
            
            // Log.Error("Current Energy Gain Rate: " + this.power.PowerNet.CurrentEnergyGainRate());
            // Log.Error("Stored Energy: " + this.power.StoredEnergy);
            base.TickRare();
        }

        #endregion

        #region Graphics-text

        public override Graphic Graphic
        {
            get
            {
                // For when it's minified or in a trade ship.
                if (this.power == null)
                {
                    return base.DefaultGraphic;
                }
                // var chargePercent = (int) ((float) this.currentCapacitorCharge / (float) this.maxCapacitorCharge) * 100;
                var chargePercent = (int) (this.power.StoredEnergyPct * 100);
                if (chargePercent <= 10)
                {
                    return Building_ZPM.chargeGraphics["Depleted"];
                }
                else if (chargePercent <= 25)
                {
                    return Building_ZPM.chargeGraphics["25%"];
                }
                else if (chargePercent <= 50)
                {
                    return Building_ZPM.chargeGraphics["50%"];
                }
                else if (chargePercent <= 75)
                {
                    return Building_ZPM.chargeGraphics["75%"];
                }
                else
                {
                    return Building_ZPM.chargeGraphics["Full"];
                }
            }
        }
        public override string GetInspectString()
        {
            return base.GetInspectString() + "\n"
                + "Dark Energy Reserve: " + this.darkEnergyReserve + " / " + this.maxDarkEnergy;
        }

        #endregion
    }
}
