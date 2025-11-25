using Ldc.Application.UseCases.Users.ChangePassword;
using Ldc.Application.UseCases.Users.Delete;
using Ldc.Application.UseCases.Users.Profile;
using Ldc.Application.UseCases.Users.Register;
using Ldc.Application.UseCases.Users.Update;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromServices] IRegisterUserUseCase useCase, [FromBody] RequestRegisterUserJson request)
        {
            var response = await useCase.Execute(request);
            return Created(string.Empty, response);
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile([FromServices] IGetUserProfileUseCase useCase)
        {
            var response = await useCase.Execute();
            return Ok(response);
        }
        
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromServices] IUpdateUserUseCase useCase, [FromBody] RequestUpdateUserJson request)
        {
            await useCase.Execute(request);
            return NoContent();
        }        
        
        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromServices] IChangePasswordUseCase useCase, [FromBody] RequestChangePasswordJson request)
        {
            await useCase.Execute(request);
            return NoContent();
        }
        
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProfile([FromServices] IDeleteUserAccountUseCase useCase)
        {
            await useCase.Execute();
            return NoContent();
        }
    }
}
