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
        const float INCR�MENT_D�PLACEMENT_AI = 0.1f;
        const float EST_PROCHE = 35f;
        const int D�LAI_MOUVEMENT = 5;
        const int D�LAI_TIR = 71;
        // const float INCR�MENT_ROTATION = 0.05f;

        Joueur Cible { get; set; }
        bool estD�truit { get; set; }
        int Num�roAI { get; set; }
        float Distance { get; set; }
        int CompteurTir { get; set; }
        int CompteurMouvement { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector2 ObjectifAnglesNormales { get; set; }
        float Orientation { get; set; }

        public AI(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible, int num�roAI)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cible = cible;
            CompteurTir = 0;
            CompteurMouvement = 0;
            Orientation = 0;
            Num�roAI = num�roAI;
        }
        public override void Update(GameTime gameTime)
        {
            Temps�coul�DepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Distance = new Vector2(Position.X - Cible.Coordonn�es.X, Position.Z - Cible.Coordonn�es.Y).Length();
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (Distance <= EST_PROCHE)
                {
                    if (CompteurTir % D�LAI_TIR == 0)
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
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        public bool EstD�truit()
        {
            estD�truit = false;
            return estD�truit;
        }

        #region M�thodes pour les mouvements
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Game, "Projectile", 0.1f, Rotation, 
                                            new Vector3(Position.X, Position.Y + 4f, Position.Z), IntervalleMAJ, 2f, 0.02f, false);
            Game.Components.Add(ProjectileTank);
        }
        
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected void GestionMouvements(bool seD�place)
        {
            ++CompteurMouvement;
            if (CompteurMouvement % D�LAI_MOUVEMENT == 0)
            {
                // Recalcul de la rotation
                Orientation = CalculOrientation(Cible.Coordonn�es);
                ModificationParam�tres(Orientation, seD�place);
            }
            else
            {
                // D�placement normal
                ModificationParam�tres(Orientation, seD�place);
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

            // (orientation >= INCR�MENT_ROTATION) ? INCR�MENT_ROTATION : orientation;
            return coeff + (float)Math.Atan(direction.X / direction.Y);
        }

        void ModificationParam�tres(float orientation, bool seD�place)
        {
            
            if(seD�place)
            {
                float posX = INCR�MENT_D�PLACEMENT_AI * (float)Math.Sin(orientation);
                float posY = INCR�MENT_D�PLACEMENT_AI * (float)Math.Cos(orientation);
                Vector2 d�placementFinal = new Vector2(posX, posY);

                float posXFinal = Position.X - d�placementFinal.X;
                float posZFinal = Position.Z - d�placementFinal.Y;

                nouvellesCoords = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0, posZFinal));

                if (!EstHorsDesBornes(nouvellesCoords))
                {
                    NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                    Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
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