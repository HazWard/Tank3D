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
        InputManager GestionInput { get; set; }
        CaméraSubjective Caméra { get; set; }
        Vector3 RotationYawTour { get; set; }
        Vector3 RotationPitchCanon { get; set; }
        //Vector3 ÉchelleTour { get; set; }
        //Vector3 ÉchelleCanon { get; set; }
        //Vector3 ÉchelleRoues { get; set; }
        Vector3 PositionCanon { get; set; }
        Vector3 PositionTour { get; set; }
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
            //ÉchelleTour = new Vector3(0.19f, 0.19f, 0.19f);
            //ÉchelleCanon = new Vector3(0.89f, 0.89f, 3.15f);
            //ÉchelleRoues = new Vector3(1.85f, 1.85f, 1.53f);
            ÉchelleTour = 0.0035f;
            ÉchelleCanon = 0.005f;
            ÉchelleRoues = 0.05f;
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

        protected override void GestionMouvements()
        {
            RotationTour();
            RotationCanon();
            
            int déplacement = GérerTouche(Keys.W) - GérerTouche(Keys.S);
            int rotation = GérerTouche(Keys.D) - GérerTouche(Keys.A);
            if (déplacement != 0 || rotation != 0)
            {
                ModificationParamètres(déplacement, rotation);
            }
        }

        void ModificationParamètres(int déplacement, int rotation)
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
                Caméra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
                Caméra.Position = new Vector3(((float)Math.Sin(rotationFinal) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_DÉFAULT, ((float)Math.Cos(rotationFinal) * DISTANCE_POURSUITE) + Position.Z);
            }

            CalculerMonde();
        }

        private void RotationTour()
        {
            int activation = GérerTouche(Keys.Left) - GérerTouche(Keys.Right);

            RotationYawTour = new Vector3(-MathHelper.PiOver2, RotationYawTour.Y + (IncrémentAngleRotationTour * activation), MathHelper.PiOver2);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(ÉchelleTour, RotationYawTour, PositionTour);
        }

        private void RotationCanon()
        {
            // À modifier pour modifier la souris
            
            int activation = GérerTouche(Keys.Left) - GérerTouche(Keys.Right);

            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, RotationPitchCanon.Y + (IncrémentAngleRotationTour * activation), MathHelper.PiOver2);
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(ÉchelleCanon, RotationPitchCanon, PositionCanon);
        }

        bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;

            return estHorsDesBornes;
        }

        int GérerTouche(Keys touche)
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
