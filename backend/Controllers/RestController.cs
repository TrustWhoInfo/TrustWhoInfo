using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace backend
{
    [ApiController]
    [Route("api")]
    public class RestController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public RestController(ApplicationDbContext db) {
            this.db = db;
        }
    }
}