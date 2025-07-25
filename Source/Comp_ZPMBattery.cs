/*
 * This file is part of ZPM, a Better Rimworlds Project.
 *
 * Copyright © 2025 Theodore R. Smith
 * Author: Theodore R. Smith <hopeseekr@gmail.com>
 *   GPG Fingerprint: D8EA 6E4D 5952 159D 7759  2BB4 EEB6 CE72 F441 EC41
 *   https://github.com/BetterRimworlds/ZPM
 *
 * This file is licensed under the Creative Commons No-Derivations v4.0 License.
 * Most rights are reserved.
 */

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterRimworlds.ZPM;

// Custom CompProperties for the ZPM battery
public class CompProperties_ZPMBattery : CompProperties_Battery
{
    public float exposedDischargeMultiplier = 22f;

    public CompProperties_ZPMBattery()
    {
        compClass = typeof(CompZPMBattery);
    }
}

// Custom battery component for ZPM
public class CompZPMBattery : CompPowerBattery
{
    private CompProperties_ZPMBattery ZPMProps => (CompProperties_ZPMBattery)props;

    private int darkEnergyReserve = 7500; // Starting dark energy reserve
    private int maxDarkEnergy = -1; // Will be calculated based on storedEnergyMax

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        // Calculate max dark energy based on stored energy max (125% of battery capacity)
        if (maxDarkEnergy == -1)
        {
            maxDarkEnergy = (int)Math.Ceiling(Props.storedEnergyMax * 1.25f);
        }
    }

    private bool detectSolarFlare()
    {
        var solarFlareDef = IncidentDefOf.SolarFlare.gameCondition;
        bool isSolarFlare = Find.World.GameConditionManager.ConditionIsActive(solarFlareDef);

        return isSolarFlare;
    }

    public override void CompTickRare()
    {
        base.CompTickRare();

        // Handle dark energy charging and discharging
        HandleDarkEnergyMechanics();
    }

    private void HandleDarkEnergyMechanics()
    {
        // Check if PowerNet exists before accessing it
        if (PowerNet == null) return;

        // Charge dark energy when there's excess power on the grid
        if (PowerNet.CurrentEnergyGainRate() > 0.01f)
        {
            darkEnergyReserve += 100;
        }

        // Cap dark energy at maximum
        if (darkEnergyReserve > maxDarkEnergy)
        {
            darkEnergyReserve = maxDarkEnergy;
        }

        // Auto-discharge dark energy when battery drops below 75%
        if (StoredEnergyPct < 0.75f && darkEnergyReserve >= 1000)
        {
            AddEnergy(1000f);
            darkEnergyReserve -= 1000;
        }
    }

    // Save/Load the dark energy reserve
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref darkEnergyReserve, "darkEnergyReserve", 7500); // Use starting value as default
        Scribe_Values.Look(ref maxDarkEnergy, "maxDarkEnergy", -1);
    }

    public override string CompInspectStringExtra()
    {
        string baseString = base.CompInspectStringExtra();
        string darkEnergyString = "Dark Energy Reserve: " + this.darkEnergyReserve + " / " + this.maxDarkEnergy;

        if (string.IsNullOrEmpty(baseString))
        {
            return darkEnergyString;
        }

        return baseString + "\n" + darkEnergyString;
    }
}
