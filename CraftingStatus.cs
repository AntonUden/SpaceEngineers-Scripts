public void DisplayMessage(string message) {
  IMyTextSurface localDisplay = Me.GetSurface(0);
  localDisplay.ContentType = ContentType.TEXT_AND_IMAGE;
  localDisplay.FontSize = 1;
  localDisplay.WriteText(message);
}

public List < IMyTextSurface > displays = new List < IMyTextSurface > ();

public void Main(string args, UpdateType updateType) {
  if ((updateType & (UpdateType.Trigger | UpdateType.Terminal)) != 0) {
    String[] argumentArray = args.Split(',');
    if (args.Length == 0) {
      DisplayMessage("Please privide a comma separated list of\ndisplays to output to");
      Runtime.UpdateFrequency = UpdateFrequency.None;
      return;
    } else {
      displays = new List < IMyTextSurface > ();

      for (int i = 0; i < argumentArray.Length; i++) {
        IMyTextSurface display = GridTerminalSystem.GetBlockWithName(argumentArray[i]) as IMyTextSurface;

        displays.Add(display);
      }

      Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
  }

  List < IMyAssembler > assemblers = new List < IMyAssembler > ();
  GridTerminalSystem.GetBlocksOfType < IMyAssembler > (assemblers);

  int items = 0;

  foreach(IMyAssembler assembler in assemblers) {
    List < MyProductionItem > queue = new List < MyProductionItem > ();
    assembler.GetQueue(queue);

    foreach(MyProductionItem item in queue) {
      items += (int)(item.Amount.RawValue / 1000000);
    }
  }

  foreach(IMyTextPanel display in displays) {
    display.ContentType = ContentType.TEXT_AND_IMAGE;
    display.WriteText(items + "\nitems in queue");

    if (items > 0) {
      display.FontColor = Color.Yellow;
    } else {
      display.FontColor = Color.Green;
    }
  }

  DisplayMessage("ok\n" + items + "\nitems in queue\n" + displays.Count + " displays\nUpdate type: " + Enum.GetName(typeof(UpdateType), updateType));
}
