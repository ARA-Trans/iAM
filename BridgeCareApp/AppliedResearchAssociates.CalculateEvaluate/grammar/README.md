This project uses [ANTLR][antlr] to provide a .NET API that translates strings in a simple number/boolean-based expression language named "Calculate-Evaluate" to dynamically compiled .NET delegates.

To build this project, you don't need anything beyond the dependencies already specified within the project metadata. The build process should take care of acquiring those dependencies automatically.

However, in order to update the Calculate-Evaluate grammar and ANTLR-generated code, we recommend VS Code with a suitable ANTLR/Java environment. To set up such an environment, you can do the following:

1. Ensure you have a correct [VS Code with Java][vscode-java] environment by downloading and running the "Visual Studio Code Java Pack Installer".
2. Install the ["ANTLR4 grammar syntax support" extension for VS Code][vscode-antlr].

Then to update the grammar:

3. Within VS Code, edit this project's grammar files as needed.
   - To debug the grammar, please refer to the documentation of the aforementioned VS Code extension.
4. Run this project's "grammar2code.bat" batch file. (In Visual Studio, right-click the file and select "Execute File".)

After updating the grammar, depending on the nature of your update, you may need to then update the non-generated code in this project.

[antlr]: https://www.antlr.org/
[vscode-java]: https://code.visualstudio.com/docs/languages/java
[vscode-antlr]: https://marketplace.visualstudio.com/items?itemName=mike-lischke.vscode-antlr4
