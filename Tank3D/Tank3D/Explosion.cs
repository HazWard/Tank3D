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

    public class Explosion : Mod�leMobile
    {
        float Temps�coul�Total { get; set; }
        const float INTERVALLE_EXPLOSION = 0.4f;
        const float VITESSE_EXPLOSION = 0.0001f;
        Vector3 PositionInitiale { get; set; }

        const float Acc�l�rationGravitationelle = -0.98f;
        const float VitesseD�part = 0.2f;
        float Incr�mentD�placementProjectile { get; set; }
        float Incr�mentHauteurProjectile { get; set; }

        float HauteurProj { get; set; }

        public Explosion(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            PositionInitiale = positionInitiale;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = 0;
            Temps�coul�Total = 0;
            Position = PositionInitiale;
            HauteurProj = 0.7f;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            Incr�mentD�placementProjectile = ((float)Math.Cos(Rotation.X) * VitesseD�part) * 2;
            Incr�mentHauteurProjectile = ((float)Math.Sin(Rotation.X) * VitesseD�part) / 2;
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            Temps�coul�Total += (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvement();
                if(Temps�coul�Total >= INTERVALLE_EXPLOSION)
                {
                    EffacerExplosion();
                    Temps�coul�Total = 0;
                }
                
                Temps�coul�DepuisMAJ = 0;
                CalculerMonde();
            }
            
            base.Update(gameTime);
        }
        void GestionMouvement()
        {
            float posX = Incr�mentD�placementProjectile * (float)Math.Sin(Rotation.Y);
            float posY = Incr�mentD�placementProjectile * (float)Math.Cos(Rotation.Y);

            //if(posY>Hauteur.GetHauteur)
            Vector2 d�placementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - d�placementFinal.X;
            float posZFinal = Position.Z - d�placementFinal.Y;

            GestionForces();
            Position = new Vector3(posXFinal, Position.Y + HauteurProj, posZFinal);
            Rotation = new Vector3(Rotation.X - 0.01f, Rotation.Y, Rotation.Z + 0.2f);

            //Position = new Vector3(Position.X, Position.Y + 0.05f, Position.Z);
        }
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
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
            Incr�mentHauteurProjectile += 0.02f;
        }
    }
}
