XsdToClasses is a simple tool that attempts to generate code from an XML Schema. This process is called Code Generation and is useful for a number of reasons. For example, if you need to specify a schema for the generation of web services or to adhere to a standard, generating the code from such a schema simplifies coding and saves a step. The XML Data can be validated against the schema before deserialization. Generating schemas and the code to serialize and deserialize them allows the programmer to specify rich object models that also have easily verifiable XML storage models.

XsdToClasses was written based on the framework tool xsd.exe. It uses the same .NET framework classes, and gives the added value that edits to the schema in Visual Studio result in generated code when the user saves the schema or when the project is built. In addition, this tool allows the user to tune the generated code to fit a number of scenarios:
  * Control generation of DataBinding code
  * including externally generated code files
  * Control code generation by excluding types from being generated
  * Extend code generation by writing custom CodeModifiers
  * Handle the case where one schema references another schema for which you’ve already generated code

  * [ChangeLog](ChangeLog.md)
  * [Usage](Usage.md)
  * [Programming](Programming.md)
  * [Configuration](Configuration.md)
  * [Example](Example.md)
  * [Errors](Errors.md)