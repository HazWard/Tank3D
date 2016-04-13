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

    public class BarreDeVie : PlanTexturé, IActivable
    {
        //float PourcentageVie { get; set; }
        //Joueur Joueur { get; set; }
        Rectangle RectangleSource { get; set; }
        public float PourcentageVie { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float LargeurInitiale { get; set; }
        float LargeureImage { get; set; }
        float AngleEntreBarreCaméraPremier { get; set; }
        float AngleEntreBarreCaméraDeuxième { get; set; }
        float ProduitScalaire { get; set; }
        Vector3 RotationImage { get; set; }
        Vector3 PointUn { get; set; }
        Vector3 PointDeux { get; set; }
        Vector3 VecteurUn { get; set; }
        Vector3 VecteurDeux { get; set; }
        Vector3 NormaleBarreDeVie { get; set; }
        Vector3 VecteurBarreDeVieCaméra { get; set; }
        public Vector3 PositionJoueur { get; set; }
        


        //ModèleMobile Cible { get; set; }
        //public Vector3 Position { get; set; }
        public BarreDeVie(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente,nomTexture, intervalleMAJ)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LargeurInitiale = Étendue.X;
            base.Initialize();
            
        }
        public void ModifierActivation()
        { 
        }
        protected override void LoadContent()
        {
            
            
            //RectangleSource = new Rectangle(0, 0, ImageDeFond.Width, ImageDeFond.Height);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ > IntervalleMAJ)
            {
                CalculerVie();
                
                //GérerAnimation();
                //GérerDéplacement()
                //CalculDeDommages();
                CalculerNormale();
                
                AjusterNormales();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        void AjusterNormales()
        {
            PointUn = new Vector3(Position.X - 1, Position.Y, Position.Z);
            PointDeux = new Vector3(Position.X, Position.Y + 1, Position.Z);
            VecteurUn = PointUn - Position;
            VecteurDeux = PointDeux - Position;

            NormaleBarreDeVie = Vector3.Cross(VecteurUn, VecteurDeux);
            NormaleBarreDeVie = Vector3.Normalize(NormaleBarreDeVie);
            VecteurBarreDeVieCaméra = new Vector3(PositionJoueur.X, Position.Y, PositionJoueur.Z) - new Vector3(Position.X, PositionJoueur.Y, Position.Z);
            VecteurBarreDeVieCaméra = Vector3.Normalize(VecteurBarreDeVieCaméra);

            //Console.WriteLine(Vector3.Dot(NormaleBarreDeVie, VecteurBarreDeVieCaméra));
            ProduitScalaire = Vector3.Dot(NormaleBarreDeVie, VecteurBarreDeVieCaméra);
            AngleEntreBarreCaméraDeuxième = (float)Math.Acos(ProduitScalaire);
            if (AngleEntreBarreCaméraDeuxième > AngleEntreBarreCaméraPremier)
            {
                AngleLacet = AngleLacet - AngleEntreBarreCaméraDeuxième + MathHelper.PiOver2;
                CalculerMatriceMonde();
            }

        }
        void CalculerNormale()
        {
            PointUn = new Vector3(Position.X - 1, Position.Y, Position.Z);
            PointDeux = new Vector3(Position.X, Position.Y +1, Position.Z);
            VecteurUn = PointUn - Position;
            VecteurDeux = PointDeux - Position;

            NormaleBarreDeVie = Vector3.Cross(VecteurUn, VecteurDeux);
            NormaleBarreDeVie = Vector3.Normalize(NormaleBarreDeVie);
            VecteurBarreDeVieCaméra = new Vector3(PositionJoueur.X, Position.Y,PositionJoueur.Z) - new Vector3(Position.X,PositionJoueur.Y,Position.Z);
            VecteurBarreDeVieCaméra = Vector3.Normalize(VecteurBarreDeVieCaméra);

            //Console.WriteLine(Vector3.Dot(NormaleBarreDeVie, VecteurBarreDeVieCaméra));
            ProduitScalaire = Vector3.Dot(NormaleBarreDeVie, VecteurBarreDeVieCaméra);
            AngleEntreBarreCaméraPremier = (float)Math.Acos(ProduitScalaire);


            //Console.WriteLine("ANGLE : {0}", MathHelper.ToDegrees(AngleEntreBarreCaméraPremier));
            AjusterAngleBarreDeVie(AngleEntreBarreCaméraPremier, ProduitScalaire);

        }
        void AjusterAngleBarreDeVie(float angle, float produitScalaire)
        {
            if (produitScalaire >= 0)
            {
                AngleLacet = AngleLacet + angle + MathHelper.PiOver2;
            }
            else
            {
                AngleLacet = AngleLacet - angle - MathHelper.PiOver2;
            }
               
            CalculerMatriceMonde();
        }
        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Homothétie) *
                    Matrix.CreateFromYawPitchRoll(AngleLacet, AngleTangage, AngleRoulis) *
                    Matrix.CreateTranslation(Position);
        }
        public void CalculerVie()
        {
            LargeureImage = PourcentageVie * Étendue.X;
            //RotationImage = new Vector3(rotationImageX.X, rotationImageY.Y, 0);
        }
        public override void Draw(GameTime gameTime)
        {
            RasterizerState JeuRasterizerState = new RasterizerState();
            RasterizerState ancienRasterizerState = EffetDeBase.GraphicsDevice.RasterizerState;
            JeuRasterizerState.CullMode = CullMode.None;
            JeuRasterizerState.FillMode = ancienRasterizerState.FillMode;
            EffetDeBase.GraphicsDevice.RasterizerState = JeuRasterizerState;
            base.Draw(gameTime);
            EffetDeBase.GraphicsDevice.RasterizerState = ancienRasterizerState;
        }
        //public void CalculerVie(Vector3 rotationImageX,Vector3 rotationImageY, Vector3 positionImage)
        //{
        //    LargeureImage = PourcentageVie * Étendue.X;
        //    RotationImage = new Vector3(rotationImageX.X, rotationImageY.Y, 0);
        //}
        //private void GérerAnimation()
        //{
        //    RectangleSource = new Rectangle(0, 0, RectangleSource.Width - DELTA_X, RectangleSource.Height);

        //    PositionImage1 = new Vector2(((PositionImage1.X + Échelle.X) % ZoneAffichage.Width), PositionImage1.Y);
        //    PositionImage2 = new Vector2(PositionImage1.X - ZoneAffichage.Width, PositionImage1.Y);

        //    if (PositionImage1.X == 0)
        //    {
        //        RectangleSource = new Rectangle(0, 0, ImageDeFond.Width, ImageDeFond.Height);
        //    }
        //}

    }
}
