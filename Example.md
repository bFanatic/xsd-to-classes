# Example #

## Schema imports ##
When one schema imports another, the code generator can not tell if code has already been generated for that import. In fact, since a schema can contain multiple namespaces internally, the code generator has no knowledge of the import, only that there are types specified which need to be generated.
How XsdToClasses solves this is by two code modifiers: RemoveSpecifiedTypes and ModifyImports.

RemoveSecifiedTypes has a configuration that allows the user to specify which types from the imrpoted schema to skip. The modifier does this by performing a text match on the class name of the generated type.
ModifyImports adds a “Using” statement to the code to reference the generated code in another namespace. If the code is generated in the same namespace then this will not be necessary.

Below is an example of this strategy.

```
<?xml version="1.0" encoding="utf-16"?>
<Configuration xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://BlueToque.ca/XsdToClasses.Configuration">
  <CodeModifiers>
    <CodeModifier Type="CodeGeneration.CodeModifiers.ConvertArraysToCollections" Assembly="CodeGeneration, Version=1.0.1.0, Culture=neutral, PublicKeyToken=46a422f5b652f20b" />

    <CodeModifier
      Type="CodeGeneration.CodeModifiers.RemoveDebuggerAttribute"
      Assembly="CodeGeneration, Version=1.0.1.0, Culture=neutral, PublicKeyToken=46a422f5b652f20b" />`

    <CodeModifier
      Type="CodeGeneration.CodeModifiers.RemoveSpecifiedTypes"
      Assembly="CodeGeneration, Version=1.0.1.0, Culture=neutral, PublicKeyToken=46a422f5b652f20b" >
      <RemoveSpecifiedTypesOptions xmlns="http://BlueToque.ca/CodeGeneration/CodeModifiers/RemoveSpecifiedTypes.Options">
        <Type Name="TagType" />
        <Type Name="TagsType" />
        <Type Name="TagTypeCollection" />
      </RemoveSpecifiedTypesOptions>
    </CodeModifier>

    <CodeModifier
      Type="CodeGeneration.CodeModifiers.ModifyImports"
      Assembly="CodeGeneration, Version=1.0.1.0, Culture=neutral, PublicKeyToken=46a422f5b652f20b" >
      <ModifyImportsOptions xmlns="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options">
        <AddImport
          Name="EmerGeo.MessageDataLib" />
      </ModifyImportsOptions>
    </CodeModifier>
  </CodeModifiers>

  <SchemaImporterExtensions />

</Configuration>
```
Figure 2: a basic configuration file

The first part of the file defines the root element and the namespaces.

The CodeModifiers element defines the group of code modifiers. The four code modifiers present are the defaults that are found in the file when it is generated. The “RemoveSpecifiedTypesOptions” element causes the code generator to remove all instances of the given types from the CodeDOM after the basic code is generated. Each `<Type>` element specifies the name of a type to remove from the CodeDOM.
The ModifyImports element allows the user to specify “using” statements for the generated code. Each `<AddImport>` element specified the namespace to import. Note that this does not add the reference to that namespace to the project; the programmer must still perform that step.