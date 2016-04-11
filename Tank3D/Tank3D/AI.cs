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
        const float EST_PROCHE = 35f;
        const int DÉLAI_MOUVEMENT = 5;
        const int DÉLAI_TIR = 71;
        // const float INCRÉMENT_ROTATION = 0.05f;

        Joueur Cible { get; set; }
        bool estDétruit { get; set; }
        int NuméroAI { get; set; }
        float Distance { get; set; }
        int CompteurTir { get; set; }
        int CompteurMouvement { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector2 ObjectifAnglesNormales { get; set; }
        float Orientation { get; set; }

        public AI(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible, int numéroAI)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cible = cible;
            CompteurTir = 0;
            CompteurMouvement = 0;
            Orientation = 0;
            NuméroAI = numéroAI;
        }
        public override void Update(GameTime gameTime)
        {
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Distance = new Vector2(Position.X - Cible.Coordonnées.X, Position.Z - Cible.Coordonnées.Y).Length();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (Distance <= EST_PROCHE)
                {
                    if (CompteurTir % DÉLAI_TIR == 0)
                    {
                        GestionProjectile();
                    }
                    GestionMouvements(false);
                }
                else
                {
                    GestionMouvements(true);
                }

                ++CompteurTir;
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        public bool EstDétruit()
        {
            estDétruit = false;
            return estDétruit;
        }

        #region Méthodes pour les mouvements
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Game, "Projectile", 0.1f, Rotation, 
                                            new Vector3(Position.X, Position.Y + 4f, Position.Z), IntervalleMAJ, 2f, 0.02f, false);
            Game.Components.Add(ProjectileTank);
        }
        
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected void GestionMouvements(bool seDéplace)
        {
            ++CompteurMouvement;
            if (CompteurMouvement % DÉLAI_MOUVEMENT == 0)
            {
                // Recalcul de la rotation
                Orientation = CalculOrientation(Cible.Coordonnées);
                ModificationParamètres(Orientation, seDéplace);
            }
            else
            {
                // Déplacement normal
                ModificationParamètres(Orientation, seDéplace);
            }
            
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
                    coeff = 3f * MathHelper.PiOver2;
                }
            }

            // (orientation >= INCRÉMENT_ROTATION) ? INCRÉMENT_ROTATION : orientation;
            return coeff + (float)Math.Atan(direction.X / direction.Y);
        }

        void ModificationParamètres(float orientation, bool seDéplace)
        {
            
            if(seDéplace)
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
                    //ObjectifAnglesNormales = GestionnaireDeNormales.GetNormale(nouvellesCoords, Rotation.Y);
                    //Rotation = new Vector3(ObjectifAnglesNormales.Y, Rotation.Y, ObjectifAnglesNormales.X);
                }
            }

            Rotation = new Vector3(Rotation.X, orientation, Rotation.Z);

            CalculerMonde();
        }
        #endregion
    }
}