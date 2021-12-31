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
            program.Children.Add(match(Token_Class.EndOfFile));
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

            // if the token is Main 
            if (GetTokenType() == Token_Class.Main)
            {
                InputPointer--;
                return null;
            }

            n.Children.Add(Dt);
            n.Children.Add(match(Token_Class.Identifier));
            n.Children.Add(match(Token_Class.LeftParentheses));

            n.Children.Add(Params());

            n.Children.Add(match(Token_Class.RightParentheses));

            n.Children.Add(CompoundStmts());

            return n;
        }

        Node MainFun()
        {
            Node fnMain = new Node("MainFunDecl");

            Node DT = DataType();
            if (DT == null) return null;


            fnMain.Children.Add(DT);
            fnMain.Children.Add(match(Token_Class.Main));
            fnMain.Children.Add(match(Token_Class.LeftParentheses));
            fnMain.Children.Add(match(Token_Class.RightParentheses));
            fnMain.Children.Add(CompoundStmts());


            return fnMain;
        }


        private Node CompoundStmts()
        {
            Node n = new Node("CompundStatments");

            n.Children.Add(match(Token_Class.LeftBraces));

            n.Children.Add(Statements());

            n.Children.Add(ReturnStmt());

            n.Children.Add(match(Token_Class.RightBraces));


            return n;
        }

        private Node ReturnStmt()
        {
            Node n = new Node("Return Stmt");

            n.Children.Add(match(Token_Class.Return));
            n.Children.Add(StatementDDash());
            n.Children.Add(match(Token_Class.SemiColon));

            return n;
        }

        private Node Statements()
        {
            Node n = new Node("Statements");

            Node statement = Statement();
            if (statement == null) return null;
            n.Children.Add(statement);
            n.Children.Add(StatementsDash());

            return n;
        }

        private Node StatementsDash()
        {
            Node n = new Node("StatementsDash");
            if (GetTokenType() == Token_Class.SemiColon)
            {
                n.Children.Add(match(Token_Class.SemiColon));
                n.Children.Add(Statement());
                n.Children.Add(StatementsDash());
            }


            return n;
        }

        private Node Statement()
        {
            Node n = new Node("Statement");

            Node VD = VarDecl();
            Node RBT = RptStmt();
            if (GetTokenType() == Token_Class.Read)
            {
                n.Children.Add(match(Token_Class.Read));
                n.Children.Add(match(Token_Class.Identifier));
            }
            else if (GetTokenType() == Token_Class.Write)
            {
                n.Children.Add(WriteStatment());
            }
            else if (GetTokenType() == Token_Class.Identifier)
            {
                n.Children.Add(match(Token_Class.Identifier));
                n.Children.Add(StatementDash());

            }
            else if (GetTokenType() == Token_Class.If)
            {
                // n.Children.Add(match(Token_Class.If));
                n.Children.Add(IfStatements());
                n.Children.Add(match(Token_Class.End));
            }
            else if (RBT!=null)
            {
                n.Children.Add(RBT);
            }
            else if (VD != null)
            {
                n.Children.Add(VD);
            }

            return n;

        }

        private Node RptStmt()
        {
            Node n = new Node("RptStmt");
             if (GetTokenType() == Token_Class.Repeat)
            {
                n.Children.Add(match(Token_Class.Repeat));
                n.Children.Add(Statements());
                n.Children.Add(match(Token_Class.Until));
                n.Children.Add(Conditions());
                n.Children.Add(Statements());
            }
            else
            {
                return null;
            }

            return n;
        }

        private Node WriteStatment()
        {
            Node n = new Node("WriteStatment");

            n.Children.Add(match(Token_Class.Write));
            n.Children.Add(WriteStatmentDash());
                
            return n;

        }

        private Node WriteStatmentDash()
        {
            Node n = new Node("WriteStatmentDash");

            if (GetTokenType() == Token_Class.Endl)
            {
                n.Children.Add(match(Token_Class.Endl));
                n.Children.Add(match(Token_Class.SemiColon));
            }
            else
            {
                n.Children.Add(StatementDDash());
                n.Children.Add(match(Token_Class.SemiColon));
            }

            return n;
        }

        private Node StatementDash()
        {
            Node n = new Node("StatementDash");
            Node fcall = FunctionCall();
            if (fcall != null)
            {
                n.Children.Add(fcall);
            }
            else
            {
                n.Children.Add(match(Token_Class.AssignmentOp));
                n.Children.Add(StatementDDash());
            }
            return n;
        }

        private Node FunctionCall()
        {
            Node n = new Node("FunctionCall");

            if (GetTokenType() == Token_Class.LeftParentheses)
            {
                n.Children.Add(match(Token_Class.LeftParentheses));
                n.Children.Add(ArgList());
                n.Children.Add(match(Token_Class.RightParentheses));
            }
            else
            {
                return null;
            }

            return n;

        }

        private Node StatementDDash()
        {
            Node n = new Node("StatementDDash");



            if (GetTokenType() == Token_Class.String)
            {
                n.Children.Add(match(Token_Class.String));
            }
            else
            {
                n.Children.Add(Expression());
            }
            return n;
        }
        private Node IfStatements()
        {
            Node n = new Node("IfStatements");
            n.Children.Add(match(Token_Class.If));
            n.Children.Add(Conditions());

            n.Children.Add(match(Token_Class.Then));
            n.Children.Add(Statements());
            n.Children.Add(ElseIFStatements());
            n.Children.Add(ElseStatments());

            return n;
        }
        private Node ElseIFStatements()
        {
            Node n = new Node("ElseIfStatements");
            n.Children.Add(ElseIFStatementsDash());


            return n;
        }
        private Node ElseIFStatementsDash()
        {
            Node n = new Node("ElseIfStatementsDash");
            Node statements = Statements();
            if (GetTokenType() == Token_Class.ElseIf)
            {
                n.Children.Add(match(Token_Class.ElseIf));
                n.Children.Add(Conditions());
                n.Children.Add(match(Token_Class.Then));

                if (statements != null)
                    n.Children.Add(Statements());

                n.Children.Add(ElseIFStatementsDash());
            }


            return n;
        }
        private Node ElseStatments()
        {
            Node n = new Node("ElseStatements");
            if (GetTokenType() == Token_Class.Else)
            {
                n.Children.Add(match(Token_Class.Else));
                n.Children.Add(Statements());
            }

            return n;
        }

        private Node Conditions()
        {
            Node n = new Node("Conditions");

            n.Children.Add(Expression());
            n.Children.Add(ConditionsDash());

            return n;
        }
        private Node ConditionsDash()
        {
            Node n = new Node("ConditionsDash");

            if (GetTokenType() == Token_Class.IsEqualOp)
            {
                n.Children.Add(match(Token_Class.IsEqualOp));
            }
            else if (GetTokenType() == Token_Class.NotEqualOp)
            {
                n.Children.Add(match(Token_Class.NotEqualOp));
            }
            else if (GetTokenType() == Token_Class.LessThanOp)
            {
                n.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (GetTokenType() == Token_Class.GreaterThanOp)
            {
                n.Children.Add(match(Token_Class.GreaterThanOp));
            }

            n.Children.Add(Expression());
            n.Children.Add(ConditionsDashDash());

            return n;
        }
        private Node ConditionsDashDash()
        {
            Node n = new Node("ConditionsDashDash");

            if (GetTokenType() == Token_Class.AndOp)
            {
                n.Children.Add(match(Token_Class.AndOp));
                n.Children.Add(Conditions());
            }
            else if (GetTokenType() == Token_Class.OrOp)
            {
                n.Children.Add(match(Token_Class.OrOp));
                n.Children.Add(Conditions());
            }



            return n;
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
        private Node ArgList()
        {
            Node n = new Node("ArgList");

            Node ArgNode = Arg();

            if (ArgNode == null) return null;

            n.Children.Add(ArgNode);
            n.Children.Add(ArgListDash());
            return n;
        }

        private Node ArgListDash()
        {
            Node n = new Node("ArgListDash");
            if (GetTokenType() == Token_Class.Coma)
            {
                n.Children.Add(match(Token_Class.Coma));
                n.Children.Add(Arg());
                n.Children.Add(ArgListDash());
            }

            return n;

        }

        private Node Arg()
        {
            Node n = new Node("Param");
            Node exp = Expression();
            if (exp == null) return null;
            n.Children.Add(exp);
            return n;

        }
        private Node VarDecl()
        {
            Node n = new Node("VarDecl");

            Node Dt = DataType();

            if (Dt == null) return null;

            n.Children.Add(DataType());
            n.Children.Add(VarDeclList());

            return n;
        }

        private Node VarHeader()
        {
            Node n = new Node("VarHeader");

            n.Children.Add(match(Token_Class.Identifier));
            n.Children.Add(VarAssign());

            return n;
        }

        private Node VarAssign()
        {
            Node n = new Node("VarAssign");


            if (GetTokenType() == Token_Class.AssignmentOp)
            {

                //   n.Children.Add(match(Token_Class.SemiColon));
                n.Children.Add(match(Token_Class.AssignmentOp));
                n.Children.Add(StatementDDash());

            }

            return n;

        }

        private Node VarDeclList()
        {
            Node n = new Node("VarDeclList");

            n.Children.Add(VarHeader());
            n.Children.Add(VarDeclListDash());

            return n;
        }

        private Node VarDeclListDash()
        {
            Node n = new Node("VarDecListDash");

            if (GetTokenType() == Token_Class.Coma)
            {
                n.Children.Add(match(Token_Class.Coma));
                n.Children.Add(VarHeader());
                n.Children.Add(VarDeclListDash());
            }

            return n;
        }

       
        private Node Expression()
        {
            Node n = new Node("Expression");
            Node term = Term();
            if (term == null) return null;
            n.Children.Add(term);
            n.Children.Add(ExpressionDash());
            return n;
        }
        private Node ExpressionDash()
        {
            Node n = new Node("ExpressionDash");
            if (GetTokenType() == Token_Class.PlusOp)
            {
                n.Children.Add(match(Token_Class.PlusOp));
                Console.WriteLine(GetTokenType());
                n.Children.Add(Term());
                n.Children.Add(ExpressionDash());

            }
            else if (GetTokenType() == Token_Class.MinusOp)
            {
                n.Children.Add(match(Token_Class.MinusOp));
                n.Children.Add(Term());
                n.Children.Add(ExpressionDash());
            }
            return n;
        }

        private Node Term()
        {
            Node n = new Node("Term");
            Node factor = Factor();
            if (factor == null) return null;
            n.Children.Add(factor);
            n.Children.Add(TermDash());
            return n;
        }
        private Node TermDash()
        {
            Node n = new Node("TermDash");
            if (GetTokenType() == Token_Class.MultiplyOp)
            {
                n.Children.Add(match(Token_Class.MultiplyOp));
                n.Children.Add(Factor());
                n.Children.Add(TermDash());

            }
            else if (GetTokenType() == Token_Class.DivideOp)
            {
                n.Children.Add(match(Token_Class.DivideOp));
                n.Children.Add(Factor());
                n.Children.Add(TermDash());
            }
            // return epsilon
            return n;
        }
        private Node Factor()
        {
            Node n = new Node("Factor");

            Console.WriteLine(GetTokenType());

            Node fn = FunctionCall();

            if (GetTokenType() == Token_Class.Number)
            {
                n.Children.Add(match(Token_Class.Number));

            }
            else if (GetTokenType() == Token_Class.Identifier)
            {
                n.Children.Add(match(Token_Class.Identifier));
                n.Children.Add(FactorDash());
            }
            else if (GetTokenType() == Token_Class.LeftParentheses)
            {
                n.Children.Add(match(Token_Class.LeftParentheses));
                n.Children.Add(Expression());
                n.Children.Add(match(Token_Class.RightParentheses));
            }
            else if (fn != null)
            {
                n.Children.Add(fn);
            }
            return n;
        }

        private Node FactorDash()
        {
            Node n = new Node("factore Dash");

            Node fn = FunctionCall();

            if (fn != null)
            {
                n.Children.Add(fn);
            }

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
                    Console.WriteLine(InputPointer);
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
