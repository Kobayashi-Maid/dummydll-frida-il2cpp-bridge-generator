# dummydll-frida-il2cpp-bridge-generator

A utility tool to generate frida-il2cpp-bridge reference script from Il2CppDumper's output.

## Feature

Generates a TypeScript file with the frida-il2cpp-bridge reference script for the specified DummyDll's classes and methods.

```typescript
// Class
const TestClass = Il2Cpp.domain.assembly("Assembly").image.class("TestClass");

// Method
const TestMethod = TestClass.method("TestMethod");
```

Starting from version 0.1.1, the program can generate JSDoc comments for the created classes and methods.

```typescript
/**
 * TestClass
 */
const TestClass = Il2Cpp.domain.assembly("Assembly").image.class("TestClass");

/**
 * TestClass.Input
 * @param UnityEngine.Vector3&.pos
 * @param UnityEngine.Vector3.inputPosition
 * @returns System.Boolean 
 */
const TestClass$$Input = TestClass.method<boolean>("Input");
```
## TODOs

- [ ] Add support for multiple DummyDlls.
- [ ] Extend the annotation to support Il2CppDumper's attributes, like `Token` and `Address`. (maybe cannot be done)

## Usage

1. Compile the project or download the Release files to obtain the program.
2. Use [Il2CppDumper](https://github.com/Perfare/Il2CppDumper) to get the DummyDll.
3. Open the .exe file directly. A file selection window will pop up. Choose the DummyDll you want to import (a single file), and then wait for the TypeScript script to be generated.

## Command-line Usage

You can also use the program from the command line. Here's an example:

`dummydll-frida-il2cpp-bridge-generator.exe --assembly=/path/to/GameAssembly.dll --tsoutput=/path/to/output.ts --annotation`

This will generate a TypeScript file with the frida-il2cpp-bridge reference script for the specified assembly, and save it to the specified output path. The `--annotation` option will add comments (like `// [Method]` and `// [Class]`) to the generated script to explain what each line types.


## Command-line Options

```
Usage: dummydll-frida-il2cpp-bridge-generator [options]
Options:
--assembly=<file path>         [Required] Path to the dummy dll (Il2cppDumper) assembly file to load.
--tsoutput=<file path>         [Required] Path to the directory where the generated TypeScript files will be saved.
--annotation                   Generate TypeScript annotations for the generated classes.
--jsdoc                        Generate JSDoc comments for the generated classes.
--help                         Print this help message
```

## Send issues or feedback

If you have any issues or feedback, please create an issue on the GitHub repository.

If generated TypeScript files are not correct or incomplete, please create an issue on the GitHub repository.