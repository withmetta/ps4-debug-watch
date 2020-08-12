// Decompiled with JetBrains decompiler
// Type: KeystoneNET.KeystoneEncoded
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

namespace KeystoneNET
{
  public class KeystoneEncoded
  {
    public KeystoneEncoded(byte[] buffer, uint statementCount, ulong address)
    {
      this.Buffer = buffer;
      this.StatementCount = statementCount;
      this.Address = address;
    }

    public ulong Address { get; private set; }

    public byte[] Buffer { get; private set; }

    public uint StatementCount { get; private set; }
  }
}
