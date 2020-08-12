// Decompiled with JetBrains decompiler
// Type: debugwatch.MemoryMapView
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using libdebug;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace debugwatch
{
  public class MemoryMapView : Form
  {
    private ProcessInfo processinfo;
    private ProcessMap processMap;
    private IContainer components;
    private Button CloseButton;
    private CheckedListBox MemoryMapCheckedListBox;
    private Label HelpLabel;
    private Button AutoSelectButton;
    private Button ClearSelectButton;
    private TextBox SearchTextBox;
    private Label SearchLabel;

    public MemoryMapView(ProcessInfo pi, ProcessMap processMap)
    {
      this.InitializeComponent();
      this.processinfo = pi;
      this.processMap = processMap;
      


      for (int index = 0; index < this.processMap.entries.Length; ++index)
      {
        MemoryEntry entry = this.processMap.entries[index];
        ulong num = entry.end - entry.start;
        this.MemoryMapCheckedListBox.Items.Add((object) (entry.name +
                                                         " start: 0x" +
                                                         entry.start.ToString("X") +
                                                         " length: 0x" +
                                                         num.ToString("X") +
                                                         " prot: " +
                                                         (object) entry.prot));
      }
      this.AutoSelectButton_Click((object) null, (EventArgs) null);
    }

    public MemoryEntry[] GetSelectedEntries()
    {
      List<MemoryEntry> memoryEntryList = new List<MemoryEntry>();
      for (int index1 = 0; index1 < this.processMap.entries.Length; ++index1)
      {
        MemoryEntry entry = this.processMap.entries[index1];
        if (entry.name.Length != 0)
        {
          for (int index2 = 0; index2 < this.MemoryMapCheckedListBox.CheckedItems.Count; ++index2)
          {
            if (this.MemoryMapCheckedListBox.CheckedItems[index2].ToString().StartsWith(entry.name))
            {
              memoryEntryList.Add(entry);
              break;
            }
          }
        }
      }
      return memoryEntryList.ToArray();
    }

    private void AutoSelectButton_Click(object sender, EventArgs e)
    {
      string[] strArray = new string[3]
      {
        "executable",
        "anon:",
        "heap"
      };
      for (int index1 = 0; index1 < this.MemoryMapCheckedListBox.Items.Count; ++index1)
      {
        for (int index2 = 0; index2 < strArray.Length; ++index2)
        {
          if (this.MemoryMapCheckedListBox.Items[index1].ToString().ToLower().Contains(strArray[index2]))
          {
            this.MemoryMapCheckedListBox.SetItemChecked(index1, true);
            break;
          }
        }
      }
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ClearSelectButton_Click(object sender, EventArgs e)
    {
      for (int index = 0; index < this.MemoryMapCheckedListBox.Items.Count; ++index)
        this.MemoryMapCheckedListBox.SetItemChecked(index, false);
    }

    private void SearchTextBox_TextChanged(object sender, EventArgs e)
    {
      this.MemoryMapCheckedListBox.Items.Clear();
      for (int index = 0; index < this.processMap.entries.Length; ++index)
      {
        MemoryEntry entry = this.processMap.entries[index];
        ulong num = entry.end - entry.start;
        if (entry.name.Contains(this.SearchTextBox.Text))
          this.MemoryMapCheckedListBox.Items.Add((object) (entry.name + " start: 0x" + entry.start.ToString("X") + " length: 0x" + num.ToString("X") + " prot: " + (object) entry.prot));
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MemoryMapView));
      this.CloseButton = new Button();
      this.MemoryMapCheckedListBox = new CheckedListBox();
      this.HelpLabel = new Label();
      this.AutoSelectButton = new Button();
      this.ClearSelectButton = new Button();
      this.SearchTextBox = new TextBox();
      this.SearchLabel = new Label();
      this.SuspendLayout();
      this.CloseButton.Location = new Point(204, 363);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new Size(75, 23);
      this.CloseButton.TabIndex = 0;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      this.CloseButton.Click += new EventHandler(this.CloseButton_Click);
      this.MemoryMapCheckedListBox.BackColor = Color.LightGray;
      this.MemoryMapCheckedListBox.Font = new Font("Consolas", 9f);
      this.MemoryMapCheckedListBox.FormattingEnabled = true;
      this.MemoryMapCheckedListBox.Location = new Point(12, 12);
      this.MemoryMapCheckedListBox.Name = "MemoryMapCheckedListBox";
      this.MemoryMapCheckedListBox.ScrollAlwaysVisible = true;
      this.MemoryMapCheckedListBox.Size = new Size(560, 344);
      this.MemoryMapCheckedListBox.TabIndex = 1;
      this.HelpLabel.AutoSize = true;
      this.HelpLabel.Font = new Font("Consolas", 8.25f);
      this.HelpLabel.Location = new Point(12, 389);
      this.HelpLabel.Name = "HelpLabel";
      this.HelpLabel.Size = new Size(421, 13);
      this.HelpLabel.TabIndex = 23;
      this.HelpLabel.Text = "Please select memory regions from above view in order to search them.";
      this.AutoSelectButton.Location = new Point(12, 363);
      this.AutoSelectButton.Name = "AutoSelectButton";
      this.AutoSelectButton.Size = new Size(75, 23);
      this.AutoSelectButton.TabIndex = 24;
      this.AutoSelectButton.Text = "Auto Select";
      this.AutoSelectButton.UseVisualStyleBackColor = true;
      this.AutoSelectButton.Click += new EventHandler(this.AutoSelectButton_Click);
      this.ClearSelectButton.Location = new Point(93, 363);
      this.ClearSelectButton.Name = "ClearSelectButton";
      this.ClearSelectButton.Size = new Size(105, 23);
      this.ClearSelectButton.TabIndex = 25;
      this.ClearSelectButton.Text = "Clear Selection";
      this.ClearSelectButton.UseVisualStyleBackColor = true;
      this.ClearSelectButton.Click += new EventHandler(this.ClearSelectButton_Click);
      this.SearchTextBox.Location = new Point(374, 365);
      this.SearchTextBox.Name = "SearchTextBox";
      this.SearchTextBox.Size = new Size(198, 20);
      this.SearchTextBox.TabIndex = 26;
      this.SearchTextBox.TextChanged += new EventHandler(this.SearchTextBox_TextChanged);
      this.SearchLabel.AutoSize = true;
      this.SearchLabel.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.SearchLabel.Location = new Point(319, 368);
      this.SearchLabel.Name = "SearchLabel";
      this.SearchLabel.Size = new Size(49, 14);
      this.SearchLabel.TabIndex = 27;
      this.SearchLabel.Text = "Search";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(584, 411);
      this.Controls.Add((Control) this.SearchLabel);
      this.Controls.Add((Control) this.SearchTextBox);
      this.Controls.Add((Control) this.ClearSelectButton);
      this.Controls.Add((Control) this.AutoSelectButton);
      this.Controls.Add((Control) this.HelpLabel);
      this.Controls.Add((Control) this.MemoryMapCheckedListBox);
      this.Controls.Add((Control) this.CloseButton);
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (MemoryMapView);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Memory Map";
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
