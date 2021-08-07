using ApiServer.Models;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System;

namespace ApiServer.Controller
{   
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController: ControllerBase
    {
        private readonly FileService _fileService;
        private IWebHostEnvironment _hostingEnvironment;
        public FilesController(
            FileService fileService,
            IWebHostEnvironment environment
        )
        {
            _fileService = fileService;
            _hostingEnvironment = environment;
        }

        [HttpGet]
        public List<File> Get() => _fileService.GetFiles();

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            try {
                if (!Request.HasFormContentType) {
                    System.Console.WriteLine("No file");
                    return  StatusCode(500, new {message = "No file"});
                }
                var file = Request.Form.Files[0];
                var allowedSize = Request.Form["allowedSize"];
                System.Console.WriteLine(allowedSize);
                var res = await _fileService.UploadFile(file);
                if (res.success) {
                    return StatusCode(200, res);
                }
                return StatusCode(400, res);
            }catch(Exception e) 
            {
                System.Console.Write(e);
                return StatusCode(500, e);
            }
        }

        [HttpGet("generalTypes")]
        public ActionResult<List<string>> Types() => _fileService.getGeneralTypes();
    }

}