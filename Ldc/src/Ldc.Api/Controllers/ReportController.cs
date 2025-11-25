using System.Net.Mime;
using Ldc.Application.UseCases.Expenses.Reports.Excel;
using Ldc.Application.UseCases.Expenses.Reports.Pdf;
using Ldc.Communication.Requests;
using Ldc.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ldc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //[Authorize(Roles = Roles.ADMIN)]
    public class ReportController : ControllerBase
    {
        [HttpGet("excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetExcel([FromServices] IGenerateExpensesReportExcelUseCase useCase ,[FromQuery] DateOnly month)
        {
            byte[] file = await useCase.Execute(month);
            if (file.Length > 0)
                return File(file, MediaTypeNames.Application.Octet, "report.xlsx");
            return NoContent();
        }
        
        [HttpGet("pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPdf([FromServices] IGenerateExpensesReportPdfUseCase useCase ,[FromQuery] DateOnly month)
        {
            byte[] file = await useCase.Execute(month);
            if (file.Length > 0)
                return File(file, MediaTypeNames.Application.Pdf, "report.pdf");
            return NoContent();
        }
    }
}
