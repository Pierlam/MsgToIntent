using System;
using System.Collections.Generic;
using System.Text;

namespace MsgToIntent.Core.System;

/// <summary>
/// examples: 
///  Je: has 2 values: 
///     j'  -> "j", "'"  
///  
/// MonteCharge:  has 3 values
///     monte-charge -> "monte", "-", "charge"   
/// </summary>
public class TopWordValue
{
    /// <summary>
    /// a top word value can have multiple parts, for example:
    /// "j'" -> "j", "'"
    /// </summary>
    public List<string> Parts { get; set; }
}

/// <summary>
/// Summary of the TopWord class.
/// exp: Je  
///   type: Subject. 
///   values: Je, j'  
/// </summary>
public class TopWord
{
    public string Name { get; set; }
    public WordType WordType { get; set; }

    /// <summary>
    /// List of values for the top word. Each value can have one or multiple parts.
    /// exp: Hello -> "Hello", "Welcome", "Hi", "Salut"
    /// exp : Je -> "Je", "j'" (the value "j'" has 2 parts: "j" and "'")
    /// </summary>
    public List<TopWordValue> Values { get; set; }= new List<TopWordValue>();

    public TopWord(string name, WordType wordType) 
    {
        Name = name;
        WordType = wordType;
    }

    public void AddValue(string value)
    {
        Values.Add(new TopWordValue { Parts = new List<string> { value } });
    }

    public void AddValues(string[] values)
    {
        Values.Add(new TopWordValue { Parts = new List<string>(values) });
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"WordType: {WordType}");
        sb.AppendLine("Values:");
        foreach (var value in Values)
        {
            sb.AppendLine($"  - {string.Join(", ", value.Parts)}");
        }
        return sb.ToString();
    }
}
