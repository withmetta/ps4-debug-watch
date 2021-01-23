// Decompiled with JetBrains decompiler
// Type: debugwatch.DebugWatchForm
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using Be.Windows.Forms;
using libdebug;
using SharpDisasm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace debugwatch
{
    public class DebugWatchForm : Form
    {
        private PS4DBG ps4;
        private ProcessList proclist;
        private int attachpid;
        private MemoryMapView mapview;
        private byte[] data;
        private IContainer components;
        private Button ConnectButton;
        private Button AttachButton;
        private TextBox IpTextBox;
        private ComboBox ProcessComboBox;
        private Button RefreshButton;
        private GroupBox ControlAndRegistersGroupBox;
        private GroupBox EditGroupBox;
        private Button StopButton;
        private Button GoButton;
        private Button SetWatchpointButton;
        private TextBox RegistersTextBox;
        private GroupBox WatchpointGroupBox;
        private ComboBox WatchpointLengthComboBox;
        private ComboBox BreaktypeComboBox;
        private GroupBox BreakpointGroupBox;
        private Button SetBreakpointButton;
        private TextBox AddressTextBox;
        private Label AddressLabel;
        private NumericUpDown BreakpointNumericUpDown;
        private NumericUpDown WatchpointNumericUpDown;
        private Button ClearBreakpoints;
        private Button ClearWatchPoints;
        private Button RebootButton;
        private HexBox MemoryHexBox;
        private Button PokeButton;
        private Button PeekButton;
        private TextBox LengthTextBox;
        private Button DetachButton;
        private Button TryFindButton;
        private Button AutoPatchButton;
        private Button MemoryMapButton;
        private CheckBox FilterProcessListCheckBox;
        private Panel MainPanel;
        private GroupBox MemoryScanGroupBox;
        private Label LengthLabel;
        private Button KillProcessButton;
        private Button ClearBreakpointButton;
        private Button ClearWatchpointButton;
        private Button NextScanButton;
        private Button NewScanButton;
        private TextBox ValueTextBox;
        private Label ValueLabel;
        private ComboBox ScanTypeComboBox;
        private Label BreakpointIndexLabel;
        private Label WatchpointIndexLabel;
        private Label WatchpointTypeLabel;
        private Label WatchpointLengthLabel;
        private Label ScanHistoryLabel;
        private Label HelpLabel;
        private ListBox ScanHistoryListBox;
        private DataGridView ScanDataGridView;
        private LinkLabel SupportLinkLabel;
        private Button AssembleButton;
        private TabControl TabControl;
        private TabPage DisassemblyTabPage;
        private TabPage MemoryTabPage;
        private TabPage ScanTabPage;
        private DataGridViewTextBoxColumn AddressColumn;
        private DataGridViewTextBoxColumn TypeColumn;
        private DataGridViewTextBoxColumn ValueColumn;
        private DataGridViewButtonColumn WatchpointColumn;
        private RichTextBox DisassemblyTextBox;
        internal ProgressBar ScanProgressBar;
        private Button CreditsButton;
        internal static DebugWatchForm Singleton;
        private MemoryScanner.SCAN_TYPE lastScanType = MemoryScanner.SCAN_TYPE.LONG;

        private ulong address
        {
            get { return Convert.ToUInt64(this.AddressTextBox.Text.Trim().Replace("0x", ""), 16); }
        }

        private int length
        {
            get { return Convert.ToInt32(this.LengthTextBox.Text.Trim().Replace("0x", ""), 16); }
        }

        public DebugWatchForm()
        {
            this.InitializeComponent();
            this.IpTextBox.Text = Settings.ip;
            this.FilterProcessListCheckBox.Checked = Settings.filter;
            this.WatchpointLengthComboBox.SelectedIndex = 0;
            this.BreaktypeComboBox.SelectedIndex = 0;
            this.ScanTypeComboBox.SelectedIndex = 0;
            Singleton = this;
        }

        private T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] objArray = new T[length];
            Array.Copy((Array) data, index, (Array) objArray, 0, length);
            return objArray;
        }

        private void DebugWatchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ps4 != null && this.ps4.IsConnected)
            {
                if (this.ps4.IsDebugging)
                    this.ps4.DetachDebugger();
                this.ps4.Disconnect();
            }

            Settings.ip = this.IpTextBox.Text;
            Settings.filter = this.FilterProcessListCheckBox.Checked;
            Application.Exit();
        }

        private string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null)
                return "<null>";
            int length = bytes.Length;
            char[] charArray1 = "0123456789ABCDEF".ToCharArray();
            int num1 = 11;
            int num2 = num1 + bytesPerLine * 3 + (bytesPerLine - 1) / 8 + 2;
            int num3 = num2 + bytesPerLine + Environment.NewLine.Length;
            char[] charArray2 =
                (new string(' ', num3 - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            StringBuilder stringBuilder = new StringBuilder((length + bytesPerLine - 1) / bytesPerLine * num3);
            for (int index1 = 0; index1 < length; index1 += bytesPerLine)
            {
                charArray2[0] = charArray1[index1 >> 28 & 15];
                charArray2[1] = charArray1[index1 >> 24 & 15];
                charArray2[2] = charArray1[index1 >> 20 & 15];
                charArray2[3] = charArray1[index1 >> 16 & 15];
                charArray2[4] = charArray1[index1 >> 12 & 15];
                charArray2[5] = charArray1[index1 >> 8 & 15];
                charArray2[6] = charArray1[index1 >> 4 & 15];
                charArray2[7] = charArray1[index1 & 15];
                int index2 = num1;
                int index3 = num2;
                for (int index4 = 0; index4 < bytesPerLine; ++index4)
                {
                    if (index4 > 0 && (index4 & 7) == 0)
                        ++index2;
                    if (index1 + index4 >= length)
                    {
                        charArray2[index2] = ' ';
                        charArray2[index2 + 1] = ' ';
                        charArray2[index3] = ' ';
                    }
                    else
                    {
                        byte num4 = bytes[index1 + index4];
                        charArray2[index2] = charArray1[(int) num4 >> 4 & 15];
                        charArray2[index2 + 1] = charArray1[(int) num4 & 15];
                        charArray2[index3] = num4 < (byte) 32 ? '·' : (char) num4;
                    }

                    index2 += 3;
                    ++index3;
                }

                stringBuilder.Append(charArray2);
            }

            return stringBuilder.ToString();
        }

        private string[] GetDisassembly(ulong address, byte[] data)
        {
            List<string> stringList = new List<string>();
            ArchitectureMode architecture = ArchitectureMode.x86_64;
            Disassembler.Translator.IncludeAddress = true;
            Disassembler.Translator.IncludeBinary = true;
            foreach (Instruction instruction in new Disassembler(data, architecture, address, true, Vendor.Any, 0UL)
                .Disassemble())
                stringList.Add(instruction.ToString());
            return stringList.ToArray();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Yes;
            if (this.ps4 != null && this.ps4.IsConnected)
                dialogResult = MessageBox.Show("You are already connected...\nWould you like to reconnect?",
                    "Debug Watch", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
            if (dialogResult != DialogResult.Yes)
                return;
            this.ps4 = new PS4DBG(this.IpTextBox.Text);
            this.ps4.Connect();
            this.ps4.Notify(210, "debug watch");
            this.AttachButton.Enabled = true;
            this.RefreshButton.Enabled = true;
            this.RebootButton.Enabled = true;
            this.MainPanel.Enabled = false;
            Settings.ip = this.IpTextBox.Text;
            Settings.filter = this.FilterProcessListCheckBox.Checked;
            this.RefreshButton_Click((object) null, (EventArgs) null);
        }

        private void AttachButton_Click(object sender, EventArgs e)
        {
            if (this.ProcessComboBox.Text.Contains(":"))
            {
                string[] strArray = this.ProcessComboBox.Text.Split(':');
                int int32 = Convert.ToInt32(strArray[0], 10);
                this.ps4.AttachDebugger(int32, new PS4DBG.DebuggerInterruptCallback(this.DebuggerInterruptCallback));
                this.attachpid = int32;
                this.mapview = new MemoryMapView(ps4.GetProcessInfo(attachpid), this.ps4.GetProcessMaps(attachpid));
                this.ps4.Notify(222, "attached to " + strArray[1]);
                this.MainPanel.Enabled = true;
                this.AttachButton.Enabled = false;
                this.DetachButton.Enabled = true;
            }
            else
            {
                int num = (int) MessageBox.Show("Please select a process in the list! Or press refresh then select!",
                    "Debug Watch", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void DetachButton_Click(object sender, EventArgs e)
        {
            if (!this.ps4.IsDebugging)
                return;
            this.ps4.DetachDebugger();
            this.MainPanel.Enabled = false;
            this.AttachButton.Enabled = true;
            this.DetachButton.Enabled = false;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            string[] strArray = new string[2]
            {
                "eboot",
                "default"
            };
            this.proclist = this.ps4.GetProcessList();
            this.ProcessComboBox.Items.Clear();
            foreach (libdebug.Process process in this.proclist.processes)
            {
                if (this.FilterProcessListCheckBox.Checked)
                {
                    foreach (string str in strArray)
                    {
                        if (process.name.Contains(str))
                            this.ProcessComboBox.Items.Add((object) (process.pid.ToString() + ":" + process.name));
                    }
                }
                else
                    this.ProcessComboBox.Items.Add((object) (process.pid.ToString() + ":" + process.name));
            }

            if (this.ProcessComboBox.Items.Count <= 0)
                return;
            this.ProcessComboBox.SelectedIndex = 0;
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            this.ps4.ProcessResume();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            this.ps4.ProcessStop();
        }

        private void DebuggerInterruptCallback(
            uint lwpid,
            uint status,
            string tdname,
            regs regs,
            fpregs fpregs,
            dbregs dbregs)
        {
            var tabControlDelegate =
                // new Action<object, object>((_param1, _param2) => this.TabControl.SelectedIndex = 0);
                new Action(() => this.TabControl.SelectedIndex = 0);
            var registersTextBoxDelegate = new Action(() =>
                this.RegistersTextBox.Text = "r15 = 0x" + regs.r_r15.ToString("X") +
                                             ", r14 = 0x" + regs.r_r14.ToString("X") +
                                             ", r13 = 0x" + regs.r_r13.ToString("X") +
                                             ", r12 = 0x" + regs.r_r12.ToString("X") +
                                             ", r11 = 0x" + regs.r_r11.ToString("X") +
                                             ", r10 = 0x" + regs.r_r10.ToString("X") +
                                             ", r9 = 0x" + regs.r_r9.ToString("X") +
                                             ", r8 = 0x" + regs.r_r8.ToString("X") +
                                             ", rdi = 0x" + regs.r_rdi.ToString("X") +
                                             ", rsi = 0x" + regs.r_rsi.ToString("X") +
                                             ", rbp = 0x" + regs.r_rbp.ToString("X") +
                                             ", rbx = 0x" + regs.r_rbx.ToString("X") +
                                             ", rdx = 0x" + regs.r_rdx.ToString("X") +
                                             ", rcx = 0x" + regs.r_rcx.ToString("X") +
                                             ", rax = 0x" + regs.r_rax.ToString("X") +
                                             ", trapno = 0x" + regs.r_trapno.ToString("X") +
                                             ", fs = 0x" + regs.r_fs.ToString("X") +
                                             ", gs = 0x" + regs.r_gs.ToString("X") +
                                             ", err = 0x" + regs.r_err.ToString("X") +
                                             ", es = 0x" + regs.r_es.ToString("X") +
                                             ", ds = 0x" + regs.r_ds.ToString("X") +
                                             ", rip = 0x" + regs.r_rip.ToString("X") +
                                             ", cs = 0x" + regs.r_cs.ToString("X") +
                                             ", rflags = 0x" + regs.r_rflags.ToString("X") +
                                             ", rsp = 0x" + regs.r_rsp.ToString("X") +
                                             ", ss = 0x" + regs.r_ss.ToString("X"));
            var addressTextBoxDelegate = new Action(() =>
                this.AddressTextBox.Text = "0x" + regs.r_rip.ToString("X"));
            var tryFindButtonDelegate = new Action(() =>
                this.TryFindButton_Click((object) null, (EventArgs) null));

            this.ps4.Notify(222, "interrupt hit\n(thread: " + tdname + " id: " + (object) lwpid + ")");
            this.TabControl.Invoke(tabControlDelegate);
            this.RegistersTextBox.Invoke(registersTextBoxDelegate);
            this.AddressTextBox.Invoke(addressTextBoxDelegate);
            this.TryFindButton.Invoke(tryFindButtonDelegate);
            this.data = this.ps4.ReadMemory(this.attachpid, this.address, this.length);
            string[] lines = this.GetDisassembly(this.address, this.data);
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 1; index < lines.Length; ++index)
                stringBuilder.AppendLine(lines[index]);
            string after = stringBuilder.ToString();
            var disassemblyTextBoxDelegate = new Action(() =>
            {
                this.DisassemblyTextBox.Clear();
                this.DisassemblyTextBox.AppendText(lines[0] + Environment.NewLine, Color.Salmon);
                this.DisassemblyTextBox.AppendText(after);
            });
            this.DisassemblyTextBox.Invoke(disassemblyTextBoxDelegate);
        }

        private void ClearBreakpoints_Click(object sender, EventArgs e)
        {
            for (int index = 0; (long) index < (long) PS4DBG.MAX_BREAKPOINTS; ++index)
                this.ps4.ChangeBreakpoint(index, false, 0UL);
        }

        private void ClearWatchPoints_Click(object sender, EventArgs e)
        {
            for (int index = 0; (long) index < (long) PS4DBG.MAX_WATCHPOINTS; ++index)
                this.ps4.ChangeWatchpoint(index, false, PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_1,
                    PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_EXEC, 0UL);
        }

        private void SetWatchpointButton_Click(object sender, EventArgs e)
        {
            PS4DBG.WATCHPT_LENGTH watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_1;
            string text1 = this.WatchpointLengthComboBox.Text;
            if (!(text1 == "1 byte"))
            {
                if (!(text1 == "2 bytes"))
                {
                    if (!(text1 == "4 bytes"))
                    {
                        if (text1 == "8 bytes")
                            watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_8;
                    }
                    else
                        watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_4;
                }
                else
                    watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_2;
            }
            else
                watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_1;

            PS4DBG.WATCHPT_BREAKTYPE watchptBreaktype = PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_EXEC;
            string text2 = this.BreaktypeComboBox.Text;
            if (!(text2 == "execute"))
            {
                if (!(text2 == "write"))
                {
                    if (text2 == "read/write")
                        watchptBreaktype = PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_RDWR;
                }
                else
                    watchptBreaktype = PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_WRONLY;
            }
            else
                watchptBreaktype = PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_EXEC;

            this.ps4.ChangeWatchpoint((int) this.WatchpointNumericUpDown.Value, true, watchptLength, watchptBreaktype,
                this.address);
        }

        private void ClearWatchpointButton_Click(object sender, EventArgs e)
        {
            this.ps4.ChangeWatchpoint((int) this.WatchpointNumericUpDown.Value, false,
                PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_1, PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_EXEC, 0UL);
        }

        private void SetBreakpointButton_Click(object sender, EventArgs e)
        {
            this.ps4.ChangeBreakpoint((int) this.BreakpointNumericUpDown.Value, true, this.address);
        }

        private void ClearBreakpoint_Click(object sender, EventArgs e)
        {
            this.ps4.ChangeBreakpoint((int) this.BreakpointNumericUpDown.Value, false, 0UL);
        }

        private void RebootButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to reboot?", "Debug Watch", MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation) != DialogResult.Yes)
                return;
            this.ps4.Reboot();
            this.RebootButton.Enabled = false;
            this.MainPanel.Enabled = false;
            this.AttachButton.Enabled = true;
            this.DetachButton.Enabled = false;
        }

        private void PeekButton_Click(object sender, EventArgs e)
        {
            this.data = this.ps4.ReadMemory(this.attachpid, this.address, this.length);
            this.DisassemblyTextBox.Clear();
            this.DisassemblyTextBox.Lines = this.GetDisassembly(this.address, this.data);
            this.MemoryHexBox.ByteProvider = (IByteProvider) new MemoryViewByteProvider(this.data);
        }

        private void PokeButton_Click(object sender, EventArgs e)
        {
            this.ps4.WriteMemory(this.attachpid, this.address,
                ((MemoryViewByteProvider) this.MemoryHexBox.ByteProvider).Bytes.ToArray());
        }

        private void TryFindButton_Click(object sender, EventArgs e)
        {
            ArchitectureMode architecture = ArchitectureMode.x86_64;
            Disassembler.Translator.IncludeAddress = true;
            Disassembler.Translator.IncludeBinary = true;
            ulong address = this.address - 50UL;
            Instruction[] array =
                new Disassembler(this.ps4.ReadMemory(this.attachpid, address, 100), architecture, address, true,
                    Vendor.Any, 0UL).Disassemble().ToArray<Instruction>();
            for (int index = 0; index < array.Length; ++index)
            {
                if ((long) array[index].PC == (long) this.address)
                {
                    this.AddressTextBox.Text = "0x" + array[index - 1].PC.ToString("X");
                    this.PeekButton_Click((object) null, (EventArgs) null);
                    break;
                }
            }
        }

        private void AutoPatchButton_Click(object sender, EventArgs e)
        {
            ArchitectureMode architecture = ArchitectureMode.x86_64;
            Disassembler.Translator.IncludeAddress = true;
            Disassembler.Translator.IncludeBinary = true;
            ulong uint64 = Convert.ToUInt64(this.AddressTextBox.Text.Trim().Replace("0x", ""), 16);
            Instruction[] array =
                new Disassembler(this.ps4.ReadMemory(this.attachpid, uint64, 50), architecture, uint64, true,
                    Vendor.Any, 0UL).Disassemble().ToArray<Instruction>();
            for (int index = 0; index < array[0].Length; ++index)
                this.ps4.WriteMemory(this.attachpid, array[0].Offset + (ulong) index, (byte) 144);
            this.PeekButton_Click((object) null, (EventArgs) null);
        }

        private void MemoryMapButton_Click(object sender, EventArgs e)
        {
            this.mapview = new MemoryMapView(this.ps4.GetProcessInfo(this.attachpid), ps4.GetProcessMaps(attachpid));
            int num = (int) this.mapview.ShowDialog();
        }

        private void KillProcessButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to kill the process?\nThis may break your game until a reboot.",
                "Debug Watch", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                return;
            this.ps4.ProcessKill();
            this.MainPanel.Enabled = false;
            this.AttachButton.Enabled = true;
            this.DetachButton.Enabled = false;
            this.RefreshButton_Click((object) null, (EventArgs) null);
        }

        private void NewScanButton_Click(object sender, EventArgs e)
        {
            this.TabControl.SelectedIndex = 2;
            this.ScanHistoryListBox.Items.Clear();
            this.ScanHistoryListBox.Items.Add((object) (this.ValueTextBox.Text + " " + this.ScanTypeComboBox.Text.ToLower()));
            this.ScanDataGridView.Rows.Clear();
            MemoryScanner.SCAN_TYPE type = MemoryScanner.StringToType(this.ScanTypeComboBox.Text);
            string str = MemoryScanner.TypeToString(type);
            byte[] numArray = (byte[]) null;

            if (isSearchValueInvalid(type)) return;

            Task.Factory.StartNew(() => searchMemeoryForValue(type, numArray, str));
        }

        private void searchMemeoryForValue(MemoryScanner.SCAN_TYPE type, byte[] numArray, string str)
        {
            this.disableInterfaceWhileSearching();

            switch (type)
            {
                case MemoryScanner.SCAN_TYPE.BYTE:
                    numArray = new byte[1]
                    {
                        Convert.ToByte(this.ValueTextBox.Text)
                    };
                    break;
                case MemoryScanner.SCAN_TYPE.SHORT:
                    numArray = BitConverter.GetBytes(Convert.ToUInt16(this.ValueTextBox.Text));
                    break;
                case MemoryScanner.SCAN_TYPE.INTEGER:
                    numArray = BitConverter.GetBytes(Convert.ToUInt32(this.ValueTextBox.Text));
                    break;
                case MemoryScanner.SCAN_TYPE.LONG:
                    numArray = BitConverter.GetBytes(Convert.ToUInt64(this.ValueTextBox.Text));
                    break;
                case MemoryScanner.SCAN_TYPE.FLOAT:
                    numArray = BitConverter.GetBytes(Convert.ToSingle(this.ValueTextBox.Text));
                    break;
                case MemoryScanner.SCAN_TYPE.DOUBLE:
                    numArray = BitConverter.GetBytes(Convert.ToDouble(this.ValueTextBox.Text));
                    break;
            }

            var memoryEntriesToSearchThrough = this.mapview.GetSelectedEntries();
            this.ScanProgressBar.Invoke((p) => p.Minimum = 0);
            this.ScanProgressBar.Invoke((p) => p.Maximum = memoryEntriesToSearchThrough.Length);
            this.ScanProgressBar.Invoke((p) => p.Value = 0);
            foreach (MemoryEntry selectedEntry in memoryEntriesToSearchThrough)
            {
                byte[] data = this.ps4.ReadMemory(this.attachpid,
                    selectedEntry.start,
                    (int) ((long) selectedEntry.end - (long) selectedEntry.start));
                if (data != null && data.Length != 0)
                {
                    // Get the numeral value of the memory segment and returns it as a string.
                    string ConvertMemorySegmentToValue(byte[] memorySegment, MemoryScanner.SCAN_TYPE scanType)
                    {
                        switch (scanType)
                        {
                            case MemoryScanner.SCAN_TYPE.BYTE:
                                return memorySegment[0].ToString();
                            case MemoryScanner.SCAN_TYPE.SHORT:
                                return BitConverter.ToInt16(memorySegment, 0).ToString();
                            case MemoryScanner.SCAN_TYPE.INTEGER:
                                return BitConverter.ToInt32(memorySegment, 0).ToString();
                            case MemoryScanner.SCAN_TYPE.LONG:
                                return BitConverter.ToInt64(memorySegment, 0).ToString();
                            case MemoryScanner.SCAN_TYPE.FLOAT:
                                return BitConverter.ToSingle(memorySegment, 0).ToString();
                            case MemoryScanner.SCAN_TYPE.DOUBLE:
                                return BitConverter.ToDouble(memorySegment, 0).ToString();
                            default:
                                throw new ArgumentOutOfRangeException(nameof(scanType), scanType, null);
                        }
                    }

                    foreach (KeyValuePair<ulong, byte[]> keyValuePair in MemoryScanner.ScanMemory(selectedEntry.start, data, type, numArray, new MemoryScanner.CompareFunction(MemoryScanner.CompareEqual)))
                    {
                        this.ScanDataGridView.Invoke((gridview )=> gridview.Rows.Add((object) ("0x" + keyValuePair.Key.ToString("X")), (object) str,
                            ConvertMemorySegmentToValue(keyValuePair.Value, type)));
                    }

                    GC.Collect();
                }
            }

            this.reEnableInterfaceAfterDoneSearching();
            this.ScanProgressBar.Invoke((p) => p.Value = 0);
            this.NextScanButton.Invoke((b) => b.Enabled = true);
        }

        private void disableInterfaceWhileSearching()
        {
            // Keep Next Scan button the same
            var cachedNextScan = NextScanButton;

            var allFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var buttonFieldInfos = allFields.Where(field => field.FieldType == typeof(Button));
            foreach (var buttonFieldInfo in buttonFieldInfos)
            {
                var btn = (buttonFieldInfo.GetValue(this) as Button);
                btn.Invoke((b) => { b.Enabled = false; });
            }

            NextScanButton = cachedNextScan;
        }

        private void reEnableInterfaceAfterDoneSearching()
        {
            // Keep Next Scan button the same
            var cachedNextScan = NextScanButton.Enabled;

            var allFields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var buttonFieldInfos = allFields.Where(field => field.FieldType == typeof(Button));
            foreach (var buttonFieldInfo in buttonFieldInfos)
            {
                var btn = (buttonFieldInfo.GetValue(this) as Button);
                btn.Invoke((b) => b.Enabled = true);
            }

            NextScanButton.Invoke(b => b.Enabled = cachedNextScan);
        }

        private bool isSearchValueInvalid(MemoryScanner.SCAN_TYPE type)
        {
            // Validate the text entered before we go any further
            // First, check the integer isn't too small for the type and make note of what type we're searching for
            UInt64 maxValueInt = 0;
            double maxValueDouble = 0.0;
            bool isIntType = true;
            switch (type)
            {
                case MemoryScanner.SCAN_TYPE.BYTE:
                    maxValueInt = Byte.MaxValue;
                    break;
                case MemoryScanner.SCAN_TYPE.SHORT:
                    maxValueInt = UInt16.MaxValue;
                    break;
                case MemoryScanner.SCAN_TYPE.INTEGER:
                    maxValueInt = UInt32.MaxValue;
                    break;
                case MemoryScanner.SCAN_TYPE.LONG:
                    maxValueInt = UInt64.MaxValue;
                    break;
                case MemoryScanner.SCAN_TYPE.FLOAT:
                    isIntType = false;
                    maxValueDouble = float.MaxValue;
                    break;
                case MemoryScanner.SCAN_TYPE.DOUBLE:
                    isIntType = false;
                    maxValueDouble = double.MaxValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Check if we can parse the value
            ulong searchValueAsInt = 0;
            var searchValueAsDouble = 0.0;
            if ((isIntType && !UInt64.TryParse(this.ValueTextBox.Text, out searchValueAsInt)) ||
                (!isIntType && !double.TryParse(this.ValueTextBox.Text, out searchValueAsDouble)))
            {
                MessageBox.Show("Value entered could not be parsed.", "Parse Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return true;
            }

            // Check the value isn't too big
            if (isIntType ? maxValueInt < searchValueAsInt : maxValueDouble < searchValueAsDouble)
            {
                MessageBox.Show("Value entered was too large for the specified type.", "Value Too Large", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return true;
            }

            return false;
        }

        private void NextScanButton_Click(object sender, EventArgs e)
        {
            this.TabControl.SelectedIndex = 2;
            this.ScanHistoryListBox.Items.Add(
                (object) (this.ValueTextBox.Text + " " + this.ScanTypeComboBox.Text.ToLower()));
            disableInterfaceWhileSearching();
            var updateValuesTask = Task.Factory.StartNew(() => recheckSavedValues());
        }

        private void recheckSavedValues()
        {
            List<string[]> strArrayList = new List<string[]>();
            this.ScanProgressBar.Invoke(s => s.Maximum = this.ScanDataGridView.Rows.Count);
            this.ScanProgressBar.Invoke(s => s.Value = 0);

            foreach (DataGridViewRow row in (IEnumerable) this.ScanDataGridView.Rows)
            {
                ulong uint64 = Convert.ToUInt64(row.Cells[0].Value.ToString().Replace("0x", ""), 16);
                MemoryScanner.SCAN_TYPE type = MemoryScanner.StringToType(row.Cells[1].Value.ToString());
                bool flag = false;
                switch (type)
                {
                    case MemoryScanner.SCAN_TYPE.BYTE:
                        if ((int) this.ps4.ReadMemory(this.attachpid, uint64, 1)[0] ==
                            (int) Convert.ToByte(this.ValueTextBox.Text))
                        {
                            flag = true;
                            break;
                        }

                        break;
                    case MemoryScanner.SCAN_TYPE.SHORT:
                        if ((int) BitConverter.ToInt16(this.ps4.ReadMemory(this.attachpid, uint64, 2), 0) ==
                            (int) Convert.ToUInt16(this.ValueTextBox.Text))
                        {
                            flag = true;
                            break;
                        }

                        break;
                    case MemoryScanner.SCAN_TYPE.INTEGER:
                        if ((int) BitConverter.ToInt32(this.ps4.ReadMemory(this.attachpid, uint64, 4), 0) ==
                            (int) Convert.ToUInt32(this.ValueTextBox.Text))
                        {
                            flag = true;
                            break;
                        }

                        break;
                    case MemoryScanner.SCAN_TYPE.LONG:
                        if ((long) BitConverter.ToInt64(this.ps4.ReadMemory(this.attachpid, uint64, 8), 0) ==
                            (long) Convert.ToUInt64(this.ValueTextBox.Text))
                        {
                            flag = true;
                            break;
                        }

                        break;
                    case MemoryScanner.SCAN_TYPE.FLOAT:
                        if (BitConverter.ToSingle(ps4.ReadMemory(this.attachpid, uint64, 4), 0) ==
                            (double) Convert.ToSingle(this.ValueTextBox.Text))
                        {
                            flag = true;
                            break;
                        }

                        break;
                    case MemoryScanner.SCAN_TYPE.DOUBLE:
                        if (BitConverter.ToDouble(this.ps4.ReadMemory(this.attachpid, uint64, 8), 0) ==
                            Convert.ToDouble(this.ValueTextBox.Text))
                        {
                            flag = true;
                            break;
                        }

                        break;
                        
                }

                if (flag)
                {
                    string[] strArray = new string[3]
                    {
                        row.Cells[0].Value.ToString(),
                        row.Cells[1].Value.ToString(),
                        this.ValueTextBox.Text
                    };
                    strArrayList.Add(strArray);
                }

                this.ScanProgressBar.Invoke(s => s.Increment(1));
            }

            this.ScanDataGridView.Invoke(s => s.Rows.Clear());
            foreach (string[] strArray in strArrayList)
            {
                this.ScanDataGridView.Invoke(s => s.Rows.Add((object)strArray[0], (object)strArray[1], (object)strArray[2]));
            }
            this.ScanProgressBar.Invoke(s => s.Value = 0);
            reEnableInterfaceAfterDoneSearching();
        }

        private void FilterProcessListCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.filter = this.FilterProcessListCheckBox.Checked;
        }

        private void ScanDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView) sender;
            if (!(dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn) || e.RowIndex < 0)
                return;
            ulong uint64 = Convert.ToUInt64(dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString().Trim().Replace("0x", ""),
                16);
            ulong typeLength =
                (ulong) MemoryScanner.GetTypeLength(dataGridView.Rows[e.RowIndex].Cells[1].ToString().Trim());
            PS4DBG.WATCHPT_LENGTH watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_1;
            switch ((long) typeLength - 1L)
            {
                case 0:
                    watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_1;
                    goto case 2;
                case 1:
                    watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_2;
                    goto case 2;
                case 2:
                    this.ps4.ChangeWatchpoint((int) this.WatchpointNumericUpDown.Value, true, watchptLength,
                        PS4DBG.WATCHPT_BREAKTYPE.DBREG_DR7_WRONLY, uint64);
                    break;
                case 3:
                    watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_4;
                    goto case 2;
                default:
                    if (typeLength == 8UL)
                    {
                        watchptLength = PS4DBG.WATCHPT_LENGTH.DBREG_DR7_LEN_8;
                        goto case 2;
                    }
                    else
                        goto case 2;
            }
        }

        private void SupportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/WXgmWFh");
        }

        private void AssembleButton_Click(object sender, EventArgs e)
        {
            ulong uint64 = Convert.ToUInt64(this.AddressTextBox.Text.Trim().Replace("0x", ""), 16);
            AssemblerView assemblerView = new AssemblerView(uint64);
            int num = (int) assemblerView.ShowDialog();
            byte[] assemblerResult = assemblerView.GetAssemblerResult();
            if (assemblerResult != null)
                this.ps4.WriteMemory(this.attachpid, uint64, assemblerResult);
            this.PeekButton_Click((object) null, (EventArgs) null);
        }

        private void CreditsButton_Click(object sender, EventArgs e)
        {
            int num = (int) MessageBox.Show(
                "This is really just a simple simple tool, you can do much much more with ps4debug if you want.\n\nDebug Watch, ps4debug, and jkpatch all created by golden\n\nShout out to all my testers, especially PS4 Guru, Shiningami, and Weysincha! Hit the link, to the left, and join the discord!",
                "Debug Watch", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DebugWatchForm));
            this.ConnectButton = new Button();
            this.AttachButton = new Button();
            this.IpTextBox = new TextBox();
            this.ProcessComboBox = new ComboBox();
            this.RefreshButton = new Button();
            this.AssembleButton = new Button();
            this.PokeButton = new Button();
            this.PeekButton = new Button();
            this.MemoryHexBox = new HexBox();
            this.ScanDataGridView = new DataGridView();
            this.AddressColumn = new DataGridViewTextBoxColumn();
            this.TypeColumn = new DataGridViewTextBoxColumn();
            this.ValueColumn = new DataGridViewTextBoxColumn();
            this.WatchpointColumn = new DataGridViewButtonColumn();
            this.EditGroupBox = new GroupBox();
            this.KillProcessButton = new Button();
            this.LengthLabel = new Label();
            this.MemoryScanGroupBox = new GroupBox();
            this.ScanProgressBar = new ProgressBar();
            this.ScanHistoryListBox = new ListBox();
            this.ScanHistoryLabel = new Label();
            this.HelpLabel = new Label();
            this.ScanTypeComboBox = new ComboBox();
            this.ValueLabel = new Label();
            this.ValueTextBox = new TextBox();
            this.NextScanButton = new Button();
            this.NewScanButton = new Button();
            this.MemoryMapButton = new Button();
            this.LengthTextBox = new TextBox();
            this.AddressLabel = new Label();
            this.AddressTextBox = new TextBox();
            this.BreakpointGroupBox = new GroupBox();
            this.BreakpointIndexLabel = new Label();
            this.ClearBreakpointButton = new Button();
            this.BreakpointNumericUpDown = new NumericUpDown();
            this.SetBreakpointButton = new Button();
            this.WatchpointGroupBox = new GroupBox();
            this.WatchpointTypeLabel = new Label();
            this.WatchpointLengthLabel = new Label();
            this.WatchpointIndexLabel = new Label();
            this.ClearWatchpointButton = new Button();
            this.WatchpointNumericUpDown = new NumericUpDown();
            this.WatchpointLengthComboBox = new ComboBox();
            this.BreaktypeComboBox = new ComboBox();
            this.SetWatchpointButton = new Button();
            this.StopButton = new Button();
            this.GoButton = new Button();
            this.ControlAndRegistersGroupBox = new GroupBox();
            this.AutoPatchButton = new Button();
            this.TryFindButton = new Button();
            this.ClearBreakpoints = new Button();
            this.ClearWatchPoints = new Button();
            this.RegistersTextBox = new TextBox();
            this.RebootButton = new Button();
            this.DetachButton = new Button();
            this.FilterProcessListCheckBox = new CheckBox();
            this.MainPanel = new Panel();
            this.TabControl = new TabControl();
            this.DisassemblyTabPage = new TabPage();
            this.DisassemblyTextBox = new RichTextBox();
            this.MemoryTabPage = new TabPage();
            this.ScanTabPage = new TabPage();
            this.SupportLinkLabel = new LinkLabel();
            this.CreditsButton = new Button();
            ((ISupportInitialize) this.ScanDataGridView).BeginInit();
            this.EditGroupBox.SuspendLayout();
            this.MemoryScanGroupBox.SuspendLayout();
            this.BreakpointGroupBox.SuspendLayout();
            this.BreakpointNumericUpDown.BeginInit();
            this.WatchpointGroupBox.SuspendLayout();
            this.WatchpointNumericUpDown.BeginInit();
            this.ControlAndRegistersGroupBox.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.DisassemblyTabPage.SuspendLayout();
            this.MemoryTabPage.SuspendLayout();
            this.ScanTabPage.SuspendLayout();
            this.SuspendLayout();
            this.ConnectButton.Location = new Point(12, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new Size(75, 23);
            this.ConnectButton.TabIndex = 0;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new EventHandler(this.ConnectButton_Click);
            this.AttachButton.Enabled = false;
            this.AttachButton.Location = new Point(205, 13);
            this.AttachButton.Name = "AttachButton";
            this.AttachButton.Size = new Size(75, 23);
            this.AttachButton.TabIndex = 1;
            this.AttachButton.Text = "Attach";
            this.AttachButton.UseVisualStyleBackColor = true;
            this.AttachButton.Click += new EventHandler(this.AttachButton_Click);
            this.IpTextBox.Location = new Point(93, 14);
            this.IpTextBox.Name = "IpTextBox";
            this.IpTextBox.Size = new Size(106, 20);
            this.IpTextBox.TabIndex = 2;
            this.IpTextBox.Text = "192.168.1.107";
            this.ProcessComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.ProcessComboBox.FormattingEnabled = true;
            this.ProcessComboBox.Location = new Point(367, 14);
            this.ProcessComboBox.Name = "ProcessComboBox";
            this.ProcessComboBox.Size = new Size(150, 21);
            this.ProcessComboBox.TabIndex = 3;
            this.RefreshButton.Enabled = false;
            this.RefreshButton.Location = new Point(523, 13);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new Size(28, 23);
            this.RefreshButton.TabIndex = 4;
            this.RefreshButton.Text = "R";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new EventHandler(this.RefreshButton_Click);
            this.AssembleButton.Location = new Point((int) sbyte.MaxValue, 93);
            this.AssembleButton.Name = "AssembleButton";
            this.AssembleButton.Size = new Size(82, 23);
            this.AssembleButton.TabIndex = 5;
            this.AssembleButton.Text = "Assemble";
            this.AssembleButton.UseVisualStyleBackColor = true;
            this.AssembleButton.Click += new EventHandler(this.AssembleButton_Click);
            this.PokeButton.Location = new Point(71, 93);
            this.PokeButton.Name = "PokeButton";
            this.PokeButton.Size = new Size(50, 23);
            this.PokeButton.TabIndex = 3;
            this.PokeButton.Text = "Poke";
            this.PokeButton.UseVisualStyleBackColor = true;
            this.PokeButton.Click += new EventHandler(this.PokeButton_Click);
            this.PeekButton.Location = new Point(12, 93);
            this.PeekButton.Name = "PeekButton";
            this.PeekButton.Size = new Size(50, 23);
            this.PeekButton.TabIndex = 2;
            this.PeekButton.Text = "Peek";
            this.PeekButton.UseVisualStyleBackColor = true;
            this.PeekButton.Click += new EventHandler(this.PeekButton_Click);
            this.MemoryHexBox.ColumnInfoVisible = true;
            this.MemoryHexBox.Dock = DockStyle.Fill;
            this.MemoryHexBox.Font = new Font("Consolas", 11f);
            this.MemoryHexBox.LineInfoVisible = true;
            this.MemoryHexBox.Location = new Point(3, 3);
            this.MemoryHexBox.Name = "MemoryHexBox";
            this.MemoryHexBox.ShadowSelectionColor = Color.FromArgb(100, 60, 188, (int) byte.MaxValue);
            this.MemoryHexBox.Size = new Size(738, 436);
            this.MemoryHexBox.StringViewVisible = true;
            this.MemoryHexBox.TabIndex = 1;
            this.MemoryHexBox.UseFixedBytesPerLine = true;
            this.MemoryHexBox.VScrollBarVisible = true;
            this.ScanDataGridView.AllowUserToAddRows = false;
            this.ScanDataGridView.AllowUserToDeleteRows = false;
            this.ScanDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ScanDataGridView.Columns.AddRange((DataGridViewColumn) this.AddressColumn,
                (DataGridViewColumn) this.TypeColumn, (DataGridViewColumn) this.ValueColumn,
                (DataGridViewColumn) this.WatchpointColumn);
            this.ScanDataGridView.Dock = DockStyle.Fill;
            this.ScanDataGridView.Location = new Point(3, 3);
            this.ScanDataGridView.Name = "ScanDataGridView";
            this.ScanDataGridView.ReadOnly = true;
            this.ScanDataGridView.Size = new Size(738, 436);
            this.ScanDataGridView.TabIndex = 4;
            this.ScanDataGridView.CellContentClick +=
                new DataGridViewCellEventHandler(this.ScanDataGridView_CellContentClick);
            this.AddressColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.AddressColumn.FillWeight = 160f;
            this.AddressColumn.HeaderText = "Address";
            this.AddressColumn.Name = "AddressColumn";
            this.AddressColumn.ReadOnly = true;
            this.AddressColumn.Resizable = DataGridViewTriState.False;
            this.TypeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.TypeColumn.FillWeight = 80f;
            this.TypeColumn.HeaderText = "Type";
            this.TypeColumn.Name = "TypeColumn";
            this.TypeColumn.ReadOnly = true;
            this.TypeColumn.Resizable = DataGridViewTriState.False;
            this.ValueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.ValueColumn.FillWeight = 140f;
            this.ValueColumn.HeaderText = "Value";
            this.ValueColumn.Name = "ValueColumn";
            this.ValueColumn.ReadOnly = true;
            this.ValueColumn.Resizable = DataGridViewTriState.False;
            this.WatchpointColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.WatchpointColumn.FillWeight = 60f;
            this.WatchpointColumn.HeaderText = "Watchpoint";
            this.WatchpointColumn.Name = "WatchpointColumn";
            this.WatchpointColumn.ReadOnly = true;
            this.WatchpointColumn.Resizable = DataGridViewTriState.False;
            this.WatchpointColumn.Text = "X";
            this.EditGroupBox.Controls.Add((Control) this.PeekButton);
            this.EditGroupBox.Controls.Add((Control) this.PokeButton);
            this.EditGroupBox.Controls.Add((Control) this.AssembleButton);
            this.EditGroupBox.Controls.Add((Control) this.KillProcessButton);
            this.EditGroupBox.Controls.Add((Control) this.LengthLabel);
            this.EditGroupBox.Controls.Add((Control) this.MemoryScanGroupBox);
            this.EditGroupBox.Controls.Add((Control) this.MemoryMapButton);
            this.EditGroupBox.Controls.Add((Control) this.LengthTextBox);
            this.EditGroupBox.Controls.Add((Control) this.AddressLabel);
            this.EditGroupBox.Controls.Add((Control) this.AddressTextBox);
            this.EditGroupBox.Controls.Add((Control) this.BreakpointGroupBox);
            this.EditGroupBox.Controls.Add((Control) this.WatchpointGroupBox);
            this.EditGroupBox.Controls.Add((Control) this.StopButton);
            this.EditGroupBox.Controls.Add((Control) this.GoButton);
            this.EditGroupBox.Location = new Point(761, 3);
            this.EditGroupBox.Name = "EditGroupBox";
            this.EditGroupBox.Size = new Size(219, 651);
            this.EditGroupBox.TabIndex = 2;
            this.EditGroupBox.TabStop = false;
            this.EditGroupBox.Text = "Edit Process";
            this.KillProcessButton.BackColor = Color.FromArgb(192, 0, 0);
            this.KillProcessButton.Font =
                new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.KillProcessButton.ForeColor = Color.Black;
            this.KillProcessButton.Location = new Point(71, 19);
            this.KillProcessButton.Name = "KillProcessButton";
            this.KillProcessButton.Size = new Size(25, 25);
            this.KillProcessButton.TabIndex = 19;
            this.KillProcessButton.Text = "■";
            this.KillProcessButton.UseVisualStyleBackColor = false;
            this.KillProcessButton.Click += new EventHandler(this.KillProcessButton_Click);
            this.LengthLabel.AutoSize = true;
            this.LengthLabel.Font = new Font("Consolas", 8.25f);
            this.LengthLabel.Location = new Point(122, 51);
            this.LengthLabel.Name = "LengthLabel";
            this.LengthLabel.Size = new Size(97, 13);
            this.LengthLabel.TabIndex = 18;
            this.LengthLabel.Text = "Length (in hex)";
            this.MemoryScanGroupBox.Controls.Add((Control) this.ScanProgressBar);
            this.MemoryScanGroupBox.Controls.Add((Control) this.ScanHistoryListBox);
            this.MemoryScanGroupBox.Controls.Add((Control) this.ScanHistoryLabel);
            this.MemoryScanGroupBox.Controls.Add((Control) this.HelpLabel);
            this.MemoryScanGroupBox.Controls.Add((Control) this.ScanTypeComboBox);
            this.MemoryScanGroupBox.Controls.Add((Control) this.ValueLabel);
            this.MemoryScanGroupBox.Controls.Add((Control) this.ValueTextBox);
            this.MemoryScanGroupBox.Controls.Add((Control) this.NextScanButton);
            this.MemoryScanGroupBox.Controls.Add((Control) this.NewScanButton);
            this.MemoryScanGroupBox.Location = new Point(9, 338);
            this.MemoryScanGroupBox.Name = "MemoryScanGroupBox";
            this.MemoryScanGroupBox.Size = new Size(204, 307);
            this.MemoryScanGroupBox.TabIndex = 17;
            this.MemoryScanGroupBox.TabStop = false;
            this.MemoryScanGroupBox.Text = "Memory Scan";
            this.ScanProgressBar.Location = new Point(6, 283);
            this.ScanProgressBar.Name = "ScanProgressBar";
            this.ScanProgressBar.Size = new Size(192, 17);
            this.ScanProgressBar.TabIndex = 25;
            this.ScanHistoryListBox.Font = new Font("Consolas", 9.2f);
            this.ScanHistoryListBox.FormattingEnabled = true;
            this.ScanHistoryListBox.ItemHeight = 14;
            this.ScanHistoryListBox.Location = new Point(6, 147);
            this.ScanHistoryListBox.Name = "ScanHistoryListBox";
            this.ScanHistoryListBox.Size = new Size(192, 130);
            this.ScanHistoryListBox.TabIndex = 24;
            this.ScanHistoryLabel.AutoSize = true;
            this.ScanHistoryLabel.Font = new Font("Consolas", 9f);
            this.ScanHistoryLabel.Location = new Point(7, 130);
            this.ScanHistoryLabel.Name = "ScanHistoryLabel";
            this.ScanHistoryLabel.Size = new Size(91, 14);
            this.ScanHistoryLabel.TabIndex = 23;
            this.ScanHistoryLabel.Text = "Scan History";
            this.HelpLabel.AutoSize = true;
            this.HelpLabel.Font = new Font("Consolas", 8.25f);
            this.HelpLabel.Location = new Point(14, 97);
            this.HelpLabel.Name = "HelpLabel";
            this.HelpLabel.Size = new Size(175, 26);
            this.HelpLabel.TabIndex = 22;
            this.HelpLabel.Text = "Please select memory regions\r\nfrom memory map view above!";
            this.ScanTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.ScanTypeComboBox.FormattingEnabled = true;
            this.ScanTypeComboBox.Items.AddRange(new object[6]
            {
                (object) "byte",
                (object) "short",
                (object) "integer",
                (object) "long",
                (object) "float",
                (object) "double"
            });
            this.ScanTypeComboBox.Location = new Point(9, 61);
            this.ScanTypeComboBox.Name = "ScanTypeComboBox";
            this.ScanTypeComboBox.Size = new Size(108, 21);
            this.ScanTypeComboBox.TabIndex = 21;
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Font = new Font("Consolas", 8.25f);
            this.ValueLabel.Location = new Point(7, 16);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new Size(97, 13);
            this.ValueLabel.TabIndex = 20;
            this.ValueLabel.Text = "Value (not hex)";
            this.ValueTextBox.Location = new Point(9, 35);
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.Size = new Size(108, 20);
            this.ValueTextBox.TabIndex = 2;
            this.ValueTextBox.Text = "100";
            this.NextScanButton.Enabled = false;
            this.NextScanButton.Location = new Point(123, 60);
            this.NextScanButton.Name = "NextScanButton";
            this.NextScanButton.Size = new Size(75, 23);
            this.NextScanButton.TabIndex = 1;
            this.NextScanButton.Text = "Next Scan";
            this.NextScanButton.UseVisualStyleBackColor = true;
            this.NextScanButton.Click += new EventHandler(this.NextScanButton_Click);
            this.NewScanButton.Location = new Point(123, 33);
            this.NewScanButton.Name = "NewScanButton";
            this.NewScanButton.Size = new Size(75, 23);
            this.NewScanButton.TabIndex = 0;
            this.NewScanButton.Text = "New Scan";
            this.NewScanButton.UseVisualStyleBackColor = true;
            this.NewScanButton.Click += new EventHandler(this.NewScanButton_Click);
            this.MemoryMapButton.Location = new Point(102, 19);
            this.MemoryMapButton.Name = "MemoryMapButton";
            this.MemoryMapButton.Size = new Size(111, 23);
            this.MemoryMapButton.TabIndex = 7;
            this.MemoryMapButton.Text = "View Memory Map";
            this.MemoryMapButton.UseVisualStyleBackColor = true;
            this.MemoryMapButton.Click += new EventHandler(this.MemoryMapButton_Click);
            this.LengthTextBox.Location = new Point(148, 67);
            this.LengthTextBox.Name = "LengthTextBox";
            this.LengthTextBox.Size = new Size(61, 20);
            this.LengthTextBox.TabIndex = 16;
            this.LengthTextBox.Text = "0x100";
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Font = new Font("Consolas", 8.25f);
            this.AddressLabel.Location = new Point(9, 51);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new Size(103, 13);
            this.AddressLabel.TabIndex = 13;
            this.AddressLabel.Text = "Address (in hex)";
            this.AddressTextBox.Location = new Point(12, 67);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new Size(130, 20);
            this.AddressTextBox.TabIndex = 12;
            this.AddressTextBox.Text = "0x400000";
            this.BreakpointGroupBox.Controls.Add((Control) this.BreakpointIndexLabel);
            this.BreakpointGroupBox.Controls.Add((Control) this.ClearBreakpointButton);
            this.BreakpointGroupBox.Controls.Add((Control) this.BreakpointNumericUpDown);
            this.BreakpointGroupBox.Controls.Add((Control) this.SetBreakpointButton);
            this.BreakpointGroupBox.Location = new Point(12, 122);
            this.BreakpointGroupBox.Name = "BreakpointGroupBox";
            this.BreakpointGroupBox.Size = new Size(201, 75);
            this.BreakpointGroupBox.TabIndex = 11;
            this.BreakpointGroupBox.TabStop = false;
            this.BreakpointGroupBox.Text = "Breakpoint";
            this.BreakpointIndexLabel.AutoSize = true;
            this.BreakpointIndexLabel.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.BreakpointIndexLabel.Location = new Point(11, 21);
            this.BreakpointIndexLabel.Name = "BreakpointIndexLabel";
            this.BreakpointIndexLabel.Size = new Size(42, 14);
            this.BreakpointIndexLabel.TabIndex = 11;
            this.BreakpointIndexLabel.Text = "Index";
            this.ClearBreakpointButton.Location = new Point(136, 45);
            this.ClearBreakpointButton.Name = "ClearBreakpointButton";
            this.ClearBreakpointButton.Size = new Size(59, 23);
            this.ClearBreakpointButton.TabIndex = 10;
            this.ClearBreakpointButton.Text = "Clear";
            this.ClearBreakpointButton.UseVisualStyleBackColor = true;
            this.ClearBreakpointButton.Click += new EventHandler(this.ClearBreakpoint_Click);
            this.BreakpointNumericUpDown.Location = new Point(59, 19);
            this.BreakpointNumericUpDown.Maximum = new Decimal(new int[4]
            {
                9,
                0,
                0,
                0
            });
            this.BreakpointNumericUpDown.Name = "BreakpointNumericUpDown";
            this.BreakpointNumericUpDown.Size = new Size(136, 20);
            this.BreakpointNumericUpDown.TabIndex = 9;
            this.SetBreakpointButton.Location = new Point(6, 45);
            this.SetBreakpointButton.Name = "SetBreakpointButton";
            this.SetBreakpointButton.Size = new Size(124, 23);
            this.SetBreakpointButton.TabIndex = 8;
            this.SetBreakpointButton.Text = "Set Breakpoint";
            this.SetBreakpointButton.UseVisualStyleBackColor = true;
            this.SetBreakpointButton.Click += new EventHandler(this.SetBreakpointButton_Click);
            this.WatchpointGroupBox.Controls.Add((Control) this.WatchpointTypeLabel);
            this.WatchpointGroupBox.Controls.Add((Control) this.WatchpointLengthLabel);
            this.WatchpointGroupBox.Controls.Add((Control) this.WatchpointIndexLabel);
            this.WatchpointGroupBox.Controls.Add((Control) this.ClearWatchpointButton);
            this.WatchpointGroupBox.Controls.Add((Control) this.WatchpointNumericUpDown);
            this.WatchpointGroupBox.Controls.Add((Control) this.WatchpointLengthComboBox);
            this.WatchpointGroupBox.Controls.Add((Control) this.BreaktypeComboBox);
            this.WatchpointGroupBox.Controls.Add((Control) this.SetWatchpointButton);
            this.WatchpointGroupBox.Location = new Point(9, 203);
            this.WatchpointGroupBox.Name = "WatchpointGroupBox";
            this.WatchpointGroupBox.Size = new Size(207, 129);
            this.WatchpointGroupBox.TabIndex = 9;
            this.WatchpointGroupBox.TabStop = false;
            this.WatchpointGroupBox.Text = "Watchpoint";
            this.WatchpointTypeLabel.AutoSize = true;
            this.WatchpointTypeLabel.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.WatchpointTypeLabel.Location = new Point(21, 75);
            this.WatchpointTypeLabel.Name = "WatchpointTypeLabel";
            this.WatchpointTypeLabel.Size = new Size(35, 14);
            this.WatchpointTypeLabel.TabIndex = 14;
            this.WatchpointTypeLabel.Text = "Type";
            this.WatchpointLengthLabel.AutoSize = true;
            this.WatchpointLengthLabel.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.WatchpointLengthLabel.Location = new Point(7, 48);
            this.WatchpointLengthLabel.Name = "WatchpointLengthLabel";
            this.WatchpointLengthLabel.Size = new Size(49, 14);
            this.WatchpointLengthLabel.TabIndex = 13;
            this.WatchpointLengthLabel.Text = "Length";
            this.WatchpointIndexLabel.AutoSize = true;
            this.WatchpointIndexLabel.Font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.WatchpointIndexLabel.Location = new Point(14, 21);
            this.WatchpointIndexLabel.Name = "WatchpointIndexLabel";
            this.WatchpointIndexLabel.Size = new Size(42, 14);
            this.WatchpointIndexLabel.TabIndex = 12;
            this.WatchpointIndexLabel.Text = "Index";
            this.ClearWatchpointButton.Location = new Point(139, 99);
            this.ClearWatchpointButton.Name = "ClearWatchpointButton";
            this.ClearWatchpointButton.Size = new Size(59, 23);
            this.ClearWatchpointButton.TabIndex = 11;
            this.ClearWatchpointButton.Text = "Clear";
            this.ClearWatchpointButton.UseVisualStyleBackColor = true;
            this.ClearWatchpointButton.Click += new EventHandler(this.ClearWatchpointButton_Click);
            this.WatchpointNumericUpDown.Location = new Point(62, 19);
            this.WatchpointNumericUpDown.Maximum = new Decimal(new int[4]
            {
                3,
                0,
                0,
                0
            });
            this.WatchpointNumericUpDown.Name = "WatchpointNumericUpDown";
            this.WatchpointNumericUpDown.Size = new Size(136, 20);
            this.WatchpointNumericUpDown.TabIndex = 10;
            this.WatchpointLengthComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.WatchpointLengthComboBox.FormattingEnabled = true;
            this.WatchpointLengthComboBox.Items.AddRange(new object[4]
            {
                (object) "1 byte",
                (object) "2 bytes",
                (object) "4 bytes",
                (object) "8 bytes"
            });
            this.WatchpointLengthComboBox.Location = new Point(62, 45);
            this.WatchpointLengthComboBox.Name = "WatchpointLengthComboBox";
            this.WatchpointLengthComboBox.Size = new Size(136, 21);
            this.WatchpointLengthComboBox.TabIndex = 10;
            this.BreaktypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.BreaktypeComboBox.FormattingEnabled = true;
            this.BreaktypeComboBox.Items.AddRange(new object[3]
            {
                (object) "execute",
                (object) "write",
                (object) "read/write"
            });
            this.BreaktypeComboBox.Location = new Point(62, 72);
            this.BreaktypeComboBox.Name = "BreaktypeComboBox";
            this.BreaktypeComboBox.Size = new Size(136, 21);
            this.BreaktypeComboBox.TabIndex = 9;
            this.SetWatchpointButton.Location = new Point(6, 99);
            this.SetWatchpointButton.Name = "SetWatchpointButton";
            this.SetWatchpointButton.Size = new Size((int) sbyte.MaxValue, 23);
            this.SetWatchpointButton.TabIndex = 8;
            this.SetWatchpointButton.Text = "Set Watchpoint";
            this.SetWatchpointButton.UseVisualStyleBackColor = true;
            this.SetWatchpointButton.Click += new EventHandler(this.SetWatchpointButton_Click);
            this.StopButton.BackColor = Color.DarkSalmon;
            this.StopButton.Font =
                new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.StopButton.ForeColor = Color.Black;
            this.StopButton.Location = new Point(40, 19);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new Size(25, 25);
            this.StopButton.TabIndex = 7;
            this.StopButton.Text = "❚❚";
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new EventHandler(this.StopButton_Click);
            this.GoButton.BackColor = Color.FromArgb(0, 192, 0);
            this.GoButton.Font = new Font("Microsoft Sans Serif", 9f);
            this.GoButton.ForeColor = Color.Black;
            this.GoButton.Location = new Point(9, 19);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new Size(25, 25);
            this.GoButton.TabIndex = 6;
            this.GoButton.Text = "▶";
            this.GoButton.UseVisualStyleBackColor = false;
            this.GoButton.Click += new EventHandler(this.GoButton_Click);
            this.ControlAndRegistersGroupBox.Controls.Add((Control) this.AutoPatchButton);
            this.ControlAndRegistersGroupBox.Controls.Add((Control) this.TryFindButton);
            this.ControlAndRegistersGroupBox.Controls.Add((Control) this.ClearBreakpoints);
            this.ControlAndRegistersGroupBox.Controls.Add((Control) this.ClearWatchPoints);
            this.ControlAndRegistersGroupBox.Controls.Add((Control) this.RegistersTextBox);
            this.ControlAndRegistersGroupBox.Location = new Point(6, 477);
            this.ControlAndRegistersGroupBox.Name = "ControlAndRegistersGroupBox";
            this.ControlAndRegistersGroupBox.Size = new Size(752, 171);
            this.ControlAndRegistersGroupBox.TabIndex = 1;
            this.ControlAndRegistersGroupBox.TabStop = false;
            this.ControlAndRegistersGroupBox.Text = "Control and Registers";
            this.AutoPatchButton.Location = new Point(6, 106);
            this.AutoPatchButton.Name = "AutoPatchButton";
            this.AutoPatchButton.Size = new Size(110, 23);
            this.AutoPatchButton.TabIndex = 6;
            this.AutoPatchButton.Text = "Auto NOP Current";
            this.AutoPatchButton.UseVisualStyleBackColor = true;
            this.AutoPatchButton.Click += new EventHandler(this.AutoPatchButton_Click);
            this.TryFindButton.Location = new Point(6, 77);
            this.TryFindButton.Name = "TryFindButton";
            this.TryFindButton.Size = new Size(110, 23);
            this.TryFindButton.TabIndex = 5;
            this.TryFindButton.Text = "Last Instruction";
            this.TryFindButton.UseVisualStyleBackColor = true;
            this.TryFindButton.Click += new EventHandler(this.TryFindButton_Click);
            this.ClearBreakpoints.Location = new Point(6, 19);
            this.ClearBreakpoints.Name = "ClearBreakpoints";
            this.ClearBreakpoints.Size = new Size(110, 23);
            this.ClearBreakpoints.TabIndex = 4;
            this.ClearBreakpoints.Text = "Clear Breakpoints";
            this.ClearBreakpoints.UseVisualStyleBackColor = true;
            this.ClearBreakpoints.Click += new EventHandler(this.ClearBreakpoints_Click);
            this.ClearWatchPoints.Location = new Point(6, 48);
            this.ClearWatchPoints.Name = "ClearWatchPoints";
            this.ClearWatchPoints.Size = new Size(110, 23);
            this.ClearWatchPoints.TabIndex = 3;
            this.ClearWatchPoints.Text = "Clear Watchpoints";
            this.ClearWatchPoints.UseVisualStyleBackColor = true;
            this.ClearWatchPoints.Click += new EventHandler(this.ClearWatchPoints_Click);
            this.RegistersTextBox.BackColor = SystemColors.Info;
            this.RegistersTextBox.Font = new Font("Consolas", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
            this.RegistersTextBox.Location = new Point(122, 19);
            this.RegistersTextBox.Multiline = true;
            this.RegistersTextBox.Name = "RegistersTextBox";
            this.RegistersTextBox.ReadOnly = true;
            this.RegistersTextBox.ScrollBars = ScrollBars.Vertical;
            this.RegistersTextBox.Size = new Size(616, 146);
            this.RegistersTextBox.TabIndex = 0;
            this.RegistersTextBox.Text = componentResourceManager.GetString("RegistersTextBox.Text");
            this.RebootButton.Enabled = false;
            this.RebootButton.Location = new Point(671, 12);
            this.RebootButton.Name = "RebootButton";
            this.RebootButton.Size = new Size(71, 23);
            this.RebootButton.TabIndex = 5;
            this.RebootButton.Text = "Reboot";
            this.RebootButton.UseVisualStyleBackColor = true;
            this.RebootButton.Click += new EventHandler(this.RebootButton_Click);
            this.DetachButton.Enabled = false;
            this.DetachButton.Location = new Point(286, 13);
            this.DetachButton.Name = "DetachButton";
            this.DetachButton.Size = new Size(75, 23);
            this.DetachButton.TabIndex = 6;
            this.DetachButton.Text = "Detach";
            this.DetachButton.UseVisualStyleBackColor = true;
            this.DetachButton.Click += new EventHandler(this.DetachButton_Click);
            this.FilterProcessListCheckBox.AutoSize = true;
            this.FilterProcessListCheckBox.Checked = true;
            this.FilterProcessListCheckBox.CheckState = CheckState.Checked;
            this.FilterProcessListCheckBox.Location = new Point(557, 16);
            this.FilterProcessListCheckBox.Name = "FilterProcessListCheckBox";
            this.FilterProcessListCheckBox.Size = new Size(108, 17);
            this.FilterProcessListCheckBox.TabIndex = 7;
            this.FilterProcessListCheckBox.Text = "Filter Process List";
            this.FilterProcessListCheckBox.UseVisualStyleBackColor = true;
            this.FilterProcessListCheckBox.CheckedChanged +=
                new EventHandler(this.FilterProcessListCheckBox_CheckedChanged);
            this.MainPanel.Controls.Add((Control) this.ControlAndRegistersGroupBox);
            this.MainPanel.Controls.Add((Control) this.EditGroupBox);
            this.MainPanel.Controls.Add((Control) this.TabControl);
            this.MainPanel.Enabled = false;
            this.MainPanel.Location = new Point(12, 42);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new Size(980, 657);
            this.MainPanel.TabIndex = 8;
            this.TabControl.Controls.Add((Control) this.DisassemblyTabPage);
            this.TabControl.Controls.Add((Control) this.MemoryTabPage);
            this.TabControl.Controls.Add((Control) this.ScanTabPage);
            this.TabControl.Location = new Point(3, 3);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new Size(752, 468);
            this.TabControl.TabIndex = 6;
            this.DisassemblyTabPage.Controls.Add((Control) this.DisassemblyTextBox);
            this.DisassemblyTabPage.Location = new Point(4, 22);
            this.DisassemblyTabPage.Name = "DisassemblyTabPage";
            this.DisassemblyTabPage.Padding = new Padding(3);
            this.DisassemblyTabPage.Size = new Size(744, 442);
            this.DisassemblyTabPage.TabIndex = 0;
            this.DisassemblyTabPage.Text = "Disassembly";
            this.DisassemblyTabPage.UseVisualStyleBackColor = true;
            this.DisassemblyTextBox.BackColor = SystemColors.WindowFrame;
            this.DisassemblyTextBox.Dock = DockStyle.Fill;
            this.DisassemblyTextBox.Font = new Font("Consolas", 11f);
            this.DisassemblyTextBox.ForeColor = SystemColors.HighlightText;
            this.DisassemblyTextBox.Location = new Point(3, 3);
            this.DisassemblyTextBox.Name = "DisassemblyTextBox";
            this.DisassemblyTextBox.Size = new Size(738, 436);
            this.DisassemblyTextBox.TabIndex = 1;
            this.DisassemblyTextBox.Text = "";
            this.MemoryTabPage.Controls.Add((Control) this.MemoryHexBox);
            this.MemoryTabPage.Location = new Point(4, 22);
            this.MemoryTabPage.Name = "MemoryTabPage";
            this.MemoryTabPage.Padding = new Padding(3);
            this.MemoryTabPage.Size = new Size(744, 442);
            this.MemoryTabPage.TabIndex = 1;
            this.MemoryTabPage.Text = "Memory";
            this.MemoryTabPage.UseVisualStyleBackColor = true;
            this.ScanTabPage.Controls.Add((Control) this.ScanDataGridView);
            this.ScanTabPage.Location = new Point(4, 22);
            this.ScanTabPage.Name = "ScanTabPage";
            this.ScanTabPage.Padding = new Padding(3);
            this.ScanTabPage.Size = new Size(744, 442);
            this.ScanTabPage.TabIndex = 2;
            this.ScanTabPage.Text = "Scan";
            this.ScanTabPage.UseVisualStyleBackColor = true;
            this.SupportLinkLabel.AutoSize = true;
            this.SupportLinkLabel.Font = new Font("Microsoft Tai Le", 9f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
            this.SupportLinkLabel.Location = new Point(756, 16);
            this.SupportLinkLabel.Name = "SupportLinkLabel";
            this.SupportLinkLabel.Size = new Size(155, 16);
            this.SupportLinkLabel.TabIndex = 10;
            this.SupportLinkLabel.TabStop = true;
            this.SupportLinkLabel.Text = "Official Debugger Support";
            this.SupportLinkLabel.LinkClicked +=
                new LinkLabelLinkClickedEventHandler(this.SupportLinkLabel_LinkClicked);
            this.CreditsButton.Location = new Point(917, 12);
            this.CreditsButton.Name = "CreditsButton";
            this.CreditsButton.Size = new Size(75, 23);
            this.CreditsButton.TabIndex = 11;
            this.CreditsButton.Text = "Credits";
            this.CreditsButton.UseVisualStyleBackColor = true;
            this.CreditsButton.Click += new EventHandler(this.CreditsButton_Click);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1004, 711);
            this.Controls.Add((Control) this.CreditsButton);
            this.Controls.Add((Control) this.SupportLinkLabel);
            this.Controls.Add((Control) this.MainPanel);
            this.Controls.Add((Control) this.FilterProcessListCheckBox);
            this.Controls.Add((Control) this.RebootButton);
            this.Controls.Add((Control) this.DetachButton);
            this.Controls.Add((Control) this.RefreshButton);
            this.Controls.Add((Control) this.ProcessComboBox);
            this.Controls.Add((Control) this.IpTextBox);
            this.Controls.Add((Control) this.AttachButton);
            this.Controls.Add((Control) this.ConnectButton);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.Name = nameof(DebugWatchForm);
            this.Text = "Debug Watch";
            this.FormClosing += new FormClosingEventHandler(this.DebugWatchForm_FormClosing);
            ((ISupportInitialize) this.ScanDataGridView).EndInit();
            this.EditGroupBox.ResumeLayout(false);
            this.EditGroupBox.PerformLayout();
            this.MemoryScanGroupBox.ResumeLayout(false);
            this.MemoryScanGroupBox.PerformLayout();
            this.BreakpointGroupBox.ResumeLayout(false);
            this.BreakpointGroupBox.PerformLayout();
            this.BreakpointNumericUpDown.EndInit();
            this.WatchpointGroupBox.ResumeLayout(false);
            this.WatchpointGroupBox.PerformLayout();
            this.WatchpointNumericUpDown.EndInit();
            this.ControlAndRegistersGroupBox.ResumeLayout(false);
            this.ControlAndRegistersGroupBox.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.DisassemblyTabPage.ResumeLayout(false);
            this.MemoryTabPage.ResumeLayout(false);
            this.ScanTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}