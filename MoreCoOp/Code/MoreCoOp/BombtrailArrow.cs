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
    class BombtrailArrow : Arrow
    {
        public int explosionDelay = 12;
        public BombtrailArrow()
        {

        }

        public override ArrowTypes ArrowType => (ArrowTypes)MoreCoOp.bombtrailArrowIndex;

        public override void Update()
        {
            if (State == ArrowStates.Shooting || State == ArrowStates.Falling)
            {
                explosionDelay--;
                if (explosionDelay < 0)
                {
                    Explode();
                    explosionDelay += 3;
                }
            }
            if (!Flashing && State >= (ArrowStates)4)
            {
                Explode();
                RemoveSelf();
            } else
            {
                base.Update();
            }
            
        }

        private void Explode()
        {
            // Explosion
            bool plusOneKill = base.State == Arrow.ArrowStates.Buried && base.BuriedIn.Entity is PlayerCorpse && !base.Level.Session.MatchSettings.IsFriendly((base.BuriedIn.Entity as PlayerCorpse).PlayerIndex, base.PlayerIndex);
            Vector2 at = (this.Position + Calc.AngleToVector(base.Direction + 3.14159274f, 10f)).Floor();
            if (!Explosion.Spawn(base.Level, at, base.PlayerIndex, plusOneKill, false, false))
            {
                Explosion.Spawn(base.Level, this.Position, base.PlayerIndex, plusOneKill, false, false);
            }
            Sounds.pu_bombArrowExplode.Play(base.X, 1f);
        }

        protected override void CreateGraphics()
        {
            this.normalSprite = new Sprite<int>(TFGame.Atlas["arrows/bombArrow"], 15, 6, 0);
            this.normalSprite.Add(0, 0.1f, new int[]
            {
                0,
                1
            });
            this.normalSprite.Add(1, 2);
            this.normalSprite.Add(2, 0);
            this.normalSprite.Origin = new Vector2(13f, 3f);
            this.buriedSprite = new Sprite<int>(TFGame.Atlas["arrows/bombArrowBuried"], 15, 6, 0);
            this.buriedSprite.Add(0, 0.1f, new int[]
            {
                0,
                1
            });
            this.buriedSprite.Add(1, 2);
            this.buriedSprite.Add(2, 3);
            this.buriedSprite.Origin = new Vector2(13f, 3f);
            this.Graphics = new Image[]
            {
                this.normalSprite,
                this.buriedSprite
            };
            base.Add(this.Graphics);
        }

        protected override void InitGraphics()
        {
            this.normalSprite.Visible = true;
            this.buriedSprite.Visible = false;
        }

        protected override void SwapToBuriedGraphics()
        {
            this.normalSprite.Visible = false;
            this.buriedSprite.Visible = true;
        }

        protected override void SwapToUnburiedGraphics()
        {
            this.normalSprite.Visible = true;
            this.buriedSprite.Visible = false;
        }

        private Sprite<int> normalSprite;

        private Sprite<int> buriedSprite;

    }
}
