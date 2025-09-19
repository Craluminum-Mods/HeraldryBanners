using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace HeraldryBanners;

public class BlockEntityPole : BlockEntityDisplay
{
    protected InventoryDisplayed inventory;

    public override string AttributeTransformCode => Block.Attributes["attributeTransformCode"].AsString();
    public override InventoryBase Inventory => inventory;
    public override string InventoryClassName => "heraldrybanners:pole";

    public BlockEntityPole()
    {
        inventory = new InventoryDisplayed(this, 1, null, null, onNewSlot: (_slotId, _inv) => new ItemSlotPole(_inv));
    }

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);

        if (!GuiDialogTransformEditor.extraTransforms.Any(x => x.AttributeName == AttributeTransformCode))
        {
            GuiDialogTransformEditor.extraTransforms.Add(new TransformConfig()
            {
                Title = Lang.Get($"{Block.Code.Domain}:transform-{AttributeTransformCode}"),
                AttributeName = AttributeTransformCode
            });
        }
    }

    public bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
    {
        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;
        int slotId = 0;

        if (slot.Empty)
        {
            if (TryTake(byPlayer, blockSel))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (slot.Itemstack.ItemAttributes.IsTrue("heraldryPoleStorable"))
            {
                AssetLocation sound = slot.Itemstack?.Block?.Sounds?.Place;

                if (TryPut(slot, blockSel, byPlayer))
                {
                    Api.World.PlaySoundAt(sound != null ? sound : new AssetLocation("sounds/player/build"), byPlayer.Entity, byPlayer, true, 16);
                    Api.World.Logger.Audit("{0} Put 1x{1} onto Pole slotid {2} at {3}.",
                        byPlayer.PlayerName,
                        inventory[slotId].Itemstack?.Collectible.Code,
                        slotId,
                        Pos
                    );
                    return true;
                }

                return false;
            }

            (Api as ICoreClientAPI)?.TriggerIngameError(this, "doesnotfit", Lang.Get("heraldrybanners:ingameerror-pole-doesnotfit"));
            return true;
        }
    }

    private bool TryPut(ItemSlot slot, BlockSelection blockSel, IPlayer player)
    {
        int slotId = 0;

        if (slotId == -1 || slotId >= inventory.Count) return false;

        if (!inventory[slotId].Empty) return false;

        int moved = slot.TryPutInto(Api.World, inventory[slotId]);

        if (moved > 0)
        {
            MarkDirty();
            updateMeshes();
        }
        return moved > 0;
    }

    private bool TryTake(IPlayer byPlayer, BlockSelection blockSel)
    {
        int slotId = 0;

        if (slotId == -1 || slotId >= inventory.Count) return false;

        if (inventory[slotId].Empty) return false;

        ItemStack stack = inventory[slotId].TakeOut(1);
        if (byPlayer.InventoryManager.TryGiveItemstack(stack))
        {
            AssetLocation sound = stack.Block?.Sounds?.Place;
            Api.World.PlaySoundAt(sound != null ? sound : new AssetLocation("sounds/player/build"), byPlayer.Entity, byPlayer, true, 16);
            Api.World.Logger.Audit("{0} Took 1x{1} from Pole slotid {2} at {3}.",
                byPlayer.PlayerName,
                stack.Collectible.Code,
                slotId,
                Pos
            );
        }

        if (stack.StackSize > 0)
        {
            Api.World.SpawnItemEntity(stack, Pos);
        }

        MarkDirty();
        updateMeshes();
        return true;
    }

    protected override string getMeshCacheKey(ItemStack stack) => $"{AttributeTransformCode}-{base.getMeshCacheKey(stack)}";

    protected override float[][] genTransformationMatrices()
    {
        tfMatrices = new float[DisplayedItems][];
        for (int i = 0; i < DisplayedItems; i++)
        {
            tfMatrices[i] = new Matrixf().Translate(Vec3f.Half).RotateYDeg(Block.Shape.rotateY).Values;
        }
        return tfMatrices;
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
    {
        foreach (BlockEntityBehavior behavior in Behaviors)
        {
            behavior.GetBlockInfo(forPlayer, dsc);
        }

        dsc.AppendLine(Inventory[0].Empty ? Lang.Get("Empty") : $"{Inventory[0].StackSize}x " + Inventory[0].GetStackName());
    }
}