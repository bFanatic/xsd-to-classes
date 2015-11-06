XsdToClasses installs as a Visual Studio Add-In.
  1. Select an XSD File. In the Properties dialog for the file there is an entry named “Custom Tool”.
  1. Enter ”XsdToClasses” in this entry.
If you wish the code to be generated in a specific namespace, in order to avoid collision of generated code type names with other names, you can either place the schema file in a subdirectory (in which case the schema’s namespace will take on the value of the directory), or specify the namespace using the “Custom Tool Namespace” field in the property grid.

![https://sites.google.com/a/bluetoque.ca/bluetoque-software/_/rsrc/1273728955589/Products/XsdToClasses/screenshot.png](https://sites.google.com/a/bluetoque.ca/bluetoque-software/_/rsrc/1273728955589/Products/XsdToClasses/screenshot.png)

After entering the text, the Add-In will activate for the first time, and attempt to generate code from your schema. It will generate two files:
`<schemaName>.Generated.cs`:  This file is the generated code. In the case that there was an error generating the code, the error and call stack information will be written as a comment in this file. See below for error information.
`<schemaName>.Generated.xml`: this file contains configurable parameters that affect the code generation. This is where parameters are set that change how or what code is generated.