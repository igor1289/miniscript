using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniScript
{
    public delegate void Command(Value[] parameters, Runtime runtime);

    protected class Instruction
    {
        public Command command;
        public Value[] parameters;

        public void Execute(Runtime runtime)
        {
            if (command != null)
            {
                command(parameters, runtime);
            }
            else if (parameters.Length == 2)
            {
                Variable variable = parameters[0].GetVariable(null);

                if (variable != null)
                {
                    Value value = runtime.GetVariableValue(variable);
                    value.SetAs(parameters[1].type, parameters[1].value);
                }
            }
        }

        public Instruction(Command command, Value[] parameters)
        {
            this.command = command;
            this.parameters = parameters;
        }
    }

    public class Runtime
    {
        public MiniScript script { get; private set; }
        private Value[] variables;

        //Playback parameters
        public int instructionIndex { get; private set; }
        public float waitTime { get; private set; }

        private bool playing, repeat, paused, stepByStep, executing;

        public void Update(float timeScale = 1)
        {
            if (!playing)
                return;

            if (paused)
            {
                if (waitTime > 0)
                {
                    waitTime -= Time.deltaTime * timeScale;

                    if (waitTime <= 0)
                    {
                        paused = false;
                    }
                }
            }

            if (paused)
                return;

            bool completed = false;

            while(true)
            {
                if (instructionIndex == script.instructions.Length)
                {
                    completed = true;
                    break;
                }

                script.instructions[instructionIndex].Execute(this);

                if (playing == false)
                    break;

                if (stepByStep || paused)
                {
                    instructionIndex++;
                    break;
                }

                instructionIndex++;
            }

            if (completed)
            {
                playing = repeat;

                if (playing)
                    instructionIndex = 0;
            }
        }

        public Value GetVariableValue(Variable variable)
        {
            return variables[variable.variableIndex];
        }

        public void Play(bool repeat = false, bool stepByStep = false)
        {
            playing = true;
            paused = false;
            this.repeat = repeat;
            this.stepByStep = stepByStep;
        }

        public void PlayFromLabel(Label label, bool repeat = false, bool stepByStep = false)
        {
            instructionIndex = label.instructionIndex;
            playing = true;
            paused = false;
            this.repeat = repeat;
            this.stepByStep = stepByStep;
        }

        public void Pause(float waitTime = 0f)
        {
            this.waitTime = waitTime;
            paused = true;
        }

        public void ResetPlayback()
        {
            playing = false;
            paused = false;
            instructionIndex = 0;
        }

        public void ResetRuntime()
        {
            for (int i = 0; i < variables.Length; i++)
                variables[i] = new Value(null, Value.VALUE_TYPE.NULL, true);
        }

        public Runtime(MiniScript script)
        {
            this.script = script;

            variables = new Value[script.variables.Count];

            ResetRuntime();
        }
    }

    public class Variable
    {
        public readonly string name;
        public readonly int variableIndex;

        public Variable(string name, int variableIndex)
        {
            this.name = name;
            this.variableIndex = variableIndex;
        }
    }

    public class Label
    {
        public readonly string name;
        public readonly int instructionIndex;

        public Label(string name, int instructionIndex)
        {
            this.name = name;
            this.instructionIndex = instructionIndex;
        }

    }

    public class Value
    {
        public enum VALUE_TYPE
        {
            NULL,
            BOOL,
            INT,
            FLOAT,
            STRING,
            STRING_TEMPLATE,
            OBJECT,
            VARIABLE,
            LABEL
        }

        public VALUE_TYPE type { get; private set; }

        public object value { get; private set; }

        private bool variable;

        public void Null()
        {
            if (variable)
            {
                type = VALUE_TYPE.NULL;
                value = null;
            }
        }

        public void Set(bool newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.BOOL;
                value = newValue;
            }
        }


        public void Set(int newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.INT;
                value = newValue;
            }
        }

        public void Set(float newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.FLOAT;
                value = newValue;
            }
        }

        public void Set(string newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.STRING;
                value = newValue;
            }
        }

        public void Set(StringTemplate newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.STRING_TEMPLATE;
                value = newValue;
            }
        }

        public void Set(object newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.OBJECT;
                value = newValue;
            }
        }
        
        public void Set(Variable newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.VARIABLE;
                value = newValue;
            }
        }

        public void Set(Label newValue)
        {
            if (variable)
            {
                type = VALUE_TYPE.LABEL;
                value = newValue;
            }
        }
        

        public void SetAs(VALUE_TYPE type, object newValue)
        {
            if (variable)
            {
                this.type = type;
                value = newValue;
            }
        }

        public bool GetBool(Runtime runtime)
        {
            if (type == VALUE_TYPE.NULL)
                return false;

            else if (type == VALUE_TYPE.BOOL)
                return (bool)value;

            else if (type == VALUE_TYPE.INT)
                return (int)value != 0;

            else if (type == VALUE_TYPE.FLOAT )
                return (float)value != 0f;

            else if (type == VALUE_TYPE.STRING)
                return (string)value != "";

            else if (type == VALUE_TYPE.STRING_TEMPLATE )
                return ((StringTemplate)value).GetString(runtime) != "";

            else
                return value != null;
        }

        public int GetInt(int defaultValue = 0)
        {
            if (type == VALUE_TYPE.INT)
                return (int)value;

            else if (type == VALUE_TYPE.FLOAT)
                return (int)((float)value);

            else
                return defaultValue;
        }

        public float GetFloat(float defaultValue = 0f)
        {
            if (type == VALUE_TYPE.FLOAT)
                return (float)value;

            else if (type == VALUE_TYPE.INT)
                return (int)value;

            else
                return defaultValue;
        }

        public string GetString(string defaultValue = "")
        {
            if (type == VALUE_TYPE.STRING)
                return (string)value;

            else if (type == VALUE_TYPE.BOOL)
                return (bool)value == true ? "True" : "False";

            else if (type == VALUE_TYPE.INT)
                return ((int)value).ToString();

            else if (type == VALUE_TYPE.FLOAT)
                return ((float)value).ToString();

            else
                return defaultValue;
        }

        public StringTemplate GetStringTemplate(StringTemplate defaultValue = null)
        {
            if (type == VALUE_TYPE.STRING_TEMPLATE)
                return (StringTemplate)value;
            else
                return defaultValue;
        }

        public object GetObject(object defaultValue = null)
        {
            if (type == VALUE_TYPE.OBJECT)
                return value;
            else
                return defaultValue;
        }

        public Variable GetVariable(Variable defaultValue = null)
        {
            if (type == VALUE_TYPE.VARIABLE)
                return (Variable)value;
            else
                return defaultValue;
        }

        public Value GetVariableValue(Runtime runtime)
        {
            if (type == VALUE_TYPE.VARIABLE)
                return runtime.GetVariableValue((Variable)value);
            else
                return null;
        }

        public Label GetLabel(Label defaultValue = null)
        {
            if (type == VALUE_TYPE.LABEL)
                return (Label)value;
            else
                return defaultValue;
        }



        public Value(object value, VALUE_TYPE type, bool variable = false)
        {
            this.value = value;
            this.type = type;
            this.variable = variable;
        }
    }

    public class StringTemplate
    {
        public readonly string template;

        private Variable[] variables;

        public string GetString(Runtime runtime)
        {
            string result = template;

            foreach (Variable variable in variables)
            {
                Value variableValue = runtime.GetVariableValue(variable);

                string variableString = "";

                if (variableValue.type == Value.VALUE_TYPE.BOOL || variableValue.type == Value.VALUE_TYPE.INT || variableValue.type == Value.VALUE_TYPE.FLOAT || variableValue.type == Value.VALUE_TYPE.STRING)
                    variableString = variableValue.value.ToString();

                result = result.Replace("$" + variable.name, variableString);
            }

            return result;
        }

        public StringTemplate(string template, Variable[] variables)
        {
            this.template = template;
            this.variables = variables;
        }

    }

    public class Compiler
    {
        private Dictionary<string, Command> commands;
        private Dictionary<string, Value> constants;

        public bool error { get; private set; }
        public string errorMessage { get; private set; }
        public Line errorLine { get; private set; }
        public int errorOffset { get; private set; }

        public void AddCommand(string name, Command command)
        {
            commands.Add(name, command);
        }

        public void AddConstant(string name, Value constant)
        {
            constants.Add(name, constant);
        }

        public class Line
        {
            public int number;
            public string text;

            public Line(int number, string text)
            {
                this.number = number;
                this.text = text;
            }
        }

        private class Token
        {
            public enum TOKEN_TYPE
            {
                IDENTIFIER,
                STRING,
                STRING_TEMPLATE,
                INTEGER,
                FLOAT,
                VARIABLE,
                LABEL,
                WHITESPACE,
                END_OF_LINE
            }

            public TOKEN_TYPE type;
            public string text;
            public int length;
            public int offset;

            public Value value;

            public Line line;

            public Token(TOKEN_TYPE type, string text, int length, int offset, Line line)
            {
                this.type = type;
                this.text = text;
                this.length = length;
                this.offset = offset;
                this.line = line;
            }

            public static Token ParseIdentifier(Line line, int charIndex)
            {
                if (char.IsLetter(line.text[charIndex]) || line.text[charIndex] == '_')
                {
                    for (int offset = charIndex + 1; offset < line.text.Length; offset++)
                    {
                        if (!char.IsLetterOrDigit(line.text[offset]) && line.text[offset] != '_')
                            return new Token(TOKEN_TYPE.IDENTIFIER, line.text.Substring(charIndex, offset - charIndex), offset - charIndex, charIndex, line);
                    }

                    return new Token(TOKEN_TYPE.IDENTIFIER, line.text.Substring(charIndex, line.text.Length - charIndex), line.text.Length - charIndex, charIndex, line);
                }

                return null;
            }

            public static Token ParseString(Compiler compiler, Line line, int charIndex)
            {
                if (line.text[charIndex] == '\u0027')
                {
                    for (int offset = charIndex + 1; offset < line.text.Length; offset++)
                    {
                        if (line.text[offset] == '\u0027')
                            return new Token(TOKEN_TYPE.STRING, line.text.Substring(charIndex + 1, offset - charIndex - 1), offset - charIndex + 1, charIndex, line);
                    }

                    compiler.Error("Unexpected end of line", line, charIndex);
                }

                return null;
            }

            public static Token ParseStringTemplate(Compiler compiler, Line line, int charIndex)
            {
                if (line.text[charIndex] == '\u0022')
                {
                    for (int offset = charIndex + 1; offset < line.text.Length; offset++)
                    {
                        if (line.text[offset] == '\u0022')
                            return new Token(TOKEN_TYPE.STRING_TEMPLATE, line.text.Substring(charIndex + 1, offset - charIndex - 1), offset - charIndex + 1, charIndex, line);
                    }

                    compiler.Error("Unexpected end of line", line, charIndex);
                }

                return null;
            }

            public static Token ParseNumber(Compiler compiler, Line line, int charIndex)
            {
                bool isNumber = false, isFloat = false, isNegative = false;

                int offset;

                for (offset = charIndex; offset < line.text.Length; offset++)
                {
                    if (char.IsDigit(line.text[offset]))
                    {
                        isNumber = true;
                    }
                    else if (line.text[offset] == '-')
                    {
                        if (offset == charIndex)
                        {
                            isNegative = true;
                        }
                        else
                        {
                            compiler.Error("Unexpected '-'", line, charIndex);
                            return null;
                        }
                    }
                    else if (line.text[offset] == '.')
                    {
                        if (!isFloat)
                        {
                            isFloat = true;
                        }
                        else
                        {
                            compiler.Error("Unexpected '.'", line, charIndex);
                            return null;
                        }
                    }
                    else if(char.IsWhiteSpace(line.text[offset]))
                    {
                        break;
                    }
                    else if(isNumber || isFloat || isNegative)
                    {
                        compiler.Error("Unexpected '" + line.text[offset] + "'", line, charIndex);
                        return null;
                    }
                }
                
                if(isNumber)
                {
                    return new Token(isFloat ? TOKEN_TYPE.FLOAT : TOKEN_TYPE.INTEGER, line.text.Substring(charIndex, offset - charIndex), offset - charIndex, charIndex, line);
                }
                else if(isFloat || isNegative)
                {
                    compiler.Error("Expecting number", line, charIndex);
                    return null;
                }
                else
                {
                    return null;
                }
            }

            public static Token ParseVariable(Compiler compiler, Line line, int charIndex)
            {
                if (line.text[charIndex] == '$')
                {
                    Token token = null;

                    for (int offset = charIndex + 1; offset < line.text.Length; offset++)
                    {
                        if (!char.IsLetterOrDigit(line.text[offset]))
                        {
                            token = new Token(TOKEN_TYPE.VARIABLE, line.text.Substring(charIndex, offset - charIndex), offset - charIndex, charIndex, line);
                            break;
                        }
                    }

                    if (token == null)
                        token = new Token(TOKEN_TYPE.VARIABLE, line.text.Substring(charIndex, line.text.Length - charIndex), line.text.Length - charIndex, charIndex, line);

                    if (token.text == "$")
                    {
                        compiler.Error("Expecting variable identifier", line, charIndex);
                        return null;
                    }

                    token.text = token.text.Substring(1);

                    return token;
                }

                return null;
            }

            public static Token ParseTemplateVariable(string template, int charIndex)
            {
                if (template[charIndex] == '$')
                {
                    Token token = null;

                    for (int offset = charIndex + 1; offset < template.Length; offset++)
                    {
                        if (!char.IsLetterOrDigit(template[offset]))
                        {
                            token = new Token(TOKEN_TYPE.VARIABLE, template.Substring(charIndex, offset - charIndex), offset - charIndex, charIndex, null);
                            break;
                        }
                    }

                    if (token == null)
                        token = new Token(TOKEN_TYPE.VARIABLE, template.Substring(charIndex, template.Length - charIndex), template.Length - charIndex, charIndex, null);

                    if (token.text == "$")
                    {
                        return null;
                    }

                    token.text = token.text.Substring(1);

                    return token;
                }

                return null;
            }

            public static Token ParseLabel(Compiler compiler, Line line, int charIndex)
            {
                if (line.text[charIndex] == '@')
                {
                    Token token = null;

                    for (int offset = charIndex + 1; offset < line.text.Length; offset++)
                    {
                        if (!char.IsLetterOrDigit(line.text[offset]))
                        {
                            token = new Token(TOKEN_TYPE.LABEL, line.text.Substring(charIndex, offset - charIndex), offset - charIndex, charIndex, line);
                            break;
                        }
                    }

                    if (token == null)
                        token = new Token(TOKEN_TYPE.LABEL, line.text.Substring(charIndex, line.text.Length - charIndex), line.text.Length - charIndex, charIndex, line);

                    if (token.text == "@")
                    {
                        compiler.Error("Expecting label identifier", line, charIndex);
                        return null;
                    }

                    token.text = token.text.Substring(1);

                    return token;
                }

                return null;
            }

            public static Token ParseWhiteSpace(Line line, int charIndex)
            {
                if (char.IsWhiteSpace(line.text[charIndex]))
                {
                    for (int offset = charIndex + 1; offset < line.text.Length; offset++)
                    {
                        if (!char.IsWhiteSpace(line.text[offset]))
                            return new Token(TOKEN_TYPE.WHITESPACE, line.text.Substring(charIndex + 1, offset - charIndex - 1), offset - charIndex, charIndex, line);
                    }
                }

                return null;
            }

            public static Token ParseToken(Compiler compiler, Line line, int charIndex)
            {
                Token token;

                bool success = (token = ParseIdentifier(line, charIndex)) != null
                        || (token = ParseString(compiler, line, charIndex)) != null
                        || (token = ParseStringTemplate(compiler, line, charIndex)) != null
                        || (token = ParseNumber(compiler, line, charIndex)) != null
                        || (token = ParseVariable(compiler, line, charIndex)) != null
                        || (token = ParseLabel(compiler, line, charIndex)) != null
                        || (token = ParseWhiteSpace(line, charIndex)) != null;

                if (success)
                    return token;
                else
                    return null;
            }

            public bool PrepareValue(Compiler compiler, List<Variable> variablesList, List<Label> labelsList, bool buildStage = false)
            {
                if (type == TOKEN_TYPE.INTEGER)
                    value = new Value(int.Parse(text), Value.VALUE_TYPE.INT);
                else if (type == TOKEN_TYPE.FLOAT)
                    value = new Value(float.Parse(text, System.Globalization.CultureInfo.InvariantCulture), Value.VALUE_TYPE.FLOAT);
                else if (type == TOKEN_TYPE.STRING)
                    value = new Value(text, Value.VALUE_TYPE.STRING);
                else if (type == TOKEN_TYPE.STRING_TEMPLATE)
                {
                    int charIndex = 0;

                    List<Variable> templateVariables = new List<Variable>();

                    while (charIndex < text.Length)
                    {
                        Token variableName = ParseTemplateVariable(text, charIndex);

                        //If variable token found
                        if (variableName != null)
                        {
                            //Search existing variable
                            Variable templateVariable = null;

                            foreach (Variable variable in variablesList)
                            {
                                if (variable.name == variableName.text)
                                {
                                    templateVariable = variable;
                                    break;
                                }
                            }

                            //if variable does not exists then create new and add it to variables lists
                            if (templateVariable == null)
                            {
                                templateVariable = new Variable(variableName.text, variablesList.Count);

                                variablesList.Add(templateVariable);
                                templateVariables.Add(templateVariable);
                            }
                            else
                            {
                                //Search existing variable in template variables
                                bool variableExisits = false;

                                foreach (Variable variable in templateVariables)
                                {
                                    if (variable == templateVariable)
                                    {
                                        variableExisits = true;
                                        break;
                                    }
                                }

                                if (!variableExisits)
                                    templateVariables.Add(templateVariable);
                            }

                            charIndex += variableName.length;
                        }

                        charIndex++;
                    }
                    //Label will be defined on label linking stage

                    value = new Value(new StringTemplate(text, templateVariables.ToArray()), Value.VALUE_TYPE.STRING_TEMPLATE);
                }
                else if(type == TOKEN_TYPE.IDENTIFIER)
                {
                    Value constantValue;
                    
                    if(compiler.constants.TryGetValue(text, out constantValue))
                    {
                        value = constantValue;
                    }
                    else
                    {
                        compiler.Error("Unknown constant " + text, line, offset);
                        return false;
                    }
                }
                else if (type == TOKEN_TYPE.VARIABLE)
                {
                    foreach (Variable variable in variablesList)
                    {
                        if (variable.name == text)
                        {
                            value = new Value(variable, Value.VALUE_TYPE.VARIABLE);
                        }
                    }

                    if(value == null)
                    {
                        Variable variable = new Variable(text, variablesList.Count);
                        variablesList.Add(variable);
                        value = new Value(variable, Value.VALUE_TYPE.VARIABLE);
                    }
                }
                else if (type == TOKEN_TYPE.LABEL)
                {
                    foreach(Label label in labelsList)
                    {
                        if(label.name == text)
                        {
                            value = new Value(label, Value.VALUE_TYPE.LABEL);
                        }
                        else if(buildStage)
                        {
                            compiler.Error("Unknown label " + text, line, offset);
                            return false;
                        }
                    }
                }

                return true;
            }

            public string Presentation()
            {
                if (type == TOKEN_TYPE.END_OF_LINE)
                    return "end of line";
                else if (type == TOKEN_TYPE.IDENTIFIER)
                    return "identifier '" + text + "'";
                else if (type == TOKEN_TYPE.INTEGER)
                    return "integer '" + text + "'";
                else if (type == TOKEN_TYPE.FLOAT)
                    return "float '" + text + "'";
                else if (type == TOKEN_TYPE.STRING)
                    return "string '" + text + "'";
                else if (type == TOKEN_TYPE.STRING_TEMPLATE)
                    return "string template '" + text + "'";
                else if (type == TOKEN_TYPE.LABEL)
                    return "label '" + text + "'";
                else if (type == TOKEN_TYPE.VARIABLE)
                    return "variable '" + text + "'";

                return "";

            }
        }


        public MiniScript Compile(string[] scriptText)
        {
            error = false;
            errorMessage = "";
            errorLine = null;
            errorOffset = -1;

            //------------------------------#1 Prepare script text------------------------------
            List<Line> linesList = new List<Line>();

            int lineNumber = 1;

            foreach (string scriptLine in scriptText)
            {
                linesList.Add(new Line(lineNumber, scriptLine.Trim()));
                lineNumber++;
            }

            //------------------------------#2 Parse lines into tokens------------------------------
            Line[] lines = linesList.ToArray();

            //Prepare tokens list
            List<Token> tokensList = new List<Token>();

            //Current line index
            int lineIndex = 0;

            //Pass multiline comment
            bool multilineComment = false;

            while (lineIndex < lines.Length)
            {
                Line line = lines[lineIndex];

                int charIndex = 0;

                while (charIndex < line.text.Length)
                {
                    if (multilineComment)
                    {
                        if ((line.text.Length - charIndex) >= 2 && line.text.Substring(charIndex, 2) == "*/")
                        {
                            multilineComment = false;
                            charIndex += 2;
                        }
                        else
                        {
                            charIndex++;
                        }
                    }
                    else if ((line.text.Length - charIndex) >= 2 && line.text.Substring(charIndex, 2) == "/*")
                    {
                        multilineComment = true;
                        charIndex += 2;
                    }
                    else if ((line.text.Length - charIndex) >= 2 && line.text.Substring(charIndex, 2) == "//")
                    {
                        break;
                    }
                    else
                    {
                        Token token = Token.ParseToken(this, line, charIndex);

                        if(token != null)
                        {
                            tokensList.Add(token);
                            charIndex += token.length;
                        }
                        else if (!error)
                        {
                            Error("Unexcpected character '" + line.text[charIndex] + "'", line, charIndex);
                            return null;
                        }
                        else
                        {
                            //Error is happened before
                            return null;
                        }
                    }
                }

                tokensList.Add(new Token(Token.TOKEN_TYPE.END_OF_LINE, "\n", 0, charIndex, line));

                lineIndex++;
            }

            //------------------------------#3 Check syntax and prepare values------------------------------
            //Initialize lists
            List<Variable> variablesList = new List<Variable>();
            List<Label> labelsList = new List<Label>();

            Token[] tokens = tokensList.ToArray();
            List<Token []> preparedInstructions = new List<Token[]>();

            int tokenIndex = 0, instructionStartIndex = -1;

            while (tokenIndex < tokens.Length)
            {
                Token token = tokens[tokenIndex];

                if (token.type != Token.TOKEN_TYPE.END_OF_LINE)
                {
                    if (instructionStartIndex == -1)
                        instructionStartIndex = tokenIndex;
                }
                else
                {
                    List<Token> instructionTokensList = new List<Token>();

                    int instructionTokenIndex = instructionStartIndex;

                    bool expectingWhitespace = false;

                    while(instructionTokenIndex < tokenIndex)
                    {
                        Token instructionToken = tokens[instructionTokenIndex];

                        if(expectingWhitespace)
                        {
                            if(instructionToken.type != Token.TOKEN_TYPE.WHITESPACE)
                            {
                                Error("Unexpected " + instructionToken.Presentation(), instructionToken.line, instructionToken.offset);
                                return null;
                            }
                            else
                            {
                                expectingWhitespace = false;
                            }
                        }
                        else
                        {
                            if (instructionToken.type != Token.TOKEN_TYPE.WHITESPACE)
                            {
                                instructionTokensList.Add(instructionToken);
                                expectingWhitespace = true;
                            }
                        }
                        
                        instructionTokenIndex++;
                    }

                    instructionStartIndex = -1;

                    if (instructionTokensList.Count == 0)
                        continue;

                    //Check specified syntax
                    Token[] instructionTokens = instructionTokensList.ToArray();

                    Token firstToken = instructionTokens[0];

                    //For command
                    if (firstToken.type == Token.TOKEN_TYPE.IDENTIFIER)
                    {
                        Command command;

                        if(!commands.TryGetValue(firstToken.text, out command))
                        {
                            Error("Command '" + firstToken.text + "' not found", firstToken.line, firstToken.offset);
                            return null;
                        }

                        for(instructionTokenIndex = 1; instructionTokenIndex < instructionTokens.Length; instructionTokenIndex++)
                        {
                            instructionTokens[instructionTokenIndex].PrepareValue(this, variablesList, labelsList);

                            if (error)
                                return null;
                        }

                        preparedInstructions.Add(instructionTokens);
                    }
                    //For label
                    else if (firstToken.type == Token.TOKEN_TYPE.LABEL)
                    {
                        if(instructionTokens.Length == 1)
                            labelsList.Add(new Label(firstToken.text, preparedInstructions.Count - 1));
                        else
                        {
                            Token nextToken = instructionTokens[1];

                            Error("Unexpected " + nextToken.Presentation(), nextToken.line, nextToken.offset);
                            return null;
                        }
                    }
                    //For variable assignment
                    else if (firstToken.type == Token.TOKEN_TYPE.VARIABLE)
                    {
                        if (instructionTokens.Length == 2)
                        {
                            instructionTokens[0].PrepareValue(this, variablesList, labelsList);
                            instructionTokens[1].PrepareValue(this, variablesList, labelsList);

                            if (error)
                                return null;

                            preparedInstructions.Add(instructionTokens);
                        }
                        else if (instructionTokens.Length > 2)
                        {
                            Token nextToken = instructionTokens[2];

                            Error("Unexpected " + nextToken.Presentation(), nextToken.line, nextToken.offset);
                            return null;
                        }
                    }
                }

                tokenIndex++;
            }

            //------------------------------#4 Build script------------------------------
            List<Instruction> instructions = new List<Instruction>();

            foreach (Token[] preparedInstruction in preparedInstructions)
            {
                Token firstToken = preparedInstruction[0];

                if(firstToken.type == Token.TOKEN_TYPE.IDENTIFIER)
                {
                    List<Value> parameters = new List<Value>();

                    for(int valueIndex = 1; valueIndex < preparedInstruction.Length; valueIndex++)
                    {
                        if (preparedInstruction[valueIndex].value == null)
                        { 
                            preparedInstruction[valueIndex].PrepareValue(this, variablesList, labelsList, true);

                            if (error)
                                return null;
                        }

                        parameters.Add(preparedInstruction[valueIndex].value);
                    }

                    Command command = commands[firstToken.text];

                    instructions.Add(new Instruction(command, parameters.ToArray()));
                }
                else if (firstToken.type == Token.TOKEN_TYPE.VARIABLE)
                {
                    if (preparedInstruction[1].value == null)
                    {
                        preparedInstruction[1].PrepareValue(this, variablesList, labelsList, true);
                        
                        if (error)
                            return null;
                    }

                    instructions.Add(new Instruction(null, new Value[] { firstToken.value, preparedInstruction[1].value}));
                }
            }

            MiniScript script = new MiniScript();
            
            script.instructions = instructions.ToArray();
            script.variables = variablesList;
            script.labels = labelsList;

            return script;
        }

        public void Error(string message, Line line, int offset)
        {
            error = true;
            errorMessage = message;
            errorLine = line;
            errorOffset = offset;
        }

        public Compiler()
        {
            commands = new Dictionary<string, Command>();
            constants = new Dictionary<string, Value>();
        }
    }

    protected Instruction[] instructions;

    protected List<Variable> variables;
    protected List<Label> labels;

}
