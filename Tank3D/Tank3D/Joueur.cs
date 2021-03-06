// Auteurs: Maxime Benoit et Starly Solon
// Fichier: Joueur.cs
// Date de cr�ation: 15 f�vrier 2016
// Description: Classe s'occupant de la gestion du d�placement 
//              et des habilit�s du tank de l'utilisateur

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
    public class Joueur : Mod�leMobile, IActivable, IModel
    {
        // Constantes
        protected const float DISTANCE_POURSUITE = 20f;
        protected const float HAUTEUR_CAM_D�FAULT = 10f;
        const float INCR�MENT_ROTATION_TOUR = 0.00005f;
        const float INCR�MENT_D�PLACEMENT = 1f;
        const float INTERVALLE_FUM�E = 0.6f;
        const float HAUTEUR_D�FAULT = 1f;

        // Propri�t�s
        public TexteCentr� TexteScore { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector2 DeltaRotationCanon { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        float RotationYaw { get; set; }
        float �chelleTour { get; set; }
        float �chelleCanon { get; set; }
        float �chelleRoues { get; set; }
        float Temps�coul�MAJFum�e { get; set; }
        public bool EstMort { get; set; }
        Sprite Fum�e { get; set; }
        int Vie { get; set; }
        public int Score { get; set; }
        int CompteurTir { get; set; }
        Matrix OrientationTank;
        public Vector2 Coordonn�es
        {
            get
            {
                return new Vector2(Position.X, Position.Z);
            }
        }
        public Vector3 GetPosition
        {
            get
            {
                return Cam�raJeu.Position;
            }
        }

        public Joueur(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ) { }

        public override void Initialize()
        {
            base.Initialize();
            OrientationTank = Matrix.Identity;
            OrientationTank *= Matrix.CreateScale(�chelle);
            TexteScore = new TexteCentr�(Game, Score.ToString(), "Arial20", new Rectangle(Game.Window.ClientBounds.Width / 12, Game.Window.ClientBounds.Height / 10, 100, 100), Color.Red, 0f);
            RotationYawTour = new Vector3(-MathHelper.PiOver2, 0, MathHelper.PiOver2);
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, 0.02f, MathHelper.PiOver2);
            SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
            �chelleTour = 0.0035f; // Valeurs pr�d�finies
            �chelleCanon = 0.005f; // pour assurer la
            �chelleRoues = 0.05f; // coh�rence des proportions
            Vie = 100;
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            Temps�coul�MAJFum�e += Temps�coul�;

            GestionScore();
            GestionCollisions();
            GestionTirs();
            GestionFum�e();
            GestionTirsRecus();

            Visible = !Cam�raJeu.EstEnZoom;

            base.Update(gameTime);
        }

        public void ModifierActivation() { }

        #region Gestion des comportements du tank
        void GestionScore()
        {
            TexteScore.Texte�Afficher = Score.ToString();

            if (!Game.Components.Contains(TexteScore))
            {
                Game.Components.Add(TexteScore);
            }
        }

        void GestionCollisions()
        {
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (!EstEnCollision)
                {
                    GestionMouvements();
                }
                Temps�coul�DepuisMAJ = 0;
            }
        }

        void GestionTirs()
        {
            A�t�Cliqu� = GestionInput.EstNouveauClicGauche();
            if (A�t�Cliqu� && CompteurTir >= 500)
            {
                float Y = -200 * RotationPitchCanon.X;
                Fum�e = new Fum�e(Game, new Vector2(Game.Window.ClientBounds.Width / 2, Y), 0.2f);
                Game.Components.Add(Fum�e);

                if (!Visible)
                {
                    GestionProjectile(2.5f);
                }
                else
                {
                    GestionProjectile(2f);
                }
                CompteurTir = 0;
            }
            CompteurTir++;
        }
        void GestionFum�e()
        {
            if (Temps�coul�MAJFum�e > INTERVALLE_FUM�E)
            {
                Game.Components.Remove(Fum�e);
                Temps�coul�MAJFum�e = 0;
            }
        }
        void GestionTirsRecus()
        {
            if (A�t�Tir�)
            {
                Game.Components.Add(new FiltreDommage(Game, IntervalleMAJ));
                Vie -= 20;
                if (Vie <= 0)
                {
                    EstMort = true;
                    Cam�raJeu.Enabled = false;
                    this.Enabled = false;
                }
                A�t�Tir� = false;
            }
        }
        #endregion

        #region M�thodes pour la gestion des d�placements et rotations du mod�le
        protected void GestionMouvements()
        {
            GestionProjectile(2f);
            GestionSouris();
            RotationTour();
            RotationCanon();

            Cam�raJeu.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Cam�raJeu.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X,
                                         ((float)Math.Tan(MathHelper.PiOver2 - RotationPitchCanon.X) * DISTANCE_POURSUITE) + Position.Y + HAUTEUR_CAM_D�FAULT,
                                         ((float)Math.Cos(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.Z);
            if (Visible)
            {
                float d�placement = G�rerTouche(Keys.W) - G�rerTouche(Keys.S);
                float rotation = G�rerTouche(Keys.D) - G�rerTouche(Keys.A);
                if (d�placement != 0 || rotation != 0)
                {
                    ModificationParam�tres(d�placement, rotation);
                }
            }
        }

        void ModificationParam�tres(float d�placement, float rotation)
        {
            RotationYaw = Rotation.Y - (Incr�mentAngleRotation * rotation) / 3f;
            float posX = d�placement * (float)Math.Sin(RotationYaw);
            float posY = d�placement * (float)Math.Cos(RotationYaw);
            Vector2 d�placementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - d�placementFinal.X;
            float posZFinal = Position.Z - d�placementFinal.Y;

            Rotation = new Vector3(Rotation.X, RotationYaw, Rotation.Z);
            nouvellesCoords = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                AncienneHauteurTerrain = NouvelleHauteurTerrain;
                NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
                #region Partie Starly
                // Utilisation d'une matrice
                OrientationTank = Matrix.CreateRotationY(RotationYaw);
                OrientationTank.Up = TerrainJeu.Normales[nouvellesCoords.X, nouvellesCoords.Y];
                OrientationTank.Right = Vector3.Normalize(Vector3.Cross(OrientationTank.Forward, OrientationTank.Up));
                OrientationTank.Forward = Vector3.Normalize(Vector3.Cross(OrientationTank.Up, OrientationTank.Right));
                OrientationTank *= Matrix.CreateScale(�chelle);
                SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
                #endregion
            }

            CalculerMonde();
        }

        void RotationTour()
        {
            RotationYawTour = new Vector3(-MathHelper.PiOver2 + Rotation.X, RotationYawTour.Y + 2 * (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.X), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(�chelleTour, RotationYawTour, PositionTour);
        }

        void RotationCanon()
        {
            RotationPitchCanon = new Vector3(RotationPitchCanon.X + (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                             RotationPitchCanon.Y + 2 * (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.X), RotationPitchCanon.Z);
            if (Visible)
            {
                if (RotationPitchCanon.X > -1.2f || RotationPitchCanon.X < -2.0f)
                {
                    RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                                     RotationPitchCanon.Y, RotationPitchCanon.Z);
                }
            }
            else
            {
                if (RotationPitchCanon.X > -1.0f || RotationPitchCanon.X < -1.8f)
                {
                    RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                                     RotationPitchCanon.Y, RotationPitchCanon.Z);
                }
            }
            
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(�chelleCanon, RotationPitchCanon, PositionCanon);
        }

        void GestionSouris()
        {
            if (GestionInput.Nouvel�tatSouris != GestionInput.Ancien�tatSouris)
            {
                DeltaRotationCanon = new Vector2(GestionInput.Nouvel�tatSouris.X - (Game.GraphicsDevice.Viewport.Width / 2), GestionInput.Nouvel�tatSouris.Y - (Game.GraphicsDevice.Viewport.Height / 2));
            }
            if ((GestionInput.Nouvel�tatSouris.X < (Game.Window.ClientBounds.Width / 2) + (Game.Window.ClientBounds.Width / 32) && GestionInput.Nouvel�tatSouris.X > (Game.Window.ClientBounds.Width / 2) - (Game.Window.ClientBounds.Width / 32)))
            {
                DeltaRotationCanon = new Vector2(0, DeltaRotationCanon.Y);
            }
            if ((GestionInput.Nouvel�tatSouris.Y < (Game.Window.ClientBounds.Height / 2) + (Game.Window.ClientBounds.Height / 16) && GestionInput.Nouvel�tatSouris.Y > (Game.Window.ClientBounds.Height / 2) - (Game.Window.ClientBounds.Height / 16)))
            {
                DeltaRotationCanon = new Vector2(DeltaRotationCanon.X, 0);
            }
        }

        void GestionProjectile(float indiceRotation)
        {
            if (A�t�Cliqu�)
            {
                ProjectileTank = new Projectile(Game, "Projectile", 0.1f,
                                                new Vector3(indiceRotation * RotationPitchCanon.X + MathHelper.Pi, RotationPitchCanon.Y - 0.05f, RotationPitchCanon.Z),
                                                new Vector3(PositionCanon.X - 5 * (float)Math.Sin(RotationPitchCanon.Y), PositionCanon.Y + 4.5f, PositionCanon.Z - 5 * (float)Math.Cos(RotationPitchCanon.Y)), IntervalleMAJ, 2f, 0.02f, false, this);
                Game.Components.Add(ProjectileTank);
            }
        }

        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? INCR�MENT_D�PLACEMENT : 0;
        }

        #region Partie Starly
        Matrix TransformationsMeshes(float �chelle, Vector3 rotation, Vector3 position)
        {
            Matrix monde = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            monde *= Matrix.CreateScale(�chelle);
            monde *= Matrix.CreateTranslation(position);

            return monde;
        }
        
        public override void Draw(GameTime gameTime)
        {
            
            Matrix[] boneTransforms = new Matrix[Mod�le.Bones.Count];
            Mod�le.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Matrix worldMatrix = OrientationTank * Matrix.CreateTranslation(Position);

            foreach (ModelMesh maille in Mod�le.Meshes)
            {
                Matrix mondeLocal;

                if (maille.Name == "Tour")
                {
                    mondeLocal = MondeTour;
                }
                else if (maille.Name == "Canon")
                {
                    mondeLocal = MondeCanon;
                }
                else
                {
                    mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * worldMatrix; ;
                }

                foreach (BasicEffect effect in maille.Effects)
                {
                    effect.View = Cam�raJeu.Vue;
                    effect.Projection = Cam�raJeu.Projection;
                    effect.World = mondeLocal;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                maille.Draw();
            
            }
        }
        #endregion
        #endregion
    }
}
