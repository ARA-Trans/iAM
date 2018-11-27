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
    public class TodosController : ApiController
    {
        private SimpleTestEntities db = new SimpleTestEntities();

        // GET: api/Todos
        public IQueryable<Todo> GetTodos()
        {
            return db.Todoes;
        }

        // GET: api/Todos/5
        [ResponseType(typeof(Todo))]
        public IHttpActionResult GetTodo(Guid id)
        {
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        // PUT: api/Todos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTodo(Guid id, Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != todo.Id)
            {
                return BadRequest();
            }

            db.Entry(todo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
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

        // POST: api/Todos
        [ResponseType(typeof(Todo))]
        public IHttpActionResult PostTodo(Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            todo.Id = Guid.NewGuid();
            db.Todoes.Add(todo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TodoExists(todo.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = todo.Id }, todo);
        }

        // DELETE: api/Todos/5
        [ResponseType(typeof(Todo))]
        public IHttpActionResult DeleteTodo(Guid id)
        {
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            db.Todoes.Remove(todo);
            db.SaveChanges();

            return Ok(todo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoExists(Guid id)
        {
            return db.Todoes.Count(e => e.Id == id) > 0;
        }
    }
}