using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace BetterRimworlds.ZPM
{
    public class WorldObjectCompProperties_EscapeShip : WorldObjectCompProperties
    {
        public WorldObjectCompProperties_EscapeShip()
        {
            compClass = typeof(VoidEscapeShipComp);
        }

        public override IEnumerable<string> ConfigErrors(WorldObjectDef parentDef)
        {
            foreach (string item in base.ConfigErrors(parentDef))
            {
                yield return item;
            }
            if (!typeof(MapParent).IsAssignableFrom(parentDef.worldObjectClass))
            {
                yield return parentDef.defName + " has WorldObjectCompProperties_EscapeShip but it's not MapParent.";
            }
        }
    }


    [StaticConstructorOnStartup]
    public class VoidEscapeShipComp : WorldObjectComp
    {
        public override void PostMapGenerate()
        {
            Building building = ((MapParent)parent).Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.Ship_Reactor).FirstOrDefault();
            Building_ShipReactor building_ShipReactor;
            if (building != null && (building_ShipReactor = (building as Building_ShipReactor)) != null)
            {
                building_ShipReactor.charlonsReactor = true;
                var comp = building_ShipReactor.GetComp<CompHibernatable>();
                if (comp != null)
                {
                    comp.State = HibernatableStateDefOf.Starting;
                }
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_VisitEscapeShip.GetFloatMenuOptions(caravan, (MapParent)parent))
            {
                yield return floatMenuOption;
            }
        }
    }
}