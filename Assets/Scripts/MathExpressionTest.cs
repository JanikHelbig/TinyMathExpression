using UnityEngine;
using Jnk.TinyMathExpression;

public class MathExpressionTest : MonoBehaviour
{
    private void Start()
    {
        var expression = new MathExpressionF("round(240 * 1.1 ^ {0})");
        float result = expression.Evaluate(3.0f, 4.0f);
    }
}