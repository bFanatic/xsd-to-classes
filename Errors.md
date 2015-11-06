# Errors #

Possible reasons for an error generating code
  * Schema is not valid. If the schema is not valid the call stack will most likely have some sort of schema parsing error.
  * Schema does not resolve to a valid representation in code.

There are many examples of schemas that cannot be represented as code. In general the XSD language can represent structures that do not have a singular code representation. The XsdToClasses code uses framework classes to attempt the resolve these into code structures, but in many cases this is not possible.