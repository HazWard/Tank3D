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
    class ModèleMobile : ObjetDeBase
    {
        
        // Constantes
        const float FACTEUR_ACCÉLÉRATION = 1f / 60f;
        const int INCRÉMENT_DÉPLACEMENT = 1;
        
        // Propriétés
        Vector3 RotationInitiale { get; set; }
        Vector3 PositionInitiale { get; set; }
        float ÉchelleInitiale { get; set; }
        float Vitesse { get; set; }
        float IntervalleMAJ { get; set; }
        float IncrémentAngleRotation { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }

        public ModèleMobile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            ÉchelleInitiale = échelleInitiale;
            RotationInitiale = rotationInitiale;
            PositionInitiale = positionInitiale;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            Vitesse = 50f;
            IncrémentAngleRotation = (MathHelper.PiOver2 * IntervalleMAJ);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
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

        void GestionTouches()
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
            float posX = (déplacement * INCRÉMENT_DÉPLACEMENT) * (float)Math.Sin(rotationFinal);
            float posY = (déplacement * INCRÉMENT_DÉPLACEMENT) * (float)Math.Cos(rotationFinal);
            Vector2 déplacementFinal = new Vector2(posX, posY);

            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            Position = new Vector3(Position.X - déplacementFinal.X, Position.Y, Position.Z - déplacementFinal.Y);

            CalculerMonde();
        }

        int GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }
        #endregion
    }
}
