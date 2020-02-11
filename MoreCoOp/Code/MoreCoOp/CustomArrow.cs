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
    class CustomArrow : ModArrow
    {
        public bool canExplode;
        public CustomArrow() : base()
        {
            explodeAlarm = Alarm.Create(Alarm.AlarmMode.Persist, new Action(Explode), 50, false);
        }

        private Alarm explodeAlarm;

        protected override void Init(LevelEntity owner, Vector2 position, float direction)
        {
            base.Init(owner, position, direction);
            this.used = (this.canDie = false);
            base.StopFlashing();
        }

        protected override void HitWall(Platform platform)
        {
            if (!this.used)
            {
                Explode();
            }
            base.HitWall(platform);
        }

        public override void HitLava()
        {
            Explode();
        }

        private void Explode()
        {
            // Bramble
            base.Add(new Coroutine(Brambles.CreateBrambles(base.Level, this.Position, base.PlayerIndex, new Action(this.FinishBrambles), 3, false)));
            this.UseBramblePower();
            // Explosion
            bool plusOneKill = base.State == Arrow.ArrowStates.Buried && base.BuriedIn.Entity is PlayerCorpse && !base.Level.Session.MatchSettings.IsFriendly((base.BuriedIn.Entity as PlayerCorpse).PlayerIndex, base.PlayerIndex);
            Vector2 at = (this.Position + Calc.AngleToVector(base.Direction + 3.14159274f, 10f)).Floor();
            if (!Explosion.Spawn(base.Level, at, base.PlayerIndex, plusOneKill, false, false))
            {
                Explosion.Spawn(base.Level, this.Position, base.PlayerIndex, plusOneKill, false, false);
            }
            Sounds.pu_bombArrowExplode.Play(base.X, 1f);

        }

        public void UseBramblePower()
        {
            this.used = true;
        }

        public void FinishBrambles()
        {
            this.canDie = true;
        }

        public override void Update()
        {
            base.Update();
            if (this.canDie && !base.Flashing && base.State >= Arrow.ArrowStates.Stuck)
            {
                base.Flash(60, new Action(base.RemoveSelf));
            }
        }

        protected override void CreateGraphics()
        {
            this.normalImage = new Image(TFGame.Atlas["arrows/brambleArrow"], null);
            this.normalImage.Origin = new Vector2(11f, 3f);
            this.buriedImage = new Image(TFGame.Atlas["arrows/brambleArrowBuried"], null);
            this.buriedImage.Origin = new Vector2(11f, 3f);
            this.Graphics = new Image[]
            {
                this.normalImage,
                this.buriedImage
            };
            base.Add(this.Graphics);
        }

        protected override void InitGraphics()
        {
            this.normalImage.Visible = true;
            this.buriedImage.Visible = false;
        }

        protected override void SwapToBuriedGraphics()
        {
            this.normalImage.Visible = false;
            this.buriedImage.Visible = true;
        }

        protected override void SwapToUnburiedGraphics()
        {
            this.normalImage.Visible = true;
            this.buriedImage.Visible = false;
        }

        public override ArrowTypes ArrowType
        {
            get
            {
                return (ArrowTypes)MoreCoOp.bramblebombIndex;
            }
        }

        public override bool IsCollectible
        {
            get
            {
                return !this.used && base.IsCollectible;
            }
        }

        public override bool CanCatchFire
        {
            get
            {
                return false;
            }
        }

        public override bool CanCatch(LevelEntity catcher)
        {
            return !this.used && base.CanCatch(catcher);
        }

        private bool used;

        private bool canDie;

        private Image normalImage;

        private Image buriedImage;
    }

    
}
