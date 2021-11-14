using UnityEngine;
using Jnk.TinyMathExpression;

public class MathExpressionTest : MonoBehaviour
{
    private void Start()
    {
        var expression = new MathExpression("round(240 * 1.1 ^ {0})");
        double result = expression.Evaluate(3.0);

        for (var i = 0; i < 10; i++)
            Debug.Log($"{string.Format(expression.ToString(), i)} = {expression.Evaluate(i)}");
    }
}