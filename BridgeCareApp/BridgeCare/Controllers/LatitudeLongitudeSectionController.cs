using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class LatitudeLongitudeSectionController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ILatitudeLongitudeSection LatitudeLongitude;

        public LatitudeLongitudeSectionController(ILatitudeLongitudeSection lattitudeLongitudeSection, BridgeCareContext context)
        {
            LatitudeLongitude = lattitudeLongitudeSection ?? throw new ArgumentNullException(nameof(lattitudeLongitudeSection));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/LatitudeLongitudesection
        public IQueryable<LatitudeLongitudeSectionModel> Get(SectionModel sectionModel) => LatitudeLongitude.GetLatitudeLongitude(sectionModel.NetworkId, db);
   
    }
}