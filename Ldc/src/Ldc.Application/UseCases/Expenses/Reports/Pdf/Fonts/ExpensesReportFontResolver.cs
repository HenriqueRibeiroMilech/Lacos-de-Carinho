using System.Reflection;
using PdfSharp.Fonts;

namespace Ldc.Application.UseCases.Expenses.Reports.Pdf.Fonts;

public class ExpensesReportFontResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        var stream = ReadFontFile(faceName);
        if (stream is null)
            stream = ReadFontFile(FontHelper.DEFAULT_FONT);
        
        var length = (int)stream!.Length;
        var data = new byte[length];
        
        stream.Read(buffer: data, offset: 0, count: length);

        return data;
    }
    
    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        return new FontResolverInfo(familyName);
    }

    private Stream? ReadFontFile(string faceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        return assembly.GetManifestResourceStream($"Ldc.Application.UseCases.Expenses.Reports.Pdf.Fonts.{faceName}.ttf");
    }
}