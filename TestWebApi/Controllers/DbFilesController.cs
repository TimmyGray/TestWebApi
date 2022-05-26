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

        [HttpPost]
        public async Task<ActionResult<IEnumerable<DbFile>>> Post(IFormFileCollection files)
        {
            if (files == null)
            {
                return BadRequest("Нужно загрузить файл");
            }
            foreach (FormFile file in files)
            {
                DbFile newf = new DbFile();
                string[] nameandtype = file.FileName.Split(".");
                newf.Name = nameandtype[0];
                newf.Size = file.Length/1024000f;
                using (var reader = new BinaryReader(file.OpenReadStream()))
                {
                    newf.Data = reader.ReadBytes((int)file.Length);
                }
                newf.Type = nameandtype[1];
                db.Files.Add(newf);
                await db.SaveChangesAsync();   
              
            }
            
            return await db.Files.TakeLast(files.Count).ToListAsync();

        }

        [HttpDelete("{id}")]

        public async Task<ActionResult<DbFile>> Delete(int id)
        {
            DbFile file = await db.Files.FirstOrDefaultAsync(x => x.Id == id);
            if (file != null)
            {
                db.Files.Remove(file);
                await db.SaveChangesAsync();
                return Ok(file.Name);
            }
            return NotFound(file.Name);
        }

        //private readonly ILogger<DbFilesController> _logger;

        //public DbFilesController(ILogger<DbFilesController> logger)
        //{
        //    _logger = logger;
        //}

    }
}