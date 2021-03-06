// Auteurs: Starly Solon
// Fichier: AI.cs
// Date de cr�ation: 16 f�vrier 2016
// Description: Classe s'occupant de la gestion du d�placement 
//              et des habilit�s des ennemis (intelligence
//              artificielle)

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
    public class AI : Mod�leMobile, IActivable, IModel
    {
        const float INCR�MENT_D�PLACEMENT_AI = 0.1f;
        const float EST_PROCHE = 35f;
        const int HAUTEUR_D�FAULT = 1;
        const int D�LAI_MOUVEMENT = 5;
        const int D�LAI_TIR = 71;
        Vector2 �TENDUE = new Vector2(100,17);
        Joueur Cible { get; set; }
        GestionnaireEnnemis GestionEnnemis { get; set; }
        int Num�roAI { get; set; }
        float Distance { get; set; }
        int CompteurTir { get; set; }
        int CompteurMouvement { get; set; }
        Projectile ProjectileTank { get; set; }
        float Orientation { get; set; }
        int Compteur { get; set; }
        Game Jeu { get; set; }
        public BarreDeVie VieAI { get; set; }
        float PourcentageVie { get; set; }
        int CompteurCollision { get; set; }

        public AI(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible, int num�roAI, GestionnaireEnnemis gestionEnnemis)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cible = cible;
            CompteurTir = 0;
            CompteurMouvement = 0;
            Orientation = 0;
            Num�roAI = num�roAI;
            Compteur = 0;
            Jeu = jeu;

            VieAI = new BarreDeVie(jeu, �chelleInitiale, rotationInitiale, new Vector3(positionInitiale.X, positionInitiale.Y + 15, positionInitiale.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleCompl�te", IntervalleMAJ);
            GestionEnnemis = gestionEnnemis;

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
                if (!EstEnCollision)
                {
                    if (Distance <= EST_PROCHE)
                    {
                        if (CompteurTir % D�LAI_TIR == 0)
                        {
                            GestionProjectile();
                        }
                        GestionMouvements(false, "non");
                    }
                    else
                    {
                        
                        GestionMouvements(true, "non");
                    }
                }
                else
                {
                    GestionMouvements(true, "oui");
                }

                if (A�t�Tir�)
                {

                    PourcentageVie -= 0.25f;
                    if (PourcentageVie == 0.75f)
                    {
                        Game.Components.Remove(VieAI);
                        VieAI = new BarreDeVie(Jeu, �chelle, Rotation, new Vector3(Position.X, Position.Y + 15, Position.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleTroisQuart", IntervalleMAJ);
                        Game.Components.Add(VieAI);
                    }
                    if (PourcentageVie == 0.50f)
                    {
                        Game.Components.Remove(VieAI);
                        VieAI = new BarreDeVie(Jeu, �chelle, Rotation, new Vector3(Position.X, Position.Y + 15, Position.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleDemie", IntervalleMAJ);
                        Game.Components.Add(VieAI);
                    }
                    if (PourcentageVie == 0.25f)
                    {
                        Game.Components.Remove(VieAI);
                        VieAI = new BarreDeVie(Jeu, �chelle, Rotation, new Vector3(Position.X, Position.Y + 15, Position.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleQuart", IntervalleMAJ);
                        Game.Components.Add(VieAI);
                    }
                    
                    if (PourcentageVie <= 0)
                    {
                        EstD�truit = true;
                    }
                    A�t�Tir� = false;
                }

                if (EstD�truit)
                {
                    Cible.Score++;
                    GestionEnnemis.DoitCr�er = true;
                    Game.Components.Add(new TankD�truit(Game, "Veteran Tiger Destroyed", 0.05f, Rotation, Position));
                    Game.Components.Remove(VieAI);
                    Game.Components.Remove(this);
                }

                ++CompteurTir;

                Compteur++;
                Temps�coul�DepuisMAJ = 0;

                CalculBarreDeVie();
                

            }
            base.Update(gameTime);
        }

        /// <summary>
        /// CalculerBarreDeVie donne la position de l'Ai et la position du joueur � la classe BarreDeVie
        /// </summary>
        void CalculBarreDeVie()
        {
            VieAI.Position = new Vector3(Position.X, Position.Y + 7, Position.Z);
            VieAI.PositionJoueur = Cible.GetPosition;

        }
        public void ModifierActivation() { }

        #region M�thodes pour les mouvements
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Game, "Projectile", 0.1f, Rotation,
                                            new Vector3(Position.X - 5 * (float)Math.Sin(Rotation.Y), Position.Y + 4.5f, Position.Z - 5 * (float)Math.Cos(Rotation.Y)), IntervalleMAJ, 2f, 0.02f, false, this);

            Game.Components.Add(ProjectileTank);
        }

        protected void GestionMouvements(bool seD�place, string collisionEffectu�e)
        {
            ++CompteurMouvement;
            switch (collisionEffectu�e)
            {
                case "oui":
                    ModificationParam�tres(Orientation, seD�place, collisionEffectu�e);
                    break;
                case "non":
                    if (CompteurMouvement % D�LAI_MOUVEMENT == 0)
                    {
                        // Recalcul de la rotation
                        Orientation = CalculOrientation(Cible.Coordonn�es);
                        ModificationParam�tres(Orientation, seD�place, collisionEffectu�e);
                    }
                    else
                    {
                        // D�placement normal
                        ModificationParam�tres(Orientation, seD�place, collisionEffectu�e);
                    }
                    break;
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

        void ModificationParam�tres(float orientation, bool seD�place, string collisionEffectu�e)
        {
            if (seD�place)
            {
                float posX = INCR�MENT_D�PLACEMENT_AI * (float)Math.Sin(orientation);
                float posY = INCR�MENT_D�PLACEMENT_AI * (float)Math.Cos(orientation);
                float posXFinal = 0;
                float posZFinal = 0;
                Vector2 d�placementFinal = new Vector2(posX, posY);

                switch (collisionEffectu�e)
                {
                    case "oui":
                        posXFinal = Position.X + d�placementFinal.X;
                        posZFinal = Position.Z + d�placementFinal.Y;
                        break;
                    case "non":
                        posXFinal = Position.X - d�placementFinal.X;
                        posZFinal = Position.Z - d�placementFinal.Y;
                        break;
                }

                nouvellesCoords = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0, posZFinal));

                if (!EstHorsDesBornes(nouvellesCoords))
                {
                    NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                    Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
                    SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
                }

                Rotation = new Vector3(Rotation.X, orientation, Rotation.Z);

                CalculerMonde();
            }
        #endregion
        }
    }
}
