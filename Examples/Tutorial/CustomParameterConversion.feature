Custom conversion of parameters based on types

You can create your own parameter converters by subclassing StorEvil.Extensibility.CustomParameterConverter.
This example converts floating point numbers.

Scenario: custom float conversion
  We can parse floats like 3.14159265
  and the result should be between 3.14 and 3.15