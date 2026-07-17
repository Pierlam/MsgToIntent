using MsgToIntent.Core;
using MsgToIntent.Core.System;

Console.WriteLine("=>DevApp MsgToIntent:");

MsgToIntentDecoder decoder = new MsgToIntentDecoder();

// define word separators
decoder.Config.CreateSeparator(",.'()+-!/=<>");

// define top words to identify the intent
decoder.Config.CreateTopWord("Hello", WordType.Welcome, "hello", "salut", "bonjour");
//decoder.Config.CreateTopWord("Test", WordType.Subject, "je veux");
decoder.Config.CreateTopWord("Je", WordType.Subject, "je", "j'");

decoder.Config.CreateTopWord("Prendre", WordType.Verb, "prendre", "prends", "prend", "pris");
decoder.Config.CreateTopWord("Vouloir", WordType.Verb, "vouloir", "veux", "veut", "voulons", "voulez", "voudraient");
decoder.Config.CreateTopWord("Souhaiter", WordType.Verb, "souhaiter", "souhaite", "souhaitent", "souhaiterons", "souhaiterez", "souhaiteraient");
decoder.Config.CreateTopWord("Pouvoir", WordType.Verb, "pouvoir", "peux", "pourrais", "pourrez", "pourriez", "pourré");
decoder.Config.CreateTopWord("Acheter", WordType.Verb, "acheter", "achète", "achètent", "achèterons", "achèterez", "achèteraient");

decoder.Config.CreateTopWord("AddProductInBasket", WordType.Grouping);

// define grouping top words to identify the intent
decoder.Config.CreateGroupingTopWord("AddProductInBasket", "Vouloir", "Acheter");
decoder.Config.CreateGroupingTopWord("AddProductInBasket", "Prendre");


// define intents based on top words
decoder.Config.CreateIntent("Welcome", "Hello");
decoder.Config.CreateIntent("AddProductInBasket", "Je", "AddProductInBasket" , "$Product");

//--decode messages to intents
//string msg = "Hello";
string msg = "je prends une Calzone";
//string msg = "j'aimerai une Calzone";
//string msg = "Hello, I want to book a flight from New York to Los Angeles next week.";

decoder.Decode(msg, out Intent matchedIntent);


// Je veux acheter une pizza:
//  1/ split en tokens
//  2/ -> Je (Subject) + Vouloir (Verb) + Acheter (Verb) + $Product (Variable)
//  3/ -> Je (Subject) + AddProductInBasket (Grouping) + $Product (Variable)
//  4/ intent: AddProductInBasket, variable: $Product = "une pizza"
