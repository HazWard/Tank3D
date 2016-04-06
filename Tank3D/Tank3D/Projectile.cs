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
    public class Projectile:ModèleMobile
    {
        bool SeDésintègre { get; set; }
        int Compteur { get; set; }
        float VitesseDépart { get; set; }
        float DeltaHauteur { get; set; }
        float IncrémentDéplacementProjectile { get; set; }
        float IncrémentHauteurProjectile { get; set; }
        
        Terrain Hauteur { get; set; }

        public Projectile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, float vitesseInitiale, float deltaHauteur, bool seDésintègre)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            VitesseDépart = vitesseInitiale;
            DeltaHauteur = deltaHauteur;
            SeDésintègre = seDésintègre;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvements();
                TempsÉcouléDepuisMAJ = 0;
                Compteur++;
            }
            base.Update(gameTime);
        }

        public override void Initialize()
        {
            Compteur = 0;
            IncrémentDéplacementProjectile = ((float)Math.Cos(Rotation.X) * VitesseDépart) * 2;
            IncrémentHauteurProjectile = ((float)Math.Sin(Rotation.X) * VitesseDépart) / 2;
            base.Initialize();
        }

        public bool EstDétruit()
        {
            bool estDétruit = false;
            return estDétruit;
        }

        #region Méthodes pour la gestion des déplacements et rotations du modèle
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

            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, déplacementFinal.Y, posZFinal));
            float hauteurMinimale = TerrainJeu.GetHauteur(nouvellesCoords);
            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, Position.Y + IncrémentHauteurProjectile, posZFinal);
                if (SeDésintègre)
                {
                    Rotation = new Vector3(Rotation.X - 0.05f, Rotation.Y, Rotation.Z + 0.2f);
                }
                else
                {
                    Rotation = new Vector3(Rotation.X - 0.01f, Rotation.Y, Rotation.Z + 0.2f);
                }
            }

            EffacerProjectile(EstHorsDesBornes(nouvellesCoords),posXFinal,posZFinal,déplacementFinal, hauteurMinimale);

            CalculerMonde();
        }

        private void EffacerProjectile(bool sortie, float posX,float posZ, Vector2 déplacementFinale, float hauteurMin)
        {
            if (Position.Y <= 0 || sortie || Position.Y <= hauteurMin)
            {
                Game.Components.Remove(this);
                float angleExplosion = 0;
                for (int i = 0; i < 7; ++i)
                {
                    Game.Components.Add(new Explosion(Game, "Projectile", 0.1f, new Vector3(0,angleExplosion,0), new Vector3(posX, hauteurMin, posZ), IntervalleMAJ));
                    angleExplosion += MathHelper.PiOver4;
                }
            }
        }

        void GestionForces()
        {
            IncrémentHauteurProjectile -= DeltaHauteur;
        }
        #endregion
    }
}
