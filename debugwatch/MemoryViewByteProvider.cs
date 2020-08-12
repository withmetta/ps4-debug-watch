// Decompiled with JetBrains decompiler
// Type: debugwatch.MemoryViewByteProvider
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using Be.Windows.Forms;
using System;

namespace debugwatch
{
  public class MemoryViewByteProvider : IByteProvider
  {
    private bool _hasChanges;
    private ByteCollection _bytes;

    public MemoryViewByteProvider(byte[] data)
      : this(new ByteCollection(data))
    {
    }

    public MemoryViewByteProvider(ByteCollection bytes)
    {
      this._bytes = bytes;
    }

    private void OnChanged(EventArgs e)
    {
      this._hasChanges = true;
      if (this.Changed == null)
        return;
      this.Changed((object) this, e);
    }

    private void OnLengthChanged(EventArgs e)
    {
      if (this.LengthChanged == null)
        return;
      this.LengthChanged((object) this, e);
    }

    public ByteCollection Bytes
    {
      get
      {
        return this._bytes;
      }
    }

    public bool HasChanges()
    {
      return this._hasChanges;
    }

    public void ApplyChanges()
    {
      this._hasChanges = false;
    }

    public event EventHandler Changed;

    public event EventHandler LengthChanged;

    public byte ReadByte(long index)
    {
      return this._bytes[(int) index];
    }

    public void WriteByte(long index, byte value)
    {
      this._bytes[(int) index] = value;
      this.OnChanged(EventArgs.Empty);
    }

    public void DeleteBytes(long index, long length)
    {
      this._bytes.RemoveRange((int) Math.Max(0L, index), (int) Math.Min((long) (int) this.Length, length));
      this.OnLengthChanged(EventArgs.Empty);
      this.OnChanged(EventArgs.Empty);
    }

    public void InsertBytes(long index, byte[] bs)
    {
      this._bytes.InsertRange((int) index, bs);
      this.OnLengthChanged(EventArgs.Empty);
      this.OnChanged(EventArgs.Empty);
    }

    public long Length
    {
      get
      {
        return (long) this._bytes.Count;
      }
    }

    public long Offset
    {
      get
      {
        return 0;
      }
    }

    public bool SupportsWriteByte()
    {
      return true;
    }

    public bool SupportsInsertBytes()
    {
      return false;
    }

    public bool SupportsDeleteBytes()
    {
      return false;
    }
  }
}
