// Decompiled with JetBrains decompiler
// Type: debugwatch.Settings
// Assembly: dbgw, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C42DB83B-FBD0-4471-97D2-F43102A97A5F
// Assembly location: C:\Users\x\Downloads\PS4\debugwatch\dbgw\dbgw.exe

using System.IO;

namespace debugwatch
{
  internal class Settings
  {
    private static readonly string SETTINGS_FILE = "settings.bin";
    private static readonly int IP_INDEX = 0;
    private static readonly int HOSTIP_INDEX = 1;
    private static readonly int FILTER_INDEX = 2;

    private static void MakeDefaultSettings()
    {
      string[] contents = new string[3]
      {
        "1.1.1.1",
        "2.2.2.2",
        "false"
      };
      File.WriteAllLines(Settings.SETTINGS_FILE, contents);
    }

    public static string ip
    {
      get
      {
        if (File.Exists(Settings.SETTINGS_FILE))
          return File.ReadAllLines(Settings.SETTINGS_FILE)[Settings.IP_INDEX];
        Settings.MakeDefaultSettings();
        return File.ReadAllLines(Settings.SETTINGS_FILE)[Settings.IP_INDEX];
      }
      set
      {
        string[] contents = File.ReadAllLines(Settings.SETTINGS_FILE);
        contents[Settings.IP_INDEX] = value;
        File.WriteAllLines(Settings.SETTINGS_FILE, contents);
      }
    }

    public static string hostip
    {
      get
      {
        if (File.Exists(Settings.SETTINGS_FILE))
          return File.ReadAllLines(Settings.SETTINGS_FILE)[Settings.HOSTIP_INDEX];
        Settings.MakeDefaultSettings();
        return File.ReadAllLines(Settings.SETTINGS_FILE)[Settings.HOSTIP_INDEX];
      }
      set
      {
        string[] contents = File.ReadAllLines(Settings.SETTINGS_FILE);
        contents[Settings.HOSTIP_INDEX] = value;
        File.WriteAllLines(Settings.SETTINGS_FILE, contents);
      }
    }

    public static bool filter
    {
      get
      {
        if (File.Exists(Settings.SETTINGS_FILE))
          return File.ReadAllLines(Settings.SETTINGS_FILE)[Settings.FILTER_INDEX] == "true";
        Settings.MakeDefaultSettings();
        return File.ReadAllLines(Settings.SETTINGS_FILE)[Settings.FILTER_INDEX] == "true";
      }
      set
      {
        string[] contents = File.ReadAllLines(Settings.SETTINGS_FILE);
        contents[Settings.FILTER_INDEX] = value ? "true" : "false";
        File.WriteAllLines(Settings.SETTINGS_FILE, contents);
      }
    }
  }
}
