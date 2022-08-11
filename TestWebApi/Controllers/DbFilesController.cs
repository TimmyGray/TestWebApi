using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApi.Context;
using TestWebApi.Models;
using System.Text.RegularExpressions;
using System.IO.Compression;

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

               await UnzipFile(newf);

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
            
            foreach (FormFile file in files)
            {
                DbFile newf = new DbFile();
                newf.File = new DataFile();

                string type = reg.Match(file.FileName).Value;
                string[] name = file.FileName.Split(".");                       //необходимо доработать алгоритм
                
                newf.Name = name[0];
                newf.Type = type;
                newf.User = User.Identity.Name;

                await ZipFile(file, newf);
               
                db.Files.Add(newf);
                db.DataFiles.Add(newf.File);
                await db.SaveChangesAsync();

            }
            forresponse = await db.Files.Where(u=>u.User==User.Identity.Name).OrderByDescending(f => f.Id).Take(files.Count).ToListAsync();
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

        private async Task ZipFile(FormFile formfile, DbFile metadata)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"{metadata.Name}.gz");

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var compressor = new GZipStream(fs, CompressionMode.Compress))
                {
                    await formfile.OpenReadStream().CopyToAsync(compressor);
                }
            }

            using (FileStream stream2 = new FileStream(path, FileMode.Open))
            {
                byte[] buffer = new byte[stream2.Length];

                await stream2.ReadAsync(buffer, 0, buffer.Length);

                metadata.File.Data = buffer;
                metadata.Size = buffer.Length / 1024000f;
            }

            System.IO.File.Delete(path);

        }

        private async Task UnzipFile(DbFile file)
        {

            string directorypath = Path.Combine(Directory.GetCurrentDirectory(), $"{file.Name}.gz");
            string newfilepath = Path.Combine(Directory.GetCurrentDirectory(), $"{file.Name}{file.Type}");
            
            FileInfo filetodec = new FileInfo(directorypath);
            FileInfo DecFile = new FileInfo(newfilepath);

            using (FileStream stream1 = filetodec.OpenWrite())
            {
                byte[] buffer = file.File.Data;
                await stream1.WriteAsync(buffer,0,buffer.Length);
            }

            using (FileStream stream2 = filetodec.OpenRead())
            {
                               
                using (FileStream stream3 = DecFile.OpenWrite())
                {
                    using (GZipStream decompress = new GZipStream(stream2, CompressionMode.Decompress))
                    {
                        await decompress.CopyToAsync(stream3);
                    }
                }
            }

            using(FileStream stream4 = DecFile.OpenRead())
            {
                byte[] buffer = new byte[DecFile.Length];
                await stream4.ReadAsync(buffer,0,buffer.Length);
                file.File.Data = buffer;

            }

            filetodec.Delete();
            DecFile.Delete();

        }

    }
}