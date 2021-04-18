using DatingApp.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult GetSecret()
        {
            return Ok("secret text");
        }

        [HttpGet("not-found")]
        public ActionResult GetNotFound()
        {
            var thing = _context.User.Find(-1);

            if (thing == null)
            {
                return NotFound("User tidak terdaftar");
            }

            return Ok(thing);
        }

        [HttpGet("server-error")]
        public ActionResult GetServerError()
        {

            var thing = _context.User.Find(-1);
            var thingToReturn = thing.ToString();
            return Ok(thingToReturn);

        }

        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            return BadRequest("This is not good request");
        }

    }
}
