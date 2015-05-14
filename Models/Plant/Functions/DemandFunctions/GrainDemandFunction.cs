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
    [Description("")]
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
        IFunction InitialFillingDuration = null;
        /// <summary>The filling rate</summary>
        [Link]
        [Units("oCd")]
        IFunction LinearFillingDuration = null;
        /// <summary>The maximum weight of an individual grain</summary>
        [Units("g DM/kernal")]
        public double MaximumSize = 0;
        /// <summary>The ripe stage</summary>
        public string RipeStage = "";
        /// <summary>The stage at which INITIAL (slow) biomass accumulation begins in grains</summary>
        public string StartInitialFillStage = "";
        /// <summary>The stage at which LINEAR (fast) biomass accumulation begins in grains</summary>
        public string StartLinearFillStage = "";
        /// <summary>The filling rate</summary>
        [Link]
        [Units("0-1")]
        [Description("Multiplier of potential individual grain weight to be reached at StartLinearFillStage")]
        IFunction InitialGrainProportion = null;


        /// <summary>??????</summary>
        /// <value>?????</value>
        [Units("g/m2/day")]
        public double Value
        {
            get
            {
                double IntialGrainWeight = InitialGrainProportion.Value * MaximumSize;
                double Number =  NumberFunction.Value;
                double Demand = 0;
                if(((Number > 0) && (Phenology.Between(StartInitialFillStage, StartLinearFillStage))))
                {
                    double FillingRate = (IntialGrainWeight / InitialFillingDuration.Value) * Phenology.ThermalTime.Value;
                    Demand = Number * FillingRate;
                }
                else if ((Number > 0) && (Phenology.Between(StartLinearFillStage, RipeStage)))
                {
                    double FillingRate = (MaximumSize / LinearFillingDuration.Value) * Phenology.ThermalTime.Value;
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


