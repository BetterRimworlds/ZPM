using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BetterRimworlds.ZPM
{
    public class RewardGeneratorUtilityLGE
    {
        public static List<Thing> GenerateStockpileReward()
        {
            List<Thing> returnList = new List<Thing>();

            // Definitely add the ZPM.
            returnList.Add(MinifyUtility.MakeMinified(new Building_ZPM()));

            List<ThingDef> potentialList = new List<ThingDef>();
            potentialList.Add(ThingDefOf.TechprofSubpersonaCore);
            potentialList.Add(ThingDefOf.AIPersonaCore);
            potentialList.Add(ThingDefOf.PsychicEmanator);
            potentialList.Add(ThingDefOf.InfiniteChemreactor);
            potentialList.Add(ThingDefOf.VanometricPowerCell);
            
            if (true)
            {
                // Spawn Artifacts
                ThingDef rewardDef;
                for (int i = 0; i < potentialList.Count(); i++)
                {
                    potentialList.TryRandomElement(out rewardDef);
                    if (rewardDef != null)
                    {
                        returnList.Add(ThingMaker.MakeThing(rewardDef));
                    }
                }
            }

            // Spawn adv Components
            int componentStackCount = Rand.RangeInclusive(1, 3);
            for (int i = 0; i < componentStackCount; i++)
            {
                Thing reward = ThingMaker.MakeThing(ThingDefOf.ComponentSpacer);
                reward.stackCount = Rand.RangeInclusive(3, 7);
                returnList.Add(reward);
            }

            // Add random exquisite resources.
            if (true)
            {
                List<ThingDef> resourcesList = new List<ThingDef>();
                resourcesList.Add(ThingDefOf.Uranium);
                resourcesList.Add(ThingDefOf.Gold);
                resourcesList.Add(ThingDefOf.Silver);

                ThingDef rewardDef;
                resourcesList.TryRandomElement(out rewardDef);
                int StackCount = Rand.RangeInclusive(4, 5);
                for (int a = 0; a < 3; ++a)
                {
                    Thing reward = ThingMaker.MakeThing(rewardDef);
                    reward.stackCount = Rand.RangeInclusive(15, 35);
                    // Larger Stackcount for Silver Stacks
                    if(rewardDef == ThingDefOf.Silver)
                    {
                        reward.stackCount = Rand.RangeInclusive(100, 280);
                    }

                    returnList.Add(reward);
                }
            }

            return returnList;
        }
    }

    public class IncidentUtilityLGE
    {
        public static List<Thing> GenerateShellStocks(int stockpileCount)
        {
            List<Thing> returnList = new List<Thing>();

            for (int i = 0; i < stockpileCount; i++)
            {
                Thing shell = ThingMaker.MakeThing(ThingDefOf.Shell_HighExplosive);
                shell.stackCount = Rand.RangeInclusive(10, 20);
                returnList.Add(shell);
            }

            return returnList;
        }
    }
}