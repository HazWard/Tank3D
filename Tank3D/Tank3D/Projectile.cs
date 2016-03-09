using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AtelierXNA
{
    class Projectile:ModèleMobile
    {
        const float AccélérationGravitationelle = -0.98f;
        const float VitesseDépart = 2f;
        float IncrémentDéplacementProjectile { get; set; }
        float IncrémentHauteurProjectile { get; set; }
        PlanExplosion ExplosionUn { get; set; }
        PlanExplosion ExplosionDeux { get; set; }

        public Projectile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {

        }
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvements();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            IncrémentDéplacementProjectile = ((float)Math.Cos(Rotation.X) * VitesseDépart) * 2;
            IncrémentHauteurProjectile = ((float)Math.Sin(Rotation.X) * VitesseDépart) / 2;
            ExplosionUn = new PlanExplosion(Game, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(0, 6, -126), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", 0f);
            ExplosionDeux = new PlanExplosion(Game, 1f, Vector3.Zero, new Vector3(0, 6, -126), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", 0f);

            base.LoadContent();
        }

        public bool EstDétruit()
        {
            bool estDétruit = false;
            return estDétruit;
        }

        #region Méthodes pour la gestion des déplacements et rotations du modèle
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected void GestionMouvements()
        {
            ModificationParamètres();
        }

        void ModificationParamètres()
        {
            float posX = IncrémentDéplacementProjectile * (float)Math.Sin(Rotation.Y);
            float posY = IncrémentDéplacementProjectile * (float)Math.Cos(Rotation.Y);
            Vector2 déplacementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;
            
            GestionForces();

            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, Position.Y + IncrémentHauteurProjectile, posZFinal);
                Rotation = new Vector3(Rotation.X - 0.01f, Rotation.Y, Rotation.Z + 0.2f);
            }

            EffacerProjectile(EstHorsDesBornes(nouvellesCoords), nouvellesCoords,posXFinal,posZFinal);

            CalculerMonde();
        }

        void EffacerProjectile(bool sortie, Point coords,float posX,float posZ)
        {
            if (sortie || Position.Y <= TerrainJeu.GetHauteur(coords))
            {
                Game.Components.Remove(this);
                //Game.Components.Add(new PlanExplosion(Game, 0.8f, Vector3.Zero, new Vector3(posX,0,posZ), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", 0f));
                //Game.Components.Add(new PlanExplosion(Game, 0.8f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(posX, 0, posZ), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", 0f));
                //Game.Components.Add(ExplosionDeux);
                
                Console.WriteLine("Projectile effacé!");
            }
        }

        void GestionForces()
        {
            IncrémentHauteurProjectile -= 0.02f;
        }
        #endregion
    }
}
