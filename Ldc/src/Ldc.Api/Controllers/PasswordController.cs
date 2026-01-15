using Ldc.Application.UseCases.Password.RequestReset;
using Ldc.Application.UseCases.Password.ResetPassword;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PasswordController : ControllerBase
{
    /// <summary>
    /// Solicita um email de recuperação de senha
    /// </summary>
    [HttpPost("request-reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestPasswordReset(
        [FromServices] IRequestPasswordResetUseCase useCase, 
        [FromBody] RequestPasswordResetJson request,
        [FromHeader(Name = "Origin")] string? origin)
    {
        // Usa o header Origin ou um fallback
        var baseUrl = origin ?? "http://localhost:4200";
        
        await useCase.Execute(request, baseUrl);
        
        // Sempre retorna sucesso (não revelamos se o email existe)
        return Ok(new { message = "Se o email existir em nossa base, você receberá um link de recuperação." });
    }

    /// <summary>
    /// Redefine a senha usando o token recebido por email
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromServices] IResetPasswordUseCase useCase,
        [FromBody] ResetPasswordJson request)
    {
        await useCase.Execute(request);
        return Ok(new { message = "Senha alterada com sucesso!" });
    }

    /// <summary>
    /// Valida se um token de reset é válido
    /// </summary>
    [HttpGet("validate-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ValidateToken([FromQuery] string token)
    {
        // Validação básica do token - o reset real valida no banco
        if (string.IsNullOrWhiteSpace(token) || token.Length < 32)
        {
            return BadRequest(new { valid = false, message = "Token inválido" });
        }
        return Ok(new { valid = true });
    }
}
