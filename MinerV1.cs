public string shipGyro = "miner_gyro";
public string shipReactor = "miner_reactor";
public string[] shipDisplays = {
  "miner_status_display"
};

public List < IMyTextSurface > displays = new List < IMyTextSurface > ();
public IMyTextSurface myDisplay = null;
public Color displayColor = Color.Yellow;

public double fuelAlarm = 10;

public void DisplayMessage(string message) {
  myDisplay.ContentType = ContentType.TEXT_AND_IMAGE;
  myDisplay.WriteText(message);
  myDisplay.FontColor = displayColor;

  foreach(IMyTextSurface display in displays) {
    display.ContentType = ContentType.TEXT_AND_IMAGE;
    display.WriteText(message);
    display.FontColor = displayColor;
  }
}

public void Main(string args, UpdateType updateType) {
  if (myDisplay == null) {
    myDisplay = Me.GetSurface(0);
  }

  if ((updateType & (UpdateType.Trigger | UpdateType.Terminal)) != 0) {
    displays = new List < IMyTextSurface > ();

    for (int i = 0; i < shipDisplays.Length; i++) {
      IMyTextSurface display = GridTerminalSystem.GetBlockWithName(shipDisplays[i]) as IMyTextSurface;

      displays.Add(display);
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update1;
  }

  IMyGyro gyro = GridTerminalSystem.GetBlockWithName(shipGyro) as IMyGyro;

  String gyroData = "Yaw: " + Math.Round(gyro.Yaw, 1) + "\nPitch: " + Math.Round(gyro.Pitch, 1) + "\nRoll: " + Math.Round(gyro.Roll, 1);

  IMyReactor reactor = GridTerminalSystem.GetBlockWithName(shipReactor) as IMyReactor;

  String power = "Power: " + Math.Round(reactor.CurrentOutput, 2) + " MW";

  MyFixedPoint fuelAmount = 0;

  String extra = "";

  List < MyInventoryItem > items = new List < MyInventoryItem > ();
  reactor.GetInventory().GetItems(items);

  foreach(MyInventoryItem item in items) {
    if (item.Type == MyItemType.MakeIngot("Uranium")) {
      fuelAmount += item.Amount;
    }
  }

  double fuelDouble = Double.Parse(fuelAmount + "");

  if (fuelDouble < fuelAlarm) {
    displayColor = Color.Red;
    extra += "Fuel low (< " + fuelAlarm + ")";
  } else {
    displayColor = Color.Lime;
  }

  String fuel = "Fuel: " + Math.Round(fuelDouble, 2);

  DisplayMessage(gyroData + "\n" + power + "\n" + fuel + "\n" + extra);
}
