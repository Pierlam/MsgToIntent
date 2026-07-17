using MsgToIntent.Core.System;

namespace MsgToIntent.Core;

/// <summary>
/// Main class for decoding messages to intents.
/// </summary>
public class MsgToIntentDecoder
{
    /// <summary>
    /// System data for the decoder.
    /// </summary>
    SysData _sysData = new SysData();

    public MsgToIntentDecoder()
    {
        Config = new Configurator(_sysData);
    }

    public Configurator Config { get; private set; }

    public bool Decode(string message, out Intent matchedIntent)
    {
        // 1/ split the message into tokens: word, punctuation, number, etc.
        StringSplitter.Split(_sysData.Separators, _sysData.DecimalSeparator, message, out List<MsgToken> msgTokens);

        // ?/ WordCorrection?

        // 2/ TopWordTransformer
        TopWordTransformer.Process(_sysData, msgTokens, out List<TopWord> topWords);

        // 3/ grouping top words 
        //TopWordsGroup.Process(_sysData, msgTokens, topWords, out List<TopWord> topWordsGrouped);

        // 4/ IntentDecoder
        IntentDecoder.Process(_sysData, topWords, out matchedIntent);

        // todo: return one (or more) intent(s) based on the message and top words

        return true;
    }
}
