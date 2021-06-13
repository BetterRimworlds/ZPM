using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected static Texture2D UI_POWER_UP;
        protected static Texture2D UI_POWER_DOWN;

        private static Dictionary<string, Graphic> chargeGraphics = new Dictionary<string, Graphic>();

        CompPowerBattery power;

        int currentCapacitorCharge = 7500;
        int maxCapacitorCharge = 20000;

        protected Map currentMap;

        #endregion

        static Building_ZPM()
        {
            UI_POWER_UP = ContentFinder<Texture2D>.Get("UI/PowerUp", true);
            UI_POWER_DOWN = ContentFinder<Texture2D>.Get("UI/PowerDown", true);

            string[] powerStates = { "Depleted", "25%", "50%", "75%", "Full" };
            foreach (var powerState in powerStates)
            {
                var graphic = new Graphic_Single();

                GraphicRequest request = new GraphicRequest(Type.GetType("Graphic_Single"),
                    $"Things/Buildings/ZPM-{powerState}", ShaderDatabase.DefaultShader, new Vector2(1, 2), Color.white,
                    Color.white, new GraphicData(), 0, null);

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
        }

        // protected void BaseTickRare()
        // {
        //     base.TickRare();
        // }
        //
        // public override void TickRare()
        // {
        //     base.TickRare();
        //     if (this.power.PowerOn)
        //     {
        //         // Charge using all the excess energy on the grid.
        //         // Log.Error("Net Power: " + this.power.);
        //         // currentCapacitorCharge += (int) (this.power.PowerNet.CurrentEnergyGainRate() / 1000);
        //     }
        //     
        //     this.power.PowerNet.CurrentEnergyGainRate()
        //
        //     if (currentCapacitorCharge > maxCapacitorCharge)
        //     {
        //         currentCapacitorCharge = maxCapacitorCharge;
        //     }
        //
        //     if (this.currentCapacitorCharge < 0)
        //     {
        //         this.currentCapacitorCharge = 0;
        //         this.chargeSpeed = 0;
        //         this.updatePowerDrain();
        //     }
        // }

        #endregion

        #region Commands

        // protected IEnumerable<Gizmo> GetDefaultGizmos()
        // {
        //     return base.GetGizmos();
        // }
        //
        // public override IEnumerable<Gizmo> GetGizmos()
        // {
        //     // Add the stock Gizmoes
        //     foreach (var g in base.GetGizmos())
        //     {
        //         yield return g;
        //     }
        //
        //     if (true)
        //     {
        //         Command_Action act = new Command_Action();
        //         //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
        //         act.action = () => this.PowerRateIncrease();
        //         act.icon = UI_POWER_UP;
        //         act.defaultLabel = "Increase Power";
        //         act.defaultDesc = "Increase Power";
        //         act.activateSound = SoundDef.Named("Click");
        //         //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
        //         //act.groupKey = 689736;
        //         yield return act;
        //     }
        //
        //     if (true)
        //     {
        //         Command_Action act = new Command_Action();
        //         //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
        //         act.action = () => this.PowerRateDecrease();
        //         act.icon = UI_POWER_DOWN;
        //         act.defaultLabel = "Decrease Power";
        //         act.defaultDesc = "Decrease Power";
        //         act.activateSound = SoundDef.Named("Click");
        //         //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
        //         //act.groupKey = 689736;
        //         yield return act;
        //     }
        // }

        // private void PowerRateIncrease()
        // {
        //     this.chargeSpeed += 1;
        //     this.updatePowerDrain();
        // }

        // private void PowerRateDecrease()
        // {
        //     this.chargeSpeed -= 1;
        //     this.updatePowerDrain();
        // }

        // private void updatePowerDrain()
        // {
        //     this.power.powerOutputInt = -1000 * this.chargeSpeed;
        // }

        #endregion

        #region Graphics-text

        public override Graphic Graphic
        {
            get
            {
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
                + "Capacitor Charge: " + this.currentCapacitorCharge + " / " + this.maxCapacitorCharge;
        }

        #endregion
    }
}
