using MsgToIntent.Core.System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MsgToIntent.Core;

internal class StringSplitter
{
    /// <summary>
    /// Splits a message into tokens or words based on whitespace and certain punctuation.
    /// MsgToken
    /// </summary>
    /// <param name="message"></param>
    /// <param name="words"></param>
    /// <returns></returns>
    public static bool Split(string separators, char decimalSep, string msg, out List<MsgToken> words)
    {
        words = new List<MsgToken>();
        if (string.IsNullOrEmpty(msg))
            return false;

        int lineNum = 0;
        MsgToken token;
        int i = 0;
        int iOut;

        while (true)
        {
            // no more char to extract
            if (i >= msg.Length)
            {
                break;
            }

            //--is there some space separator?
            if (ProcessSpaceChars(msg, i, out iOut))
            {
                i = iOut;
                continue;
            }

            //--is is a number?, integer or double, should be before separators extraction! because of decimal separator
            if (ProcessNumber(separators, decimalSep, msg, i, out iOut, out token))
            {
                token.LineNum = lineNum;
                words.Add(token);
                i = iOut;

                // manage case: StringBadFormated, end tag not found
                if (token.MsgTokenType == MsgTokenType.WrongNumber)
                {
                    //lastTokenType = MsgTokenType.WrongNumber;
                    return false;
                }
                continue;
            }

            //--is is a char separator?  manage special cases: >=, <=, <>
            if (ProcessSeparator(separators, msg, i, out iOut, out token))
            {
                token.LineNum = lineNum;
                words.Add(token);
                i = iOut;

                continue;
            }

            //--is it a name? variable, constant, object, function  or method
            if (ProcessWord(msg, i, out iOut, out token))
            {
                token.LineNum = lineNum;
                words.Add(token);
                i = iOut;

                //// check special error case
                //if (systVarStartTag != ' ' & token.Value.Trim().Equals(systVarStartTag.ToString()))
                //{
                //    token.MsgTokenType = MsgTokenType.WrongSystName;
                //    lastTokenType = MsgTokenType.WrongSystName;
                //    return false;
                //}

                //lastTokenType = MsgTokenType.Name;
                continue;
            }

            token = new MsgToken();
            //token.ScriptTokenType = ScriptTokenType.Undefined;
            token.LineNum = lineNum;
            token.ColNum = i;
            token.Value = msg[i].ToString();
            words.Add(token);
            i++;

            //lastTokenType = ScriptTokenType.Undefined;
            //return false;

        }

        //            words = message.Split(new char[] { ' ', '\t', '\n', '\r', '\f', '\'' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        return true;
    }

    /// <summary>
    /// Move cursor on each space char found.
    /// can be: space, \n, \t, \r.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="i"></param>
    /// <param name="iOut"></param>
    private static bool ProcessSpaceChars(string line, int i, out int iOut)
    {
        bool spaceFound = false;

        iOut = i;
        while (true)
        {
            if (i >= line.Length) return spaceFound;

            // space or special char (\n,\r, \t,...) found?
            if (!IsSpaceCharExt(line[i])) return spaceFound;

            spaceFound = true;
            i++;
            iOut = i;
        }
    }

