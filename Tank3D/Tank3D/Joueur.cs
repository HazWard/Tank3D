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
    public class Joueur : Mod�leMobile, IActivable, IModel
    {
        VertexPositionColor[] ListePointsColor { get; set; }
        Vector3[] ListePoints { get; set; }
        protected BasicEffect EffetDeBase { get; set; }

        // Constantes
        protected const float DISTANCE_POURSUITE = 20f;
        protected const float HAUTEUR_CAM_D�FAULT = 10f;
        const float INCR�MENT_ROTATION_TOUR = 0.00005f;
        const float INCR�MENT_D�PLACEMENT = 1f;
        const float INTERVALLE_FUM�E = 0.6f;
        const float HAUTEUR_D�FAULT = 1f;

        // Propri�t�s
        Cam�raSubjective Cam�ra { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector2 DeltaRotationCanon { get; set; }
        Vector2 AnglesIncr�ments { get; set; }
        Vector2 AncienAnglesIncr�ments { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        float �chelleTour { get; set; }
        float �chelleCanon { get; set; }
        float �chelleRoues { get; set; }
        float Temps�coul�MAJFum�e { get; set; }
        Sprite Fum�e { get; set; }
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
                return Cam�ra.Position;
            }
        }

        public BoundingSphere Sphere
        {
            get
            {
                return SphereCollision;
            }
        }

        public Joueur(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
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
            SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
            �chelleTour = 0.0035f;
            �chelleCanon = 0.005f;
            �chelleRoues = 0.05f;
            Temps�coul�MAJFum�e = 0f;
        }

        protected override void LoadContent()
        {
            Cam�ra = Game.Services.GetService(typeof(Cam�ra)) as Cam�raSubjective;
            base.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            Temps�coul�MAJFum�e += Temps�coul�;
            A�t�Cliqu� = GestionInput.EstNouveauClicGauche();
            if (A�t�Cliqu�)
            {
                float Y = -200 * RotationPitchCanon.X;
                Fum�e = new Fum�e(Game, new Vector2(Game.Window.ClientBounds.Width / 2, Y), 0.2f);
                Game.Components.Add(Fum�e);
                GestionProjectile();
            }

            if (Temps�coul�MAJFum�e > INTERVALLE_FUM�E)
            {
                Game.Components.Remove(Fum�e);
                Temps�coul�MAJFum�e = 0;
            }

            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (!EstEnCollision)
                {
                    GestionMouvements();
                }
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public void ModifierActivation()
        {

        }

        #region M�thodes pour la gestion des d�placements et rotations du mod�le
        protected void GestionMouvements()
        {
            GestionProjectile();
            GestionSouris();
            RotationTour();
            RotationCanon();

            Cam�ra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Cam�ra.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X,
                                         ((float)Math.Tan(MathHelper.PiOver2 - RotationPitchCanon.X) * DISTANCE_POURSUITE) + Position.Y + HAUTEUR_CAM_D�FAULT,
                                         ((float)Math.Cos(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.Z);
            float d�placement = G�rerTouche(Keys.W) - G�rerTouche(Keys.S);
            float rotation = G�rerTouche(Keys.D) - G�rerTouche(Keys.A);
            if (d�placement != 0 || rotation != 0)
            {
                ModificationParam�tres(d�placement, rotation);
            }

        }

        void ModificationParam�tres(float d�placement, float rotation)
        {
            float rotationFinal = Rotation.Y - (Incr�mentAngleRotation * rotation) / 3f;
            float posX = d�placement * (float)Math.Sin(rotationFinal);
            float posY = d�placement * (float)Math.Cos(rotationFinal);
            Vector2 d�placementFinal = new Vector2(posX, posY);
            float posXFinal = Position.X - d�placementFinal.X;
            float posZFinal = Position.Z - d�placementFinal.Y;

            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            nouvellesCoords = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                AncienneHauteurTerrain = NouvelleHauteurTerrain;
                NouvelleHauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
                Position = new Vector3(posXFinal, NouvelleHauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
                AnglesIncr�ments = GestionnaireDeNormales.GetNormale(nouvellesCoords, Rotation);

                if (Math.Abs(Rotation.Y % MathHelper.TwoPi) >= MathHelper.PiOver2 && Math.Abs(Rotation.Y % MathHelper.TwoPi) <= MathHelper.PiOver2 * 3)
                {
                    AnglesIncr�ments = new Vector2(AnglesIncr�ments.X, -AnglesIncr�ments.Y);
                }

                TraitementNormales(nouvellesCoords, "X");
                TraitementNormales(nouvellesCoords, "Y");
                AncienAnglesIncr�ments = AnglesIncr�ments;
                SphereCollision = new BoundingSphere(Position, RAYON_COLLISION);
            }
            CalculerMonde();
        }

        void TraitementNormales(Point coords, string axe)
        {
            switch (axe)
            {
                case "X":
                    Rotation = new Vector3(AnglesIncr�ments.Y, Rotation.Y, Rotation.Z);
                    break;

                case "Y":
                    Rotation = new Vector3(Rotation.X, Rotation.Y, AnglesIncr�ments.X);
                    break;
            }
        }

        void RotationTour()
        {
            GestionSouris();
            RotationYawTour = new Vector3(-MathHelper.PiOver2 + Rotation.X, RotationYawTour.Y + 2 * (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.X), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(�chelleTour, RotationYawTour, PositionTour);
        }

        void RotationCanon()
        {
            GestionSouris();
            RotationPitchCanon = new Vector3(RotationPitchCanon.X + (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                             RotationPitchCanon.Y + 2 * (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.X), RotationPitchCanon.Z);
            if (RotationPitchCanon.X > -1.3 || RotationPitchCanon.X < -1.8)
            {
                RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-INCR�MENT_ROTATION_TOUR * DeltaRotationCanon.Y),
                                                 RotationPitchCanon.Y, RotationPitchCanon.Z);
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
            if ((GestionInput.Nouvel�tatSouris.X < (Game.Window.ClientBounds.Width / 2) + 20 && GestionInput.Nouvel�tatSouris.X > (Game.Window.ClientBounds.Width / 2) - 20))
            {
                DeltaRotationCanon = new Vector2(0, DeltaRotationCanon.Y);
            }
            if ((GestionInput.Nouvel�tatSouris.Y < (Game.Window.ClientBounds.Height / 2) + 20 && GestionInput.Nouvel�tatSouris.Y > (Game.Window.ClientBounds.Height / 2) - 20))
            {
                DeltaRotationCanon = new Vector2(DeltaRotationCanon.X, 0);
            }
        }

        void GestionProjectile()
        {
            if (A�t�Cliqu�)
            {
                ProjectileTank = new Projectile(Game, "Projectile", 0.1f,
                                                new Vector3(2 * RotationPitchCanon.X + MathHelper.Pi, RotationPitchCanon.Y - 0.05f, RotationPitchCanon.Z),
                                                new Vector3(PositionCanon.X - 5 * (float)Math.Sin(RotationPitchCanon.Y), PositionCanon.Y + 4.5f, PositionCanon.Z - 5 * (float)Math.Cos(RotationPitchCanon.Y)), IntervalleMAJ, 2f, 0.02f, false, this);
                Game.Components.Add(ProjectileTank);
            }
        }

        float G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? INCR�MENT_D�PLACEMENT : 0;
        }

        Matrix TransformationsMeshes(float �chelle, Vector3 rotation, Vector3 position)
        {
            Matrix monde = Matrix.Identity;
            monde *= Matrix.CreateScale(�chelle);
            monde *= Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            monde *= Matrix.CreateTranslation(position);

            return monde;
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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
                    mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                }

                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = Cam�raJeu.Projection;
                    effet.View = Cam�raJeu.Vue;
                    effet.World = mondeLocal;
                }
                maille.Draw();
            }

        }
        #endregion
    }
}
