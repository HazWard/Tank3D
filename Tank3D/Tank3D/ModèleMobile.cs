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
    class ModèleMobile : ObjetDeBase, IActivable
    {
        const float INCRÉMENT_DÉPLACEMENT = 0.2f;
        const float HAUTEUR_DÉFAULT = 1f;
        const float DISTANCE_POURSUITE = 15f;
        const float HAUTEUR_CAM_DÉFAULT = 5f;

        Vector3 positionCaméraSubjective = new Vector3(0, 15, 115);

        float HauteurTerrain { get; set; }
        CaméraSubjective Caméra { get; set; }     
        Vector3 RotationInitiale { get; set; }
        Vector3 PositionInitiale { get; set; }
        float ÉchelleInitiale { get; set; }
        float IntervalleMAJ { get; set; }
        float IncrémentAngleRotation { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        Terrain TerrainJeu { get; set; }


        public ModèleMobile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            ÉchelleInitiale = échelleInitiale;
            RotationInitiale = rotationInitiale;
            PositionInitiale = positionInitiale;
            IntervalleMAJ = intervalleMAJ;
            Caméra = new CaméraSubjective(jeu, positionCaméraSubjective, PositionInitiale, Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Caméra);
            Game.Services.AddService(typeof(Caméra), Caméra);
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            IncrémentAngleRotation = 2 * (MathHelper.PiOver2 * IntervalleMAJ);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            TerrainJeu = Game.Services.GetService(typeof(Terrain)) as Terrain;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionTouches();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        #region Méthodes pour la gestion des déplacements et rotations du modèle
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        /// <summary>
        /// Méthode servant à gérer les touches entrées par l'utilisateur
        /// </summary>
        void GestionTouches()
        {
            int déplacement = GérerTouche(Keys.W) - GérerTouche(Keys.S);
            int rotation = GérerTouche(Keys.D) - GérerTouche(Keys.A);
            if (déplacement != 0 || rotation != 0)
            {
                ModificationParamètres(déplacement, rotation);
            }
        }

        /// <summary>
        /// Méthode modifiant la position et la rotation du modèle
        /// </summary>
        /// <param name="déplacement">Déplacement sur les axes X et Z</param>
        /// <param name="rotation">Rotation sur l'axe des Y (Lacet)</param>
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

            Point nouvellesCoords = ConvertionCordonnées(posXFinal, posZFinal);
            HauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);

                Caméra.Cible = Position;
                Caméra.Position = new Vector3(((float)Math.Sin(rotationFinal) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_DÉFAULT, ((float)Math.Cos(rotationFinal) * DISTANCE_POURSUITE) + Position.Z);
            }
            // Console.WriteLine("X: {0}, Y:{1}, Z: {2}", Position.X, Position.Y, Position.Z);
            CalculerMonde();
        }

        private bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;
            
            return estHorsDesBornes;
        }

        Point ConvertionCordonnées(float coordX, float coordZ)
        {
            int x = (int)Math.Abs((coordX + TerrainJeu.Étendue.X / 2) / TerrainJeu.Delta.X);
            int y = (int)Math.Abs((coordZ - TerrainJeu.Étendue.Z / 2) / TerrainJeu.Delta.Z);

            return new Point(x, y);
        }

        /// <summary>
        /// Méthode vérifiant l'état d'une touche
        /// </summary>
        /// <param name="touche">Touche enfoncée</param>
        /// <returns>L'état de la touche (V/F)</returns>
        int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? 1 : 0;
        }

        /// <summary>
        /// Méthode modifiant l'activation (Pause)
        /// </summary>
        public void ModifierActivation()
        {
            base.Enabled = !base.Enabled;
        }
        #endregion
    }
}
