using System;
using System.Collections.Generic;
using System.Text;
using Models.Core;
using Models.PMF.Phen;

namespace Models.PMF.Functions.DemandFunctions
{
    /// <summary>
    /// Calculates the (actual or potential?) daily demand of biomass to the grain
    /// </summary>
    [Serializable]
    [Description("This must be renamed DMDemandFunction for the source code to recoginise it!!!!.  This function returns the specified proportion of total DM supply.  The organ may not get this proportion if the sum of demands from other organs exceeds DM supply")]
    public class GrainDemandFunction : Model, IFunction
    {
        /// <summary>The filling rate</summary>
        [Link]
        [Units("grains/m2")]
        IFunction NumberFunction = null;
        /// <summary>The plant</summary>
        [Link]
        protected Plant Plant = null;
        /// <summary>The phenology</summary>
        [Link]
        protected Phenology Phenology = null;
        /// <summary>The filling rate</summary>
        [Link]
        [Units("oCd")]
        IFunction FillingDuration = null;
        /// <summary>The maximum weight of an individual grain</summary>
        [Units("g DM/kernal")]
        public double MaximumSize = 0;
        /// <summary>The ripe stage</summary>
        public string RipeStage = "";
        /// <summary>The stage at which biomass accumulation begins in grains</summary>
        public string StartFillStage = "";

        /// <summary>??????</summary>
        /// <value>?????</value>
        [Units("g/m2/day")]
        public double Value
        {
            get
            {
                double Number =  NumberFunction.Value;
                double Demand = 0;
                if ((Number > 0) && (Phenology.Between(StartFillStage, RipeStage)))
                {
                    double FillingRate = (MaximumSize / FillingDuration.Value) * Phenology.ThermalTime.Value;
                    Demand = Number * FillingRate;
                }
                else
                {
                    Demand = 0;
                }
                return Demand;
            }
        }

    }
}


