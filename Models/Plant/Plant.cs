using System;
using System.Collections.Generic;
using System.Text;
using Models.Core;

using System.Reflection;
using System.Collections;
using Models.PMF.Organs;
using Models.PMF.Phen;
using Models.Soils;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Models.PMF
{
    /// <summary>
    /// 
    /// </summary>
    public class WaterUptakesCalculatedUptakesType
    {
        /// <summary>The name</summary>
        public String Name = "";
        /// <summary>The amount</summary>
        public Double[] Amount;
    }
    /// <summary>
    /// 
    /// </summary>
    public class WaterUptakesCalculatedType
    {
        /// <summary>The uptakes</summary>
        public WaterUptakesCalculatedUptakesType[] Uptakes;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">The data.</param>
    public delegate void WaterUptakesCalculatedDelegate(WaterUptakesCalculatedType Data);
    /// <summary>
    /// 
    /// </summary>
    public class WaterChangedType
    {
        /// <summary>The delta water</summary>
        public Double[] DeltaWater;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">The data.</param>
    public delegate void WaterChangedDelegate(WaterChangedType Data);
    /// <summary>
    /// 
    /// </summary>
    public class PruneType
    {
        /// <summary>The bud number</summary>
        public Double BudNumber;
    }
    /// <summary>
    /// 
    /// </summary>
    public class KillLeafType
    {
        /// <summary>The kill fraction</summary>
        public Single KillFraction;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="NewCanopyData">The new canopy data.</param>
    public delegate void NewCanopyDelegate(NewCanopyType NewCanopyData);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">The data.</param>
    public delegate void FOMLayerDelegate(FOMLayerType Data);
    /// <summary>
    /// 
    /// </summary>
    public delegate void NullTypeDelegate();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">The data.</param>
    public delegate void NewCropDelegate(NewCropType Data);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">The data.</param>
    public delegate void BiomassRemovedDelegate(BiomassRemovedType Data);
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SowPlant2Type
    {
        /// <summary>The cultivar</summary>
        public String Cultivar = "";
        /// <summary>The population</summary>
        public Double Population = 100;
        /// <summary>The depth</summary>
        public Double Depth = 100;
        /// <summary>The row spacing</summary>
        public Double RowSpacing = 150;
        /// <summary>The maximum cover</summary>
        public Double MaxCover = 1;
        /// <summary>The bud number</summary>
        public Double BudNumber = 1;
        /// <summary>The crop class</summary>
        public String CropClass = "Plant";
        /// <summary>The skip row</summary>
        public Double SkipRow; //Not yet handled in Code
        /// <summary>The skip plant</summary>
        public Double SkipPlant; //Not yet handled in Code
    }
    /// <summary>
    /// 
    /// </summary>
    public class BiomassRemovedType
    {
        /// <summary>The crop_type</summary>
        public String crop_type = "";
        /// <summary>The dm_type</summary>
        public String[] dm_type;
        /// <summary>The dlt_crop_dm</summary>
        public Single[] dlt_crop_dm;
        /// <summary>The DLT_DM_N</summary>
        public Single[] dlt_dm_n;
        /// <summary>The DLT_DM_P</summary>
        public Single[] dlt_dm_p;
        /// <summary>The fraction_to_residue</summary>
        public Single[] fraction_to_residue;
    }
    /// <summary>
    /// 
    /// </summary>
    public class NewCropType
    {
        /// <summary>The sender</summary>
        public String sender = "";
        /// <summary>The crop_type</summary>
        public String crop_type = "";
    }

	/*! <summary>
	The root function of plant model framework.
	</summary>
	\param CropType The crop type defined in the Plant function.
	<remarks>
	
	</remarks>
	*/
    /// <summary>
    /// The generic plant model
    /// </summary>
    [Serializable]
    public class Plant : ModelCollectionFromResource, ICrop2
    {
        #region Class links and lists
        /// <summary>The summary</summary>
        [Link] ISummary Summary = null;
        /// <summary>The phenology</summary>
        [Link(IsOptional = true)] public Phenology Phenology = null;
        /// <summary>The arbitrator</summary>
        [Link(IsOptional = true)] public OrganArbitrator Arbitrator = null;
        /// <summary>The rue model</summary>
        [Link(IsOptional = true)] private Models.PMF.Functions.SupplyFunctions.RUEModel RUEModel = null;
        /// <summary>The structure</summary>
        [Link(IsOptional=true)] public Structure Structure = null;
        /// <summary>The soil</summary>
        [Link] Soils.Soil Soil = null;
        /// <summary>The leaf</summary>
        [Link(IsOptional=true)] public Leaf Leaf = null;
        /// <summary>The root</summary>
        [Link(IsOptional=true)] public Root Root = null;
        #endregion

        #region Class properties and fields
        /// <summary>
        /// MicroClimate will get 'CropType' and use it to look up
        /// canopy properties for this crop.
        /// </summary>
        public string CropType { get; set; }
        /// <summary>The sowing data</summary>
        [XmlIgnore]
        public SowPlant2Type SowingData;
        /// <summary>The _ organs</summary>
        private Organ[] _Organs = null;
        /// <summary>Gets the organs.</summary>
        /// <value>The organs.</value>
        [XmlIgnore]
        public Organ[] Organs 
        { 
            get 

            {
                if (_Organs == null)
                {
                    List<Organ> organs = new List<Organ>();
                    foreach (Organ organ in Apsim.Children(this, typeof(Organ)))
                        organs.Add(organ);
                    _Organs = organs.ToArray();
                }
                return _Organs;
            } 
        }
        /// <summary>Gets the canopy data.</summary>
        /// <value>The canopy data.</value>
        [XmlIgnore]
        public NewCanopyType CanopyData { get { return LocalCanopyData; } }
        /// <summary>The local canopy data</summary>
        [XmlIgnore]
        public NewCanopyType LocalCanopyData;
        /// <summary>The local canopy data2</summary>
        private CanopyProperties LocalCanopyData2;
        /// <summary>The local root data</summary>
        private RootProperties LocalRootData;
        /// <summary>Gets a list of cultivar names</summary>
        public string[] CultivarNames
        {
            get
            {
                SortedSet<string> cultivarNames = new SortedSet<string>();
                foreach (Cultivar cultivar in this.Cultivars)
                {
                    cultivarNames.Add(cultivar.Name);
                    if (cultivar.Aliases != null)
                    {
                        foreach (string alias in cultivar.Aliases)
                            cultivarNames.Add(alias);
                    }
                }

                return new List<string>(cultivarNames).ToArray();
            }
        }
        /// <summary>A property to return all cultivar definitions.</summary>
        /// <value>The cultivars.</value>
        private List<Cultivar> Cultivars
        {
            get
            {
                List<Cultivar> cultivars = new List<Cultivar>();
                foreach (Model model in Apsim.Children(this, typeof(Cultivar)))
                {
                    cultivars.Add(model as Cultivar);
                }

                return cultivars;
            }
        }
        /// <summary>The current cultivar definition.</summary>
        private Cultivar cultivarDefinition;
        /// <summary>MicroClimate needs FRGR.</summary>
        /// <value>The FRGR.</value>
        public double FRGR 
        { 
            get 
            {
                double frgr = 1;
                foreach (Organ Child in Organs)
                {
                    if (Child.FRGR <= 1)
                      frgr = Child.FRGR;
                }
                return frgr; 
            } 
        }
        /// <summary>Root system information</summary>
        /// <value>The root system.</value>
        [XmlIgnore]
        public Models.Soils.RootSystem RootSystem { get { return new Models.Soils.RootSystem(); } }

        /// <summary>MicroClimate supplies light profile.</summary>
        [XmlIgnore]
        public CanopyEnergyBalanceInterceptionlayerType[] LightProfile { get; set; }
        /// <summary>MicroClimate supplies Potential EP</summary>
        /// <value>The potential ep.</value>
        [XmlIgnore]
        public double PotentialEP {get; set;}

        /// <summary>Gets the water supply demand ratio.</summary>
        /// <value>The water supply demand ratio.</value>
        [XmlIgnore]
        public double WaterSupplyDemandRatio
        {
            get
            {
                double F;
                if (demandWater > 0)
                    F = Utility.Math.Sum(uptakeWater) / demandWater;
                else
                    F = 1;
                return F;
            }
        }
        /// <summary>Gets or sets the population.</summary>
        /// <value>The population.</value>
        [XmlIgnore]
        [Description("Number of plants per meter2")]
        [Units("/m2")]
        public double Population { get; set; }
        /// <summary>Gets or sets the plant transpiration.</summary>
        /// <value>The plant transpiration.</value>
        [XmlIgnore]
        public double PlantTranspiration { get; set; }
        #endregion

        #region Interface properties
        /// <summary>Provides canopy data to Arbitrator.</summary>
        public CanopyProperties CanopyProperties { get { return LocalCanopyData2; } }
        /// <summary>Provides root data to Arbitrator.</summary>
        public RootProperties RootProperties { get { return LocalRootData; } }
        /// <summary>
        /// Potential evapotranspiration. Arbitrator calculates this and sets this property in the crop.
        /// </summary>
        [XmlIgnore]
        public double demandWater { get; set; }
        /// <summary>
        /// Actual transpiration by the crop. Calculated by Arbitrator based on PotentialEP across all crops, soil and root properties
        /// </summary>
        [XmlIgnore]
        public double[] uptakeWater { get; set; }
        /// <summary>Crop calculates potentialNitrogenDemand after getting its water allocation</summary>
        [XmlIgnore]
        public double demandNitrogen { get; set; }
        /// <summary>
        /// Arbitrator supplies actualNitrogenSupply based on soil supply and other crop demand
        /// </summary>
        [XmlIgnore]
        public double[] uptakeNitrogen { get; set; }
        /// <summary>The proportion of supplyNitrogen that is supplied as NO3, the remainder is NH4</summary>
        [XmlIgnore]
        public double[] uptakeNitrogenPropNO3 { get;  set; }
        /// <summary>
        /// The initial value of the extent to which the roots have penetrated the soil layer (0-1)
        /// </summary>
        /// <value>The local root exploration by layer.</value>
        [XmlIgnore] public double[] localRootExplorationByLayer { get; set; }
        /// <summary>The initial value of the root length densities for each soil layer (mm/mm3)</summary>
        /// <value>The local root length density by volume.</value>
        [XmlIgnore] public double[] localRootLengthDensityByVolume { get; set; }
        /// <summary>Is the plant in the ground?</summary>
        [XmlIgnore]
        public bool PlantInGround
        {
            get
            {
                return SowingData != null;
            }
        }
        /// <summary>Test if the plant has emerged</summary>
        [XmlIgnore]
        public bool PlantEmerged
        {
            get
            {
                if (Phenology != null)
                    return Phenology.Emerged;
                else
                    return true;
            }
        }


        #endregion

        #region Class Events
        /// <summary>Occurs when [sowing].</summary>
        public event EventHandler Sowing;
        /// <summary>Occurs when [harvesting].</summary>
        public event EventHandler Harvesting;
        /// <summary>Occurs when [cutting].</summary>
        public event EventHandler Cutting;
        /// <summary>Occurs when [plant ending].</summary>
        public event EventHandler PlantEnding;
        /// <summary>Occurs when [biomass removed].</summary>
        public event BiomassRemovedDelegate BiomassRemoved;
        #endregion

        #region External Communications.  Method calls and EventHandlers
        /// <summary>Sow the crop with the specified parameters.</summary>
        /// <param name="Cultivar">The cultivar.</param>
        /// <param name="Population">The population.</param>
        /// <param name="Depth">The depth.</param>
        /// <param name="RowSpacing">The row spacing.</param>
        /// <param name="MaxCover">The maximum cover.</param>
        /// <param name="BudNumber">The bud number.</param>
        /// <param name="CropClass">The crop class.</param>
        public void Sow(string Cultivar, double Population = 1, double Depth = 100, double RowSpacing = 150, double MaxCover = 1, double BudNumber = 1, string CropClass = "Plant")
        {
            SowingData = new SowPlant2Type();
            SowingData.Population = Population;
            SowingData.Depth = Depth;
            SowingData.Cultivar = Cultivar;
            SowingData.MaxCover = MaxCover;
            SowingData.BudNumber = BudNumber;
            SowingData.RowSpacing = RowSpacing;
            SowingData.CropClass = CropClass;

            // Find cultivar and apply cultivar overrides.
            cultivarDefinition = PMF.Cultivar.Find(Cultivars, SowingData.Cultivar);
            cultivarDefinition.Apply(this);



            // Invoke a sowing event.
            if (Sowing != null)
                Sowing.Invoke(this, new EventArgs());

            this.Population = Population;

            // tell all our children about sow
            foreach (Organ Child in Organs)
                Child.OnSow(SowingData);
            if (Structure != null)
               Structure.OnSow(SowingData);
            if (Phenology != null)
            Phenology.OnSow();

            
       
            Summary.WriteMessage(this, string.Format("A crop of " + CropType +" (cultivar = " + Cultivar + " Class = " + CropClass + ") was sown today at a population of " + Population + " plants/m2 with " + BudNumber + " buds per plant at a row spacing of " + RowSpacing + " and a depth of " + Depth + " mm"));
        }
        /// <summary>Harvest the crop.</summary>
        public void Harvest()
        {
            // Invoke a harvesting event.
            if (Harvesting != null)
                Harvesting.Invoke(this, new EventArgs());

            // tell all our children about harvest
            foreach (Organ Child in Organs)
                Child.OnHarvest();

            Phenology.OnHarvest();

            Summary.WriteMessage(this, string.Format("A crop of " + CropType + " was harvested today, Yeahhh"));
        }
        /// <summary>End the crop.</summary>
        public void EndCrop()
        {
            Summary.WriteMessage(this, "Crop ending");

            if (PlantEnding != null)
                PlantEnding.Invoke(this, new EventArgs());

            BiomassRemovedType BiomassRemovedData = new BiomassRemovedType();
            BiomassRemovedData.crop_type = CropType;
            BiomassRemovedData.dm_type = new string[Organs.Length];
            BiomassRemovedData.dlt_crop_dm = new float[Organs.Length];
            BiomassRemovedData.dlt_dm_n = new float[Organs.Length];
            BiomassRemovedData.dlt_dm_p = new float[Organs.Length];
            BiomassRemovedData.fraction_to_residue = new float[Organs.Length];
            int i = 0;
            foreach (Organ O in Organs)
            {
                if (O is AboveGround)
                {
                    BiomassRemovedData.dm_type[i] = O.Name;
                    BiomassRemovedData.dlt_crop_dm[i] = (float)(O.Live.Wt + O.Dead.Wt) * 10f;
                    BiomassRemovedData.dlt_dm_n[i] = (float)(O.Live.N + O.Dead.N) * 10f;
                    BiomassRemovedData.dlt_dm_p[i] = 0f;
                    BiomassRemovedData.fraction_to_residue[i] = 1f;
                }
                else
                {
                    BiomassRemovedData.dm_type[i] = O.Name;
                    BiomassRemovedData.dlt_crop_dm[i] = 0f;
                    BiomassRemovedData.dlt_dm_n[i] = 0f;
                    BiomassRemovedData.dlt_dm_p[i] = 0f;
                    BiomassRemovedData.fraction_to_residue[i] = 0f;
                }
                i++;
            }
            BiomassRemoved.Invoke(BiomassRemovedData);

            // tell all our children about endcrop
            foreach (Organ Child in Organs)
                Child.OnEndCrop();
            Clear();

            cultivarDefinition.Unapply();
        }
        /// <summary>Clears this instance.</summary>
        private void Clear()
        {
            SowingData = null;
            //WaterSupplyDemandRatio = 0;
            Population = 0;
            if (Structure != null)
               Structure.Clear();
            if (Phenology != null)
               Phenology.Clear();
            if (Arbitrator != null)
               Arbitrator.Clear();
        }
        /// <summary>Cut the crop.</summary>
        public void Cut()
        {
            if (Cutting != null)
                Cutting.Invoke(this, new EventArgs());

            // tell all our children about endcrop
            foreach (Organ Child in Organs)
                Child.OnCut();
        }
        /// <summary>Things the plant model does when the simulation starts</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [EventSubscribe("Commencing")]
        private void OnSimulationCommencing(object sender, EventArgs e)
        {
            Clear();
            foreach (Organ o in Organs)
            {
                o.Clear();
            }
            InitialiseInterfaceTypes();
        }
        /// <summary>Things that happen when the clock broadcasts DoPlantGrowth Event</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        
        
        [EventSubscribe("DoPotentialPlantGrowth")]
        private void OnDoPotentialPlantGrowth(object sender, EventArgs e)
        {
            if (PlantInGround)
            {
                
                if (Phenology != null)
                {
                    DoPhenology();
                    //if (Phenology.Emerged == true)
                    //{
                        DoDMSetUp();//Sets organs water limited DM supplys and demands
                    //}
                }
                else
                {
                    DoDMSetUp();//Sets organs water limited DM supplys and demands
                }
            }
        }

        /// <summary>Called when [do water arbitration].</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [EventSubscribe("DoWaterArbitration")]
        private void OnDoWaterArbitration(object sender, EventArgs e) //this should be put into DoWater arbitration to test the effect of the changed order and then replaced by microc climate 
        {

            //Take out water process so arbitrator can do it.
            /* if (Phenology != null)
               if (Phenology.Emerged == true)
               {
                   DoWater();
               }
           else 
                if (InGround == true)
                {
                    DoWater();
                }  */
        }

        /// <summary>Called when [do nutrient arbitration].</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [EventSubscribe("DoNutrientArbitration")]
        private void OnDoNutrientArbitration(object sender, EventArgs e)
        {
            if ((PlantInGround) && (Arbitrator != null))
            {
                //if (Phenology != null)
                //{
                    //if (Phenology.Emerged == true)
                    //{

                        Arbitrator.DoWaterLimitedDMAllocations(Organs);
                        Arbitrator.DoNutrientDemandSetUp(Organs);
                        Arbitrator.SetNutrientUptake(Organs);
                        Arbitrator.DoNutrientAllocations(Organs);
                        Arbitrator.DoNutrientLimitedGrowth(Organs);
                    //}
                //}
                //else
                //{
                //    Arbitrator.DoWaterLimitedDMAllocations(Organs);
                //    Arbitrator.DoNutrientDemandSetUp(Organs);
                //    Arbitrator.SetNutrientUptake(Organs);
                //    Arbitrator.DoNutrientAllocations(Organs);
                //    Arbitrator.DoNutrientLimitedGrowth(Organs);
                //}
            }
        }

        /// <summary>Called when [do actual plant growth].</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [EventSubscribe("DoActualPlantGrowth")]
        private void OnDoActualPlantGrowth(object sender, EventArgs e)
        {
            if (PlantInGround)
            {
                //if (Phenology != null)
                //{
                //    if (Phenology.Emerged == true)
                //        DoActualGrowth();
                //}
                //else
                    DoActualGrowth();
            }
        }
        //[EventSubscribe("DoPlantGrowth")]
        //private void OnDoPlantGrowth(object sender, EventArgs e)
        //{
           //This is an old event handler that needs to be deleted once testing is complete
        //}
        #endregion

        #region Internal Communications.  Method calls
        /// <summary>Does the phenology.</summary>
        private void DoPhenology()
        {
            if (Phenology != null)
                Phenology.DoTimeStep();
        }
        /// <summary>Does the dm set up.</summary>
        private void DoDMSetUp()
        {
            if (Structure != null)
                Structure.DoPotentialDM();
            foreach (Organ o in Organs)
                o.DoPotentialDM();
        }
        /// <summary>Does the nutrient set up.</summary>
        private void DoNutrientSetUp()
        {
            foreach (Organ o in Organs)
                o.DoPotentialNutrient();
        }
        /// <summary>Does the water.</summary>
        private void DoWater()
        {
            double Supply = 0;
            double Demand = 0;
            foreach (Organ o in Organs)
            {
                Supply += o.WaterSupply;
                Demand += o.WaterDemand;
            }

            /*if (Demand > 0)
                WaterSupplyDemandRatio = Supply / Demand;
            else
                WaterSupplyDemandRatio = 1;*/

            double fraction = 1;
            if (Demand > 0)
                fraction = Math.Min(1.0, Supply / Demand);

            foreach (Organ o in Organs)
                if (o.WaterDemand > 0)
                    o.WaterAllocation = fraction * o.WaterDemand;

            double FractionUsed = 0;
            if (Supply > 0)
                FractionUsed = Math.Min(1.0, Demand / Supply);

            foreach (Organ o in Organs)
                o.DoWaterUptake(FractionUsed * Supply);
        }
        /// <summary>Does the actual growth.</summary>
        private void DoActualGrowth()
        {
            if (Structure != null)
                Structure.DoActualGrowth();
            foreach (Organ o in Organs)
                o.DoActualGrowth();
        }
        /// <summary>Initialises the interface types.</summary>
        private void InitialiseInterfaceTypes()
        {
            uptakeWater = new double[Soil.SoilWater.dlayer.Length];
            uptakeNitrogen = new double[Soil.SoilWater.dlayer.Length];
            uptakeNitrogenPropNO3 = new double[Soil.SoilWater.dlayer.Length];

            //Set up CanopyData and root data types
            LocalCanopyData = new NewCanopyType();
            LocalCanopyData2 = new CanopyProperties();
            LocalRootData = new RootProperties();

            CanopyProperties.Name = CropType;
            CanopyProperties.CoverGreen = 0;
            CanopyProperties.CoverTot = 0;
            CanopyProperties.CanopyDepth = 0;
            CanopyProperties.CanopyHeight = 0;
            CanopyProperties.LAIGreen = 0;
            CanopyProperties.LAItot = 0;
            CanopyProperties.Frgr = 0;
            if (Leaf != null)
            {
                CanopyProperties.MaximumStomatalConductance = Leaf.GsMax;
                CanopyProperties.HalfSatStomatalConductance = Leaf.R50;
                CanopyProperties.CanopyEmissivity = Leaf.Emissivity;
            }
            else
            {
                CanopyProperties.MaximumStomatalConductance = 0;
                CanopyProperties.HalfSatStomatalConductance = 0;
                CanopyProperties.CanopyEmissivity = 0;
            }

            SoilCrop soilCrop = this.Soil.Crop(Name) as SoilCrop;

            RootProperties.KL = soilCrop.KL;
            RootProperties.LowerLimitDep = soilCrop.LL;
            RootProperties.RootDepth = 0;
            RootProperties.MaximumDailyNUptake = 0;
            RootProperties.KNO3 = Root.KNO3;
            RootProperties.KNH4 = Root.KNH4;

            localRootExplorationByLayer = new double[Soil.SoilWater.dlayer.Length];
            localRootLengthDensityByVolume = new double[Soil.SoilWater.dlayer.Length];

            demandWater = 0;
            demandNitrogen = 0;

            RootProperties.RootExplorationByLayer = localRootExplorationByLayer;
            RootProperties.RootLengthDensityByVolume = localRootLengthDensityByVolume;
        }

        #endregion
     }
}
