// Decompiled with JetBrains decompiler
// Type: debugwatch.AssemblerView
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using KeystoneNET;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace debugwatch
{
  public class AssemblerView : Form
  {
    private ulong _address;
    private byte[] result;
    private IContainer components;
    private Button AssembleButton;
    private Button CloseButton;
    private TextBox AssemblerTextBox;

    public AssemblerView(ulong address)
    {
      this.InitializeComponent();
      this._address = address;
    }

    public byte[] GetAssemblerResult()
    {
      return this.result;
    }

    private void AssembleButton_Click(object sender, EventArgs e)
    {
      using (Keystone keystone = new Keystone(KeystoneArchitecture.KS_ARCH_X86, KeystoneMode.KS_MODE_MIPS64, true))
        this.result = keystone.Assemble(this.AssemblerTextBox.Text, this._address).Buffer;
      this.Close();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (AssemblerView));
      this.AssembleButton = new Button();
      this.CloseButton = new Button();
      this.AssemblerTextBox = new TextBox();
      this.SuspendLayout();
      this.AssembleButton.Location = new Point(12, 326);
      this.AssembleButton.Name = "AssembleButton";
      this.AssembleButton.Size = new Size(75, 23);
      this.AssembleButton.TabIndex = 0;
      this.AssembleButton.Text = "Assemble";
      this.AssembleButton.UseVisualStyleBackColor = true;
      this.AssembleButton.Click += new EventHandler(this.AssembleButton_Click);
      this.CloseButton.Location = new Point(93, 326);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new Size(75, 23);
      this.CloseButton.TabIndex = 1;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      this.CloseButton.Click += new EventHandler(this.CloseButton_Click);
      this.AssemblerTextBox.BackColor = SystemColors.ScrollBar;
      this.AssemblerTextBox.Font = new Font("Consolas", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.AssemblerTextBox.ForeColor = Color.Black;
      this.AssemblerTextBox.Location = new Point(12, 12);
      this.AssemblerTextBox.Multiline = true;
      this.AssemblerTextBox.Name = "AssemblerTextBox";
      this.AssemblerTextBox.ScrollBars = ScrollBars.Both;
      this.AssemblerTextBox.Size = new Size(460, 308);
      this.AssemblerTextBox.TabIndex = 2;
      this.AssemblerTextBox.WordWrap = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(484, 361);
      this.Controls.Add((Control) this.AssemblerTextBox);
      this.Controls.Add((Control) this.CloseButton);
      this.Controls.Add((Control) this.AssembleButton);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AssemblerView);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Assembler";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
