using Vintagestory.API.Common;

namespace HeraldryBanners;

public class ItemSlotPole(InventoryBase inventory) : ItemSlot(inventory)
{
    public override int MaxSlotStackSize => 1;

    public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
    {
        if (sourceSlot?.Itemstack?.ItemAttributes?.IsTrue("heraldryPoleStorable") == true)
        {
            return base.CanTakeFrom(sourceSlot, priority);
        }
        return false;
    }

    public override bool CanHold(ItemSlot sourceSlot)
    {
        if (base.CanHold(sourceSlot) && sourceSlot?.Itemstack?.ItemAttributes?.IsTrue("heraldryPoleStorable") == true)
        {
            return inventory.CanContain(this, sourceSlot);
        }
        return false;
    }
}