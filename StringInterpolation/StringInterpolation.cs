using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicExpresso;

namespace Kea
{
    /// <summary>
    /// Subtitute c# expressions enclosed by {{ }} by its values 
    /// </summary>
    public static class StringInterpolation
    {
        /// <summary>
        /// Evaluate an expression given the expression and a collection of variables.
        /// Expression can be any C# expression referencing the given variables.
        /// If the expression contains a " | ", the string after the pipe will indicate the string format. 
        /// 
        /// Example:
        /// 
        /// x + 1 | 0.00
        /// 
        /// Returns the expression x + 1 with the "0.00" string format.
        /// 
        /// If the expression evaluation results on an exception this method returns as if the expression was evaluated to a null
        /// </summary>
        /// <param name="Expression">The expression to evaluate</param>
        /// <param name="Variables">Variables available to the expression</param>
        /// <returns>A tuple with the evaluated value and the string format if any</returns>
        public static Tuple<object, string> Eval(string Expression, IEnumerable<KeyValuePair<string, object>> Variables)
        {
            var Split = Expression.LastIndexOf(" | ");
            string StringFormat = null;
            if (Split != -1)
            {
                StringFormat = Expression.Substring(Split + 3);
                Expression = Expression.Substring(0, Split);
            }

            var I = new Interpreter();
            foreach (var V in Variables)
            {
                I.SetVariable(V.Key, V.Value);
            }

            try
            {
                var Ret = I.Eval(Expression);
                return Tuple.Create(Ret, StringFormat);
            }
            catch (Exception)
            {
                return Tuple.Create<object, string>(null, null);
            }
        }

        /// <summary>
        /// Evaluate all expressions inside a string enclosed by {{ }}
        /// 
        /// Example:
        /// 
        /// If x = 1.0
        /// 
        /// the string
        /// 
        /// "the value is {{x + 1 | 0.00}}"
        /// 
        /// evaluates to
        /// 
        /// "the value is 2.0"
        /// </summary>
        /// <param name="text">The string to be interpolated</param>
        /// <param name="variables">Variables available inside expressions</param>
        /// <returns></returns>
        public static string Interpolate(string text, IEnumerable<KeyValuePair<string, object>> variables)
        {
            StringBuilder Result = new StringBuilder();
            int currentIndex = 0;

            while (currentIndex < text.Length)
            {
                //Encuentra el siguiente {{
                var NextOpen = text.IndexOf("{{", currentIndex);
                if (NextOpen == -1)
                {
                    Result.Append(text, currentIndex, text.Length - currentIndex);
                    break;
                }
                else
                {
                    //Agrega el texto que esta antes de este
                    Result.Append(text, currentIndex, NextOpen - currentIndex);
                    //Encuentra el siguiente }} despues del {{ encontrado
                    var NextClosed = text.IndexOf("}}", currentIndex + 2);
                    if (NextClosed == -1)
                        throw new ArgumentException("Found a {{ without a }} on index" + currentIndex.ToString());
                    else
                    {
                        //Extrae el contenido dentro de los curlys
                        var Expression = text.Substring(NextOpen + 2, NextClosed - NextOpen - 2);
                        var ExpR = Eval(Expression, variables);


                        if (ExpR.Item2 != null)
                            Result.Append(((dynamic)ExpR.Item1).ToString(ExpR.Item2));
                        else
                            Result.Append(ExpR.Item1);
                    }
                    currentIndex = NextClosed + 2;
                }
            }
            return Result.ToString();
        }
    }
}
