/*
 * This file is part of ZPM, a Better Rimworlds Project.
 *
 * Copyright © 2021-2025 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *   GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41
 *   https://github.com/BetterRimworlds/ZPM
 *
 * This file is licensed under the Creative Commons No-Derivations v4.0 License.
 * Most rights are reserved.
 */

using RimWorld;
using UnityEngine;
using Verse;

namespace BetterRimworlds.ZPM;

[StaticConstructorOnStartup]
class Building_ZPM : Building
{
    private static Dictionary<string, Graphic> chargeGraphics = new Dictionary<string, Graphic>();

    CompZPMBattery zpmPower = null!;

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
            var request = new GraphicRequest(Type.GetType("Graphic_Single"),
                $"Things/Buildings/ZPM-{powerState}", ShaderDatabase.DefaultShader, new Vector2(1, 2), Color.white,
                Color.white, new GraphicData(), 0, null);
        #else
            var request = new GraphicRequest(Type.GetType("Graphic_Single"),
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
        base.SpawnSetup(map, respawningAfterLoad);
        this.zpmPower = base.GetComp<CompZPMBattery>();
    }

    #endregion

    #region Graphics-text

    public override Graphic Graphic
    {
        get
        {
            var chargePercent = (int) (this.zpmPower.StoredEnergyPct * 100);
            return chargePercent switch
            {
                <= 10 => Building_ZPM.chargeGraphics["Depleted"],
                <= 25 => Building_ZPM.chargeGraphics["25%"],
                <= 50 => Building_ZPM.chargeGraphics["50%"],
                <= 75 => Building_ZPM.chargeGraphics["75%"],
                _ => Building_ZPM.chargeGraphics["Full"]
            };
        }
    }

    #endregion
}
