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

  float reactorOutput = 0;
  MyFixedPoint fuelAmount = 0;

  String extra = "";

  List < IMyReactor > reactors = new List < IMyReactor > ();
  GridTerminalSystem.GetBlocksOfType < IMyReactor > (reactors);

  foreach(IMyReactor reactor in reactors) {
    reactorOutput += reactor.CurrentOutput;

    List < MyInventoryItem > items = new List < MyInventoryItem > ();
    reactor.GetInventory().GetItems(items);

    foreach(MyInventoryItem item in items) {
      if (item.Type == MyItemType.MakeIngot("Uranium")) {
        fuelAmount += item.Amount;
      }
    }
  }

  String power = "Power: " + Math.Round(reactorOutput, 2) + " MW";

  double fuelDouble = Double.Parse(fuelAmount + "");

  if (fuelDouble < fuelAlarm) {
    displayColor = Color.Red;
    extra += "Fuel low (< " + fuelAlarm + ")";
  } else {
    displayColor = Color.Lime;
  }

  String fuel = "Fuel: " + Math.Round(fuelDouble, 2);

  DisplayMessage(power + "\n" + fuel + "\n" + extra);
}
