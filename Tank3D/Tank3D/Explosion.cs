using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{

    public class Explosion : ModèleMobile
    {
        float TempsÉcouléTotal { get; set; }
        const float INTERVALLE_EXPLOSION = 0.4f;
        const float VITESSE_EXPLOSION = 0.0001f;
        Vector3 PositionInitiale { get; set; }

        const float AccélérationGravitationelle = -0.98f;
        const float VitesseDépart = 0.2f;
        float IncrémentDéplacementProjectile { get; set; }
        float IncrémentHauteurProjectile { get; set; }

        float HauteurProj { get; set; }

        public Explosion(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            PositionInitiale = positionInitiale;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            TempsÉcouléTotal = 0;
            Position = PositionInitiale;
            HauteurProj = 0.7f;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            IncrémentDéplacementProjectile = ((float)Math.Cos(Rotation.X) * VitesseDépart) * 2;
            IncrémentHauteurProjectile = ((float)Math.Sin(Rotation.X) * VitesseDépart) / 2;
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            TempsÉcouléTotal += (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvement();
                if(TempsÉcouléTotal >= INTERVALLE_EXPLOSION)
                {
                    EffacerExplosion();
                    TempsÉcouléTotal = 0;
                }
                
                TempsÉcouléDepuisMAJ = 0;
                CalculerMonde();
            }
            
            base.Update(gameTime);
        }
        void GestionMouvement()
        {
            float posX = IncrémentDéplacementProjectile * (float)Math.Sin(Rotation.Y);
            float posY = IncrémentDéplacementProjectile * (float)Math.Cos(Rotation.Y);

            //if(posY>Hauteur.GetHauteur)
            Vector2 déplacementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            GestionForces();
            Position = new Vector3(posXFinal, Position.Y + HauteurProj, posZFinal);
            Rotation = new Vector3(Rotation.X - 0.01f, Rotation.Y, Rotation.Z + 0.2f);

            //Position = new Vector3(Position.X, Position.Y + 0.05f, Position.Z);
        }
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }
        void EffacerExplosion()
        {
            Game.Components.Remove(this);
        }
        void GestionForces()
        {
            HauteurProj -= 0.04f;
            IncrémentHauteurProjectile += 0.02f;
        }
    }
}
