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
    public class Joueur : ModèleMobile, IActivable
    {
        VertexPositionColor[] ListePointsColor { get; set; }
        Vector3[] ListePoints { get; set; }
        protected BasicEffect EffetDeBase { get; set; }

        // Constantes
        protected const float DISTANCE_POURSUITE = 20f;
        protected const float HAUTEUR_CAM_DÉFAULT = 10f;
        const float INCRÉMENT_ROTATION_TOUR = 0.00005f;
        const float INCRÉMENT_DÉPLACEMENT = 1f;
        const float INTERVALLE_FUMÉE = 0.6f;
        const float HAUTEUR_DÉFAULT = 1f;

        // Propriétés
        CaméraSubjective Caméra { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector2 DeltaRotationCanon { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        Matrix MondeBoundingBox { get; set; }
        float ÉchelleTour { get; set; }
        float ÉchelleCanon { get; set; }
        float ÉchelleRoues { get; set; }
        Vector2 ObjectifAnglesNormales { get; set; }
        float TempsÉcouléMAJFumée { get; set; }
        Sprite Fumée { get; set; }
        Sprite Terre { get; set; }
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
                return Caméra.Position;
            }
        }

        public Joueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            ListePointsColor = new VertexPositionColor[8];
            ListePoints = new Vector3[8];
            EffetDeBase = new BasicEffect(GraphicsDevice);
	    RotationYawTour = new Vector3(-MathHelper.PiOver2, 0, MathHelper.PiOver2);
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, 0.02f, MathHelper.PiOver2);
            BoundingBoxModèle = new BoundingBoxSimple(Game, Position);
            Game.Components.Add(BoundingBoxModèle);
            ÉchelleTour = 0.00175f;
            ÉchelleCanon = 0.0025f;
            ÉchelleRoues = 0.025f;
            TempsÉcouléMAJFumée = 0f;
        }

        protected override void LoadContent()
        {
            Caméra = Game.Services.GetService(typeof(Caméra)) as CaméraSubjective;
            base.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            TempsÉcouléMAJFumée += TempsÉcoulé;
            AÉtéCliqué = GestionInput.EstNouveauClicGauche();
            if (AÉtéCliqué)
            {
                float Y = -200 * RotationPitchCanon.X;
                Fumée = new Fumée(Game, new Vector2(Game.Window.ClientBounds.Width / 2, Y), 0.2f);
                Game.Components.Add(Fumée);
                GestionProjectile();
            }

            if (TempsÉcouléMAJFumée > INTERVALLE_FUMÉE)
            {
                Game.Components.Remove(Fumée);
                TempsÉcouléMAJFumée = 0;
            }

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvements();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public void ModifierActivation()
        {

        }

        #region Méthodes pour la gestion des déplacements et rotations du modèle
        protected void GestionMouvements()
        {
            GestionProjectile();
            GestionSouris();
            RotationTour();
            RotationCanon();

            Caméra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Caméra.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X,
                                         ((float)Math.Tan(MathHelper.PiOver2 - RotationPitchCanon.X) * DISTANCE_POURSUITE) + Position.Y + HAUTEUR_CAM_DÉFAULT,
                                         ((float)Math.Cos(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.Z);

            float déplacement = GérerTouche(Keys.W) - GérerTouche(Keys.S);
            float rotation = GérerTouche(Keys.D) - GérerTouche(Keys.A);
            if (déplacement != 0 || rotation != 0)
            {
                ModificationParamètres(déplacement, rotation);
            }
        }

        void ModificationParamètres(float déplacement, float rotation)
        {
            float rotationFinal = Rotation.Y - (IncrémentAngleRotation * rotation) / 3f;
            float posX = déplacement * (float)Math.Sin(rotationFinal);
            float posY = déplacement * (float)Math.Cos(rotationFinal);
            Vector2 déplacementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;
            
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                AncienneHauteurTerrain = NouvelleHauteurTerrain;
                NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
                BoundingBoxModèle.MettreÀJour(déplacementFinal.X, déplacementFinal.Y);
                MondeBoundingBox = TransformationsMeshes(0.05f, Rotation, Position);
                ObjectifAnglesNormales = GestionnaireDeNormales.GetNormale(nouvellesCoords);
                Rotation = Rotation = new Vector3(ObjectifAnglesNormales.Y, Rotation.Y, ObjectifAnglesNormales.X);
            }
            CalculerMonde();
        }

        void TraitementNormales(Point coords, string axe)
        {
            int sens;
            switch (axe)
            {
                case "X":
                    sens = (ObjectifAnglesNormales.X < Rotation.X) ? 1 : -1;
                    if (ApproximationÉgalité(ObjectifAnglesNormales.X, Rotation.X))
                    {
                        ObjectifAnglesNormales = GestionnaireDeNormales.GetNormale(coords);
                    }
                    else
                    {
                        Rotation = new Vector3(Rotation.X + sens * IncrémentAngleRotation, Rotation.Y, Rotation.Z);
                    }
                    break;

                case "Y":
                    sens = (ObjectifAnglesNormales.Y < Rotation.Z) ? 1 : -1;
                    if (ApproximationÉgalité(ObjectifAnglesNormales.Y, Rotation.Z))
                    {
                        ObjectifAnglesNormales = GestionnaireDeNormales.GetNormale(coords);
                    }
                    else
                    {
                        Rotation = new Vector3(Rotation.X, Rotation.Y, Rotation.Z + sens * IncrémentAngleRotation);
                    }
                    break;
            }
        }

        bool ApproximationÉgalité(float valeur1, float valeur2)
        {
            double tolérance = valeur1 * 0.0001; // 0.01 % de difference acceptable
            return Math.Abs(valeur1 - valeur2) <= tolérance;
        }

        void RotationTour()
        {
            GestionSouris();
            RotationYawTour = new Vector3(-MathHelper.PiOver2, RotationYawTour.Y + 2 * (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.X), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(ÉchelleTour, RotationYawTour, PositionTour);
        }

        void RotationCanon()
        {
            GestionSouris();
            RotationPitchCanon = new Vector3(RotationPitchCanon.X + (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                             RotationPitchCanon.Y + 2 * (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.X), RotationPitchCanon.Z);
            if (RotationPitchCanon.X > -1.3 || RotationPitchCanon.X < -1.8)
            {
                RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-INCRÉMENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                                 RotationPitchCanon.Y, RotationPitchCanon.Z);
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
            if ((GestionInput.NouvelÉtatSouris.X < (Game.Window.ClientBounds.Width / 2) + 20 && GestionInput.NouvelÉtatSouris.X > (Game.Window.ClientBounds.Width / 2) - 20))
            {
                DeltaRotationCanon = new Vector2(0, DeltaRotationCanon.Y);
            }
            if ((GestionInput.NouvelÉtatSouris.Y < (Game.Window.ClientBounds.Height / 2) + 20 && GestionInput.NouvelÉtatSouris.Y > (Game.Window.ClientBounds.Height / 2) - 20))
            {
                DeltaRotationCanon = new Vector2(DeltaRotationCanon.X, 0);
            }
        }

        void GestionProjectile()
        {
            if (AÉtéCliqué)
            {
                ProjectileTank = new Projectile(Game, "Projectile", 0.1f,
                                                new Vector3(2 * RotationPitchCanon.X + MathHelper.Pi, RotationPitchCanon.Y - 0.05f, RotationPitchCanon.Z),
                                                new Vector3(PositionCanon.X, PositionCanon.Y + 4.6f, PositionCanon.Z), IntervalleMAJ, 2f, 0.02f, false);
                Game.Components.Add(ProjectileTank);
            }
        }

        /*
        public bool EstDétruit()
        {
            bool estDétruit = false;
            return estDétruit;
        }
        */

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }

        Matrix TransformationsMeshes(float échelle, Vector3 rotation, Vector3 position)
        {
            Matrix monde = Matrix.Identity;
            monde *= Matrix.CreateScale(échelle);
            monde *= Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            monde *= Matrix.CreateTranslation(position);

            return monde;
        }

        public override void Draw(GameTime gameTime)
        {
            ListePointsColor = new VertexPositionColor[8];
            EffetDeBase.World = MondeBoundingBox;
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            EffetDeBase.CurrentTechnique.Passes[0].Apply();

            for (int i = 0; i < BoundingBoxModèle.ListePoints.Count(); i++)
            {
                ListePointsColor[i] = new VertexPositionColor(BoundingBoxModèle.ListePoints[i], Color.Red);
            }
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, ListePointsColor, 0, 7);


            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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
                    mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                }

                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = CaméraJeu.Projection;
                    effet.View = CaméraJeu.Vue;
                    effet.World = mondeLocal;
                }
                maille.Draw();
            }
            
        }
        #endregion
    }
}
