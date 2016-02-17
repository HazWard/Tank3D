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
        MouseState Nouvel�tatSouris { get; set; }
        MouseState Ancien�tatSouris { get; set; }
        InputManager GestionInput { get; set; }
        Cam�raSubjective Cam�ra { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector2 AnciennePositionSouris { get; set; }
        Vector2 D�placementSouris { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        Matrix MondeRoues { get; set; }
        float �chelleTour { get; set; }
        float �chelleCanon { get; set; }
        float �chelleRoues { get; set; }
        float Incr�mentAngleRotationTour { get; set; }

        public Joueur(Game jeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Cam�ra = new Cam�raSubjective(jeu, new Vector3(positionInitiale.X, positionInitiale.Y + HAUTEUR_CAM_D�FAULT, positionInitiale.Z + DISTANCE_POURSUITE),
                                               new Vector3(Position.X, Position.Y + 4, Position.Z), 
                                               Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Cam�ra);
            Game.Services.AddService(typeof(Cam�ra), Cam�ra);
        }

        public override void Initialize()
        {
            base.Initialize();
            RotationYawTour = Vector3.Zero;
            RotationPitchCanon = Vector3.Zero;
            Incr�mentAngleRotationTour = Incr�mentAngleRotation / 2f;
            �chelleTour = 0.0035f;
            �chelleCanon = 0.005f;
            �chelleRoues = 0.05f;
            D�placementSouris = new Vector2(0, 0);
            Nouvel�tatSouris = Mouse.GetState();
            Mouse.SetPosition(400, 200);
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
        //if (currentMouseState != previousMouseState)
        //{
        //    deltaX = currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
        //    deltaY = currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

        //    mouseRotationBuffer.X -= 0.1f * deltaX * dt;
        //    mouseRotationBuffer.Y -= 0.1f * deltaY * dt;

        //    if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
        //        mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));

        //    if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
        //        mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));

        //    Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)),
        //        MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

        //    deltaX = 0;
        //    deltaY = 0;
        //}

        //Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
        //previousMouseState = currentMouseState;
        protected override void GestionMouvements()
        {
            Nouvel�tatSouris = Mouse.GetState();
            Console.WriteLine(Nouvel�tatSouris.X);
            Console.WriteLine(Nouvel�tatSouris.Y);
            if (Nouvel�tatSouris.X > (Game.Window.ClientBounds.Width / 2) + 40)
            {
                D�placementSouris = new Vector2(-Incr�mentAngleRotation, D�placementSouris.Y);
            }
            if (Nouvel�tatSouris.X < (Game.Window.ClientBounds.Width / 2) - 40)
            {
                D�placementSouris = new Vector2(Incr�mentAngleRotation, D�placementSouris.Y);
            }
            if ((Nouvel�tatSouris.X < (Game.Window.ClientBounds.Width / 2) + 40 && Nouvel�tatSouris.X > (Game.Window.ClientBounds.Width / 2) - 40))
            {
                D�placementSouris = new Vector2(0,0);
            }
            //else
            //{
            //    D�placementSouris = new Vector2(Incr�mentAngleRotation, D�placementSouris.Y);
            //}
            //if (Nouvel�tatSouris.Y > Game.Window.ClientBounds.Height / 2)
            //{
            //    D�placementSouris = new Vector2(D�placementSouris.X, D�placementSouris.Y - Incr�mentAngleRotation);
            //}
            //else
            //{
            //    D�placementSouris = new Vector2(D�placementSouris.X, D�placementSouris.Y + Incr�mentAngleRotation);
            //}

            float activation = G�rerTouche(Keys.Left) - G�rerTouche(Keys.Right);

            RotationTour(activation);
            RotationCanon(activation);

            Cam�ra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Cam�ra.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X,
                                            Position.Y + HAUTEUR_CAM_D�FAULT,
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
                //Cam�ra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
                //Cam�ra.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_D�FAULT, ((float)Math.Cos(rotationFinal) * DISTANCE_POURSUITE) + Position.Z);
            }

            CalculerMonde();
        }

        private void RotationTour(float activation)
        {
            RotationYawTour = new Vector3(-MathHelper.PiOver2, RotationYawTour.Y + (Incr�mentAngleRotation * D�placementSouris.X), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(�chelleTour, RotationYawTour, PositionTour);
        }

        private void RotationCanon(float activation)
        {
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, RotationPitchCanon.Y + (Incr�mentAngleRotation * D�placementSouris.X), MathHelper.PiOver2);
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(�chelleCanon, RotationPitchCanon, PositionCanon);
        }

        bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;

            return estHorsDesBornes;
        }

        float G�rerTouche(Keys touche)
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
                    if (maille.Name == "Canon")
                    {
                        mondeLocal = MondeCanon;
                    }
                    else
                    {
                        //mondeLocal = MondeCanon;
                        mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                    }
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
