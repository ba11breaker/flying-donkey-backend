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

        [HttpGet("filter")]
        public List<File> Filter([FromQuery] string type, [FromQuery] string name) {
            return _fileService.FilterFiles(type, name);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm]Int64 allowedSize, [FromForm]string allowedType, [FromForm]string time, IFormFile file)
        {
            try {
                if (file == null) {
                    return StatusCode(400, new{message = "No file."});
                }
                if (allowedSize == 0) {
                    return StatusCode(400, new{message = "No allowedSize."});
                }
                if (allowedType == null) {
                    return StatusCode(400, new{message = "No allowedType."});
                }
                if (time == null) {
                    return StatusCode(400, new{message = "No time."});
                }
                if (!_fileService.checkType(allowedType)) {
                    return StatusCode(400, new{message = "Invalid File Type"});
                }   
                var res = await _fileService.UploadFile(file, allowedSize, allowedType, time);
                if (res.success) {
                    return StatusCode(200, res);
                }
                return StatusCode(400, res);
            }catch(Exception e) 
            {
                System.Console.Write(e);
                return StatusCode(500, new {message = e});
            }
        }

        [HttpGet("generalTypes")]
        public ActionResult<List<string>> Types() => _fileService.getGeneralTypes();
    }

}