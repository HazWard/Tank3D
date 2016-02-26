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
        const float EST_PROCHE = 50f;
        

        Joueur Cible { get; set; }
        bool estD�truit { get; set; }
        float Distance { get; set; }
        int Compteur { get; set; }
        Mod�leMobile ProjectileTank { get; set; }
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

        public AI(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Jeu = jeu;
            Cible = cible;
            Compteur = 0;
        }
        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;

            Distance = new Vector2(Position.X - Cible.Coordonn�es.X, Position.Z - Cible.Coordonn�es.Y).Length();
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (Distance <= EST_PROCHE && Compteur % 100 == 0)
                {
                    GestionProjectile();
                }
                
                GestionMouvements();
                Compteur++;
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        public bool EstD�truit()
        {
            estD�truit = false;
            if (Compteur == 1000)
            {
                estD�truit = true;
            }
            return estD�truit;
        }

        #region M�thodes pour la gestion des d�placements et rotations du mod�le
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Jeu, "Projectile", 0.1f, Rotation, 
                                            new Vector3(Position.X, Position.Y + 4f, Position.Z), IntervalleMAJ);
            Game.Components.Add(ProjectileTank);
        }
        
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected void GestionMouvements()
        {
            ModificationParam�tres(CalculOrientation(Cible.Coordonn�es));
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

        void ModificationParam�tres(float orientation)
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
            }

            Rotation = new Vector3(Rotation.X, orientation, Rotation.Z);

            CalculerMonde();
        }
        #endregion
    }
}