    /// <summary>
    /// find numbers: integer and double. exp: 12  12.34
    /// Decimal separator is the dot: .
    /// 12E1O, 23E-10
    /// </summary>
    /// <param name="line"></param>
    /// <param name="i"></param>
    /// <param name="iOut"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool ProcessNumber(string separators, char decimalSep, string line, int i, out int iOut, out MsgToken token)
    {
        iOut = i;
        token = null;
        bool tokenFound = false;

        while (true)
        {
            // no more char
            if (i >= line.Length) break;

            char c = line[i];

            if (char.IsDigit(c))
            {
                if (token == null)
                {
                    token = new MsgToken();
                    token.MsgTokenType = MsgTokenType.Integer;
                    token.ColNum = i;
                }

                token.Value += c.ToString();
                i++;
                iOut = i;
                tokenFound = true;
                continue;
            }

            // is it the decimal separator?
            if (c == decimalSep)
            {
                if (token == null)
                {
                    // the next char should exists and should be a digit
                    if (i + 1 < line.Length)
                    {
                        // next char must be a digit
                        if (char.IsDigit(line[i + 1]))
                        {
                            token = new MsgToken();
                            token.MsgTokenType = MsgTokenType.Double;
                            token.ColNum = i;
                            continue;
                        }
                    }

                    // not a double number!
                    break;
                }

                // sure it's a double
                token.MsgTokenType = MsgTokenType.Double;

                token.Value += c.ToString();
                i++;
                iOut = i;
                continue;
            }

            // is it the E exposant separator ?
            if (c == 'e' || c == 'E')
            {
                // the token should exists
                if (token != null)
                {
                    // sure it's a double
                    token.MsgTokenType = MsgTokenType.Double;

                    token.Value += c.ToString();
                    i++;
                    iOut = i;
                    continue;
                }
            }

            // special case: minus char, exp 23E-10
            if (c == '-')
            {
                if (token != null)
                {
                    // previous char should exists and should be: E
                    if (token.Value.Length > 0)
                    {
                        if (token.Value.Last() == 'E' || token.Value.Last() == 'e')
                        {
                            token.Value += c.ToString();
                            i++;
                            iOut = i;
                            continue;
                        }
                    }
                }
            }

            // not a digit or a decimal separator!
            break;
        }

        // no integer or double found, bye
        if (token == null)
            return false;

        // the next char must be a separator or there is no more char
        if (iOut < line.Length)
        {
            // there is a next char, should be a space separator
            if (!IsSpaceCharExt(line[i]) && !separators.Contains(line[i]))
            {
                // error!  -> voir plus haut faire une fct
                // TODO: space, \n,\r
                token.MsgTokenType = MsgTokenType.WrongNumber;
                return true;
            }
        }

        // check the number, convert it
        if (token.MsgTokenType == MsgTokenType.Double)
        {
            if (double.TryParse(token.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                token.ValueDouble = d;
            else
                token.MsgTokenType = MsgTokenType.WrongNumber;
            return true;
        }

        if (token.MsgTokenType == MsgTokenType.Integer)
        {
            if (int.TryParse(token.Value, out int d))
                token.ValueInt = d;
            else
                token.MsgTokenType = MsgTokenType.WrongNumber;
            return true;
        }

        // a number was found
        return true;
    }

    /// <summary>
    /// is the current char a separator?  .=,;+-
    /// Manage special cases: >=, <=, <>
    /// </summary>
    /// <param name="separators"></param>
    /// <param name="line"></param>
    /// <param name="i"></param>
    /// <param name="iOut"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool ProcessSeparator(string separators, string line, int i, out int iOut, out MsgToken token)
    {
        iOut = i;
        token = null;

        if (separators.Contains(line[i]))
        {
            // it's a separator
            token = new MsgToken();
            token.MsgTokenType=MsgTokenType.Punctuation;
            token.Value = line[i].ToString();
            token.ColNum = i;
            // next
            i++;
            iOut = i;

            // special case:  >=, <=, <>
            ProcessLessGreaterEqualSeparator(line, i, token, out iOut);
            return true;
        }
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="line"></param>
    /// <param name="i"></param>
    /// <param name="token"></param>
    /// <param name="iOut"></param>
    /// <returns></returns>
    private static bool ProcessLessGreaterEqualSeparator(string line, int i, MsgToken token, out int iOut)
    {
        iOut = i;
        // no more char to process
        if (i > line.Length) return true;

        // >=
        if (token.Value == ">" && line[i] == '=')
        {
            token.MsgTokenType = MsgTokenType.CompOperator;
            token.Value = ">=";
            iOut = i + 1;
            return true;
        }

        // <=
        if (token.Value == "<" && line[i] == '=')
        {
            token.MsgTokenType = MsgTokenType.CompOperator;
            token.Value = "<=";
            iOut = i + 1;
            return true;
        }

        // <>
        if (token.Value == "<" && line[i] == '>')
        {
            token.MsgTokenType = MsgTokenType.CompOperator;
            token.Value = "<>";
            iOut = i + 1;
            return true;
        }
        return true;
    }

    /// <summary>
    /// Is it a word?
    /// </summary>
    /// <param name="systVarStartTag"></param>
    /// <param name="line"></param>
    /// <param name="i"></param>
    /// <param name="iOut"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool ProcessWord(string line, int i, out int iOut, out MsgToken token)
    {
        iOut = i;
        token = null;
        bool tokenFound = false;

        // manage first char system variable if defined
        //if (systVarStartTag != ' ' && line[i] == systVarStartTag)
        //{
        //    token = new MsgToken();
        //    token.MsgTokenType = MsgTokenType.SystName;
        //    token.ColNum = i;
        //    token.Value += systVarStartTag;
        //    tokenFound = true;

        //    // move to the next char
        //    i++;
        //}

        bool isFirstChar = true;

        while (true)
        {
            if (i >= line.Length) return tokenFound;

            char c = line[i];


            // is it a letter or a digit or an underscore?
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                // the first char can't be a digit, only a letter or an underscore
                if (isFirstChar && char.IsDigit(c))
                {
                    // stop here
                    return tokenFound;
                }

                isFirstChar = false;

                if (token == null)
                {
                    token = new MsgToken();
                    token.MsgTokenType = MsgTokenType.Word;
                    token.ColNum = i;
                }

                token.Value += c.ToString();
                i++;
                iOut = i;
                tokenFound = true;
                continue;
            }

            return tokenFound;
        }
    }


    /// <summary>
    /// Is the char a pure separator char like space?
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static bool IsSpaceCharExt(char c)
    {
        if (c == ' ') return true;
        if (c == '\r') return true;
        if (c == '\n') return true;
        if (c == '\t') return true;
        return false;
    }

}
