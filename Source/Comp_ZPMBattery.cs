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
    private int lastUnroofWarningTick;

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

        CheckExposedDischarge();
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

    private void CheckExposedDischarge()
    {
        // Only check if battery is charged and can potentially discharge
        if (StoredEnergy <= 0f) return;

        bool isRoofed = parent.Map.roofGrid.Roofed(parent.Position);

        if (!isRoofed)
        {
            // Calculate base discharge chance (you may need to adjust this value)
            float baseDischargeChance = 0.00005f;
            float exposedDischargeChance = baseDischargeChance * ZPMProps.exposedDischargeMultiplier;
            float inducedSolarFlareChance = 0.00025f;
            float inducedFlashStormChance = 0.0025f;

            if (Find.TickManager.TicksGame - this.lastUnroofWarningTick >= 2500 * 6)
            {
                Messages.Message("BRW.ZPM.Discharge.Warning".Translate(parent.LabelShort), MessageTypeDefOf.NeutralEvent);
                this.lastUnroofWarningTick = Find.TickManager.TicksGame;
            }

            if (Rand.Chance(exposedDischargeChance))
            {
                TriggerZPMDischargeIncident();
                TriggerDischarge();
                TriggerZPMInducedSolarFlare();

                return;
            }

            if (this.detectSolarFlare() == false && Rand.Chance(inducedSolarFlareChance))
            {
                TriggerZPMInducedSolarFlare();
                TriggerZPMInducedFlashStorm();

                return;
            }

            if (Rand.Chance(inducedFlashStormChance))
            {
                TriggerZPMInducedFlashStorm();
            }
        }
    }

    private void TriggerZPMInducedFlashStorm()
    {
        var flashstorm = DefDatabase<IncidentDef>.GetNamed("Flashstorm");
        // Skip if this version of Rimowrld doesn't have flashstorms (v1.2).
        if (flashstorm == null)
        {
            return;
        }

        int duration = BetterRandom.pick(1111, 5555);
        string message = "BRW.ZPM.InducedFlashstorm.Title".Translate(duration.ToStringTicksToPeriod());
        Log.Message(message);

        GameCondition flare = GameConditionMaker.MakeCondition(
            flashstorm.gameCondition,
            duration
        );

        parent.Map.gameConditionManager.RegisterCondition(flare);

        Find.LetterStack.ReceiveLetter(
            "BRW.ZPM.InducedFlashstorm.Title".Translate(),
            "BRW.ZPM.InducedFlashstorm".Translate(duration.ToStringTicksToPeriod()),
            LetterDefOf.ThreatBig,
            parent
        );
    }

    private void TriggerZPMInducedSolarFlare()
    {
        // 1. Cause the flare
        int duration = BetterRandom.pick(8 * 2500, 48 * 2500) + BetterRandom.pick(0, 1000);
        GameCondition flare = GameConditionMaker.MakeCondition(
            IncidentDefOf.SolarFlare.gameCondition,
            duration
        );
        Find.World.GameConditionManager.RegisterCondition(flare);


        // 2. Custom explanation letter
        Find.LetterStack.ReceiveLetter(
            "BRW.ZPM.InducedSolarFlare.Title".Translate(),
            "BRW.ZPM.InducedSolarFlare".Translate(duration.ToStringTicksToPeriod()),
            LetterDefOf.ThreatBig,
            parent
        );
    }

    private void TriggerZPMDischargeIncident()
    {
        // Create and trigger the major disaster incident
        string message = "BRW.ZPMDischarge.Letter".Translate();
        IncidentParms parms = new IncidentParms
        {
            target = parent.Map,
            forced = true,
            spawnCenter = parent.Position,
            customLetterText = message,
        };


        IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamed("BRW_ZPMDischarge");
        if (incidentDef.Worker.TryExecute(parms))
        {
            Messages.Message("BRW.ZPM.Meltdown".Translate(), parent, MessageTypeDefOf.ThreatBig);
            Find.WindowStack.Add(new Dialog_MessageBox(message, null, null, null, null, null, true));
        }
    }

    private void TriggerDischarge()
    {
        // Find all connected power conduits and select random discharge location
        IntVec3 dischargeLocation = GetRandomPowerGridLocation();

        // Calculate explosion radius based on dark energy reserve
        float baseRadius = 20f;
        float bonusRadius = darkEnergyReserve / 5000f;
        float totalRadius = baseRadius + bonusRadius;

        // Use the existing battery discharge mechanism
        if (StoredEnergy > 0f)
        {
            // Create the explosion effect at the random grid location
            GenExplosion.DoExplosion(
                center: dischargeLocation,
                map: parent.Map,
                radius: totalRadius,
                damType: DamageDefOf.Bomb,
                instigator: parent,
                damAmount: Mathf.RoundToInt(StoredEnergy / 100f), // Scale damage with stored energy
                armorPenetration: 0.5f,
                weapon: null,
                projectile: null,
                intendedTarget: null,
                postExplosionSpawnThingDef: null,
                postExplosionSpawnChance: 0f,
                postExplosionSpawnThingCount: 0,
                applyDamageToExplosionCellsNeighbors: true,
                preExplosionSpawnThingDef: null,
                preExplosionSpawnChance: 0f,
                preExplosionSpawnThingCount: 0,
                chanceToStartFire: 1f,
                damageFalloff: true
            );

            // Completely drain the battery
            DrawPower(StoredEnergy);

            // Damage the ZPM instead of destroying it (20-95% damage)
            float damagePercent = Rand.Range(0.20f, 0.95f);
            int damageAmount = Mathf.RoundToInt(parent.MaxHitPoints * damagePercent);

            // Apply the damage to the ZPM
            var damageInfo = new DamageInfo(
                DamageDefOf.Bomb,
                damageAmount,
                0f, // armor penetration
                -1f, // angle
                parent, // instigator
                null, // hit part
                null, // weapon
                DamageInfo.SourceCategory.ThingOrUnknown,
                parent
            );

            parent.TakeDamage(damageInfo);

            // Reset dark energy reserve after discharge
            darkEnergyReserve = 0;
        }
    }

    private IntVec3 GetRandomPowerGridLocation()
    {
        // Check if PowerNet exists
        if (PowerNet == null)
        {
            return parent.Position;
        }

        // Collect all transmitter positions (conduits, power sources, etc.)
        List<IntVec3> powerGridPositions = new List<IntVec3>();

        // Add all power transmitters (conduits)
        foreach (var compPower in PowerNet.transmitters)
        {
            if (compPower?.parent?.Position != null)
            {
                powerGridPositions.Add(compPower.parent.Position);
            }
        }

        // Add all power batteries
        foreach (var battery in PowerNet.batteryComps)
        {
            if (battery?.parent?.Position != null)
            {
                powerGridPositions.Add(battery.parent.Position);
            }
        }

        // Add all power generators
        foreach (var generator in PowerNet.powerComps)
        {
            if (generator?.parent?.Position != null && generator.PowerOutput > 0)
            {
                powerGridPositions.Add(generator.parent.Position);
            }
        }

        // Return random position from the power grid, or ZPM position as fallback
        if (powerGridPositions.Count > 0)
        {
            return powerGridPositions.RandomElement();
        }

        return parent.Position;
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
        string darkEnergyString = "BRW.ZPM.DarkEnergyReserve".Translate() + ": " + this.darkEnergyReserve + " / " + this.maxDarkEnergy;

        if (string.IsNullOrEmpty(baseString))
        {
            return darkEnergyString;
        }

        return baseString + "\n" + darkEnergyString;
    }
}

// Custom Incident Worker for ZPM Discharge
public class IncidentWorker_ZPMDischarge : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return true; // Always can fire when called
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;

        string letterText = parms.customLetterText;

        Find.LetterStack.ReceiveLetter(
            "BRW.ZPMDischarge.Title".Translate(),
            letterText,
            LetterDefOf.ThreatBig
        );

        return true;
    }
}
