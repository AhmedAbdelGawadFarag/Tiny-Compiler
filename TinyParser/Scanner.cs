
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Tiny_Compiler
{
    public enum Token_Class
    {
        Number, String, Int, Float, Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return, Endl,
        Comment, RightParentheses, LeftParentheses, PlusOp, MinusOp, MultiplyOp, DivideOp, AssignmentOp, SemiColon,
        Coma, LessThanOp, GreaterThanOp, IsEqualOp, NotEqualOp, AndOp, OrOp, LeftBraces, RightBraces,
        Main, Identifier, End,
        EndOfFile, Invalid

    }
    public class Token
    {
        public Token()
        {

        }
        public Token(string lex, Token_Class tc)
        {
            this.lex = lex;
            this.token_type = tc;
        }
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<char, Token_Class> specialChar = new Dictionary<char, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("end", Token_Class.End);

            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("=", Token_Class.IsEqualOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(":=", Token_Class.AssignmentOp);


            specialChar.Add(';', Token_Class.SemiColon);
            specialChar.Add(',', Token_Class.Coma);
            specialChar.Add('(', Token_Class.LeftParentheses);
            specialChar.Add(')', Token_Class.RightParentheses);
            specialChar.Add('{', Token_Class.LeftBraces);
            specialChar.Add('}', Token_Class.RightBraces);

        }

        public List<Token> StartScanning(string source)
        {
            // outer loop to parse the whole source code char by char
            for (int i = 0; i < source.Length; ++i)
            {
                // j for inner loops
                int j = i;
                char c = source[i];
                // add initail char to current lexeme 
                string curr = c.ToString();
                // ignore if space, tab or new line
                if (char.IsWhiteSpace(c))
                    continue;

                //check if current char is letter [a-z]
                if (char.IsLetter(c))
                {

                    Token token;
                    // only lexemes starting with letter is reserved words or identifier 
                    // if the first char is letter then search for next till finding non letter or didgit char
                    if (j != source.Length - 1)
                    {
                        ++j;
                        c = source[j];
                        // loop till there is no more letters or digits
                        while (char.IsLetterOrDigit(c) && (j <= source.Length - 1))
                        {
                            // add every letter or digit we find
                            curr += c;

                            // update char
                            if (j != source.Length - 1)
                            {
                                ++j;
                                c = source[j];
                            }
                            else
                                break;
                        }
                        // update outer loop index
                        if (j != source.Length - 1)
                            i = j - 1;
                        else
                            i = j;
                    }

                    // see if teh lexeme is reserved word or identifier
                    if (this.isReservedWord(curr))
                        token = new Token(curr, this.ReservedWords[curr]);
                    else if (this.isIdentifier(curr))
                        token = new Token(curr, Token_Class.Identifier);
                    // if not one of them then it's invalid input
                    else
                        token = new Token(curr, Token_Class.Invalid);
                    // add the result to the list of Tokens to return at the end
                    this.Tokens.Add(token);
                    continue;
                }
                // if first char is digit then it must be number or invalid input
                else if (char.IsDigit(c))
                {
                    Token token;
                    if (j != source.Length - 1)
                    {
                        ++j;
                        c = source[j];
                        // parse till finding non digit nor dot
                        while ((char.IsDigit(c) || c == '.') && (j <= source.Length - 1))
                        {
                            // add the digit or dot to current lexeme
                            curr += c;
                            // update char
                            if (j != source.Length - 1)
                            {
                                ++j;
                                c = source[j];
                            }
                            else
                                break;
                        }
                    }
                    // update outer loop index
                    if (j != source.Length - 1)
                        i = j - 1;
                    else
                        i = j;
                    // validiate if it's a number or not
                    if (this.isNumber(curr))
                        token = new Token(curr, Token_Class.Number);
                    else
                        token = new Token(curr, Token_Class.Invalid);

                    this.Tokens.Add(token);
                    continue;
                }
                // if there is / and not the last char in source then search for the * if exist loop till finding the ending * and /
                else if ((j < source.Length - 2) && (source[j] == '/' && source[j + 1] == '*'))
                {
                    Token token;
                    curr += source[j + 1];
                    j += 2;
                    c = source[j];
                    while (source[j] != '*' && source[j + 1] != '/')
                    {
                        curr += c;
                        if (j != source.Length - 2)
                        {
                            ++j;
                            c = source[j];
                        }
                        else
                            break;
                    }

                    if (j != source.Length - 2)
                    {
                        i = j + 1;
                        curr += c;
                        curr += source[j + 1];
                        token = new Token(curr, Token_Class.Comment);
                       // this.Tokens.Add(token);
                    }
                    else
                    {
                        i = j + 1;
                        curr += c;
                        curr += source[j + 1];
                        token = new Token(curr, Token_Class.Invalid);
                        this.Tokens.Add(token);
                    }

                }
                // find if the char is sepcial one
                else if (this.specialChar.ContainsKey(c))
                {
                    Token token = new Token(c.ToString(), specialChar[c]);
                    this.Tokens.Add(token);
                    continue;
                }
                // find if the char is double char operator && || <> := 
                else if (
                    (c == '&' && source[j + 1] == '&')
                    || (c == '|' && source[j + 1] == '|')
                    || (c == '<' && source[j + 1] == '>')
                    || (c == ':' && source[j + 1] == '='))
                {
                    string temp = c.ToString() + source[j + 1];
                    Token token = new Token(temp, this.Operators[c.ToString() + source[j + 1]]);
                    this.Tokens.Add(token);
                    ++j;
                    i = j;
                    continue;

                }
                // find if the char is operator
                else if (this.Operators.ContainsKey(c.ToString()))
                {
                    Token token = new Token(c.ToString(), Operators[c.ToString()]);
                    this.Tokens.Add(token);
                    continue;
                }
                // search for comment, if " is found then parse till finding another one
                else if (c == '"')
                {
                    Token token;
                    ++j;
                    c = source[j];
                    while ((c != '"') && (j < source.Length - 1))
                    {
                        curr += c;
                        if (j != source.Length - 1)
                        {
                            ++j;
                            c = source[j];
                        }
                    }
                    if (j != source.Length - 1)
                    {
                        i = j;
                        curr += c;
                        token = new Token(curr, Token_Class.String);
                        this.Tokens.Add(token);
                    }

                }
                // if the char is none of the above then add it as invalid
                else
                {
                    this.Tokens.Add(new Token(curr, Token_Class.Invalid));
                }
            }
            // here we parsed teh whole source code
            this.Tokens.Add(new Token("", Token_Class.EndOfFile));
            return this.Tokens;
        }

        private bool isReservedWord(string lexeme) => this.ReservedWords.ContainsKey(lexeme);
        private bool isIdentifier(string lexeme) => Regex.IsMatch(lexeme, @"^[a-zA-Z_]([a-zA-Z_]|\d)*$");
        private bool isOperator(string lexeme) => this.Operators.ContainsKey(lexeme);
        private bool isSpecialChar(char lexeme) => this.specialChar.ContainsKey(lexeme);
        private bool isNumber(string lexeme) => Regex.IsMatch(lexeme, @"^(\d+|(\d+\.\d+))$");
        private bool isComment(string lexeme) => Regex.IsMatch(lexeme, @"^/\*.*\*/$");
        private bool isString(string lexeme) => Regex.IsMatch(lexeme, "^\".*\"$");
    }
}



