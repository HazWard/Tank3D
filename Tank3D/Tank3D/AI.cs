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
    public class AI : ModèleMobile, IActivable, IModel
    {
        const float INCRÉMENT_DÉPLACEMENT_AI = 0.1f;
        const float EST_PROCHE = 35f;
        const int HAUTEUR_DÉFAULT = 1;
        const int DÉLAI_MOUVEMENT = 5;
        const int DÉLAI_TIR = 71;
        Vector2 ÉTENDUE = new Vector2(100,17);
        // const float INCRÉMENT_ROTATION = 0.05f;
        Joueur Cible { get; set; }
        GestionnaireEnnemis GestionEnnemis { get; set; }
        bool estDétruit { get; set; }
        int NuméroAI { get; set; }
        float Distance { get; set; }
        int CompteurTir { get; set; }
        int CompteurMouvement { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector2 ObjectifAnglesNormales { get; set; }
        float Orientation { get; set; }
        int Compteur { get; set; }
        Game Jeu { get; set; }
        public BarreDeVie VieAI { get; set; }
        public BarreDeVie VieAIFond { get; set; }
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

        public AI(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ, Joueur cible, int numéroAI, GestionnaireEnnemis gestionEnnemis)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cible = cible;
            CompteurTir = 0;
            CompteurMouvement = 0;
            Orientation = 0;
            NuméroAI = numéroAI;
            Compteur = 0;
            Jeu = jeu;

            VieAI = new BarreDeVie(jeu, échelleInitiale, rotationInitiale, new Vector3(positionInitiale.X, positionInitiale.Y + 15, positionInitiale.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleComplète", IntervalleMAJ, 4);

            GestionEnnemis = gestionEnnemis;
           

            Game.Components.Add(VieAI);
            //VieAIFond = new BarreDeVie(jeu, échelleInitiale, rotationInitiale, new Vector3(positionInitiale.X + 3*(float)Math.Cos(Rotation.Y), positionInitiale.Y + 15, positionInitiale.Z + 3*(float)Math.Cos(Rotation.Y)), new Vector2(90, 14), new Vector2(1, 1), "FondVertBarreDeVie", IntervalleMAJ,4);
            //Game.Components.Add(VieAIFond);
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
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Distance = new Vector2(Position.X - Cible.Coordonnées.X, Position.Z - Cible.Coordonnées.Y).Length();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (!EstEnCollision)
                {
                    if (Distance <= EST_PROCHE)
                    {
                        if (CompteurTir % DÉLAI_TIR == 0)
                        {
                            GestionProjectile();
                        }
                        GestionMouvements(false, "pas");
                    }
                    else
                    {
                        
                        GestionMouvements(true, "pas");
                    }
                }
                else
                {
                    GestionMouvements(true, "collision");
                }

                if (AÉtéTiré)
                {

                    PourcentageVie -= 0.25f;
                    if (PourcentageVie == 0.75f)
                    {
                        Game.Components.Remove(VieAI);
                        VieAI = new BarreDeVie(Jeu, Échelle, Rotation, new Vector3(Position.X, Position.Y + 15, Position.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleTroisQuart", IntervalleMAJ, 4);
                        Game.Components.Add(VieAI);
                    }
                    if (PourcentageVie == 0.50f)
                    {
                        Game.Components.Remove(VieAI);
                        VieAI = new BarreDeVie(Jeu, Échelle, Rotation, new Vector3(Position.X, Position.Y + 15, Position.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleDemie", IntervalleMAJ, 4);
                        Game.Components.Add(VieAI);
                    }
                    if (PourcentageVie == 0.25f)
                    {
                        Game.Components.Remove(VieAI);
                        VieAI = new BarreDeVie(Jeu, Échelle, Rotation, new Vector3(Position.X, Position.Y + 15, Position.Z), new Vector2(100, 17), new Vector2(1, 1), "BarreDeVieRectangleQuart", IntervalleMAJ, 4);
                        Game.Components.Add(VieAI);
                    }
                    
                    if (PourcentageVie <= 0)
                    {
                        EstDétruit = true;
                    }
                    AÉtéTiré = false;
                }

                if (EstDétruit)
                {
                    Cible.Score++;
                    GestionEnnemis.DoitCréer = true;
                    Game.Components.Add(new TankDétruit(Game, "Veteran Tiger Destroyed", 0.05f, Rotation, Position));
                    Game.Components.Remove(VieAI);
                    Game.Components.Remove(this);
                }

                ++CompteurTir;

                Compteur++;
                TempsÉcouléDepuisMAJ = 0;

                CalculBarreDeVie();
                //VieAI.CalculerVie();

            }
            base.Update(gameTime);
        }
        void CalculBarreDeVie()
        {
            VieAI.Position = new Vector3(Position.X, Position.Y + 7, Position.Z);
            //VieAIFond.Position = new Vector3(Position.X, Position.Y + 7, Position.Z);
            VieAI.PositionJoueur = Cible.GetPosition;
            //VieAIFond.PositionJoueur = Cible.GetPosition;
            VieAI.PourcentageVie = PourcentageVie;
            //VieAIFond.PourcentageVie = PourcentageVie;
            if (EstEnCollision)
            {
                VieAI.Étendue = new Vector2(ÉTENDUE.X - 25, VieAI.Étendue.Y);
            }

        }
        public void ModifierActivation()
        {

        }

        #region Méthodes pour les mouvements
        void GestionProjectile()
        {
            ProjectileTank = new Projectile(Game, "Projectile", 0.1f, Rotation,
                                            new Vector3(Position.X - 5 * (float)Math.Sin(Rotation.Y), Position.Y + 4.5f, Position.Z - 5 * (float)Math.Cos(Rotation.Y)), IntervalleMAJ, 2f, 0.02f, false, this);

            Game.Components.Add(ProjectileTank);
        }

        protected void GestionMouvements(bool seDéplace, string collisionOuPas)
        {
            ++CompteurMouvement;
            switch (collisionOuPas)
            {
                case "collision":
                    ModificationParamètres(Orientation, seDéplace, collisionOuPas);
                    break;
                case "pas":
                    if (CompteurMouvement % DÉLAI_MOUVEMENT == 0)
                    {
                        // Recalcul de la rotation
                        Orientation = CalculOrientation(Cible.Coordonnées);
                        ModificationParamètres(Orientation, seDéplace, collisionOuPas);
                    }
                    else
                    {
                        // Déplacement normal
                        ModificationParamètres(Orientation, seDéplace, collisionOuPas);
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

            // (orientation >= INCRÉMENT_ROTATION) ? INCRÉMENT_ROTATION : orientation;
            return coeff + (float)Math.Atan(direction.X / direction.Y);
        }

        void ModificationParamètres(float orientation, bool seDéplace, string collisionOuPas)
        {
            if (seDéplace)
            {
                float posX = INCRÉMENT_DÉPLACEMENT_AI * (float)Math.Sin(orientation);
                float posY = INCRÉMENT_DÉPLACEMENT_AI * (float)Math.Cos(orientation);
                float posXFinal = 0;
                float posZFinal = 0;
                Vector2 déplacementFinal = new Vector2(posX, posY);

                switch (collisionOuPas)
                {
                    case "collision":
                        posXFinal = Position.X + déplacementFinal.X;
                        posZFinal = Position.Z + déplacementFinal.Y;
                        break;
                    case "pas":
                        posXFinal = Position.X - déplacementFinal.X;
                        posZFinal = Position.Z - déplacementFinal.Y;
                        break;
                }

                nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal));

                if (!EstHorsDesBornes(nouvellesCoords))
                {
                    NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                    Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
                    SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
                }

                Rotation = new Vector3(Rotation.X, orientation, Rotation.Z);

                CalculerMonde();
            }
        #endregion
        }
    }
}
