using Vintagestory.API.Common;

namespace HeraldryBanners;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockClass("HeraldryBanners.BlockPole", typeof(BlockPole));
        api.RegisterBlockEntityClass("HeraldryBanners.BEPole", typeof(BlockEntityPole));
    }
}