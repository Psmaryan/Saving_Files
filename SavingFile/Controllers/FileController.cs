using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomIdentityApp.Models;
using SavingFile.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SavingFile.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SavingFile.Controllers
{
    public class ParentDTO
    {
        public int? idFile { get; set; }
        public string idUser { get; set; }
    }

    [Authorize]
    public class FileController : Controller
    {
        private readonly UserManager<User> _userManager;

        ApplicationContext _context;
        public FileController(ApplicationContext context, UserManager<User> userManager, IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();

            if (users != null)
                return Ok(users.Select(u => new { id = u.Id, name = u.UserName }));
            return NotFound();

        }

        [HttpGet("{id}")]
        public IActionResult GetUsers(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
                return Ok(user);
            return NotFound();
        }


        private IHostingEnvironment _hostingEnvironment;
        [HttpPost]
        public IActionResult CreateFileAsync(string idUser, IFormFile file)
        {
            User usr = _context.Users.Where(us => us.Id == idUser).First();
            // User usr = _userManager.FindById(idUser);
            if (usr != null)
            {
                if (file != null)
                {
                    SaveFile saveFile = new SaveFile();

                    //Convert IformFile to byte[];
                    saveFile.File = ConvertToBytes(file);
                    saveFile.Name = file.FileName;


                    _context.SaveFiles.Add(saveFile);
                    _context.SaveChanges();


                    usr.UserSaveFiles.Add(new UserSaveFile { UserId = usr.Id, SaveFileId = saveFile.Id });
                    _context.SaveChanges();

                    return StatusCode(201);
                }
            }

            return StatusCode(302);
        }
        private byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)image.Length);
            return CoverImageBytes;
        }

        [HttpGet]
        public FileResult DownloadFile(int? idFile)
        {
            if (idFile != null)
            {
                SaveFile file = _context.SaveFiles.Where(f => f.Id == idFile).First();
                string p = file.Name;
                string e = Path.GetExtension(p);
                e = e.Replace(".", string.Empty);

                byte[] mas = file.File;
                string file_type = GetTypeFile.mimeTypes[e];
                string file_name = file.Name;
                return File(mas, file_type, file_name);
            }

            return null;
        }

        [HttpGet]
        public IActionResult getNameFile(string idUser)
        {
            if(idUser != null)
            {
                List<object[]> list = new List<object[]>();
                var nameFiles = _context.UserSaveFiles.Where(usf => usf.UserId == idUser)
                               .Join(_context.SaveFiles, usf => usf.SaveFileId, sf => sf.Id, (usf, sf) => new { id = sf.Id, name = sf.Name, dateAdded = usf.DateAdded, idUSF = usf.Id})
                               .ToList();

                if (nameFiles.Count > 0)
                {
                    foreach (var nF in nameFiles)
                        list.Add(new object[] {nF.id, nF.name, nF.dateAdded, nF.idUSF});

                    ViewBag.IdUser = idUser;
                    return PartialView(list);
                }

                return new HtmlResult("<div class=\"text-danger\">user does not have any file</div>");
            }
            return new HtmlResult("<div class=\"text-danger\">does not be load any file</div>");
        }
               
        [HttpPost]
        public IActionResult DeleteFile([FromBody] ParentDTO parentr)
        {
            const string query = "DELETE FROM [dbo].[UserSaveFiles] WHERE [Id]={0}";
            var rows = _context.Database.ExecuteSqlCommand(query, parentr.idFile.Value);
            return new HtmlResult("Its Ok!");
        }

        [HttpPost]
        public IActionResult AddFileToUser([FromBody] ParentDTO parentr)
        {
            if (parentr != null)
            {
                UserSaveFile usf = new UserSaveFile() { UserId = parentr.idUser, SaveFileId = parentr.idFile.Value };
                _context.UserSaveFiles.Add(usf);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}