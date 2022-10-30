# Overview

This repository contains the code which implements a typical Business Rules Engine System using Azure Functions and Microsoft Rules Engine.

# Tech Stack

Following Libraries/ Tech Stack are used to build the code

| Library/Technology/Platform| Short Description|Reference|
|--------------|-------------|-----------------|
|Azure Functions V 4.0 with .Net 6.0| Azure functions is a cloud service available on demand that provides capability to run the custom made peices of code and the functions platform provides all latest and greatest in the infrastructure and security| [Azure Functions Documention](https://learn.microsoft.com/en-us/azure/azure-functions/)|
| Microsoft Rules Engine| Rules Engine is a library/NuGet package for abstracting business logic/rules/policies out of a system. It provides a simple way of giving you the ability to put your rules in a store outside the core logic of the system, thus ensuring that any change in rules don't affect the core system. | [Rules Engine Documentation](https://microsoft.github.io/RulesEngine/)|
| Fluent Validations| This library allows developers to write model validation rules in a fluent style.| [Fluent Validations Documentation](https://fluentvalidation.net/)|
|Blob Storage Library| This library allows developer to communicate with the azure blob storage|[Blob Storage Nuget Package](https://www.nuget.org/packages/Azure.Storage.Blobs/)|
|Automapper| AutoMapper in C# is a library used to map data from one object to another. It acts as a mapper between two objects and transforms one object type into another.| [Automapper Documentation](https://automapper.org/)| 
|XUnit.net| XUnit.net is a free, community focused unit testing framework for the .NET framework| [XUnit.net Documentation](https://xunit.net/)|
|Fluent Assertions| Fluent Assertions present a cleaner way of writing assertions in the tests| [Fluent Assertions](https://fluentassertions.com/)|

# Opinions

1. The Code for this implementation has been written using the concepts of the Test driven development, meaning, the tests are written first and then the code statisfying the tests are written. The Red- Green - Refactor paradigm was followed and the structure of code is derived through tests i.e. design through tests paradigm of TDD

2. Azure Functions provide an opinionated way of writing pieces of code where the inputs and outputs are generally implemented using bindings. Following my personal style of coding, I only use the primary input binding for the functions all the other communications with the external systems is driven through repository/ handler design patterns.
