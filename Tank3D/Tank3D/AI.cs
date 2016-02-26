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
        const float INCRÉMENT_DÉPLACEMENT_AI = 0.1f;
        const float EST_PROCHE = 50f;
        

        Joueur Cible { get; set; }
        bool estDétruit { get; set; }
        float Distance { get; set; }
        int Compteur { get; set; }
        ModèleMobile ProjectileTank { get; set; }
        Game Jeu { get; set; }
        public Vector3 GetPosition
        {
            get
            {
                return Position;
            }
            private set { }
        }
        public Vector3 GetRotation
        {
            get
            {
                return Rotation;
            }
            private set { }
        }

        public AI(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Jeu = jeu;
            Cible = cible;
            Compteur = 0;
        }
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;

            Distance = new Vector2(Position.X - Cible.Coordonnées.X, Position.Z - Cible.Coordonnées.Y).Length();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (Distance <= EST_PROCHE && Compteur % 100 == 0)
                {
                    GestionProjectile();
                }
                
                GestionMouvements();
                Compteur++;
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        public bool EstDétruit()
        {
            estDétruit = false;
            if (Compteur == 1000)
            {
                estDétruit = true;
            }
            return estDétruit;
        }

        #region Méthodes pour la gestion des déplacements et rotations du modèle
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Jeu, "Projectile", 0.1f, Rotation, 
                                            new Vector3(Position.X, Position.Y + 4f, Position.Z), IntervalleMAJ);
            Game.Components.Add(ProjectileTank);
        }
        
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected void GestionMouvements()
        {
            ModificationParamètres(CalculOrientation(Cible.Coordonnées));
        }

        float CalculOrientation(Vector2 cible)
        {
            Vector2 direction = Vector2.Normalize(new Vector2(Position.X - cible.X, Position.Z - cible.Y));
            float coeff = 0;
            if (direction.X <= 0)
            {
                if (direction.Y <= 0)
                {
                    coeff = MathHelper.Pi;
                }
                else
                {
                    coeff = MathHelper.PiOver2;
                }
            }
            else
            {
                if (direction.Y <= 0)
                {
                    coeff = 3 * MathHelper.PiOver2;
                }
            }
            float orientation = coeff + (float)Math.Atan(direction.X / direction.Y);

            return orientation;
        }

        void ModificationParamètres(float orientation)
        {
            float posX = INCRÉMENT_DÉPLACEMENT_AI * (float)Math.Sin(orientation);
            float posY = INCRÉMENT_DÉPLACEMENT_AI * (float)Math.Cos(orientation);
            Vector2 déplacementFinal = new Vector2(posX, posY);

            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
            }

            Rotation = new Vector3(Rotation.X, orientation, Rotation.Z);

            CalculerMonde();
        }
        #endregion
    }
}