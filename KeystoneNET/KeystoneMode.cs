// Decompiled with JetBrains decompiler
// Type: KeystoneNET.KeystoneMode
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

namespace KeystoneNET
{
  public enum KeystoneMode : uint
  {
    KS_MODE_LITTLE_ENDIAN = 0,
    KS_MODE_ARM = 1,
    KS_MODE_16 = 2,
    KS_MODE_32 = 4,
    KS_MODE_MIPS32 = 4,
    KS_MODE_PPC32 = 4,
    KS_MODE_SPARC32 = 4,
    KS_MODE_64 = 8,
    KS_MODE_MIPS64 = 8,
    KS_MODE_PPC64 = 8,
    KS_MODE_SPARC64 = 8,
    KS_MODE_MICRO = 16, // 0x00000010
    KS_MODE_QPX = 16, // 0x00000010
    KS_MODE_THUMB = 16, // 0x00000010
    KS_MODE_V9 = 16, // 0x00000010
    KS_MODE_MIPS3 = 32, // 0x00000020
    KS_MODE_MIPS32R6 = 64, // 0x00000040
    KS_MODE_V8 = 64, // 0x00000040
    KS_MODE_BIG_ENDIAN = 1073741824, // 0x40000000
  }
}
