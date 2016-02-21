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
        const float INCRÉMENT_DÉPLACEMENT_PROJECTILE = 0.5f;

        public Projectile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {

        }

        public bool EstDétruit()
        {
            bool estDétruit = false;
            if (Position.Y < TerrainJeu.GetHauteur(new Point((int)Position.X, (int)Position.Z)))
            {
                estDétruit = true;
            }
            if (EstHorsDesBornes(new Point((int) Position.X, (int) Position.Z)))
            {
                estDétruit = true;
            }
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

        protected override void GestionMouvements()
        {
            ModificationParamètres();
        }

        void ModificationParamètres()
        {
            float posX = INCRÉMENT_DÉPLACEMENT_PROJECTILE * (float)Math.Sin(Rotation.Y);
            float posY = INCRÉMENT_DÉPLACEMENT_PROJECTILE * (float)Math.Cos(Rotation.Y);
            Vector2 déplacementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;
            
            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, Position.Y, posZFinal);
                Rotation = new Vector3(Rotation.X, Rotation.Y, Rotation.Z + 0.2f);
            }
            //Position = new Vector3(posXFinal, Position.Y, posZFinal);
            //Rotation = new Vector3(Rotation.X, Rotation.Y, Rotation.Z + 0.2f);
            CalculerMonde();
        }
        #endregion
    }
}
