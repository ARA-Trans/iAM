using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccessLayer.DTOs;
using System.Data.SqlClient;
using System.IO;

namespace DataAccessLayer
{
    /// <summary>
    /// Fills the Decision Engine tables necessary for an analysis.
    /// </summary>
    public static class PrepareAnalysis
    {
        /// <summary>
        /// Convert SimulationStore into necessary tables entries for RoadCare
        /// </summary>
        /// <param name="simulationID"></param>
        public static void Simulation(string simulationID)
        {
            SimulationStore simulation = SelectScenario.GetSimulationStore(simulationID);
            OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(simulation.Asset);
            PrepareAnalysis.Priorities(simulation);
            //PrepareAnalysis.TargetAndDeficients(simulation);
            PrepareAnalysis.ConditionCategoryWeights(simulation);
            //PrepareAnalysis.Performance(simulation);
            //PrepareAnalysis.RepeatedActivities(simulation);
            //PrepareAnalysis.ActivityFeasibilityCostConsequences(simulation);
            List<AttributeStore> attributes = PrepareAnalysis.Attributes(simulation);
            AssetRequestOMSDataStore assetRequest = new AssetRequestOMSDataStore(DateTime.Now, attributes, oci);
            Dictionary<string, AssetReplyOMSLookupTable> assetLookups = OMS.GetAssetLookupData(assetRequest);
            List<AssetReplyOMSDataStore> assets = OMS.GetAssetData(assetRequest);
            List<AssetReplyOMSDataStore> assetsWithCondition = assets.FindAll(delegate(AssetReplyOMSDataStore arods) { return arods.ConditionIndices != null; });

            PrepareAnalysis.Assets(simulation, assetsWithCondition, attributes, assetRequest, assetLookups);
    
        }


