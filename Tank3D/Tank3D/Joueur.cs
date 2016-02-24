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
    public class Joueur : Mod�leMobile
    {
        // Constantes
        protected const float DISTANCE_POURSUITE = 20f;
        protected const float HAUTEUR_CAM_D�FAULT = 10f;

        // Propri�t�s
        Game Jeu { get; set; }
        Cam�raSubjective Cam�ra { get; set; }
        Projectile ProjectileTank { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 RotationProjectile { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector3 PositionProjectile { get; set; }
        Vector2 DeltaRotationCanon { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        Matrix MondeRoues { get; set; }
        Matrix MondeProjectile { get; set; }
        float �chelleTour { get; set; }
        float �chelleCanon { get; set; }
        float �chelleRoues { get; set; }
        float Incr�mentAngleRotationX { get; set; }
        float Incr�mentAngleRotationY { get; set; }
        public Vector2 Coordonn�es
        {
            get
            {
                return new Vector2(Position.X, Position.Z);
            }
        }

        public Joueur(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Jeu = jeu;
            Cam�ra = new Cam�raSubjective(jeu, new Vector3(positionInitiale.X, positionInitiale.Y + HAUTEUR_CAM_D�FAULT, positionInitiale.Z + DISTANCE_POURSUITE),
                                               new Vector3(Position.X, Position.Y + 4, Position.Z),
                                               Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Cam�ra);
            Game.Services.AddService(typeof(Cam�ra), Cam�ra);
        }

        public override void Initialize()
        {
            base.Initialize();
            RotationYawTour = new Vector3(-MathHelper.PiOver2, 0, MathHelper.PiOver2);
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, 0.02f, MathHelper.PiOver2);
            �chelleTour = 0.0035f;
            �chelleCanon = 0.005f;
            �chelleRoues = 0.05f;
            Mouse.SetPosition(400, 200);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            A�t�Cliqu� = GestionInput.EstNouveauClicGauche();
            if (A�t�Cliqu�)
            {
                GestionProjectile();
            }

            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvements();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        #region M�thodes pour la gestion des d�placements et rotations du mod�le
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected void GestionMouvements()
        {
            GestionProjectile();
            GestionSouris();
            RotationTour();
            RotationCanon();

            Cam�ra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Cam�ra.Position= new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_D�FAULT,
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
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            float posXFinal = Position.X - d�placementFinal.X;
            float posZFinal = Position.Z - d�placementFinal.Y;

            nouvellesCoords = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0, posZFinal));

            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
                HauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
            }
            CalculerMonde();
        }

        void RotationTour()
        {
            GestionSouris();
            RotationYawTour = new Vector3(RotationYawTour.X, RotationYawTour.Y + 2 * (-0.00005f * DeltaRotationCanon.X), RotationYawTour.Z);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(�chelleTour, RotationYawTour, PositionTour);
        }
        void RotationCanon()
        {
            GestionSouris();
            RotationPitchCanon = new Vector3(RotationPitchCanon.X + (-0.00005f * DeltaRotationCanon.Y),
                                             RotationPitchCanon.Y + 2 * (-0.00005f * DeltaRotationCanon.X), RotationPitchCanon.Z);
            if (RotationPitchCanon.X > -1.3 || RotationPitchCanon.X < -1.8)
            {
                RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-0.00005f * DeltaRotationCanon.Y),
                                                 RotationPitchCanon.Y, RotationPitchCanon.Z);
            }
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(�chelleCanon, RotationPitchCanon, PositionCanon);
            //DeltaRotationCanon = new Vector2(0, 0);
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
                ProjectileTank = new Projectile(Jeu, "Projectile", 0.1f, 
                                                new Vector3(2 * RotationPitchCanon.X + MathHelper.Pi, RotationPitchCanon.Y, RotationPitchCanon.Z),
                                                new Vector3(PositionCanon.X, PositionCanon.Y + 4.6f, PositionCanon.Z), IntervalleMAJ);
                Game.Components.Add(ProjectileTank);
            }
        }
        public bool EstD�truit()
        {
            bool estD�truit = false;
            return estD�truit;
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
