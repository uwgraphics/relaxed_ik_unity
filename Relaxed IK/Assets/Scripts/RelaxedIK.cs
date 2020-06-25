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

[PluginAttr("relaxed_ik")]
public static class RelaxedIK
{
    /*[DllImport("relaxed_ik")]
    private static extern Opt run_solver(double[] pos_arr, int pos_length, double[] quat_arr, int quat_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Opt RunSolver(double[] pos_arr, int pos_len, double[] quat_arr, int quat_len)
    {
        return run_solver(pos_arr, pos_len, quat_arr, quat_len);
    }*/

    [PluginFunctionAttr("run_unity")]
    public static RunUnity runUnity = null;
    public delegate Opt RunUnity(double[] pos_arr, int pos_len, double[] quat_arr, int quat_len);
}