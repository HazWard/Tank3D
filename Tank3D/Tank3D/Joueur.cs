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
        Game Jeu { get; set; }
        MouseState NouvelÉtatSouris { get; set; }
        MouseState AncienÉtatSouris { get; set; }
        InputManager GestionInput { get; set; }
        CaméraSubjective Caméra { get; set; }
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
        float ÉchelleTour { get; set; }
        float ÉchelleCanon { get; set; }
        float ÉchelleRoues { get; set; }

        public Joueur(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            Jeu = jeu;
            Caméra = new CaméraSubjective(jeu, new Vector3(positionInitiale.X, positionInitiale.Y + HAUTEUR_CAM_DÉFAULT, positionInitiale.Z + DISTANCE_POURSUITE),
                                               new Vector3(Position.X, Position.Y + 4, Position.Z), 
                                               Vector3.Up, IntervalleMAJ);
            Game.Components.Add(Caméra);
            Game.Services.AddService(typeof(Caméra), Caméra);
        }

        public override void Initialize()
        {
            base.Initialize();
            RotationYawTour = new Vector3(-MathHelper.PiOver2, 0, MathHelper.PiOver2);
            RotationPitchCanon = new Vector3(-MathHelper.PiOver2, 0, MathHelper.PiOver2);
            ÉchelleTour = 0.0035f;
            ÉchelleCanon = 0.005f;
            ÉchelleRoues = 0.05f;
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
       
        protected override void GestionMouvements()
        {
            NouvelÉtatSouris = Mouse.GetState();
            GestionProjectile();
            RotationTour();
            RotationCanon();
            
            Caméra.Cible = new Vector3(Position.X, Position.Y + 4, Position.Z);
            Caméra.Position
                = new Vector3(((float)Math.Sin(RotationYawTour.Y) * DISTANCE_POURSUITE) + Position.X, Position.Y + HAUTEUR_CAM_DÉFAULT,
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
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            float posXFinal = Position.X - déplacementFinal.X;
            float posZFinal = Position.Z - déplacementFinal.Y;

            nouvellesCoords = TerrainJeu.ConvertionCoordonnées(new Vector3(posXFinal, 0,posZFinal));
            
            if (!EstHorsDesBornes(nouvellesCoords))
            {
                Position = new Vector3(posXFinal, HauteurTerrain + HAUTEUR_DÉFAULT, posZFinal);
                HauteurTerrain = TerrainJeu.GetHauteur(nouvellesCoords);
            }
            CalculerMonde();
        }

        void RotationTour()
        {
            GestionSouris();
            RotationYawTour = new Vector3(RotationYawTour.X, RotationYawTour.Y + 2 * (-0.00005f * DeltaRotationCanon.X), RotationYawTour.Z);
            PositionTour = new Vector3(Position.X, Position.Y + 0.3f, Position.Z);
            MondeTour = TransformationsMeshes(ÉchelleTour, RotationYawTour, PositionTour);
        }
        void RotationCanon()
        {
            GestionSouris();

            RotationPitchCanon = new Vector3(RotationPitchCanon.X + (-0.00005f * DeltaRotationCanon.Y),
                                             RotationPitchCanon.Y + 2 * (-0.00005f * DeltaRotationCanon.X), RotationPitchCanon.Z);
            if (RotationPitchCanon.X > -1.3 || RotationPitchCanon.X < -1.7)
            {
                RotationPitchCanon = new Vector3(RotationPitchCanon.X - (-0.00005f * DeltaRotationCanon.Y),
                                                 RotationPitchCanon.Y, RotationPitchCanon.Z);
            }
            PositionCanon = new Vector3(Position.X, Position.Y - 1f, Position.Z);
            MondeCanon = TransformationsMeshes(ÉchelleCanon, RotationPitchCanon, PositionCanon);
            DeltaRotationCanon = new Vector2(0,0);
        }

        void GestionSouris()
        {
            if (NouvelÉtatSouris != AncienÉtatSouris)
            {
                DeltaRotationCanon = new Vector2(NouvelÉtatSouris.X - (Game.GraphicsDevice.Viewport.Width / 2), NouvelÉtatSouris.Y - (Game.GraphicsDevice.Viewport.Height / 2));
            }
            if ((NouvelÉtatSouris.X < (Game.Window.ClientBounds.Width / 2) + 20 && NouvelÉtatSouris.X > (Game.Window.ClientBounds.Width / 2) - 20))
            {
                DeltaRotationCanon = new Vector2(0, DeltaRotationCanon.Y);
            }
            if ((NouvelÉtatSouris.Y < (Game.Window.ClientBounds.Height / 2) + 20 && NouvelÉtatSouris.Y > (Game.Window.ClientBounds.Height / 2) - 20))
            {
                DeltaRotationCanon = new Vector2(DeltaRotationCanon.X, 0);
            }
        }

        void GestionProjectile()
        {
            if (GestionInput.EstAncienClicGauche())
            {
                Console.WriteLine("Tir");
                ProjectileTank = new Projectile(Jeu, "Projectile", 0.1f, new Vector3(0, RotationPitchCanon.Y, RotationPitchCanon.Z), PositionCanon, IntervalleMAJ);
                Game.Components.Add(ProjectileTank);
            }
        }

        float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }

        Matrix TransformationsMeshes(float échelle, Vector3 rotation,Vector3 position)
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
