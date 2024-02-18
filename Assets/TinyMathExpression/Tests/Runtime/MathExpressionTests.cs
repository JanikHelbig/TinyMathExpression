using System;
using System.Buffers;
using Jnk.TinyMathExpression;
using NUnit.Framework;
using UnityEngine;
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
        [TestCase("10 - 5 - 2", 3)]
        [TestCase("16 / 4 / 2", 2)]
        public static void Evaluate_ReturnsCorrectValue(string expression, double expected)
        {
            var expr = new MathExpression(expression);
            Debug.Log(expr.ToInstructionString());
            double actual = expr.Evaluate();

            Assert.That(actual, Is.EqualTo(expected));
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
        public static void Evaluate_DoesNotAllocateGCMemory(string expression)
        {
            var expr = new MathExpression(expression);
            void TestDelegate() => expr.Evaluate();

            Assert.That(TestDelegate, Is.Not.AllocatingGCMemory());
        }

        [TestCase("3 + + 2")]
        [TestCase("round()")]
        [TestCase("round(2..2) + 3.0")]
        public static void Constructor_WithBadInput_ThrowsException(string expression)
        {
            void TestDelegate()
            {
                var expr = new MathExpression(expression);
            }

            Assert.That(TestDelegate, Throws.ArgumentException);
        }

        [TestCase("{0}", 1, 1)]
        public static void Evaluate_With1Parameter(string expression, double arg0, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {0}", 1, 2)]
        public static void Evaluate_With1Parameter_With2Occurrences(string expression, double arg0, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1}", 1, 1, 2)]
        public static void Evaluate_With2Parameters(string expression, double arg0, double arg1, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2}", 1, 1, 1, 3)]
        public static void Evaluate_With3Parameters(string expression, double arg0, double arg1, double arg2, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3}", 1, 1, 1, 1, 4)]
        public static void Evaluate_With4Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4}", 1, 1, 1, 1, 1, 5)]
        public static void Evaluate_With5Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5}", 1, 1, 1, 1, 1, 1, 6)]
        public static void Evaluate_With6Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6}", 1, 1, 1, 1, 1, 1, 1, 7)]
        public static void Evaluate_With7Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7}", 1, 1, 1, 1, 1, 1, 1, 1, 8)]
        public static void Evaluate_With8Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7} + {8}", 1, 1, 1, 1, 1, 1, 1, 1, 1, 9)]
        public static void Evaluate_With9Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7} + {8} + {9}", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 10)]
        public static void Evaluate_With10Parameters(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double arg9, double expected)
        {
            double value = new MathExpression(expression).Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            Assert.That(value, Is.EqualTo(expected));
        }

        [TestCase("{0} + {1} + {2} + {3} + {4} + {5} + {6} + {7} + {8} + {9}", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)]
        public static void Evaluate_With10Parameters_DoesNotAllocateGCMemory(string expression, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double arg9)
        {
            var expr = new MathExpression(expression);
            void Eval() => expr.Evaluate(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

            Assert.That(Eval, Is.Not.AllocatingGCMemory());
        }
    }
}