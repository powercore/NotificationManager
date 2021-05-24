using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NotificationManager
{
  public static class Helpers
  {
    public static bool Exists(this object obj)
    {
      return (obj != null) ? true : false;
    }

    public static bool IsValid(this string obj)
    {
      if (obj.Exists())
        return (obj.Length != 0) ? true : false;
      else
        return false;
    }
    public static bool IsEmailValid( this string obj )
    {
      return obj.IsValid() && Regex.Match(obj, "^\\S+@\\S+\\.\\S+$").Success;
    }
  }

  public class Signal
  {
    public string Name { get; set; }
    public List<string> Recipients { get; set; }
   
    public void SetupSignal(string name)
    {
      Name = name;
      Recipients = new List<string>();
    }

    private string GetRecipient(string email)
    {
      return email.IsValid() ? Recipients.FirstOrDefault(recipientEmail => recipientEmail == email) : null;
    }

    public bool IsRecipientExists(string email)
    {
      return email.IsValid() && GetRecipient(email).Exists();
    }

    public void AddNewRecipient(string email)
    {
      if (!email.IsValid()) return;
      if (!IsRecipientExists(email))
        Recipients.Add(email);
    }

    public void DeleteRecipient(string recipientEmail)
    {
      if (!recipientEmail.IsValid()) return;
      for (int i = 0; i < Recipients.Count; i++)
        if (Recipients[i] == recipientEmail)
        {
          Recipients.RemoveAt(i);
          break;
        }
    }

    public void SendAlarm()
    {
      foreach (string email in Recipients)
        Console.WriteLine($"Send email to: {email} with code: {Name}");
    }
  }

  public interface INotificationManager
  {
    List<Signal> Signals { get; set; }

    void ProceedSignal(string signalSignature);
    void AddRecipientToSignal(string signalSignature, string recipientEmail);
    void AddRecipientToAllSignals(string recipientEmail);
    void RemoveRecipientFromSignal(string signalSignature, string recipientEmail);
    void RemoveRecipientFromAllSignals(string recipientEmail);
    void AddNewSignal(string signalSignature);
    bool IsRecipientAssignedToSignal(string recipientEmail, string signalSignature);
    string GetAllSignals();
    void SaveChanges();
    bool Restore();
  }

  public class NotificationManager : INotificationManager
  {
    public List<Signal> Signals { get; set; }

    public NotificationManager()
    {
      Signals = new List<Signal>();
    }

    private Signal GetSignal(string signalSignature)
    {
      return Signals.FirstOrDefault(signal => signal.Name == signalSignature);
    }

    private bool IsSignalExists(string signalSignature)
    {
      return signalSignature.IsValid() && GetSignal(signalSignature).Exists();
    }

    public void AddNewSignal(string signalSignature)
    {
      if (!signalSignature.IsValid()) return;
      if (IsSignalExists(signalSignature)) return;
      Signal signal = new Signal();
      signal.SetupSignal(signalSignature);
      Signals.Add(signal);
    }

    public string GetAllSignals()
    {
      string result = "Available signals:\n";

      foreach (Signal signal in Signals)
        result += $"{signal.Name}\n";

      return result;
    }

    public void AddRecipientToSignal(string signalSignature, string recipientEmail)
    {
      if (!signalSignature.IsValid() || !recipientEmail.IsEmailValid()) return;
      Signal signal = GetSignal(signalSignature);

      if (signal.Exists())
        signal.AddNewRecipient(recipientEmail);
    }

    public void AddRecipientToAllSignals(string recipientEmail)
    {
      if (!recipientEmail.IsEmailValid()) return;
      foreach (Signal signal in Signals)
        AddRecipientToSignal(signal.Name, recipientEmail);
    }

    public bool IsRecipientAssignedToSignal(string signalSignature, string recipientEmail)
    {
      if (!signalSignature.IsValid() || !recipientEmail.IsValid()) return false;
      Signal signal = GetSignal(signalSignature);

      return signal.Exists() && signal.IsRecipientExists(recipientEmail);
    }

    public void ProceedSignal(string signalSignature)
    {
      if (!signalSignature.IsValid()) return;
      Signal signal = GetSignal(signalSignature);

      if (signal.Exists())
        signal.SendAlarm();
    }

    public void RemoveRecipientFromSignal(string signalSignature, string recipientEmail)
    {
      if (!signalSignature.IsValid() || !recipientEmail.IsValid()) return;
      Signal signal = GetSignal(signalSignature);

      if (signal.Exists())
        signal.DeleteRecipient(recipientEmail);
    }

    public void RemoveRecipientFromAllSignals(string recipientEmail)
    {
      if (!recipientEmail.IsValid()) return;
      foreach (var signal in Signals.Where(signal => signal.Exists()))
        signal.DeleteRecipient(recipientEmail);
    }
    public void SaveChanges()
    {
      XmlSerializer sr = new XmlSerializer(this.GetType());
      TextWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "data.xml");
      sr.Serialize(writer, this);
      writer.Close();
    }

    public bool Restore()
    {
      if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data.xml")) return false;
      XmlSerializer sr = new XmlSerializer(this.GetType());
      TextReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "data.xml");
      NotificationManager temp = (NotificationManager)sr.Deserialize(reader);
      foreach (Signal signal in temp.Signals)
      {
        Signal newSignal = new Signal();
        newSignal.SetupSignal(signal.Name);

        foreach (string email in signal.Recipients)
          newSignal.Recipients.Add(email);

        Signals.Add(newSignal);
      }
      return true;
    }

  }
}