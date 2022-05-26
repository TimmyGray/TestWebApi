using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApi.Context;
using TestWebApi.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DbFilesController : ControllerBase
    {
        readonly DbFileContext db;
        public DbFilesController(DbFileContext db)
        {
            
            this.db = db;
            if (!db.Files.Any())
            {
                db.Files.AddRange(
                    new DbFile { Name = "Test1", Size = 25, Type = "txt" },
                    new DbFile { Name = "Test2", Size = 50, Type = "mp3" },
                    new DbFile { Name = "Test3", Size = 75, Type = "jpeg" });
                db.SaveChanges();

            }

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DbFile>>> GetAll()
        {
           return await  db.Files.ToListAsync();
          
        }

        //[HttpGet("{id}")]

        //public async Task<ActionResult<DbFile>> GetOne(int id)
        //{
        //    DbFile newf = await db.Files.FirstOrDefaultAsync(f => f.Id == id);
        //    if (newf != null)
        //    {
        //        return new ObjectResult(newf);
        //    }
        //    return NotFound(newf);
        //}

        //[HttpPost]
        //public async Task<ActionResult<DbFile>> Post(DbFile file)
        //{
        //    if(file == null)
        //    {
        //        return BadRequest("Нужно загрузить файл");
        //    }
        //    db.Files.Add(file);
        //    await db.SaveChangesAsync();
        //    return Ok(file);

        //}

        //[HttpDelete("{id}")]

        //public async Task<ActionResult<DbFile>> Delete(int id)
        //{
        //    DbFile file = await db.Files.FirstOrDefaultAsync(x => x.Id == id);
        //    if (file != null)
        //    {
        //        db.Files.Remove(file);
        //        await db.SaveChangesAsync();
        //        return Ok(file);
        //    }
        //    return NotFound(file);
        //}

        //private readonly ILogger<DbFilesController> _logger;

        //public DbFilesController(ILogger<DbFilesController> logger)
        //{
        //    _logger = logger;
        //}

    }
}