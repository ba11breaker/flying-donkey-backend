using ApiServer.Models;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiServer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(
            UserService userService
        )
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<User>> Get() => _userService.GetUsers();

        [HttpPost]
        public ActionResult<User> Create (User user)
        {
            _userService.Create(user);
            return StatusCode(200, new {Id = user.Id, userName = user.UserName});
        }
    }
}