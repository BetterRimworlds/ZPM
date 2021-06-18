using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetsGoExplore;
using Verse;

/**
 * This file was extended from Let's Go Explore, licensed CC-NC-SA v4.0 International
 * @see https://github.com/SaintBenedict/ModPack-RW-1.1/blob/master/%23LC_Go_Explore/LICENSE
 */
namespace BetterRimworlds.ZPM
{
    // Token: 0x02000352 RID: 850
    public class IncidentWorker_QuestFoundZPM : IncidentWorker
    {
        private static readonly IntRange TimeoutDaysRange = new IntRange(30, 120);
        private static readonly int minDist = 15;
        private static readonly int maxDist = 50;

        public static Site CreateInfestedCitySite(int tile, int days)
        {
            Site site = SiteMaker.MakeSite(SiteDefOf.InfestedLostCityLGE, tile: tile, faction: Faction.OfAncients);
            site.GetComponent<TimeoutComp>().StartTimeout(days * 60000);
            Find.WorldObjects.Add(site);
            return site;
        }

        public static Site CreateToxicCitySite(int tile, int days)
        {
            Site site = SiteMaker.MakeSite(SiteDefOf.ToxicLostCityLGE, tile: tile, faction: Faction.OfAncients);
            site.GetComponent<TimeoutComp>().StartTimeout(days * 60000);
            Find.WorldObjects.Add(site);
            return site;
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int tile;

            return base.CanFireNowSub(parms) && TileFinder.TryFindNewSiteTile(out tile, minDist, maxDist, false, true, -1);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            int tile;
            if (!TileFinder.TryFindNewSiteTile(out tile, minDist, maxDist, false, true, -1))
            {
                return false;
            }

            if (Rand.Chance(0.5f))
            {
                Site site = CreateInfestedCitySite(tile, TimeoutDaysRange.RandomInRange);
                Find.LetterStack.ReceiveLetter("LetterLabelInfestedLostCityLGE".Translate(), "LetterInfestedLostCityLGE".Translate(), LetterDefOf.PositiveEvent, site, null);
            }
            else
            {
                Site site = CreateToxicCitySite(tile, TimeoutDaysRange.RandomInRange);
                Find.LetterStack.ReceiveLetter("LetterLabelToxicLostCityLGE".Translate(), "LetterToxicLostCityLGE".Translate(), LetterDefOf.PositiveEvent, site, null);
            }
            
            return true;
        }
    }
}
