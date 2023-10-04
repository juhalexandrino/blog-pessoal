using blogpessoal.Model;
using blogpessoal.Security;
using blogpessoal.Service;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blogpessoal.Controllers
{
    [Route("~/usuarios")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<User> _userValidator;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IValidator<User> userValidator, IAuthService authService)
        {
            _userService = userService;
            _userValidator = userValidator;
            _authService = authService;
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _userService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var Resposta = await _userService.GetById(id);

            if (Resposta is null)
                return NotFound();

            return Ok(Resposta);
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult> Create([FromBody] User user)
        {
            var validarUser = await _userValidator.ValidateAsync(user);

            if (!validarUser.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, validarUser);

            var Resposta = await _userService.Create(user);

            if (Resposta is null)
                return BadRequest("Usuário já cadastrado! Tente novamente.");

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("atualizar")]
        public async Task<ActionResult> Update([FromBody] User user)
        {
            if (user.Id == 0)
                return BadRequest("Id do usuário é invalído.");

            var validarUser = await _userValidator.ValidateAsync(user);

            if (!validarUser.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, validarUser);

            var UserUpdate = await _userService.GetByUsuario(user.Usuario);

            if (UserUpdate is not null && UserUpdate.Id != user.Id)
                return BadRequest("O usuário (e-mail) já está em uso.");

            var Resposta = await _userService.Update(user);

            if (Resposta is null)
                return NotFound("Usuário não encontrado!");

            return Ok(Resposta);
        }

        [AllowAnonymous]
        [HttpPost("logar")]
        public async Task<ActionResult> Autenticar([FromBody] UserLogin usuarioLogin)
        {
            var Resposta = await _authService.Autenticar(usuarioLogin);

            if (Resposta is null)
                return Unauthorized("Usuário e/ou senha inválidos!");

            return Ok(Resposta);

        }
    }
}
