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
        public Vector3 PositionJoueur { get; set; }
        public bool EstTouché { get;set; }


        Rectangle RectangleSource { get; set; }
        public float PourcentageVie { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float LargeurInitiale { get; set; }
        float LargeureImage { get; set; }
        
        Matrix Rot { get; set; }
        int LigneSheet { get; set; }
        int DeltaX { get; set; }
        int DeltaY { get; set; }
        float DeltaPointX { get; set; }
        Rectangle RectangleSourceVie { get; set; }
        int NbImages { get; set; }
        SpriteBatch GestionnaireDeSprite { get; set; }


        //ModèleMobile Cible { get; set; }
        //public Vector3 Position { get; set; }
        public BarreDeVie(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTexture, float intervalleMAJ, int nbImages)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente,nomTexture, intervalleMAJ)
        {
            NbImages = nbImages;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LargeurInitiale = Étendue.X;
            //LigneSheet = 1;
            base.Initialize();
            
        }
        
        public void ModifierActivation()
        { 
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            DeltaX = (int)(Texture.Width / NbImages);
            DeltaY = (int)(Texture.Height);
            //DeltaPointX = DeltaX / Texture.Width;
            RectangleSourceVie = new Rectangle(0, 0, DeltaX, DeltaY);
            GestionnaireDeSprite = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch; 
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
                EstTouché = true;
                if (EstTouché)
                { 
                    CalculerVie();
                }
                
                CalculerNormales();
                //CréerTableauPointsTexture();
                //CréerTableauPoints();
                CalculerMatriceMonde();
              
                    
                
                
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }



//        The actual math involved, as long as O is not linearly dependent with U, is simple.
//1) Create the vector D = (O-P).
//2) Create the vector U cross D, and normalize. Call this "Right"
//3) Create the vector Right cross U and normalize. Call this "Backwards"
//4) Create the vector Backwards cross Right. Call this "Up"
//4) Your matrix is now these three vectors written in rows (assuming row vectors on the right):
//// O is your object's position
//// P is the position of the object to face
//// U is the nominal "up" vector (typically Vector3.Y)
//// Note: this does not work when O is straight below or straight above P

        void CalculerNormales()
        {
            Vector3 U = Vector3.Up;
            Vector3 D = Position - PositionJoueur;
            Vector3 Right = Vector3.Cross(U, D);
            Vector3.Normalize(ref Right, out Right);
            Vector3 Backwards = Vector3.Cross(Right, U);
            Vector3 Up = Vector3.Cross(Backwards, Right);
            Rot = new Matrix(Right.X, Right.Y, Right.Z, 0, Up.X, Up.Y, Up.Z, 0, Backwards.X, Backwards.Y, Backwards.Z, 0, 0, 0, 0, 1);
        }

        protected override void CalculerMatriceMonde()
        {
            Monde = Matrix.Identity *
                    Matrix.CreateScale(Homothétie) *
                    Rot *
                    Matrix.CreateTranslation(Position);
        }
        public void CalculerVie()
        {
            if (LigneSheet < NbImages - 1)
            { 
                LigneSheet = LigneSheet + 1;    
            }
            RectangleSource = new Rectangle((RectangleSource.X + (DeltaX * 0)) % Texture.Width, 0, DeltaX, DeltaY);
            
            //LargeureImage = PourcentageVie * Étendue.X;
        }
        //private void CréerTableauPointsTexture()
        //{
        //    PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
        //    //float DeltaX = 1f / NbColonnes;
        //    //float DeltaY = 1f / NbRangées;
        //    int i = LigneSheet * DeltaX;
        //    PtsTexture[0, 0] = new Vector2(0, 0);
        //    PtsTexture[1, 0] = new Vector2(0 + DeltaPointX, 0);
        //    PtsTexture[0, 1] = new Vector2(0, 1);
        //    PtsTexture[1, 1] = new Vector2(0 + DeltaPointX, 1);

        //    //for (int i = 0; i <= NbColonnes; ++i)
        //    //{
        //    //    for (int j = 0; j <= NbRangées; ++j)
        //    //    {
        //    //        PtsTexture[i, j] = new Vector2(i * DeltaX, (NbRangées - j) * DeltaY);
        //    //    }
        //    //}
        //}
        //private void CréerTableauPoints()
        //{
        //    for (int i = 0; i < PtsSommets.GetLength(0); ++i)
        //    {
        //        for (int j = 0; j < PtsSommets.GetLength(1); ++j)
        //        {
        //            PtsSommets[i, j] = new Vector3(0 + i * DeltaX, 0 + j * DeltaY, Origine.Z);
        //        }
        //    }

        //}
        public override void Draw(GameTime gameTime)
        {
            RasterizerState JeuRasterizerState = new RasterizerState();
            RasterizerState ancienRasterizerState = EffetDeBase.GraphicsDevice.RasterizerState;
            JeuRasterizerState.CullMode = CullMode.None;
            JeuRasterizerState.FillMode = ancienRasterizerState.FillMode;
            EffetDeBase.GraphicsDevice.RasterizerState = JeuRasterizerState;
            //GestionnaireDeSprite.Begin();
            //GestionnaireDeSprite.Draw(Texture, new Rectangle((int)Position.X,(int)Position.Y, Texture.Width,Texture.Height), RectangleSource, Color.White, Rot, Origine, SpriteEffects.None, 1f);
            //GestionnaireDeSprite.End();
            base.Draw(gameTime);
            EffetDeBase.GraphicsDevice.RasterizerState = ancienRasterizerState;
        }
    }
}