        public static List<AttributeStore> InitializeRoadCareAttributes(List<String> assets, List<String> additionalAttributes)
        {
            DeleteScenario.DeleteAssets("0");
            List<AttributeStore> crossAssets = new List<AttributeStore>();
            foreach (String asset in assets)
            {
                List<AttributeStore> attributes = new List<AttributeStore>();
                OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(asset);
                for (int i = 0; i < oci.ConditionIndexes.Count; i++)
                {
                    string criteria = oci.Weights[i].Criteria;
                    if (criteria != null)
                    {
                        List<AttributeStore> ociCriteriaAttributes = OMS.ParseAttributes(asset, criteria);
                        foreach (AttributeStore attribute in ociCriteriaAttributes)
                        {
                            if (!attributes.Contains(attribute))
                            {
                                attributes.Add(attribute);
                            }
                        }
                    }
                }






                //Get additional data requested.
                List<AttributeStore> attributeAsset = OMS.GetAssetAttributes(asset);
                foreach(String additionalAttribute in additionalAttributes)
                {
                    AttributeStore additional = attributeAsset.Find(delegate(AttributeStore  a) { return a.AttributeField == additionalAttribute; });
                    if (!attributes.Contains(additional))
                    {
                        if (additional != null)
                        {
                            attributes.Add(additional);
                        }
                    }

                    AttributeStore crossAsset = crossAssets.Find(delegate(AttributeStore a) { return a.AttributeField == additionalAttribute; });
                    if(crossAsset == null)
                    {
                        if (additional != null)
                        {
                            crossAssets.Add(additional);
                        }
                    }
                }


                AssetRequestOMSDataStore assetRequest = new AssetRequestOMSDataStore(DateTime.Now, attributes, oci);
                Dictionary<string, AssetReplyOMSLookupTable> assetLookups = OMS.GetAssetLookupData(assetRequest);
                List<AssetReplyOMSDataStore> assetsFromRequest = OMS.GetAssetData(assetRequest);
                List<AssetReplyOMSDataStore> assetsWithCondition = assetsFromRequest.FindAll(delegate(AssetReplyOMSDataStore arods) { return arods.ConditionIndices != null; });

                PrepareAnalysis.Assets("0", assetsWithCondition, attributes, assetRequest, assetLookups,false);
            }

            //Do something with cross attributes.
            AttributeStore assetType = new AttributeStore(null, "AssetType", "AssetType", null);
            assetType.FieldType = "Text";
            crossAssets.Add(assetType);

            AttributeStore overallConditionIndex = new AttributeStore(null, "OverallConditionIndex", "OverallConditionIndex", null);
            overallConditionIndex.FieldType = "Number";
            overallConditionIndex.Minimum = 0;
            overallConditionIndex.Maximum = 100;
            overallConditionIndex.Format = "f1";
            overallConditionIndex.InitialValue = "100";
            overallConditionIndex.Ascending = true;
            crossAssets.Add(overallConditionIndex);
 

            foreach(String asset in assets)
            {
                OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(asset);
            
                foreach(OMSConditionIndexStore ci  in oci.ConditionIndexes)
                {
                    String conditionIndex = "__" + ci.ConditionCategory.Replace(" ", "").Replace("/", "");
                    AttributeStore attributeConditionIndex = crossAssets.Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == conditionIndex; });
                    if (attributeConditionIndex == null)
                    {
                        attributeConditionIndex = new AttributeStore(null, conditionIndex, conditionIndex, null);
                        attributeConditionIndex.FieldType = "Number";
                        crossAssets.Add(attributeConditionIndex);
                    }
                }
            }
            return crossAssets;
        }


        /// <summary>
        /// Fills the Targets and Deficients tables with values from SimulationStore
        /// </summary>
        /// <param name="simulation">SimulationStore for this scenario</param>
        public static void TargetAndDeficients(SimulationStore simulation)
        {
            string simulationID = simulation.SimulationID;
            //Delete previous Targets and deficient.  Recalculate from SimulationStore
            DeleteScenario.DeleteTargets(simulationID);
            //This function will only perform useful work if Deficients are added below.
            DeleteScenario.DeleteDeficients(simulationID);

            try
            {
                //If USE_TARGET then set analysis type to Until Targets and Deficients Met            
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "TARGETS " +
                                         "(SIMULATIONID,ATTRIBUTE_,TARGETMEAN,TARGETNAME)" +
                                          "VALUES('" + simulationID + "','OverallConditionIndex','" + simulation.TargetOCI + "','OMS')";

                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();

                    //Allow determination of OCI limit and possible separate OCI Remaining Life
                    insert = "INSERT INTO " + DB.TablePrefix + "DEFICIENTS " +
                          "(SIMULATIONID,ATTRIBUTE_,DEFICIENTNAME,DEFICIENT,PERCENTDEFICIENT)" +
                           "VALUES('" + simulationID + "','OverallConditionIndex','OCI Deficient','0','0')";
                    cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();      
   
                    //This is so remaining life can be calculated for each individiual condition index
                    OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(simulation.Asset);
                    foreach (OMSConditionIndexStore ci in oci.ConditionIndexes)
                    {
                        insert = "INSERT INTO " + DB.TablePrefix + "DEFICIENTS " +
                                         "(SIMULATIONID,ATTRIBUTE_,DEFICIENTNAME,DEFICIENT,PERCENTDEFICIENT)" +
                                          "VALUES('" + simulationID + "','" + ci.AttributeDE + "','" + ci.AttributeDE + " Deficient','0','0')";
                        cmd = new SqlCommand(insert, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
            }
        }


        public static void ConditionCategoryWeights(SimulationStore simulation)
        {
            List<String> assets = new List<String>();
            assets.Add(simulation.Asset);
            ConditionCategoryWeights(simulation.SimulationID, assets);

        }



        /// <summary>
        /// Gets the OCI weightings from the OMS database and inserts the result into the DecisionEngine OCI_WEIGHTS table
        /// </summary>
        /// <param name="simulation">SimlulationID for which to insert OCI weights</param>
        public static void ConditionCategoryWeights(String simulationID, List<String> assets)
        {
            DeleteScenario.DeleteConditionCategoryWeight(simulationID);
            foreach (String asset in assets)
            {
                OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(asset);

                for (int i = 0; i < oci.ConditionIndexes.Count; i++)
                {
                    string conditionIndex = oci.ConditionIndexes[i].AttributeDE;
                    string weight = oci.Weights[i].Weight.ToString();
                    string criteria = oci.Weights[i].Criteria;

                    if (assets.Count > 1 && criteria != null)
                    {
                        criteria += "[AssetType]=|" + asset + "|";
                    }
                    else if(assets.Count > 1)
                    {
                        criteria = "[AssetType]=|" + asset + "|";
                    }

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                        {
                            connection.Open();
                            string insert = "INSERT INTO " + DB.TablePrefix + "OCI_WEIGHTS (SIMULATIONID,CONDITION_CATEGORY,WEIGHT,CRITERIA) VALUES(@simulationID,@conditionIndex,@weight,@criteria)";
                            SqlCommand cmd = new SqlCommand(insert, connection);
                            cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                            cmd.Parameters.Add(new SqlParameter("conditionIndex", "__" + conditionIndex.Replace(" ", "").Replace("/", "")));
                            cmd.Parameters.Add(new SqlParameter("weight", weight));
                            if (criteria == null)
                                cmd.Parameters.Add(new SqlParameter("criteria", DBNull.Value));
                            else
                                cmd.Parameters.Add(new SqlParameter("criteria", criteria));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                    }
                }
            }
        }


        private static void Performance(SimulationStore simulation)
        {
            List<String> assets = new List<String>();
            assets.Add(simulation.Asset);
            PrepareAnalysis.Performance(simulation.SimulationID, assets);
        }

        /// <summary>
        /// Insert the Prediction curve from PrectionGroups and PredictionPoints into PERFORMANCE
        /// </summary>
        /// <param name="simulation">Simulation for which to build performance curves for.</param>
        public static void Performance(String simulationID, List<String> assets )
        {
            DeleteScenario.DeletePerformance(simulationID);

            foreach (String asset in assets)
            {
                List<OMSPrediction> predictions = OMS.GetPredictionsCurves(asset);

                foreach (OMSPrediction prediction in predictions)
                {
                    foreach (OMSCategoryPrediction conditionIndex in prediction.CategoryPredictions)
                    {
                        string attribute = "__" + conditionIndex.ConditionCategory.Replace(" ", "").Replace("/", "");
                        string equationName = prediction.PredictionGroupName;
                        string equation = conditionIndex.GetPiecewiseCurve();
                        string criteria = prediction.PredictionFilter;
                        bool piecewise = true;
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                            {
                                connection.Open();
                                string insert = "INSERT INTO " + DB.TablePrefix + "PERFORMANCE " +
                                                     "(SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,PIECEWISE)" +
                                                      "VALUES('" + simulationID + "','" + attribute + "','" + equationName + "','" + criteria + "','" + equation + "','" + piecewise.ToString() + "')";

                                SqlCommand cmd = new SqlCommand(insert, connection);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                        }
                    }
                }
            }
            
        }


        private static void RepeatedActivities(SimulationStore simulation)
        {
            // Delete previous repeated activities
            DeleteScenario.DeleteRepeatedActivities(simulation.SimulationID);
            //Insert current repeated activities
            

        }

        /// <summary>
        /// This function for testing purposes.   This should be filled by the interface.
        /// </summary>
        /// <param name="simulation"></param>
        private static void ActivityFeasibilityCostConsequences(SimulationStore simulation)
        {
            if(!IsTreamentsDefined(simulation.SimulationID))
            {
                List<OMSActivityStore> activities = OMS.GetActivities(simulation.Asset);
                if (activities != null) 
                {
                    foreach (OMSActivityStore activity in activities)
                    {
                        CreateScenario.AddActivity(activity,simulation);
                    }
                }
            }
        }





        private static void Priorities(SimulationStore simulation)
        {
            DeleteScenario.DeletePriority(simulation.SimulationID);

            int identity = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string insert = "INSERT INTO " + DB.TablePrefix + "PRIORITY " +
                                            "(SIMULATIONID,PRIORITYLEVEL)" +
                                            "VALUES('" + simulation.SimulationID + "','1')";

                    SqlCommand cmd = new SqlCommand(insert, connection);
                    cmd.ExecuteNonQuery();

                    string table = DB.TablePrefix + "PRIORITY";
                    string selectIdentity = "SELECT IDENT_CURRENT ('" + table + "') FROM " + table;
                    cmd = new SqlCommand(selectIdentity, connection);
                    identity = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
            catch (Exception ex)
            {
                identity = 0;
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
            }

            foreach(CategoryBudgetStore budget in simulation.CategoryBudgets)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        connection.Open();
                        string insert = "INSERT INTO " + DB.TablePrefix + "PRIORITYFUND " +
                                                "(PRIORITYID,BUDGET,FUNDING)" +
                                                "VALUES('" + identity + "','" + budget.Key +"','100')";

                        SqlCommand cmd = new SqlCommand(insert, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    identity = 0;
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                }
            }
            
        }

        /// <summary>
        /// Used to create OMS_ATTRIBUTES for standalone RoadCare
        /// </summary>
        /// <param name="simulationID"></param>
        /// <param name="assets"></param>
        /// <returns></returns>
        public static void Attributes(String simulationID)
        {
            DeleteScenario.DeleteAttributes(simulationID);
            List<AttributeStore> attributes = new List<AttributeStore>();
            try
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string select = "SELECT ATTRIBUTE_, TYPE_, DEFAULT_VALUE, MINIMUM_, MAXIMUM, ASCENDING, FORMAT FROM ATTRIBUTES_";
                    SqlCommand cmd = new SqlCommand(select, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while(dr.Read())
                    {
                        AttributeStore attribute = new AttributeStore();
                        attribute.AttributeField = Convert.ToString(dr["ATTRIBUTE_"]);
                        attribute.FieldType = Convert.ToString(dr["TYPE_"]);
                        attribute.InitialValue = Convert.ToString(dr["DEFAULT_VALUE"]);
                        attribute.Minimum = 0;
                        attribute.Maximum = 0;
                        attribute.Format = null;
                        attribute.Ascending = true;
                        
                        if (attribute.AttributeField.Substring(0, 2) == ("__")) attribute.IsConditionCategory = true;
                        else attribute.IsConditionCategory = false;

                        if (dr["MINIMUM_"] != DBNull.Value) attribute.Minimum = Convert.ToSingle(dr["MINIMUM_"]);
                        if (dr["MAXIMUM"] != DBNull.Value) attribute.Maximum = Convert.ToSingle(dr["MAXIMUM"]);
                        if (dr["FORMAT"] != DBNull.Value) attribute.Format = Convert.ToString(dr["FORMAT"]);
                        
                        attributes.Add(attribute);
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Error getting attributes." + ex.Message);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
            
                    foreach(AttributeStore attribute in attributes)
                    { 
                        string insert = "INSERT INTO " + DB.TablePrefix + "OMS_ATTRIBUTES " +
                                             "(SIMULATIONID,ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT,IS_CONDITION_INDEX,ATTRIBUTE_OMS) " +
                                              "VALUES(@simulationID,@attribute,@type,@defaultValue,@minimum,@maximum,@ascending,@format,@isConditionCategory,@omsHierarchy)";

                        SqlCommand cmd = new SqlCommand(insert, connection);
                        cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                        cmd.Parameters.Add(new SqlParameter("attribute", attribute.AttributeField));
                        cmd.Parameters.Add(new SqlParameter("type", attribute.FieldType));
                        cmd.Parameters.Add(new SqlParameter("defaultValue", attribute.InitialValue));
                        cmd.Parameters.Add(new SqlParameter("minimum", attribute.Minimum));
                        cmd.Parameters.Add(new SqlParameter("maximum", attribute.Maximum));
                        cmd.Parameters.Add(new SqlParameter("ascending", attribute.Ascending));
                        
                        if (attribute.Format == null) cmd.Parameters.Add(new SqlParameter("format", DBNull.Value));
                        else cmd.Parameters.Add(new SqlParameter("format", attribute.Format));

                        cmd.Parameters.Add(new SqlParameter("isConditionCategory", attribute.IsConditionCategory));
                        cmd.Parameters.Add(new SqlParameter("omsHierarchy", attribute.AttributeField));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Error writing OMS_ATTRIBUTES." + ex.Message);
            }
        }

        public static void SetSimulationArea(String simulationID, String area)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    connection.Open();
                    string update = "UPDATE SIMULATIONS SET SIMULATION_AREA=@area WHERE SIMULATIONID=@simulationID";
                    
                    SqlCommand cmd = new SqlCommand(update, connection);
                    cmd.Parameters.Add(new SqlParameter("simulationID", simulationID));
                    cmd.Parameters.Add(new SqlParameter("area", area));
                    cmd.ExecuteNonQuery();
                    
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Write("Error setting simulation area. " + e.Message);
            }
        }

        


        private static List<AttributeStore> Attributes(SimulationStore simulation)
        {
            DeleteScenario.DeleteAttributes(simulation.SimulationID);
            
            List<AttributeStore> attributes = DecisionEngine.GetTreatmentAttributes(simulation.Asset, simulation.SimulationID);


            List<AttributeStore> attributePriority = DecisionEngine.GetPriorityAttributes(simulation.Asset, simulation.SimulationID);
            foreach (AttributeStore attribute in attributePriority)
            {
                if (!attributes.Contains(attribute))
                {
                    attributes.Add(attribute);
                }
            }

            List<AttributeStore> attributePerformance = DecisionEngine.GetPerformanceAttributes(simulation.Asset, simulation.SimulationID);
            foreach (AttributeStore attribute in attributePerformance)
            {
                if (!attributes.Contains(attribute))
                {
                    attributes.Add(attribute);
                }
            }

            List<AttributeStore> attributeArea = OMS.ParseAttributes(simulation.Asset, simulation.SimulationArea);
            foreach (AttributeStore attribute in attributeArea)
            {
                if (!attributes.Contains(attribute))
                {
                    attributes.Add(attribute);
                }
            }

            //Retrieve a list of attributes that are marked as condition categories.
            List<AttributeStore> conditionCategoryAttributes = OMS.GetAssetAttributes(simulation.Asset).FindAll(delegate(AttributeStore a) { return a.IsConditionCategory == true; });
            foreach (AttributeStore attribute in conditionCategoryAttributes)
            {
                if (!attributes.Contains(attribute))
                {
                    attributes.Add(attribute);
                }
            }


            OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(simulation.Asset);
            for (int i = 0; i < oci.ConditionIndexes.Count; i++)
            {
                string criteria = oci.Weights[i].Criteria;
                if (criteria != null)
                {
                    List<AttributeStore> ociCriteriaAttributes = OMS.ParseAttributes(simulation.Asset, criteria);
                    foreach (AttributeStore attribute in ociCriteriaAttributes)
                    {
                        if (!attributes.Contains(attribute))
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
            }


            //Adds attributes for JurisdictionFilter
            List<AttributeStore> jurisdictionAttributes = OMS.ParseAttributes(simulation.Asset, simulation.Jurisdiction);
            foreach (AttributeStore attribute in jurisdictionAttributes)
            {
                if (!attributes.Contains(attribute))
                {
                    attributes.Add(attribute);
                }
            }


            //Add street for output display
            AttributeStore attributeStreet = OMS.GetAssetAttributes(simulation.Asset).Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == "Street"; });
            if (attributeStreet != null && !attributes.Contains(attributeStreet)) attributes.Add(attributeStreet);

            //Add ID for output display
            AttributeStore attributeID = OMS.GetAssetAttributes(simulation.Asset).Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == "ID"; });
            if (attributeID != null && !attributes.Contains(attributeID)) attributes.Add(attributeID);

            //Add Installed for calculation of AGE
            AttributeStore attributeInstalled = OMS.GetAssetAttributes(simulation.Asset).Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == "Installed"; });
            if (attributeInstalled != null && !attributes.Contains(attributeInstalled)) attributes.Add(attributeInstalled);

            //Add Replaced for calculation of AGE
            AttributeStore attributeReplaced = OMS.GetAssetAttributes(simulation.Asset).Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == "Replaced"; });
            if (attributeReplaced != null && !attributes.Contains(attributeReplaced)) attributes.Add(attributeReplaced);

            //Add gis shape for output display
            AttributeStore attributecgShape = OMS.GetAssetAttributes(simulation.Asset).Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == "cgShape"; });
            if (attributecgShape != null && !attributes.Contains(attributecgShape)) attributes.Add(attributecgShape);


            foreach (AttributeStore attribute in attributes)
            {
                string simulationID = simulation.SimulationID;
                string attribute_ = attribute.OmsObjectUserIDHierarchy;
                if (attribute.IsConditionCategory && attribute_ != "OverallConditionIndex") attribute_ = "__" + attribute_.Replace(" ", "").Replace("/", "");
                string type_ = attribute.FieldType;
                string default_value="";
                string maximum = "";
                string minimum = "";
                bool ascending = attribute.Ascending;
                string format = "";
                string isConditionCategory = attribute.IsConditionCategory.ToString(); ;

                if (attribute.InitialValue != null) default_value = attribute.InitialValue;
                if (!float.IsNaN(attribute.Minimum)) minimum = attribute.Minimum.ToString();
                if (!float.IsNaN(attribute.Maximum)) maximum = attribute.Maximum.ToString();
                if (attribute.Format != null) format = attribute.Format;


                switch (type_)
                {

                    case "Time":
                    case "Date":
                    case "DateTime":
                        type_ = "DATETIME";
                        break;
                    case "YesNo":
                    case "Text":
                    case "Lookup":
                    case "Quantity.unit":
                        type_ = "STRING";
                        break;
                    case "Number":
                    case "Integer":
                    case "Quantity":
                    case "Currency":
                        type_ = "NUMBER";
                        break;

                    default:
                        break;

               }

                try
                {
                    using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                    {
                        connection.Open();
                        string insert = "INSERT INTO " + DB.TablePrefix + "OMS_ATTRIBUTES " +
                                             "(SIMULATIONID,ATTRIBUTE_,TYPE_,DEFAULT_VALUE,MINIMUM_,MAXIMUM,ASCENDING,FORMAT,IS_CONDITION_INDEX,ATTRIBUTE_OMS)" +
                                              "VALUES('" + simulationID + "','" + attribute_ + "','" + type_ + "','" + default_value + "','" + minimum + "','" + maximum + "','" + ascending.ToString() + "','" + format + "','" + isConditionCategory + "','" + attribute.OmsHierarchy +"')";

                        SqlCommand cmd = new SqlCommand(insert, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                }
            }
            return attributes;
        }


        private static void Assets(SimulationStore simulation,  List<AssetReplyOMSDataStore> assets, List<AttributeStore> attributes, AssetRequestOMSDataStore assetRequest, Dictionary<string, AssetReplyOMSLookupTable> assetLookups)
        {
            PrepareAnalysis.Assets(simulation.SimulationID, assets, attributes, assetRequest, assetLookups, true);
        }


        private static void Assets(String simulationID,  List<AssetReplyOMSDataStore> assets, List<AttributeStore> attributes, AssetRequestOMSDataStore assetRequest, Dictionary<string, AssetReplyOMSLookupTable> assetLookups, Boolean isDelete)
        {
            if (isDelete)
            {
                DeleteScenario.DeleteAssets(simulationID);
            }
            string path = Path.GetTempPath()+"DecisionEngine";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "\\oms_assets_" + simulationID + ".txt";
            TextWriter tw = null;
            try
            {
                tw = new StreamWriter(path);
                foreach (AssetReplyOMSDataStore asset in assets)
                {
                    List<AttributeValueDEStore> values = asset.GetDecisionEngineAttributes(attributes, assetRequest, assetLookups);

                    //JWC_PROJECT
                    //Only include non-retired projects. If retired is non-null (ie has date), don't import it.
                    AttributeValueDEStore retired = values.Find(delegate(AttributeValueDEStore a) { return a.AttributeField == "Retired"; });
                    if (retired != null && retired.Value == null)
                    {

                        foreach (AttributeValueDEStore value in values)
                        {
                            string row;
                            if (value.AttributeField != null)
                            {
                                row = "\t" + simulationID + "\t" + asset.OID + "\t" + value.AttributeField + "\t" + value.LastEntry.ToString() + "\t";
                            }
                            else
                            {
                                row = "\t" + simulationID + "\t" + asset.OID + "\t" + value.OmsObjectUserIDHierarchy + "\t" + value.LastEntry.ToString() + "\t";
                            }

                            if (value.Value != null)
                            {
                                row += value.Value.ToString();
                            }
                            tw.WriteLine(row);

                            if (value.OmsObjectUserIDHierarchy == "ID")
                            {
                                row = "\t" + simulationID + "\t" + asset.OID + "\tAssetType\t" + value.LastEntry.ToString() + "\t" + attributes[0].AssetType;
                                tw.WriteLine(row);
                            }

                            //JWC_PROJECT
                            //Import for case where user has entered an estimated OCI of 0.
                            //Need to add virtual condition indexes with all zeros so model works properly.
                            if (value.OmsObjectUserIDHierarchy == "EstimatedOCI")
                            {
                                if (value.Value != null)
                                {
                                    double estimatedOCI = Convert.ToDouble(value.Value);
                                    if (estimatedOCI == 0)
                                    {
                                        foreach (AssetReplyOMSConditionIndex conditionIndex in asset.ConditionIndices)
                                        {
                                            string ci = "__" + conditionIndex.ConditionIndex.Replace(" ", "").Replace("/", "");
                                            row = "\t" + simulationID + "\t" + asset.OID + "\t" + ci + "\t" + DateTime.Now.ToString() + "\t0";
                                            tw.WriteLine(row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                tw.Close();
            }
            catch(Exception e)
            {

            }
            finally
            {
                if (tw != null) tw.Close();
            }

            DB.SQLBulkLoad(DB.TablePrefix + "OMS_ASSETS", path, '\t');
        }

        private static bool IsTreamentsDefined(string simulationID)
        {
            bool isTreatmentDefined = false;
            using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM " + DB.TablePrefix + "TREATMENTS WHERE SIMULATIONID = '" + simulationID + "'";
                SqlCommand cmd = new SqlCommand(query, connection);
                int numberTreatments = Convert.ToInt32(cmd.ExecuteScalar());
                if (numberTreatments > 0)
                {
                    isTreatmentDefined = true;
                }
            }
            return isTreatmentDefined;
        }
    }
}
