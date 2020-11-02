public void Main(string args) {
  String[] argumentArray = args.Split(',');

  if (argumentArray.Length < 2) {
    IMyTextSurface display = Me.GetSurface(0);
    display.ContentType = ContentType.TEXT_AND_IMAGE;
    display.FontSize = 1;
    display.WriteText("Bad argument\narray size:\n" + argumentArray.Length);
  } else {

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

    for (int i = 1; i < argumentArray.Length; i++) {
      IMyTextSurface display = GridTerminalSystem.GetBlockWithName(argumentArray[i]) as IMyTextSurface;
      display.ContentType = ContentType.TEXT_AND_IMAGE;
      display.WriteText(items + "\nitems in queue");

      if (items > 0) {
        display.FontColor = Color.Yellow;
      } else {
        display.FontColor = Color.Green;
      }
    }

    IMyTextSurface localDisplay = Me.GetSurface(0);
    localDisplay.ContentType = ContentType.TEXT_AND_IMAGE;
    localDisplay.FontSize = 1;
    localDisplay.WriteText("ok\n" + items + "\nitems in queue");

    IMyTimerBlock timer = GridTerminalSystem.GetBlockWithName(argumentArray[0]) as IMyTimerBlock;
    timer.StartCountdown();
  }
}
