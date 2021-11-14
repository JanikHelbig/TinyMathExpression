# TinyMathExpression
A simple math expression evaluator. Expressions are parsed and transformed into postfix notation when calling the constructor. Evaluating the expressions does not allocate any garbage and should be reasonably performant.

## Installation
You can install this package via the Unity Package Manager "Add package from git URL" option by using this URL:
`https://github.com/JanikHelbig/TinyMathExpression.git?path=Assets/TinyMathExpression` or add `"com.jnk.tinymathexpression" : "https://github.com/JanikHelbig/TinyMathExpression.git?path=Assets/TinyMathExpression"` to your `Packages/manifest.json`.

Use the `#*.*.*` postfix to use a specific release of the package, e.g. `https://github.com/JanikHelbig/TinyMathExpression.git?path=Assets/TinyMathExpression#1.0.0`.

## Usage

```cs
// Create a new MathExpression
var expression = new MathExpression("round(240 * 1.1 ^ {0})");

// Evaluate the expression
double result = expression.Evaluate(3.0);
```
The number of arguments passed to `Evaluate()` has to match the number of parameters in the expression.

## Syntax

The following syntax is supported:
```cs
// Parameters
"2 + {0}" // [{0}, {1}, ..., {9}]

// Operators
"3 + 2"
"3 - 2"
"3 * 2"
"3 / 2"
"3 ^ 2"

// Parenthesis
"(2 + 3) * 2"

// Functions
"round(1.3)"
"floor(1.3)"
"ceil(1.3)"
"sqrt(4)"
"log(4)"
"sin(1)"
"cos(1)"

// Constants
"2 * PI" // Equivalent to Math.PI
"2 * E"  // Equivalent to Math.E
```
Spaces are not required.
