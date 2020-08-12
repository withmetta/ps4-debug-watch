// Decompiled with JetBrains decompiler
// Type: KeystoneNET.KeystoneImports
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KeystoneNET
{
  internal class KeystoneImports
  {
    static KeystoneImports()
    {
      string str = Path.GetDirectoryName(new Uri(typeof (KeystoneImports).Assembly.CodeBase).LocalPath) + (IntPtr.Size == 8 ? "\\win64\\" : "\\win32\\") + "keystone.dll";
      if (!File.Exists(str))
        return;
      KeystoneImports.LoadLibrary(str);
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("keystone.dll", EntryPoint = "ks_version", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint Version(ref uint major, ref uint minor);

    [DllImport("keystone.dll", EntryPoint = "ks_open", CallingConvention = CallingConvention.Cdecl)]
    internal static extern KeystoneError Open(
      KeystoneArchitecture arch,
      int mode,
      ref IntPtr ks);

    [DllImport("keystone.dll", EntryPoint = "ks_close", CallingConvention = CallingConvention.Cdecl)]
    internal static extern KeystoneError Close(IntPtr ks);

    [DllImport("keystone.dll", EntryPoint = "ks_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void Free(IntPtr buffer);

    [DllImport("keystone.dll", EntryPoint = "ks_strerror", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr ErrorToString(KeystoneError code);

    [DllImport("keystone.dll", EntryPoint = "ks_errno", CallingConvention = CallingConvention.Cdecl)]
    internal static extern KeystoneError GetLastKeystoneError(IntPtr ks);

    [DllImport("keystone.dll", EntryPoint = "ks_arch_supported", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool IsArchitectureSupported(KeystoneArchitecture arch);

    [DllImport("keystone.dll", EntryPoint = "ks_option", CallingConvention = CallingConvention.Cdecl)]
    internal static extern KeystoneError SetOption(
      IntPtr ks,
      KeystoneOptionType type,
      IntPtr value);

    [DllImport("keystone.dll", EntryPoint = "ks_asm", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int Assemble(
      IntPtr ks,
      [MarshalAs(UnmanagedType.LPStr)] string toEncode,
      ulong baseAddress,
      out IntPtr encoding,
      out uint size,
      out uint statements);
  }
}
