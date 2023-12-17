# What is it? 🚀

**privatelike** provides an attribute, generator and analyzer to declare "private-like" types in C# (internal but producing errors if not used as if they were declared as private). This can be used to still enforce the same acessibility as using the `private` keyword in scenarios where types actually still need to be accessible from elsewhere in the assembly. For instance, this is the case for some source generators (eg. the COM generators, or the ones from [ComputeSharp](https://github.com/Sergio0694/ComputeSharp)). Using the `[privatelike]` attribute can things simpler in this scenario: the source generators can just suppress the errors, but the rest of the code will not, and should avoid referencing these types incorrectly.

# How to use it 📖

Simply declare a type as follows:

```csharp
[privatelike] partial struct MyPrivateLikeType
{  
}
```

The type will end up using `internal`, but referencing it from a place where it would've been an error if it had been using `private` will result in an error as well.