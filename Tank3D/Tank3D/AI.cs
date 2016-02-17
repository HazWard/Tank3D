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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AI : ModèleMobile
    {
        Joueur Cible { get; set; }
        float Orientation { get; set; }

        public AI(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cible = cible;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
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
            Orientation = AjustementDéplacements(Cible.Coordonnées);
            ModificationParamètres();
        }

        float AjustementDéplacements(Vector2 cible)
        {
            Vector2 direction = Vector2.Normalize(new Vector2(Position.X - cible.X, Position.Z - cible.Y));
            float orientation = (float)Math.Atan(direction.Y / direction.X);
            return orientation;
        }

        void ModificationParamètres()
        {
            float posX = 0 * (float)Math.Sin(Orientation);
            float posY = 1 * (float)Math.Cos(Orientation);
            Vector2 déplacementFinal = new Vector2(posX, posY) * 2f;

            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            nouvellesCoordonnées = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal)));

            // Vérification de la future position
            if (!EstHorsDesBornes(nouvellesCoordonnées))
            {
                HauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoordonnées);
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
            }

            Rotation = new Vector3(Rotation.X, Orientation, Rotation.Z);

            CalculerMonde();
        }
        #endregion
    }
}
