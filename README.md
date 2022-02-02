<div id="top"></div>

<div align="center">
  <a href="https://github.com/othneildrew/Best-README-Template">
  </a>

  <h3 align="center">Tiny Compiler</h3>

  <p align="center">
    A  parser and scanner for a c like programming language
    <br />
    <br />
    <br />
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#language-Description">Language Description</a></li>
        <li><a href="#language-cfg">Language CFG</a></li>
      </ul>
    </li>
  
  </ol>
</details>



<!-- ABOUT THE PROJECT -->

## About The Project
A program in TINY consists of a set of functions (any number of functions and ends with a main function),
each function is a sequence of statements including (declaration, assignment, write, read, if, repeat, function, comment, …)
each statement consists of (number, string, identifier, expression, condition, …).

### Built With

* [dotnet 4.7 - winforms](https://docs.microsoft.com/en-us/dotnet/)

<!-- GETTING STARTED -->
# Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

<!--Language-Description -->
### Language Description

- Number: any sequence of digits and maybe floats (e.g. 123 \| 554 \| 205 \| 0.23 \| …)
- String: starts with double quotes followed by any combination of characters and digits then ends with double quotes (e.g. “Hello” \| “2nd + 3rd” \| …)
- Reserved_Keywords: int \| float \| string \| read \| write \| repeat \| until \| if \| elseif \| else \| then \| return \| endl
- Comment_Statement: starts with /* followed by any combination of characters and digits then ends with */ (e.g. /*this is a comment*/ \| …)
- Identifiers: starts with letter then any combination of letters and digits. (e.g. x \| val \| counter1 \| str1 \| s2 \| …)
- Function_Call: starts with Identifier then left bracket “(“ followed by zero or more Identifier separated by “,” and ends with right bracket “)”. (e.g. sum(a,b) \| factorial(c) \| rand() \| … )
- Term: maybe Number or Identifier or function call. (e.g. 441 \| var1 \| sum(a,b) \| …)
- Arithmetic_Operator: any arithmetic operation (+ \| - \| * \| / )
- Equation: starts with Term or left bracket “(“ followed by one or more Arithmetic_Operator and Term. with right bracket “)” for each left bracket (e.g. 3+5 \| x +1 \| (2+3)*10 \| …)
- Expression: may be a String, Term or Equation (e.g. “hi” \| counter \| 404 \| 2+3 \| …)
- Assignment_Statement: starts with Identifier then assignment operator “:=” followed by Expression (e.g. x := 1 \| y:= 2+3 \| z := 2+3*2+(2-3)/1 \| …)
- Datatype: set of reserved keywords (int, float, string)
- Declaration_Statement: starts with Datatype then one or more identifiers (assignment statement might exist) separated by coma and ends with semi-colon. (e.g. int x; \| float x1,x2:=1,xy:=3; \| …)
- Write_Statement: starts with reserved keyword “write” followed by an Expression or endl and ends with semi-colon (e.g. write x; \| write 5; \| write 3+5; \| write “Hello World”; \| …)
- Read_Statement: starts with reserved keyword “read” followed by an Identifier and ends with semi-colon (e.g. read x; \| …)
- Return_Statement: starts with reserved keyword “return” followed by Expression then ends with semi-colon (e.g. return a+b; \| return 5; \| return “Hi”; \| …)
- Condition_Operator: ( less than “<” \| greater than “>” \| is equal “=” \| not equal “<>”)
- Condition: starts with Identifier then Condition_Operator then Term (e.g. z1 <> 10)
- Boolean_Operator: AND operator “&&” and OR operator “\|\|”
- Condition_Statement: starts with Condition followed by zero or more Boolean_Operator and Condition  (e.g. x < 5 && x > 1)
- If_Statement: starts with reserved keyword “if” followed by Condition_Statement then reserved keyword “then” followed by set of Statements (i.e. any type of statement: write, read, assignment, declaration, …) then Else_If_Statment or Else_Statment or reserved keyword “end”
- Else_If_Statement: same as if statement but starts with reserved keyword “elseif”
- Else_Statement: starts with reserved keyword “else” followed by a set of Statements then ends with reserved keyword “end”
- Repeat_Statement: starts with reserved keyword “repeat” followed by a set of Statements then reserved keyword “until” followed by Condition_Statement
- FunctionName: same as Identifier
- Parameter: starts with Datatype followed by Identifier  (e.g. int x)
- Function_Declaration: starts with Datatype followed by FunctionName followed by “(“ then zero or more Parameter separated by “,” then “)” (e.g. int sum(int a, int b) \| …)
- Function_Body: starts with curly bracket “{” then a set of Statements followed by Return_Statement and ends with “}”
- Function_Statement: starts with Function_Declaration followed by Function_Body
- Main_Function: starts with Datatype followed by reserved keyword “main” then “()” followed by Function_Body
- Program: has zero or more Function_Statement followed by Main_Function

<!--language-cfg-->
### Language CFG
1- Program →  FunDecls MainFun

2- FuncDecls → FunDecls FunDecl | FunDecl | ε
 - FuncDecls → FunDecl  FuncDecls’
 - FunDecls’ → FunDecl FunDecls’ | ε

3- FunDecl → DataType identifier ( Params ) CompoundStmts

4- MainFun → DataType main () CompoundStmts

5- Parmas → ParamList |  ε

6- ParamList → ParamList, Param | Param
  - ParamList→  Param ParamList`
  - ParamList`→  ,Param ParamList` | ε

7- Param → DataType identifier

8- VarDecl → DataType VarDeclList

9- VarDecList →  VarDecList, VarHeader | VarHeader
 - VarDecList →  VarHeader VarDecList`
 - VarDecList` →  ,VarHeader VarDecList`  | ε

10- VarAssign →  := Statement`` | ε

11- VarHeader →  identifier VarAssign

12- Statements → Statements Statement | Statement
 - Statements → Statement Statements`
 - Statements` → Statement Statements` | ε

13- Statement →   read identifier ;  | WriteStatement ;
| identifier Statement` ;
| IfStatments end
| RptStmt
| VarDecL ;
| ε



14- WriteStatement →  write WriteStatement`
 - WriteStatement` →  Statement``  | endl 

15- Statement` →  FunctionCall | :=  Statement`` 

16- Statement`` →   Expression | String 

17- RptStmt → repeat Statements until Conditions

18- IfStatements → If Conditions then Statements ElseIfStatments  ElseStatements  

19- ElseIfStatements → ElseIfStatements elseif Conditions then Statements | ε
 - ElseIfStatements →  ElseIfStatements`
 - ElseifStatements` →  elseif Conditions then Statements ElseifStatement` | ε

20- ElseStatements  → else Statements | ε


21- Expression → Expression AddOp Term | Term 
 - Expression -> Term Expression'
 - Expression' -> AddOp Term Expression' | ε

22- Conditions → Expression RelOp Expression BoolOp Conditions | Expression Relop Expression
 - Conditions →  Expression Conditions'
 - Conditions' →   RelOp Expression conditions''
 - Conditions'' →  BoolOp Conditions | ε

23- Term → Term MultOp Factor | Factor
 - Term → Factor Term`
 - Term` → MultOp Factor Term` | ε

24- Factor → (Expression) | identifier FactorDash | Number 

25- FactorDash → FunctionCall | ε

26- FunctionCall → identifier (ArgList)

27- CompoundStmts → { Statements  ReturnStmt}

28- ArgList →   ArgList, Arg | Arg
 - ArgList →  Arg ArgList`
 - ArgList` →  ,Arg ArgList` | ε

29- Arg -> Expression

30- Comment -> /* String */






