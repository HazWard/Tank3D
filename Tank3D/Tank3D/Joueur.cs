using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
    public class Joueur : ModèleMobile, IActivable, IModel
    {
        // Constantes
        protected const float DISTANCE_POURSUITE = 20f;
        protected const float HAUTEUR_CAM_DÉFAULT = 10f;
        const float INCRÉMENT_ROTATION_TOUR = 0.00005f;
        const float INCRÉMENT_DÉPLACEMENT = 1f;
        const float INTERVALLE_FUMÉE = 0.6f;
        const float HAUTEUR_DÉFAULT = 1f;

        // Propriétés
        public TexteCentré TexteScore { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector2 DeltaRotationCanon { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        float RotationYaw { get; set; }
        float ÉchelleTour { get; set; }
        float ÉchelleCanon { get; set; }
        float ÉchelleRoues { get; set; }
        float TempsÉcouléMAJFumée { get; set; }
        public bool EstMort { get; set; }
        Sprite Fumée { get; set; }
        int Vie { get; set; }
        public int Score { get; set; }
        int CompteurTir { get; set; }
        Matrix OrientationTank;
        public Vector2 Coordonnées
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
                return CaméraJeu.Position;
            }
        }

        public Joueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ) { }

        public override void Initialize()
        {
            base.Initialize();
            OrientationTank = Matrix.Identity;
            OrientationTank *= Matrix.CreateScale(Échelle);
            TexteScore = new TexteCentré(Game, Score.ToString(), "Arial20", new Rectangle(Game.Window.ClientBounds.Width / 12, Game.Window.ClientBounds.Height / 10, 100, 100), Color.Red, 0f);
            RotationYawTour = new Vector3(-MathHelper.PiOver2, 0, MathHelper.PiOver2);
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, 0.02f, MathHelper.PiOver2);
            SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
            ÉchelleTour = 0.0035f; // Valeurs prédéfinies
            ÉchelleCanon = 0.005f; // pour assurer la
            ÉchelleRoues = 0.05f; // cohérence des proportions
            Vie = 100;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            TempsÉcouléMAJFumée += TempsÉcoulé;

            GestionScore();
            GestionCollisions();
            GestionTirs();
            GestionFumée();
            GestionTirsRecus();

            Visible = !CaméraJeu.EstEnZoom;

            base.Update(gameTime);
        }

        public void ModifierActivation() { }

        #region Gestion des comportements du tank
        void GestionScore()
        {
            TexteScore.TexteÀAfficher = Score.ToString();

            if (!Game.Components.Contains(TexteScore))
            {
                Game.Components.Add(TexteScore);
            }
        }

        void GestionCollisions()
        {
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (!EstEnCollision)
                {
                    GestionMouvements();
                }
                TempsÉcouléDepuisMAJ = 0;
            }
        }

        void GestionTirs()
        {
            AÉtéCliqué = GestionInput.EstNouveauClicGauche();
            if (AÉtéCliqué && CompteurTir >= 500)
            {
                float Y = -200 * RotationPitchCanon.X;
                Fumée = new Fumée(Game, new Vector2(Game.Window.ClientBounds.Width / 2, Y), 0.2f);
                Game.Components.Add(Fumée);

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
        void GestionFumée()
        {
            if (TempsÉcouléMAJFumée > INTERVALLE_FUMÉE)
            {
                Game.Components.Remove(Fumée);
                TempsÉcouléMAJFumée = 0;
            }
        }
        void GestionTirsRecus()
        {
            if (AÉtéTiré)
            {
                Game.Components.Add(new FiltreDommage(Game, IntervalleMAJ));
                Vie -= 20;
                if (Vie <= 0)
                {
                    EstMort = true;
                    CaméraJeu.Enabled = false;
                    this.Enabled = false;
                }
                AÉtéTiré = false;
            }
        }
        #endregion

        #region Méthodes pour la gestion des déplacements et rotations du modèle
        protected void GestionMouvements()
        {
            GestionProjectile(2f);
            GestionSouris();
            RotationTour();
            RotationCanon();

            CaméraJeu.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            CaméraJeu.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X,
                                         ((float)Math.Tan(MathHelper.PiOver2 - RotationPitchCanon.X) * DISTANCE_POURSUITE) + Position.Y + HAUTEUR_CAM_DÉFAULT,
                                         ((float)Math.Cos(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.Z);
            if (Visible)
            {
                float déplacement = GérerTouche(Keys.W) - GérerTouche(Keys.S);
                float rotation = GérerTouche(Keys.D) - GérerTouche(Keys.A);
                if (déplacement != 0 || rotation != 0)
                {
                    ModificationParamètres(déplacement, rotation);
                }
            }
        }

        void ModificationParamètres(float déplacement, float rotation)
        {
            RotationYaw = Rotation.Y - (IncrémentAngleRotation * rotation) / 3f;
            float posX = déplacement * (float)Math.Sin(RotationYaw);
            float posY = déplacement * (float)Math.Cos(RotationYaw);
            Vector2 déplacementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            Rotation = new Vector3(Rotation.X, RotationYaw, Rotation.Z);
            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                AncienneHauteurTerrain = NouvelleHauteurTerrain;
                NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
                #region Partie Starly
                // Utilisation d'une matrice
                OrientationTank = Matrix.CreateRotationY(RotationYaw);
                OrientationTank.Up = TerrainJeu.Normales[nouvellesCoords.X, nouvellesCoords.Y];
                OrientationTank.Right = Vector3.Normalize(Vector3.Cross(OrientationTank.Forward, OrientationTank.Up));
                OrientationTank.Forward = Vector3.Normalize(Vector3.Cross(OrientationTank.Up, OrientationTank.Right));
                OrientationTank *= Matrix.CreateScale(Échelle);
                SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
                #endregion
            }

            CalculerMonde();
        }

        void RotationTour()
        {
            RotationYawTour = new Vector3(-MathHelper.PiOver2 + Rotation.X, RotationYawTour.Y + 2 * (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.X), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(ÉchelleTour, RotationYawTour, PositionTour);
        }

        void RotationCanon()
        {
            RotationPitchCanon = new Vector3(RotationPitchCanon.X + (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                             RotationPitchCanon.Y + 2 * (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.X), RotationPitchCanon.Z);
            if (Visible)
            {
                if (RotationPitchCanon.X > -1.2f || RotationPitchCanon.X < -2.0f)
                {
                    RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                                     RotationPitchCanon.Y, RotationPitchCanon.Z);
                }
            }
            else
            {
                if (RotationPitchCanon.X > -1.0f || RotationPitchCanon.X < -1.8f)
                {
                    RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                                     RotationPitchCanon.Y, RotationPitchCanon.Z);
                }
            }
            
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(ÉchelleCanon, RotationPitchCanon, PositionCanon);
        }

        void GestionSouris()
        {
            if (GestionInput.NouvelÉtatSouris != GestionInput.AncienÉtatSouris)
            {
                DeltaRotationCanon = new Vector2(GestionInput.NouvelÉtatSouris.X - (Game.GraphicsDevice.Viewport.Width / 2), GestionInput.NouvelÉtatSouris.Y - (Game.GraphicsDevice.Viewport.Height / 2));
            }
            if ((GestionInput.NouvelÉtatSouris.X < (Game.Window.ClientBounds.Width / 2) + (Game.Window.ClientBounds.Width / 32) && GestionInput.NouvelÉtatSouris.X > (Game.Window.ClientBounds.Width / 2) - (Game.Window.ClientBounds.Width / 32)))
            {
                DeltaRotationCanon = new Vector2(0, DeltaRotationCanon.Y);
            }
            if ((GestionInput.NouvelÉtatSouris.Y < (Game.Window.ClientBounds.Height / 2) + (Game.Window.ClientBounds.Height / 16) && GestionInput.NouvelÉtatSouris.Y > (Game.Window.ClientBounds.Height / 2) - (Game.Window.ClientBounds.Height / 16)))
            {
                DeltaRotationCanon = new Vector2(DeltaRotationCanon.X, 0);
            }
        }

        void GestionProjectile(float indiceRotation)
        {
            if (AÉtéCliqué)
            {
                ProjectileTank = new Projectile(Game, "Projectile", 0.1f,
                                                new Vector3(indiceRotation * RotationPitchCanon.X + MathHelper.Pi, RotationPitchCanon.Y - 0.05f, RotationPitchCanon.Z),
                                                new Vector3(PositionCanon.X - 5 * (float)Math.Sin(RotationPitchCanon.Y), PositionCanon.Y + 4.5f, PositionCanon.Z - 5 * (float)Math.Cos(RotationPitchCanon.Y)), IntervalleMAJ, 2f, 0.02f, false, this);
                Game.Components.Add(ProjectileTank);
            }
        }

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }

        #region Partie Starly
        Matrix TransformationsMeshes(float échelle, Vector3 rotation, Vector3 position)
        {
            Matrix monde = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            monde *= Matrix.CreateScale(échelle);
            monde *= Matrix.CreateTranslation(position);

            return monde;
        }
        
        public override void Draw(GameTime gameTime)
        {
            
            Matrix[] boneTransforms = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Matrix worldMatrix = OrientationTank * Matrix.CreateTranslation(Position);

            foreach (ModelMesh maille in Modèle.Meshes)
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
                    mondeLocal = TransformationsModèle[maille.ParentBone.Index] * worldMatrix; ;
                }

                foreach (BasicEffect effect in maille.Effects)
                {
                    effect.View = CaméraJeu.Vue;
                    effect.Projection = CaméraJeu.Projection;
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
