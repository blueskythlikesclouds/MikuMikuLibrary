using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ExpressionConverter
{
    public class ExpressionNode
    {
        public char type { get; set; }
        public string value { get; set; }
        public int priority { get; set; }
        public ExpressionNode childNode1 { get; set; }
        public ExpressionNode childNode2 { get; set; }
        public ExpressionNode childNode3 { get; set; }

        public ExpressionNode(char type, string value)
        {
            this.type = type;
            this.value = value;
            this.priority = calcPriority(type, value);
            this.childNode1 = null;
            this.childNode2 = null;
            this.childNode3 = null;
        }
        public int calcPriority(char type, string value)
        {
            switch (type){
                case 'n': case 'v': case '\0':              return 0;
                case 'f': case 'h':                         return 1;
                case 'g':                                   break;
                default:                                    return 20; //used as a symble of '(' ')'mark or ',' in in2post function
            }
            switch (value)
            {
                case "*": case "/": case "%":               return 3;
                case "+": case "-":                         return 4;
                case ">": case "<": case ">=": case "<=":   return 6;
                case "==": case "!=":                       return 7;
                case "&&":                                  return 11;
                case "||":                                  return 12;
                default:                                    return 1;
            }
        }
    }
    public class ExpressionTree
    {
        ExpressionNode root = null;
        char dependent = '\0';
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[\+\-]?\d+[\.]?\d*$");
        }
        public static bool IsVariable(string value)
        {
            return Regex.IsMatch(value, @"^t|([0-8]+\.[a-zA-Z0-9_]+)$");
        }
        public static bool IsType(string value)
        {
            return Regex.IsMatch(value, @"^v|n|f|g|h$");
        }

        public int constructWithInfix(string expression)
            // using stack translate infix expression to post expression, 
            // and construct expression tree base on the post expression string
        {
            string[] exps = splitInfixExpression(expression);

            if (exps.Length < 3 ) return 0;
            if (exps[1] != "=" || !Regex.IsMatch(exps[0], @"^[0-8]$")) return 0;
            //TODO Currently won't recognize expression start with "Position.X ="
            // only receive expressions such as "0 = ..."

            string postfix = "= " + exps[0] + " ";
            List<ExpressionNode> expStack = new List<ExpressionNode>();

            for (int i = 2; i < exps.Length; i++)
            {
                if (IsNumeric(exps[i]))
                {   //if is a number
                    postfix = postfix + "n " + exps[i] +" ";
                    continue;
                }
                if (IsVariable(exps[i]))
                {   //if is a variable, like "t" or "0.mune_b"
                    postfix = postfix + "v " + exps[i] + " ";
                    continue;
                }
                if (exps[i] == "(")
                {   //if is a '(', have the highest priority, push to the stack
                    expStack.Add(new ExpressionNode('(', exps[i]));
                    continue;
                }
                if (exps[i] == ",")
                {   //if is a ',' push to stack as a symble of another argument
                    expStack.Add(new ExpressionNode(',', exps[i]));
                    continue;
                }
                if (Regex.IsMatch(exps[i], @"^[a-zA-Z]"))
                {   //if is a name of a function, 
                    //determine by it's not a variable but start with letter
                    //have highest priority (except "()") push directly.
                    expStack.Add(new ExpressionNode('f', exps[i]));
                    continue;
                }
                if (exps[i] == ")")
                {   //end of a "()" pop everything between them
                    int num_arguments = 1;
                    while (expStack.Count > 0 && expStack.Last().type != '(')
                    {   //pop everything from stack, until meet the matched '('
                        if (expStack.Count == 0) return 0;
                        if (expStack.Last().type == ',')
                        {   // use the count of ',' to reflect how many 
                            // arguments in this function (if it is)
                            num_arguments++;
                        }
                        else
                        {
                            postfix = postfix + toPostfixNode(expStack.Last());
                        }
                        expStack.RemoveAt(expStack.Count - 1);
                    }
                    expStack.RemoveAt(expStack.Count - 1);
                    if (expStack.Count > 0 && expStack.Last().type == 'f')
                    {   //check if the "()" belongs to a function
                        switch (num_arguments)
                        {
                            case 1: break;
                            case 2:
                                expStack.Last().type = 'g'; break;
                            case 3:
                                expStack.Last().type = 'h'; break;
                            default: return 0;
                        }
                        postfix = postfix + toPostfixNode(expStack.Last());
                        expStack.RemoveAt(expStack.Count - 1);
                    }
                    continue;
                }
                { //other propability, should be only be operators, like '+'
                    ExpressionNode newNode = new ExpressionNode('g', exps[i]);
                    if (newNode.priority == 1) return 0;
                    while (expStack.Count > 0 && expStack.Last().priority != 20
                        && expStack.Last().priority <= newNode.priority)
                    {   //pop every node with a higher priority but except '('
                        postfix = postfix + toPostfixNode(expStack.Last());
                        expStack.RemoveAt(expStack.Count - 1);
                    }
                    expStack.Add(newNode); //and push this new operator
                }

            } 
            while (expStack.Count > 0 && expStack.Last().priority != 20)
            {   //finished read through the infix expression
                //pop everything remain
                postfix = postfix + toPostfixNode(expStack.Last());
                expStack.RemoveAt(expStack.Count - 1);
            }
            if (expStack.Count > 0) return 0;
            // stack should have no "()" after read through

            return constructWithPostfix(postfix);
            //use the postfix constructor...but why? we already got the postfix
        }
        private string[] splitInfixExpression(string expression)
        {
            //TODO not smart enough, only accept devided string for now
            return expression.Split(' ');
        }

        public int constructWithPostfix(string expression)
        {
            string[] exps = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (exps.Length < 4 || exps.Length % 2 == 1) return 0;
            if (exps[0] != "=" || !Regex.IsMatch(exps[1], @"^[0-8]$")) return 0;
            this.dependent = exps[1][0];
            List<ExpressionNode> expStack = new List<ExpressionNode>();
            for(int i=2;i< exps.Length; )
            {
                try
                {
                    if (!IsType(exps[i])) return 0;
                    ExpressionNode newNode = new ExpressionNode(exps[i][0], exps[i + 1]);
                    switch (exps[i][0])
                    {
                        case 'h':
                            newNode.childNode3 = expStack.Last();
                            expStack.RemoveAt(expStack.Count-1);
                            goto case 'g';
                        case 'g':
                            newNode.childNode2 = expStack.Last();
                            expStack.RemoveAt(expStack.Count-1);
                            goto case 'f';
                        case 'f':
                            newNode.childNode1 = expStack.Last();
                            expStack.RemoveAt(expStack.Count-1);
                            goto case 'n';
                        case 'v':
                        case 'n':
                            expStack.Add(newNode);
                            break;
                        default: break;
                    }
                    i += 2;
                }
                catch (ArgumentNullException)
                {
                    return 0;
                }
            }
            if (expStack.Count != 1) return 0;
            this.root = expStack[0];
            return 1;
        }

        public string toPostfix()
        {
            if (this.root == null) return null;
            return "= " + this.dependent + " " + toPostfix(this.root);
        }
        private static string toPostfix(ExpressionNode node)
        {
            switch (node.type)
            {
                case 'v': case 'n':
                    return toPostfixNode(node);
                case 'f':
                    return toPostfix(node.childNode1) + toPostfixNode(node);
                case 'g':
                    return toPostfix(node.childNode1) + toPostfix(node.childNode2) + toPostfixNode(node);
                case 'h':
                    return toPostfix(node.childNode1) + toPostfix(node.childNode2) + toPostfix(node.childNode3) + toPostfixNode(node);
                default:
                    throw new Exception();
            }
        }

        private static string toPostfixNode(ExpressionNode node)
        {
            return node.type + " " + node.value + " ";
        }

        public string toInfix()
        {
            if (this.root == null) return null;
            return paramToString(this.dependent) + " = " + toInfix(this.root, 1);
        }
        private static string toInfix(ExpressionNode node, int priority)
        {
            switch (node.type)
            {
                case 'v': case 'n':
                    return node.value;
                case 'f':
                    return node.value + "(" + toInfix(node.childNode1, node.priority) + ")";
                case 'g':
                    if (node.priority == 1)//is function
                    {
                        return node.value + "(" 
                            + toInfix(node.childNode1, node.priority) + ", "
                            + toInfix(node.childNode2, node.priority) + ")";
                    }
                    else
                    {
                        if (node.priority <= priority || priority == 1)
                            return toInfix(node.childNode1, node.priority) + " "
                                + node.value + " "
                                + toInfix(node.childNode2, node.priority);
                        else
                            return "(" + toInfix(node.childNode1, node.priority) + " "
                                + node.value + " "
                                + toInfix(node.childNode2, node.priority) + ")";
                    }
                case 'h':
                    return node.value + "("
                        + toInfix(node.childNode1, node.priority) + ", "
                        + toInfix(node.childNode2, node.priority) + ", "
                        + toInfix(node.childNode3, node.priority) + ")";
                default: throw new Exception();
            }
        }
        public string paramToString(char param)
        {
            switch (param)
            {
                case '0': return "Position.X";
                case '1': return "Position.Y";
                case '2': return "Position.Z";
                case '3': return "Rotation.X";
                case '4': return "Rotation.Y";
                case '5': return "Rotation.Z";
                case '6': return "Scale.X";
                case '7': return "Scale.Y";
                case '8': return "Scale.Z";
                default: throw new Exception();
            }
        }

        public ExpressionTree()
        {
            this.root = null;
            dependent = '\0';
        }   

        /* example code
        static void Main(string[] args)
        {
            while (true)
            {
                ExpressionTree t=new ExpressionTree();
                if (t.constructWithPostfix(Console.ReadLine()) == 0)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(t.toInfix());
                    Console.WriteLine(t.toPostfix());
                    Console.WriteLine();
                }
                if (t.constructWithInfix(Console.ReadLine()) == 0)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(t.toInfix());
                    Console.WriteLine(t.toPostfix());
                    Console.WriteLine();
                }
            }
            
        }*/
    }
}
