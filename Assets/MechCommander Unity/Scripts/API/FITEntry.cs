// Decompiled with JetBrains decompiler
// Type: MCGDataFileProcessor.INIEntry
// Assembly: MCGDataFileProcessor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F59F319E-F7ED-4C26-B97D-C3D3356D67FD
// Assembly location: C:\Users\dit gestion\Downloads\1\MCX Original\Finished Tools - Tigress\MCGDataFileProcessor.dll

namespace MechCommanderUnity.API
{
  public class FITEntry
  {
    public string dataType { get; private set; }

    public string keyName { get; private set; }

    public string value { get; private set; }

    public FITEntry()
    {
      this.dataType = (string) null;
      this.keyName = (string) null;
      this.value = (string) null;
    }

    public FITEntry(string line)
    {
      this.dataType = line.Split(' ')[0].Trim();
      string[] strArray = line.Split('=');
      if (strArray[1].Contains("//"))
        strArray[1] = strArray[1].Substring(0, strArray[1].IndexOf('/'));
      this.keyName = strArray[0].Substring(this.dataType.Length).Trim();
      this.value = strArray[1].Trim();
    }

    public override string ToString()
    {
      return this.dataType + " " + this.keyName + "=" + this.value;
    }

    public string getValueByKey(string key)
    {
      if (!(this.keyName == key))
        return (string) null;
      return this.value;
    }

    public bool tryGetValueByKey(string key, out string outValue)
    {
      outValue = this.getValueByKey(key);
      return outValue != null;
    }

    public void setValue(string val)
    {
      this.value = val;
    }
  }
}
