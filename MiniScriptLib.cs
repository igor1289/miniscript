using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniScriptLib
{
    static void GoTo(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        MiniScript.Value labelParameter= parameters.Get(0, null);

        if (labelParameter == null)
            return;

        MiniScript.Label label = labelParameter.TryGetLabel(runtime, null);

        if (label != null)
            runtime.PlayFromLabel(label);
    }

    static void GoToIf(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { 0, (uint)MiniScript.Value.VALUE_TYPE.VARIABLE + (uint)MiniScript.Value.VALUE_TYPE.LABEL}))
        {
            MiniScript.Label label = parameters.Get(1).TryGetLabel(runtime);

            if (label != null && parameters.Get(0).TryGetBool(runtime) == true)
                runtime.PlayFromLabel(label);
        }
    }

    static void Add(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] {(uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) + b.TryGetFloat(runtime, 0f));
        }
    }

    static void Sub(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) - b.TryGetFloat(runtime, 0f));
        }
    }

    static void Mul(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) * b.TryGetFloat(runtime, 0f));
        }
    }

    static void Div(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            float divisor = b.TryGetFloat(runtime, 0f);

            if(divisor != 0f)
                result.Set(a.TryGetFloat(runtime, 0f) / divisor);
            else
                result.Set("Infinity");
        }
    }

    static void Mod(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] {(uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) % b.TryGetFloat(runtime, 0f));
        }
    }

    static void Pow(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(Mathf.Pow(a.TryGetFloat(runtime, 0f), b.TryGetFloat(runtime, 0f)));
        }
    }

    static void And(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        int resultIndex = parameters.Length - 1;

        if (resultIndex < 1 || parameters.Get(resultIndex).type != MiniScript.Value.VALUE_TYPE.VARIABLE)
            return;

        MiniScript.Value result = parameters.Get(resultIndex).GetVariableValue(runtime);

        result.Set(true);

        for(int parameterIndex = 0; parameterIndex < resultIndex; parameterIndex++)
        {
            MiniScript.Value parameter = parameters.Get(parameterIndex);

            if (!parameter.TryGetBool(runtime))
            {
                result.Set(false);
                return;
            }
        }
    }

    static void Or(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        int resultIndex = parameters.Length - 1;

        if (resultIndex < 1 || parameters.Get(resultIndex).type != MiniScript.Value.VALUE_TYPE.VARIABLE)
            return;

        MiniScript.Value result = parameters.Get(resultIndex).GetVariableValue(runtime);

        result.Set(true);

        for(int parameterIndex = 0; parameterIndex < resultIndex; parameterIndex++)
        {
            MiniScript.Value parameter = parameters.Get(parameterIndex);

            if (parameter.TryGetBool(runtime))
            {
                result.Set(true);
                return;
            }
        }
    }

    static void Not(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 1))
        {
            MiniScript.Value result = parameters.Get(1).GetVariableValue(runtime);

            MiniScript.Value parameter = parameters.Get(0);

            result.Set(!parameter.TryGetBool(runtime));
        }
    }

    static void Equal(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(MiniScript.Value.IsEqual(a, b, runtime));
        }
    }

    static void Greater(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) > b.TryGetFloat(runtime, 0f));
        }
    }

    static void GreaterOrEqual(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) > b.TryGetFloat(runtime, 0f));
        }
    }

    static void Less(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) < b.TryGetFloat(runtime, 0f));
        }
    }

    static void LessOrEqual(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        if (parameters.IsValid(new uint[] { (uint)MiniScript.Value.VALUE_TYPE.VARIABLE }, 2))
        {
            MiniScript.Value a = parameters.Get(0);
            MiniScript.Value b = parameters.Get(1);
            MiniScript.Value result = parameters.Get(2).GetVariableValue(runtime);

            a = a.TryGetVariableValue(runtime, a);
            b = b.TryGetVariableValue(runtime, b);

            result.Set(a.TryGetFloat(runtime, 0f) <= b.TryGetFloat(runtime, 0f));
        }
    }

    static void Pause(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
    {
        float waitTime;

        if (parameters.Length >= 1)
            waitTime = parameters.Get(0).TryGetFloat(runtime, -1);
        else
            waitTime = -1;

        runtime.Pause(waitTime);
    }

    static void Exit(MiniScript.Parameters parameters, MiniScript.Runtime runtime)
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
