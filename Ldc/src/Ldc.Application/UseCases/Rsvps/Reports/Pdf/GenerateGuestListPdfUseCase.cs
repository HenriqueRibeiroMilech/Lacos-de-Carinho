using Ldc.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories.Rsvp;
using Ldc.Domain.Repositories.WeddingList;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception.ExceptionBase;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace Ldc.Application.UseCases.Rsvps.Reports.Pdf;

public class GenerateGuestListPdfUseCase : IGenerateGuestListPdfUseCase
{
    private const int HEIGHT_ROW = 25;
    
    // Cores do tema rosa
    private static readonly Color ROSE_DARK = Color.Parse("#D97F97");
    private static readonly Color ROSE_LIGHT = Color.Parse("#FDF2F4");
    private static readonly Color ROSE_MEDIUM = Color.Parse("#F5D7DE");
    private static readonly Color BLACK = Color.Parse("#333333");
    private static readonly Color WHITE = Color.Parse("#FFFFFF");
    private static readonly Color GREEN = Color.Parse("#10B981");
    private static readonly Color AMBER = Color.Parse("#F59E0B");
    private static readonly Color GRAY = Color.Parse("#6B7280");

    private readonly IRsvpReadOnlyRepository _rsvpRepository;
    private readonly IWeddingListReadOnlyRepository _weddingListRepository;
    private readonly ILoggedUser _loggedUser;

    public GenerateGuestListPdfUseCase(
        IRsvpReadOnlyRepository rsvpRepository,
        IWeddingListReadOnlyRepository weddingListRepository,
        ILoggedUser loggedUser)
    {
        _rsvpRepository = rsvpRepository;
        _weddingListRepository = weddingListRepository;
        _loggedUser = loggedUser;

        GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
    }

    public async Task<byte[]> Execute(long weddingListId)
    {
        var loggedUser = await _loggedUser.Get();
        
        // Verifica se o usuário é dono da lista
        var weddingList = await _weddingListRepository.GetById(weddingListId);
        if (weddingList is null || weddingList.OwnerId != loggedUser.Id)
        {
            throw new NotFoundException("Lista não encontrada");
        }

        var rsvps = await _rsvpRepository.GetByWeddingListId(weddingListId);
        
        if (rsvps.Count == 0)
            return [];

        var document = CreateDocument(weddingList.Title);
        var page = CreatePage(document);

        // Header
        CreateHeader(page, weddingList.Title, weddingList.EventDate);

        // Seção de Confirmados (inclui acompanhantes como pessoas individuais)
        var confirmedRsvps = rsvps.Where(r => r.Status == RsvpStatus.Confirmed).ToList();
        if (confirmedRsvps.Any())
        {
            var expandedConfirmed = ExpandRsvpsWithGuests(confirmedRsvps);
            CreateSectionTitle(page, $"✓ Confirmados ({expandedConfirmed.Count})");
            CreateGuestTable(page, expandedConfirmed, GREEN);
        }

        // Seção de Pendentes
        var pendingRsvps = rsvps.Where(r => r.Status == RsvpStatus.Pending).ToList();
        if (pendingRsvps.Any())
        {
            var expandedPending = ExpandRsvpsWithGuests(pendingRsvps);
            CreateSectionTitle(page, $"⏳ Aguardando Resposta ({expandedPending.Count})");
            CreateGuestTable(page, expandedPending, AMBER);
        }

        // Seção de Declinados (opcional)
        var declinedRsvps = rsvps.Where(r => r.Status == RsvpStatus.Declined).ToList();
        if (declinedRsvps.Any())
        {
            var expandedDeclined = ExpandRsvpsWithGuests(declinedRsvps);
            CreateSectionTitle(page, $"✗ Não Irão Comparecer ({expandedDeclined.Count})");
            CreateGuestTable(page, expandedDeclined, GRAY);
        }

        return RenderDocument(document);
    }

    private Document CreateDocument(string title)
    {
        var document = new Document();
        document.Info.Title = $"Lista de Convidados - {title}";
        document.Info.Author = "Laços de Carinho";

        var style = document.Styles["Normal"];
        style!.Font.Name = FontHelper.RALEWAY_REGULAR;

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();
        section.PageSetup = document.DefaultPageSetup.Clone();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 40;
        section.PageSetup.BottomMargin = 40;

        return section;
    }

