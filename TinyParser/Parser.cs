using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(FunDecls());
            program.Children.Add(MainFun());
            return program;
        }
        Node FunDecls()
        {
            Node n = new Node("FunDecls");

            n.Children.Add(FunDecl());
            n.Children.Add(FunDelcsDash());

            return n;
        }

        private Node FunDelcsDash()
        {
            Node n = new Node("FunDeclsDash");

            Node Decl = FunDecl();
            if (Decl != null)
            {
                n.Children.Add(Decl);
                n.Children.Add(FunDelcsDash());
            }
            // if null means epsilon just return the node

            return n;
        }

        private Node FunDecl()
        {
            Node n = new Node("FunDecl");

            Node Dt = DataType();

            if (Dt == null) return null;

            n.Children.Add(Dt);
            n.Children.Add(match(Token_Class.Identifier));
            n.Children.Add(match(Token_Class.LeftParentheses));

            n.Children.Add(Params());

            n.Children.Add(match(Token_Class.RightParentheses));

            //n.Children.Add(CompoundStmts());

            return n;
        }

        private Node CompoundStmts()
        {
            Node n = new Node("CompundStatments");

            n.Children.Add(match(Token_Class.LeftBraces));

            n.Children.Add(Statments());

            n.Children.Add(match(Token_Class.RightBraces));

            return n;
        }

        private Node Statments()
        {
            Node n = new Node("Statments");

            n.Children.Add(Statment());

            return null;
        }

        private Node Statment()
        {
            Node n = new Node("Statment");

            if (GetTokenType() == Token_Class.Read)
            {
                
            }else if (GetTokenType()==Token_Class.Write)
            {

            }

            return null;
        }

        private Node Params()
        {
            Node n = new Node("Params");

            Node ParamL = ParamList();

            if (ParamL == null) return null;

            n.Children.Add(ParamL);

            return n;
        }

        private Node ParamList()
        {
            Node n = new Node("ParamList");

            Node ParamNode = Param();

            if (ParamNode == null) return null;

            n.Children.Add(ParamNode);
            n.Children.Add(ParamListDash());
            return n;
        }

        private Node ParamListDash()
        {
            Node n = new Node("ParamListDash");
            if (GetTokenType() == Token_Class.Coma)
            {
                n.Children.Add(match(Token_Class.Coma));
                n.Children.Add(Param());
                n.Children.Add(ParamListDash());
            }

            return n;

        }

        private Node Param()
        {
            Node n = new Node("Param");

            Node dt = DataType();

            if (dt == null) return null;


            n.Children.Add(dt);
            n.Children.Add(match(Token_Class.Identifier));
            return n;

        }

        private Node DataType()
        {
            Node n = new Node("Datatype");

            if (GetTokenType() == Token_Class.Int)
            {
                n.Children.Add(match(Token_Class.Int));
            }
            else if (GetTokenType() == Token_Class.Float)
            {
                n.Children.Add(match(Token_Class.Float));
            }
            else if (GetTokenType() == Token_Class.String)
            {
                n.Children.Add(match(Token_Class.String));
            }
            else
            {
                return null;
            }

            return n;

        }
        private Token_Class GetTokenType()
        {
            if (GetCurrentToken() == null) return Token_Class.Invalid;

            return GetCurrentToken().token_type;
        }
        private Token GetCurrentToken()
        {
            if (InputPointer < TokenStream.Count) return TokenStream[InputPointer];
            return null;
        }
        Node MainFun()
        {
            return null;
        }

        Node Header()
        {
            Node header = new Node("Header");
            // write your code here to check the header sructure
            return header;
        }
        Node DeclSec()
        {
            Node declsec = new Node("DeclSec");
            // write your code here to check atleast the declare sturcure 
            // without adding procedures
            return declsec;
        }
        Node Block()
        {
            Node block = new Node("block");
            // write your code here to match statements
            return block;
        }

        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
