namespace Ldc.Application.UseCases.Rsvps.Reports.Pdf;

public interface IGenerateGuestListPdfUseCase
{
    Task<byte[]> Execute(long weddingListId);
}
