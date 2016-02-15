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
        protected const float DISTANCE_POURSUITE = 15f;
        protected const float HAUTEUR_CAM_D�FAULT = 5f;
        
        // Propri�t�s
        InputManager GestionInput { get; set; }
        Cam�raSubjective Cam�ra { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        //Vector3 �chelleTour { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Matrix MondeTour { get; set; }

        public Joueur(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cam�ra = new Cam�raSubjective(jeu, new Vector3(0, 24, 140), positionInitiale, Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Cam�ra);
            Game.Services.AddService(typeof(Cam�ra), Cam�ra);
        }

        public override void Initialize()
        {
            base.Initialize();
            RotationYawTour = Vector3.Zero;
            //�chelleTour = new Vector3(2f, 1.5f, 1.3f);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        public Vector2 Coordonn�es
        {
            get
            {
                return new Vector2(Position.X, Position.Z);
            }
        }

        #region M�thodes pour la gestion des d�placements et rotations du mod�le
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected override void GestionMouvements()
        {
            RotationTour();
            
            int d�placement = G�rerTouche(Keys.W) - G�rerTouche(Keys.S);
            int rotation = G�rerTouche(Keys.D) - G�rerTouche(Keys.A);
            if (d�placement != 0 || rotation != 0)
            {
                ModificationParam�tres(d�placement, rotation);
            }
        }

        void ModificationParam�tres(int d�placement, int rotation)
        {
            float rotationFinal = Rotation.Y - Incr�mentAngleRotation * rotation;
            float posX = d�placement * (float)Math.Sin(rotationFinal);
            float posY = d�placement * (float)Math.Cos(rotationFinal);

            Vector2 d�placementFinal = new Vector2(posX, posY);
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            Position = new Vector3(Position.X - d�placementFinal.X, Position.Y, Position.Z - d�placementFinal.Y);
            float posXFinal = Position.X - d�placementFinal.X;
            float posZFinal = Position.Z - d�placementFinal.Y;

            Point nouvellesCoords = TerrainJeu.ConvertionCoordonn�es(new Vector3(posXFinal, 0,posZFinal));
            
            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_D�FAULT, posZFinal);
                HauteurTerrain = TerrainJeu.GetHauteur(Position);
                Cam�ra.Cible = Position;
                Cam�ra.Position = new Vector3(((float)Math.Sin(rotationFinal) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_D�FAULT, ((float)Math.Cos(rotationFinal) * DISTANCE_POURSUITE) + Position.Z);
            }

            CalculerMonde();
        }

        private void RotationTour()
        {
            ModelMesh tour = Mod�le.Meshes.First(x => x.Name == "Tour");            
            RotationYawTour=  new Vector3(-MathHelper.PiOver2,RotationYawTour.Y + (Incr�mentAngleRotation * 1),180);
            PositionTour = new Vector3(Position.X,Position.Y + 3f, Position.Z);
            MondeTour = TransformationsMeshes(0.12f, RotationYawTour, PositionTour);
        }

        private void RotationCanon()
        {
            // � modifier pour modifier la souris
            
            ModelMesh canon = Mod�le.Meshes.First(x => x.Name == "Canon");
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, RotationPitchCanon.Y + (Incr�mentAngleRotation * 1.5f), 180);
            PositionCanon = new Vector3(Position.X, Position.Y + 3f, Position.Z);
            MondeTour = TransformationsMeshes(0.12f, RotationPitchCanon, PositionCanon);
        }

        bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;

            return estHorsDesBornes;
        }

        int G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? INCR�MENT_D�PLACEMENT : 0;
        }

        private Matrix TransformationsMeshes(float �chelle, Vector3 rotation,Vector3 position)
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
                
                if(maille.Name == "Tour")
                {
                    mondeLocal = MondeTour;
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
