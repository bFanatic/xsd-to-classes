# Configuration #

Configuration
The configuration schema for XsdToClasses is included in the installation of the program. Basic features are as follows.
  * EnableDataBinding: causes the code generator to create databinding compatible code. Each class derives from INotifyPropertyChanged, each property calls the PropertyChanged event.
  * GenerateOrder: generates an order attribute that causes the XmlSerializer to enforce the order of elements in the serialized XML.
  * GenerateProperties: causes properties to be created for each element and attribute. If this is set to false, the generator creates public fields. Setting this to false will have the side effect of disabling data binding (see above)
  * CodeModifiers: code modifiers are the heart of the code generator. This configuration section specifies a set of code modifiers that are run in order after the basic code is generated. Each code modifier inspects the CodeDOM and can modify it in some way. Each code modifier can have it’s own configuration section nested in the code modifier description.
  * SchemaImporterExtensions: this set of references can change how the schema is imported. For example, it can change how a custom type is mapped to a framework type, and can even map a ComplexType to an existing framework class.