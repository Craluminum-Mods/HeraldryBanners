using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace HeraldryBanners;

public class BlockPole : BlockGeneric, IMultiBlockColSelBoxes
{
    public ValuesByMultiblockOffset ValuesByMultiblockOffset { get; set; } = new();

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        ValuesByMultiblockOffset = ValuesByMultiblockOffset.FromAttributes(this);
    }

    public override bool DoParticalSelection(IWorldAccessor world, BlockPos pos) => true;

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityPole blockEntity)
        {
            return blockEntity.OnInteract(byPlayer, blockSel);
        }
        return base.OnBlockInteractStart(world, byPlayer, blockSel);
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        WorldInteraction[] interactions =
        [
            new WorldInteraction()
            {
                MouseButton = EnumMouseButton.Right,
                ActionLangCode = "blockhelp-displaycase-place",
            },
            new WorldInteraction()
            {
                MouseButton = EnumMouseButton.Right,
                RequireFreeHand = true,
                ActionLangCode = "blockhelp-displaycase-remove",
            }
        ];
        return interactions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
    }

    Cuboidf[] IMultiBlockColSelBoxes.MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset)
    {
        if (ValuesByMultiblockOffset.CollisionBoxesByOffset.TryGetValue(offset, out Cuboidf[] collisionBoxes))
        {
            return collisionBoxes;
        }
        Block originaBlock = blockAccessor.GetBlock(pos.AddCopy(offset.X, offset.Y, offset.Z));
        return originaBlock.GetCollisionBoxes(blockAccessor, pos);
    }

    Cuboidf[] IMultiBlockColSelBoxes.MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset)
    {
        if (ValuesByMultiblockOffset.SelectionBoxesByOffset.TryGetValue(offset, out Cuboidf[] selectionBoxes))
        {
            return selectionBoxes;
        }
        Block originaBlock = blockAccessor.GetBlock(pos.AddCopy(offset.X, offset.Y, offset.Z));
        return originaBlock.GetSelectionBoxes(blockAccessor, pos);
    }
}