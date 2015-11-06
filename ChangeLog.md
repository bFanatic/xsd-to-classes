# Change Log #
  * 1.2.23 25/07/10: Added ImportFixer code modifier to take the place of the combination of "RemoveSpecifiedTypes" and "ModifyImports"; this code modifier will remove all classes that are generated from a given namespace, and modify the imports to import the library where the code is generated. This handles situations where you have a massive number of classes generated in an imported library, and changes to those classes (wither renaming or additions) causes a maintenance problem updating all the references. Documentation to follow.
  * 1.2.22 18/06/10: Added RemoveXmlTypeAttribute Code Modifier to remove the XmlTypeAttribute in generated code for classes that implement IXmlSerializable. An error serializing these classes look like this:
> > `Only XmlRoot attribute may be specified for the type <type>. Please use XmlSchemaProviderAttribute to specify schema type.`
  * 1.2.21 12/05/10:
    * added Support for Visual Studio 2010
    * added a "date generated" string to generated files
    * fixed uninstall problem (see Patch section above), generated patch assembly
  * 1.1.21 26/10/09: minor bug fix
  * 1.1.17 04/11/08:
    * support for Visual Studio Express Editions
    * added non serialized attribute to members that should not be serialized using a binary formatter
    * Using generated code with WCF endpoints using a binary encoding caused errors for fields that should not be serialized.
  * 1.1.15 16/01/08: support for Visual Studio 2008
  * 1.0.21.0: Set the following defaults:
    * EnableDataBinding = true;
    * GenerateComplexTypes = true;
    * GenerateComplexTypesSpecified = true;
    * GenerateDebuggableCode = true;
    * GenerateProperties = true;
    * GenerateSoapTypes = false;
  * 1.0.20.0: Bug fix: parameters for DataBinding, order and Properties being set but not honoured
  * 1.0.19.0: Added parameters to config to enable and disable DataBinding, Order and Properties
  * 1.0.17
    * Added the ability to import types from foreign schemas without generating code
    * Import types from other assemblies using ModifyImports code modifier
    * Exclude types from being generated using the RemoveSpecifiedTypes code modifier
  * 1.0.11: added generic collections
  * 1.0.10: for added support for VS 2008
  * 1.0.7: added import of externally referenced schemas (via the import schema directive)
  * 1.0.5: bug fixes and refinements
  * 1.0.3 03/04/06: added extensible support for code modifiers, created the XmlGridControl as an example project
  * 1.0.2: added support from "tree" view of generated file and config file
  * 1.0.1: added support for configuration files
  * 1.0.0: initial edition automates the generation of code from an XSD Schema