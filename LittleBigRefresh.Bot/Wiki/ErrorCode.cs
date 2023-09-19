namespace LittleBigRefresh.Bot.Wiki;

#nullable disable

public class ErrorCode
{
    public required string Section { get; init; }
    public string Name { get; set; } // Description
    public string Code { get; set; } // Errorcode
    public string Description { get; set; } // Remarks

    public override string ToString()
    {
        return $"{nameof(Section)}: {Section}, {nameof(Name)}: {Name}, {nameof(Code)}: {Code}, {nameof(Description)}: {Description}";
    }
}