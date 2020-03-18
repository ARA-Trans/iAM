using System;
using System.Collections.Generic;
using System.Text;

namespace ARA.iAM.Analysis.Interfaces
{
    interface ICriteriaDriven
    {
         Criterion Criterion { get; }
    }

    static class CriteriaDrivenExtensions
    {
        /// <summary>
        /// Determines whether the provided criteria-driven analysis setting applies to
        /// the provided asset
        /// </summary>
        /// <param name="criteriaDriven">A criteria-driven analysis setting</param>
        /// <param name="asset">An inventory asset</param>
        /// <returns>true if and only if the setting applies</returns>
        public static bool ApplicableToAsset(this ICriteriaDriven criteriaDriven, Asset asset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given an enumerable of assets, returns those assets for which this criteria-driven analysis setting
        /// applies.
        /// </summary>
        /// <param name="criteriaDriven">A criteria-driven analysis setting</param>
        /// <param name="assets">Inventory assets</param>
        /// <returns>Valid inventory assets</returns>
        public static IEnumerable<Asset> FilterAssets(this ICriteriaDriven criteriaDriven, IEnumerable<Asset> assets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given an enumerable of criteria-driven settings, returns those settings which apply to the
        /// provided asset
        /// </summary>
        /// <typeparam name="T">A criteria-driven analysis setting type</typeparam>
        /// <param name="criteriaDrivenSettings">Criteria-driven settings</param>
        /// <param name="asset">Inventory asset</param>
        /// <returns>Applicable criteria-driven settings</returns>
        public static IEnumerable<T> ApplicableTo<T>(this IEnumerable<T> criteriaDrivenSettings, Asset asset) where T: ICriteriaDriven
        {
            throw new NotImplementedException();
        }
    }
}
