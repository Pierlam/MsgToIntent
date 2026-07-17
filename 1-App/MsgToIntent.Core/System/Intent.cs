using System;
using System.Collections.Generic;
using System.Text;

namespace MsgToIntent.Core.System;

/// <summary>
/// Represents a user intent derived from a message.
/// </summary>
public class Intent
{
    public string Name { get; set; }

    /// <summary>
    /// The top words that define this intent.
    /// </summary>
    public List<string> TopWords { get; set; }

    public Intent(string name, params string[] topWords)
    {
        Name = name;
        TopWords = new List<string>(topWords);
    }

    public override string ToString()
    {
        return $"Intent: {Name}";
    }
}
