// Decompiled with JetBrains decompiler
// Type: debugwatch.RichTextBoxExtensions
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using System.Drawing;
using System.Windows.Forms;

namespace debugwatch
{
  public static class RichTextBoxExtensions
  {
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
      box.SelectionStart = box.TextLength;
      box.SelectionLength = 0;
      box.SelectionColor = color;
      box.AppendText(text);
      box.SelectionColor = box.ForeColor;
    }
  }
}
