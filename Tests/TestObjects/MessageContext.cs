using System.Text;

namespace Tests.TestObjects;

public class MessageContext
{
    public string? UserName { get; set; }
    public string? Response { get; set; }
    public int Count { get; set; }
    public int MaxCount { get; set; }

    private StringBuilder _sb { get; set; } = new StringBuilder();

    public void Log(string message)
    {
        _sb.AppendLine(message);
    }

    public string GetLog()
    {
        return _sb.ToString();
    }

}

