using System;
using System.Linq;
using LetsGoExplore;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace BetterRimworlds.ZPM
{
    public class SymbolResolver_LostCityBaseLGE : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            //Add rubble
            ResolveParams resolveParamsDebris = rp;
            BaseGen.symbolStack.Push("scatterRubbleLGE", resolveParamsDebris);

            //set up variables for the rooms to skip floors and walls to give the ruined look
            int roomCount = rp.rect.Width * rp.rect.Height / 30;
            ResolveParams resolveParams = rp;
            resolveParams.chanceToSkipWallBlock = 0.38f;
            resolveParams.chanceToSkipFloor = 0.38f;
            resolveParams.noRoof = true;

            for (int i = 0; i < roomCount; i++)
            {
                int width = Rand.RangeInclusive(3, 12);
                int height = Rand.RangeInclusive(3, 12);

                resolveParams.wallStuff = GenStuff.RandomStuffInexpensiveFor(ThingDefOf.Wall, TechLevel.Industrial);
                resolveParams.floorDef = BaseGenUtility.CorrespondingTerrainDef(resolveParams.wallStuff, true);
                resolveParams.rect = new CellRect(Rand.RangeInclusive(rp.rect.minX, rp.rect.maxX - width), Rand.RangeInclusive(rp.rect.minZ, rp.rect.maxZ - height), width, height);
                BaseGen.symbolStack.Push("emptyRoom", resolveParams);
            }

            //Clear out the entire map (including roofs)
            rp.clearRoof = true;
            BaseGen.symbolStack.Push("clear", rp);

        }
    }

    public class SymbolResolver_ScatterSkeletonsLGE : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            //spawn dead dudes
            ResolveParams resolveParamsCorpses = rp;
            int corpseCount = rp.rect.Width * rp.rect.Height / 180; //Adjust as needed. Maybe run symbol multiple times in parent function
            for (int i = 0; i < corpseCount; i++)
            {
                PawnGenerationRequest request = new PawnGenerationRequest(DefsOfLGE.CityDwellerLGE, Faction.OfAncients, PawnGenerationContext.NonPlayer, canGeneratePawnRelations: false, forceGenerateNewPawn: true);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                HealthUtility.DamageUntilDead(pawn);
                pawn.Corpse.GetComp<CompRottable>().RotProgress = 5000000;
                //This corpse is now 10 years old with 36mio ticks
                pawn.Corpse.Age = 36000000;
                resolveParamsCorpses.singleThingToSpawn = pawn.Corpse;
                BaseGen.symbolStack.Push("thing", resolveParamsCorpses);
            }
        }
    }

    public class SymbolResolver_ScatterRubbleLGE : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            ResolveParams resolveParamsDebris = rp;
            resolveParamsDebris.singleThingDef = ThingDefOf.Filth_RubbleBuilding;
            int buildingRubbleCount = rp.rect.Width * rp.rect.Height / 3;
            for (int i = 0; i < buildingRubbleCount; i++)
            {
                BaseGen.symbolStack.Push("thing", resolveParamsDebris);
            }
            resolveParamsDebris.singleThingDef = ThingDefOf.Filth_RubbleRock;
            int slagRubbleCount = rp.rect.Width * rp.rect.Height / 3;
            for (int i = 0; i < slagRubbleCount; i++)
            {
                BaseGen.symbolStack.Push("thing", resolveParamsDebris);
            }
        }
    }

    public class SymbolResolver_ExampleBaselineLostCityLGE : SymbolResolver
    {

        public override void Resolve(ResolveParams rp)
        {
            //spawn rewards
            int rewardCount = Rand.RangeInclusive(5, 6);
            ResolveParams resolveParamsReward = rp;
            for (int i = 0; i < rewardCount; i++)
            {
                int width = Rand.RangeInclusive(6, 9);
                int height = Rand.RangeInclusive(6, 9);

                resolveParamsReward.rect = new CellRect(Rand.RangeInclusive(rp.rect.minX, rp.rect.maxX - width), Rand.RangeInclusive(rp.rect.minZ, rp.rect.maxZ - height), width, height);
                if (Rand.Chance(0.7f))
                {
                    BaseGen.symbolStack.Push("unguardedStockpileLGE", resolveParamsReward);
                }
                else
                {
                    BaseGen.symbolStack.Push("unguardedStockpileLGE", resolveParamsReward);
                }
                
            }

            //spawn dead dudes
            ResolveParams resolveParamsCorpses = rp;
            BaseGen.symbolStack.Push("scatteredSkeletonsLGE", resolveParamsCorpses);

            //City baseline
            ResolveParams resolveParamsCityLandscape = rp;
            BaseGen.symbolStack.Push("lostCityBaseLGE", resolveParamsCityLandscape);

        }
    }
    
    public class SymbolResolver_ToxicFalloutCityBaseLGE : SymbolResolver
    {

        public override void Resolve(ResolveParams rp)
        {
            //Decide wether danger should be Hives or Mechs
            if (Rand.Chance(0.5f)) //is 0.5 usually
            {
                rp.disableHives = true;
            }
            else
            {
                rp.disableHives = false;
            }

            //Spawn shelters
            int shelterCount = Rand.RangeInclusive(5, 6); //4-6 default value
            ResolveParams resolveParamsShelter = rp;
            for (int i = 0; i < shelterCount; i++)
            {
                int width = Rand.RangeInclusive(11, 13);
                int height = Rand.RangeInclusive(9, 13);

                resolveParamsShelter.rect = new CellRect(Rand.RangeInclusive(rp.rect.minX, rp.rect.maxX - width), Rand.RangeInclusive(rp.rect.minZ, rp.rect.maxZ - height), width, height);
                BaseGen.symbolStack.Push("falloutShelterLGE", resolveParamsShelter);
            }

            //spawn fallout victims
            ResolveParams resolveParamsCorpses = rp;
            BaseGen.symbolStack.Push("scatteredFalloutVictimsLGE", resolveParamsCorpses);

            //City baseline
            ResolveParams resolveParamsCityLandscape = rp;
            BaseGen.symbolStack.Push("lostCityBaseLGE", resolveParamsCityLandscape);
        }
    }

    public class SymbolResolver_ScatterFalloutVictimsLGE : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            //spawn dead dudes
            ResolveParams resolveParamsCorpses = rp;
            int corpseCount = rp.rect.Width * rp.rect.Height / 180; //Adjust as needed. Maybe run symbol multiple times in parent function
            for (int i = 0; i < corpseCount; i++)
            {
                PawnGenerationRequest request = new PawnGenerationRequest(DefsOfLGE.CityDwellerLGE, Faction.OfAncients, PawnGenerationContext.NonPlayer, -1, canGeneratePawnRelations: false, forceGenerateNewPawn: true);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, 1);
                //progress just 200k ticks so they are just rotted not sceletons
                pawn.Corpse.GetComp<CompRottable>().RotProgress = 200000;
                pawn.Corpse.Age = 1880000;
                resolveParamsCorpses.singleThingToSpawn = pawn.Corpse;
                BaseGen.symbolStack.Push("thing", resolveParamsCorpses);
            }
        }
    }

    public class SymbolResolver_InfestedCityBaseLGE : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            //spawn insect hives
            ResolveParams resolveParamsHives = rp;
            resolveParamsHives.rect = resolveParamsHives.rect.ContractedBy(15);
            BaseGen.symbolStack.Push("scatterInsectHivesLGE", resolveParamsHives);

            //spawn rewards
            int rewardCount = Rand.RangeInclusive(4, 5);
            ResolveParams resolveParamsReward = rp;
            resolveParamsReward.disableHives = false;
            for (int i = 0; i < rewardCount; i++)
            {
                int width = Rand.RangeInclusive(6, 9);
                int height = Rand.RangeInclusive(6, 9);

                resolveParamsReward.rect = new CellRect(Rand.RangeInclusive(rp.rect.minX, rp.rect.maxX - width), Rand.RangeInclusive(rp.rect.minZ, rp.rect.maxZ - height), width, height);
                BaseGen.symbolStack.Push("unguardedStockpileLGE", resolveParamsReward);
            }

            //spawn skeletons
            ResolveParams resolveParamsCorpses = rp;
            BaseGen.symbolStack.Push("scatteredSkeletonsLGE", resolveParamsCorpses);

            //City baseline
            ResolveParams resolveParamsCityLandscape = rp;
            BaseGen.symbolStack.Push("lostCityBaseLGE", resolveParamsCityLandscape);
        }
    }

    public class SymbolResolver_ScatterInsectHivesLGE : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            //spawn Hives
            ResolveParams resolveParamsHives = rp;
            int hiveCount = rp.rect.Width * rp.rect.Height / 1200; //Adjust as needed. Maybe run symbol multiple times in parent function. It was 800 at first
            IntVec3 loc;
            hiveCount = Math.Min(30, hiveCount);
            for (int i = 0; i < hiveCount; i++)
            {
                if (this.TryFindHivePos(rp.rect, out loc))
                {
                    Thing thingHive = ThingMaker.MakeThing(ThingDefOf.Hive, null);
                    thingHive.SetFaction(Faction.OfInsects, null);
                    Hive hive = (Hive)GenSpawn.Spawn(thingHive, loc, BaseGen.globalSettings.map, WipeMode.Vanish);
                    hive.PawnSpawner.SpawnPawnsUntilPoints(Hive.InitialPawnsPoints);
                }
            }
        }

        private bool TryFindHivePos(CellRect rect, out IntVec3 pos)
        {
            Map map = BaseGen.globalSettings.map;
            return (from mc in rect.Cells
                    where mc.Standable(map)
                    select mc).TryRandomElement(out pos);
        }
    }
    
    public class SymbolResolver_UnguardedStockpileLGE : SymbolResolver
    {

        public override void Resolve(ResolveParams rp)
        {
            //Generate Loot
            ResolveParams resolveParamsReward = rp;
            //Chance for a storage container
            // Scattered Loot
            resolveParamsReward.stockpileConcreteContents = RewardGeneratorUtilityLGE.GenerateStockpileReward();

            resolveParamsReward.rect.ContractedBy(1);
            BaseGen.symbolStack.Push("spawnStockpileLGE", resolveParamsReward);

            //spawn empty room
            BaseGen.symbolStack.Push("emptyRoom", rp);

            //Clear out the entire rect (including roofs)
            rp.clearRoof = true;
            BaseGen.symbolStack.Push("clear", rp);
        }
    }
}