// Decompiled with JetBrains decompiler
// Type: KeystoneNET.KeystoneOptionValue
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

namespace KeystoneNET
{
  public enum KeystoneOptionValue : short
  {
    KS_OPT_SYNTAX_INTEL = 1,
    KS_OPT_SYNTAX_ATT = 2,
    KS_OPT_SYNTAX_NASM = 4,
    KS_OPT_SYNTAX_MASM = 8,
    KS_OPT_SYNTAX_GAS = 16, // 0x0010
    KS_OPT_SYNTAX_RADIX16 = 32, // 0x0020
  }
}
