using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace AspWebApi.Controllers
{
    public class REPORT_13_9Controller : ApiController
    {
        private BridgeCareEntities db = new BridgeCareEntities();

        // GET: api/REPORT_13_9
        public IQueryable<REPORT_13_9> GetREPORT_13_9()
        {
            return db.REPORT_13_9.Take(100);
        }

        // GET: api/REPORT_13_9/5
        [ResponseType(typeof(REPORT_13_9))]
        public IHttpActionResult GetREPORT_13_9(int id)
        {
            REPORT_13_9 rEPORT_13_9 = db.REPORT_13_9.Where(_ => _.ID_ == id).FirstOrDefault();
            if (rEPORT_13_9 == null)
            {
                return NotFound();
            }

            return Ok(rEPORT_13_9);
        }

        // PUT: api/REPORT_13_9/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutREPORT_13_9(int id, REPORT_13_9 rEPORT_13_9)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rEPORT_13_9.ID_)
            {
                return BadRequest();
            }

            db.Entry(rEPORT_13_9).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!REPORT_13_9Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/REPORT_13_9
        [ResponseType(typeof(REPORT_13_9))]
        public IHttpActionResult PostREPORT_13_9(REPORT_13_9 rEPORT_13_9)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.REPORT_13_9.Add(rEPORT_13_9);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = rEPORT_13_9.ID_ }, rEPORT_13_9);
        }

        // DELETE: api/REPORT_13_9/5
        [ResponseType(typeof(REPORT_13_9))]
        public IHttpActionResult DeleteREPORT_13_9(int id)
        {
            REPORT_13_9 rEPORT_13_9 = db.REPORT_13_9.Find(id);
            if (rEPORT_13_9 == null)
            {
                return NotFound();
            }

            db.REPORT_13_9.Remove(rEPORT_13_9);
            db.SaveChanges();

            return Ok(rEPORT_13_9);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool REPORT_13_9Exists(int id)
        {
            return db.REPORT_13_9.Count(e => e.ID_ == id) > 0;
        }
    }
}