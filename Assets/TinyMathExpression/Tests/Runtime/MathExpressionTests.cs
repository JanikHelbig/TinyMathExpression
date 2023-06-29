using Jnk.TinyMathExpression;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = NUnit.Framework.Is;

namespace TinyMathExpressions.Tests.Runtime
{
    public static class MathExpressionTests
    {
        [TestCase("3 +  2", 5)]
        [TestCase("3 + -2", 1)]
        [TestCase("3 -  2", 1)]
        [TestCase("3 - -2", 5)]
        [TestCase("3 *  2", 6)]
        [TestCase("3 * -2", -6)]
        [TestCase("3 /  2", 1.5)]
        [TestCase("3 / -2", -1.5)]
        [TestCase("3 ^  2", 9)]
        [TestCase("3 ^ -2", 0.1111111111111111)]
        [TestCase("round(1.3)", 1)]
        [TestCase("floor(1.7)", 1)]
        [TestCase("ceil(1.3)", 2)]
        [TestCase("sqrt(4)", 2)]
        [TestCase("log(4)", 1.3862943611198906)]
        [TestCase("sin( 0.5 * PI )", 1)]
        [TestCase("cos( PI )", -1)]
        [TestCase("(3 * 4) - 5", 7)]
        [TestCase("3 * (4 - 5)", -3)]
        public static void Evaluate(string expression, double expected)
        {
            double value = new MathExpression(expression).Evaluate();
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("3 +  2")]
        [TestCase("3 + -2")]
        [TestCase("3 -  2")]
        [TestCase("3 - -2")]
        [TestCase("3 *  2")]
        [TestCase("3 * -2")]
        [TestCase("3 /  2")]
        [TestCase("3 / -2")]
        [TestCase("3 ^  2")]
        [TestCase("3 ^ -2")]
        [TestCase("round(1.3)")]
        [TestCase("floor(1.7)")]
        [TestCase("ceil(1.3)")]
        [TestCase("sqrt(4)")]
        [TestCase("log(4)")]
        [TestCase("sin( 0.5 * PI )")]
        [TestCase("cos( PI )")]
        [TestCase("(3 * 4) - 5")]
        [TestCase("3 * (4 - 5)")]
        public static void Evaluate_Does_Not_Allocate_GC_Memory(string expression)
        {
            var expr = new MathExpression(expression);
            void Eval() => expr.Evaluate();

            Assert.That(Eval, Is.Not.AllocatingGCMemory());
        }

        [TestCase("{0}", 1, 1)]
        public static void Evaluate_With_01_Parameter(string expression, double var1, double expected)
        {
            double value = new MathExpression(expression).Evaluate(var1);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1}", 1, 1, 2)]
        public static void Evaluate_With_02_Parameters(string expression, double arg0, double arg1, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2}", 1, 1, 1, 3)]
        public static void Evaluate_With_03_Parameters(string expression, double arg0, double arg1, double arg2, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3}", 1, 1, 1, 1, 4)]
        public static void Evaluate_With_04_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4}", 1, 1, 1, 1, 1, 5)]
        public static void Evaluate_With_05_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5}", 1, 1, 1, 1, 1, 1, 6)]
        public static void Evaluate_With_06_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6}", 1, 1, 1, 1, 1, 1, 1, 7)]
        public static void Evaluate_With_07_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7}", 1, 1, 1, 1, 1, 1, 1, 1, 8)]
        public static void Evaluate_With_08_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7} + {8}", 1, 1, 1, 1, 1, 1, 1, 1, 1, 9)]
        public static void Evaluate_With_09_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7} + {8} + {9}", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 10)]
        public static void Evaluate_With_10_Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double arg9, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7} + {8} + {9}", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)]
        public static void Evaluate_With_10_Parameters_Does_Not_Allocate_GC_Memory(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double arg9)
        {
            var expr = new MathExpression(expression);
            void Eval() => expr.Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

            Assert.That(Eval, Is.Not.AllocatingGCMemory());
        }
    }
}