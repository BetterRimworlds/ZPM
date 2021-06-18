using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace LetsGoExplore
{
    [DefOf]
    public static class DefsOfLGE
    {
        public static DutyDef DefendAmbrosiaSproutLGE;

        public static PawnKindDef CityDwellerLGE;

        public static ThingDef AncientStorageUnitLGE;

        public static ThingDef BrokenStorageUnitLGE;

        public static ThingDef Plant_MotherAmbrosiaLGE;

        public static JobDef OfferRescueLGE;

        public static DutyDef ChillAsPrisonerLGE;
    }

    [DefOf]
    public static class ThingDefOfVanilla
    {
        public static ThingDef PsychicSoothePulser;

        public static ThingDef PsychicAnimalPulser;

        public static ThingDef PsychicShockLance;

        public static ThingDef PsychicInsanityLance;

        public static ThingDef Ambrosia;

        public static PawnKindDef Warg;

        public static HediffDef AmbrosiaAddiction;
    }

    [DefOf]
    public static class SiteDefOf
    {
        public static SitePartDef ToxicLostCityLGE;

        public static SitePartDef InfestedLostCityLGE;

        public static SitePartDef StandartLostCityLGE;



        public static WorldObjectDef ResearchRequestLGE;

        public static WorldObjectDef InterceptedMessageLGE;

        public static WorldObjectDef PrisonSiteLGE;

        public static WorldObjectDef ShipCoreStartupSiteLGE;

        public static SitePartDef OrbitalBombardmentLGE;
    }
}