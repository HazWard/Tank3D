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
    public class Joueur : ModèleMobile
    {
        // Constantes
        protected const float DISTANCE_POURSUITE = 15f;
        protected const float HAUTEUR_CAM_DÉFAULT = 5f;
        
        // Propriétés
        InputManager GestionInput { get; set; }
        CaméraSubjective Caméra { get; set; }

        public Joueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Caméra = new CaméraSubjective(jeu, new Vector3(0, 24, 140), positionInitiale, Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Caméra);
            Game.Services.AddService(typeof(Caméra), Caméra);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        public Vector2 Coordonnées
        {
            get
            {
                return new Vector2(Position.X, Position.Z);
            }
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
            int déplacement = GérerTouche(Keys.W) - GérerTouche(Keys.S);
            int rotation = GérerTouche(Keys.D) - GérerTouche(Keys.A);
            if (déplacement != 0 || rotation != 0)
            {
                ModificationParamètres(déplacement, rotation);
            }
        }

        void ModificationParamètres(int déplacement, int rotation)
        {
            float rotationFinal = Rotation.Y - IncrémentAngleRotation * rotation;
            float posX = déplacement * (float)Math.Sin(rotationFinal);
            float posY = déplacement * (float)Math.Cos(rotationFinal);

            Vector2 déplacementFinal = new Vector2(posX, posY);
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            Position = new Vector3(Position.X - déplacementFinal.X, Position.Y, Position.Z - déplacementFinal.Y);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            Point nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0,posZFinal));
            
            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
                HauteurTerrain = TerrainJeu.GetHauteur(Position);
                Caméra.Cible = Position;
                Caméra.Position = new Vector3(((float)Math.Sin(rotationFinal) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_DÉFAULT, ((float)Math.Cos(rotationFinal) * DISTANCE_POURSUITE) + Position.Z);

            }

            CalculerMonde();
        }

        bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;

            return estHorsDesBornes;
        }

        int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }
        #endregion
    }
}
