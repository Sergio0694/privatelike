![image](https://user-images.githubusercontent.com/10199417/197341200-3810e13c-9104-4911-90fc-b8add0862458.png)
<br/>
[![.NET](https://github.com/Sergio0694/PrivateLike/workflows/.NET/badge.svg)](https://github.com/Sergio0694/PrivateLike/actions) [![NuGet](https://img.shields.io/nuget/dt/PrivateLike.svg)](https://www.nuget.org/stats/packages/PrivateLike?groupby=Version) [![NuGet](https://img.shields.io/nuget/vpre/PrivateLike.svg)](https://www.nuget.org/packages/PrivateLike/)

# What is it? 🚀

**PrivateLike** provides generated, source-only polyfills for C# language features, to easily use all runtime-agnostic features downlevel. The package is distributed as a source generator, so that it will automatically detect which polyfills are needed depending on the target framework and project in use: just add a reference to **PrivateLike**, set your C# language version to latest, and have fun!

# TLDR? What is this for? ✨

Put simply: are you working on .NET Framework, or UWP, or some other older .NET runtime and still would like to use all the cool new features that C# 12 has? Well this library lets you do just that! It will generate for you all the "magic types" that the C# compiler needs to "see" in order for it to allow using new language features even if you're not using the latest framework out there.

Here's an example of some of the new features that **PrivateLike** can enable downlevel:

![image](https://user-images.githubusercontent.com/10199417/198630498-df1e215c-6788-4aef-8ba5-b0b71772233e.png)

> **Note**: not all the new C# features can be "tricked" this way (eg. those requiring runtime support, such as [static abstract members](https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members), still won't work). But almost everything else will (eg. nullability annotations, pattern matching, etc.), and this applies to a big number of new C# features. Try **PrivateLike** out, don't get stuck on C# 6.0 or other older versions! 🎉

> **Note**: use on .NET Framework 3.5 is particularly limited due to shortcomings of the BCL there. In particular, the `System.Range` type will not be generated unless `System.ValueTuple` is referenced (meaning that eg. list patterns won't work by default), and some features such as records will not be usable at all due to the C# compiler missing some necessary APIs that cannot be polyfilled. All other features should work as expected.

# Documentation 📖

**PrivateLike** includes the following polyfills:
- Nullability attributes (for [nullable reference types](https://learn.microsoft.com/dotnet/csharp/nullable-references)):
  - `[AllowNull]`
  - `[DisallowNull]`
  - `[DoesNotReturn]`
  - `[DoesNotReturnIf]`
  - `[MaybeNull]`
  - `[MaybeNullWhen]`
  - `[MemberNotNull]`
  - `[MemberNotNullWhen]`
  - `[NotNull]`
  - `[NotNullIfNotNull]`
  - `[NotNullWhen]`
- `Index` and `Range` (see [indices and ranges](https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/ranges-indexes))
- `[UnscopedRef]` (see [low-level struct improvements](https://github.com/dotnet/csharplang/blob/main/proposals/low-level-struct-improvements.md))
- Required members (see [required modifier](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/required))
  - `[RequiredMember]`
  - `[SetsRequiredMembers]`
- `[CompilerFeatureRequired]` (needed to support several features)
- `[IsExternalInit]` (needed for [init-only properties](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/init))
- `[SkipLocalsInit]` (see [docs](https://learn.microsoft.com/dotnet/csharp/language-reference/attributes/general#skiplocalsinit-attribute))
- Interpolated string handlers (see [docs](https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/interpolated-string-handler))
  - `[InterpolatedStringHandler]`
  - `[InterpolatedStringHandlerArgument]`
- `[CallerArgumentExpression]` (see [docs](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-10.0/caller-argument-expression))
- `[RequiresPreviewFeatures]` (needed for [preview features](https://github.com/dotnet/designs/blob/main/accepted/2021/preview-features/preview-features.md))
- `[AsyncMethodBuilder]` (needed for [custom method builder types](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-10.0/async-method-builders))
- `[StringSyntax]` (needed to enable [syntax highlight in the IDE](https://github.com/dotnet/runtime/issues/62505))
- `[ModuleInitializer]` (needed to enable [custom module initializers](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-9.0/module-initializers))
- `[RequiresLocation]` (needed to enable [ref readonly parameters](https://github.com/dotnet/csharplang/issues/6010))
- `[CollectionBuilder]` (needed for [collection expressions](https://github.com/dotnet/csharplang/issues/5354))
- `[Experimental]` (needed for [experimental features](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/experimental-attribute))

To leverage them, make sure to bump your C# language version. You can do this by setting the `<LangVersion>` MSBuild property in your project. For instance, by adding `<LangVersion>12.0</LangVersion>` (or your desired C# version) to the first `<PropertyGroup>` of your .csproj file. For more info on this, [see here](https://sergiopedri.medium.com/enabling-and-using-c-9-features-on-older-and-unsupported-runtimes-ce384d8debb), but remember that you don't need to manually copy polyfills anymore: simply adding a reference to **PrivateLike** will do this for you automatically.

It also includes the following optional runtime-supported polyfills:
- Reflection annotation attributes (see [docs](https://learn.microsoft.com/dotnet/core/deploying/trimming/prepare-libraries-for-trimming)):
  - `[DynamicallyAccessedMembers]`
  - `[DynamicDependency]`
  - `[RequiresUnreferencedCode]`
  - `[RequiresDynamicCode]`
  - `[UnconditionalSuppressMessage]`
  - `[RequiresAssemblyFiles]`
- `[StackTraceHidden]` (see [here](https://makolyte.com/csharp-exclude-exception-throw-helper-methods-from-the-stack-trace/))
- `[UnmanagedCallersOnly]` (see [docs](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.unmanagedcallersonlyattribute)))
- Platform support annotation attributes (see [docs](https://learn.microsoft.com/dotnet/standard/analyzers/platform-compat-analyzer)):
  - `[ObsoletedOSPlatform]`
  - `[SupportedOSPlatform]`
  - `[SupportedOSPlatformGuard]`
  - `[TargetPlatform]`
  - `[UnsupportedOSPlatform]`
  - `[UnsupportedOSPlatformGuard]`
- `[SuppressGCTransition]` (see [here](https://devblogs.microsoft.com/dotnet/improvements-in-native-code-interop-in-net-5-0/))
- `[DisableRuntimeMarshalling]` (see [here](https://learn.microsoft.com/dotnet/standard/native-interop/disabled-marshalling))
- `[UnsafeAccessor]` (see [here](https://github.com/dotnet/runtime/issues/81741))
- `[InlineArray]` (see [here](https://learn.microsoft.com/dotnet/csharp/language-reference/proposals/csharp-12.0/inline-arrays))

# Options ⚙️

**PrivateLike**'s generation can be configured through some MSBuild properties to set in consuming projects.

The following properties are available:
- "PrivateLikeUsePublicAccessibilityForGeneratedTypes": makes all generated types public.
- "PrivateLikeIncludeRuntimeSupportedAttributes": enables polyfills for (dummy) runtime-supported attributes too.
- "PrivateLikeUseInteropServices2NamespaceForUnmanagedCallersOnlyAttribute": moves `[UnmanagedCallersOnly]`.
- "PrivateLikeExcludeGeneratedTypes": excludes specific types from generation (';' or ',' separated type names).
- "PrivateLikeIncludeGeneratedTypes": only includes specific types for generation (';' or ',' separated type names).
- "PrivateLikeExcludeTypeForwardedToDeclarations": never generates any `[TypeForwardedTo]` declarations.
