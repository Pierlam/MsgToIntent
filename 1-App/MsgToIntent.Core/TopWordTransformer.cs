using MsgToIntent.Core.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MsgToIntent.Core;

/// <summary>
/// Transforms, formats list of words into a list of top words, based on the defined top words in the system data.
/// </summary>
internal class TopWordTransformer
{
    /// <summary>
    /// Processes the list of msg tokens and transforms them into a list of top words, based on the defined top words in the system data.
    /// </summary>
    /// <param name="sysData"></param>
    /// <param name="msgTokens"></param>
    /// <param name="topWords"></param>
    /// <returns></returns>
    public static bool Process(SysData sysData, List<MsgToken> msgTokens, out List<TopWord> topWords)
    {
        topWords = new List<TopWord>();

        // for each msg token, check if it matches a top word
        int i = 0;
        int iOut = 0;
        while(true)
        {
            if (i >= msgTokens.Count)
                break;

            // scan the list of top words to find a match for the current msg token
            ScanTopWords(sysData, msgTokens, i, out iOut, out TopWord topWordMatch);

            // if a match is found, add the top word to the list of top words
            if (topWordMatch != null)
            {
                topWords.Add(topWordMatch);
            }else
            {
                // the msg token does not match any top word!
                // todo: so what???  create a special top word for unknown words? 
            }

            // next msg token to process
            i++;
        }

        return true;
    }

    /// <summary>
    /// Scan the list of top words to find a match for the current msg token.
    /// </summary>
    /// <param name="sysData"></param>
    /// <param name="msgTokens"></param>
    /// <param name="i"></param>
    /// <param name="iOut"></param>
    private static void ScanTopWords(SysData sysData, List<MsgToken> msgTokens, int i,  out int iOut, out TopWord topWordMatch)
    {
        iOut = i;
        MsgToken word;

        // scan top words to find a match for the given word
        foreach (TopWord topWord in sysData.TopWords)
        {
            // scan values of the top word to find a match, exp: "Je" has 2 values: "je", "j'"
            ScanTopWordValues(sysData, i, msgTokens, topWord, out iOut, out TopWordValue topWordValueMatch);
            if(topWordValueMatch != null) {
                // match found, add the top word to the list of top words
                iOut = i;
                topWordMatch = topWord;
                return;
            }
        }

        topWordMatch = null;
    }

    /// <summary>
    /// Scan values of the top word to find a match, exp: "Je" has 2 values: "je", "j'"
    /// </summary>
    /// <param name="sysData"></param>
    /// <param name="i"></param>
    /// <param name="topWord"></param>
    /// <param name="iOut"></param>
    private static void ScanTopWordValues(SysData sysData, int i, List<MsgToken> msgTokens, TopWord topWord, out int iOut, out TopWordValue topWordValueMatch)
    {
        iOut = i;

        foreach (TopWordValue topWordValue in topWord.Values)
        {
            // basic case, the value has only one part, exp: "Hello" -> "Hello"
            if(topWordValue.Parts.Count == 1)
            {
                if (topWordValue.Parts[0].Equals(msgTokens[i].Value, StringComparison.CurrentCultureIgnoreCase))
                {
                    // match found, add the top word to the list of top words
                    iOut = i;
                    topWordValueMatch= topWordValue;
                    return;
                }
            }

            // complex case, the value has multiple parts, exp: "j'" -> "j" + "'"
            // todo:
        }

        topWordValueMatch = null;
    }

}
