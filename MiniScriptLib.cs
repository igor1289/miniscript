using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniScriptLib
{
    static void GoTo(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if(parameters.Length >= 1)
        {
            MiniScript.Label label = parameters[0].GetLabel();

            if (label != null)
                runtime.PlayFromLabel(label);
        }
    }

    static void GoToIf(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 2)
        {
            MiniScript.Label label = parameters[0].GetLabel();
            MiniScript.Value value;

            if (parameters[1].type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                value = parameters[1].GetVariableValue(runtime);
            else
                value = parameters[1];

            if (label != null && value.GetBool(runtime) == true)
                runtime.PlayFromLabel(label);
        }
    }

    static void Add(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if(parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                {
                    a = a.GetVariableValue(runtime);
                }

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                {
                    b = b.GetVariableValue(runtime);
                }

                if (a.type == MiniScript.Value.VALUE_TYPE.FLOAT || b.type == MiniScript.Value.VALUE_TYPE.FLOAT)
                    result.Set(a.GetFloat() + b.GetFloat());
                else
                    result.Set((int)a.value + (int)b.value);
            }
        }
    }

    static void Sub(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                if (a.type == MiniScript.Value.VALUE_TYPE.FLOAT || b.type == MiniScript.Value.VALUE_TYPE.FLOAT)
                    result.Set(a.GetFloat() - b.GetFloat());
                else
                    result.Set((int)a.value - (int)b.value);
            }
        }
    }

    static void Mul(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                if (a.type == MiniScript.Value.VALUE_TYPE.FLOAT || b.type == MiniScript.Value.VALUE_TYPE.FLOAT)
                    result.Set(a.GetFloat() * b.GetFloat());
                else
                    result.Set((int)a.value * (int)b.value);
            }
        }
    }

    static void Div(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                float divisor = b.GetFloat();

                if(divisor != 0f)
                    result.Set(a.GetFloat() / b.GetFloat());
                else
                    result.Set("Infinity");
            }
        }
    }

    static void Mod(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                if (a.type == MiniScript.Value.VALUE_TYPE.FLOAT || b.type == MiniScript.Value.VALUE_TYPE.FLOAT)
                    result.Set(a.GetFloat() % b.GetFloat());
                else
                    result.Set((int)a.value % (int)b.value);
            }
        }
    }

    static void Pow(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                if (a.type == MiniScript.Value.VALUE_TYPE.FLOAT || b.type == MiniScript.Value.VALUE_TYPE.FLOAT)
                    result.Set(Mathf.Pow(a.GetFloat(), b.GetFloat()));
                else
                    result.Set(Mathf.Pow((int)a.value, (int)b.value));
            }
        }
    }

    static void And(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        int resultIndex = parameters.Length - 1;

        if (resultIndex < 1 || parameters[resultIndex].type != MiniScript.Value.VALUE_TYPE.VARIABLE)
            return;

        MiniScript.Value result = parameters[resultIndex].GetVariableValue(runtime);

        result.Set(true);

        for(int parameterIndex = 0; parameterIndex < resultIndex; parameterIndex++)
        {
            MiniScript.Value parameter = parameters[parameterIndex];

            if(parameter.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                parameter = parameter.GetVariableValue(runtime);
            }

            if (!parameter.GetBool(runtime))
            {
                result.Set(false);
                return;
            }
        }
    }

    static void Or(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        int resultIndex = parameters.Length - 1;

        if (resultIndex < 1 || parameters[resultIndex].type != MiniScript.Value.VALUE_TYPE.VARIABLE)
            return;

        MiniScript.Value result = parameters[resultIndex].GetVariableValue(runtime);

        result.Set(false);

        for (int parameterIndex = 0; parameterIndex < resultIndex; parameterIndex++)
        {
            MiniScript.Value parameter = parameters[parameterIndex];

            if (parameter.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                parameter = parameter.GetVariableValue(runtime);
            }

            if (parameter.GetBool(runtime))
            {
                result.Set(true);
                return;
            }
        }
    }

    static void Not(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 2)
        {
            MiniScript.Value result = parameters[1];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value parameter = parameters[0];

                result.Set(!parameter.GetBool(runtime));
            }
        }
    }

    static void Equal(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];


                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);


                if (a.type == MiniScript.Value.VALUE_TYPE.NULL && b.type == MiniScript.Value.VALUE_TYPE.NULL)
                {
                    result.Set(true);
                    return;
                }
                else if (a.type == MiniScript.Value.VALUE_TYPE.BOOL && b.type == MiniScript.Value.VALUE_TYPE.BOOL)
                {
                    result.Set((bool)a.value == (bool)b.value);
                    return;
                }
                else if ((a.type == MiniScript.Value.VALUE_TYPE.INT || a.type == MiniScript.Value.VALUE_TYPE.FLOAT) && (b.type == MiniScript.Value.VALUE_TYPE.INT || b.type == MiniScript.Value.VALUE_TYPE.FLOAT))
                {
                    result.Set(a.GetFloat() == b.GetFloat());
                    return;
                }
                else if ((a.type == MiniScript.Value.VALUE_TYPE.STRING || a.type == MiniScript.Value.VALUE_TYPE.STRING_TEMPLATE) && (b.type == MiniScript.Value.VALUE_TYPE.STRING || b.type == MiniScript.Value.VALUE_TYPE.STRING_TEMPLATE))
                {
                    string str_a = a.type == MiniScript.Value.VALUE_TYPE.STRING ? (string)a.value : a.GetStringTemplate().GetString(runtime);
                    string str_b = b.type == MiniScript.Value.VALUE_TYPE.STRING ? (string)b.value : b.GetStringTemplate().GetString(runtime);

                    result.Set(str_a == str_b);
                    return;
                }
                else if (a.type == MiniScript.Value.VALUE_TYPE.LABEL && b.type == MiniScript.Value.VALUE_TYPE.LABEL || a.type == MiniScript.Value.VALUE_TYPE.OBJECT && b.type == MiniScript.Value.VALUE_TYPE.OBJECT)
                {
                    result.Set(a.value == b.value);
                    return;
                }

                result.Set(false);
            }
        }
    }

    static void Greater(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];

                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                float af = a.GetFloat();
                float bf = b.GetFloat();

                result.Set(a.GetFloat() > b.GetFloat());
            }
        }
    }

    static void GreaterOrEqual(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];


                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);

                result.Set(a.GetFloat() >= b.GetFloat());
            }
        }
    }

    static void Less(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];


                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);


                result.Set(a.GetFloat() < b.GetFloat());
            }
        }
    }

    static void LessOrEqual(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        if (parameters.Length >= 3)
        {
            MiniScript.Value result = parameters[2];

            if (result.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
            {
                result = result.GetVariableValue(runtime);

                MiniScript.Value a = parameters[0];
                MiniScript.Value b = parameters[1];


                if (a.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    a = a.GetVariableValue(runtime);

                if (b.type == MiniScript.Value.VALUE_TYPE.VARIABLE)
                    b = b.GetVariableValue(runtime);


                result.Set(a.GetFloat() <= b.GetFloat());
            }
        }
    }

    static void Pause(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        float waitTime;

        if (parameters.Length >= 1)
            if(parameters[0].type != MiniScript.Value.VALUE_TYPE.VARIABLE)
                waitTime = parameters[0].GetFloat(-1);
            else
                waitTime = parameters[0].GetVariableValue(runtime).GetFloat(-1);
        else
            waitTime = -1;

        runtime.Pause(waitTime);
    }

    static void Exit(MiniScript.Value[] parameters, MiniScript.Runtime runtime)
    {
        runtime.ResetPlayback();
    }

    public static void Include(MiniScript.Compiler compiler)
    {
        compiler.AddCommand("goto", GoTo);                          //goto          @label
        compiler.AddCommand("gotoif", GoToIf);                      //gotoif        value       @label
        compiler.AddCommand("add", Add);                            //add           valueA      valueB      $result
        compiler.AddCommand("sub", Sub);                            //sub           valueA      valueB      $result
        compiler.AddCommand("mul", Mul);                            //mul           valueA      valueB      $result
        compiler.AddCommand("div", Div);                            //div           valueA      valueB      $result
        compiler.AddCommand("mod", Mod);                            //mod           valueA      valueB      $result
        compiler.AddCommand("pow", Pow);                            //pow           valueA      valueB      $result
        compiler.AddCommand("and", And);                            //and           valueA      valueN      $result
        compiler.AddCommand("or", Or);                              //or            valueA      valueN      $result
        compiler.AddCommand("not", Not);                            //not           valueA      $result
        compiler.AddCommand("equal", Equal);                        //equal         valueA      valueB      $result
        compiler.AddCommand("greater", Greater);                    //greater       valueA      valueB      $result
        compiler.AddCommand("greater_eq", GreaterOrEqual);          //greater_eq    valueA      valueB      $result
        compiler.AddCommand("less", Less);                          //less          valueA      valueB      $result
        compiler.AddCommand("less_eq", LessOrEqual);                //less_eq       valueA      valueB      $result
        compiler.AddCommand("pause", Pause);                        //pause         [wait_time]
        compiler.AddCommand("exit", Exit);                          //exit

        compiler.AddConstant("null", new MiniScript.Value(null, MiniScript.Value.VALUE_TYPE.NULL));
        compiler.AddConstant("true", new MiniScript.Value(true, MiniScript.Value.VALUE_TYPE.BOOL));
        compiler.AddConstant("false", new MiniScript.Value(false, MiniScript.Value.VALUE_TYPE.BOOL));
    }
}
