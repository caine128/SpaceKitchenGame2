

public record ToolTipInfo()
{
    public readonly string[] bodyTextAsColumns;
    public readonly string header;
    public readonly string footer;

    public ToolTipInfo(string[] bodytextAsColumns, string header, string footer) : this()
    {
        this.bodyTextAsColumns = bodytextAsColumns;
        this.header = header;
        this.footer = footer;
    }
}
