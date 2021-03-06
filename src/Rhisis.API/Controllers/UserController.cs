﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Models;
using Rhisis.Core.Services;

namespace Rhisis.API.Controllers
{
    /// <summary>
    /// Provides API routes to manage users.
    /// </summary>
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        /// <summary>
        /// Creates a new <see cref="UserController"/> instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            this._logger = logger;
            this._userService = userService;
        }

        /// <summary>
        /// Registers an user.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] UserRegisterModel registerModel)
        {
            this._logger.LogInformation("An unknown user want to register a new account.");
            this._userService.CreateUser(registerModel);
            this._logger.LogInformation($"User {registerModel.Username} has been created.");

            return Ok();
        }

        /// <summary>
        /// Check if a user is already used using the given username.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("exists/{username}")]
        public IActionResult UserExists(string username)
        {
            this._logger.LogInformation($"An unknown user want to check if user '{username}' exists.");

            bool exists = this._userService.HasUser(username);
            
            return Ok(exists);
        }

        /// <summary>
        /// Check if the email address is used.
        /// </summary>
        /// <param name="email">Email address to check</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("mail/exists/{email}")]
        public IActionResult EmailExists(string email)
        {
            this._logger.LogInformation($"An unknown user want to check if email '{email}' exists.");

            bool emailExists = this._userService.HasUserWithEmail(email);

            return Ok(emailExists);
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="userLogin">User login model</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] UserLoginModel userLogin)
        {
            // TODO: authenticate user and generate JWT
            return Ok();
        }
    }
}
