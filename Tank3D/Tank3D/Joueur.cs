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
    public class Joueur : ModèleMobile
    {
        // Constantes
        protected const float DISTANCE_POURSUITE = 20f;
        protected const float HAUTEUR_CAM_DÉFAULT = 10f;
        
        // Propriétés
        MouseState NouvelÉtatSouris { get; set; }
        MouseState AncienÉtatSouris { get; set; }
        InputManager GestionInput { get; set; }
        CaméraSubjective Caméra { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
        Vector2 AnciennePositionSouris { get; set; }
        Vector2 DéplacementSouris { get; set; }
        Matrix MondeTour { get; set; }
        Matrix MondeCanon { get; set; }
        Matrix MondeRoues { get; set; }
        float ÉchelleTour { get; set; }
        float ÉchelleCanon { get; set; }
        float ÉchelleRoues { get; set; }
        float IncrémentAngleRotationTour { get; set; }

        public Joueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Caméra = new CaméraSubjective(jeu, new Vector3(positionInitiale.X, positionInitiale.Y + HAUTEUR_CAM_DÉFAULT, positionInitiale.Z + DISTANCE_POURSUITE),
                                               new Vector3(Position.X, Position.Y + 4, Position.Z), 
                                               Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Caméra);
            Game.Services.AddService(typeof(Caméra), Caméra);
        }

        public override void Initialize()
        {
            base.Initialize();
            RotationYawTour = Vector3.Zero;
            RotationPitchCanon = Vector3.Zero;
            IncrémentAngleRotationTour = IncrémentAngleRotation / 2f;
            ÉchelleTour = 0.0035f;
            ÉchelleCanon = 0.005f;
            ÉchelleRoues = 0.05f;
            DéplacementSouris = new Vector2(0, 0);
            NouvelÉtatSouris = Mouse.GetState();
            Mouse.SetPosition(400, 200);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        public Vector2 Coordonnées
        {
            get
            {
                return new Vector2(Position.X, Position.Z);
            }
        }

        #region Méthodes pour la gestion des déplacements et rotations du modèle
        void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
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
            NouvelÉtatSouris = Mouse.GetState();
            Console.WriteLine(NouvelÉtatSouris.X);
            Console.WriteLine(NouvelÉtatSouris.Y);
            if (NouvelÉtatSouris.X > (Game.Window.ClientBounds.Width / 2) + 40)
            {
                DéplacementSouris = new Vector2(-IncrémentAngleRotation, DéplacementSouris.Y);
            }
            if (NouvelÉtatSouris.X < (Game.Window.ClientBounds.Width / 2) - 40)
            {
                DéplacementSouris = new Vector2(IncrémentAngleRotation, DéplacementSouris.Y);
            }
            if ((NouvelÉtatSouris.X < (Game.Window.ClientBounds.Width / 2) + 40 && NouvelÉtatSouris.X > (Game.Window.ClientBounds.Width / 2) - 40))
            {
                DéplacementSouris = new Vector2(0,0);
            }
            //else
            //{
            //    DéplacementSouris = new Vector2(IncrémentAngleRotation, DéplacementSouris.Y);
            //}
            //if (NouvelÉtatSouris.Y > Game.Window.ClientBounds.Height / 2)
            //{
            //    DéplacementSouris = new Vector2(DéplacementSouris.X, DéplacementSouris.Y - IncrémentAngleRotation);
            //}
            //else
            //{
            //    DéplacementSouris = new Vector2(DéplacementSouris.X, DéplacementSouris.Y + IncrémentAngleRotation);
            //}

            float activation = GérerTouche(Keys.Left) - GérerTouche(Keys.Right);

            RotationTour(activation);
            RotationCanon(activation);

            Caméra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Caméra.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X,
                                            Position.Y + HAUTEUR_CAM_DÉFAULT,
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
            float rotationFinal = Rotation.Y - IncrémentAngleRotation * rotation;
            float posX = déplacement * (float)Math.Sin(rotationFinal);
            float posY = déplacement * (float)Math.Cos(rotationFinal);

            Vector2 déplacementFinal = new Vector2(posX, posY);
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            Position = new Vector3(Position.X - déplacementFinal.X, Position.Y, Position.Z - déplacementFinal.Y);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            Point nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0,posZFinal));
            
            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
                HauteurTerrain = TerrainJeu.GetHauteur(Position);
                //Caméra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
                //Caméra.Position = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_DÉFAULT, ((float)Math.Cos(rotationFinal) * DISTANCE_POURSUITE) + Position.Z);
            }

            CalculerMonde();
        }

        private void RotationTour(float activation)
        {
            RotationYawTour = new Vector3(-MathHelper.PiOver2, RotationYawTour.Y + (IncrémentAngleRotation * DéplacementSouris.X), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(ÉchelleTour, RotationYawTour, PositionTour);
        }

        private void RotationCanon(float activation)
        {
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, RotationPitchCanon.Y + (IncrémentAngleRotation * DéplacementSouris.X), MathHelper.PiOver2);
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(ÉchelleCanon, RotationPitchCanon, PositionCanon);
        }

        bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;

            return estHorsDesBornes;
        }

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }

        private Matrix TransformationsMeshes(float échelle, Vector3 rotation,Vector3 position)
        {   
            Matrix monde = Matrix.Identity;
            monde *= Matrix.CreateScale(échelle);
            monde *= Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            monde *= Matrix.CreateTranslation(position);

            return monde;
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh maille in Modèle.Meshes)
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
                        mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                    }
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
