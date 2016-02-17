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
    public class AI : Mod�leMobile
    {
        Joueur Cible { get; set; }
        float Orientation { get; set; }

        public AI(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
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


        #region M�thodes pour la gestion des d�placements et rotations du mod�le
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected override void GestionMouvements()
        {
            Orientation = AjustementD�placements(Cible.Coordonn�es);
            ModificationParam�tres();
        }

        float AjustementD�placements(Vector2 cible)
        {
            Vector2 direction = Vector2.Normalize(new Vector2(Position.X - cible.X, Position.Z - cible.Y));
            float orientation = (float)Math.Atan(direction.Y / direction.X);
            return orientation;
        }

        void ModificationParam�tres()
        {
            float posX = 0 * (float)Math.Sin(Orientation);
            float posY = 1 * (float)Math.Cos(Orientation);
            Vector2 d�placementFinal = new Vector2(posX, posY) * 2f;

            float posXFinal = Position.X - d�placementFinal.X;
            float posZFinal = Position.Z - d�placementFinal.Y;

            nouvellesCoordonn�es = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0, posZFinal)));

            // V�rification de la future position
            if (!EstHorsDesBornes(nouvellesCoordonn�es))
            {
                HauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoordonn�es);
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
            }

            Rotation = new Vector3(Rotation.X, Orientation, Rotation.Z);

            CalculerMonde();
        }
        #endregion
    }
}
