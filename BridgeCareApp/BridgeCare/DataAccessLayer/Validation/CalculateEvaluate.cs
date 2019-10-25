using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BridgeCare.DataAccessLayer
{
    [Serializable]
    public class CalculateEvaluate
    {

        ArrayList _mathMembers = new ArrayList();
        Hashtable _mathMembersMap = new Hashtable();

        //DEBUGGING
        //public StringBuilder _source = new StringBuilder();
        StringBuilder _source = new StringBuilder();

        public String m_strResult = "";
        public bool m_bCalculate;
        public string m_expression;
        public List<String> m_listError = new List<String>();
        public object m_assemblyInstance = null;
        public MethodInfo m_methodInfo = null;
        public CompilerResults m_cr = null;
        //public byte[] array;		//Unacceptable.
        private byte[] _array;
        public List<String> m_listParameters = new List<String>();
        private string _dllName;
        private string _originalInput;
        bool _isTemporaryClass = false;

        public string OriginalInput
        {
            get { return _originalInput; }
            set { _originalInput = value; }
        }


        public List<String> Parameters
        {
            get { return m_listParameters; }
        }


        public MethodInfo methodInfo
        {
            get { return m_methodInfo; }
            set { m_methodInfo = value; }
        }

        public object assemblyInstance
        {
            get { return m_assemblyInstance; }
            set { m_assemblyInstance = value; }
        }

        public CompilerResults CompiledResult
        {
            get { return m_cr; }
            set { m_cr = value; }
        }

        public String Expression
        {
            get { return m_expression; }
            set
            {
                m_expression = value;
            }
        }

        public bool IsCalculated
        {
            get { return m_bCalculate; }
            set { m_bCalculate = value; }
        }

        public CalculateEvaluate()
        {
            GetMathMemberNames();  // track all members of the math namespace
        }



        /// <summary>
        /// Main driving routine for building a class
        /// </summary>
        public void BuildTemporaryClass(string expression, bool bCalculate)
        {
            _isTemporaryClass = true;
            BuildClass(expression, bCalculate, null);

        }


        /// <summary>
        /// Main driving routine for building a class
        /// </summary>
        public void BuildClass(string expression, bool bCalculate)
        {

            BuildClass(expression, bCalculate, null);

        }

        public void BuildClass(string expression, bool bCalculate, string dllName)
        {
            if (expression == "" || expression == null)
            {
                return;
            }
            _originalInput = expression.Replace("[$", "[").Replace("[@", "["); ;
            _dllName = dllName;
            m_bCalculate = bCalculate;
            m_expression = expression;
            expression = RefineEvaluationString(expression);
            Regex expressionWhiteSpaceMechanic = new Regex(@"\s");
            if (!bCalculate) //Calculating equations only use numbers.  No need for any manipulation.  The expression is already to be compiled.
            {
                //This can't happen because it ToUpper() the string literals
                //expression = expression.ToUpper();

                expression = expression.Replace("||", "OR");
                expression = expression.Replace("!=", "<>");
                expression = expression.Replace("==", "=");
                expression = expression.Replace("|", "'");
                expression = expression.Replace(">=", ">>");//This is so we can replace = with double ==
                expression = expression.Replace("<=", "<<");//This is so we can replace = with double ==

                //we keep missing non-space whitespace...
                expression = expressionWhiteSpaceMechanic.Replace(expression, " ");
                expression = expression.Replace(" AND ", " && ");
                expression = expression.Replace(" OR ", " || ");


                expression = expression.Replace("=", "==");
                expression = expression.Replace("<>", "!=");
                expression = expression.Replace("<<", "<=");
                expression = expression.Replace(">>", ">=");

                int nOpen = -1;
                bool bStringVariable = false;
                bool bDateVariable = false;

                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression.Substring(i, 1) == "[")
                    {
                        if (expression.Substring(i + 1, 1) == "@")
                        {
                            bStringVariable = true;
                            bDateVariable = false;
                        }
                        else if (expression.Substring(i + 1, 1) == "$")
                        {
                            bStringVariable = false;
                            bDateVariable = true;
                        }
                        else
                        {
                            bStringVariable = false;
                            bDateVariable = false;
                        }
                    }


                    if (expression.Substring(i, 1) == "'" && nOpen < 0)
                    {
                        nOpen = i;
                        continue;
                    }


                    if (expression.Substring(i, 1) == "'" && nOpen > -1)
                    {
                        //Get the value between open and (i)
                        //
                        String strValueWithQuotes = expression.Substring(nOpen, i - nOpen + 1);
                        String strValue = expression.Substring(nOpen + 1, i - nOpen - 1);

                        if (bStringVariable)
                        {
                            strValue = strValueWithQuotes.Replace("'", "\"");
                        }
                        else if (bDateVariable)
                        {
                            strValue = strValueWithQuotes.Replace("'", "\"");
                            strValue = " Convert.ToDateTime(" + strValue + ") ";
                        }
                        else
                        {
                            try
                            {
                                float fValue = float.Parse(strValue);
                            }
                            catch
                            {
                                strValue = strValueWithQuotes.Replace("'", "\"");

                            }
                        }
                        expression = expression.Remove(nOpen, i - nOpen + 1).Insert(nOpen, strValue);
                        //expression = expression.Replace(strValueWithQuotes, strValue);
                        nOpen = -1;
                        i = 0;
                    }
                }
            }


            // need a string to put the code into
            _source = new StringBuilder();
            StringWriter sw = new StringWriter(_source);

            //Declare your provider and generator
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeGenerator generator = codeProvider.CreateGenerator(sw);
            CodeGeneratorOptions codeOpts = new CodeGeneratorOptions();

            CodeNamespace myNamespace = new CodeNamespace("ExpressionEvaluator");
            myNamespace.Imports.Add(new CodeNamespaceImport("System"));
            myNamespace.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));

            //Build the class declaration and member variables			
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration();
            classDeclaration.IsClass = true;
            if (bCalculate)
                classDeclaration.Name = "Calculator";
            else
                classDeclaration.Name = "Evaluator";

            classDeclaration.Attributes = MemberAttributes.Public;

            if (bCalculate)
                classDeclaration.Members.Add(FieldVariable("answer", typeof(double), MemberAttributes.Private));
            else
                classDeclaration.Members.Add(FieldVariable("answer", typeof(bool), MemberAttributes.Private));



            //default constructor
            CodeConstructor defaultConstructor = new CodeConstructor();
            defaultConstructor.Attributes = MemberAttributes.Public;
            defaultConstructor.Comments.Add(new CodeCommentStatement("Default Constructor for class", true));
            defaultConstructor.Statements.Add(new CodeSnippetStatement("//TODO: implement default constructor"));
            classDeclaration.Members.Add(defaultConstructor);

            //property
            if (bCalculate)
                classDeclaration.Members.Add(this.MakeProperty("Answer", "answer", typeof(double)));
            else
                classDeclaration.Members.Add(this.MakeProperty("Answer", "answer", typeof(bool)));

            //Our Calculate Method
            CodeMemberMethod myMethod = new CodeMemberMethod();

            if (bCalculate)
            {
                myMethod.Name = "Calculate";
                myMethod.ReturnType = new CodeTypeReference(typeof(double));
                myMethod.Comments.Add(new CodeCommentStatement("Calculate an expression", true));
                myMethod.Attributes = MemberAttributes.Public;
                myMethod.Parameters.AddRange(GetParametersFromBracketedItems(expression));

                myMethod.Statements.Add(new CodeAssignStatement(new CodeSnippetExpression("Answer"), new CodeSnippetExpression(System.Text.RegularExpressions.Regex.Replace(expression, "[[\\]]", ""))));
                //            myMethod.Statements.Add(new CodeSnippetExpression("MessageBox.Show(String.Format(\"Answer = {0}\", Answer))"));
                myMethod.Statements.Add(
                    new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Answer")));
                classDeclaration.Members.Add(myMethod);


            }
            else
            {
                myMethod.Name = "Evaluate";
                myMethod.ReturnType = new CodeTypeReference(typeof(bool));
                myMethod.Comments.Add(new CodeCommentStatement("Evaluate an expression", true));
                myMethod.Attributes = MemberAttributes.Public;
                myMethod.Parameters.AddRange(GetParametersFromBracketedItems(expression));
                expression = expression.Replace("@", "").Replace("$", "");
                myMethod.Statements.Add(new CodeConditionStatement(new CodeVariableReferenceExpression(System.Text.RegularExpressions.Regex.Replace(expression, "[[\\]]", "")), new CodeStatement[] { new CodeSnippetStatement("this.Answer = true;") }, new CodeStatement[] { new CodeSnippetStatement("this.Answer = false;") }));

                myMethod.Statements.Add(
                        new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Answer")));

                classDeclaration.Members.Add(myMethod);
            }




            //write code
            myNamespace.Types.Add(classDeclaration);
            generator.GenerateCodeFromNamespace(myNamespace, sw, codeOpts);
            sw.Flush();
            sw.Close();
        }

        public CompilerResults CompileAssembly()
        {
            // create a compiler
            //ICodeCompiler compiler = CreateCompiler(); //Obsolete
            // get all the compiler parameters
            CompilerParameters parms = CreateCompilerParameters();

            // compile the code into an assembly
            //CompilerResults results = CompileCode(compiler, parms, _source.ToString());//Obsolete
            CompilerResults results = CompileCode(parms, _source.ToString());
            m_cr = results;
            return results;

        }



        /// <summary>
        /// Compiles the code from the code string
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="parms"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        //private CompilerResults CompileCode(ICodeCompiler compiler, CompilerParameters parms, string source)//Obsolete
        private CompilerResults CompileCode(CompilerParameters parms, string source)
        {
            // actually compile the code
            //Obsolete
            //CompilerResults results = compiler.CompileAssemblyFromSource(
            //                            parms, source);

            CodeDomProvider codeProvider = new CSharpCodeProvider();


            CompilerResults results = null;
            bool isNameCollision = true;
            while (isNameCollision)
            {
                isNameCollision = false;
                results = codeProvider.CompileAssemblyFromSource(parms, source);
                foreach (CompilerError error in results.Errors)
                {
                    if (error.ErrorNumber == "CS0016")//The process cannot access the file because it is being used by another process. 
                    {
                        isNameCollision = true;
                        parms.OutputAssembly = IncrementCompiledAssemblyNameOnCollision(parms.OutputAssembly);
                    }
                }
            }
            if (results.Errors.HasErrors)
            {
                string errorMessage = "";
                string separator = "";
                foreach (CompilerError error in results.Errors)
                {
                    errorMessage += separator + error.ErrorText;
                    separator = "  ";
                }
                throw new InvalidOperationException("Error compiling code: " + errorMessage);
            }
            else
            {
                //we can't open the file if there were compilation errors....
                if (!_isTemporaryClass)
                {
                    Stream stream = null;
                    if (_dllName != null)
                    {
                        stream = File.Open(results.PathToAssembly, FileMode.Open);
                    }
                    else
                    {
                        stream = File.Open(results.TempFiles.BasePath + ".dll", FileMode.Open);
                    }
                    _array = new byte[(int)stream.Length];
                    stream.Read(_array, 0, (int)stream.Length);
                    stream.Close();
                }
            }
            return results;
        }

        /// <summary>
        /// Create parameters for compiling
        /// </summary>
        /// <returns></returns>
        CompilerParameters CreateCompilerParameters()
        {
            //add compiler parameters and assembly references
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.CompilerOptions = "/target:library /optimize";
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = false;
            compilerParams.IncludeDebugInformation = false;
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            if (_dllName != null)
            {
                compilerParams.OutputAssembly = Path.GetTempPath() + _dllName + ".dll";
            }
            else if (_isTemporaryClass)
            {

                compilerParams.GenerateInMemory = true;
            }
            return compilerParams;
        }

        void GetMathMemberNames()
        {
            // get a reflected assembly of the System assembly
            Assembly systemAssembly = Assembly.GetAssembly(typeof(System.Math));
            try
            {
                //cant call the entry method if the assembly is null
                if (systemAssembly != null)
                {
                    //Use reflection to get a reference to the Math class
                    Module[] modules = systemAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the Math class
                    foreach (Type type in types)
                    {
                        if (type.Name == "Math")
                        {
                            // get all of the members of the math class and map them to the same member
                            // name in uppercase
                            MemberInfo[] mis = type.GetMembers();
                            foreach (MemberInfo mi in mis)
                            {
                                _mathMembers.Add(mi.Name);
                                _mathMembersMap[mi.Name.ToUpper()] = mi.Name;
                            }
                        }
                        //if the entry point method does return in Int32, then capture it and return it
                    }


                    //if it got here, then there was no entry point method defined.  Tell user about it
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:  An exception occurred while executing the script", ex);
            }
        }

        /// <summary>
        /// Need to change eval string to use .NET Math library
        /// </summary>
        /// <param name="eval">evaluation expression</param>
        /// <returns></returns>
        string RefineEvaluationString(string eval)
        {
            // look for regular expressions with only letters
            //Regex regularExpression = new Regex("[a-zA-Z_]+");
            Regex regularExpression = new Regex(@"(?<!(@|\||\[)[a-zA-Z_]*)[a-zA-Z_]+");
            //string[] originalValues = eval.Split('\'');
            eval = eval.Replace("'", "Z_B_X");




            // track all functions and constants in the evaluation expression we already replaced
            ArrayList replacelist = new ArrayList();

            // find all alpha words inside the evaluation function that are possible functions
            MatchCollection matches = regularExpression.Matches(eval);
            foreach (Match m in matches)
            {
                // if the word is found in the math member map, add a Math prefix to it
                bool isContainedInMathLibrary = _mathMembersMap[m.Value.ToUpper()] != null;
                if (replacelist.Contains(m.Value) == false && isContainedInMathLibrary)
                {
                    eval = eval.Replace(m.Value, "Math." + _mathMembersMap[m.Value.ToUpper()]);
                }

                // we matched it already, so don't allow us to replace it again
                replacelist.Add(m.Value);
            }

            eval = eval.Replace("Z_B_X", "'");
            // return the modified evaluation string
            return eval;
        }

        /// <summary>
        /// Very simplistic getter/setter properties
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="internalName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        CodeMemberProperty MakeProperty(string propertyName, string internalName, Type type)
        {
            CodeMemberProperty myProperty = new CodeMemberProperty();
            myProperty.Name = propertyName;
            myProperty.Comments.Add(new CodeCommentStatement(String.Format("The {0} property is the returned result", propertyName)));
            myProperty.Attributes = MemberAttributes.Public;
            myProperty.Type = new CodeTypeReference(type);
            myProperty.HasGet = true;
            myProperty.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), internalName)));

            myProperty.HasSet = true;
            myProperty.SetStatements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), internalName),
                    new CodePropertySetValueReferenceExpression()));

            return myProperty;
        }

        CodeMemberField FieldVariable(string fieldName, string typeName, MemberAttributes accessLevel)
        {
            CodeMemberField field = new CodeMemberField(typeName, fieldName);
            field.Attributes = accessLevel;
            return field;
        }
        CodeMemberField FieldVariable(string fieldName, Type type, MemberAttributes accessLevel)
        {
            CodeMemberField field = new CodeMemberField(type, fieldName);
            field.Attributes = accessLevel;
            return field;
        }

        private CodeParameterDeclarationExpressionCollection GetParametersFromBracketedItems(string expression)
        {
            CodeParameterDeclarationExpressionCollection toReturn = new CodeParameterDeclarationExpressionCollection();
            System.Collections.Generic.List<string> uniqueParameters = new System.Collections.Generic.List<string>();
            string sVariableName = "";

            foreach (System.Text.RegularExpressions.Match rem in System.Text.RegularExpressions.Regex.Matches(expression, "\\[[^[\\]]+\\]"))
            {
                if (!uniqueParameters.Contains(rem.Value))
                {
                    sVariableName = System.Text.RegularExpressions.Regex.Replace(rem.Value,
                                                                                    "(^\\[*|\\]*$)",
                                                                                    "");

                    bool bIsString = false;
                    bool bIsDateTime = false;
                    // String variables will have an @ sign at their beginning.
                    if (sVariableName.Substring(0, 1) == "@")
                    {
                        bIsString = true;
                        sVariableName = sVariableName.Substring(1);
                    }

                    // String variables will have an @ sign at their beginning.
                    if (sVariableName.Substring(0, 1) == "$")
                    {
                        bIsDateTime = true;
                        sVariableName = sVariableName.Substring(1);
                    }


                    if (!m_listParameters.Contains(sVariableName))
                    {
                        m_listParameters.Add(sVariableName);
                    }


                    uniqueParameters.Add(rem.Value);
                    if (bIsString)
                    {
                        toReturn.Add(new CodeParameterDeclarationExpression(typeof(string), sVariableName));
                    }
                    else if (bIsDateTime)
                    {
                        toReturn.Add(new CodeParameterDeclarationExpression(typeof(DateTime?), sVariableName));
                    }
                    else
                    {
                        if (m_bCalculate)
                        {
                            toReturn.Add(new CodeParameterDeclarationExpression(typeof(double), sVariableName)); // TODO: May want to change this to be the type of whatever is in brackets, but that's a DB lookup, not something that can be handled naïvely
                        }
                        else
                        {
                            toReturn.Add(new CodeParameterDeclarationExpression(typeof(double?), sVariableName));
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Runs the Calculate method in our on-the-fly assembly
        /// </summary>
        /// <param name="results"></param>
        public void RunCode(CompilerResults results, object[] obj)
        {
            string dllName = results.PathToAssembly;
            if (!_isTemporaryClass && !File.Exists(dllName))
            {
                FileStream fs = new FileStream(dllName, FileMode.Create);
                fs.Write(_array, 0, _array.Length);
                fs.Close();
            }

            Assembly executingAssembly = results.CompiledAssembly;
            try
            {
                //cant call the entry method if the assembly is null
                if (executingAssembly != null)
                {
                    object assemblyInstance;
                    if (m_bCalculate)
                    {
                        assemblyInstance = executingAssembly.CreateInstance("ExpressionEvaluator.Calculator");
                    }
                    else
                    {
                        assemblyInstance = executingAssembly.CreateInstance("ExpressionEvaluator.Evaluator");
                    }
                    //Use reflection to call the static Main function

                    Module[] modules = executingAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the entry point method
                    foreach (Type type in types)
                    {
                        MethodInfo[] mis = type.GetMethods();
                        foreach (MethodInfo mi in mis)
                        {
                            if (m_bCalculate)
                            {
                                if (mi.Name == "Calculate")
                                {
                                    m_methodInfo = mi;
                                    m_assemblyInstance = assemblyInstance;
                                    object result = mi.Invoke(assemblyInstance, obj);
                                    m_strResult = result.ToString();
                                }
                            }
                            else
                            {
                                if (mi.Name == "Evaluate")
                                {
                                    m_methodInfo = mi;
                                    m_assemblyInstance = assemblyInstance;

                                    object result = mi.Invoke(assemblyInstance, obj);
                                    m_strResult = result.ToString();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:  An exception occurred while executing the script", ex);
            }
        }

        public object RunMethod(object[] obj)
        {
            //If you don't CompileAssembly first, array isn't guaranteed to have been written to.
            //This will probably screw up the performance advantage of having precompiled assemblies, but that check should be occuring
            //in compileassembly() anyway
            //if(m_methodInfo == null) RunCode(m_cr,obj);
            if (m_methodInfo == null)
            {
                //The performance hit is too big.
                //if (_array == null || !File.Exists(m_cr.PathToAssembly))	
                if (!_isTemporaryClass && !File.Exists(m_cr.PathToAssembly))
                {
                    CompileAssembly();
                }
                RunCode(m_cr, obj);
            }
            try
            {
                return m_methodInfo.Invoke(m_assemblyInstance, obj);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(exception.Message);
            }
        }


        /// <summary>
        /// Main driving routine for building a class
        /// </summary>
        public void BuildFunctionClass(string expression, string strReturnType, string dllName)
        {
            //if( expression == "29487*[AREA]" )
            //{
            //}
            _dllName = dllName;
            bool bCalculate = true;
            m_bCalculate = bCalculate;
            m_expression = expression;
            // expression = RefineEvaluationString(expression);
            //  expression = expression.ToUpper();


            // need a string to put the code into
            _source = new StringBuilder();
            StringWriter sw = new StringWriter(_source);

            //Declare your provider and generator
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeGenerator generator = codeProvider.CreateGenerator(sw);
            CodeGeneratorOptions codeOpts = new CodeGeneratorOptions();

            CodeNamespace myNamespace = new CodeNamespace("ExpressionEvaluator");
            myNamespace.Imports.Add(new CodeNamespaceImport("System"));
            myNamespace.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));

            //Build the class declaration and member variables			
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration();
            classDeclaration.IsClass = true;
            classDeclaration.Name = "Calculator";
            classDeclaration.Attributes = MemberAttributes.Public;

            switch (strReturnType)
            {
                case "String":
                    classDeclaration.Members.Add(FieldVariable("answer", typeof(String), MemberAttributes.Private));
                    break;
                case "double":
                    classDeclaration.Members.Add(FieldVariable("answer", typeof(double), MemberAttributes.Private));
                    break;
                case "Boolean":
                    classDeclaration.Members.Add(FieldVariable("answer", typeof(Boolean), MemberAttributes.Private));
                    break;
                default:
                    return;
                    //break;
            }
            //default constructor
            CodeConstructor defaultConstructor = new CodeConstructor();
            defaultConstructor.Attributes = MemberAttributes.Public;
            defaultConstructor.Comments.Add(new CodeCommentStatement("Default Constructor for class", true));
            //   defaultConstructor.Statements.Add(new CodeSnippetStatement("//TODO: implement default constructor"));
            classDeclaration.Members.Add(defaultConstructor);

            switch (strReturnType)
            {
                case "String":
                    classDeclaration.Members.Add(this.MakeProperty("Answer", "answer", typeof(String)));
                    break;
                case "double":
                    classDeclaration.Members.Add(this.MakeProperty("Answer", "answer", typeof(double)));
                    break;
                case "Boolean":
                    classDeclaration.Members.Add(this.MakeProperty("Answer", "answer", typeof(Boolean)));
                    break;
                default:
                    return;
                    //break;
            }




            //Our Calculate Method
            CodeMemberMethod myMethod = new CodeMemberMethod();

            myMethod.Name = "Calculate";
            switch (strReturnType)
            {
                case "String":
                    myMethod.ReturnType = new CodeTypeReference(typeof(String));
                    break;
                case "double":
                    myMethod.ReturnType = new CodeTypeReference(typeof(double));
                    break;
                case "Boolean":
                    myMethod.ReturnType = new CodeTypeReference(typeof(Boolean));
                    break;
                default:
                    return;
                    //break;
            }

            myMethod.Comments.Add(new CodeCommentStatement("Calculate an expression", true));
            myMethod.Attributes = MemberAttributes.Public;
            myMethod.Parameters.AddRange(GetParametersFromBracketedItems(expression));
            int nLastSemiColon = expression.LastIndexOf(";");
            if (nLastSemiColon > 0)
            {
                expression = expression.Substring(0, nLastSemiColon);
            }
            expression = expression.Replace("[", "");
            expression = expression.Replace("@", "");
            expression = expression.Replace("]", "");
            myMethod.Statements.Add(new CodeSnippetExpression(expression));
            classDeclaration.Members.Add(myMethod);

            //write code
            myNamespace.Types.Add(classDeclaration);
            generator.GenerateCodeFromNamespace(myNamespace, sw, codeOpts);
            sw.Flush();
            sw.Close();
        }


        /// <summary>
        /// Implements the assembly name if collision with locked dll.
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        private string IncrementCompiledAssemblyNameOnCollision(string dllPath)
        {
            if (!string.IsNullOrWhiteSpace(dllPath))
            {
                //Remove the .dll
                dllPath = dllPath.Replace(".dll", "");
                // If last character is not ), just add (2)
                if (dllPath.Substring(dllPath.Length - 1) != ")")
                {
                    dllPath += "(2)";
                }
                else
                {
                    int index = dllPath.LastIndexOf("(");//Check if last character a ), if so look for a (.  Increment number in between.
                    string duplicate = dllPath.Substring(index + 1, dllPath.Length - index - 2);
                    int increment;
                    try
                    {
                        increment = int.Parse(duplicate);
                        increment++;
                        dllPath = dllPath.Substring(0, index + 1) + increment.ToString() + ")";
                    }
                    catch                //If there is ( and a ) at the end, but the number does not parse add a (2)
                    {
                        dllPath += "(2)";
                    }
                }
            }
            return dllPath + ".dll";
        }
    }
}
