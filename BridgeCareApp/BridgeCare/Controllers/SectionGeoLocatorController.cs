using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class SectionGeoLocatorController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ISectionGeoLocator LatitudeLongitude;

        public SectionGeoLocatorController(ISectionGeoLocator latitudeLongitudeSection, BridgeCareContext context)
        {
            LatitudeLongitude = latitudeLongitudeSection ?? throw new ArgumentNullException(nameof(latitudeLongitudeSection));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/LatitudeLongitudesection
        public IQueryable<LatitudeLongitudeSectionModel> Get(SectionModel sectionModel) => LatitudeLongitude.GetLatitudeLongitude(sectionModel.NetworkId, db);
    }
}