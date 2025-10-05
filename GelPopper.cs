using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GelPopper
{
    public class GelPopper : ModSystem
    {
        private static readonly int[] Poppable = [ProjectileID.QueenSlimeMinionBlueSpike, ProjectileID.QueenSlimeMinionPinkBall];

        public override void Load()
        {
            On_Player.ItemCheck_MeleeHitNPCs += static (On_Player.orig_ItemCheck_MeleeHitNPCs orig, Player self, Item sItem, Rectangle itemRectangle, int originalDamage, float knockBack) =>
            {
                PopProjectiles(p => p.Colliding(p.Hitbox, itemRectangle));
                orig.Invoke(self, sItem, itemRectangle, originalDamage, knockBack);
            };

            var damage_GetHitbox = typeof(Projectile).GetMethod("Damage_GetHitbox", BindingFlags.NonPublic | BindingFlags.Instance);
            On_Projectile.Damage += (On_Projectile.orig_Damage orig, Projectile self) =>
            {
                if (self.friendly)
                {
                    var vanillaCanDamage = true;
                    var type = self.type;
                    var aiStyle = self.aiStyle;
                    var localAI = self.localAI;
                    var ai = self.ai;
                    var penetrate = self.penetrate;
                    var velocity = self.velocity;
                    var frame = self.frame;

                    if (type == 18 || type == 72 || type == 86 || type == 87 || aiStyle == 31 || aiStyle == 32 || type == 226 || type == 378 || type == 613 || type == 650 || type == 882 || type == 888 || type == 895 || type == 896 || (type == 434 && localAI[0] != 0f) || type == 439 || type == 444 || (type == 451 && ((int)(ai[0] - 1f) / penetrate == 0 || ai[1] < 5f) && ai[0] != 0f) || type == 500 || type == 653 || type == 1018 || type == 460 || type == 633 || type == 600 || type == 601 || type == 602 || type == 535 || (type == 631 && localAI[1] == 0f) || (type == 537 && localAI[0] <= 30f) || type == 651 || (type == 188 && localAI[0] < 5f) || (aiStyle == 137 && ai[0] != 0f) || aiStyle == 138 || (type == 261 && velocity.Length() < 1.5f) || (type == 818 && ai[0] < 1f) || type == 831 || type == 970 || (type == 833 && ai[0] == 4f) || (type == 834 && ai[0] == 4f) || (type == 835 && ai[0] == 4f) || (type == 281 && ai[0] == -3f) || ((type == 598 || type == 636 || type == 614 || type == 971 || type == 975) && ai[0] == 1f) || (type == 923 && localAI[0] <= 60f) || (type == 919 && localAI[0] <= 60f) || (aiStyle == 15 && ai[0] == 0f && localAI[1] <= 12f) || type == 861 || (type >= 511 && type <= 513 && ai[1] >= 1f) || type == 1007 || (aiStyle == 93 && ai[0] != 0f && ai[0] != 2f) || (aiStyle == 10 && localAI[1] == -1f) || (type == 85 && localAI[0] >= 54f) || (Main.projPet[type] && type != 266 && type != 407 && type != 317 && (type != 388 || ai[0] != 2f) && (type < 390 || type > 392) && (type < 393 || type > 395) && (type != 533 || !(ai[0] >= 6f) || !(ai[0] <= 8f)) && (type < 625 || type > 628) && (type != 755 || ai[0] == 0f) && (type != 946 || ai[0] == 0f) && type != 758 && type != 951 && type != 963 && (type != 759 || frame == Main.projFrames[type] - 1) && type != 833 && type != 834 && type != 835 && type != 864 && (type != 623 || ai[0] != 2f)))
                    {
                        vanillaCanDamage = false;
                    }

                    if (ProjectileLoader.CanDamage(self) ?? vanillaCanDamage)
                    {
                        var hitbox = (Rectangle)damage_GetHitbox.Invoke(self, []);
                        PopProjectiles(p => self.Colliding(hitbox, p.Hitbox) || p.Colliding(p.Hitbox, hitbox));
                    }
                }

                orig.Invoke(self);
            };
        }

        private static void PopProjectiles(Predicate<Projectile> pred)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (Poppable.Contains(proj.type) && pred.Invoke(proj))
                {
                    proj.Kill();
                }
            }
        }
    }
}
