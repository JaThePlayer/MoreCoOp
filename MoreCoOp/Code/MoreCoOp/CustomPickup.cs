using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod;
using TowerFall;
using TowerFall.ModLoader.mm;

namespace MoreCoOp
{
    // bomb pickup but more bombs
    class CustomPickup : ModPickup
    {
        private Sprite<int>[] images;
        public CustomPickup(Vector2 position, Vector2 targetPosition, int playerIndex) : base(position, targetPosition, playerIndex)
        {
            this.playerIndex = playerIndex;
            Tag(GameTags.LightSource);
            images = new Sprite<int>[3];
            for (int i = 0; i < images.Length; i++)
            {
                Sprite<int> image;
                image = new Sprite<int>(TFGame.Atlas["pickups/bomb"], 12, 12, 0);
                image.X += 14;
                image.X -= 14 * i;
                image.CenterOrigin();
                image.Add(0, 0.05f, new int[]
                {
                0,
                0,
                2,
                1,
                1,
                2
                });

                image.Play(0, false);
                Add(image);
                images[i] = image;
            }
            
        }

        public override void Render()
        {
            base.DrawGlowRed();
            foreach (Sprite<int> image in images)
                image.DrawOutline(1);
            base.Render();
        }

        public override void FinishUnpack(Tween tt)
        {
            base.FinishUnpack(tt);
            this.Collidable = false;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, EXPLODE_TIME, true);
            tween.OnUpdate = delegate (Tween t)
            {
                foreach (Sprite<int> image in images)
                {
                    image.Scale = Vector2.One * MathHelper.Lerp(1f, 3f, t.Eased);
                    image.Rate = MathHelper.Lerp(1f, 4f, t.Eased);
                    image.Rotation = MathHelper.Lerp(0f, 6.28318548f, t.Eased);
                }
                    
            };
            tween.OnComplete = new Action<Tween>(this.Explode);
            Add(tween);
            BombPickup.SFXNewest = this;
            Sounds.sfx_bombChestLoop.Play(base.X, 1f);
        }

        public override void TweenUpdate(float t)
        {
            base.TweenUpdate(t);
            foreach (Sprite<int> image in images)
                image.Scale = Vector2.One * t;
        }

        private void Explode(Tween t = null)
        {
            if (BombPickup.SFXNewest == this)
            {
                Sounds.sfx_bombChestLoop.Stop(true);
            }
            Sounds.pu_bombArrowExplode.Play(base.X, 1f);
            for (int i = 0; i < images.Length; i++)
            {
                Explosion.Spawn(base.Level, this.Position + new Vector2(14 - (14*i), 0), this.playerIndex, false, false, true);
            }
                
            base.RemoveSelf();
        }

        public static Entity SFXNewest;

        private const int EXPLOSION_RADIUS = 45;

        private const int EXPLOSION_HIT_RADIUS = 35;

        private const int EXPLODE_TIME = 40;

        private int playerIndex;

        //private Sprite<int> image;
    }
}
