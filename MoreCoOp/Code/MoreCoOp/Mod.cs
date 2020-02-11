using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;
using TowerFall.ModLoader.mm;

namespace MoreCoOp
{
    public class MoreCoOp : Mod
    {
        public MoreCoOp() : base()
        {
            Instance = this;
        }

        internal static MoreCoOp Instance;
        public static int bramblebombIndex;
        int triplebombPickupIndex;
        public static int bombtrailArrowIndex;

        public override void Load()
        {
            bramblebombIndex = RegisterArrowType(typeof(CustomArrow), TFGame.Atlas["player/arrowHUD/bombArrow"], Color.Blue, Color.LightBlue);
            bombtrailArrowIndex = RegisterArrowType(typeof(BombtrailArrow), TFGame.Atlas["player/arrowHUD/bombArrow"], Color.Red, Color.OrangeRed);
           
            RegisterEnemy(typeof(RainbowSlime), "MoreCoOp/RainbowSlime");
            triplebombPickupIndex = RegisterPickup(typeof(CustomPickup), "MoreCoOp/TripleBomb");
        }

        public override void Unload()
        {
        }
    }
}
