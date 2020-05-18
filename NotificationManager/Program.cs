using System;

namespace NotificationManager
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      NotificationManager nm = new NotificationManager();

      if (!nm.Restore())
      { 
        nm.AddNewSignal("NEW_TANG");
        nm.AddRecipientToSignal("NEW_TANG", "requests@yeahbabymgmt.com");
        nm.AddNewSignal("ROOF_IS_ON_FIRE");
        nm.AddRecipientToSignal("ROOF_IS_ON_FIRE", "Hughlaurie@studiofanmail.com");
        nm.AddRecipientToSignal("ROOF_IS_ON_FIRE", "therock@studiofanmail.com");

        nm.AddNewSignal("HOW_MUCH_IS_DA_FISH");
        nm.AddRecipientToSignal("HOW_MUCH_IS_DA_FISH", "management@scootertechno.com");
        nm.AddRecipientToSignal("HOW_MUCH_IS_DA_FISH", "info@leomessi.com");

        nm.AddRecipientToAllSignals("miketysonlive@gmail.com");

        Console.WriteLine(nm.GetAllSignals());
        Console.WriteLine($"Mike Tyson assigned to alarm \"NEW_TANG\" and it is { nm.IsRecipientAssignedToSignal("NEW_TANG", "miketysonlive@gmail.com")}");

        nm.ProceedSignal("ROOF_IS_ON_FIRE");
        Console.WriteLine("Removing mr Johnson from \"ROOF_IS_ON_FIRE\" alarm list.");

        nm.RemoveRecipientFromSignal("ROOF_IS_ON_FIRE", "therock@studiofanmail.com");

        nm.ProceedSignal("ROOF_IS_ON_FIRE");

        Console.WriteLine("Removing Mike from all alarm lists.");

        nm.RemoveRecipientFromAllSignals("miketysonlive@gmail.com");

        nm.ProceedSignal("ROOF_IS_ON_FIRE");
        nm.SaveChanges();
      }
       else
      {
        Console.WriteLine(nm.GetAllSignals());
        nm.ProceedSignal("ROOF_IS_ON_FIRE");
        nm.ProceedSignal("NEW_TANG");
        nm.ProceedSignal("HOW_MUCH_IS_DA_FISH");
      }
    }
  }
}

