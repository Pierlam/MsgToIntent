using MsgToIntent.Core.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace MsgToIntent.Core;

internal class IntentDecoder
{
    /// <summary>
    /// Find the intent matching the top words. 
    /// </summary>
    /// <param name="sysData"></param>
    /// <param name="topWords"></param>
    public static void Process(SysData sysData, List<TopWord> topWords, out Intent matchedIntent)
    {
        matchedIntent = null;

        // scan the list of intents to find a match for the top words
        foreach (Intent intent in sysData.Intents)
        {
            // check if the intent matches the top words
            if(IntentMatch(intent, topWords))
            {
                matchedIntent= intent;
                return;
            }
        }
    }

    private static bool IntentMatch(Intent intent, List<TopWord> topWords)
    {
        int i = 0;

        // check if the intent matches the top words
        foreach (string topWord in intent.TopWords) 
        {
            if(i >= topWords.Count)
                return false;

            if (!topWord.Equals(topWords[i].Name,StringComparison.InvariantCultureIgnoreCase)) return false;

            i++;
        }

        // all top words matched
        return true;
    }
}
