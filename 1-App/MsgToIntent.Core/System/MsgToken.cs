using System;
using System.Collections.Generic;
using System.Text;

namespace MsgToIntent.Core.System;

public enum MsgTokenType
{
    Unknown,
    WrongNumber,
    Integer,
    Double,
    //String,
    Word,
    Punctuation,
    CompOperator
}

/// <summary>
/// Represents a token or word extracted from a user text message.
/// </summary>
public class MsgToken
{
    public string Value { get; set; }
    public MsgTokenType MsgTokenType { get; set; } = MsgTokenType.Unknown;
    public int LineNum { get; set; } = 0;

    public int ColNum { get; set; } = 0;

    public int ValueInt { get; set; } = 0;
    public double ValueDouble { get; set; } = 0;
}
