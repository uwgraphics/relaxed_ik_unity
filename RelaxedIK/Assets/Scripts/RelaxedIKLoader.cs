using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using fts;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Opt
{
    public double* data;
    public int length;
}

[PluginAttr("relaxed_ik_lib")]
public static class RelaxedIKLoader
{
    [PluginFunctionAttr("solve")]
    public static Solve solve = null;
    public delegate Opt Solve(double[] pos_arr, int pos_len, double[] quat_arr, int quat_len);
}