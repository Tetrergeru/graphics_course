using System;
using System.Collections.Generic;
using System.Globalization;

namespace GraphFunc
{
    public static class Utils
    {
        private static Func<float, float, float> GetFunc(string func)
        {
            var list = func.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return (x, y) =>
            {
                var stack = new Stack<float>();
                foreach (var command in list)
                {
                    switch (command.ToLower())
                    {
                        case "x":
                            stack.Push(x);
                            break;
                        case "y":
                            stack.Push(y);
                            break;
                        case "e":
                            stack.Push((float) Math.E);
                            break;
                        case "pi":
                            stack.Push((float) Math.PI);
                            break;
                        case "+":
                            stack.Push(stack.Pop() + stack.Pop());
                            break;
                        case "--":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push(a - b);
                            break;
                        }
                        case "*":
                            stack.Push(stack.Pop() * stack.Pop());
                            break;
                        case "-":
                            stack.Push(-stack.Pop());
                            break;
                        case "/":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push(a / b);
                            break;
                        }
                        case "^":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push((float) Math.Pow(a, b));
                            break;
                        }
                        case "sin":
                            stack.Push((float) Math.Sin(stack.Pop()));
                            break;
                        case "cos":
                            stack.Push((float) Math.Cos(stack.Pop()));
                            break;
                        case "tg":
                            stack.Push((float) Math.Tan(stack.Pop()));
                            break;
                        case "lg":
                            stack.Push((float) Math.Log(stack.Pop(), 2));
                            break;
                        case "log":
                        {
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push((float) Math.Log(a, b));
                            break;
                        }
                        default:
                            stack.Push(float.Parse(command, new CultureInfo("en-US")));
                            break;
                    }
                }

                return stack.Pop();
            };
        }

    }
}