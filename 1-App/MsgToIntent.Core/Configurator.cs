using MsgToIntent.Core.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MsgToIntent.Core;

public class Configurator
{
    
    SysData _sysData;

    public Configurator(SysData sysData)
    {
        _sysData = sysData;
    }
    public bool CreateSeparator(params string[] separators)
    {
        _sysData.Separators = string.Join("", separators);
        return true;
    }

    /// <summary>
    /// Create a basic/standard top word.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="wordType"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public bool CreateTopWord(string name, WordType wordType, params string[] values)
    {
        var topWord = new TopWord(name, wordType);

        // split values by separators
        foreach (string value in values) 
        {
            StringSplitter.Split(_sysData.Separators, _sysData.DecimalSeparator, value, out List<MsgToken> words);
            if (words.Count == 0) continue;
            if (words.Count == 1) 
            {
                topWord.AddValue(words[0].Value);
                continue;
            }
            List<string> wordValues = new List<string>(); 
            foreach(var word in words)
            {
                wordValues.Add(word.Value);
            }
            topWord.AddValues(wordValues.ToArray());
        }

        _sysData.TopWords.Add(topWord);
        return true;
    }


    /// <summary>
    /// Groups top words to a new one, type must be Grouping.
    /// </summary>
    /// <param name="topWordName"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public bool CreateGroupingTopWord(string topWordName, params string[] topWords)
    {
        // check that top words are defined
        foreach (string value in topWords)
        {
            if (_sysData.TopWords.FirstOrDefault(x => x.Name.Equals(value)) == null) return false;
        }

        var topWord = new TopWord(topWordName, WordType.Grouping);

        // add each values
        topWord.AddValues(topWords);

        _sysData.TopWords.Add(topWord);
        return true;
    }

    /// <summary>
    /// Creates an intent based on top words.
    /// </summary>
    /// <param name="intentName"></param>
    /// <param name="topWords"></param>
    /// <returns></returns>
    public bool CreateIntent(string intentName, params string[] topWords)
    {
        var intent = new Intent(intentName, topWords);
        _sysData.Intents.Add(intent);
        return true;
    }
}