    private void CreateHeader(Section page, string title, DateOnly eventDate)
    {
        // Logo/Título do sistema
        var headerParagraph = page.AddParagraph();
        headerParagraph.Format.Alignment = ParagraphAlignment.Center;
        headerParagraph.Format.SpaceAfter = 5;
        
        var logoText = headerParagraph.AddFormattedText("💝 Laços de Carinho", TextFormat.Bold);
        logoText.Font.Size = 12;
        logoText.Font.Color = ROSE_DARK;

        // Título do evento
        var titleParagraph = page.AddParagraph();
        titleParagraph.Format.Alignment = ParagraphAlignment.Center;
        titleParagraph.Format.SpaceAfter = 5;
        
        var titleText = titleParagraph.AddFormattedText(title, TextFormat.Bold);
        titleText.Font.Size = 22;
        titleText.Font.Color = BLACK;

        // Data do evento
        var dateParagraph = page.AddParagraph();
        dateParagraph.Format.Alignment = ParagraphAlignment.Center;
        dateParagraph.Format.SpaceAfter = 20;
        
        var dateText = dateParagraph.AddFormattedText(eventDate.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR")), TextFormat.Italic);
        dateText.Font.Size = 11;
        dateText.Font.Color = GRAY;

        // Linha decorativa
        var line = page.AddParagraph();
        line.Format.Borders.Bottom.Width = 2;
        line.Format.Borders.Bottom.Color = ROSE_LIGHT;
        line.Format.SpaceAfter = 20;
    }

    private void CreateSectionTitle(Section page, string title)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = 15;
        paragraph.Format.SpaceAfter = 10;
        
        var text = paragraph.AddFormattedText(title, TextFormat.Bold);
        text.Font.Size = 14;
        text.Font.Color = BLACK;
    }

    // Classe auxiliar para representar uma pessoa na lista
    private record GuestEntry(string Name, string Email);

    // Expande os RSVPs incluindo acompanhantes como entradas individuais
    private List<GuestEntry> ExpandRsvpsWithGuests(List<Domain.Entities.Rsvp> rsvps)
    {
        var expanded = new List<GuestEntry>();
        
        foreach (var rsvp in rsvps.OrderBy(r => r.Guest.Name))
        {
            // Adiciona o convidado principal
            expanded.Add(new GuestEntry(rsvp.Guest.Name, rsvp.Guest.Email));
            
            // Adiciona acompanhantes com o mesmo email do convidado principal
            if (!string.IsNullOrEmpty(rsvp.AdditionalGuests))
            {
                var guests = rsvp.AdditionalGuests.Split(',')
                    .Select(g => g.Trim())
                    .Where(g => !string.IsNullOrEmpty(g));
                    
                foreach (var guestName in guests)
                {
                    expanded.Add(new GuestEntry(guestName, rsvp.Guest.Email));
                }
            }
        }
        
        return expanded;
    }

    private void CreateGuestTable(Section page, List<GuestEntry> guests, Color statusColor)
    {
        var table = page.AddTable();
        table.Borders.Width = 0.5;
        table.Borders.Color = ROSE_MEDIUM;

        var pageWidth = page.PageSetup.PageWidth - page.PageSetup.LeftMargin - page.PageSetup.RightMargin;
        
        // Colunas: #, Nome, Email
        table.AddColumn(30);  // #
        table.AddColumn(pageWidth * 0.40 - 30);  // Nome
        table.AddColumn(pageWidth * 0.60);  // Email

        // Header da tabela
        var headerRow = table.AddRow();
        headerRow.Height = HEIGHT_ROW;
        headerRow.Shading.Color = ROSE_DARK;
        
        AddTableHeader(headerRow.Cells[0], "#");
        AddTableHeader(headerRow.Cells[1], "Nome");
        AddTableHeader(headerRow.Cells[2], "Email");

        // Linhas de convidados
        var index = 1;
        foreach (var guest in guests)
        {
            var row = table.AddRow();
            row.Height = HEIGHT_ROW;
            row.Shading.Color = index % 2 == 0 ? ROSE_LIGHT : WHITE;

            AddTableCell(row.Cells[0], index.ToString(), ParagraphAlignment.Center);
            AddTableCell(row.Cells[1], guest.Name);
            AddTableCell(row.Cells[2], guest.Email);

            index++;
        }

        // Espaço após tabela
        var space = page.AddParagraph();
        space.Format.SpaceAfter = 10;
    }

    private void AddTableHeader(Cell cell, string text)
    {
        cell.Format.Alignment = ParagraphAlignment.Center;
        cell.VerticalAlignment = VerticalAlignment.Center;
        
        var paragraph = cell.AddParagraph();
        var formattedText = paragraph.AddFormattedText(text, TextFormat.Bold);
        formattedText.Font.Size = 10;
        formattedText.Font.Color = WHITE;
    }

    private void AddTableCell(Cell cell, string text, ParagraphAlignment alignment = ParagraphAlignment.Left)
    {
        cell.Format.Alignment = alignment;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.Format.LeftIndent = alignment == ParagraphAlignment.Left ? 5 : 0;
        
        var paragraph = cell.AddParagraph();
        var formattedText = paragraph.AddFormattedText(text);
        formattedText.Font.Size = 9;
        formattedText.Font.Color = BLACK;
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document
        };

        renderer.RenderDocument();

        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);
        
        return file.ToArray();
    }
}
