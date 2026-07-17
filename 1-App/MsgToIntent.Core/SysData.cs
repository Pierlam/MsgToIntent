using MsgToIntent.Core.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace MsgToIntent.Core;

public class SysData
{
    public string Separators = string.Empty;
    public char DecimalSeparator = '.';

    public List<TopWord> TopWords = new List<TopWord>();
    
    //public List<TopWordAggregate> TopWordAggregates = new List<TopWordAggregate>();

    public List<Intent> Intents = new List<Intent>();

}
