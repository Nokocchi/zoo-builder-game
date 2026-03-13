namespace ZooBuilder.entities.player;

public class SelectedHotbarSlotChangedItemEvent(int index)
{
    public int Index { get; private set; } = index;
}