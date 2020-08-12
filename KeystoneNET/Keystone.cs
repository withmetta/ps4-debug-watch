// Decompiled with JetBrains decompiler
// Type: KeystoneNET.Keystone
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace KeystoneNET
{
  public class Keystone : IDisposable
  {
    private IntPtr engine = IntPtr.Zero;
    private List<Keystone.Resolver> resolvers = new List<Keystone.Resolver>();
    private bool throwOnError;
    private bool addedResolveSymbol;
    private Keystone.ResolverInternal internalImpl;

    public event Keystone.Resolver ResolveSymbol
    {
      add
      {
        this.resolvers.Add(value);
        if (this.addedResolveSymbol)
          return;
        int num = (int) KeystoneImports.SetOption(this.engine, KeystoneOptionType.KS_OPT_SYM_RESOLVER, Marshal.GetFunctionPointerForDelegate((Delegate) this.internalImpl));
        this.addedResolveSymbol = true;
      }
      remove
      {
        this.resolvers.Remove(value);
        if (!this.addedResolveSymbol || this.resolvers.Count != 0)
          return;
        int num = (int) KeystoneImports.SetOption(this.engine, KeystoneOptionType.KS_OPT_SYM_RESOLVER, IntPtr.Zero);
        this.addedResolveSymbol = false;
      }
    }

    private bool SymbolResolver(IntPtr symbolPtr, ref ulong value)
    {
      string stringAnsi = Marshal.PtrToStringAnsi(symbolPtr);
      foreach (Keystone.Resolver resolver in this.resolvers)
      {
        if (resolver(stringAnsi, ref value))
          return true;
      }
      return false;
    }

    public Keystone(
      KeystoneArchitecture architecture,
      KeystoneMode mode,
      bool throwOnKeystoneError = true)
    {
      this.internalImpl = new Keystone.ResolverInternal(this.SymbolResolver);
      this.throwOnError = throwOnKeystoneError;
      KeystoneError result = KeystoneImports.Open(architecture, (int) mode, ref this.engine);
      if ((uint) result > 0U & throwOnKeystoneError)
        throw new InvalidOperationException(string.Format("Error while initializing keystone: {0}", (object) Keystone.ErrorToString(result)));
    }

    public bool SetOption(KeystoneOptionType type, uint value)
    {
      KeystoneError result = KeystoneImports.SetOption(this.engine, type, (IntPtr) (long) value);
      if (result == KeystoneError.KS_ERR_OK)
        return true;
      if (this.throwOnError)
        throw new InvalidOperationException(string.Format("Error while setting option in keystone: {0}", (object) Keystone.ErrorToString(result)));
      return false;
    }

    public static string ErrorToString(KeystoneError result)
    {
      IntPtr ptr = KeystoneImports.ErrorToString(result);
      return ptr != IntPtr.Zero ? Marshal.PtrToStringAnsi(ptr) : string.Empty;
    }

    public KeystoneEncoded Assemble(string toEncode, ulong address)
    {
      IntPtr encoding;
      uint size;
      uint statements;
      if (KeystoneImports.Assemble(this.engine, toEncode, address, out encoding, out size, out statements) != 0)
      {
        if (this.throwOnError)
          throw new InvalidOperationException(string.Format("Error while assembling {0}: {1}", (object) toEncode, (object) Keystone.ErrorToString(this.GetLastKeystoneError())));
        return (KeystoneEncoded) null;
      }
      byte[] numArray = new byte[(int) size];
      Marshal.Copy(encoding, numArray, 0, (int) size);
      KeystoneImports.Free(encoding);
      return new KeystoneEncoded(numArray, statements, address);
    }

    public bool AppendAssemble(
      string toEncode,
      ICollection<byte> encoded,
      ulong address,
      out int size,
      out uint statements)
    {
      if (encoded == null)
        throw new ArgumentNullException(nameof (encoded));
      if (toEncode == null)
        throw new ArgumentNullException(nameof (toEncode));
      if (encoded.IsReadOnly)
        throw new ArgumentException("encoded collection can't be read-only.");
      KeystoneEncoded keystoneEncoded = this.Assemble(toEncode, address);
      if (keystoneEncoded != null)
      {
        foreach (byte num in keystoneEncoded.Buffer)
          encoded.Add(num);
        size = keystoneEncoded.Buffer.Length;
        statements = keystoneEncoded.StatementCount;
        return true;
      }
      size = 0;
      statements = 0U;
      return false;
    }

    public bool AppendAssemble(
      string toEncode,
      ICollection<byte> encoded,
      ulong address,
      out int size)
    {
      return this.AppendAssemble(toEncode, encoded, address, out size, out uint _);
    }

    public bool AppendAssemble(string toEncode, ICollection<byte> encoded, ulong address)
    {
      return this.AppendAssemble(toEncode, encoded, address, out int _, out uint _);
    }

    public bool AppendAssemble(string toEncode, ICollection<byte> encoded)
    {
      return this.AppendAssemble(toEncode, encoded, 0UL, out int _, out uint _);
    }

    public KeystoneError GetLastKeystoneError()
    {
      return KeystoneImports.GetLastKeystoneError(this.engine);
    }

    public static bool IsArchitectureSupported(KeystoneArchitecture architecture)
    {
      return KeystoneImports.IsArchitectureSupported(architecture);
    }

    public static uint GetKeystoneVersion(ref uint major, ref uint minor)
    {
      return KeystoneImports.Version(ref major, ref minor);
    }

    public void Dispose()
    {
      IntPtr ks = Interlocked.Exchange(ref this.engine, IntPtr.Zero);
      if (ks != IntPtr.Zero)
      {
        int num = (int) KeystoneImports.Close(ks);
      }
      GC.SuppressFinalize((object) this);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool ResolverInternal(IntPtr symbol, ref ulong value);

    public delegate bool Resolver(string symbol, ref ulong value);
  }
}
