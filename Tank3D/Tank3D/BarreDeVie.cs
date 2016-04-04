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

    public class BarreDeVie : PlanTextur�
    {
        //float PourcentageVie { get; set; }
        //Joueur Joueur { get; set; }
        Rectangle RectangleSource { get; set; }
        public float PourcentageVie { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        float LargeurInitiale { get; set; }
        float LargeureImage { get; set; }
        Vector3 RotationImage { get; set; }
        Vector3 PointUn { get; set; }
        Vector3 PointDeux { get; set; }
        Vector3 VecteurUn { get; set; }
        Vector3 VecteurDeux { get; set; }
        Vector3 NormaleBarreDeVie { get; set; }
        Vector3 VecteurBarreDeVieCam�ra { get; set; }
        public Vector3 PositionJoueur { get; set; }
        


        //Mod�leMobile Cible { get; set; }
        //public Vector3 Position { get; set; }
        public BarreDeVie(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 �tendue, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, homoth�tieInitiale, rotationInitiale, positionInitiale, �tendue, charpente,nomTexture, intervalleMAJ)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LargeurInitiale = �tendue.X;
            base.Initialize();
            
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
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ > IntervalleMAJ)
            {
                CalculerVie();
                CalculerMatriceMonde();
                //G�rerAnimation();
                //G�rerD�placement()
                //CalculDeDommages();
                CalculerNormale();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        void CalculerNormale()
        {
            PointUn = new Vector3(Position.X - 1, Position.Y, Position.Z);
            PointDeux = new Vector3(Position.X, Position.Y +1, Position.Z);
            VecteurUn = PointUn - Position;
            VecteurDeux = PointDeux - Position;

            NormaleBarreDeVie = Vector3.Cross(VecteurUn, VecteurDeux);
            NormaleBarreDeVie = Vector3.Normalize(NormaleBarreDeVie);
            VecteurBarreDeVieCam�ra = PositionJoueur - Position;
            VecteurBarreDeVieCam�ra = Vector3.Normalize(VecteurBarreDeVieCam�ra);

            float angleEntreBarreCam�ra = (float)Math.Acos(Vector3.Dot(NormaleBarreDeVie, VecteurBarreDeVieCam�ra));
        }
        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Homoth�tie) *
                    Matrix.CreateFromYawPitchRoll(AngleLacet, AngleTangage, AngleRoulis) *
                    Matrix.CreateTranslation(Position);
        }
        public void CalculerVie()
        {
            LargeureImage = PourcentageVie * �tendue.X;
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
        //    LargeureImage = PourcentageVie * �tendue.X;
        //    RotationImage = new Vector3(rotationImageX.X, rotationImageY.Y, 0);
        //}
        //private void G�rerAnimation()
        //{
        //    RectangleSource = new Rectangle(0, 0, RectangleSource.Width - DELTA_X, RectangleSource.Height);

        //    PositionImage1 = new Vector2(((PositionImage1.X + �chelle.X) % ZoneAffichage.Width), PositionImage1.Y);
        //    PositionImage2 = new Vector2(PositionImage1.X - ZoneAffichage.Width, PositionImage1.Y);

        //    if (PositionImage1.X == 0)
        //    {
        //        RectangleSource = new Rectangle(0, 0, ImageDeFond.Width, ImageDeFond.Height);
        //    }
        //}

    }
}
