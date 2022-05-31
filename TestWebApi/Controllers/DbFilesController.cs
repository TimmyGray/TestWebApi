using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApi.Context;
using TestWebApi.Models;
using System.Text.RegularExpressions;

namespace TestWebApi.Controllers
{
    [Authorize]
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

            return await db.Files.Where(u => u.User == User.Identity.Name).ToListAsync();

        }

        [HttpGet("{id}")]

        public async Task<ActionResult<DataFile>> GetOne(int id)
        {
            DbFile newf = await db.Files.Include(d=>d.File).FirstOrDefaultAsync(f => f.Id == id);
            if (newf != null)
            {
                return new ObjectResult(newf);
            }
            return NotFound(newf);
        }


        [HttpPost]

        public async Task<ActionResult<IEnumerable<DbFile>>> Post(IFormFileCollection files)
        {
            if (files == null||files.Count==0)
            {
                return BadRequest("Нужно загрузить файл");
            }

            Regex reg = new Regex(@"\.\w*$");

            List<DbFile> forresponse = new List<DbFile>();
           // User user = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            
            foreach (FormFile file in files)
            {
                DbFile newf = new DbFile();
                DataFile newdata = new DataFile();
                //newdata.File = newf;
                newf.File = newdata;

                string type = reg.Match(file.FileName).Value;
                string[] name = file.FileName.Split(".");                       //необходимо доработать алгоритм
                
                newf.Name = name[0];
                newf.Size = file.Length / 1024000f;                     //необходимо доработать алгоритм

                using (var reader = new BinaryReader(file.OpenReadStream()))
                {
                    newdata.Data = reader.ReadBytes((int)file.Length);
                }

                newf.Type = type;
                newf.User = User.Identity.Name;
                db.Files.Add(newf);
                db.DataFiles.Add(newdata);
                await db.SaveChangesAsync();

            }
            forresponse = await db.Files.Where(u=>u.User==User.Identity.Name).OrderByDescending(f => f.Id).ToListAsync();
            return forresponse;

        }

        [HttpDelete("{id}")]

        public async Task<ActionResult<DbFile>> Delete(int id)
        {
            DbFile file = await db.Files.Include(d=>d.File).FirstOrDefaultAsync(x => x.Id == id);
            if (file != null)
            {
                DataFile data = file.File;
                db.DataFiles.Remove(data);
                db.Files.Remove(file);
                await db.SaveChangesAsync();
                return Ok(file.Name);
            }
            return NotFound();
        }

    }
}