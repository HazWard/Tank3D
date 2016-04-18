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
    public class AI : Mod�leMobile, IActivable, IModel
    {
        const float INCR�MENT_D�PLACEMENT_AI = 0.1f;
        const float EST_PROCHE = 35f;
        const int HAUTEUR_D�FAULT = 1;
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
        int Compteur { get; set; }
        Game Jeu { get; set; }
        public BarreDeVie VieAI { get; set; }
        float PourcentageVie { get; set; }
        int CompteurCollision { get; set; }

        public Vector3 GetRotation
        {
            get
            {
                return Rotation;
            }
            private set { }
        }

        public AI(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible, int num�roAI)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cible = cible;
            CompteurTir = 0;
            CompteurMouvement = 0;
            Orientation = 0;
            Num�roAI = num�roAI;
            Compteur = 0;
            VieAI = new BarreDeVie(jeu, �chelleInitiale, rotationInitiale, new Vector3(positionInitiale.X, positionInitiale.Y + 15, positionInitiale.Z) , new Vector2(100, 17), new Vector2(5, 10), "FondInstructions", IntervalleMAJ);
            Game.Components.Add(VieAI);
        }
        public override void Initialize()
        {
            PourcentageVie = 1;
            SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
            CompteurCollision = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Temps�coul�DepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Distance = new Vector2(Position.X - Cible.Coordonn�es.X, Position.Z - Cible.Coordonn�es.Y).Length();
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (EstEnCollision)
                {
                    Recule();
                    EstEnCollision = false;
                }
                else
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
                }
                
                if (EstD�truit)
                {
                    Game.Components.Add(new ObjetDeBase(Game, "Veteran Tiger Forest", 0.05f, Vector3.Zero, Position));
                    Game.Components.Remove(this);
                }

                ++CompteurTir;

                Compteur++;
                Temps�coul�DepuisMAJ = 0;

                CalculBarreDeVie();
                //VieAI.CalculerVie();

            }
            base.Update(gameTime);
        }
        void CalculBarreDeVie()
        {
            VieAI.Position = new Vector3(Position.X, Position.Y + 7, Position.Z);
            //VieAI.AngleLacet = Rotation.Y;
            VieAI.PositionJoueur = Cible.GetPosition;

            VieAI.PourcentageVie = PourcentageVie;

            //VieJoueur.CalculerVie(RotationPitchCanon, RotationYawTour, PositionTour);
        }
        public void ModifierActivation()
        {

        }

        #region M�thodes pour les mouvements
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Game, "Projectile", 0.1f, Rotation,
                                            new Vector3(Position.X, Position.Y + 4f, Position.Z), IntervalleMAJ, 2f, 0.02f, false);

            Game.Components.Add(ProjectileTank);
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
            if (seD�place)
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
                    SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
                }

                Rotation = new Vector3(Rotation.X, orientation, Rotation.Z);

                if (EstD�truit)
                {
                    Game.Components.Remove(this);
                }
                CalculerMonde();
            }
        #endregion
        }

        void Recule()
        {
            Position = new Vector3(Position.X, Position.Y, Position.Z + 1f);
            CalculerMonde();
        }
    }
}
